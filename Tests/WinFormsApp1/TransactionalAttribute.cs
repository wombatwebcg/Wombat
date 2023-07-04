using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wombat.Core.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace WinFormsApp1
{
    /// <summary>
    /// 事务拦截
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class TransactionalAttribute : AOPBaseAttribute
    {

        /// <summary>
        /// 事务拦截
        /// </summary>
        /// <param name="dbContextTypes">数据上下文</param>
        public TransactionalAttribute()
        {
        }

        public override Task Before(IAOPContext context)
        {
            Console.WriteLine(11111);
            return base.Before(context);    
        }
        public override Task After(IAOPContext context)
        {
            Console.WriteLine(22222);
            context.ReturnValue = 123456;
            return base.After(context); 
        }


    }

}