using System;
using System.Collections.Generic;
using System.Text;

namespace Wombat.Infrastructure
{
   public class TimeStampHelper
    {
        public static string NowLong()
        {
            return DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:fff");
        }

        public static string NowShort()
        {
            return DateTime.Now.ToString("HH:mm:ss:fff");
        }

    }
}
