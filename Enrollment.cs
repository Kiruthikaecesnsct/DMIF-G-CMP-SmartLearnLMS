using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Week_1.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Week_1
    {
    public class Enrollment : IReportable
        {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EnrollmentId { get; set; }

        // Made nullable with ? — so EF Core does not require it when saving
     
        public string? StudentUsername { get; set; }

        // Foreign keys — EF Core uses these to link Students and Courses tables
        public int StudentId { get; set; }
        public int CourseId { get; set; }

        public DateTime EnrollmentDate { get; set; }
        public int ProgressPercentage { get; set; }
        public bool IsCompleted { get; set; }
        public string Status { get; set; } = "Active";

        // ✅ [NotMapped] REMOVED from both — EF Core needs these for .Include().ThenInclude()
        // These are the real database relationships
        public Student Student { get; set; }
        public Course Course { get; set; }

        // Parameterless constructor — required by EF Core
        public Enrollment() { }

        // Your existing constructor — kept exactly as before
        public Enrollment(int enrollmentId, string studentUsername, int courseId,
                          DateTime enrollmentDate, int progressPercentage, bool isCompleted)
            {
            EnrollmentId = enrollmentId;
            StudentUsername = studentUsername;
            CourseId = courseId;
            EnrollmentDate = enrollmentDate;
            ProgressPercentage = progressPercentage;
            IsCompleted = isCompleted;
            }

        public void UpdateProgress(int percentage)
            {
            ProgressPercentage = percentage;
            if (percentage >= 100)
                {
                MarkComplete();
                }
            }

        public void MarkComplete()
            {
            IsCompleted = true;
            ProgressPercentage = 100;
            Status = "Completed";
            }

        public void DisplayInfo()
            {
            Console.WriteLine($"Enrollment ID: {EnrollmentId}");
            Console.WriteLine($"Student: {StudentUsername}");
            Console.WriteLine($"Course ID: {CourseId}");
            Console.WriteLine($"Enrolled On: {EnrollmentDate.ToShortDateString()}");
            Console.WriteLine($"Progress: {ProgressPercentage}%");
            Console.WriteLine($"Status: {(IsCompleted ? "Completed" : "In Progress")}");
            }

        public string GenerateReport()
            {
            return $"Enrollment #{EnrollmentId} | Student: {StudentUsername} | " +
                   $"Course: {CourseId} | Progress: {ProgressPercentage}% | " +
                   $"Status: {(IsCompleted ? "Completed ✅" : "In Progress")}";
            }
        }
    }