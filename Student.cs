using System;
using System.Collections.Generic;
using System.Linq;

namespace Week_1
{
    public class Student : User, ISearchable, INotifiable
    {
        public List<int> EnrolledCourseIds { get; set; }
        public Dictionary<int, int> CourseProgress { get; set; }

        private int _progressPercentage;
        public int ProgressPercentage
        {
            get => _progressPercentage;
            set
            {
                if (value < 0 || value > 100)
                {
                    Console.WriteLine("  ✗ Progress must be 0-100");
                    return;
                }
                _progressPercentage = value;
            }
        }

        private List<string> _notifications;

        public Student(string username, string password, string email)
            : base(username, password, email)
        {
            EnrolledCourseIds = new List<int>();
            CourseProgress = new Dictionary<int, int>();
            _notifications = new List<string>();
        }

        public void EnrollInCourse(int courseId)
        {
            if (!EnrolledCourseIds.Contains(courseId))
            {
                EnrolledCourseIds.Add(courseId);
                CourseProgress[courseId] = 0;
            }
        }

        // ── Abstract overrides ──
        public override void DisplayDashboard()
        {
            Console.WriteLine("╔════════════════════════════════╗");
            Console.WriteLine("║        STUDENT DASHBOARD       ║");
            Console.WriteLine("╚════════════════════════════════╝");
            Console.WriteLine($"  Welcome, {Username}!");
            Console.WriteLine($"  Enrolled Courses: {EnrolledCourseIds.Count}");
            Console.WriteLine();
            Console.WriteLine("  [1] Browse Available Courses");
            Console.WriteLine("  [2] My Enrolled Courses");
            Console.WriteLine("  [3] Update Progress");
            Console.WriteLine("  [4] My Statistics");
            Console.WriteLine("  [5] Rate a Course");
            Console.WriteLine("  [6] My Notifications");
            Console.WriteLine("  [7] Logout");
        }

        public override string GetUserType() => "Student";

        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Console.WriteLine($"  Role       : Student");
            Console.WriteLine($"  Courses    : {EnrolledCourseIds.Count}");
            double avgProgress = CourseProgress.Count > 0 ? CourseProgress.Values.Average() : 0;
            Console.WriteLine($"  Avg Progress: {avgProgress:F1}%");
        }

        // ── ISearchable ──
        public bool MatchesSearch(string keyword)
        {
            return Username.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                   (Email ?? "").Contains(keyword, StringComparison.OrdinalIgnoreCase);
        }

        public string GetSearchSummary()
        {
            return $"[Student] {Username} ({Email}) - {EnrolledCourseIds.Count} courses";
        }

        // ── INotifiable ──
        public void SendNotification(string message)
        {
            string entry = $"[{DateTime.Now:g}] {message}";
            _notifications.Add(entry);
            Console.WriteLine($"  🔔 {message}");
        }

        public List<string> GetNotificationHistory() => new List<string>(_notifications);
    }
}