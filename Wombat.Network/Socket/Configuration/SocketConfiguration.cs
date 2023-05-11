using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Wombat.Network.Buffer;

namespace Wombat.Network.Socket
{
   public class SocketConfiguration
    {

        /// <summary>
        /// 获取或设置用于管理缓冲区的对象
        /// </summary>
        public ISegmentBufferManager BufferManager { get; set; }

        /// <summary>
        /// 获取或设置接收缓冲区的大小
        /// </summary>
        public int ReceiveBufferSize { get; set; }

        /// <summary>
        /// 获取或设置发送缓冲区的大小
        /// </summary>
        public int SendBufferSize { get; set; }

        /// <summary>
        /// 获取或设置接收操作的超时时间
        /// </summary>
        public TimeSpan ReceiveTimeout { get; set; }

        /// <summary>
        /// 获取或设置发送操作的超时时间
        /// </summary>
        public TimeSpan SendTimeout { get; set; }

        /// <summary>
        /// 获取或设置一个值，该值指示是否禁用 Nagle 算法
        /// </summary>
        public bool NoDelay { get; set; }

        /// <summary>
        /// 获取或设置一个值，该值指示是否启用套接字的延迟关闭
        /// </summary>
        public LingerOption LingerState { get; set; }

        /// <summary>
        /// 获取或设置一个值，该值指示是否启用 TCP keep-alive
        /// </summary>
        public bool KeepAlive { get; set; }

        /// <summary>
        /// 获取或设置 TCP keep-alive 间隔
        /// </summary>
        public TimeSpan KeepAliveInterval { get; set; }

        /// <summary>
        /// 获取或设置一个值，该值指示是否允许多个套接字绑定到同一个端口
        /// </summary>
        public bool ReuseAddress { get; set; }

        /// <summary>
        /// 获取或设置一个值，该值指示是否启用 SSL/TLS 加密
        /// </summary>
        public bool SslEnabled { get; set; }

        /// <summary>
        /// 获取或设置 SSL/TLS 目标主机名
        /// </summary>
        public string SslTargetHost { get; set; }

        /// <summary>
        /// 获取或设置 SSL/TLS 客户端证书集合
        /// </summary>
        public X509CertificateCollection SslClientCertificates { get; set; }

        /// <summary>
        /// 获取或设置 SSL/TLS 加密策略
        /// </summary>
        public EncryptionPolicy SslEncryptionPolicy { get; set; }

        /// <summary>
        /// 获取或设置 SSL/TLS 启用的协议版本
        /// </summary>
        public SslProtocols SslEnabledProtocols { get; set; }

        /// <summary>
        /// 获取或设置一个值，该值指示是否检查证书吊销
        /// </summary>
        public bool SslCheckCertificateRevocation { get; set; }

        /// <summary>
        /// 获取或设置一个值，该值指示是否绕过 SSL/TLS 策略错误
        /// </summary>
        public bool SslPolicyErrorsBypassed { get; set; }

        /// <summary>
        /// 获取或设置连接操作的超时时间
        /// </summary>
        public TimeSpan ConnectTimeout { get; set; }

    }
}
