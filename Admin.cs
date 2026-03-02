using System;
using System.Text.Json.Serialization;

namespace Week_1
{
    public class Admin : User
    {
        public bool CanManageUsers { get; set; }
        public bool CanManageCourses { get; set; }
        public string AdminLevel { get; set; }

        // ── Parameterless constructor for JSON ──
        public Admin() : base()
        {
            CanManageUsers = true;
            CanManageCourses = true;
            AdminLevel = "Super";
        }

        public Admin(string username, string password, string email)
            : base(username, password, email)
        {
            CanManageUsers = true;
            CanManageCourses = true;
            AdminLevel = "Super";
        }

        public override void DisplayDashboard()
        {
            Console.WriteLine("╔════════════════════════════════╗");
            Console.WriteLine("║        ADMIN DASHBOARD         ║");
            Console.WriteLine("╚════════════════════════════════╝");
            Console.WriteLine($"  Welcome, Admin {Username}!");
            Console.WriteLine($"  Level: {AdminLevel}");
            Console.WriteLine();
            Console.WriteLine("  [1] Manage Users");
            Console.WriteLine("  [2] Manage Courses");
            Console.WriteLine("  [3] View System Reports");
            Console.WriteLine("  [4] System Settings");
            Console.WriteLine("  [5] Logout");
        }

        public override string GetUserType() => "Admin";

        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Console.WriteLine($"  Role        : Admin");
            Console.WriteLine($"  Admin Level : {AdminLevel}");
            Console.WriteLine($"  Manage Users  : {(CanManageUsers ? "✓" : "✗")}");
            Console.WriteLine($"  Manage Courses: {(CanManageCourses ? "✓" : "✗")}");
        }
    }
}