using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Week_1.Logging
    {

    public class AuditLog
        {
        [Key]
        public int AuditLogId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string Action { get; set; }
        public string Category { get; set; }
        public string Username { get; set; }
        public int? UserId { get; set; }
        public string Details { get; set; }
        public string IpAddress { get; set; }
        }
    }
