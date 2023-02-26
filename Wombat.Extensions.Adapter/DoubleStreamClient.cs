using System;
using System.Collections.Generic;
using System.Text;
using Wombat.IndustrialProtocol;
using Wombat.IndustrialProtocol.PLC;
using Wombat.Infrastructure;

namespace Wombat.Extensions.Adapter
{
    public class DoubleStreamClient
    {
        IEthernetClient _readClient;
        IEthernetClient _writeClient;

        public DoubleStreamClient(IEthernetClient readClient, IEthernetClient writeClient)
        {
            _readClient = readClient;
            _writeClient = writeClient;
        }

        public string Version => _readClient.Version;

        public bool IsConnect => _readClient.IsConnect & _writeClient.IsConnect;

        public  OperationResult<Dictionary<string, object>> BatchRead(Dictionary<string, DataTypeEnum> addresses)
        {
            return _readClient.BatchRead(addresses);
        }

        public  OperationResult BatchWrite(Dictionary<string, object> addresses)
        {
            return _writeClient.BatchWrite(addresses);
        }




        protected OperationResult Connect()
        {
            var read = _readClient.Connect();
            var write = _writeClient.Connect();
            return new OperationResult() { IsSuccess = read.IsSuccess & write.IsSuccess};
        }

        protected OperationResult Disconnect()
        {
            var read = _readClient.Disconnect();
            var write = _writeClient.Disconnect();
            return new OperationResult() { IsSuccess = read.IsSuccess & write.IsSuccess};
        }

        #region Read
        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="length"></param>
        /// <param name="isBit"></param>
        /// <param name="setEndian">返回值是否设置大小端</param>
        /// <returns></returns>
        public virtual OperationResult<byte[]> Read(string address, int length, bool isBit = false)
            => _readClient.Read(address, length, isBit);

        /// <summary>
        /// 读取Boolean
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public virtual OperationResult<bool> ReadBoolean(string address)
            => _readClient.ReadBoolean(address);
        /// <summary>
        /// 读取Boolean
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public virtual OperationResult<bool[]> ReadBoolean(string address, int length)
            => _readClient.ReadBoolean(address,length);





        /// <summary>
        /// 读取Int16
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public OperationResult<short> ReadInt16(string address)
            => _readClient.ReadInt16(address);

        /// <summary>
        /// 读取Int16
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public OperationResult<short[]> ReadInt16(string address, int length)
            => _readClient.ReadInt16(address,length);



        /// <summary>
        /// 读取UInt16
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public OperationResult<ushort> ReadUInt16(string address)
            => _readClient.ReadUInt16(address);

        /// <summary>
        /// 读取UInt16
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public OperationResult<ushort[]> ReadUInt16(string address, int length)
            => _readClient.ReadUInt16(address, length);




        /// <summary>
        /// 读取Int32
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public OperationResult<int> ReadInt32(string address)
            => _readClient.ReadInt32(address);

        /// <summary>
        /// 读取Int32
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public OperationResult<int[]> ReadInt32(string address, int length)
            => _readClient.ReadInt32(address,length);



        /// <summary>
        /// 读取UInt32
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public OperationResult<uint> ReadUInt32(string address)
            => _readClient.ReadUInt32(address);


        /// <summary>
        /// 读取UInt32
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public OperationResult<uint[]> ReadUInt32(string address, int length)
            => _readClient.ReadUInt32(address, length);




        /// <summary>
        /// 读取Int64
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public OperationResult<long> ReadInt64(string address)
            => _readClient.ReadInt64(address);

        /// <summary>
        /// 读取Int64
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public OperationResult<long[]> ReadInt64(string address, int length)
            => _readClient.ReadInt64(address, length);



        /// <summary>
        /// 读取UInt64
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public OperationResult<ulong> ReadUInt64(string address)
            => _readClient.ReadUInt64(address);

        /// <summary>
        /// 读取UInt64
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public OperationResult<ulong[]> ReadUInt64(string address, int length)
            => _readClient.ReadUInt64(address, length);




        /// <summary>
        /// 读取Float
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public OperationResult<float> ReadFloat(string address)
            => _readClient.ReadFloat(address);

        /// <summary>
        /// 读取Float
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public OperationResult<float[]> ReadFloat(string address, int length)
            => _readClient.ReadFloat(address,length);



        /// <summary>
        /// 读取Double
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public OperationResult<double> ReadDouble(string address)
            => _readClient.ReadDouble(address);

        /// <summary>
        /// 读取Double
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public OperationResult<double[]> ReadDouble(string address, int length)
            => _readClient.ReadDouble(address, length);



        public OperationResult<string> ReadString(string address, int length)
            => _readClient.ReadString(address, length);


        #endregion

        #region Write

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="data">值</param>
        /// <param name="isBit">值</param>
        /// <returns></returns>
        public  OperationResult Write(string address, byte[] data, bool isBit = false)
            => _writeClient.Write(address, data,isBit);

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public  OperationResult Write(string address, bool value)
            => _writeClient.Write(address, value);

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public  OperationResult Write(string address, bool[] value)
            => _writeClient.Write(address, value);


        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public OperationResult Write(string address, sbyte value)
            => _writeClient.Write(address, value);

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public OperationResult Write(string address, short value)
            => _writeClient.Write(address, value);

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public OperationResult Write(string address, short[] value)
            => _writeClient.Write(address, value);


        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public OperationResult Write(string address, ushort value)
            => _writeClient.Write(address, value);

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public OperationResult Write(string address, ushort[] value)
            => _writeClient.Write(address, value);

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public OperationResult Write(string address, int value)
            => _writeClient.Write(address, value);

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public OperationResult Write(string address, int[] value)
            => _writeClient.Write(address, value);

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public OperationResult Write(string address, uint value)
            => _writeClient.Write(address, value);

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public OperationResult Write(string address, uint[] value)
            => _writeClient.Write(address, value);



        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public OperationResult Write(string address, long value)
            => _writeClient.Write(address, value);

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public OperationResult Write(string address, long[] value)
            => _writeClient.Write(address, value);



        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public OperationResult Write(string address, ulong value)
            => _writeClient.Write(address, value);



        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public OperationResult Write(string address, ulong[] value)
            => _writeClient.Write(address, value);


        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public OperationResult Write(string address, float value)
            => _writeClient.Write(address, value);

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public OperationResult Write(string address, float[] value)
            => _writeClient.Write(address, value);


        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public OperationResult Write(string address, double value)
            => _writeClient.Write(address, value);


        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public OperationResult Write(string address, double[] value)
            => _writeClient.Write(address, value);

        public OperationResult Write(string address, string value)
            => _writeClient.Write(address, value);



        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <param name="type">数据类型</param>
        /// <returns></returns>
        public OperationResult Write(string address, object value, DataTypeEnum type)
            => _writeClient.Write(address, value,type);




        #endregion

    }
}
