using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Week_1
{
    public class Admin : User
    {
        public bool CanManageUsers { get; set; }
        public bool CanManageCourses { get; set; }

        public Admin(string username, string password, string email)
            : base(username, password, email, "Admin")
        {
            CanManageUsers = true;
            CanManageCourses = true;
        }

        public override void DisplayDashboard()
        {
            Console.WriteLine("\n=== ADMIN DASHBOARD ===");
            Console.WriteLine($"  Welcome, {Username}!");
            Console.WriteLine("  1. Manage Users");
            Console.WriteLine("  2. Manage Courses");
            Console.WriteLine("  3. System Stats");
            Console.WriteLine("  4. Logout");
        }

        public override string GetUserType() => "Admin";

        public void DisplayPermissions()
        {
            Console.WriteLine("\n  Admin Permissions:");
            Console.WriteLine($"  Manage Users   : {CanManageUsers}");
            Console.WriteLine($"  Manage Courses : {CanManageCourses}");
        }

        public override string GenerateReport()
        {
            return $"Admin Report | {Username} | ManageUsers: {CanManageUsers} | ManageCourses: {CanManageCourses}";
        }
    }
}
