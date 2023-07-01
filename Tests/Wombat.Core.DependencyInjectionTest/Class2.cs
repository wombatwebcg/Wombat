using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wombat.Core.DependencyInjection;

namespace Wombat.Core.DependencyInjectionTest
{
    [Component(Lifetime = ServiceLifetime.Transient)]
    //[Transient]

    public class Class2 
    {

        private readonly IServiceProvider _serviceProvider;

        public Class2(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        [Transactional]

        public void HelloWorld()
        {
            Console.WriteLine(222222222);
        }


        public void Before()
        {
            Console.WriteLine("Before");
        }

        public void After()
        {
            Console.WriteLine("After");
        }

    }
}
