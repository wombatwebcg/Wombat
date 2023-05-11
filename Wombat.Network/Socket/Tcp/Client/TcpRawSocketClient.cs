﻿using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Wombat.Core;
using Wombat.Network.Buffer;

namespace Wombat.Network.Socket.Tcp
{
    public class TcpRawSocketClient
    {
        #region Fields

        private static Logger _logger;
        private TcpClient _tcpClient;

        private readonly TcpSocketClientConfiguration _configuration;
        private readonly IPEndPoint _remoteEndPoint;
        private readonly IPEndPoint _localEndPoint;
        private Stream _stream;
        private ArraySegment<byte> _receiveBuffer = default(ArraySegment<byte>);
        private int _receiveBufferOffset = 0;

        private int _state;
        private const int _none = 0;
        private const int _connecting = 1;
        private const int _connected = 2;
        private const int _closeing = 5;
        private const int _closed = 6;
        private bool _isDispatcherModel = false;
        #endregion

        #region Constructors

        public TcpRawSocketClient(IPAddress remoteAddress, int remotePort, IPAddress localAddress, int localPort, TcpSocketClientConfiguration configuration = null)
                : this(new IPEndPoint(remoteAddress, remotePort), new IPEndPoint(localAddress, localPort), configuration)
        {
        }

        public TcpRawSocketClient(IPAddress remoteAddress, int remotePort, IPEndPoint localEP, TcpSocketClientConfiguration configuration = null)
            : this(new IPEndPoint(remoteAddress, remotePort), localEP, configuration)
        {
        }

        public TcpRawSocketClient(IPAddress remoteAddress, int remotePort, TcpSocketClientConfiguration configuration = null)
            : this(new IPEndPoint(remoteAddress, remotePort), configuration)
        {
        }

        public TcpRawSocketClient(IPEndPoint remoteEP, TcpSocketClientConfiguration configuration = null)
            : this(remoteEP, null, configuration)
        {

        }

        public TcpRawSocketClient(IPEndPoint remoteEP, IPEndPoint localEP, TcpSocketClientConfiguration configuration = null)
        {
            if (remoteEP == null)
                throw new ArgumentNullException("remoteEP");

            _remoteEndPoint = remoteEP;
            _localEndPoint = localEP;
            _configuration = configuration ?? new TcpSocketClientConfiguration();

            if (_configuration.BufferManager == null)
                throw new InvalidProgramException("The buffer manager in configuration cannot be null.");
        }

        //public TcpRawSocketClient(IPAddress remoteAddress, int remotePort, IPAddress localAddress, int localPort,
        //    Func<TcpRawSocketClient, byte[], int, int, Task> onServerDataReceived = null,
        //    Func<TcpRawSocketClient, Task> onServerConnected = null,
        //    Func<TcpRawSocketClient, Task> onServerDisconnected = null,
        //    TcpSocketClientConfiguration configuration = null)
        //    : this(new IPEndPoint(remoteAddress, remotePort), new IPEndPoint(localAddress, localPort),
        //          onServerDataReceived, onServerConnected, onServerDisconnected, configuration)
        //{
        //}

        //public TcpRawSocketClient(IPAddress remoteAddress, int remotePort, IPEndPoint localEP,
        //    Func<TcpRawSocketClient, byte[], int, int, Task> onServerDataReceived = null,
        //    Func<TcpRawSocketClient, Task> onServerConnected = null,
        //    Func<TcpRawSocketClient, Task> onServerDisconnected = null,
        //    TcpSocketClientConfiguration configuration = null)
        //    : this(new IPEndPoint(remoteAddress, remotePort), localEP,
        //          onServerDataReceived, onServerConnected, onServerDisconnected, configuration)
        //{
        //}

