using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

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

        [JsonIgnore]
        private List<string> _notifications;

        // ── Parameterless constructor required for JSON deserialization ──
        public Student() : base("", "", "")
        {
            EnrolledCourseIds = new List<int>();
            CourseProgress = new Dictionary<int, int>();
            _notifications = new List<string>();
        }

        public Student(string username, string password, string email)
            : base(username, password, email)
        {
            EnrolledCourseIds = new List<int>();
            CourseProgress = new Dictionary<int, int>();
            _notifications = new List<string>();
        }

        // ── Static sample data factory ──
        public static List<Student> GetAllStudents()
        {
            return new List<Student>
            {
                new Student("alice",   "password1", "alice@email.com")   { ProgressPercentage = 85 },
                new Student("bob",     "password2", "bob@email.com")     { ProgressPercentage = 45 },
                new Student("charlie", "password3", "charlie@email.com") { ProgressPercentage = 92 },
                new Student("diana",   "password4", "diana@email.com")   { ProgressPercentage = 78 },
                new Student("eve",     "password5", "eve@email.com")     { ProgressPercentage = 30 },
            };
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
            Console.WriteLine($"  Enrolled Courses : {EnrolledCourseIds.Count}");
            Console.WriteLine($"  Progress         : {ProgressPercentage}%");
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
            Console.WriteLine($"  Role         : Student");
            Console.WriteLine($"  Progress     : {ProgressPercentage}%");
            Console.WriteLine($"  Courses      : {EnrolledCourseIds.Count}");
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
            if (_notifications == null) _notifications = new List<string>();
            string entry = $"[{DateTime.Now:g}] {message}";
            _notifications.Add(entry);
            Console.WriteLine($"  🔔 {message}");
        }

        public List<string> GetNotificationHistory()
        {
            if (_notifications == null) _notifications = new List<string>();
            return new List<string>(_notifications);
        }
    }
}