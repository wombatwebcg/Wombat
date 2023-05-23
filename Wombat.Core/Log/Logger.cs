using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Wombat.Core
{
    public sealed class Logger : ILog
    {
         LogEventLevel _minimumLevel = LogEventLevel.None;
        bool _isUseConsoleLogger = false;
        bool _isUseFileLogger = false;
        LogFileTypes _logFileType = LogFileTypes.Single;
        RollingInterval _logRollingInterval = RollingInterval.Infinite;
        int _logFileSize = int.MaxValue;

        public Logger(LogEventLevel minimumLevel= LogEventLevel.None)
        {
            _minimumLevel = minimumLevel;
        }

        public LogEventLevel MinimumLevel
        {
            get { return _minimumLevel; }
            set { _minimumLevel = value; }
        }

        public void UseConsoleLogger(bool isUseConsoleLogger = true)
        {
            _isUseConsoleLogger = isUseConsoleLogger;
        }


        public void UseFileLogger(bool isUseFileLogger)
        {
            _isUseFileLogger = isUseFileLogger;
            if (_isUseFileLogger) FileLogger.Initialization();
            //_logFileType = logFileType;
            //_logFileSize = logFileSize;
            //_logRollingInterval = logRollingInterval;

        }

        public LogFileTypes LogFileType
        {
            get { return _logFileType; }
            set { _logFileType = value; }

        }

        public RollingInterval LogRollingInterval
        {
            get { return _logRollingInterval; }
            set {_logRollingInterval = value; }

        }



        public void Log(LogEventLevel logType, object source, string message, Exception exception)
        {
            if (IsEnabled(logType))
            {
                WriteLog(logType, source, message, exception);
            }
        }


        public Task LogAsync(LogEventLevel logType, object source, string message,Exception exception)
        {
            return Task.Run(() =>
            {
                Log(logType, source, message, exception);
            });
        }


        public bool IsEnabled(LogEventLevel level)
        {
            return level>=_minimumLevel;
        }


        public  void WriteLog(LogEventLevel logType, object source, string message, Exception exception)
        {
            if(_isUseConsoleLogger)
            {
                ConsoleLogger.WriteLog(logType,source,message,exception);
            }
            if(_isUseFileLogger)
            {
                FileLogger.WriteLog(logType, source, message, exception);
            }
        }



    }
}
