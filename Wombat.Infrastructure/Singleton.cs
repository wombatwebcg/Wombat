using System;
using System.Collections.Generic;
using System.Text;

namespace Wombat.Infrastructure
{
    public sealed class Singleton
    {
        private volatile static Singleton instance = null;
        private static readonly object padlock = new object();

        private Singleton()
        {
        }

        public static Singleton Instance
        {
            get
            {
                if (instance == null)
                {
                    // 当第一个线程运行到这里时，此时会对locker对象 "加锁"，
                    // 当第二个线程运行该方法时，首先检测到locker对象为"加锁"状态，
                    // 该线程就会挂起等待第一个线程解锁
                    // 第一个线程运行完之后, 会对该对象"解锁"
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = new Singleton();
                        }
                    }
                }
                return instance;
            }
        }
    }
}
