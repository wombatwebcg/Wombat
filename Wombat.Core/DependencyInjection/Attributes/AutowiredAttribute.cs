using System;
using System.Threading.Tasks;

namespace Wombat.Core.DependencyInjection
{
    /// <summary>
    /// 属性注入实例
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class AutowiredAttribute : AOPBaseAttribute
    {

        public override Task Before(IAOPContext context)
        {
            return base.Before(context);
        }


        /// <summary>
        /// 之后
        /// </summary>
        /// <param name="aopContext"></param>
        public override Task After(IAOPContext aopContext)
        {
            if (aopContext.Invocation.ReturnValue != null)
            {
                return Task.CompletedTask;

            }
            var name = aopContext.Invocation.Method.Name;
            name = name.Replace("get_", "");
            name = name.Replace("Set_", "");
            var type = aopContext.Invocation.Method.DeclaringType.GetProperty(name).PropertyType;
            var service = aopContext.ServiceProvider.GetService(type);
            aopContext.Invocation.ReturnValue = service;
            return Task.CompletedTask;
        }

    }
}