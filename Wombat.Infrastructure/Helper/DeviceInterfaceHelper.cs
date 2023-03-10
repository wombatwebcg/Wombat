using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace Wombat.Infrastructure
{
    public static class DeviceInterfaceHelper
    {

        public static bool CheckSerialPort(string portName)
        {
            if (portName == null) return false;
            var ports = SerialPort.GetPortNames();
            foreach (var p in ports)
            {
                if (p.ToLower() == portName.ToLower())
                {
                    return true;
                }
            }
            return false;

        }


        public static bool PingIp(string strIP)
        {
            if (strIP == null) return false;

            bool bRet = false;
            try
            {
                Ping pingSend = new Ping();
                PingReply reply = pingSend.Send(strIP, 30);
                if (reply.Status == IPStatus.Success)
                    bRet = true;
            }
            catch (Exception)
            {
                bRet = false;
            }
            return bRet;
        }

        /// <summary>
        /// 是否连接到服务器,true表示连接成功,false表示连接失败
        /// </summary>
        /// <param name="strIpOrDName">输入参数,表示IP地址或域名</param>
        /// <returns></returns>
        public static bool PingIpOrDomainName(string strIpOrDName)
        {
            if (strIpOrDName == null) return false;

            try
            {
                Ping objPingSender = new Ping();

                PingOptions objPinOptions = new PingOptions();

                objPinOptions.DontFragment = true;

                string data = "";

                byte[] buffer = Encoding.UTF8.GetBytes(data);

                int intTimeout = 120;

                PingReply objPinReply = objPingSender.Send(strIpOrDName, intTimeout, buffer, objPinOptions);

                string strInfo = objPinReply.Status.ToString();

                if (strInfo == "Success")
                {
                    return true;
                }

                else
                {
                    return false;
                }

            }

            catch (Exception)
            {

                return false;

            }

        }

        /// <summary>
        /// 安全关闭
        /// </summary>
        /// <param name="socket"></param>
        public static void SafeClose(this Socket socket)
        {
            try
            {
                if (socket?.Connected ?? false) socket?.Shutdown(SocketShutdown.Both);//正常关闭连接
            }
            catch { }

            try
            {
                socket?.Close();
            }
            catch { }
        }


    }
}
