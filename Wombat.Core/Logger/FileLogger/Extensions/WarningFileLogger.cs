using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.Logging.File
{
    [ProviderAlias("WarningFile")] // use this alias in appsettings.json to configure this provider
    class WarningFileLoggerProvider : FileLoggerProvider
    {
        public WarningFileLoggerProvider(FileLoggerContext context, IOptionsMonitor<FileLoggerOptions> options, string optionsName) : base(context, options, optionsName) { }

        protected override FileLogger CreateLoggerCore(string categoryName)
        {
            // we instantiate our derived file logger which is modified to log only messages with log level information or below
            return new WarningFileLogger(categoryName, Processor, Settings, GetScopeProvider(), Context.GetTimestamp);
        }
    }

    class WarningFileLogger : FileLogger
    {
        public WarningFileLogger(string categoryName, IFileLoggerProcessor processor, IFileLoggerSettings settings, IExternalScopeProvider scopeProvider = null, Func<DateTimeOffset> timestampGetter = null) 
            : base(categoryName, processor, settings, scopeProvider, timestampGetter) { }

        public override bool IsEnabled(LogLevel logLevel)
        {
            return
                logLevel <= LogLevel.Warning &&  // don't allow messages more severe than information to pass through
                base.IsEnabled(logLevel);
        }
    }
}
