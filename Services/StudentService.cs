using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Week_1.Services
    {
    /// <summary>
    /// Assignment 7 — Part 3 Task 1: StudentService
    /// Uses navigation properties and Include/ThenInclude for efficient data loading.
    /// </summary>
    public class StudentService
        {
        // ══════════════════════════════════════════════════════
        //  GetStudentDashboard
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// Load a student's full dashboard:
        /// – All enrolled courses with progress, status, and instructor name.
        /// – Summary statistics: total courses, completed, average progress.
        /// Uses Include() + ThenInclude() to avoid N+1 queries.
        /// </summary>
        public void GetStudentDashboard(int studentId)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();

                // Eager-load Enrollments → Course → Instructor
                var student = ctx.Users
                    .Include(u => u.Enrollments)
                        .ThenInclude(e => e.Course)
                            .ThenInclude(c => c.Instructor)
                    .FirstOrDefault(u => u.UserId == studentId && u.UserType == "Student");

                if (student == null) { Console.WriteLine("  ✗ Student not found."); return; }

                Console.WriteLine($"\n╔══════════════════════════════════════════════╗");
                Console.WriteLine($"║  STUDENT DASHBOARD — {student.Username,-24}║");
                Console.WriteLine($"╚══════════════════════════════════════════════╝");

                if (!student.Enrollments.Any())
                    { Console.WriteLine("  No enrollments yet."); return; }

                Console.WriteLine($"\n  {"Course",-34}{"Instructor",-22}{"Progress",-10}Status");
                Console.WriteLine("  " + new string('─', 82));

                foreach (var e in student.Enrollments.OrderBy(e => e.EnrolledDate))
                    {
                    string courseTitle = e.Course?.Title ?? $"ID:{e.CourseId}";
                    if (courseTitle.Length > 32) courseTitle = courseTitle[..29] + "...";
                    string instructor = e.Course?.Instructor?.Username ?? e.Course?.InstructorName ?? "N/A";
                    Console.WriteLine($"  {courseTitle,-34}{instructor,-22}{e.ProgressPercent + "%",-10}{e.Status}");
                    }

                // Summary statistics
                int total = student.Enrollments.Count;
                int completed = student.Enrollments.Count(e => e.Status == "Completed");
                int active = student.Enrollments.Count(e => e.Status == "Active");
                double avgProg = student.Enrollments.Average(e => e.ProgressPercent);

                Console.WriteLine($"\n  ── Summary ──────────────────────────────");
                Console.WriteLine($"  Total Courses   : {total}");
                Console.WriteLine($"  Completed       : {completed}");
                Console.WriteLine($"  Active          : {active}");
                Console.WriteLine($"  Average Progress: {avgProg:F1}%");
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); }
            }

        // ══════════════════════════════════════════════════════
        //  GetStudentCompletedCourses
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// Returns only completed courses for a student,
        /// including course details and completion dates.
        /// </summary>
        public List<EnrollmentEntity> GetStudentCompletedCourses(int studentId)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                return ctx.Enrollments
                    .Include(e => e.Course)
                        .ThenInclude(c => c.Instructor)
                    .Where(e => e.StudentId == studentId && e.Status == "Completed")
                    .OrderByDescending(e => e.CompletionDate)
                    .ToList();
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return new List<EnrollmentEntity>(); }
            }

        // ══════════════════════════════════════════════════════
        //  UpdateCourseProgress
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// Update progress for a student's enrollment in a specific course.
        /// If progress reaches 100%, automatically sets Status = 'Completed'
        /// and records the CompletionDate.
        /// </summary>
        public bool UpdateCourseProgress(int studentId, int courseId, double newProgress)
            {
            if (newProgress < 0 || newProgress > 100)
                { Console.WriteLine("  ✗ Progress must be 0-100."); return false; }

            try
                {
                using var ctx = new SmartLearnDbContext();

                var enrollment = ctx.Enrollments
                    .Include(e => e.Course)
                    .FirstOrDefault(e => e.StudentId == studentId && e.CourseId == courseId);

                if (enrollment == null)
                    { Console.WriteLine("  ✗ Enrollment not found."); return false; }

                enrollment.ProgressPercent = (int)newProgress;

                if (newProgress >= 100)
                    {
                    enrollment.ProgressPercent = 100;
                    enrollment.Status = "Completed";
                    enrollment.CompletionDate = DateTime.Now;
                    Console.WriteLine($"  🎉 '{enrollment.Course?.Title}' marked as Completed!");
                    }

                ctx.SaveChanges();
                Console.WriteLine($"  ✓ Progress updated to {enrollment.ProgressPercent}%.");
                return true;
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return false; }
            }

        // ══════════════════════════════════════════════════════
        //  GetStudentAtRiskCourses
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// Returns active enrollments where:
        ///   – Progress is below 30%
        ///   – The student enrolled more than 2 weeks ago (falling behind).
        /// </summary>
        public List<EnrollmentEntity> GetStudentAtRiskCourses(int studentId)
            {
            var twoWeeksAgo = DateTime.Now.AddDays(-14);
            try
                {
                using var ctx = new SmartLearnDbContext();
                return ctx.Enrollments
                    .Include(e => e.Course)
                    .Where(e => e.StudentId == studentId
                             && e.Status == "Active"
                             && e.ProgressPercent < 30
                             && e.EnrolledDate <= twoWeeksAgo)
                    .OrderBy(e => e.ProgressPercent)
                    .ToList();
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return new List<EnrollmentEntity>(); }
            }

        // ── DISPLAY HELPERS ──────────────────────────────────

        public void DisplayCompletedCourses(List<EnrollmentEntity> enrollments)
            {
            if (!enrollments.Any()) { Console.WriteLine("  No completed courses."); return; }
            Console.WriteLine($"\n  {"Course",-36}{"Completed",-14}Progress");
            Console.WriteLine("  " + new string('─', 60));
            foreach (var e in enrollments)
                Console.WriteLine($"  {e.Course?.Title ?? $"ID:{e.CourseId}",-36}{e.CompletionDate?.ToString("d") ?? "N/A",-14}{e.ProgressPercent}%");
            }

        public void DisplayAtRiskCourses(List<EnrollmentEntity> enrollments)
            {
            if (!enrollments.Any()) { Console.WriteLine("  ✓ No at-risk courses — great job!"); return; }
            Console.WriteLine($"\n  ⚠  At-Risk Courses ({enrollments.Count}):");
            Console.WriteLine($"  {"Course",-36}{"Progress",-10}{"Enrolled",-14}Days Since Enroll");
            Console.WriteLine("  " + new string('─', 75));
            foreach (var e in enrollments)
                {
                int days = (int)(DateTime.Now - e.EnrolledDate).TotalDays;
                Console.WriteLine($"  {e.Course?.Title ?? $"ID:{e.CourseId}",-36}{e.ProgressPercent + "%",-10}{e.EnrolledDate:d,-14}{days} days");
                }
            }
        }
    }