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
            : base(username, password, email,"Admin")
        {
            CanManageUsers = true;
            CanManageCourses = true;
        }

        public void DisplayPermissions()
        {
            Console.WriteLine("Admin Permissions:");
            Console.WriteLine($"Manage Users: {CanManageUsers}");
            Console.WriteLine($"Manage Courses: {CanManageCourses}");
        }
    }
}
