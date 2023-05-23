using System;
using System.Text;
using System.Threading.Tasks;
using Wombat.Core;
using Wombat.Network.Socket;

namespace Wombat.Socket.TestTcpSocketServer
{
    class Program
    {
        static TcpSocketServer _server;

        static void Main(string[] args)
        {
            var logger = new LoggerBuilder().LogEventLevel(LogEventLevel.Debug).UseConsoleLogger().UseFileLogger().CreateLogger();
            try
            {
                var config = new TcpSocketServerConfiguration();
                //config.UseSsl = true;
                //config.SslServerCertificate = new System.Security.Cryptography.X509Certificates.X509Certificate2(@"D:\\Cowboy.pfx", "Cowboy");
                //config.SslPolicyErrorsBypassed = false;

                //config.FrameBuilder = new FixedLengthFrameBuilder(20000);
                //config.FrameBuilder = new RawBufferFrameBuilder();
                //config.FrameBuilder = new LineBasedFrameBuilder();
                //config.FrameBuilder = new LengthPrefixedFrameBuilder();
                //config.FrameBuilder = new LengthFieldBasedFrameBuilder();

                _server = new TcpSocketServer(22222, new SimpleEventDispatcher(), config, frameBuilder: new LengthPrefixedFrameBuilder());
                _server.UsgLogger(logger);
                _server.Listen();

                Console.WriteLine("TCP server has been started on [{0}].", _server.ListenedEndPoint);
                Console.WriteLine("Type something to send to clients...");
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
                                text = new string('x', 8192);
                                for (int i = 0; i < 1000000; i++)
                                {
                                    await _server.BroadcastAsync(Encoding.UTF8.GetBytes(text));
                                    Console.WriteLine("Server [{0}] broadcasts text -> [{1}].", _server.ListenedEndPoint, text);
                                }
                            }
                            else if (text == "big1k")
                            {
                                text = new string('x', 1024 * 1);
                                await _server.BroadcastAsync(Encoding.UTF8.GetBytes(text));
                                Console.WriteLine("Server [{0}] broadcasts text -> [{1} Bytes].", _server.ListenedEndPoint, text.Length);
                            }
                            else if (text == "big10k")
                            {
                                text = new string('x', 1024 * 10);
                                await _server.BroadcastAsync(Encoding.UTF8.GetBytes(text));
                                Console.WriteLine("Server [{0}] broadcasts text -> [{1} Bytes].", _server.ListenedEndPoint, text.Length);
                            }
                            else if (text == "big100k")
                            {
                                text = new string('x', 1024 * 100);
                                await _server.BroadcastAsync(Encoding.UTF8.GetBytes(text));
                                Console.WriteLine("Server [{0}] broadcasts text -> [{1} Bytes].", _server.ListenedEndPoint, text.Length);
                            }
                            else if (text == "big1m")
                            {
                                text = new string('x', 1024 * 1024 * 1);
                                await _server.BroadcastAsync(Encoding.UTF8.GetBytes(text));
                                Console.WriteLine("Server [{0}] broadcasts text -> [{1} Bytes].", _server.ListenedEndPoint, text.Length);
                            }
                            else if (text == "big10m")
                            {
                                text = new string('x', 1024 * 1024 * 10);
                                await _server.BroadcastAsync(Encoding.UTF8.GetBytes(text));
                                Console.WriteLine("Server [{0}] broadcasts text -> [{1} Bytes].", _server.ListenedEndPoint, text.Length);
                            }
                            else if (text == "big100m")
                            {
                                text = new string('x', 1024 * 1024 * 100);
                                await _server.BroadcastAsync(Encoding.UTF8.GetBytes(text));
                                Console.WriteLine("Server [{0}] broadcasts text -> [{1} Bytes].", _server.ListenedEndPoint, text.Length);
                            }
                            else if (text == "big1g")
                            {
                                text = new string('x', 1024 * 1024 * 1024);
                                await _server.BroadcastAsync(Encoding.UTF8.GetBytes(text));
                                Console.WriteLine("Server [{0}] broadcasts text -> [{1} Bytes].", _server.ListenedEndPoint, text.Length);
                            }
                            else
                            {
                                await _server.BroadcastAsync(Encoding.UTF8.GetBytes(text));
                                Console.WriteLine("Server [{0}] broadcasts text -> [{1} Bytes].", _server.ListenedEndPoint, text.Length);
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        logger.Exception(ex.Message, ex);
                    }
                }

                _server.Shutdown();
                Console.WriteLine("TCP server has been stopped on [{0}].", _server.ListenedEndPoint);
            }
            catch (Exception ex)
            {
                logger.Exception(ex.Message, ex);
            }

            Console.ReadKey();
        }
    }
}
