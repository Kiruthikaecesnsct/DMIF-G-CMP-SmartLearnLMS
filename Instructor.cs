using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Week_1
    {
    public class Instructor : User
        {
        // [Key] = primary key for the Instructors table
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InstructorId { get; set; }

        // [NotMapped] = EF Core cannot store List<int> as a column
        // Kept for your existing in-memory logic
        [NotMapped]
        public List<int> CourseIds { get; set; }

        // Navigation property — EF Core uses this to load related Courses
        // [NotMapped] until migrations are done, then remove this line
        [NotMapped]
        public List<Course> Courses { get; set; } = new List<Course>();

        // Parameterless constructor — required by EF Core
        public Instructor() : base()
            {
            CourseIds = new List<int>();
            }

        // Your existing constructor — unchanged
        public Instructor(string username, string password, string email)
            : base(username, password, email)
            {
            CourseIds = new List<int>();
            }

        // All methods below unchanged
        public override void DisplayDashboard()
            {
            Console.WriteLine("\n=== INSTRUCTOR DASHBOARD ===");
            Console.WriteLine($"Welcome, {Username}!");
            Console.WriteLine("\n1. My Courses");
            Console.WriteLine("2. View Students");
            Console.WriteLine("3. Create Course");
            Console.WriteLine("4. Logout");
            }

        public override string GetUserType()
            {
            return "Instructor";
            }

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
            Console.WriteLine("My Courses:");
            foreach (int courseId in CourseIds)
                {
                Console.WriteLine($"Course ID: {courseId}");
                }
            }
        }
    }