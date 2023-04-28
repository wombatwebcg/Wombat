using System;
using System.Collections.Generic;
using System.Text;

namespace Wombat.Core
{
   public static class LevelAlias
    {
        /// <summary>
        /// The least significant level of event.
        /// </summary>
        public const LogEventLevel Minimum = LogEventLevel.None;

        /// <summary>
        /// The most significant level of event.
        /// </summary>
        public const LogEventLevel Maximum = LogEventLevel.Fatal;

    }
}
