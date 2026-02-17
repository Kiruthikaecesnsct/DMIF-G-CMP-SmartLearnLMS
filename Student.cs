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

    public class Student : User, ISearchable
    {
        public List<int> EnrolledCourseIds { get; set; }
        public Dictionary<int, int> CourseProgress { get; set; }

        public Student(string username, string password, string email)
            : base(username, password, email)
        {
            EnrolledCourseIds = new List<int>();
            CourseProgress = new Dictionary<int, int>();
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

        public override string GetUserType()
        {
            return "Student";
        }

        public void EnrollInCourse(int courseId)
        {
            if (!EnrolledCourseIds.Contains(courseId))
            {
                EnrolledCourseIds.Add(courseId);
                CourseProgress[courseId] = 0;
                Console.WriteLine("✓ Successfully enrolled!");
            }
            else
            {
                Console.WriteLine("❌ Already enrolled in this course!");
            }
        }
        public void UpdateProgress(int courseId, int percentage)
        {
            // Check if courseId exists in CourseProgress 
            // If yes, update it 
            // If no, show error 
        }

        public void ShowEnrolledCourses()
        {
            // Loop through EnrolledCourseIds 
            // Display each with its progress 
        }
        public bool MatchesSearch(string keyword)
        {
            return Username.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                   Email.Contains(keyword, StringComparison.OrdinalIgnoreCase);
        }

        public string GetSearchSummary()
        {
            return $"{Username} ({Email})";
        }


    }

    //AASSIGNMENT IN SESSION
    //public class Student : User
    //{
    //    public List<int> EnrolledCourseIds { get; set; }
    //    public Dictionary<int, int> CourseProgress { get; set; }

    //    public Student(string username, string password, string email)
    //        : base(username, password, email)
    //    {
    //        EnrolledCourseIds = new List<int>();
    //        CourseProgress = new Dictionary<int, int>();
    //    }

    //    public void EnrollInCourse(int courseId)
    //    {
    //        if (!EnrolledCourseIds.Contains(courseId))
    //        {
    //            EnrolledCourseIds.Add(courseId);
    //            CourseProgress[courseId] = 0;
    //            Console.WriteLine("✓ Successfully enrolled!");
    //        }
    //        else
    //        {
    //            Console.WriteLine("❌ Already enrolled in this course!");
    //        }
    //    }

    //    public void UpdateProgress(int courseId, int percentage)
    //    {
    //        if (CourseProgress.ContainsKey(courseId))
    //        {
    //            CourseProgress[courseId] = percentage;
    //            Console.WriteLine($"✓ Progress updated to {percentage}%");
    //        }
    //        else
    //        {
    //            Console.WriteLine("❌ Not enrolled in this course!");
    //        }
    //    }

    //    public void ShowEnrolledCourses()
    //    {
    //        Console.WriteLine("Enrolled Courses:");
    //        foreach (int courseId in EnrolledCourseIds)
    //        {
    //            Console.WriteLine($"Course {courseId} - Progress: {CourseProgress[courseId]}%");
    //        }
    //    }
    //}






}
