using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Week_1
{

    //public class Student : User
    //{
    //    public Student(string username, string password, string email)
    //: base(username, password, email)
    //    {
    //    }

    //}

    //ASSIGNMENT IN SESSION
    public class Student : User, ISearchable
    {
        public List<int> EnrolledCourseIds { get; set; }

        private Dictionary<int, int> _courseProgress = new Dictionary<int, int>();
        public Dictionary<int, int> CourseProgress => _courseProgress;

        // Validated progress
        private int _progressPercentage;
        public int ProgressPercentage
        {
            get => _progressPercentage;
            set
            {
                if (value < 0 || value > 100)
                {
                    Console.WriteLine("✗ Progress must be between 0 and 100");
                    return;
                }
                _progressPercentage = value;
            }
        }

        public Student(string username, string password, string email)
            : base(username, password, email, "Student")
        {
            EnrolledCourseIds = new List<int>();
        }

        // ── Abstract method implementations ───────────────────────────────────
        public override void DisplayDashboard()
        {
            Console.WriteLine("\n=== STUDENT DASHBOARD ===");
            Console.WriteLine($"  Welcome, {Username}!");
            Console.WriteLine("  1. Browse Courses");
            Console.WriteLine("  2. My Enrolled Courses");
            Console.WriteLine("  3. Update Progress");
            Console.WriteLine("  4. Logout");
        }

        public override string GetUserType() => "Student";

        // ── Enrolment methods ──────────────────────────────────────────────────
        public void EnrollInCourse(int courseId)
        {
            if (!EnrolledCourseIds.Contains(courseId))
            {
                EnrolledCourseIds.Add(courseId);
                _courseProgress[courseId] = 0;
                Console.WriteLine("✓ Successfully enrolled!");
                SendNotification($"You have been enrolled in Course #{courseId}");
            }
            else
            {
                Console.WriteLine("❌ Already enrolled in this course!");
            }
        }

        public void UpdateProgress(int courseId, int percentage)
        {
            if (_courseProgress.ContainsKey(courseId))
            {
                if (percentage < 0 || percentage > 100)
                {
                    Console.WriteLine("✗ Progress must be 0-100");
                    return;
                }
                _courseProgress[courseId] = percentage;
                Console.WriteLine($"✓ Progress updated to {percentage}%");
                if (percentage >= 100)
                    SendNotification($"🎉 You completed Course #{courseId}!");
            }
            else
            {
                Console.WriteLine("❌ Not enrolled in this course!");
            }
        }

        public void ShowEnrolledCourses()
        {
            Console.WriteLine($"\n  Enrolled Courses for {Username}:");
            if (EnrolledCourseIds.Count == 0)
            {
                Console.WriteLine("  (No courses enrolled yet)");
                return;
            }
            foreach (int courseId in EnrolledCourseIds)
                Console.WriteLine($"  Course #{courseId} — Progress: {_courseProgress[courseId]}%");
        }

        // ── ISearchable ────────────────────────────────────────────────────────
        public bool MatchesSearch(string keyword)
        {
            return Username.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                   Email.Contains(keyword, StringComparison.OrdinalIgnoreCase);
        }

        public string GetSearchSummary() => $"[Student] {Username} ({Email})";

        // ── IReportable override ───────────────────────────────────────────────
        public override string GenerateReport()
        {
            return $"Student Report | {Username} | Enrolled Courses: {EnrolledCourseIds.Count} | Registered: {DateRegistered:dd MMM yyyy}";
        }
    }






}
