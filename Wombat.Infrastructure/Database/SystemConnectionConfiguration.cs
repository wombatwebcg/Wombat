using Autofac.Annotation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Wombat.Infrastructure;

namespace Wombat.Infrastructure
{

    /// <summary>
    /// 程序配置信息映射类 appsettings.json
    /// </summary>
    /// 
    [AutoConfiguration]
    public class SystemConnectionConfiguration
    {

        [Bean]
        public static ConnectionStringsOptions ConnectionStrings()
        {
            return _connectionStrings;
        }



        [Bean]

        public static InfluxDbOptions InfluxDbConfiguration()
        {
            return _influxDbSetting;
        }




        static ConnectionStringsOptions _connectionStrings;
        static InfluxDbOptions _influxDbSetting;

        /// <summary>
        /// 程序配置信息映射类
        /// </summary>
        /// <param name="configuration"></param>
        /// 
        public SystemConnectionConfiguration (IConfiguration configuration)
        {
            _connectionStrings = configuration.GetSection(nameof(ConnectionStrings)).Get<ConnectionStringsOptions>();
            _influxDbSetting = configuration.GetSection(nameof(InfluxDbConfiguration)).Get<InfluxDbOptions>();
        }

    }


    /// <summary>
    /// InfluxDb配置信息
    /// </summary>
    public class InfluxDbOptions
    {

        /// <summary>
        /// 发行者
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 受众
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 秘钥
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        public int AtInterval { get; set; }
        /// <summary>
        /// 刷新时间
        /// </summary>
        public string DbName { get; set; }
    }



}