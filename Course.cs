using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Week_1
{
    public class Course
    {
        // Properties
        public int CourseId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string InstructorName { get; set; }
        public int MaxStudents { get; set; }
        public int CurrentEnrollments { get; set; }

        // Constructor
        public Course(int courseId, string title, string description, string instructorName, int maxStudents, int currentEnrollments)
        {
            CourseId = courseId;
            Title = title;
            Description = description;
            InstructorName = instructorName;
            MaxStudents = maxStudents;
            CurrentEnrollments = currentEnrollments;
        }

        // Method to check if course can accept enrollments
        public bool CanEnroll()
        {
            return CurrentEnrollments < MaxStudents;
        }

        // Method to display course information
        public void DisplayInfo()
        {
            Console.WriteLine($"Course ID: {CourseId}");
            Console.WriteLine($"Title: {Title}");
            Console.WriteLine($"Description: {Description}");
            Console.WriteLine($"Instructor: {InstructorName}");
            Console.WriteLine($"Enrollment: {CurrentEnrollments}/{MaxStudents}");
            Console.WriteLine($"Available: {(CanEnroll() ? "Yes" : "No")}");
        }
    }
}
