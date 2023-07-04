
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Wombat;
using Microsoft.Extensions.DependencyInjection;

using Castle.DynamicProxy;

namespace Wombat.Core.DependencyInjection
{
    /// <summary>
    /// IOC 依赖注入服务
    /// </summary>
    public static class DynamicProxyClass
    {
        private readonly static ProxyGenerator _proxyGenerator;

        static DynamicProxyClass()
        {
            if (_proxyGenerator == null)
            {
                _proxyGenerator = new ProxyGenerator();
            }

        }
        #region  记录存储服务提供者

        private static IServiceProvider _serviceProvider;

        /// <summary>
        /// 注册服务提供者
        /// </summary>
        /// <param name="serviceProvider"></param>
        public static void UseServiceProvider(this IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 获取服务提供者
        /// </summary>
        /// <returns></returns>
        public static IServiceProvider GetServiceProvider() => _serviceProvider;


        #endregion

        #region 动态服务注册

        /// <summary>
        /// 扫描服务 自动注入服务
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="assemblyFilter"></param>
        public static void DependencyInjectionService(this IServiceCollection serviceCollection, string assemblyFilter = "")
        {
            IEnumerable<Assembly> assemblies = default;

            if (string.IsNullOrWhiteSpace(assemblyFilter))
            {
                assemblies = GetAssemblyList();
            }
            //else
            //{
            //    assemblies = GetAssemblyList(w =>
            //    {
            //        var name = w.GetName().Name;
            //        return name != null && name.StartsWith(assemblyFilter);
            //    });
            //}

            if (assemblies == null) return;

            // 服务自动注册
            //serviceCollection.ScanComponent(ServiceLifetime.Singleton, assemblies);
            //serviceCollection.ScanComponent(ServiceLifetime.Scoped, assemblies);
            //serviceCollection.ScanComponent(ServiceLifetime.Transient, assemblies);

            ScanComponent(serviceCollection, assemblies);

            // 动态代理注册
            //ScanningAopComponent(serviceCollection, assemblies);
        }

        /// <summary>
        /// 获取当前工程下所有要用到的dll
        /// </summary>
        /// <returns></returns>
        public static List<Assembly> GetAssemblyList()
        {
            List<Assembly> loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            var loadedPaths = loadedAssemblies.Select(
                a =>
                {
                    // prevent exception accessing Location
                    try
                    {
                        return a.Location;
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
            ).ToArray();
            var referencedPaths = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
            var toLoad = referencedPaths.Where(r => !loadedPaths.Contains(r, StringComparer.InvariantCultureIgnoreCase)).ToList();
            toLoad.ForEach(
                path =>
                {
                    // prevent exception loading some assembly
                    try
                    {
                        loadedAssemblies.Add(AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(path)));
                    }
                    catch (Exception)
                    {
                        ; // DO NOTHING
                    }
                }
            );

            // prevent loading of dynamic assembly, autofac doesn't support dynamic assembly
            loadedAssemblies.RemoveAll(i => i.IsDynamic);

            return loadedAssemblies;
        }


        /// <summary>
        /// 服务自动注册
        /// </summary>
        private static void ScanComponent(this IServiceCollection serviceCollection, IEnumerable<Assembly> assemblies)
        {

            //List<Type> types = assemblies.SelectMany(t => t.GetTypes()).Where(t => t.GetCustomAttributes(typeof(ComponentAttribute), false).Length > 0 && t.GetCustomAttribute<ComponentAttribute>()?.Lifetime == serviceLifetime && t.IsClass && !t.IsAbstract).ToList();

            foreach (var sl in Enum.GetValues(typeof(ServiceLifetime)))
            {
                var serviceLifetime = (ServiceLifetime)(sl);
                List<Type> types = assemblies.SelectMany(t => t.GetTypes()).Where(t => t.GetCustomAttributes(typeof(ComponentAttribute), false).Length > 0 && t.GetCustomAttribute<ComponentAttribute>()?.Lifetime == serviceLifetime && t.IsClass && !t.IsAbstract).ToList();
                foreach (var aType in types)
                {
                    //serviceCollection.Add(new ServiceDescriptor(aType, aType, serviceLifetime));
                    var interfaces = assemblies.SelectMany(x => x.GetTypes()).ToArray().Where(x => x.IsAssignableFrom(aType) && x.IsInterface ).ToList();
                    if (interfaces.Count == 0)
                    {
                        inject(serviceLifetime, aType);

                    }
                    var constructors = aType.GetConstructors()?.FirstOrDefault()?.GetParameters()?.Select(w => w.ParameterType)?.ToArray();

                    foreach (var aInterface in interfaces)
                    {
                        inject(serviceLifetime, aType, aInterface);
                        var classAopBaseAttributes = aType.GetCustomAttribute<AOPBaseAttribute>() != null;
                        var propertyAopBaseAttributes = aType.GetProperties().Count(w => w.GetCustomAttribute<AOPBaseAttribute>() != null) > 0;
                        var methodAopBaseAttributes = aType.GetMethods().Count(w => w.GetCustomAttribute<AOPBaseAttribute>() != null) > 0;
                        // 找不到 AopBaseAttribute 标记的 class 一律跳过
                        if (!classAopBaseAttributes && !propertyAopBaseAttributes && !methodAopBaseAttributes) continue;
                        //注入AOP

                        if(propertyAopBaseAttributes)
                        {

                        }
                        serviceCollection.Add(new ServiceDescriptor(aInterface, serviceProvider =>
                        {
                            CastleInterceptor castleInterceptor = new CastleInterceptor(serviceProvider);
                            var constructorArguments = constructors.Select(w => serviceProvider.GetService(w)).ToArray();
                            //return _proxyGenerator.CreateClassProxy(aType, constructorArguments, castleInterceptor);
                            return _proxyGenerator.CreateInterfaceProxyWithTarget(aInterface, serviceProvider.GetService(aType), castleInterceptor);
                        }, serviceLifetime));
                    }
                }
            }

            void inject(ServiceLifetime serviceLifetime,Type type,Type typeInterface = null)
            {

                //服务非继承自接口的直接注入
                switch (serviceLifetime)
                {
                    case ServiceLifetime.Singleton: serviceCollection.AddSingleton(type); break;
                    case ServiceLifetime.Scoped: serviceCollection.AddScoped(type); break;
                    case ServiceLifetime.Transient: serviceCollection.AddTransient(type); break;
                }
                if (typeInterface != null)
                {
                    //服务继承自接口的和接口一起注入
                    switch (serviceLifetime)
                    {
                        case ServiceLifetime.Singleton: serviceCollection.AddSingleton(typeInterface, type); break;
                        case ServiceLifetime.Scoped: serviceCollection.AddScoped(typeInterface, type); break;
                        case ServiceLifetime.Transient: serviceCollection.AddTransient(typeInterface, type); break;
                    }
                }
            }

        }

        /// <summary>
        /// 创建代理类
        /// </summary>
        /// <param name="_class"></param>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        private static object CreateClassProxy(Type _class, IServiceProvider serviceProvider)
        {
            var constructors = _class.GetConstructors()
                   ?.FirstOrDefault()
                   ?.GetParameters()
                   ?.Select(w => w.ParameterType)
                   ?.ToArray()
                   ;

            var constructorArguments = constructors.Select(w => serviceProvider.GetService(w)).ToArray();
            return _proxyGenerator.CreateClassProxy(_class, constructorArguments, serviceProvider.GetService<IAsyncInterceptor>());
        }

        #endregion



        /// <summary>
        /// 创建代理类
        /// </summary>
        /// <param name="_class"></param>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        //private static object CreateClassProxy(Type _class, IServiceProvider serviceProvider)
        //{
        //    var constructors = _class.GetConstructors()?.FirstOrDefault()
        //           ?.GetParameters()
        //           ?.Select(w => w.ParameterType)
        //           ?.ToArray();

        //    var constructorArguments = constructors.Select(w => serviceProvider.GetService(w)).ToArray();
        //    return _proxyGenerator.CreateClassProxy(_class, constructorArguments, serviceProvider.GetService<IAsyncInterceptor>());
        //}

















        /// <summary>
        /// 创建代理类
        /// </summary>
        /// <param name="proxyGenerator"></param>
        /// <param name="_class"></param>
        /// <param name="constructors"></param>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        //private static object CreateClassProxy(ProxyGenerator proxyGenerator, Type _class, Type[] constructors, IServiceProvider serviceProvider)
        //{
        //    var constructorArguments = constructors.Select(w => serviceProvider.GetService(w)).ToArray();
        //    return proxyGenerator.CreateClassProxy(_class, constructorArguments, serviceProvider.GetService<IAsyncInterceptor>());
        //}






        //private static void ScanComponent(IServiceCollection serviceCollection, IEnumerable<Assembly> assemblies)
        //{
        //    serviceCollection.Scan(w =>
        //    {
        //        w.FromAssemblies(assemblies)
        //          //接口注册Scoped
        //          .AddClasses(classes => classes.WithAttribute<ComponentAttribute>(t => t.Lifetime == ServiceLifetime.Scoped))
        //              .AsImplementedInterfaces()
        //              .WithScopedLifetime()
        //          //接口注册Singleton
        //          .AddClasses(classes => classes.WithAttribute<ComponentAttribute>(t => t.Lifetime == ServiceLifetime.Singleton))
        //                .AsImplementedInterfaces()
        //                .WithSingletonLifetime()
        //          //接口注册Transient
        //          .AddClasses(classes => classes.WithAttribute<ComponentAttribute>(t => t.Lifetime == ServiceLifetime.Transient))
        //                .AsImplementedInterfaces()
        //                .WithTransientLifetime()
        //          //具体类注册Scoped
        //          .AddClasses(classes => classes.WithAttribute<ComponentAttribute>(t => t.Lifetime == ServiceLifetime.Scoped))
        //                .AsSelf()
        //                .WithScopedLifetime()
        //          //具体类注册Singleton
        //          .AddClasses(classes => classes.WithAttribute<ComponentAttribute>(t => t.Lifetime == ServiceLifetime.Singleton))
        //                .AsSelf()
        //                .WithSingletonLifetime()
        //          //具体类注册Transient
        //          .AddClasses(classes => classes.WithAttribute<ComponentAttribute>(t => t.Lifetime == ServiceLifetime.Scoped))
        //                .AsSelf()
        //                .WithTransientLifetime();
        //    });

        //}






    }
}
