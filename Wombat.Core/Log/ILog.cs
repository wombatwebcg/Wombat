using System;
using System.Collections.Generic;
using System.Text;

namespace Wombat.Core
{

    public interface ILog
    {
        /// <summary>
        /// 日志输出类型。
        /// 当<see cref="Log(LogEventLevel, object, string, Exception)"/>的类型，在该设置之内时，才会真正输出日志。
        /// </summary>
        LogEventLevel MinimumLevel { get; set; }

        /// <summary>
        /// 日志记录
        /// </summary>
        /// <param name="logType"></param>
        /// <param name="source"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        void Log(LogEventLevel logType, object source, string message, Exception exception);


        void UseConsoleLogger(bool isUseConsoleLogger = true);

        void UseFileLogger(bool isUseFileLogger);


    }
}
