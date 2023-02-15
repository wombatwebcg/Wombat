using InfluxData.Net.Common.Enums;
using InfluxData.Net.InfluxDb;
using InfluxData.Net.InfluxDb.Models;
using InfluxData.Net.InfluxDb.Models.Responses;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Wombat.Infrastructure
{
    public static class InfluxCoreModule
    {

        public static IServiceCollection AddInfluxCoreModule(this IServiceCollection services, InfluxDbOptions options)
        {
            _influxDbClient = new InfluxDbClient(options.Address, options.UserName, options.Password, InfluxDbVersion.v_1_0_0);
            services.AddSingleton(_influxDbClient);
            return services;
        }

        static InfluxDbClient _influxDbClient;



        public static Task<IEnumerable<Database>> GetDBAsync()
        {
            return ServiceProvider.GetService<InfluxDbClient>().Database.GetDatabasesAsync();
        }

        public static Task CreateDBAsync(string db)
        {
            return ServiceProvider.GetService<InfluxDbClient>().Database.CreateDatabaseAsync(db);
        }



        public static Task WriteAsync(string serieName, Dictionary<string, object> tags, Dictionary<string, object> fields)
        {
            var pointToWrite = new Point();
            pointToWrite.Name = serieName;
            pointToWrite.Tags = tags;
            pointToWrite.Fields = fields;
            pointToWrite.Timestamp = DateTime.Now;
            var client = ServiceProvider.GetService<InfluxDbClient>().Client;
            return client.WriteAsync(pointToWrite, SystemConnectionConfiguration.InfluxDbConfiguration().DbName,precision:"ms");
        }

        public static Task QueryAsync(string serieName, string startTime = "now()-24H", string endTime = "now() - 5M")
        {
            var client = ServiceProvider.GetService<InfluxDbClient>().Client;

            //where time >= '2022-02-15T00:00:00Z' and time< '2022-02-16T23:59:59Z'
            var query = $"SELECT * FROM \"{serieName}\" WHERE time >=\"{startTime}\" and time <\"{endTime}\"";
            return client.QueryAsync(query, SystemConnectionConfiguration.InfluxDbConfiguration().DbName);
        }

        public static Task QueryAsync(string serieName)
        {
            var client = ServiceProvider.GetService<InfluxDbClient>().Client;

            //where time >= '2022-02-15T00:00:00Z' and time< '2022-02-16T23:59:59Z'
            var query = $"SELECT * FROM \"{serieName}\" WHERE time >= 0";
            return client.QueryAsync(query, SystemConnectionConfiguration.InfluxDbConfiguration().DbName);
        }

        public static Task<IEnumerable<Serie>> QueryAllAsync(string measurementName)
        {
            var client = ServiceProvider.GetService<InfluxDbClient>().Client;

            //where time >= '2022-02-15T00:00:00Z' and time< '2022-02-16T23:59:59Z'
            var query = $"SELECT * FROM \"{measurementName}\" WHERE time >= 0";
            return client.QueryAsync(query, SystemConnectionConfiguration.InfluxDbConfiguration().DbName);
        }

        public static Task<IEnumerable<SerieSet>> QuerySerieAsync(string measurementName = null, IEnumerable<string> filters = null)
        {
            var client = ServiceProvider.GetService<InfluxDbClient>();

            return client.Serie.GetSeriesAsync(SystemConnectionConfiguration.InfluxDbConfiguration().DbName, measurementName, filters);
        }

        public static Task<IEnumerable<Measurement>> GetMeasurementsAsync()
        {
            var client = ServiceProvider.GetService<InfluxDbClient>();
            return client.Serie.GetMeasurementsAsync(SystemConnectionConfiguration.InfluxDbConfiguration().DbName);

        }


        public static Task<IEnumerable<FieldKey>> GetFieldKeysAsync(string measurementName)
        {
            var client = ServiceProvider.GetService<InfluxDbClient>();
            return client.Serie.GetFieldKeysAsync(SystemConnectionConfiguration.InfluxDbConfiguration().DbName, measurementName);

        }
        public static Task<IEnumerable<string>> GetTagKeysAsync( string measurementName)
        {
            var client = ServiceProvider.GetService<InfluxDbClient>();
            return client.Serie.GetTagKeysAsync(SystemConnectionConfiguration.InfluxDbConfiguration().DbName, measurementName);

        }
        public static Task<IEnumerable<TagValue>> GetTagValuesAsync(string measurementName, string tagName)
        {
            var client = ServiceProvider.GetService<InfluxDbClient>();
            return client.Serie.GetTagValuesAsync(SystemConnectionConfiguration.InfluxDbConfiguration().DbName, measurementName,tagName);

        }
























        /// <summary>
        /// 将当前时间转换成unix时间戳形式
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        //public static long ToUnixTimestamp(this DateTime datetime)
        //{
        //    return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
        //}





        /*另外一个轮子
                            using (Metrics.Collector = new CollectorConfiguration()
                                .Tag.With("id", ky.Id.ToString())
                                .Tag.With("pos1", positionAndTemperatures[0].ToString())
                                .Tag.With("pos2", positionAndTemperatures[1].ToString())
                                .Tag.With("pos3", positionAndTemperatures[2].ToString())
                                .Tag.With("temp1", positionAndTemperatures[3].ToString())
                                .Tag.With("temp2", positionAndTemperatures[4].ToString())
                                .Tag.With("temp3", positionAndTemperatures[5].ToString())
                                //.Tag.With("pos1max", ky.Position1Max.ToString())
                                //.Tag.With("pos1min", ky.Position1Min.ToString())
                                //.Tag.With("pos2max", ky.Position2Max.ToString())
                                //.Tag.With("pos2min", ky.Position2Min.ToString())
                                //.Tag.With("pos3max", ky.Position3Max.ToString())
                                //.Tag.With("pos3min", ky.Position3Min.ToString())
                                //.Tag.With("temp1max", ky.Temperature1Max.ToString())
                                //.Tag.With("temp1min", ky.Temperature1Min.ToString())
                                //.Tag.With("temp2max", ky.Temperature2Max.ToString())
                                //.Tag.With("temp2min", ky.Temperature2Min.ToString())
                                //.Tag.With("temp3max", ky.Temperature3Max.ToString())
                                //.Tag.With("temp3min", ky.Temperature3Min.ToString())
                                //.Batch.AtInterval(TimeSpan.FromSeconds(_appConfiguration.InfluxDbSetting.AtInterval))
                                .WriteTo.InfluxDB(new Uri(_appConfiguration.InfluxDbSetting.Address), _appConfiguration.InfluxDbSetting.DbName, _appConfiguration.InfluxDbSetting.UserName, _appConfiguration.InfluxDbSetting.Password)
                                .CreateCollector())
                            {
                                Metrics.Increment(DateTime.Now.ToString("d"),5);

                            }
        */
    }
}
