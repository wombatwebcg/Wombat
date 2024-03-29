﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Wombat.DependencyInjection
{
   public static class CustomServiceProvider
    {
        #region  记录存储服务提供者

        internal static IServiceProvider _serviceProvider;
        internal static IConfiguration _configuration;

        /// <summary>
        /// 注册服务提供者
        /// </summary>
        /// <param name="serviceProvider"></param>
        public static void UseCustomServiceProvider(this IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 注册服务提供者
        /// </summary>
        /// <param name="serviceProvider"></param>
        public static void UseCustomConfigurationProvider(this IConfiguration configuration)
        {
            _configuration = configuration;
        }



        /// <summary>
        /// 获取服务提供者
        /// </summary>
        /// <returns></returns>
        public static T GetService<T>()
        {
          return  _serviceProvider.GetService<T>();
        }


        /// <summary>
        /// 获取服务提供者
        /// </summary>
        /// <returns></returns>
        public static T GetRequiredService<T>()
        {
            return _serviceProvider.GetRequiredService<T>();
        }



        public static IConfiguration GetConfiguration()
        {
            return _configuration;

        }
        #endregion

    }
}
