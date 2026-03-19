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
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? InstructorId { get; set; }
        [NotMapped]
        public string? Specialization { get; set; }
        [NotMapped]
        public int YearsOfExperience { get; set; }

        [NotMapped]
        public List<int> CourseIds { get; set; }

        // ✅ [NotMapped] REMOVED — EF Core needs this for .Include(i => i.Courses)
        public List<Course> Courses { get; set; } = new List<Course>();

        public Instructor() : base()
            {
            CourseIds = new List<int>();
            }

        public Instructor(string username, string password, string email)
            : base(username, password, email)
            {
            CourseIds = new List<int>();
            }

        public override void DisplayDashboard()
            {
            Console.WriteLine("\n=== INSTRUCTOR DASHBOARD ===");
            Console.WriteLine($"Welcome, {Username}!");
            Console.WriteLine("\n1. My Courses");
            Console.WriteLine("2. View Students");
            Console.WriteLine("3. Create Course");
            Console.WriteLine("4. Logout");
            }

        public override string GetUserType() => "Instructor";

        public void AddCourse(int courseId)
            {
            if (!CourseIds.Contains(courseId))
                { CourseIds.Add(courseId); Console.WriteLine("✓ Course added!"); }
            else
                Console.WriteLine("❌ Course already exists!");
            }

        public void RemoveCourse(int courseId)
            {
            if (CourseIds.Contains(courseId))
                { CourseIds.Remove(courseId); Console.WriteLine("✓ Course removed!"); }
            else
                Console.WriteLine("❌ Course not found!");
            }

        public void ShowMyCourses()
            {
            Console.WriteLine("My Courses:");
            foreach (int courseId in CourseIds)
                Console.WriteLine($"Course ID: {courseId}");
            }
        }
    }