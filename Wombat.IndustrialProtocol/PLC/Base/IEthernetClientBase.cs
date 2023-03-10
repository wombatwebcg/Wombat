using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Wombat.Infrastructure;

namespace Wombat.IndustrialProtocol.PLC
{
   public abstract class IEthernetClientBase: BaseMessageModel, IEthernetClient
    {
        public  IPEndPoint IpEndPoint { get; set; }

        public abstract  string Version { get; }


        /// <summary>
        /// 分批缓冲区大小
        /// </summary>
        protected const int BufferSize = 4096;



        /// <summary>
        /// Socket读取
        /// </summary>
        /// <param name="socket">socket</param>
        /// <param name="receiveCount">读取长度</param>          
        /// <returns></returns>
        protected OperationResult<byte[]> SocketRead(Socket socket, int receiveCount)
        {
            var result = new OperationResult<byte[]>();
            if (receiveCount < 0)
            {
                result.IsSuccess = false;
                result.Message = $"读取长度[receiveCount]为{receiveCount}";
                result.AddMessage2List();
                return result;
            }

            byte[] receiveBytes = new byte[receiveCount];
            int receiveFinish = 0;
            while (receiveFinish < receiveCount)
            {
                // 分批读取
                int receiveLength = (receiveCount - receiveFinish) >= BufferSize ? BufferSize : (receiveCount - receiveFinish);
                try
                {
                    var readLeng = socket.Receive(receiveBytes, receiveFinish, receiveLength, SocketFlags.None);
                    if (readLeng == 0)
                    {
                        socket?.SafeClose();
                        result.IsSuccess = false;
                        result.Message = $"连接被断开";
                        result.AddMessage2List();
                        return result;
                    }
                    receiveFinish += readLeng;
                }
                catch (SocketException ex)
                {
                    socket?.SafeClose();
                    if (ex.SocketErrorCode == SocketError.TimedOut)
                        result.Message = $"连接超时：{ex.Message}";
                    else
                        result.Message = $"连接被断开，{ex.Message}";
                    result.IsSuccess = false;
                    result.AddMessage2List();
                    result.Exception = ex;
                    return result;
                }
            }
            result.Value = receiveBytes;
            return result.EndTime();
        }


