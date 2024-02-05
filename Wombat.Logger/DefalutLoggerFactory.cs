using Microsoft.Extensions.Logging;
using Wombat.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wombat.Logger
{
    public enum SplitTypes
    {
        Default,
        SplitByLogLevel
    }
   public static class DefalutLoggerFactory
    {
        static string LogFilePath(string LogEvent) =>
    $@"{AppDomain.CurrentDomain.BaseDirectory}/AppLogs/{DateTime.Now.Year}/{DateTime.Now.Month}_{DateTime.Now.Day}/{LogEvent}/.log";

        public static ILoggingBuilder AddDefalutFileLogger(this ILoggingBuilder loggingBuilder, LogLevel logLevel = LogLevel.Trace, SplitTypes splitTypes = SplitTypes.Default)
        {
            //var minLevel2 = new Dictionary<string, LogLevel>();
            //minLevel2.Add("Default", LogLevel.Warning);

            loggingBuilder.SetMinimumLevel(logLevel);

            switch (splitTypes)
            {
                case SplitTypes.Default:
                    var minLevel = new Dictionary<string, LogLevel>();
                    minLevel.Add("Default", logLevel);
                    loggingBuilder.AddFile(configure =>
                    {
                        configure.FileAccessMode = LogFileAccessMode.KeepOpenAndAutoFlush;
                        configure.FileEncodingName = "utf-8";
                        configure.DateFormat = "yyyyMMdd";
                        configure.MaxFileSize = 1000000;
                        configure.MaxQueueSize = 100;
                        configure.RootPath = AppContext.BaseDirectory;
                        configure.BasePath = "Logs";
                        configure.CounterFormat = "000";
                        configure.Files = new LogFileOptions[1]{new LogFileOptions()
                        {
                            MaxFileSize = 1000000,
                            MinLevel = minLevel,
                            Path = $"<date:yyyyMMdd>/<counter>.log"
                        } };
                    });

                    break;
                case SplitTypes.SplitByLogLevel:
                    loggingBuilder.AddFile<TraceFileLoggerProvider>(configure: configure =>
                     {

                         configure.FileAccessMode = LogFileAccessMode.KeepOpenAndAutoFlush;
                         configure.FileEncodingName = "utf-8";
                         configure.DateFormat = "yyyyMMdd";
                         configure.MaxFileSize = 1000000;
                         configure.MaxQueueSize = 100;
                         configure.RootPath = AppContext.BaseDirectory;
                         configure.BasePath = "Logs";
                         configure.CounterFormat = "000";
                         configure.Files = new LogFileOptions[1]{ new LogFileOptions()
                        {
                            MaxFileSize = 100000,
                            Path = $"<date:yyyyMMdd>/Trace/<counter>.log"
                         }};
                     });
                    loggingBuilder.AddFile<DebugFileLoggerProvider>(configure: configure =>
                    {

                        configure.FileAccessMode = LogFileAccessMode.KeepOpenAndAutoFlush;
                        configure.FileEncodingName = "utf-8";
                        configure.DateFormat = "yyyyMMdd";
                        configure.MaxFileSize = 1000000;
                        configure.MaxQueueSize = 100;
                        configure.RootPath = AppContext.BaseDirectory;
                        configure.BasePath = "Logs";
                        configure.CounterFormat = "000";
                        configure.Files = new LogFileOptions[1]{ new LogFileOptions()
                        {
                            MaxFileSize = 100000,
                            Path = $"<date:yyyyMMdd>/Debug/<counter>.log"
                         }};
                    });
                    loggingBuilder.AddFile<ErrorFileLoggerProvider>(configure: configure =>
                    {

                        configure.FileAccessMode = LogFileAccessMode.KeepOpenAndAutoFlush;
                        configure.FileEncodingName = "utf-8";
                        configure.DateFormat = "yyyyMMdd";
                        configure.MaxFileSize = 1000000;
                        configure.MaxQueueSize = 100;
                        configure.RootPath = AppContext.BaseDirectory;
                        configure.BasePath = "Logs";
                        configure.CounterFormat = "000";
                        configure.Files = new LogFileOptions[1]{ new LogFileOptions()
                        {
                            MaxFileSize = 100000,
                            Path = $"<date:yyyyMMdd>/Error/<counter>.log"
                         }};
                    });
                    loggingBuilder.AddFile<WarningFileLoggerProvider>(configure: configure =>
                    {

                        configure.FileAccessMode = LogFileAccessMode.KeepOpenAndAutoFlush;
                        configure.FileEncodingName = "utf-8";
                        configure.DateFormat = "yyyyMMdd";
                        configure.MaxFileSize = 1000000;
                        configure.MaxQueueSize = 100;
                        configure.RootPath = AppContext.BaseDirectory;
                        configure.BasePath = "Logs";
                        configure.CounterFormat = "000";
                        configure.Files = new LogFileOptions[1]{ new LogFileOptions()
                        {
                            MaxFileSize = 100000,
                            Path = $"<date:yyyyMMdd>/Warning/<counter>.log"
                         }};
                    });
                    loggingBuilder.AddFile<CriticalFileLoggerProvider>(configure: configure =>
                    {

                        configure.FileAccessMode = LogFileAccessMode.KeepOpenAndAutoFlush;
                        configure.FileEncodingName = "utf-8";
                        configure.DateFormat = "yyyyMMdd";
                        configure.MaxFileSize = 1000000;
                        configure.MaxQueueSize = 100;
                        configure.RootPath = AppContext.BaseDirectory;
                        configure.BasePath = "Logs";
                        configure.CounterFormat = "000";
                        configure.Files = new LogFileOptions[1]{ new LogFileOptions()
                        {
                            MaxFileSize = 100000,
                            Path = $"<date:yyyyMMdd>/Critical/<counter>.log"
                         }};
                    });
                    loggingBuilder.AddFile<InformationFileLoggerProvider>(configure: configure =>
                    {

                        configure.FileAccessMode = LogFileAccessMode.KeepOpenAndAutoFlush;
                        configure.FileEncodingName = "utf-8";
                        configure.DateFormat = "yyyyMMdd";
                        configure.MaxFileSize = 1000000;
                        configure.MaxQueueSize = 100;
                        configure.RootPath = AppContext.BaseDirectory;
                        configure.BasePath = "Logs";
                        configure.CounterFormat = "000";
                        configure.Files = new LogFileOptions[1]{ new LogFileOptions()
                        {
                            MaxFileSize = 100000,
                            Path = $"<date:yyyyMMdd>/Information/<counter>.log"
                         }};
                    });
                    break;
                default:
                    break;
            }
            return loggingBuilder;
        }
    }
}
