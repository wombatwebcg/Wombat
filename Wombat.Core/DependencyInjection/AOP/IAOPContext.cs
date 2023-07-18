using Castle.DynamicProxy;
using System;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public interface IAOPContext
    {
        IServiceProvider ServiceProvider { get; }

        IInvocation Invocation { get; }
    }
}
