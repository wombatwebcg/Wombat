
using System;

namespace Wombat.Core
{
    public static partial class LoggerExtensions
    {

        public static void Debug<TLog>(this ILog logger, string msg) where TLog : ILog
        {
            logger.Log<TLog>(LogEventLevel.Debug, null, msg, null);
        }

        public static void Error<TLog>(this ILog logger, string msg) where TLog : ILog
        {
            logger.Log<TLog>(LogEventLevel.Error, null, msg, null);
        }

        public static void Error<TLog>(this ILog logger, object source, string msg) where TLog : ILog
        {
            logger.Log<TLog>(LogEventLevel.Error, source, msg, null);
        }

        public static void Exception<TLog>(this ILog logger, Exception ex) where TLog : ILog
        {
            logger.Log<TLog>(LogEventLevel.Error, null, ex.Message, ex);
        }

        public static void Exception<TLog>(this ILog logger, object source, Exception ex) where TLog : ILog
        {
            logger.Log<TLog>(LogEventLevel.Error, source, ex.Message, ex);
        }

        public static void Info<TLog>(this ILog logger, string msg) where TLog : ILog
        {
            logger.Log<TLog>(LogEventLevel.Info, null, msg, null);
        }

        public static void Info<TLog>(this ILog logger, object source, string msg) where TLog : ILog
        {
            logger.Log<TLog>(LogEventLevel.Info, source, msg, null);
        }

        public static void Log<TLog>(this ILog logger, LogEventLevel logType, object source, string message, Exception exception) where TLog : ILog
        {
            logger.Log<TLog>(logType, source, message, exception);
        }

        /// <summary>
        /// 指定在<see cref="LoggerGroup"/>中的特定日志类型中输出详细日志
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="msg"></param>
        public static void Trace<TLog>(this ILog logger, string msg) where TLog : ILog
        {
            logger.Log<TLog>(LogEventLevel.Trace, null, msg, null);
        }

        /// <summary>
        /// 指定在<see cref="LoggerGroup"/>中的特定日志类型中输出警示日志
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="msg"></param>
        public static void Warning<TLog>(this ILog logger, string msg) where TLog : ILog
        {
            logger.Log<TLog>(LogEventLevel.Warning, null, msg, null);
        }

        /// <summary>
        /// 指定在<see cref="LoggerGroup"/>中的特定日志类型中输出警示日志
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="source"></param>
        /// <param name="msg"></param>
        public static void Warning<TLog>(this ILog logger, object source, string msg) where TLog : ILog
        {
            logger.Log<TLog>(LogEventLevel.Warning, source, msg, null);
        }


        #region 日志

        /// <summary>
        /// 输出调试日志
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="msg"></param>
        public static void Debug(this ILog logger, string msg)
        {
            logger.Log(LogEventLevel.Debug, null, msg, null);
        }

        /// <summary>
        /// 输出错误日志
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="msg"></param>
        public static void Error(this ILog logger, string msg)
        {
            logger.Log(LogEventLevel.Error, null, msg, null);
        }

        /// <summary>
        /// 输出错误日志
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="source"></param>
        /// <param name="msg"></param>
        public static void Error(this ILog logger, object source, string msg)
        {
            logger.Log(LogEventLevel.Error, source, msg, null);
        }

        /// <summary>
        /// 输出异常日志
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="ex"></param>
        public static void Exception(this ILog logger, Exception ex)
        {
            logger.Log(LogEventLevel.Error, null, ex.Message, ex);
        }

        /// <summary>
        /// 输出异常日志
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="source"></param>
        /// <param name="ex"></param>
        public static void Exception(this ILog logger, object source, Exception ex)
        {
            logger.Log(LogEventLevel.Error, source, ex.Message, ex);
        }

        /// <summary>
        /// 输出消息日志
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="msg"></param>
        public static void Info(this ILog logger, string msg)
        {
            logger.Log(LogEventLevel.Info, null, msg, null);
        }

        /// <summary>
        /// 输出消息日志
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="source"></param>
        /// <param name="msg"></param>
        public static void Info(this ILog logger, object source, string msg)
        {
            logger.Log(LogEventLevel.Info, source, msg, null);
        }

        /// <summary>
        /// 输出详细日志
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="msg"></param>
        public static void Trace(this ILog logger, string msg)
        {
            logger.Log(LogEventLevel.Trace, null, msg, null);
        }

        /// <summary>
        /// 输出警示日志
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="msg"></param>
        public static void Warning(this ILog logger, string msg)
        {
            logger.Log(LogEventLevel.Warning, null, msg, null);
        }

        /// <summary>
        /// 输出警示日志
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="source"></param>
        /// <param name="msg"></param>
        public static void Warning(this ILog logger, object source, string msg)
        {
            logger.Log(LogEventLevel.Warning, source, msg, null);
        }

        #endregion 日志
    }
}