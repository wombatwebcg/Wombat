using Autofac.Annotation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wombat.Infrastructure.Autofac
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    [Component(AutofacScope = AutofacScope.SingleInstance)]
    public  class SingletonAttribute : Attribute
    {
        public SingletonAttribute()
        {
           
        }

    }

    [Component(AutofacScope = AutofacScope.InstancePerDependency)]
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]

    public class DependencyAttribute : Attribute
    {
    }


    [Component(AutofacScope = AutofacScope.InstancePerLifetimeScope)]
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class LifetimeScopeAttribute : Attribute
    {
    }



    [Component(AutofacScope = AutofacScope.InstancePerRequest)]
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class RequestAttribute : Attribute
    {
    }

}
