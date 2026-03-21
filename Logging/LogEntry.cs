using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Week_1.Logging
    {
    public class LogEntry
        {
        public DateTime Timestamp { get; set; }
        public LogLevel Level { get; set; }
        public string Message { get; set; }
        public string Category { get; set; }
        public string Username { get; set; }
        public string StackTrace { get; set; }
        public Dictionary<string, string> AdditionalData { get; set; }

        public override string ToString()
            {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"[{Timestamp:yyyy-MM-dd HH:mm:ss}] [{Level}] [{Category}]");
            sb.AppendLine($"Message: {Message}");
            if (!string.IsNullOrEmpty(Username)) sb.AppendLine($"User: {Username}");
            if (!string.IsNullOrEmpty(StackTrace)) sb.AppendLine($"Stack Trace:\n{StackTrace}");
            if (AdditionalData != null && AdditionalData.Any())
                {
                sb.AppendLine("Additional Data:");
                foreach (var kvp in AdditionalData)
                    sb.AppendLine($"  {kvp.Key}: {kvp.Value}");
                }
            sb.AppendLine("─────────────────────────────────────────");
            return sb.ToString();
            }
        }
    }
