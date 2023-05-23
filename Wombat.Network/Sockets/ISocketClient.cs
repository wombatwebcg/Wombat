using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Wombat.Network.Sockets
{
    public interface ISocketClient
    {



        #region Properties

        TcpSocketClientConfiguration SocketConfiguration { get; set; }

        bool Connected { get; }

        IPEndPoint RemoteEndPoint { get; }

        SocketConnectionState State { get;}

        IPEndPoint LocalEndPoint { get; set; }

        ClientSecurityOptions Security { get; set; }



        #endregion

        #region Connect

        Task ConnectAsync(IPEndPoint remoteEndPoint);

        void Connect(IPEndPoint remoteEndPoint);

        Task CloseAsync();

        void Close();

        #endregion

        #region Send

       void Send(byte[] data);

        void Send(byte[] data, int offset, int count);

        Task SendAsync(byte[] data);

        Task SendAsync(byte[] data, int offset, int count);

        #endregion

        #region Receive

        int Receive(byte[] data);

        int Receive(byte[] data, int offset, int count);

        ValueTask<int> ReceiveAsync(byte[] data);

        ValueTask<int> ReceiveAsync(byte[] data, int offset, int count);

        #endregion

    }
}
