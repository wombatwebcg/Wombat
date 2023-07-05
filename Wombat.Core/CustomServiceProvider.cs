using System;
using System.Collections.Generic;
using System.Text;

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
        public static IServiceProvider GetServiceProvider() => _serviceProvider;


        #endregion

    }
}
