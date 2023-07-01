
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
        /// 注册 服务提供者
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

        /// <summary>
        /// 创建服务域
        /// </summary>
        /// <returns></returns>
        public static IServiceScope CreateScope() => _serviceProvider.CreateScope();

        #endregion

        #region 动态服务注册

        /// <summary>
        /// 扫描服务 自动注入服务
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="assemblyFilter"></param>
        public static void AddHzyScanDiService(this IServiceCollection serviceCollection, string assemblyFilter = "")
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
            ScanningAopomponent(serviceCollection, assemblies);
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
        private static void ScanComponent(this IServiceCollection serviceCollection, ServiceLifetime serviceLifetime, IEnumerable<Assembly> assemblies)
        {

            List<Type> types = assemblies.SelectMany(t => t.GetTypes()).Where(t => t.GetCustomAttributes(typeof(ComponentAttribute), false).Length > 0 && t.GetCustomAttribute<ComponentAttribute>()?.Lifetime == serviceLifetime && t.IsClass && !t.IsAbstract).ToList();

            foreach (var type in types)
            {
                //服务非继承自接口的直接注入
                switch (serviceLifetime)
                {
                    case ServiceLifetime.Singleton: serviceCollection.AddSingleton(type); break;
                    case ServiceLifetime.Scoped: serviceCollection.AddScoped(type); break;
                    case ServiceLifetime.Transient: serviceCollection.AddTransient(type); break;
                }
                Type typeInterface = type.GetInterfaces().FirstOrDefault();
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
        /// 扫描符合代理类的服务自动注册
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="assemblies"></param>
        private static void ScanningAopomponent(IServiceCollection serviceCollection, IEnumerable<Assembly> assemblies)
        {
            serviceCollection.AddTransient<IAsyncInterceptor, AopInterceptor>();
            var typeDependencys = new List<ServiceLifetime>();
            typeDependencys.Add(ServiceLifetime.Singleton);
            typeDependencys.Add(ServiceLifetime.Scoped);
            typeDependencys.Add(ServiceLifetime.Transient);
            //将代码写入字典
            var dicDependency = new Dictionary<string, Func<Type, Type, IServiceCollection>>();
            dicDependency.Add(nameof(ServiceLifetime.Scoped), (_class, _interface) =>
            serviceCollection.AddScoped(_interface, (serviceProvider) => CreateClassProxy(_class, serviceProvider)));
            dicDependency.Add(nameof(ServiceLifetime.Singleton), (_class, _interface) =>
            serviceCollection.AddSingleton(_interface, (serviceProvider) => CreateClassProxy(_class, serviceProvider)));
            dicDependency.Add(nameof(ServiceLifetime.Transient), (_class, _interface) =>
            serviceCollection.AddTransient(_interface, (serviceProvider) => CreateClassProxy(_class, serviceProvider)));
            ////将代码写入字典
            var dicSelfDependency = new Dictionary<string, Func<Type, IServiceCollection>>();
            dicSelfDependency.Add(nameof(ServiceLifetime.Scoped), (_class) =>
            serviceCollection.AddScoped(_class, (serviceProvider) => CreateClassProxy(_class, serviceProvider)));
            dicSelfDependency.Add(nameof(ServiceLifetime.Singleton), (_class) =>
            serviceCollection.AddSingleton(_class, (serviceProvider) => CreateClassProxy(_class, serviceProvider)));
            dicSelfDependency.Add(nameof(ServiceLifetime.Transient), (_class) =>
            serviceCollection.AddTransient(_class, (serviceProvider) => CreateClassProxy(_class, serviceProvider)));
            //// 控制器服务注册 写入代码
            //dicSelfDependency.Add(nameof(ControllerBase), (_class, _constructors) =>
            //serviceCollection.AddTransient(_class, (serviceProvider) => CreateClassProxy(proxyGenerator, _class, _constructors, serviceProvider)));
            //
            //var prefix = "I";
            //var suffix = "Dependency";
            //var self = "Self";
            //
            foreach (var item in assemblies.Where(w => !w.IsDynamic))
            {
                // 必须是 class 并且 不能是 泛型类
                var classList = item.ExportedTypes
                    .Where(w => w.IsClass && !w.IsGenericType && w.IsPublic);
                    //不包含 IDependency 接口直接跳过
                    //.Where(t => t.GetCustomAttributes(typeof(ComponentAttribute), false).Length > 0);

                if (classList.Count() == 0)
                {
                    continue;
                }


                foreach (var _class in classList)
                {

                    //查找 class 接口
                    var allInterfaces = _class.GetInterfaces();

                    //检测类型种是否有 AopBaseAttribute 特性
                    var propertyInfos = _class.GetProperties();
                    var methodInfos = _class.GetMethods();
                    //获取特性 从 类、属性、函数 获取特性，如果一个都找不到则跳过不生成代理类
                    var classAopBaseAttributes = _class.GetCustomAttribute<AopBaseAttribute>() != null;
                    var propertyAopBaseAttributes = propertyInfos.Count(w => w.GetCustomAttribute<AopBaseAttribute>() != null) > 0;
                    var methodAopBaseAttributes = methodInfos.Count(w => w.GetCustomAttribute<AopBaseAttribute>() != null) > 0;
                    // 找不到 AopBaseAttribute 标记的 class 一律跳过
                    if (!classAopBaseAttributes && !propertyAopBaseAttributes && !methodAopBaseAttributes) continue;

                    //接口过滤
                    //var interfaces = allInterfaces.Where(w => !typeDependencys.Contains(w.));


                    // 服务注册
                    var _interface = allInterfaces.FirstOrDefault();
                    var key = Enum.GetName(typeof(ServiceLifetime), _class.GetCustomAttribute<ComponentAttribute>().Lifetime);
                    if (_interface != null)
                    {
                        dicDependency[key]?.Invoke(_class, _interface);
                    }

                    // 服务注册 自身
                    dicSelfDependency[key]?.Invoke(_class);

                }
            }
        }

        #endregion



        /// <summary>
        /// 创建代理类
        /// </summary>
        /// <param name="_class"></param>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        private static object CreateClassProxy(Type _class, IServiceProvider serviceProvider)
        {
            var constructors = _class.GetConstructors()?.FirstOrDefault()
                   ?.GetParameters()
                   ?.Select(w => w.ParameterType)
                   ?.ToArray();

            var constructorArguments = constructors.Select(w => serviceProvider.GetService(w)).ToArray();
            return _proxyGenerator.CreateClassProxy(_class, constructorArguments, serviceProvider.GetService<IAsyncInterceptor>());
        }

















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






        private static void ScanComponent(IServiceCollection serviceCollection, IEnumerable<Assembly> assemblies)
        {
            serviceCollection.Scan(w =>
            {
                w.FromAssemblies(assemblies)
                  //接口注册Scoped
                  .AddClasses(classes => classes.WithAttribute<ComponentAttribute>(t => t.Lifetime == ServiceLifetime.Scoped))
                      .AsImplementedInterfaces()
                      .WithScopedLifetime()
                  //接口注册Singleton
                  .AddClasses(classes => classes.WithAttribute<ComponentAttribute>(t => t.Lifetime == ServiceLifetime.Singleton))
                        .AsImplementedInterfaces()
                        .WithSingletonLifetime()
                  //接口注册Transient
                  .AddClasses(classes => classes.WithAttribute<ComponentAttribute>(t => t.Lifetime == ServiceLifetime.Transient))
                        .AsImplementedInterfaces()
                        .WithTransientLifetime()
                  //具体类注册Scoped
                  .AddClasses(classes => classes.WithAttribute<ComponentAttribute>(t => t.Lifetime == ServiceLifetime.Scoped))
                        .AsSelf()
                        .WithScopedLifetime()
                  //具体类注册Singleton
                  .AddClasses(classes => classes.WithAttribute<ComponentAttribute>(t => t.Lifetime == ServiceLifetime.Singleton))
                        .AsSelf()
                        .WithSingletonLifetime()
                  //具体类注册Transient
                  .AddClasses(classes => classes.WithAttribute<ComponentAttribute>(t => t.Lifetime == ServiceLifetime.Scoped))
                        .AsSelf()
                        .WithTransientLifetime();
            });

        }

    }
}
