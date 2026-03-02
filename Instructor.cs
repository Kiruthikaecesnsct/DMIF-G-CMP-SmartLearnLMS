using System;
using System.Collections.Generic;

namespace Week_1
{
    public class Instructor : User, ISearchable, INotifiable, IReportable
    {
        public List<int> CourseIds { get; set; }
        private List<string> notificationHistory = new List<string>();

        public Instructor(string username, string password, string email)
            : base(username, password, email)
        {
            CourseIds = new List<int>();
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

        public void RemoveCourse(int courseId)
        {
            if (CourseIds.Remove(courseId))
                Console.WriteLine($"  ✓ Course ID {courseId} removed.");
            else
                Console.WriteLine("  Course not found in your list.");
        }

        public void ShowMyCourses()
        {
            Console.WriteLine($"  {Username}'s Course IDs: {string.Join(", ", CourseIds)}");
        }

        // --- Abstract overrides ---
        public override void DisplayInfo()
        {
            Console.WriteLine($"  Username : {Username}");
            Console.WriteLine($"  Email    : {Email}");
            Console.WriteLine($"  Role     : Instructor");
            Console.WriteLine($"  Courses  : {CourseIds.Count}");
        }

        public override void DisplayDashboard()
        {
            Console.WriteLine("\n=== INSTRUCTOR DASHBOARD ===");
            Console.WriteLine($"  Welcome, {Username}!");
            Console.WriteLine("\n  1. My Courses");
            Console.WriteLine("  2. View Students");
            Console.WriteLine("  3. Create Course");
            Console.WriteLine("  4. Logout");
        }

        public override string GetUserType() => "Instructor";

        // --- ISearchable ---
        public bool MatchesSearch(string keyword)
        {
            return Username.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                   (Email ?? "").Contains(keyword, StringComparison.OrdinalIgnoreCase);
        }

        public string GetSearchSummary() => $"{Username} ({Email}) [Instructor]";

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
            return $"Instructor Report | User: {Username} | Courses Teaching: {CourseIds.Count}";
        }

        public void DisplayReport()
        {
            Console.WriteLine($"\n  === Instructor Report: {Username} ===");
            Console.WriteLine($"  Email          : {Email}");
            Console.WriteLine($"  Courses taught : {CourseIds.Count}");
            Console.WriteLine($"  Course IDs     : {string.Join(", ", CourseIds)}");
        }
    }
}