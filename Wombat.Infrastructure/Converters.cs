using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Wombat.Infrastructure
{
   public  static class Converters
    {
        /// <summary>
        /// Bytes to hexadecimal.
        /// </summary>
        /// <param name="bytes">The byte array.</param>
        /// <param name="separator">The separator,default is space</param>
        /// <returns>string of hex.</returns>
        public static string ToHexString(this byte[] bytes, string separator = " ")
        {
            if (bytes == null || bytes.Length < 1)
            {
                return null;
            }

            List<string> list = new List<string>();
            foreach (byte b in bytes)
            {
                list.Add(b.ToString("X2"));
            }

            return string.Join(separator, list.ToArray());
        }

        /// <summary>
        /// Hexadecimal string to an byte array.
        /// </summary>
        /// <param name="hex">The hex string.</param>
        /// <returns>An byte array.</returns>
        public static byte[] HexToByte(this string hex)
        {
            // remove space
            hex = hex.Replace(" ", "");

            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < hex.Length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }





        #region Hex string and Byte[] transform


        /// <summary>
        /// 字节数据转化成16进制表示的字符串 ->
        /// Byte data into a string of 16 binary representations
        /// </summary>
        /// <param name="InBytes">字节数组</param>
        /// <returns>返回的字符串</returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <example>
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="ByteToHexStringExample1" title="ByteToHexString示例" />
        /// </example>
        public static string ByteToHexString(byte[] InBytes)
        {
            return ByteToHexString(InBytes, (char)0);
        }


        /// <summary>
        /// 字节数据转化成16进制表示的字符串 ->
        /// Byte data into a string of 16 binary representations
        /// </summary>
        /// <param name="InBytes">字节数组</param>
        /// <returns>返回的字符串</returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <example>
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="ByteToHexStringExample1" title="ByteToHexString示例" />
        /// </example>
        public static string ByteToString(byte[] InBytes)
        {
            return Encoding.UTF8.GetString(InBytes);
        }



        /// <summary>
        /// 字节数据转化成16进制表示的字符串 ->
        /// Byte data into a string of 16 binary representations
        /// </summary>
        /// <param name="InBytes">字节数组</param>
        /// <param name="segment">分割符</param>
        /// <returns>返回的字符串</returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <example>
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="ByteToHexStringExample2" title="ByteToHexString示例" />
        /// </example>
        public static string ByteToHexString(byte[] InBytes, char segment)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte InByte in InBytes)
            {
                if (segment == 0) sb.Append(string.Format("{0:X2}", InByte));
                else sb.Append(string.Format("{0:X2}{1}", InByte, segment));
            }

            if (segment != 0 && sb.Length > 1 && sb[sb.Length - 1] == segment)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            return sb.ToString();
        }



        /// <summary>
        /// 字符串数据转化成16进制表示的字符串 ->
        /// string data into a string of 16 binary representations
        /// </summary>
        /// <param name="InString">输入的字符串数据</param>
        /// <returns>返回的字符串</returns>
        /// <exception cref="NullReferenceException"></exception>
        public static string ByteToHexString(string InString)
        {
            return ByteToHexString(Encoding.Unicode.GetBytes(InString));
        }


        private static List<char> hexCharList = new List<char>()
            {
                '0','1','2','3','4','5','6','7','8','9','A','B','C','D','E','F'
            };

        /// <summary>
        /// 将16进制的字符串转化成Byte数据，将检测每2个字符转化，也就是说，中间可以是任意字符 ->
        /// Converts a 16-character string into byte data, which will detect every 2 characters converted, that is, the middle can be any character
        /// </summary>
        /// <param name="hex">十六进制的字符串，中间可以是任意的分隔符</param>
        /// <returns>转换后的字节数组</returns>
        /// <remarks>参数举例：AA 01 34 A8</remarks>
        /// <example>
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="HexStringToBytesExample" title="HexStringToBytes示例" />
        /// </example>
        public static byte[] HexStringToBytes(string hex)
        {
            hex = hex.ToUpper();

            MemoryStream ms = new MemoryStream();

            for (int i = 0; i < hex.Length; i++)
            {
                if ((i + 1) < hex.Length)
                {
                    if (hexCharList.Contains(hex[i]) && hexCharList.Contains(hex[i + 1]))
                    {
                        // 这是一个合格的字节数据
                        ms.WriteByte((byte)(hexCharList.IndexOf(hex[i]) * 16 + hexCharList.IndexOf(hex[i + 1])));
                        i++;
                    }
                }
            }

            byte[] result = ms.ToArray();
            ms.Dispose();
            return result;
        }

        #endregion

        #region Byte Reverse By Word

        /// <summary>
        /// 将byte数组按照双字节进行反转，如果为单数的情况，则自动补齐 ->
        /// Reverses the byte array by double byte, or if the singular is the case, automatically
        /// </summary>
        /// <param name="inBytes">输入的字节信息</param>
        /// <returns>反转后的数据</returns>
        /// <example>
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="BytesReverseByWord" title="BytesReverseByWord示例" />
        /// </example>
        //public static byte[] BytesReverseByWord(byte[] inBytes)
        //{
        //    if (inBytes == null) return null;
        //    byte[] buffer = ArrayExpandToLengthEven(inBytes);

        //    for (int i = 0; i < buffer.Length / 2; i++)
        //    {
        //        byte tmp = buffer[i * 2 + 0];
        //        buffer[i * 2 + 0] = buffer[i * 2 + 1];
        //        buffer[i * 2 + 1] = tmp;
        //    }

        //    return buffer;
        //}

        #endregion

        #region Byte[] and AsciiByte[] transform

        /// <summary>
        /// 将原始的byte数组转换成ascii格式的byte数组 ->
        /// Converts the original byte array to an ASCII-formatted byte array
        /// </summary>
        /// <param name="inBytes">等待转换的byte数组</param>
        /// <returns>转换后的数组</returns>
        public static byte[] BytesToAsciiBytes(byte[] inBytes)
        {
            return Encoding.ASCII.GetBytes(ByteToHexString(inBytes));
        }

        /// <summary>
        /// 将ascii格式的byte数组转换成原始的byte数组 ->
        /// Converts an ASCII-formatted byte array to the original byte array
        /// </summary>
        /// <param name="inBytes">等待转换的byte数组</param>
        /// <returns>转换后的数组</returns>
        public static byte[] AsciiBytesToBytes(byte[] inBytes)
        {
            return HexStringToBytes(Encoding.ASCII.GetString(inBytes));
        }

        /// <summary>
        /// 从字节构建一个ASCII格式的数据内容
        /// </summary>
        /// <param name="value">数据</param>
        /// <returns>ASCII格式的字节数组</returns>
        public static byte[] BuildAsciiBytesFrom(byte value)
        {
            return Encoding.ASCII.GetBytes(value.ToString("X2"));
        }

        /// <summary>
        /// 从short构建一个ASCII格式的数据内容
        /// </summary>
        /// <param name="value">数据</param>
        /// <returns>ASCII格式的字节数组</returns>
        public static byte[] BuildAsciiBytesFrom(short value)
        {
            return Encoding.ASCII.GetBytes(value.ToString("X4"));
        }

        /// <summary>
        /// 从ushort构建一个ASCII格式的数据内容
        /// </summary>
        /// <param name="value">数据</param>
        /// <returns>ASCII格式的字节数组</returns>
        public static byte[] BuildAsciiBytesFrom(ushort value)
        {
            return Encoding.ASCII.GetBytes(value.ToString("X4"));
        }

        /// <summary>
        /// 从字节数组构建一个ASCII格式的数据内容
        /// </summary>
        /// <param name="value">字节信息</param>
        /// <returns>ASCII格式的地址</returns>
        //public static byte[] BuildAsciiBytesFrom(byte[] value)
        //{
        //    byte[] buffer = new byte[value.Length * 2];
        //    for (int i = 0; i < value.Length; i++)
        //    {
        //        SoftBasic.BuildAsciiBytesFrom(value[i]).CopyTo(buffer, 2 * i);
        //    }
        //    return buffer;
        //}

        #endregion

        #region Bool[] and byte[] transform

        /// <summary>
        /// 将bool数组转换到byte数组 ->
        /// Converting a bool array to a byte array
        /// </summary>
        /// <param name="array">bool数组</param>
        /// <returns>转换后的字节数组</returns>
        /// <example>
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="BoolArrayToByte" title="BoolArrayToByte示例" />
        /// </example>
        public static byte[] BoolArrayToByte(bool[] array)
        {
            if (array == null) return null;

            int length = array.Length % 8 == 0 ? array.Length / 8 : array.Length / 8 + 1;
            byte[] buffer = new byte[length];

            for (int i = 0; i < array.Length; i++)
            {
                int index = i / 8;
                int offect = i % 8;

                byte temp = 0;
                switch (offect)
                {
                    case 0: temp = 0x01; break;
                    case 1: temp = 0x02; break;
                    case 2: temp = 0x04; break;
                    case 3: temp = 0x08; break;
                    case 4: temp = 0x10; break;
                    case 5: temp = 0x20; break;
                    case 6: temp = 0x40; break;
                    case 7: temp = 0x80; break;
                    default: break;
                }

                if (array[i]) buffer[index] += temp;
            }

            return buffer;
        }

        /// <summary>
        /// 从Byte数组中提取位数组，length代表位数 ->
        /// Extracts a bit array from a byte array, length represents the number of digits
        /// </summary>
        /// <param name="InBytes">原先的字节数组</param>
        /// <param name="length">想要转换的长度，如果超出自动会缩小到数组最大长度</param>
        /// <returns>转换后的bool数组</returns>
        /// <example>
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="ByteToBoolArray" title="ByteToBoolArray示例" />
        /// </example> 
        public static bool[] ByteToBoolArray(byte[] InBytes, int length)
        {
            if (InBytes == null) return null;

            if (length > InBytes.Length * 8) length = InBytes.Length * 8;
            bool[] buffer = new bool[length];

            for (int i = 0; i < length; i++)
            {
                int index = i / 8;
                int offect = i % 8;

                byte temp = 0;
                switch (offect)
                {
                    case 0: temp = 0x01; break;
                    case 1: temp = 0x02; break;
                    case 2: temp = 0x04; break;
                    case 3: temp = 0x08; break;
                    case 4: temp = 0x10; break;
                    case 5: temp = 0x20; break;
                    case 6: temp = 0x40; break;
                    case 7: temp = 0x80; break;
                    default: break;
                }

                if ((InBytes[index] & temp) == temp)
                {
                    buffer[i] = true;
                }
            }

            return buffer;
        }

        /// <summary>
        /// 从Byte数组中提取所有的位数组 ->
        /// Extracts a bit array from a byte array, length represents the number of digits
        /// </summary>
        /// <param name="InBytes">原先的字节数组</param>
        /// <returns>转换后的bool数组</returns>
        /// <example>
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="ByteToBoolArray" title="ByteToBoolArray示例" />
        /// </example> 
        public static bool[] ByteToBoolArray(byte[] InBytes)
        {
            if (InBytes == null) return null;

            return ByteToBoolArray(InBytes, InBytes.Length * 8);
        }

        #endregion

        #region Byte[] Splice

        /// <summary>
        /// 拼接2个字节数组成一个数组 ->
        /// Splicing 2 bytes to to an array
        /// </summary>
        /// <param name="bytes1">数组一</param>
        /// <param name="bytes2">数组二</param>
        /// <returns>拼接后的数组</returns>
        /// <example>
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="SpliceTwoByteArray" title="SpliceTwoByteArray示例" />
        /// </example> 
        public static byte[] SpliceTwoByteArray(byte[] bytes1, byte[] bytes2)
        {
            if (bytes1 == null && bytes2 == null) return null;
            if (bytes1 == null) return bytes2;
            if (bytes2 == null) return bytes1;

            byte[] buffer = new byte[bytes1.Length + bytes2.Length];
            bytes1.CopyTo(buffer, 0);
            bytes2.CopyTo(buffer, bytes1.Length);
            return buffer;
        }

        /// <summary>
        /// 选择一个byte数组的前面的几个byte数据信息
        /// </summary>
        /// <param name="value">原始的数据信息</param>
        /// <param name="length">数据的长度</param>
        /// <returns>选择的前面的几个数据信息</returns>
        public static byte[] BytesArraySelectBegin(byte[] value, int length)
        {
            byte[] buffer = new byte[Math.Min(value.Length, length)];
            Array.Copy(value, 0, buffer, 0, buffer.Length);
            return buffer;
        }

        /// <summary>
        /// 将一个byte数组的前面指定位数移除，返回新的一个数组 ->
        /// Removes the preceding specified number of bits in a byte array, returning a new array
        /// </summary>
        /// <param name="value">字节数组</param>
        /// <param name="length">等待移除的长度</param>
        /// <returns>新的数据</returns>
        /// <example>
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="BytesArrayRemoveBegin" title="BytesArrayRemoveBegin示例" />
        /// </example> 
        public static byte[] BytesArrayRemoveBegin(byte[] value, int length)
        {
            return BytesArrayRemoveDouble(value, length, 0);
        }

        /// <summary>
        /// 将一个byte数组的后面指定位数移除，返回新的一个数组 ->
        /// Removes the specified number of digits after a byte array, returning a new array
        /// </summary>
        /// <param name="value">字节数组</param>
        /// <param name="length">等待移除的长度</param>
        /// <returns>新的数据</returns>
        /// <example>
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="BytesArrayRemoveLast" title="BytesArrayRemoveLast示例" />
        /// </example> 
        public static byte[] BytesArrayRemoveLast(byte[] value, int length)
        {
            return BytesArrayRemoveDouble(value, 0, length);
        }

        /// <summary>
        /// 将一个byte数组的前后移除指定位数，返回新的一个数组 ->
        /// Removes a byte array before and after the specified number of bits, returning a new array
        /// </summary>
        /// <param name="value">字节数组</param>
        /// <param name="leftLength">前面的位数</param>
        /// <param name="rightLength">后面的位数</param>
        /// <returns>新的数据</returns>
        /// <example>
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="BytesArrayRemoveDouble" title="BytesArrayRemoveDouble示例" />
        /// </example> 
        public static byte[] BytesArrayRemoveDouble(byte[] value, int leftLength, int rightLength)
        {
            if (value == null) return null;
            if (value.Length <= (leftLength + rightLength)) return new byte[0];

            byte[] buffer = new byte[value.Length - leftLength - rightLength];
            Array.Copy(value, leftLength, buffer, 0, buffer.Length);

            return buffer;
        }

        #endregion

    }
}
