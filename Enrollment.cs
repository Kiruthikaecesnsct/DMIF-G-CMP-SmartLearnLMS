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


        public int StudentId { get; set; }
        public int CourseId { get; set; }

        public DateTime EnrolledDate { get; set; }

        public int ProgressPercent { get; set; }

        [NotMapped]
        public bool IsCompleted { get; set; }

        // StudentUsername doesn't exist in DB — mark it NotMapped  
        [NotMapped]
        public string? StudentUsername { get; set; }
        public string Status { get; set; } = "Active";

        // NEW — used in analytics queries from Week 7 script
        public DateTime? CompletionDate { get; set; }

        public Student Student { get; set; }
        public Course Course { get; set; }

        public Enrollment() { }

        public Enrollment(int enrollmentId, string studentUsername, int courseId,
                          DateTime enrollmentDate, int progressPercentage, bool isCompleted)
            {
            EnrollmentId = enrollmentId;
            StudentUsername = studentUsername;
            CourseId = courseId;
            EnrolledDate = enrollmentDate;
            ProgressPercent = progressPercentage;
            IsCompleted = isCompleted;
            }

        public void UpdateProgress(int percentage)
            {
            ProgressPercent = percentage;
            if (percentage >= 100)
                MarkComplete();
            }

        public void MarkComplete()
            {
            IsCompleted = true;
            ProgressPercent = 100;
            Status = "Completed";
            CompletionDate = DateTime.Now;   // NEW
            }

        // NEW — for EnrollmentService.DropCourse
        public void Drop()
            {
            Status = "Dropped";
            }

        public void DisplayInfo()
            {
            Console.WriteLine($"Enrollment ID: {EnrollmentId}");
            Console.WriteLine($"Student: {StudentUsername}");
            Console.WriteLine($"Course ID: {CourseId}");
            Console.WriteLine($"Enrolled On: {EnrolledDate.ToShortDateString()}");
            Console.WriteLine($"Progress: {ProgressPercent}%");
            Console.WriteLine($"Status: {(IsCompleted ? "Completed" : "In Progress")}");
            }

        public string GenerateReport()
            {
            return $"Enrollment #{EnrollmentId} | Student: {StudentUsername} | " +
                   $"Course: {CourseId} | Progress: {ProgressPercent}% | " +
                   $"Status: {(IsCompleted ? "Completed ✅" : "In Progress")}";
            }
        }
    }