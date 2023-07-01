using Microsoft.Extensions.DependencyInjection;
using System;
using Wombat.Core.DependencyInjection;
using Castle.Core;

namespace Wombat.Core.DependencyInjectionTest
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceCollection services = new ServiceCollection();

            services.AddHzyScanDiService();
            var serviceProvider = services.BuildServiceProvider();
            var sss = serviceProvider.GetRequiredService<Class1>();


            string text = Console.ReadLine();

            //if (text=="1")
            //{
            //    var sss = serviceProvider.GetRequiredService<Class1>();
            //    sss = serviceProvider.GetRequiredService<Class1>();
            //    sss.HelloWorld();

            //    Class1 class1 = new Class1(serviceProvider);
            //    class1.HelloWorld();
            //  var sss1 = serviceProvider.GetRequiredService<Class2>();

            //    sss1.HelloWorld();
            //}
            //IOCUtil.GetServiceProvider().GetService<IClass1>().HelloWorld2();



            Console.ReadKey();
        }


        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddHzyScanDiService();

            var serviceProvider = services.BuildServiceProvider();

            serviceProvider.UseServiceProvider();


        }
    }
}