        //public TcpRawSocketClient(IPAddress remoteAddress, int remotePort,
        //    Func<TcpRawSocketClient, byte[], int, int, Task> onServerDataReceived = null,
        //    Func<TcpRawSocketClient, Task> onServerConnected = null,
        //    Func<TcpRawSocketClient, Task> onServerDisconnected = null,
        //    TcpSocketClientConfiguration configuration = null)
        //    : this(new IPEndPoint(remoteAddress, remotePort),
        //          onServerDataReceived, onServerConnected, onServerDisconnected, configuration)
        //{
        //}

        //public TcpRawSocketClient(IPEndPoint remoteEP,
        //    Func<TcpRawSocketClient, byte[], int, int, Task> onServerDataReceived = null,
        //    Func<TcpRawSocketClient, Task> onServerConnected = null,
        //    Func<TcpRawSocketClient, Task> onServerDisconnected = null,
        //    TcpSocketClientConfiguration configuration = null)
        //    : this(remoteEP, null,
        //          onServerDataReceived, onServerConnected, onServerDisconnected, configuration)
        //{
        //}

        //public TcpRawSocketClient(IPEndPoint remoteEP, IPEndPoint localEP,
        //    Func<TcpRawSocketClient, byte[], int, int, Task> onServerDataReceived = null,
        //    Func<TcpRawSocketClient, Task> onServerConnected = null,
        //    Func<TcpRawSocketClient, Task> onServerDisconnected = null,
        //    TcpSocketClientConfiguration configuration = null)
        //    : this(remoteEP, localEP,
        //         new DefaultTcpRawSocketClientEventDispatcher(onServerDataReceived, onServerConnected, onServerDisconnected),
        //         configuration)
        //{
        //}



        public static TcpRawSocketClient UsgLogger(TcpRawSocketClient client, Logger log)
        {
            _logger = log;
            return client;
        }


        #endregion

        #region Properties

        public TimeSpan ConnectTimeout { get { return _configuration.ConnectTimeout; } }

        private bool Connected { get { return _tcpClient != null && _tcpClient.Client.Connected; } }
        public IPEndPoint RemoteEndPoint { get { return Connected ? (IPEndPoint)_tcpClient.Client.RemoteEndPoint : _remoteEndPoint; } }
        public IPEndPoint LocalEndPoint { get { return Connected ? (IPEndPoint)_tcpClient.Client.LocalEndPoint : _localEndPoint; } }

        public SocketConnectionState State
        {
            get
            {
                switch (_state)
                {
                    case _none:
                        return SocketConnectionState.None;
                    case _connecting:
                        return SocketConnectionState.Connecting;
                    case _connected:
                        return SocketConnectionState.Connected;
                    case _closed:
                        return SocketConnectionState.Closed;
                    case _closeing:
                        return SocketConnectionState.Closeing;

                    default:
                        return SocketConnectionState.Closed;
                }
            }
        }

        public override string ToString()
        {
            return string.Format("RemoteEndPoint[{0}], LocalEndPoint[{1}]",
                this.RemoteEndPoint, this.LocalEndPoint);
        }

        #endregion

        #region Connect

        public async Task ConnectAsync()
        {
            int origin = Interlocked.Exchange(ref _state, _connecting);
            if (!(origin == _none || origin == _closed))
            {
                await CloseAsync(false); // connecting with wrong state
                throw new InvalidOperationException("This tcp socket client is in invalid state when connecting.");
            }

            Clean(); // force to clean

            try
            {
                _tcpClient = _localEndPoint != null ?
                    new TcpClient(_localEndPoint) :
                    new TcpClient(_remoteEndPoint.Address.AddressFamily);
                SetSocketOptions();

                var awaiter = _tcpClient.ConnectAsync(_remoteEndPoint.Address, _remoteEndPoint.Port);
                if (!awaiter.Wait(ConnectTimeout))
                {
                    await CloseAsync(false); // connect timeout
                    throw new TimeoutException(string.Format(
                        "Connect to [{0}] timeout [{1}].", _remoteEndPoint, ConnectTimeout));
                }

                var negotiator = NegotiateStream(_tcpClient.GetStream());
                if (!negotiator.Wait(ConnectTimeout))
                {
                    await CloseAsync(false); // ssl negotiation timeout
                    throw new TimeoutException(string.Format(
                        "Negotiate SSL/TSL with remote [{0}] timeout [{1}].", _remoteEndPoint, ConnectTimeout));
                }
                _stream = negotiator.Result;

                if (_receiveBuffer == default(ArraySegment<byte>))
                    _receiveBuffer = _configuration.BufferManager.BorrowBuffer();
                _receiveBufferOffset = 0;

                if (Interlocked.CompareExchange(ref _state, _connected, _connecting) != _connecting)
                {
                    await CloseAsync(false); // connected with wrong state
                    throw new InvalidOperationException("This tcp socket client is in invalid state when connected.");
                }

            }
            catch (Exception ex) // catch exceptions then log then re-throw
            {
                _logger?.Exception(ex.Message, ex);
                await CloseAsync(true); // handle tcp connection error occurred
                throw;
            }
        }

