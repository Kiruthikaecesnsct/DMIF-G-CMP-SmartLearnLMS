using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Week_1.Logging
    {
    public class Logger
        {
        private static readonly string LogDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
        private static readonly string LogFile = Path.Combine(LogDirectory, $"smartlearn_{DateTime.Now:yyyyMMdd}.log");
        private static readonly object _lock = new object();

        static Logger()
            {
            if (!Directory.Exists(LogDirectory))
                Directory.CreateDirectory(LogDirectory);
            }

        public static void Log(LogLevel level, string category, string message,
            string username = null, Exception ex = null,
            Dictionary<string, string> additionalData = null)
            {
            var entry = new LogEntry
                {
                Timestamp = DateTime.Now,
                Level = level,
                Category = category,
                Message = message,
                Username = username,
                StackTrace = ex?.StackTrace,
                AdditionalData = additionalData
                };
            WriteToConsole(entry);
            WriteToFile(entry);
            }

        private static void WriteToConsole(LogEntry entry)
            {
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = entry.Level switch
                {
                    LogLevel.Debug => ConsoleColor.Gray,
                    LogLevel.Info => ConsoleColor.White,
                    LogLevel.Warning => ConsoleColor.Yellow,
                    LogLevel.Error => ConsoleColor.Red,
                    LogLevel.Critical => ConsoleColor.DarkRed,
                    _ => ConsoleColor.White
                    };
            Console.WriteLine(entry.ToString());
            Console.ForegroundColor = originalColor;
            }

        private static void WriteToFile(LogEntry entry)
            {
            try
                {
                lock (_lock)
                    {
                    File.AppendAllText(LogFile, entry.ToString() + Environment.NewLine);
                    }
                }
            catch { /* Can't log a logging error — just continue */ }
            }

        public static void Debug(string category, string message, string username = null)
            => Log(LogLevel.Debug, category, message, username);

        public static void Info(string category, string message, string username = null)
            => Log(LogLevel.Info, category, message, username);

        public static void Warning(string category, string message, string username = null)
            => Log(LogLevel.Warning, category, message, username);

        public static void Error(string category, string message, string username = null, Exception ex = null)
            => Log(LogLevel.Error, category, message, username, ex);

        public static void Critical(string category, string message, string username = null, Exception ex = null)
            => Log(LogLevel.Critical, category, message, username, ex);
        }
    }
