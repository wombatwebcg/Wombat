using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.DynamicProxy;

namespace Wombat.Core.DependencyInjection
{
    internal class CastleInterceptor : AsyncInterceptorBase
    {
        private readonly IServiceProvider _serviceProvider;
        public CastleInterceptor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private IAOPContext _aopContext;
        private List<AOPBaseAttribute> _aops;
        private async Task Before()
        {
            foreach (var aAop in _aops)
            {
                await aAop.Before(_aopContext);
            }
        }
        private async Task After()
        {
            foreach (var aAop in _aops)
            {
                await aAop.After(_aopContext);
            }
        }
        private void Init(IInvocation invocation)
        {
            _aopContext = new CastleAOPContext(invocation, _serviceProvider);

            _aops = invocation.MethodInvocationTarget.GetCustomAttributes(typeof(AOPBaseAttribute), true)
                .Concat(invocation.InvocationTarget.GetType().GetCustomAttributes(typeof(AOPBaseAttribute), true))
                .Select(x => (AOPBaseAttribute)x)
                .ToList();
        }


        protected override async Task InterceptAsync(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task> proceed)
        {
            Init(invocation);

            await Before();
            await proceed(invocation,proceedInfo);
            await After();
        }

        protected override async Task<TResult> InterceptAsync<TResult>(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task<TResult>> proceed)
        {
            Init(invocation);

            TResult result;

            await Before();
            result = await proceed(invocation,proceedInfo);
            await After();

            return result;
        }
    }
}