        public void Connect() => ConnectAsync().Wait();

        private void SetSocketOptions()
        {
            _tcpClient.ReceiveBufferSize = _configuration.ReceiveBufferSize;
            _tcpClient.SendBufferSize = _configuration.SendBufferSize;
            _tcpClient.ReceiveTimeout = (int)_configuration.ReceiveTimeout.TotalMilliseconds;
            _tcpClient.SendTimeout = (int)_configuration.SendTimeout.TotalMilliseconds;
            _tcpClient.NoDelay = _configuration.NoDelay;
            _tcpClient.LingerState = _configuration.LingerState;

            if (_configuration.KeepAlive)
            {
                _tcpClient.Client.SetSocketOption(
                    SocketOptionLevel.Socket,
                    SocketOptionName.KeepAlive,
                    (int)_configuration.KeepAliveInterval.TotalMilliseconds);
            }

            _tcpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, _configuration.ReuseAddress);
        }

        private async Task<Stream> NegotiateStream(Stream stream)
        {
            if (!_configuration.SslEnabled)
                return stream;

            var validateRemoteCertificate = new RemoteCertificateValidationCallback(
                (object sender,
                X509Certificate certificate,
                X509Chain chain,
                SslPolicyErrors sslPolicyErrors)
                =>
                {
                    if (sslPolicyErrors == SslPolicyErrors.None)
                        return true;

                    if (_configuration.SslPolicyErrorsBypassed)
                        return true;
                    else
                        _logger?.Error($"Error occurred when validating remote certificate: [{ this.RemoteEndPoint}], [{sslPolicyErrors}]");
                    return false;
                });

            var sslStream = new SslStream(
                stream,
                false,
                validateRemoteCertificate,
                null,
                _configuration.SslEncryptionPolicy);

            if (_configuration.SslClientCertificates == null || _configuration.SslClientCertificates.Count == 0)
            {
                await sslStream.AuthenticateAsClientAsync( // No client certificates are used in the authentication. The certificate revocation list is not checked during authentication.
                    _configuration.SslTargetHost); // The name of the server that will share this SslStream. The value specified for targetHost must match the name on the server's certificate.
            }
            else
            {
                await sslStream.AuthenticateAsClientAsync(
                    _configuration.SslTargetHost, // The name of the server that will share this SslStream. The value specified for targetHost must match the name on the server's certificate.
                    _configuration.SslClientCertificates, // The X509CertificateCollection that contains client certificates.
                    _configuration.SslEnabledProtocols, // The SslProtocols value that represents the protocol used for authentication.
                    _configuration.SslCheckCertificateRevocation); // A Boolean value that specifies whether the certificate revocation list is checked during authentication.
            }

            // When authentication succeeds, you must check the IsEncrypted and IsSigned properties 
            // to determine what security services are used by the SslStream. 
            // Check the IsMutuallyAuthenticated property to determine whether mutual authentication occurred.
            _logger?.Debug(
                $"Ssl Stream: SslProtocol[{sslStream.SslProtocol}]," +
                $" IsServer[{sslStream.IsServer}], " +
                $"IsAuthenticated[{sslStream.IsAuthenticated}], " +
                $"IsEncrypted[{sslStream.IsEncrypted}], " +
                $"IsSigned[{ sslStream.IsSigned}], " +
                $"IsMutuallyAuthenticated[{sslStream.IsMutuallyAuthenticated}]," +
                $"HashAlgorithm[{sslStream.HashAlgorithm}]," +
                $" HashStrength[{sslStream.HashStrength}], " +
                $"KeyExchangeAlgorithm[{sslStream.KeyExchangeAlgorithm}]," +
                $" KeyExchangeStrength[{sslStream.KeyExchangeStrength}]," +
                $" CipherAlgorithm[{sslStream.CipherAlgorithm}], " +
                $"CipherStrength[{sslStream.CipherStrength}].");

            return sslStream;
        }

