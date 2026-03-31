using System;
using System.IO;
using System.Text;

namespace Week_1.Logging
    {
    // ══════════════════════════════════════════════════════════════════
    //  ASSIGNMENT 8 — PART 3 TASK 1: Logger
    // ══════════════════════════════════════════════════════════════════

    /// <summary>Severity levels for log entries.</summary>
    public enum LogLevel { Debug, Info, Warning, Error, Critical }

    /// <summary>
    /// Thread-safe logger that writes to the console (colour-coded) and
    /// to daily rotating log files in the <c>Logs/</c> folder.
    /// </summary>
    public class Logger
        {
        // ── Singleton ──────────────────────────────────────────────────
        private static readonly Lazy<Logger> _instance =
            new(() => new Logger());

        /// <summary>Application-wide logger instance.</summary>
        public static Logger Instance => _instance.Value;

        // ── State ──────────────────────────────────────────────────────
        private readonly object _fileLock = new();
        private readonly string _logDirectory;

        private Logger()
            {
            _logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            Directory.CreateDirectory(_logDirectory);
            }

        // ── Core method ────────────────────────────────────────────────

        /// <summary>Writes a log entry to the console and to today's log file.</summary>
        public void Log(LogLevel level, string category, string message,
                        string username = null, Exception exception = null,
                        string additionalData = null)
            {
            var entry = BuildEntry(level, category, message, username, exception, additionalData);
            WriteToConsole(level, entry);
            WriteToFile(entry);
            }

        // ── Convenience wrappers ───────────────────────────────────────

        /// <summary>Logs a Debug-level message (development diagnostics).</summary>
        public void Debug(string category, string message, string username = null)
            => Log(LogLevel.Debug, category, message, username);

        /// <summary>Logs an Info-level message (normal application events).</summary>
        public void Info(string category, string message, string username = null)
            => Log(LogLevel.Info, category, message, username);

        /// <summary>Logs a Warning-level message (unexpected but non-fatal).</summary>
        public void Warning(string category, string message, string username = null)
            => Log(LogLevel.Warning, category, message, username);

        /// <summary>Logs an Error-level message (operation failures).</summary>
        public void Error(string category, string message, string username = null,
                          Exception exception = null)
            => Log(LogLevel.Error, category, message, username, exception);

        /// <summary>Logs a Critical-level message (system-threatening failures).</summary>
        public void Critical(string category, string message, string username = null,
                              Exception exception = null)
            => Log(LogLevel.Critical, category, message, username, exception);

        // ── Private helpers ────────────────────────────────────────────

        private static string BuildEntry(LogLevel level, string category, string message,
                                         string username, Exception exception,
                                         string additionalData)
            {
            var sb = new StringBuilder();
            sb.Append($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}]");
            sb.Append($" [{level.ToString().ToUpper(),-8}]");
            sb.Append($" [{category,-20}]");

            if (!string.IsNullOrWhiteSpace(username))
                sb.Append($" [User: {username}]");

            sb.Append($" {message}");

            if (!string.IsNullOrWhiteSpace(additionalData))
                sb.Append($" | Data: {additionalData}");

            if (exception != null)
                {
                sb.AppendLine();
                sb.Append($"  Exception : {exception.GetType().Name}: {exception.Message}");
                if (exception.StackTrace != null)
                    {
                    sb.AppendLine();
                    sb.Append($"  StackTrace: {exception.StackTrace.Trim()}");
                    }
                }

            return sb.ToString();
            }

        private static void WriteToConsole(LogLevel level, string entry)
            {
            var savedColor = Console.ForegroundColor;
            Console.ForegroundColor = level switch
                {
                    LogLevel.Debug => ConsoleColor.Gray,
                    LogLevel.Info => ConsoleColor.Cyan,
                    LogLevel.Warning => ConsoleColor.Yellow,
                    LogLevel.Error => ConsoleColor.Red,
                    LogLevel.Critical => ConsoleColor.Magenta,
                    _ => ConsoleColor.White
                    };
            Console.WriteLine(entry);
            Console.ForegroundColor = savedColor;
            }

        private void WriteToFile(string entry)
            {
            try
                {
                // Daily file: Logs/SmartLearn_2025-12-01.log
                string filePath = Path.Combine(
                    _logDirectory,
                    $"SmartLearn_{DateTime.Now:yyyy-MM-dd}.log");

                lock (_fileLock)
                    {
                    File.AppendAllText(filePath, entry + Environment.NewLine);
                    }
                }
            catch
                {
                // Logging must never crash the application
                }
            }
        }
    }