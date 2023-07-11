﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wombat.Core.DependencyInjection;

namespace Wombat.Core.DependencyInjectionTest
{
    [Component((typeof(IClass)),Lifetime = ServiceLifetime.Scoped)]

    public class Class1:IClass,IClass1
    {

        private readonly IServiceProvider _serviceProvider;

        public Class1(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }


        [Transactional]
        public void HelloWorld()
        {
            Console.WriteLine("HelloWorld11111");
        }

        public void HelloWorld2()
        {
            Console.WriteLine("HelloWorld22222222");
        }
    }
}
