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
       static Logger _logger;
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
            lock(this)
            if (_logger == null)
            {
                _logger = new Logger();
            }
            _logger.MinimumLevel = _minimumLevel;
            _logger.UseConsoleLogger(_isUseConsoleLogger);
            _logger.UseFileLogger(_isUseFileLogger);
            return _logger;
        }
    }
}
