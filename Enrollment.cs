// Enrollment.cs — complete answer (show AFTER students try)
using System;

namespace SmartLearnLMS
{
    public class Enrollment
    {
        public int EnrollmentId { get; set; }
        public string StudentUsername { get; set; }
        public int CourseId { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public int ProgressPercentage { get; set; }
        public bool IsCompleted { get; set; }

        public Enrollment(int id, string studentUsername, int courseId,
                          DateTime enrollmentDate, int progress, bool isCompleted)
        {
            EnrollmentId = id;
            StudentUsername = studentUsername;
            CourseId = courseId;
            EnrollmentDate = enrollmentDate;
            ProgressPercentage = progress;
            IsCompleted = isCompleted;
        }

        public void UpdateProgress(int percentage)
        {
            ProgressPercentage = percentage;
            if (percentage >= 100)
            {
                IsCompleted = true;
                Console.WriteLine("  Course completed!");
            }
            Console.WriteLine($"  Progress updated to {ProgressPercentage}%");
        }

        public void MarkComplete()
        {
            IsCompleted = true;
            ProgressPercentage = 100;
            Console.WriteLine("  Enrollment marked as complete!");
        }

        public void DisplayInfo()
        {
            Console.WriteLine($"  Enrollment ID : {EnrollmentId}");
            Console.WriteLine($"  Student       : {StudentUsername}");
            Console.WriteLine($"  Course ID     : {CourseId}");
            Console.WriteLine($"  Enrolled On   : {EnrollmentDate:dd MMM yyyy}");
            Console.WriteLine($"  Progress      : {ProgressPercentage}%");
            Console.WriteLine($"  Completed     : {(IsCompleted ? "Yes" : "No")}");
        }
    }
}