
using System;
using System.IO;
using System.Text;

namespace Wombat.Core
{
    /// <summary>
    /// 文件日志记录器
    /// <para>会在指定目录下，生成logs文件夹，然后按[yyyy-MM-dd].log的形式，每日生成日志</para>
    /// </summary>
    internal  class FileLogger 
    {
        private static string _rootPath;


        public static void Initialization(string rootPath = null)
        {
            lock (typeof(FileLogger))
            {
                rootPath = _rootPath == null ? AppDomain.CurrentDomain.BaseDirectory : _rootPath;

                if (_rootPath.IsNullOrEmpty())
                {
                    _rootPath = Path.Combine(rootPath, "logs");
                }
                else if (_rootPath != Path.Combine(rootPath, "logs"))
                {
                    throw new Exception($"{_rootPath.GetType()}无法指向不同的根路径。");
                }
            }

        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="logType"></param>
        /// <param name="source"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public static void WriteLog(LogEventLevel logType, object source, string message, Exception exception)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffff"));
            stringBuilder.Append(" | ");
            stringBuilder.Append(logType.ToString());
            stringBuilder.Append(" | ");
            stringBuilder.Append(message);

            if (exception != null)
            {
                stringBuilder.Append(" | ");
                stringBuilder.Append($"【异常消息】：{exception.Message}");
                stringBuilder.Append($"【堆栈】：{(exception == null ? "未知" : exception.StackTrace)}");
            }
            stringBuilder.AppendLine();

            Print(stringBuilder.ToString());
        }

        private static FileStorageWriter _writer;

        private static void Print(string logString)
        {
            try
            {
                lock (typeof(FileLogger))
                {

                    string dir = Path.Combine(_rootPath, DateTime.Now.ToString("[yyyy-MM-dd]"));
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    if (_writer == null)
                    {
                        int count = 0;
                        string path = null;
                        while (true)
                        {
                            path = Path.Combine(dir, $"{count:0000}" + ".log");
                            if (!File.Exists(path))
                            {
                                _writer = FilePool.GetWriter(path);
                                break;
                            }
                            count++;
                        }
                    }
                    _writer.Write(Encoding.UTF8.GetBytes(logString));
                    if (_writer.FileStorage.Length > 1024 * 1024)
                    {
                        _writer.Dispose();
                        _writer = null;
                    }
                }
            }
            catch
            {
                _writer.Dispose();
                _writer = null;
            }
        }
    }
}