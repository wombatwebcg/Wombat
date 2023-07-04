using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Wombat.Core.DependencyInjection;

namespace WinFormsApp1
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ServiceCollection services = new ServiceCollection();

            services.DependencyInjectionService();
            var serviceProvider = services.BuildServiceProvider();
            var sss = serviceProvider.GetRequiredService<Form1>();
            sss.ShowDialog();
            //Application.Run(new Form1());
        }
    }
}
