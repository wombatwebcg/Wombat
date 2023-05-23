using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Wombat.Core;
using Wombat.Network.Socket;

namespace Wombat.Socket.TestTcpSocketClient 
{ 
    class Program
    {
        static TcpSocketClient _client1;
        static TcpSocketClient _client2;
        static Logger logger;
        static void Main(string[] args)
        {
              logger = new LoggerBuilder().LogEventLevel(LogEventLevel.Debug).UseConsoleLogger().UseFileLogger().CreateLogger();

            try
            {
                var config = new TcpSocketClientConfiguration();
                //config.UseSsl = true;
                //config.SslTargetHost = "Cowboy";
                //config.SslClientCertificates.Add(new System.Security.Cryptography.X509Certificates.X509Certificate2(@"D:\\Cowboy.cer"));
                //config.SslPolicyErrorsBypassed = false;

                //config.FrameBuilder = new FixedLengthFrameBuilder(20000);
                //config.FrameBuilder = new RawBufferFrameBuilder();
                //config.FrameBuilder = new LineBasedFrameBuilder();
                //config.FrameBuilder = new LengthPrefixedFrameBuilder();
                //config.FrameBuilder = new LengthFieldBasedFrameBuilder();

                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 22222);
                _client1 = new TcpSocketClient(remoteEP,dispatcher: new SimpleEventDispatcher(),configuration: new TcpSocketClientConfiguration(),frameBuilder:new LengthPrefixedFrameBuilder());
                _client2 = new TcpSocketClient(remoteEP,dispatcher: new SimpleEventDispatcher(),configuration: new TcpSocketClientConfiguration(), frameBuilder: new LengthPrefixedFrameBuilder());
                _client2.UsgLogger(logger);
                _client1.UsgLogger(logger);
                _client1.ConnectAsync().Wait();
                _client2.ConnectAsync().Wait();
                Console.WriteLine("TCP client has connected to server [{0}].", remoteEP);
                Console.WriteLine("Type something to send to server...");
                while (true)
                {
                    try
                    {
                        string text = Console.ReadLine();
                        if (text == "quit")
                            break;
                        Task.Run(async () =>
                        {
                            if (text == "many")
                            {
                                text = "";
                                for (int i = 0; i < 100000; i++)
                                {
                                    text += $"{i},";
                                }

                                //text = new string('123456789', 10);
                                for (int i = 0; i < 1000; i++)
                                {
                                    await _client1.SendAsync(Encoding.UTF8.GetBytes(text));
                                    await _client2.SendAsync(Encoding.UTF8.GetBytes(text));

                                    Console.WriteLine("Client [{0}] send text -> [{1}].", _client1.LocalEndPoint, text);
                                    Console.WriteLine("Client [{0}] send text -> [{1}].", _client2.LocalEndPoint, text);

                                }
                            }
                            else if (text == "big1k")
                            {
                                text = new string('x', 1024 * 1);
                                await _client1.SendAsync(Encoding.UTF8.GetBytes(text));
                                Console.WriteLine("Client [{0}] send text -> [{1} Bytes].", _client1.LocalEndPoint, text.Length);
                            }
                            else if (text == "big10k")
                            {
                                text = new string('x', 1024 * 10);
                                await _client1.SendAsync(Encoding.UTF8.GetBytes(text));
                                Console.WriteLine("Client [{0}] send text -> [{1} Bytes].", _client1.LocalEndPoint, text.Length);
                            }
                            else if (text == "big100k")
                            {
                                text = new string('x', 1024 * 100);
                                await _client1.SendAsync(Encoding.UTF8.GetBytes(text));
                                Console.WriteLine("Client [{0}] send text -> [{1} Bytes].", _client1.LocalEndPoint, text.Length);
                            }
                            else if (text == "big1m")
                            {
                                text = new string('x', 1024 * 1024 * 1);
                                await _client1.SendAsync(Encoding.UTF8.GetBytes(text));
                                Console.WriteLine("Client [{0}] send text -> [{1} Bytes].", _client1.LocalEndPoint, text.Length);
                            }
                            else if (text == "big10m")
                            {
                                text = new string('x', 1024 * 1024 * 10);
                                await _client1.SendAsync(Encoding.UTF8.GetBytes(text));
                                Console.WriteLine("Client [{0}] send text -> [{1} Bytes].", _client1.LocalEndPoint, text.Length);
                            }
                            else if (text == "big100m")
                            {
                                text = new string('x', 1024 * 1024 * 100);
                                await _client1.SendAsync(Encoding.UTF8.GetBytes(text));
                                Console.WriteLine("Client [{0}] send text -> [{1} Bytes].", _client1.LocalEndPoint, text.Length);
                            }
                            else if (text == "big1g")
                            {
                                text = new string('x', 1024 * 1024 * 1024);
                                await _client1.SendAsync(Encoding.UTF8.GetBytes(text));
                                Console.WriteLine("Client [{0}] send text -> [{1} Bytes].", _client1.LocalEndPoint, text.Length);
                            }
                            else
                            {
                                await _client1.SendAsync(Encoding.UTF8.GetBytes(text));
                                Console.WriteLine("Client [{0}] send text -> [{1} Bytes].", _client1.LocalEndPoint, text.Length);
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        logger.Exception(ex.Message, ex);
                    }
                }

                _client1.Shutdown();
                _client2.Shutdown();

                Console.WriteLine("TCP client has disconnected from server [{0}].", remoteEP);
            }
            catch (Exception ex)
            {
                logger.Exception(ex.Message, ex);
            }

            Console.ReadKey();
        }
    }
}
