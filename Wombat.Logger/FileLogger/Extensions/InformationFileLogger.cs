using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Wombat.Logger
{
    [ProviderAlias("InformationFile")] // use this alias in appsettings.json to configure this provider
    class InformationFileLoggerProvider : FileLoggerProvider
    {
        public InformationFileLoggerProvider(FileLoggerContext context, IOptionsMonitor<FileLoggerOptions> options, string optionsName) : base(context, options, optionsName) { }

        protected override FileLogger CreateLoggerCore(string categoryName)
        {
            // we instantiate our derived file logger which is modified to log only messages with log level Informationrmation or below
            return new InformationFileLogger(categoryName, Processor, Settings, GetScopeProvider(), Context.GetTimestamp);
        }
    }

    class InformationFileLogger : FileLogger
    {
        public InformationFileLogger(string categoryName, IFileLoggerProcessor processor, IFileLoggerSettings settings, IExternalScopeProvider scopeProvider = null, Func<DateTimeOffset> timestampGetter = null) 
            : base(categoryName, processor, settings, scopeProvider, timestampGetter) { }

        public override bool IsEnabled(LogLevel logLevel)
        {
            return
                logLevel <= LogLevel.Information &&  // don't allow messages more severe than Informationrmation to pass through
                base.IsEnabled(logLevel);
        }
    }
}
