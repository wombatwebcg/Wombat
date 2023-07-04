using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wombat.Core.DependencyInjection;

namespace WinFormsApp1
{
    [Component(Lifetime = ServiceLifetime.Transient)]

    public class Class2 : IClass2
    {

        private readonly IServiceProvider _serviceProvider;

        public Class2(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        [Autowired]
        public virtual IClass Class1 { get { 
                
                
                return _serviceProvider.GetRequiredService<IClass>();
            
            } }

        public void HelloWorld(int data)
        {

            Class1?.HelloWorld1();
        }


    }
}
