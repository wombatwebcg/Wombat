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

        private readonly IClass _serviceProvider;

        public Class2(IClass serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }


        public void HelloWorld()
        {
            _serviceProvider.HelloWorld();
        }



    }
}
