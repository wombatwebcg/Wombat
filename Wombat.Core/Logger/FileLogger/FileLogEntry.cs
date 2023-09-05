using System;

namespace Microsoft.Extensions.Logging.File
{
    public class FileLogEntry
    {
        public string Text { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
}
