using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Wombat.Core;
using Wombat.Network.Buffer;

namespace Wombat.Network.Sockets
{
    public class TcpRawSocketClient: SocketClientBase
    {

        #region Constructors

        public TcpRawSocketClient(IPAddress localAddress, int localPort, TcpSocketClientConfiguration configuration = null)
                : this(new IPEndPoint(localAddress, localPort), configuration)
        {
        }


        public TcpRawSocketClient(TcpSocketClientConfiguration configuration = null)
            : this(null, configuration)
        {

        }

        public TcpRawSocketClient(IPEndPoint localEP, TcpSocketClientConfiguration configuration = null)
        {
            base.LocalEndPoint = localEP;
            SocketConfiguration = configuration ?? new TcpSocketClientConfiguration();
            Security = Security ?? new ClientSecurityOptions();
            if (SocketConfiguration.BufferManager == null)
                throw new InvalidProgramException("The buffer manager in configuration cannot be null.");
        }







        #endregion



    }


}
