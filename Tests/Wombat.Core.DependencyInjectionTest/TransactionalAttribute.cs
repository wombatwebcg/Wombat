using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wombat.Core.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Wombat.Core.DependencyInjectionTest
{
    /// <summary>
    /// 事务拦截
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class TransactionalAttribute : AopBaseAttribute
    {

        /// <summary>
        /// 事务拦截
        /// </summary>
        /// <param name="dbContextTypes">数据上下文</param>
        public TransactionalAttribute()
        {
        }

        /// <summary>
        /// 之前
        /// </summary>
        /// <param name="aopContext"></param>
        public override void Before(AopContext aopContext)
        {
            Console.WriteLine(11111);
            var dbcontext = aopContext.ServiceProvider.GetService<Class2>();
            dbcontext.Before();
        }

        /// <summary>
        /// 之后
        /// </summary>
        /// <param name="aopContext"></param>
        public override void After(AopContext aopContext)
        {
            Console.WriteLine(111122222221);

            var dbcontext = aopContext.ServiceProvider.GetService<Class2>();
            dbcontext.After();
        }

        /// <summary>
        /// 之前
        /// </summary>
        /// <param name="aopContext"></param>
        public override void Before<TResult>(AopContext aopContext)
        {
            this.Before(aopContext);
        }

        /// <summary>
        /// 之后
        /// </summary>
        /// <param name="aopContext"></param>
        public override void After<TResult>(AopContext aopContext, TResult result)
        {
            this.After(aopContext);
        }

        /// <summary>
        /// 异常
        /// </summary>
        /// <param name="aopContext"></param>
        /// <param name="exception"></param>
        private void OnException(AopContext aopContext, Exception exception)
        {
            throw exception;
        }


    }

}