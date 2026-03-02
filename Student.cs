using System;
using System.Collections.Generic;
using System.Linq;

namespace Week_1
{
    public class Student : User, ISearchable, INotifiable, IReportable
    {
        public List<int> EnrolledCourseIds { get; set; }
        public Dictionary<int, int> CourseProgress { get; set; }

        // Notification history
        private List<string> notificationHistory = new List<string>();

        // Email validation backing field
        private string _email;
        public new string Email
        {
            get => _email;
            set
            {
                if (string.IsNullOrWhiteSpace(value) || !value.Contains("@"))
                {
                    Console.WriteLine("  ✗ Invalid email");
                    return;
                }
                _email = value;
            }
        }

        // Progress validation backing field
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

        public Student(string username, string password, string email)
            : base(username, password, email)
        {
            _email = email;
            EnrolledCourseIds = new List<int>();
            CourseProgress = new Dictionary<int, int>();
        }

        public void EnrollInCourse(int courseId)
        {
            if (!EnrolledCourseIds.Contains(courseId))
            {
                EnrolledCourseIds.Add(courseId);
                CourseProgress[courseId] = 0;
                Console.WriteLine($"  ✓ Enrolled in course ID {courseId}");
                SendNotification($"You have been enrolled in course ID {courseId}.");
            }
            else
            {
                Console.WriteLine("  Already enrolled in this course.");
            }
        }

        // --- Abstract overrides ---
        public override void DisplayInfo()
        {
            Console.WriteLine($"  Username : {Username}");
            Console.WriteLine($"  Email    : {Email}");
            Console.WriteLine($"  Role     : Student");
            Console.WriteLine($"  Courses  : {EnrolledCourseIds.Count}");
        }

        public override void DisplayDashboard()
        {
            Console.WriteLine("\n=== STUDENT DASHBOARD ===");
            Console.WriteLine($"  Welcome, {Username}!");
            Console.WriteLine("\n  1. Browse Courses");
            Console.WriteLine("  2. My Enrolled Courses");
            Console.WriteLine("  3. Update Progress");
            Console.WriteLine("  4. Logout");
        }

        public override string GetUserType() => "Student";

        // --- ISearchable ---
        public bool MatchesSearch(string keyword)
        {
            return Username.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                   (Email ?? "").Contains(keyword, StringComparison.OrdinalIgnoreCase);
        }

        public string GetSearchSummary() => $"{Username} ({Email}) [Student]";

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
            return $"Student Report | User: {Username} | Courses: {EnrolledCourseIds.Count} | Progress entries: {CourseProgress.Count}";
        }

        public void DisplayReport()
        {
            Console.WriteLine($"\n  === Student Report: {Username} ===");
            Console.WriteLine($"  Email        : {Email}");
            Console.WriteLine($"  Enrolled in  : {EnrolledCourseIds.Count} course(s)");
            foreach (var kv in CourseProgress)
                Console.WriteLine($"    Course ID {kv.Key}: {kv.Value}%");
        }
    }
}