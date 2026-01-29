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

    public class Student : User
    {
        public List<int> EnrolledCourseIds { get; set; }
        public Dictionary<int, int> CourseProgress { get; set; }

        public Student(string username, string password, string email)
            : base(username, password, email)
        {
            EnrolledCourseIds = new List<int>();
            CourseProgress = new Dictionary<int, int>();
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
