using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Week_1
{
    public class Enrollment
    {
        // Properties
        public int EnrollmentId { get; set; }
        public string StudentUsername { get; set; }
        public int CourseId { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public int ProgressPercentage { get; set; }
        public bool IsCompleted { get; set; }

        // Constructor
        public Enrollment(int enrollmentId, string studentUsername, int courseId, DateTime enrollmentDate, int progressPercentage, bool isCompleted)
        {
            EnrollmentId = enrollmentId;
            StudentUsername = studentUsername;
            CourseId = courseId;
            EnrollmentDate = enrollmentDate;
            ProgressPercentage = progressPercentage;
            IsCompleted = isCompleted;
        }

        // Method to update progress
        public void UpdateProgress(int percentage)
        {
            ProgressPercentage = percentage;
            if (percentage >= 100)
            {
                MarkComplete();
            }
        }

        // Method to mark enrollment as complete
        public void MarkComplete()
        {
            IsCompleted = true;
            ProgressPercentage = 100;
        }

        // Method to display enrollment information
        public void DisplayInfo()
        {
            Console.WriteLine($"Enrollment ID: {EnrollmentId}");
            Console.WriteLine($"Student: {StudentUsername}");
            Console.WriteLine($"Course ID: {CourseId}");
            Console.WriteLine($"Enrolled On: {EnrollmentDate.ToShortDateString()}");
            Console.WriteLine($"Progress: {ProgressPercentage}%");
            Console.WriteLine($"Status: {(IsCompleted ? "Completed" : "In Progress")}");
        }
    }
}
