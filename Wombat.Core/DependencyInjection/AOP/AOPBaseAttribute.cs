using System;
using System.Threading.Tasks;

namespace Wombat.Core.DependencyInjection
{
    /// <summary>
    /// AOP基类
    /// 注:不支持控制器,需要定义接口并实现接口,自定义AOP特性放到接口实现类上
    /// </summary>
    /// 
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public abstract class AOPBaseAttribute : Attribute
    {
        public virtual async Task Before(IAOPContext context)
        {
            await Task.CompletedTask;
        }

        public virtual async Task After(IAOPContext context)
        {
            await Task.CompletedTask;
        }
    }
}
