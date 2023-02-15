using Wombat.Infrastructure;
using Wombat.IndustrialProtocol.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Wombat.IndustrialProtocol.Modbus
{
    /// <summary>
    /// ModbusTcp协议客户端
    /// </summary>
    public class ModbusTcpClient : ModbusSocketBase
    {
        public ModbusTcpClient(IPEndPoint ipAndPoint) : base(ipAndPoint)
        {
        }

        public ModbusTcpClient(string ip, int port) : base(ip, port)
        {
        }


    }
}

