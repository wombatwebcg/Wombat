using System;
using System.Collections.Generic;
using System.Text;
using Wombat.IndustrialProtocol;
using Wombat.IndustrialProtocol.PLC;
using Wombat.Infrastructure;

namespace Wombat.Extensions.Adapter
{
    public class DoubleStreamClient : IEthernetClientBase
    {
        public DoubleStreamClient()
        {
        }

        public override string Version => throw new NotImplementedException();

        public override bool IsConnect => throw new NotImplementedException();

        public override OperationResult<Dictionary<string, object>> BatchRead(Dictionary<string, DataTypeEnum> addresses)
        {
            throw new NotImplementedException();
        }

        public override OperationResult BatchWrite(Dictionary<string, object> addresses)
        {
            throw new NotImplementedException();
        }

        public override OperationResult<byte[]> Read(string address, int length, bool isBit = false)
        {
            throw new NotImplementedException();
        }

        public override OperationResult<byte[]> SendPackageSingle(byte[] command)
        {
            throw new NotImplementedException();
        }

        public override OperationResult Write(string address, byte[] data, bool isBit = false)
        {
            throw new NotImplementedException();
        }

        protected override OperationResult DoConnect()
        {
            throw new NotImplementedException();
        }

        protected override OperationResult DoDisconnect()
        {
            throw new NotImplementedException();
        }
    }
}
