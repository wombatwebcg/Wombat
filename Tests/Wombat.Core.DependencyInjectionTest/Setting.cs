using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Wombat.Core.DependencyInjectionTest
{
    [Component]
   public class Setting
    {
        [AppSettings("Test:Value1")]
        public virtual double Value1 { get; }


        [AppSettings("Test:Name")]
        public virtual string Test1 { get; }


    }
}
