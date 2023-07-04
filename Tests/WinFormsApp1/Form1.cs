using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Wombat.Core.DependencyInjection;

namespace WinFormsApp1
{
    [Component(Lifetime = ServiceLifetime.Transient)]
    public partial class Form1 : Form
    {
        IClass _serviceProvider;
        Class2 _class2;

        public Form1(IClass serviceProvider,Class2 class2)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider;
            _class2 = class2;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _serviceProvider.HelloWorld();
            _class2.Class1?.HelloWorld();

            
        }
    }
}
