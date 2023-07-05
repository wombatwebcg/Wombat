using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wombat.Core.DependencyInjection;

namespace Wombat.Core.DependencyInjectionTest
{
    [Component(Lifetime = ServiceLifetime.Scoped)]
    //[Transient]
    public class Class1 : IClass
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

    }
}
