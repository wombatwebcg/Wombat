using System;
using Castle.DynamicProxy;

namespace Wombat.Core.DependencyInjection
{
    /// <summary>
    /// 拦截器上下文
    /// </summary>
    public class AopContext
    {
        /// <summary>
        /// 拦截器上下文
        /// </summary>
        /// <param name="invocation"></param>
        /// <param name="serviceProvider"></param>
        public AopContext(IInvocation invocation, IServiceProvider serviceProvider)
        {
            Invocation = invocation;
            ServiceProvider = serviceProvider;
        }

        /// <summary>
        /// 拦截器 拦截信息熟性
        /// </summary>
        /// <value></value>
        public IInvocation Invocation { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public IServiceProvider ServiceProvider { get; }

    }
}