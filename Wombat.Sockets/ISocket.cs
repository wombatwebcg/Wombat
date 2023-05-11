using System;
using System.Collections.Generic;
using System.Text;
using Wombat.Core;

namespace Wombat.Socket
{
    /// <summary>
    /// Socket基接口
    /// </summary>
    public interface ISocket : IDisposable
    {
        /// <summary>
        /// 数据交互缓存池限制
        /// </summary>
        int BufferLength { get; }

        /// <summary>
        /// 日志记录器
        /// </summary>
        ILog Logger { get; }
    }
}
