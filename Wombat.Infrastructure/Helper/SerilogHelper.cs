using Serilog;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wombat.Infrastructure
{
    /// <summary>
    /// 日志工具
    /// </summary>
    public static class LogHelper
    {
        static string LogFilePath(string LogEvent) =>
            $@"{AppDomain.CurrentDomain.BaseDirectory}/AppLogs/{DateTime.Now.Year}/{DateTime.Now.Month}_{DateTime.Now.Day}/{LogEvent}/.log";

        static object _instance = null;
        /// <summary>
        /// 启动
        /// </summary>
        public static void Build()
        {
            if (_instance == null)
            {
                _instance = new object();
                var now = DateTime.Now;
                var logger = new LoggerConfiguration()
                    .Enrich.With(new DateTimeNowEnricher())
                    .MinimumLevel.Debug()//最小记录级别
                    .Enrich.FromLogContext()//记录相关上下文信息 
                    .MinimumLevel.Override(nameof(Microsoft), LogEventLevel.Debug)//对其他日志进行重写,除此之外,目前框架只有微软自带的日志组件
                    .WriteTo.Console();

                Serilog.Log.Logger = logger
                    .WriteTo.Logger(lg =>
                    {
                        lg.Filter
                        .ByIncludingOnly(p => p.Level == LogEventLevel.Debug)
                        .WriteTo.File(LogFilePath(nameof(LogEventLevel.Debug)), rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true);
                    })
                    .WriteTo.Logger(lg =>
                    {
                        lg.Filter
                        .ByIncludingOnly(p => p.Level == LogEventLevel.Information)
                        .WriteTo.File(LogFilePath(nameof(LogEventLevel.Information)), rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true);
                    })
                    .WriteTo.Logger(lg =>
                    {
                        lg.Filter
                        .ByIncludingOnly(p => p.Level == LogEventLevel.Warning)
                        .WriteTo.File(LogFilePath(nameof(LogEventLevel.Warning)), rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true);
                    })
                    .WriteTo.Logger(lg =>
                    {
                        lg.Filter
                        .ByIncludingOnly(p => p.Level == LogEventLevel.Error)
                        .WriteTo.File(LogFilePath(nameof(LogEventLevel.Error)), rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true);
                    })
                    .WriteTo.Logger(lg =>
                    {
                        lg.Filter
                        .ByIncludingOnly(p => p.Level == LogEventLevel.Fatal)
                        .WriteTo.File(LogFilePath(nameof(LogEventLevel.Fatal)), rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true);
                    })
                    .CreateLogger();
            }
        }





        /// <summary>
        /// 日志对象
        /// </summary>
        public static ILogger Log => Serilog.Log.Logger;

        public static void LogException(Exception ex)
        {
            Log?.Error(ex.Message);
            Log?.Error(ex.Source);
            Log?.Error(ex.StackTrace);

        }
    }

    /// <summary>
    /// 时间处理
    /// </summary>
    class DateTimeNowEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
                "DateTimeNow", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
        }
    }

}