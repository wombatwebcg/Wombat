using System;
using System.Collections.Generic;
using System.Text;

namespace Wombat.Core
{
    public class LoggerBuilder
    {
        bool _isUseConsoleLogger = false;
        bool _isUseFileLogger = false;
        LogEventLevel _minimumLevel;
        public LoggerBuilder LogEventLevel(LogEventLevel minimumLevel)
        {
            _minimumLevel = minimumLevel;
            return this;
        }


        public LoggerBuilder UseConsoleLogger(bool isUseConsoleLogger = true)
        {
            _isUseConsoleLogger = isUseConsoleLogger;

            return this;
        }


        public LoggerBuilder UseFileLogger(bool isUseFileLogger = true)
        {
            _isUseFileLogger = isUseFileLogger;
            return this;

        }


        public Logger CreateLogger()
        {
            Logger logger = new Logger();
            logger.MinimumLevel = _minimumLevel;
            logger.UseConsoleLogger(_isUseConsoleLogger);
            logger.UseFileLogger(_isUseFileLogger);
            return logger;
        }
    }
}
