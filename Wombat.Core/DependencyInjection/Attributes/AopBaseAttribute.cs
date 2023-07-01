using System;

namespace Wombat.Core.DependencyInjection
{
    /// <summary>
    /// aop 基础特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public abstract class AopBaseAttribute : Attribute
    {
        /// <summary>
        /// 函数执行异常事件
        /// </summary>
        public Action<AopContext, Exception> ExceptionEvent { get; set; }

        /// <summary>
        /// 函数执行前
        /// </summary>
        /// <param name="aopContext"></param>
        public virtual void Before(AopContext aopContext)
        {

        }

        /// <summary>
        /// 函数执行后
        /// </summary>
        /// <param name="aopContext"></param>
        public virtual void After(AopContext aopContext)
        {

        }

        /// <summary>
        /// 函数执行前 异步函数 带有泛型 只有 Task《TResult》 返回类型才触发
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="aopContext"></param>
        public virtual void Before<TResult>(AopContext aopContext)
        {

        }

        /// <summary>
        /// 函数执行后 异步函数 带有泛型 只有 Task《TResult》 返回类型才触发
        /// </summary>
        /// <param name="aopContext"></param>
        /// <param name="result"></param>
        /// <typeparam name="TResult"></typeparam>
        public virtual void After<TResult>(AopContext aopContext, TResult result)
        {

        }
    }
}
