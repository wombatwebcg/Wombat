using System;
using System.Reflection;
using System.Threading.Tasks;
using Castle.DynamicProxy;

namespace Wombat.Core.DependencyInjection
{
    /// <summary>
    /// 拦截器入口类
    /// </summary>
    public class AopInterceptor : IAsyncInterceptor
    {
        private readonly IServiceProvider _serviceProvider;

        public AopInterceptor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 异步拦截函数 带有 Task 返回值
        /// </summary>
        /// <param name="invocation"></param>
        public void InterceptAsynchronous(IInvocation invocation)
        {
            invocation.ReturnValue = InternalInterceptAsynchronous(invocation);
        }
        private async Task InternalInterceptAsynchronous(IInvocation invocation)
        {
            var aopBaseAttribute = GetAopBaseAttribute(invocation);

            if (aopBaseAttribute == null)
            {
                invocation.Proceed();
                await (Task)invocation.ReturnValue;
                return;
            }

            var aopContext = new AopContext(invocation, _serviceProvider);
            // 执行调用之前的函数
            aopBaseAttribute.Before(aopContext);

            //如果需要拦截异常
            if (aopBaseAttribute.ExceptionEvent != null)
            {
                //执行函数返回数据
                try
                {
                    invocation.Proceed();
                    await (Task)invocation.ReturnValue;
                    aopBaseAttribute.After(aopContext);
                    return;
                }
                catch (Exception exception)
                {
                    aopBaseAttribute.ExceptionEvent(aopContext, exception);
                }
            }

            //执行函数返回数据
            invocation.Proceed();
            await (Task)invocation.ReturnValue;
            aopBaseAttribute.After(aopContext);
        }

        /// <summary>
        /// 异步拦截函数 带有 Task《T》 返回值
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="invocation"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void InterceptAsynchronous<TResult>(IInvocation invocation)
        {
            invocation.ReturnValue = InternalInterceptAsynchronous<TResult>(invocation);
        }
        private async Task<TResult> InternalInterceptAsynchronous<TResult>(IInvocation invocation)
        {
            var aopBaseAttribute = GetAopBaseAttribute(invocation);

            TResult result;

            if (aopBaseAttribute == null)
            {
                invocation.Proceed();
                result = await (Task<TResult>)invocation.ReturnValue;
                return result;
            }

            var aopContext = new AopContext(invocation, _serviceProvider);
            // 执行调用之前的函数
            aopBaseAttribute.Before<TResult>(aopContext);

            #region 检查返回值有无 如果有了则不执行函数直接返回数据
            if (invocation.ReturnValue != null)
            {
                if (invocation.ReturnValue.GetType().Name == "Task`1")
                {
                    return await (Task<TResult>)invocation.ReturnValue;
                }

                return (TResult)invocation.ReturnValue;
            }
            #endregion

            //如果需要拦截异常
            if (aopBaseAttribute.ExceptionEvent != null)
            {
                //执行函数返回数据
                try
                {
                    invocation.Proceed();
                    result = await (Task<TResult>)invocation.ReturnValue;
                    aopBaseAttribute.After(aopContext, result);
                    return result;
                }
                catch (Exception exception)
                {
                    aopBaseAttribute.ExceptionEvent(aopContext, exception);
                }
            }

            //执行函数返回数据
            invocation.Proceed();
            result = await (Task<TResult>)invocation.ReturnValue;
            aopBaseAttribute.After(aopContext, result);
            return result;
        }

        /// <summary>
        /// 同步拦截函数
        /// </summary>
        /// <param name="invocation"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void InterceptSynchronous(IInvocation invocation)
        {
            var aopBaseAttribute = GetAopBaseAttribute(invocation);

            if (aopBaseAttribute == null)
            {
                invocation.Proceed();
                return;
            }

            var aopContext = new AopContext(invocation, _serviceProvider);
            // 执行调用之前的函数
            aopBaseAttribute.Before(aopContext);

            #region 检查返回值有无 如果有了则不执行函数直接返回数据
            var result = invocation.ReturnValue;
            if (result != null)
            {
                //aopBaseAttribute.After(aopContext);
                return;
            }
            #endregion

            //如果需要拦截异常
            if (aopBaseAttribute.ExceptionEvent != null)
            {
                try
                {
                    invocation.Proceed();
                    aopBaseAttribute.After(aopContext);
                }
                catch (Exception exception)
                {
                    aopBaseAttribute.ExceptionEvent(aopContext, exception);
                }
            }

            //执行函数返回数据
            invocation.Proceed();
            aopBaseAttribute.After(aopContext);
        }

        /// <summary>
        /// 获取 AopBaseAttribute
        /// </summary>
        /// <param name="invocation"></param>
        /// <returns></returns>
        private AopBaseAttribute GetAopBaseAttribute(IInvocation invocation)
        {

            // 从函数上拿取标记
            var aopBaseAttribute = invocation.Method.GetCustomAttribute<AopBaseAttribute>();

            // 从类上拿取标记
            if (aopBaseAttribute == null)
            {
                aopBaseAttribute = invocation.TargetType.GetCustomAttribute<AopBaseAttribute>();
            }

            var name = invocation.Method.Name;
            // 从属性上拿取标记
            if (aopBaseAttribute == null && (name.StartsWith("get_") || name.StartsWith("set_")))
            {
                name = name.Replace("get_", "");
                name = name.Replace("Set_", "");
                var propertyInfo = invocation.Method.DeclaringType.GetProperty(name);
                if (propertyInfo != null)
                {
                    aopBaseAttribute = propertyInfo.GetCustomAttribute<AopBaseAttribute>();
                }
            }

            return aopBaseAttribute;

        }


    }
}