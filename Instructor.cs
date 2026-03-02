using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Week_1
{
    public class Instructor : User, INotifiable
    {
        public List<int> CourseIds { get; set; }
        public string Department { get; set; }

        [JsonIgnore]
        private List<string> _notifications;

        // ── Parameterless constructor for JSON ──
        public Instructor() : base()
        {
            CourseIds = new List<int>();
            Department = "Computer Science";
            _notifications = new List<string>();
        }

        public Instructor(string username, string password, string email)
            : base(username, password, email)
        {
            CourseIds = new List<int>();
            Department = "Computer Science";
            _notifications = new List<string>();
        }

        public void AddCourse(int courseId)
        {
            if (!CourseIds.Contains(courseId))
            {
                CourseIds.Add(courseId);
                Console.WriteLine($"  ✓ Course ID {courseId} added.");
            }
            else
                Console.WriteLine("  Course already in your list.");
        }

        public override void DisplayDashboard()
        {
            Console.WriteLine("╔════════════════════════════════╗");
            Console.WriteLine("║      INSTRUCTOR DASHBOARD      ║");
            Console.WriteLine("╚════════════════════════════════╝");
            Console.WriteLine($"  Welcome, Professor {Username}!");
            Console.WriteLine($"  Department: {Department}");
            Console.WriteLine($"  Teaching {CourseIds.Count} course(s)");
            Console.WriteLine();
            Console.WriteLine("  [1] My Courses");
            Console.WriteLine("  [2] Create New Course");
            Console.WriteLine("  [3] View Student Roster");
            Console.WriteLine("  [4] Grade Assignments");
            Console.WriteLine("  [5] My Notifications");
            Console.WriteLine("  [6] Logout");
        }

        public override string GetUserType() => "Instructor";

        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Console.WriteLine($"  Role       : Instructor");
            Console.WriteLine($"  Department : {Department}");
            Console.WriteLine($"  Teaching   : {CourseIds.Count} course(s)");
        }

        public void SendNotification(string message)
        {
            if (_notifications == null) _notifications = new List<string>();
            _notifications.Add($"[{DateTime.Now:g}] {message}");
            Console.WriteLine($"  🔔 {message}");
        }

        public List<string> GetNotificationHistory()
        {
            if (_notifications == null) _notifications = new List<string>();
            return new List<string>(_notifications);
        }
    }
}