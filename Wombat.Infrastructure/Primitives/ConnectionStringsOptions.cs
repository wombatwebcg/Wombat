using System;
using System.Collections.Generic;
using System.Text;

namespace Wombat.Infrastructure
{
    /// <summary>
    /// 连接字符串配置
    /// </summary>
    public class ConnectionStringsOptions
    {
        /// <summary>
        /// 默认数据库类型
        /// </summary>
        /// <value></value>
        public DefaultDatabaseType DefaultDatabaseType { get; set; }
        /// <summary>
        /// sqlserver
        /// </summary>
        /// <value></value>
        public string DefaultSqlServer { get; set; }
        /// <summary>
        /// mysql
        /// </summary>
        /// <value></value>
        public string DefaultMySql { get; set; }
        /// <summary>
        /// PostgreSql
        /// </summary>
        /// <value></value>
        public string DefaultPostgreSql { get; set; }

        /// <summary>
        /// PostgreSql
        /// </summary>
        /// <value></value>
        public string DefaultSqlite { get; set; }

        /// <summary>
        /// redis 地址
        /// </summary>
        /// <value></value>
        public string Redis { get; set; }
    }
}
