using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Week_1.Data;

namespace Week_1.Logging
    {

    public class AuditService
        {
        public static void LogAction(SmartLearnDbContext db, string action, string category,
            string username, int? userId = null, object details = null)
            {
            try
                {
                var detailsJson = details != null
                    ? System.Text.Json.JsonSerializer.Serialize(details)
                    : null;

                var auditLog = new AuditLog
                    {
                    Action = action,
                    Category = category,
                    Username = username,
                    UserId = userId,
                    Details = detailsJson,
                    Timestamp = DateTime.Now
                    };

                db.AuditLogs.Add(auditLog);
                db.SaveChanges();
                }
            catch { /* Don't let audit logging break the main operation */ }
            }

        public static List<AuditLog> GetUserActivity(SmartLearnDbContext db, string username, int days = 30)
            {
            var startDate = DateTime.Now.AddDays(-days);
            return db.AuditLogs
                .Where(a => a.Username == username && a.Timestamp >= startDate)
                .OrderByDescending(a => a.Timestamp)
                .ToList();
            }

        public static List<AuditLog> GetRecentActivity(SmartLearnDbContext db, int count = 50)
            {
            return db.AuditLogs
                .OrderByDescending(a => a.Timestamp)
                .Take(count)
                .ToList();
            }
        }
    }
