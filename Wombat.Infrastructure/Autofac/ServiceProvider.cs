using Autofac;
using Autofac.Annotation;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Wombat.Infrastructure
{
    /// <summary>
    /// Autofac依赖注入服务
    /// </summary>
    public static class ServiceProvider
    {

        public static IServiceProvider UseServiceProvider(this IServiceCollection services, Assembly[] assemblies = null )
        {

            // Create a container-builder and register dependencies
            var builder = new ContainerBuilder();

            // Populate the service-descriptors added to `IServiceCollection`
            // BEFORE you add things to Autofac so that the Autofac
            // registrations can override stuff in the `IServiceCollection`
            // as needed
            builder.Populate(services);

            // 在这里 在这里 在这里

            if(assemblies!=null)
            builder.RegisterAssemblyTypes(assemblies);


            builder.RegisterModule(new AutofacAnnotationModule());
            //注册类库程序集

            Container = builder.Build();

            // this will be used as the service-provider for the application!
            AutofacServiceProvider = new AutofacServiceProvider(Container);

            return AutofacServiceProvider;
        }

        public static ContainerBuilder Containerbuilder { get;set; }

        /// <summary>
        /// Autofac依赖注入静态服务
        /// </summary>
       private static AutofacServiceProvider AutofacServiceProvider { get; set; }

        /// <summary>
        /// Autofac依赖注入静态服务
        /// </summary>
        public static ILifetimeScope Container { get; set; }

        /// <summary>
        /// 获取服务(Single)
        /// </summary>
        /// <typeparam name="T">接口类型</typeparam>
        /// <returns></returns>
        public static T GetService<T>() where T : class
        {
            return Container.Resolve<T>();
        }

        /// <summary>
        /// 获取服务(请求生命周期内)
        /// </summary>
        /// <typeparam name="T">接口类型</typeparam>
        /// <returns></returns>
        //public static T GetScopeService<T>() where T : class
        //{


        //}



    }
}
