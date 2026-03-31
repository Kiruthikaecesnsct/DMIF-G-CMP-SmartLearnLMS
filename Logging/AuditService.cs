using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;

namespace Week_1.Logging
    {
    // ══════════════════════════════════════════════════════════════════
    //  ASSIGNMENT 8 — PART 3 TASK 3: AuditLog Entity & AuditService
    // ══════════════════════════════════════════════════════════════════

    // ── Entity ─────────────────────────────────────────────────────────

    /// <summary>
    /// Represents a single audit trail entry persisted to the database.
    /// </summary>
    public class AuditLog
        {
        [Key]
        public int AuditLogId { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;

        // All string columns are nullable so EF Core maps them to NULL-able SQL columns.
        // Removing [Required] on Action also fixes NOT NULL constraint violations when
        // the table was previously created with stricter DDL.
        [MaxLength(50)]
        public string? Action { get; set; }

        [MaxLength(50)]
        public string? Category { get; set; }

        [MaxLength(50)]
        public string? Username { get; set; }

        public int? UserId { get; set; }

        /// <summary>JSON-serialised additional detail dictionary.</summary>
        public string? Details { get; set; }

        [MaxLength(45)]
        public string? IpAddress { get; set; }
        }

    // ── Service ────────────────────────────────────────────────────────

    /// <summary>
    /// Records business-critical events to the <see cref="AuditLog"/> table
    /// and provides query methods for reviewing activity.
    /// </summary>
    public class AuditService
        {
        private static readonly Logger _log = Logger.Instance;

        // ── LogAction ─────────────────────────────────────────────────

        /// <summary>
        /// Persists one audit entry for a user-triggered action.
        /// </summary>
        /// <param name="action">Short action name, e.g. "UserRegistered".</param>
        /// <param name="category">Domain area, e.g. "Auth", "Enrollment".</param>
        /// <param name="username">Username of the actor (may be null for system).</param>
        /// <param name="userId">Database ID of the actor (nullable).</param>
        /// <param name="details">Dictionary of extra key-value pairs serialised to JSON.</param>
        /// <param name="ipAddress">Client IP, if available.</param>
        public void LogAction(string action, string category,
                              string username = null, int? userId = null,
                              Dictionary<string, object> details = null,
                              string ipAddress = null)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();

                var entry = new AuditLog
                    {
                    Action = action,
                    Category = category,
                    Username = username,
                    UserId = userId,
                    IpAddress = ipAddress,
                    Details = details != null
                                ? JsonSerializer.Serialize(details)
                                : null
                    };

                ctx.AuditLogs.Add(entry);
                ctx.SaveChanges();

                _log.Info("AuditService", $"[AUDIT] {action} | {category} | User: {username ?? "system"}",
                          username);
                }
            catch (Exception ex)
                {
                // Audit failures must never crash business operations
                _log.Error("AuditService", $"Failed to write audit entry: {ex.Message}", username, ex);
                }
            }

        // ── GetUserActivity ───────────────────────────────────────────

        /// <summary>
        /// Returns the audit history for a specific user, newest first.
        /// </summary>
        public List<AuditLog> GetUserActivity(int userId, int maxRecords = 50)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                return ctx.AuditLogs
                    .Where(a => a.UserId == userId)
                    .OrderByDescending(a => a.Timestamp)
                    .Take(maxRecords)
                    .ToList();
                }
            catch (Exception ex)
                {
                _log.Error("AuditService", $"GetUserActivity failed: {ex.Message}", null, ex);
                return new List<AuditLog>();
                }
            }

        // ── GetRecentActivity ─────────────────────────────────────────

        /// <summary>
        /// Returns the most recent system-wide audit entries.
        /// </summary>
        public List<AuditLog> GetRecentActivity(int maxRecords = 100)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                return ctx.AuditLogs
                    .OrderByDescending(a => a.Timestamp)
                    .Take(maxRecords)
                    .ToList();
                }
            catch (Exception ex)
                {
                _log.Error("AuditService", $"GetRecentActivity failed: {ex.Message}", null, ex);
                return new List<AuditLog>();
                }
            }

        // ── Display ───────────────────────────────────────────────────

        /// <summary>Prints an audit log list to the console.</summary>
        public void DisplayAuditLog(List<AuditLog> logs)
            {
            if (!logs.Any()) { Console.WriteLine("  No audit entries found."); return; }

            Console.WriteLine($"\n  {"Timestamp",-22}{"Action",-26}{"Category",-16}{"User",-20}Details");
            Console.WriteLine("  " + new string('─', 100));
            foreach (var a in logs)
                {
                string detail = a.Details?.Length > 40
                    ? a.Details[..37] + "..."
                    : (a.Details ?? "");
                Console.WriteLine($"  {a.Timestamp:yyyy-MM-dd HH:mm:ss,-22}{a.Action,-26}{a.Category,-16}{a.Username ?? "system",-20}{detail}");
                }
            }
        }
    }