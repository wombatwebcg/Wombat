using System;

namespace Wombat.Logger
{
    public class FileLogEntry
    {
        public string Text { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
}
