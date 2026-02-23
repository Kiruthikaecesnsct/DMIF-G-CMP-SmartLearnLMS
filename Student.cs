using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Week_1
{
    public class Student : User, ISearchable
    {
        // ❌ REMOVED duplicate Username — it already exists in User base class
        // public string Username { get; set; }

        public List<int> EnrolledCourseIds { get; set; }
        public Dictionary<int, int> CourseProgress { get; set; }
        public double ProgressPercentage { get; set; }

        // ✅ Parameterless constructor — required for JSON Deserialize
        public Student() : base()
        {
            EnrolledCourseIds = new List<int>();
            CourseProgress = new Dictionary<int, int>();
        }

        // ✅ Full constructor for creating new students
        public Student(string username, string password, string email)
            : base(username, password, email)
        {
            EnrolledCourseIds = new List<int>();
            CourseProgress = new Dictionary<int, int>();
        }

        // ✅ Sample data for demos
        public static List<Student> GetAllStudents()
        {
            return new List<Student>
            {
                new Student("alice", "pass123", "alice@email.com")  { ProgressPercentage = 92, EnrolledCourseIds = new List<int>{1, 2} },
                new Student("bob",   "pass123", "bob@email.com")    { ProgressPercentage = 65, EnrolledCourseIds = new List<int>{1, 3} },
                new Student("carol", "pass123", "carol@email.com")  { ProgressPercentage = 40, EnrolledCourseIds = new List<int>{2}    },
                new Student("dave",  "pass123", "dave@email.com")   { ProgressPercentage = 78, EnrolledCourseIds = new List<int>{3, 4} },
                new Student("eve",   "pass123", "eve@email.com")    { ProgressPercentage = 85, EnrolledCourseIds = new List<int>{1, 4} },
                new Student("frank", "pass123", "frank@email.com")  { ProgressPercentage = 30, EnrolledCourseIds = new List<int>{2, 3} },
            };
        }

        public void EnrollInCourse(int courseId)
        {
            if (!EnrolledCourseIds.Contains(courseId))
            {
                EnrolledCourseIds.Add(courseId);
                CourseProgress[courseId] = 0;
                Console.WriteLine("✅ Successfully enrolled!");
            }
            else
            {
                Console.WriteLine("❌ Already enrolled in this course!");
            }
        }

        public void UpdateProgress(int courseId, int percentage)
        {
            if (CourseProgress.ContainsKey(courseId))
            {
                CourseProgress[courseId] = percentage;
                Console.WriteLine($"✅ Progress updated to {percentage}%");
            }
            else
            {
                Console.WriteLine("❌ Not enrolled in this course!");
            }
        }

        public void ShowEnrolledCourses()
        {
            if (EnrolledCourseIds.Count == 0)
            {
                Console.WriteLine("No courses enrolled yet.");
                return;
            }

            Console.WriteLine("Enrolled Courses:");
            foreach (int courseId in EnrolledCourseIds)
            {
                int progress = CourseProgress.ContainsKey(courseId) ? CourseProgress[courseId] : 0;
                Console.WriteLine($"  Course {courseId} - Progress: {progress}%");
            }
        }

        public override void DisplayDashboard()
        {
            Console.WriteLine("\n=== STUDENT DASHBOARD ===");
            Console.WriteLine($"Welcome, {Username}!");
            Console.WriteLine("\n1. Browse Courses");
            Console.WriteLine("2. My Enrolled Courses");
            Console.WriteLine("3. Update Progress");
            Console.WriteLine("4. Logout");
        }

        public override string GetUserType() => "Student";

        public bool MatchesSearch(string keyword)
        {
            return Username.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                   Email.Contains(keyword, StringComparison.OrdinalIgnoreCase);
        }

        public string GetSearchSummary() => $"{Username} ({Email})";
    }
}