        #endregion

        #region Close

        public async Task CloseAsync()
        {
            await CloseAsync(true); // close by external
        }

        private async Task CloseAsync(bool shallNotifyUserSide)
        {

            if (Interlocked.Exchange(ref _state, _connected) == _closed)
            {
                return;
            }
            await Task.Run(() => { Shutdown(); });
            if (shallNotifyUserSide)
            {
                _logger?.Debug($"Disconnected from server [{this.RemoteEndPoint}] " +
                    $"on [{DateTime.UtcNow.ToString(@"yyyy-MM-dd HH:mm:ss.fffffff")}].");

            }

            Clean();
        }


        public void Close()
        {
            CloseAsync(true).Wait(); // close by external
        }



        public void Shutdown()
        {
            // The correct way to shut down the connection (especially if you are in a full-duplex conversation) 
            // is to call socket.Shutdown(SocketShutdown.Send) and give the remote party some time to close 
            // their send channel. This ensures that you receive any pending data instead of slamming the 
            // connection shut. ObjectDisposedException should never be part of the normal application flow.
            if (_tcpClient != null && _tcpClient.Connected)
            {
                _tcpClient.Client.Shutdown(SocketShutdown.Send);
            }
        }

        private void Clean()
        {
            try
            {
                try
                {
                    if (_stream != null)
                    {
                        _stream.Dispose();
                    }
                }
                catch { }
                try
                {
                    if (_tcpClient != null)
                    {
                        _tcpClient.Close();
                    }
                }
                catch { }
            }
            catch { }
            finally
            {
                _stream = null;
                _tcpClient = null;
            }

            if (_receiveBuffer != default(ArraySegment<byte>))
                _configuration.BufferManager.ReturnBuffer(_receiveBuffer);
            _receiveBuffer = default(ArraySegment<byte>);
            _receiveBufferOffset = 0;
        }

        #endregion

        #region Exception Handler

        private async Task HandleSendOperationException(Exception ex)
        {
            if (IsSocketTimeOut(ex))
            {
                await CloseIfShould(ex);
                throw new TcpSocketException(ex.Message, new TimeoutException(ex.Message, ex));
            }

            await CloseIfShould(ex);
            throw new TcpSocketException(ex.Message, ex);
        }

        private async Task HandleReceiveOperationException(Exception ex)
        {
            if (IsSocketTimeOut(ex))
            {
                await CloseIfShould(ex);
                throw new TcpSocketException(ex.Message, new TimeoutException(ex.Message, ex));
            }

            await CloseIfShould(ex);
            throw new TcpSocketException(ex.Message, ex);
        }

        private bool IsSocketTimeOut(Exception ex)
        {
            return ex is IOException
                && ex.InnerException != null
                && ex.InnerException is SocketException
                && (ex.InnerException as SocketException).SocketErrorCode == SocketError.TimedOut;
        }

