using System;
using System.Collections.Generic;

namespace Week_1
{
    public class Admin : User, INotifiable, IReportable
    {
        private List<string> permissions = new List<string> { "ManageUsers", "ManageCourses", "ViewStats", "SystemConfig" };
        private List<string> notificationHistory = new List<string>();

        public Admin(string username, string password, string email)
            : base(username, password, email)
        {
        }

        public void DisplayPermissions()
        {
            Console.WriteLine("  Permissions:");
            foreach (string p in permissions)
                Console.WriteLine($"    ✓ {p}");
        }

        // --- Abstract overrides ---
        public override void DisplayInfo()
        {
            Console.WriteLine($"  Username : {Username}");
            Console.WriteLine($"  Email    : {Email}");
            Console.WriteLine($"  Role     : Admin");
        }

        public override void DisplayDashboard()
        {
            Console.WriteLine("\n=== ADMIN DASHBOARD ===");
            Console.WriteLine($"  Welcome, {Username}!");
            Console.WriteLine("\n  1. Manage Users");
            Console.WriteLine("  2. Manage Courses");
            Console.WriteLine("  3. System Stats");
            Console.WriteLine("  4. Logout");
        }

        public override string GetUserType() => "Admin";

        // --- INotifiable ---
        public void SendNotification(string message)
        {
            string entry = $"[{DateTime.Now:g}] {message}";
            notificationHistory.Add(entry);
            Console.WriteLine($"  🔔 Notification: {message}");
        }

        public List<string> GetNotificationHistory() => notificationHistory;

        // --- IReportable ---
        public string GenerateReport()
        {
            return $"Admin Report | User: {Username} | Permissions: {permissions.Count}";
        }

        public void DisplayReport()
        {
            Console.WriteLine($"\n  === Admin Report: {Username} ===");
            Console.WriteLine($"  Email       : {Email}");
            DisplayPermissions();
        }
    }
}