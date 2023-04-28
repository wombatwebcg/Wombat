using System;
using System.Threading.Tasks;
using Wombat.Core;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            AsyncLock asyncLock = new AsyncLock();
            var logger = new LoggerBuilder().LogEventLevel(LogEventLevel.Warning).UseConsoleLogger().UseFileLogger().CreateLogger();
            logger.Debug("1111");
            logger.Error("2222");
            logger.Info("33333");
            logger.Warning("4444");
            logger.Debug("1111");
            logger.Debug("1111");

            Task.Run(() =>
            {
                int c = 0;
                while (c < 500)
                {
                    c++;
                    logger.Warning("2222");
                }

            });
            Task.Run(() =>
            {
                int c = 0;
                while (c<500)
                {
                    c++;
                    logger.Warning("3333");
                }

            });

            Console.ReadLine();
            Console.ReadLine();

        }
    }
}
