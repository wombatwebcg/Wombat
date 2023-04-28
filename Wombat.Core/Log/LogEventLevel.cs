using System;
using System.Collections.Generic;
using System.Text;

namespace Wombat.Core
{
    /// <summary>
    /// 日志类型。
    /// </summary>
    public enum LogEventLevel
    {
        /// <summary>
        /// 不使用日志类输出
        /// </summary>
        None = 0,

        /// <summary>
        /// 调试信息日志
        /// </summary>
        Debug = 1,


        /// <summary>
        /// 详细步骤日志输出
        /// </summary>
        Trace = 2,


        /// <summary>
        /// 消息类日志输出
        /// </summary>
        Info = 4,

        /// <summary>
        /// 警告类日志输出
        /// </summary>
        Warning = 8,

        /// <summary>
        /// 错误类日志输出
        /// </summary>
        Error = 16,


        Fatal = 32
    }
}
