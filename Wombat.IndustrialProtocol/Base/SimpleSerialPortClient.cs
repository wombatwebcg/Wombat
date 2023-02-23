using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Net;
using System.Text;
using Wombat.Infrastructure;

namespace Wombat.IndustrialProtocol
{
    public class SimpleSerialPortClient : SerialPortBase
    {
        private AdvancedHybirdLock _advancedHybirdLock;

        public override bool IsConnect => base.IsConnect;

        public SimpleSerialPortClient():base()
        {
            _advancedHybirdLock = new AdvancedHybirdLock();
        }

        public SimpleSerialPortClient(string portName, int baudRate = 9600, int dataBits = 8, StopBits stopBits = StopBits.One, Parity parity = Parity.None, Handshake handshake = Handshake.None) : base(portName, baudRate, dataBits, stopBits, parity, handshake)
        {
            _advancedHybirdLock = new AdvancedHybirdLock();

        }

        public override OperationResult<byte[]> SendPackageSingle(byte[] command)
        {
            _advancedHybirdLock.Enter();
            var result = base.SendPackageSingle(command);
           _advancedHybirdLock.Leave();
            return result;
        }

        public override OperationResult<string> SendPackageReliable(string command, Encoding encoding)
        {
            _advancedHybirdLock.Enter();
            var result = base.SendPackageReliable(command, encoding);
            _advancedHybirdLock.Leave();
            return result;
        }
    }
}
