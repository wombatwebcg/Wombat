using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.File;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.Logging.File
{
    [ProviderAlias("TraceFile")] // use this alias in appsettings.json to configure this provider
   class TraceFileLoggerProvider : FileLoggerProvider
    {
        public TraceFileLoggerProvider(FileLoggerContext context, IOptionsMonitor<FileLoggerOptions> options, string optionsName) : base(context, options, optionsName) { }

        protected override FileLogger CreateLoggerCore(string categoryName)
        {
            // we instantiate our derived file logger which is modified to log only messages with log level information or below
            return new TraceFileLogger(categoryName, Processor, Settings, GetScopeProvider(), Context.GetTimestamp);
        }
    }

   class TraceFileLogger : FileLogger
    {
        public TraceFileLogger(string categoryName, IFileLoggerProcessor processor, IFileLoggerSettings settings, IExternalScopeProvider scopeProvider = null, Func<DateTimeOffset> timestampGetter = null) 
            : base(categoryName, processor, settings, scopeProvider, timestampGetter) { }

        public override bool IsEnabled(LogLevel logLevel)
        {
            return
                logLevel <= LogLevel.Trace &&  // don't allow messages more severe than information to pass through
                base.IsEnabled(logLevel);
        }
    }
}
