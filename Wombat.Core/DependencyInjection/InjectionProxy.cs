
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Wombat;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Castle.DynamicProxy;

namespace Wombat.Core.DependencyInjection
{
    /// <summary>
    /// IOC 依赖注入服务
    /// </summary>
    public static class InjectionProxy
    {
        private readonly static ProxyGenerator _proxyGenerator;

        static InjectionProxy()
        {
            if (_proxyGenerator == null)
            {
                _proxyGenerator = new ProxyGenerator();
            }

        }

        #region 动态服务注册
        /// <summary>
        /// 扫描服务 自动注入服务
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="assemblyFilter"></param>
        public static void DependencyInjectionService(this IServiceCollection serviceCollection,params string[] assemblyNames)
        {
            IEnumerable<Assembly> assemblies = default;

            if (assemblyNames.Length == 0)
            {
                assemblies = AssemblyLoader.GetAssemblyList();

            }
            else
            {
                assemblies = AssemblyLoader.GetAssemblyList(assemblyNames);
            }

            if (assemblies == null) return;

            // 注入配置
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")}.json", optional: true, reloadOnChange: false);
            IConfiguration configurationRoot = configurationBuilder.Build();
            serviceCollection.AddSingleton(configurationRoot);


            // 服务自动注册
            ScanComponent(serviceCollection, assemblies);

            // 动态代理注册
            //ScanningAopComponent(serviceCollection, assemblies);
        }
















        /// <summary>
        /// 服务自动注册
        /// </summary>
        private static void ScanComponent(this IServiceCollection serviceCollection, IEnumerable<Assembly> assemblies)
        {

            //List<Type> types = assemblies.SelectMany(t => t.GetTypes()).Where(t => t.GetCustomAttributes(typeof(ComponentAttribute), false).Length > 0 && t.GetCustomAttribute<ComponentAttribute>()?.Lifetime == serviceLifetime && t.IsClass && !t.IsAbstract).ToList();
            serviceCollection.AddTransient<IAsyncInterceptor, AOPInterceptor>();

            try
            {
                foreach (var localServiceLifetime in Enum.GetValues(typeof(ServiceLifetime)))
                {
                    var serviceLifetime = (Microsoft.Extensions.DependencyInjection.ServiceLifetime)(localServiceLifetime);
                    List<Type> types = assemblies.Where(w => !w.IsDynamic).SelectMany(t => t.GetTypes()).Where(t => t.GetCustomAttributes(typeof(ComponentAttribute), false).Length > 0 && t.GetCustomAttribute<ComponentAttribute>()?.Lifetime ==(ServiceLifetime)localServiceLifetime && t.IsClass && !t.IsAbstract).ToList();
                    foreach (var aType in types)
                    {
                        //serviceCollection.Add(new ServiceDescriptor(aType, aType, serviceLifetime));
                        var interfaces = assemblies.Where(w => !w.IsDynamic).SelectMany(x => x.GetTypes()).ToArray().Where(x => x.IsAssignableFrom(aType) && x.IsInterface).ToList();

                        var classAopBaseAttributes = aType.GetCustomAttribute<AOPBaseAttribute>() != null;
                        var propertyAopBaseAttributes = aType.GetProperties().Count(w => w.GetCustomAttribute<AOPBaseAttribute>() != null) > 0;
                        var methodAopBaseAttributes = aType.GetMethods().Count(w => w.GetCustomAttribute<AOPBaseAttribute>() != null) > 0;
                        var constructors = aType.GetConstructors()?.FirstOrDefault()?.GetParameters()?.Select(w => w.ParameterType)?.ToArray();

                        if (interfaces.Count == 0)
                        {
                            inject(serviceLifetime, aType);

                            if (!classAopBaseAttributes && !propertyAopBaseAttributes && !methodAopBaseAttributes)
                            {
                                continue;
                            }
                            //injectProxy(serviceLifetime, aType);
                            serviceCollection.Add(new ServiceDescriptor(aType, serviceProvider =>
                            {
                                var constructorArguments = constructors.Select(w => serviceProvider.GetService(w)).ToArray();
                                return _proxyGenerator.CreateClassProxy(aType, constructorArguments, serviceProvider.GetService<IAsyncInterceptor>());
                                //return _proxyGenerator.CreateClassProxyWithTarget(aType, serviceProvider.GetService(aType), castleInterceptor);
                            }, serviceLifetime));
                            continue;
                        }

                        foreach (var aInterface in interfaces)
                        {
                            inject(serviceLifetime, aType, aInterface);
                            if (!classAopBaseAttributes && !propertyAopBaseAttributes && !methodAopBaseAttributes)
                            {
                                continue;
                            }
                            //注入AOP
                            serviceCollection.Add(new ServiceDescriptor(aInterface, serviceProvider =>
                            {
                                var constructorArguments = constructors.Select(w => serviceProvider.GetService(w)).ToArray();
                                return _proxyGenerator.CreateInterfaceProxyWithTarget(aInterface, serviceProvider.GetService(aType), serviceProvider.GetService<IAsyncInterceptor>());
                            }, serviceLifetime));

                            serviceCollection.Add(new ServiceDescriptor(aType, serviceProvider =>
                            {
                                var constructorArguments = constructors.Select(w => serviceProvider.GetService(w)).ToArray();
                                return _proxyGenerator.CreateClassProxy(aType, constructorArguments, serviceProvider.GetService<IAsyncInterceptor>());
                            }, serviceLifetime));




                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex; 
            }
            void inject(Microsoft.Extensions.DependencyInjection.ServiceLifetime serviceLifetime,Type type,Type typeInterface = null)
            {

                //服务非继承自接口的直接注入
                switch (serviceLifetime)
                {
                    case Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton: serviceCollection.AddSingleton(type); break;
                    case Microsoft.Extensions.DependencyInjection.ServiceLifetime.Scoped: serviceCollection.AddScoped(type); break;
                    case Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient: serviceCollection.AddTransient(type); break;
                }
                if (typeInterface != null)
                {
                    //服务继承自接口的和接口一起注入
                    switch (serviceLifetime)
                    {
                        case Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton: serviceCollection.AddSingleton(typeInterface, type); break;
                        case Microsoft.Extensions.DependencyInjection.ServiceLifetime.Scoped: serviceCollection.AddScoped(typeInterface, type); break;
                        case Microsoft.Extensions.DependencyInjection. ServiceLifetime.Transient: serviceCollection.AddTransient(typeInterface, type); break;
                    }
                }
            }

        }


        #endregion


    }
}
