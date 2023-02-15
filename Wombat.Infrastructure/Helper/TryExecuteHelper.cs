using System;
using System.Collections.Generic;
using System.Text;

namespace Wombat.Infrastructure
{
    public class TryExecuteHelper
    {

        public static bool MultipleTryExecute(bool cancel, Func<bool> exceute, Action successEvent = null, Action failEvent = null, int tryTimes = 3)
        {
            int localTimes = 0;
            while (cancel)
            {
                if (exceute.Invoke())
                {
                    successEvent?.Invoke();
                    return true;
                }
                else
                {
                    if (tryTimes > localTimes)
                    {
                        failEvent?.Invoke();
                        localTimes++;
                    }
                    else
                    {
                        return false;

                    }

                }

            }
            return false;

        }
    }
}
