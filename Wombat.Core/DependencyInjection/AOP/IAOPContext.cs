using Castle.DynamicProxy;
using System;
using System.Reflection;

namespace Wombat.Core.DependencyInjection
{
    public interface IAOPContext
    {
        IServiceProvider ServiceProvider { get; }
        object[] Arguments { get; }
        Type[] GenericArguments { get; }
        MethodInfo Method { get; }
        MethodInfo MethodInvocationTarget { get; }
        object Proxy { get; }
        object ReturnValue { get; set; }
        Type TargetType { get; }
        IInvocation Invocation { get; }
    }
}
