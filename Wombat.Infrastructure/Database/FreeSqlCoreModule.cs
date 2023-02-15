using FreeSql;
using FreeSql.Internal;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Wombat.Infrastructure
{

    /// <summary>
    /// FreeSql 模块配置
    /// </summary>
    public static class FreeSqlCoreModule
    {
        /// <summary>
        /// 注册 仓储层
        /// </summary>
        /// <param name="services"></param>
        /// <param name="appConfiguration"></param>
        /// <param name="assemblyFilter"></param>
        /// <exception cref="Exception"></exception>
        public static IServiceCollection AddFreeSql(this IServiceCollection services, ConnectionStringsOptions connectionStrings , string assemblyFilter = "KYDevicesServices.Repository")
        {
            var databaseType = connectionStrings.DefaultDatabaseType;
            var dataType = DataType.SqlServer;
            var connectionString = connectionStrings.DefaultSqlServer;

            if (databaseType == DefaultDatabaseType.SqlServer)
            {
                connectionString = connectionStrings.DefaultSqlServer;
                dataType = DataType.SqlServer;
            }
            if (databaseType == DefaultDatabaseType.MySql)
            {
                connectionString = connectionStrings.DefaultMySql;
                dataType = DataType.MySql;
            }
            if (databaseType == DefaultDatabaseType.PostgreSql)
            {
                connectionString = connectionStrings.DefaultPostgreSql;
                dataType = DataType.PostgreSQL;
            }
            if(databaseType == DefaultDatabaseType.Sqlite)
            {
                connectionString = connectionStrings.DefaultSqlite;
                dataType = DataType.Sqlite;

            }
            var freeSql = CreateFreeSql(connectionString, dataType);

            services.AddSingleton(freeSql);

            //services.AddScoped<UnitOfWorkManager>();

            //var ttt = services.AddFreeRepository(null, GlobalAssemblies.GetAssemblyLis(w =>
            //{
            //    var name = w.GetName().Name;
            //    return name != null && name.StartsWith(assemblyFilter);
            //}).ToArray());

            return services;
        }

        /// <summary>
        /// 创建 free sql 对象
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="dataType"></param>
        /// <returns></returns>
        private static IFreeSql CreateFreeSql(string connectionString, DataType dataType)
        {
            var freeSql = new FreeSqlBuilder()
                .UseConnectionString(dataType, connectionString)          
                .UseAutoSyncStructure(true)//自动迁移实体的结构到数据库
                .UseLazyLoading(false)
                .UseNameConvert(NameConvertType.None)
                .UseNoneCommandParameter(true)
                .UseExitAutoDisposePool(true)
                //.UseMonitorCommand(cmd => Console.Write(cmd.CommandText))
                .Build(); //请务必定义成 Singleton 单例模式

            // sql执行后
            freeSql.Aop.CurdAfter += (s, curdAfter) =>
            {
                if (curdAfter.ElapsedMilliseconds > 1000)
                {
                    var stringBuilder = new StringBuilder();
                    stringBuilder.Append($"\r\n====[Sql Start 耗时: {curdAfter.ElapsedMilliseconds} ms]=========");
                    stringBuilder.Append($"\r\n{curdAfter.Sql}");
                    stringBuilder.Append($"\r\n====[Sql End 线程Id:{Environment.CurrentManagedThreadId}]=========");
                    Console.WriteLine(stringBuilder);
                    Log.Warning(stringBuilder.ToString());
                }
            };

            // 审计属性值
            //freeSql.Aop.AuditValue += (s, auditInfo) =>
            //{
            //    switch (auditInfo.AuditValueType)
            //    {
            //        case AuditValueType.Insert:
            //            {
            //                if (auditInfo.Property.Name == nameof(BaseModel.CreateTime) ||
            //                    auditInfo.Property.Name == nameof(BaseModel.UpdateTime))
            //                {
            //                    auditInfo.Value = DateTime.Now;
            //                }

            //                break;
            //            }
            //        case AuditValueType.Update:
            //        case AuditValueType.InsertOrUpdate:
            //            {
            //                if (auditInfo.Property.Name == nameof(BaseModel.UpdateTime))
            //                {
            //                    auditInfo.Value = DateTime.Now;
            //                }

            //                break;
            //            }
            //        default:
            //            throw new ArgumentOutOfRangeException();
            //    }
            //};

            return freeSql;
        }

        /// <summary>
        /// 批量注入 Repository，可以参考代码自行调整
        /// </summary>
        /// <param name="services"></param>
        /// <param name="globalDataFilter"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public static IServiceCollection AddFreeRepository(this IServiceCollection services, Action<FluentDataFilter> globalDataFilter = null, params Assembly[] assemblies)
        {
            if (globalDataFilter != null)
            {
                //DataFilterUtil._globalDataFilter = globalDataFilter;
                //如果看到了这里的代码，想自己调整，但因为 _globalDataFilter 是内部属性，无法修改？
                //请考虑改用 fsql.GlobalFilter.Apply
            }

            services.AddScoped(typeof(IBaseRepository<>), typeof(GuidRepository<>));
            services.AddScoped(typeof(BaseRepository<>), typeof(GuidRepository<>));

            services.AddScoped(typeof(IBaseRepository<,>), typeof(DefaultRepository<,>));
            services.AddScoped(typeof(BaseRepository<,>), typeof(DefaultRepository<,>));

            if (assemblies?.Any() == true)
                foreach (var asse in assemblies)
                    foreach (var repo in asse.GetTypes().Where(a => a.IsAbstract == false && typeof(IBaseRepository).IsAssignableFrom(a)))
                    {
                        services.AddScoped(repo);
                    }

            return services;
        }







    }

}