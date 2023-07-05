using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Wombat.Core
{
   public static class CustomServiceProvider
    {
        #region  记录存储服务提供者

        private static IServiceProvider _serviceProvider;

        /// <summary>
        /// 注册服务提供者
        /// </summary>
        /// <param name="serviceProvider"></param>
        public static void UseServiceProvider(this IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
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

        #endregion

    }
}
