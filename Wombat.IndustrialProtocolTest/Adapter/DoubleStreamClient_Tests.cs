using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wombat.IndustrialProtocol.PLC;
using Xunit;
using Wombat.Infrastructure;
using Wombat.IndustrialProtocol;
using Wombat.Extensions.Adapter;
using Wombat.IndustrialProtocol.PLC.Enums;

namespace Wombat.IndustrialProtocolTest.PLCTests
{
   public class DoubleStreamClient_Tests
    {
        private DoubleStreamClient client;
        private IEthernetClient readClient;
        private IEthernetClient writeClient;

        string ip = "159.75.78.22";

        public DoubleStreamClient_Tests()
        {

        }

        [Fact]
        //[InlineData(MitsubishiVersion.A_1E, 6001)]
        public void 长连接测试()
        {
            readClient = new MitsubishiClient(MitsubishiVersion.Qna_3E, ip, 8000);
            writeClient = new MitsubishiClient(MitsubishiVersion.Qna_3E, ip, 8001);

            client = new DoubleStreamClient(readClient, writeClient);
            client.Connect();

            ReadWrite();
            client.Disconnect();
        }

        private void ReadWrite()
        {
            var t1 = TimeStampHelper.NowLong();
            client.ReadBoolean("M900", 20).Wait();
            client.ReadBoolean("M900", 20).Wait();
             client.ReadBoolean("M900", 20).Wait();
            var t2 = TimeStampHelper.NowLong();


            Random rnd = new Random((int)Stopwatch.GetTimestamp());
            for (int i = 0; i < 10; i++)
            {
                short short_number = (short)rnd.Next(short.MinValue, short.MaxValue);
                int int_number = rnd.Next(int.MinValue, int.MaxValue);
                float float_number = int_number / 100;
                var bool_value = short_number % 2 == 1;

                //client.Write("Y100", true);
                //Assert.True(client.ReadBoolean("Y100").Value == true);
                Task.Run(() => { client.Write("M900", true); }).Wait();
                var r1 = client.ReadBoolean("M900");

                Assert.True(r1.Result.Value == true);

                Assert.True(client.ReadBoolean("M900").Result.Value == true);
                client.Write("M901", bool_value);
                Assert.True(client.ReadBoolean("M901").Result.Value == bool_value);
                client.Write("M902", bool_value);
                Assert.True(client.ReadBoolean("M902").Result.Value == bool_value);
                client.Write("M903", !bool_value);
                Assert.True(client.ReadBoolean("M903").Result.Value == !bool_value);
                client.Write("M904", bool_value);
                Assert.True(client.ReadBoolean("M904").Result.Value == bool_value);



                //client.Write("L100", !bool_value);
                //Assert.True(client.ReadBoolean("L100").Value == !bool_value);
                //client.Write("F100", bool_value);
                //Assert.True(client.ReadBoolean("F100").Value == bool_value);
                //client.Write("V100", !bool_value);
                //Assert.True(client.ReadBoolean("V100").Value == !bool_value);
                //client.Write("B100", bool_value);
                //Assert.True(client.ReadBoolean("B100").Value == bool_value);
                //client.Write("S100", bool_value);
                //Assert.True(client.ReadBoolean("S100").Value == bool_value);

                client.Write("D200", short_number);
                Assert.True(client.ReadInt16("D200").Value == short_number);

                client.Write("D200", int_number);
                Assert.True(client.ReadInt32("D200").Value == int_number);

                client.Write("D200", Convert.ToInt64(int_number));
                Assert.True(client.ReadInt64("D200").Value == Convert.ToInt64(int_number));

                client.Write("D200", float_number);
                Assert.True(client.ReadFloat("D200").Value == float_number);

                client.Write("D200", Convert.ToDouble(float_number));
                Assert.True(client.ReadDouble("D200").Value == Convert.ToDouble(float_number));

                bool[] bool_values = { false, true, false, false, true, false, false, false, false, false
                        , false, false, false,false,false,false,false,false,false, true };

                var sss1 = client.Write("M900", bool_values);
                var bool_values_result = client.ReadBoolean("M900", bool_values.Length);
                for (int j = 0; j < bool_values_result.Result.Value.Length; j++)
                {
                    Assert.True(bool_values_result.Result.Value[j] == bool_values[j]);

                }

                short[] short_values = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
                client.Write("D300", short_values);
                var short_values_result = client.ReadInt16("D300", short_values.Length);
                for (int j = 0; j < short_values_result.Value.Length; j++)
                {
                    Assert.True(short_values_result.Value[j] == short_values[j]);

                }

                ushort[] ushort_values = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
                client.Write("D300", ushort_values);
                var ushort_values_result = client.ReadInt16("D300", ushort_values.Length);
                for (int j = 0; j < ushort_values_result.Value.Length; j++)
                {
                    Assert.True(ushort_values_result.Value[j] == ushort_values[j]);

                }

                int[] int_values = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
                client.Write("D300", int_values);
                var int_values_result = client.ReadInt32("D300", int_values.Length);
                for (int j = 0; j < int_values_result.Value.Length; j++)
                {
                    Assert.True(int_values_result.Value[j] == int_values[j]);

                }

                uint[] uint_values = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
                client.Write("D300", uint_values);
                var uint_values_result = client.ReadUInt32("D300", uint_values.Length);
                for (int j = 0; j < uint_values_result.Value.Length; j++)
                {
                    Assert.True(uint_values_result.Value[j] == uint_values[j]);

                }

                long[] long_values = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
                client.Write("D300", long_values);
                var long_values_result = client.ReadInt64("D300", long_values.Length);
                for (long j = 0; j < long_values_result.Value.Length; j++)
                {
                    Assert.True(long_values_result.Value[j] == long_values[j]);

                }

                ulong[] ulong_values = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
                client.Write("D300", ulong_values);
                var ulong_values_result = client.ReadUInt64("D300", ulong_values.Length);
                for (int j = 0; j < ulong_values_result.Value.Length; j++)
                {
                    Assert.True(ulong_values_result.Value[j] == ulong_values[j]);

                }

                float[] float_values = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
                client.Write("D300", float_values);
                var float_values_result = client.ReadFloat("D300", float_values.Length);
                for (int j = 0; j < float_values_result.Value.Length; j++)
                {
                    Assert.True(float_values_result.Value[j] == float_values[j]);

                }
                double[] double_values = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
                client.Write("D300", double_values);
                var double_values_result = client.ReadDouble("D300", double_values.Length);
                for (int j = 0; j < double_values_result.Value.Length; j++)
                {
                    Assert.True(double_values_result.Value[j] == double_values[j]);

                }
            }
        }

    }
}
