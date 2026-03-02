using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Week_1
{
    public class Instructor : User, ISearchable
    {
        public List<int> CourseIds { get; set; }

        public Instructor(string username, string password, string email)
            : base(username, password, email, "Instructor")
        {
            CourseIds = new List<int>();
        }

        // ── Abstract method implementations ───────────────────────────────────
        public override void DisplayDashboard()
        {
            Console.WriteLine("\n=== INSTRUCTOR DASHBOARD ===");
            Console.WriteLine($"  Welcome, {Username}!");
            Console.WriteLine("  1. My Courses");
            Console.WriteLine("  2. View Students");
            Console.WriteLine("  3. Create Course");
            Console.WriteLine("  4. Logout");
        }

        public override string GetUserType() => "Instructor";

        // ── Course management ──────────────────────────────────────────────────
        public void AddCourse(int courseId)
        {
            if (!CourseIds.Contains(courseId))
            {
                CourseIds.Add(courseId);
                Console.WriteLine("✓ Course added!");
            }
            else
            {
                Console.WriteLine("❌ Course already exists!");
            }
        }

        public void RemoveCourse(int courseId)
        {
            if (CourseIds.Contains(courseId))
            {
                CourseIds.Remove(courseId);
                Console.WriteLine("✓ Course removed!");
            }
            else
            {
                Console.WriteLine("❌ Course not found!");
            }
        }

        public void ShowMyCourses()
        {
            Console.WriteLine($"\n  Courses taught by {Username}:");
            if (CourseIds.Count == 0)
            {
                Console.WriteLine("  (No courses yet)");
                return;
            }
            foreach (int courseId in CourseIds)
                Console.WriteLine($"  Course #{courseId}");
        }

        // ── ISearchable ────────────────────────────────────────────────────────
        public bool MatchesSearch(string keyword)
        {
            return Username.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                   Email.Contains(keyword, StringComparison.OrdinalIgnoreCase);
        }

        public string GetSearchSummary() => $"[Instructor] {Username} ({Email})";

        // ── IReportable override ───────────────────────────────────────────────
        public override string GenerateReport()
        {
            return $"Instructor Report | {Username} | Courses: {CourseIds.Count} | Registered: {DateRegistered:dd MMM yyyy}";
        }
    }
}
