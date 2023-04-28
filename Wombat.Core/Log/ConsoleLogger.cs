using System;
using System.Collections.Generic;
using System.Text;

namespace Wombat.Core
{
    /// <summary>
    /// 控制台日志记录器
    /// </summary>
   internal class ConsoleLogger
    {

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="logType"></param>
        /// <param name="source"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public  static void WriteLog(LogEventLevel logType, object source, string message, Exception exception)
        {
            lock (typeof(ConsoleLogger))
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffff"));
                Console.Write(" | ");
                switch (logType)
                {
                    case LogEventLevel.Warning:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;

                    case LogEventLevel.Error:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;

                    case LogEventLevel.Fatal:
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        break;

                    case LogEventLevel.Info:
                        Console.ForegroundColor = ConsoleColor.Blue;
                        break;
                    default:
                        Console.ForegroundColor = Console.ForegroundColor;
                        break;
                }
                Console.Write(logType.ToString());
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(" | ");
                Console.Write(message);

                if (exception != null)
                {
                    Console.Write(" | ");
                    Console.Write($"【异常消息】：{exception.Message}");
                    Console.Write($"【堆栈】：{(exception == null ? "未知" : exception.StackTrace)}");
                }
                Console.WriteLine();
            }
        }
    }
}
