using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;

namespace Wombat.SerialPorts.Extensions
{
    public  interface ISerialPortsBaseParms
    {
        /// <summary>
        /// 串口名称
        /// </summary>
        /// <value>
        /// The name of the port.
        /// </value>
        string PortName { get; set; }

        /// <summary>
        /// 波特率
        /// </summary>
        /// <value>
        /// The baud rate.
        /// </value>
        int BaudRate { get; set; }

        /// <summary>
        /// Gets or sets the data bits.
        /// </summary>
        /// <value>
        /// The data bits.
        /// </value>
        int DataBits { get; set; }

        /// <summary>
        /// Gets or sets the stop bits.
        /// </summary>
        /// <value>
        /// The stop bits.
        /// </value>
        StopBits StopBits { get; set; }

        /// <summary>
        /// Gets or sets the parity bits.
        /// </summary>
        /// <value>
        /// The parity bits.
        /// </value>
        Parity Parity { get; set; }

        /// <summary>
        /// Gets or sets the handshake.
        /// </summary>
        /// <value>
        /// The handshake.
        /// </value>
        Handshake Handshake { get; set; }

    }
}