        /// <summary>
        /// 发送报文，并获取响应报文（如果网络异常，会自动进行一次重试）
        /// TODO 重试机制应改成用户主动设置
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public override OperationResult<byte[]> SendPackageReliable(byte[] command)
        {
            try
            {
                var result = SendPackageSingle(command);
                if (!result.IsSuccess)
                {
                    WarningLog?.Invoke(result.Message, result.Exception);
                    //如果出现异常，则进行一次重试         
                    var conentOperationResult = Connect();
                    if (!conentOperationResult.IsSuccess)
                    {
                        return new OperationResult<byte[]>(conentOperationResult);

                    }
                    else
                    {
                        result = SendPackageSingle(command); ;
                        return result.EndTime();
                    }
                }
                else
                {
                    return result.EndTime();

                }
            }
            catch (Exception ex)
            {
                try
                {
                    WarningLog?.Invoke(ex.Message, ex);
                    //如果出现异常，则进行一次重试                
                    var conentOperationResult = Connect();
                    if (!conentOperationResult.IsSuccess)
                    {
                        return new OperationResult<byte[]>(conentOperationResult);
                    }
                    else
                    {
                      var  result = SendPackageSingle(command); ;
                        return result.EndTime();
                    }
                }
                catch (Exception ex2)
                {
                    var result = new OperationResult<byte[]>();
                    result.IsSuccess = false;
                    result.Message = ex2.Message;
                    result.AddMessage2List();
                    return result.EndTime();
                }
            }
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
        public abstract OperationResult<byte[]> Read(string address, int length, bool isBit = false);

        /// <summary>
        /// 读取Boolean
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public virtual OperationResult<bool> ReadBoolean(string address)
        {
            var result = ReadBoolean(address, 1);
            if(result.IsSuccess)
                return new OperationResult<bool>(result) { Value = result.Value[0] }.EndTime();
            else
                return new OperationResult<bool>(result).EndTime();
        }


        /// <summary>
        /// 读取Boolean
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public virtual OperationResult<bool[]> ReadBoolean(string address,int length)
        {
            //int reallength = (int)Math.Ceiling(length*1.0 /8);
            var readResult = Read(address, length, isBit: true);
            var result = new OperationResult<bool[]>(readResult);
            if (result.IsSuccess)
                result.Value = readResult.Value.TransBool(0, length, IsReverse);
            return result.EndTime();
        }


        public OperationResult<bool> ReadBoolean(int startAddressInt, int addressInt, byte[] values)
        {
            try
            {
                var interval = addressInt - startAddressInt;
                var byteArry = values.Skip(interval * 1).Take(1).ToArray();
                return new OperationResult<bool>
                {
                    Value = BitConverter.ToBoolean(byteArry, 0)
                };
            }
            catch (Exception ex)
            {
                return new OperationResult<bool>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }


        /// <summary>
        /// 读取Int16
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public OperationResult<short> ReadInt16(string address)
        {
            var result = ReadInt16(address, 1);
            if (result.IsSuccess)
                return new OperationResult<short>(result) { Value = result.Value[0] }.EndTime();
            else
                return new OperationResult<short>(result).EndTime();
        }

        /// <summary>
        /// 读取Int16
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public OperationResult<short[]> ReadInt16(string address,int length)
        {
            var readResult = Read(address, 2*length);
            var result = new OperationResult<short[]>(readResult);
            if (result.IsSuccess)
                result.Value = readResult.Value.TransInt16(0,length ,IsReverse);
            return result.EndTime();
        }


        public OperationResult<short> ReadInt16(int startAddressInt, int addressInt, byte[] values)
        {
            try
            {
                var interval = addressInt - startAddressInt;
                var byteArry = values.Skip(interval * 2).Take(2).Reverse().ToArray();
                return new OperationResult<short>
                {
                    Value = BitConverter.ToInt16(byteArry, 0)
                };
            }
            catch (Exception ex)
            {
                return new OperationResult<short>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// 读取UInt16
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public OperationResult<ushort> ReadUInt16(string address)
        {
            var result = ReadUInt16(address, 1);
            if(result.IsSuccess)
                return new OperationResult<ushort>(result) { Value = result.Value[0] }.EndTime();
            else
                return new OperationResult<ushort>(result).EndTime();
        }

        /// <summary>
        /// 读取UInt16
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public OperationResult<ushort[]> ReadUInt16(string address, int length)
        {
            var readResult = Read(address, 2 * length);
            var result = new OperationResult<ushort[]>(readResult);
            if (result.IsSuccess)
                result.Value = readResult.Value.TransUInt16(0, length, IsReverse);
            return result.EndTime();
        }



        public OperationResult<ushort> ReadUInt16(int startAddressInt, int addressInt, byte[] values)
        {
            try
            {
                var interval = addressInt - startAddressInt;
                var byteArry = values.Skip(interval * 2).Take(2).Reverse().ToArray();
                return new OperationResult<ushort>
                {
                    Value = BitConverter.ToUInt16(byteArry, 0)
                };
            }
            catch (Exception ex)
            {
                return new OperationResult<ushort>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// 读取Int32
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public OperationResult<int> ReadInt32(string address)
        {
            var result = ReadInt32(address, 1);
            if (result.IsSuccess)
                return new OperationResult<int>(result) {Value = result.Value[0] }.EndTime();
            else
                return new OperationResult<int>(result).EndTime();

        }

        /// <summary>
        /// 读取Int32
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public OperationResult<int[]> ReadInt32(string address,int length)
        {
            var readResult = Read(address, 4*length);
            var result = new OperationResult<int[]>(readResult);
            if (result.IsSuccess)
                result.Value = readResult.Value.TransInt32(0,length ,DataFormat, IsReverse);
            return result.EndTime();
        }


        public OperationResult<int> ReadInt32(int startAddressInt, int addressInt, byte[] values)
        {
            try
            {
                var interval = (addressInt - startAddressInt) / 2;
                var offset = (addressInt - startAddressInt) % 2 * 2;//取余 乘以2（每个地址16位，占两个字节）
                var byteArry = values.Skip(interval * 2 * 2 + offset).Take(2 * 2).ToArray();
                return new OperationResult<int>
                {
                    Value = byteArry.TransInt32(0, DataFormat, IsReverse)
                };
            }
            catch (Exception ex)
            {
                return new OperationResult<int>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// 读取UInt32
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public OperationResult<uint> ReadUInt32(string address)
        {
            var result = ReadUInt32(address, 1);
            if (result.IsSuccess)
                return new OperationResult<uint>(result) { Value = result.Value[0] }.EndTime();
            else
                return new OperationResult<uint>(result).EndTime();
        }


        /// <summary>
        /// 读取UInt32
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public OperationResult<uint[]> ReadUInt32(string address,int length)
        {
            var readResult = Read(address, 4 * length);
            var result = new OperationResult<uint[]>(readResult);
            if (result.IsSuccess)
                result.Value = readResult.Value.TransUInt32(0, length, DataFormat, IsReverse);
            return result.EndTime();
        }



        public OperationResult<uint> ReadUInt32(int startAddressInt, int addressInt, byte[] values)
        {
            try
            {
                var interval = (addressInt - startAddressInt) / 2;
                var offset = (addressInt - startAddressInt) % 2 * 2;//取余 乘以2（每个地址16位，占两个字节）
                var byteArry = values.Skip(interval * 2 * 2 + offset).Take(2 * 2).ToArray();
                return new OperationResult<uint>
                {
                    Value = byteArry.TransUInt32(0,  DataFormat, IsReverse)
            };
            }
            catch (Exception ex)
            {
                return new OperationResult<uint>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// 读取Int64
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public OperationResult<long> ReadInt64(string address)
        {
            var result = ReadInt64(address, 1);
            if (result.IsSuccess)
                return new OperationResult<long>(result) { Value = result.Value[0] }.EndTime();
            else
                return new OperationResult<long>(result).EndTime();
        }

        /// <summary>
        /// 读取Int64
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public OperationResult<long[]> ReadInt64(string address,int length)
        {
            var readResult = Read(address, 8*length);
            var result = new OperationResult<long[]>(readResult);
            if (result.IsSuccess)
                result.Value = readResult.Value.TransInt64(0,length ,DataFormat, IsReverse);
            return result.EndTime();
        }


        public OperationResult<long> ReadInt64(int startAddressInt, int addressInt, byte[] values)
        {
            try
            {
                var interval = (addressInt - startAddressInt) / 4;
                var offset = (addressInt - startAddressInt) % 4 * 2;
                var byteArry = values.Skip(interval * 2 * 4 + offset).Take(2 * 4).ToArray();
                return new OperationResult<long>
                {
                    Value = byteArry.TransInt64(0, DataFormat, IsReverse)
                };
            }
            catch (Exception ex)
            {
                return new OperationResult<long>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// 读取UInt64
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public OperationResult<ulong> ReadUInt64(string address)
        {
            var result = ReadUInt64(address, 1);
            if (result.IsSuccess)
                return new OperationResult<ulong>(result) { Value = result.Value[0] }.EndTime();
            else
                return new OperationResult<ulong>(result).EndTime();
        }

        /// <summary>
        /// 读取UInt64
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public OperationResult<ulong[]> ReadUInt64(string address,int length)
        {
            var readResult = Read(address, 8*length);
            var result = new OperationResult<ulong[]>(readResult);
            if (result.IsSuccess)
                result.Value = readResult.Value.TransUInt64(0,length ,DataFormat, IsReverse);
            return result.EndTime();
        }



        public OperationResult<ulong> ReadUInt64(int startAddressInt, int addressInt, byte[] values)
        {
            try
            {
                var interval = (addressInt - startAddressInt) / 4;
                var offset = (addressInt - startAddressInt) % 4 * 2;
                var byteArry = values.Skip(interval * 2 * 4 + offset).Take(2 * 4).ToArray();
                return new OperationResult<ulong>
                {
                    Value = byteArry.TransUInt64(0, DataFormat, IsReverse)
            };
            }
            catch (Exception ex)
            {
                return new OperationResult<ulong>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// 读取Float
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public OperationResult<float> ReadFloat(string address)
        {
            var result = ReadFloat(address, 1);
            if (result.IsSuccess)
                return new OperationResult<float>(result) { Value = result.Value[0] }.EndTime();
            else
                return new OperationResult<float>(result).EndTime();
        }

        /// <summary>
        /// 读取Float
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public OperationResult<float[]> ReadFloat(string address,int length)
        {
            var readResult = Read(address, 4*length);
            var result = new OperationResult<float[]>(readResult);
            if (result.IsSuccess)
                result.Value = readResult.Value.TransFloat(0, length, DataFormat, IsReverse);
            return result.EndTime();
        }


        public OperationResult<float> ReadFloat(int beginAddressInt, int addressInt, byte[] values)
        {
            try
            {
                var interval = (addressInt - beginAddressInt) / 2;
                var offset = (addressInt - beginAddressInt) % 2 * 2;//取余 乘以2（每个地址16位，占两个字节）
                var byteArry = values.Skip(interval * 2 * 2 + offset).Take(2 * 2).ToArray();
                return new OperationResult<float>
                {
                    Value = byteArry.TransFloat(0,DataFormat,IsReverse)
                };
            }
            catch (Exception ex)
            {
                return new OperationResult<float>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// 读取Double
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public OperationResult<double> ReadDouble(string address)
        {
            var result = ReadDouble(address, 1);
            if (result.IsSuccess)
                return new OperationResult<double>(result) { Value = result.Value[0] }.EndTime();
            else
                return new OperationResult<double>(result).EndTime();
        }

        /// <summary>
        /// 读取Double
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public OperationResult<double[]> ReadDouble(string address,int length)
        {
            var readResult = Read(address, 8*length);
            var result = new OperationResult<double[]>(readResult);
            if (result.IsSuccess)
                result.Value = readResult.Value.TransDouble(0,length, DataFormat, IsReverse);
            return result.EndTime();
        }


        public OperationResult<double> ReadDouble(int beginAddressInt, int addressInt, byte[] values)
        {
            try
            {
                var interval = (addressInt - beginAddressInt) / 4;
                var offset = (addressInt - beginAddressInt) % 4 * 2;
                var byteArry = values.Skip(interval * 2 * 4 + offset).Take(2 * 4).ToArray();
                return new OperationResult<double>
                {
                    Value = byteArry.TransDouble(0,DataFormat,IsReverse)
                };
            }
            catch (Exception ex)
            {
                return new OperationResult<double>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public OperationResult<string> ReadString(string address, int length)
        {
            var readResult = Read(address, 4 * length);
            var result = new OperationResult<string>(readResult);
            if (result.IsSuccess)
                result.Value = readResult.Value.TransString(0, length, encoding:Encoding.ASCII);
            return result.EndTime();
        }


        #endregion

        #region Write

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="data">值</param>
        /// <param name="isBit">值</param>
        /// <returns></returns>
        public abstract OperationResult Write(string address, byte[] data, bool isBit = false);

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public virtual OperationResult Write(string address, bool value)
        {
            return Write(address, value ? new byte[] { 0x01 } : new byte[] { 0x00 }, true);
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public virtual OperationResult Write(string address, bool[] value)
        {
            return Write(address, value.TransByte(),true);
        }


        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public OperationResult Write(string address, sbyte value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public OperationResult Write(string address, short value)
        {
            return Write(address, value.TransByte(IsReverse) );
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public OperationResult Write(string address, short[] value)
        {
            return Write(address, value.TransByte(IsReverse));
        }


        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public OperationResult Write(string address, ushort value)
        {
            return Write(address, value.TransByte(IsReverse));
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public OperationResult Write(string address, ushort[] value)
        {
            return Write(address, value.TransByte(IsReverse));
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public OperationResult Write(string address, int value)
        {
            return Write(address, value.TransByte(DataFormat,IsReverse));
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public OperationResult Write(string address, int[] value)
        {
            return Write(address, value.TransByte(DataFormat, IsReverse));
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public OperationResult Write(string address, uint value)
        {
            return Write(address, value.TransByte(DataFormat, IsReverse));
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public OperationResult Write(string address, uint[] value)
        {
            return Write(address, value.TransByte(DataFormat, IsReverse));
        }



        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public OperationResult Write(string address, long value)
        {
            return Write(address, value.TransByte(DataFormat, IsReverse));
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public OperationResult Write(string address, long[] value)
        {
            return Write(address, value.TransByte(DataFormat, IsReverse));
        }



        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public OperationResult Write(string address, ulong value)
        {
            return Write(address, value.TransByte(DataFormat, IsReverse));
        }



        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public OperationResult Write(string address, ulong[] value)
        {
            return Write(address, value.TransByte(DataFormat, IsReverse));
        }


        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public OperationResult Write(string address, float value)
        {
            return Write(address, value.TransByte(DataFormat, IsReverse));
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public OperationResult Write(string address, float[] value)
        {
            return Write(address, value.TransByte(DataFormat, IsReverse));
        }


        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public OperationResult Write(string address, double value)
        {
            return Write(address, value.TransByte(DataFormat, IsReverse));
        }


        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public OperationResult Write(string address, double[] value)
        {
            return Write(address, value.TransByte(DataFormat, IsReverse));
        }

        public OperationResult Write(string address, string value)
        {
            return Write(address, value.TransByte(Encoding.ASCII, DataFormat, IsReverse));
        }



        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <param name="type">数据类型</param>
        /// <returns></returns>
        public OperationResult Write(string address, object value, DataTypeEnum type)
        {
            var result = new OperationResult() { IsSuccess = false };
            switch (type)
            {
                case DataTypeEnum.Bool:
                    result = Write(address, Convert.ToBoolean(value));
                    break;
                case DataTypeEnum.Byte:
                    result = Write(address, Convert.ToByte(value));
                    break;
                case DataTypeEnum.Int16:
                    result = Write(address, Convert.ToInt16(value));
                    break;
                case DataTypeEnum.UInt16:
                    result = Write(address, Convert.ToUInt16(value));
                    break;
                case DataTypeEnum.Int32:
                    result = Write(address, Convert.ToInt32(value));
                    break;
                case DataTypeEnum.UInt32:
                    result = Write(address, Convert.ToUInt32(value));
                    break;
                case DataTypeEnum.Int64:
                    result = Write(address, Convert.ToInt64(value));
                    break;
                case DataTypeEnum.UInt64:
                    result = Write(address, Convert.ToUInt64(value));
                    break;
                case DataTypeEnum.Float:
                    result = Write(address, Convert.ToSingle(value));
                    break;
                case DataTypeEnum.Double:
                    result = Write(address, Convert.ToDouble(value));
                    break;
            }
            return result;
        }

        public abstract OperationResult<Dictionary<string, object>> BatchRead(Dictionary<string, DataTypeEnum> addresses);


        public abstract OperationResult BatchWrite(Dictionary<string, object> addresses);


        #endregion

    }
}