        private async Task<bool> CloseIfShould(Exception ex)
        {
            if (ex is ObjectDisposedException
                || ex is InvalidOperationException
                || ex is SocketException
                || ex is IOException
                || ex is NullReferenceException // buffer array operation
                || ex is ArgumentException      // buffer array operation
                )
            {
                _logger?.Exception(ex.Message, ex);

                await CloseAsync(false); // intend to close the session

                return true;
            }

            return false;
        }

        private async Task HandleUserSideError(Exception ex)
        {
            _logger?.Error($"Client [{this}] error occurred in user side [{ex.Message}].");
            await Task.CompletedTask;
        }

        #endregion

        #region Send

        public void Send(byte[] data)
        {
            SendAsync(data, 0, data.Length).Wait(_configuration.SendTimeout);
        }

        public void Send(byte[] data, int offset, int count)
        {
            SendAsync(data, offset, count).Wait(_configuration.SendTimeout);
        }

        public async Task SendAsync(byte[] data)
        {
            await SendAsync(data, 0, data.Length);
        }

        public async Task SendAsync(byte[] data, int offset, int count)
        {
            BufferValidator.ValidateBuffer(data, offset, count, "data");

            if (State != SocketConnectionState.Connected)
            {
                throw new InvalidOperationException("This client has not connected to server.");
            }

            try
            {
                await _stream.WriteAsync(data, offset, count);
            }
            catch (Exception ex)
            {
                await HandleSendOperationException(ex);
            }
        }

        #endregion

        //public async Task<ArraySegment<byte>> Receive(byte[] buffer, int offset, int count)
        //{
        //    try
        //    {
        //        int frameLength;
        //        byte[] payload;
        //        int payloadOffset;
        //        int payloadCount;
        //        int consumedLength = 0;

        //        if (State == SocketConnectionState.Connected)
        //        {
        //            int receiveCount = await _stream.ReadAsync(
        //                _receiveBuffer.Array,
        //                _receiveBuffer.Offset + _receiveBufferOffset,
        //                _receiveBuffer.Count - _receiveBufferOffset);
        //            if (receiveCount == 0)
        //                break;

        //            SegmentBufferDeflector.ReplaceBuffer(_configuration.BufferManager, ref _receiveBuffer, ref _receiveBufferOffset, receiveCount);
        //            consumedLength = 0;

        //            while (true)
        //            {
        //                frameLength = 0;
        //                payload = null;
        //                payloadOffset = 0;
        //                payloadCount = 0;

        //                if (_frameBuilder.Decoder.TryDecodeFrame(
        //                    _receiveBuffer.Array,
        //                    _receiveBuffer.Offset + consumedLength,
        //                    _receiveBufferOffset - consumedLength,
        //                    out frameLength, out payload, out payloadOffset, out payloadCount))
        //                {
        //                    try
        //                    {
        //                        await _dispatcher.OnServerDataReceived(this, payload, payloadOffset, payloadCount);
        //                    }
        //                    catch (Exception ex) // catch all exceptions from out-side
        //                    {
        //                        await HandleUserSideError(ex);
        //                    }
        //                    finally
        //                    {
        //                        consumedLength += frameLength;
        //                    }
        //                }
        //                else
        //                {
        //                    break;
        //                }
        //            }

        //            if (_receiveBuffer != null && _receiveBuffer.Array != null)
        //            {
        //                SegmentBufferDeflector.ShiftBuffer(_configuration.BufferManager, consumedLength, ref _receiveBuffer, ref _receiveBufferOffset);
        //            }
        //        }
        //    }
        //    catch (ObjectDisposedException)
        //    {
        //        // looking forward to a graceful quit from the ReadAsync but the inside EndRead will raise the ObjectDisposedException,
        //        // so a gracefully close for the socket should be a Shutdown, but we cannot avoid the Close triggers this happen.
        //    }
        //    catch (Exception ex)
        //    {
        //        await HandleReceiveOperationException(ex);
        //        await CloseAsync(true); // read async buffer returned, remote notifies closed

        //    }
        //    finally
        //    {
        //    }

        //}

    }


}
