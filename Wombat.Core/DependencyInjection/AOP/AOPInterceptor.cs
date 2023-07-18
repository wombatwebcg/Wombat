using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Castle.DynamicProxy;

namespace Microsoft.Extensions.DependencyInjection
{
    public class AOPInterceptor : IAsyncInterceptor
    {
        private readonly IServiceProvider _serviceProvider;
        public AOPInterceptor(IServiceProvider serviceProvider)
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
            var aopBaseAttribute = GetAOPBaseAttribute(invocation);

            if (aopBaseAttribute == null)
            {
                invocation.Proceed();
                await (Task)invocation.ReturnValue;
                return;
            }

            var aopContext = new AOPContext(invocation, _serviceProvider);
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

            var aopBaseAttribute = GetAOPBaseAttribute(invocation);

            TResult result;

            if (aopBaseAttribute == null)
            {
                invocation.Proceed();
                result = await (Task<TResult>)invocation.ReturnValue;
                return result;
            }

            var aopContext = new AOPContext(invocation, _serviceProvider);
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

            var aopBaseAttribute = GetAOPBaseAttribute(invocation);

            if (aopBaseAttribute == null)
            {
                invocation.Proceed();
                return;
            }

            var aopContext = new AOPContext(invocation, _serviceProvider);
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
        /// 获取 AOPBaseAttribute
        /// </summary>
        /// <param name="invocation"></param>
        /// <returns></returns>
        private AOPBaseAttribute GetAOPBaseAttribute(IInvocation invocation)
        {

            var sss = invocation.TargetType.GetMethods().Where(x => x.GetCustomAttribute<AOPBaseAttribute>()!=null);

            // 从函数上拿取标记

            var sssss = invocation.MethodInvocationTarget;

            var aopBaseAttribute = invocation.Method.GetCustomAttribute<AOPBaseAttribute>();

            // 从类上拿取标记
            if (aopBaseAttribute == null)
            {
                aopBaseAttribute = invocation.MethodInvocationTarget.GetCustomAttribute<AOPBaseAttribute>();
            }

            var name = invocation.MethodInvocationTarget.Name;
            // 从属性上拿取标记
            if (aopBaseAttribute == null && (name.StartsWith("get_") || name.StartsWith("set_")))
            {
                name = name.Replace("get_", "");
                name = name.Replace("Set_", "");
                var propertyInfo = invocation.Method.DeclaringType.GetProperty(name);
                if (propertyInfo != null)
                {
                    aopBaseAttribute = propertyInfo.GetCustomAttribute<AOPBaseAttribute>();
                }
            }

            return aopBaseAttribute;

        }
    }
}
