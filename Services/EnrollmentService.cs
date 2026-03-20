using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Week_1.Services
    {
    /// <summary>
    /// Assignment 7 — Part 3 Task 4: EnrollmentService
    /// Enroll, drop, complete, and trend analysis for enrollments.
    /// </summary>
    public class EnrollmentService
        {
        // ══════════════════════════════════════════════════════
        //  EnrollStudent
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// Enroll a student in a course.
        ///  – Checks for duplicate enrollment.
        ///  – Checks available capacity.
        ///  – Creates enrollment with Status = 'Active' and Progress = 0.
        ///  – Updates course enrollment count.
        /// </summary>
        public EnrollmentEntity EnrollStudent(int studentId, int courseId)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();

                var student = ctx.Users.Find(studentId);
                if (student == null || student.UserType != "Student")
                    { Console.WriteLine("  ✗ Student not found."); return null; }

                var course = ctx.Courses.Find(courseId);
                if (course == null)
                    { Console.WriteLine("  ✗ Course not found."); return null; }

                // Duplicate enrollment check
                bool alreadyEnrolled = ctx.Enrollments
                    .Any(e => e.StudentId == studentId && e.CourseId == courseId);
                if (alreadyEnrolled)
                    { Console.WriteLine($"  ✗ '{student.Username}' is already enrolled in '{course.Title}'."); return null; }

                // Capacity check
                if (course.CurrentEnrollments >= course.MaxCapacity)
                    { Console.WriteLine($"  ✗ '{course.Title}' is at full capacity ({course.MaxCapacity})."); return null; }

                var enrollment = new EnrollmentEntity
                    {
                    StudentId = studentId,
                    CourseId = courseId,
                    EnrolledDate = DateTime.Now,
                    ProgressPercent = 0,
                    Status = "Active"
                    };

                ctx.Enrollments.Add(enrollment);
                course.CurrentEnrollments++;
                ctx.SaveChanges();

                Console.WriteLine($"  ✓ '{student.Username}' enrolled in '{course.Title}'! Enrollment ID: {enrollment.EnrollmentId}");
                return enrollment;
                }
            catch (DbUpdateException ex)
                {
                if (ex.InnerException?.Message.Contains("UNIQUE") == true)
                    Console.WriteLine("  ✗ Already enrolled (DB constraint).");
                else
                    Console.WriteLine($"  ✗ DB error: {ex.InnerException?.Message ?? ex.Message}");
                return null;
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return null; }
            }

        // ══════════════════════════════════════════════════════
        //  DropCourse
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// Drop a course: sets Status = 'Dropped' and updates enrollment count.
        /// </summary>
        public bool DropCourse(int studentId, int courseId)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();

                var enrollment = ctx.Enrollments
                    .FirstOrDefault(e => e.StudentId == studentId && e.CourseId == courseId);
                if (enrollment == null) { Console.WriteLine("  ✗ Enrollment not found."); return false; }

                var course = ctx.Courses.Find(courseId);
                enrollment.Status = "Dropped";

                if (course != null && course.CurrentEnrollments > 0)
                    course.CurrentEnrollments--;

                ctx.SaveChanges();
                Console.WriteLine("  ✓ Course dropped successfully.");
                return true;
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return false; }
            }

        // ══════════════════════════════════════════════════════
        //  MarkAsCompleted
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// Mark an enrollment as completed:
        ///  – Status = 'Completed', ProgressPercent = 100, CompletionDate = now.
        /// </summary>
        public bool MarkAsCompleted(int enrollmentId)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();

                var enrollment = ctx.Enrollments
                    .Include(e => e.Student)
                    .Include(e => e.Course)
                    .FirstOrDefault(e => e.EnrollmentId == enrollmentId);

                if (enrollment == null) { Console.WriteLine("  ✗ Enrollment not found."); return false; }

                enrollment.Status = "Completed";
                enrollment.ProgressPercent = 100;
                enrollment.CompletionDate = DateTime.Now;

                ctx.SaveChanges();

                Console.WriteLine($"  ✓ '{enrollment.Student?.Username}' completed '{enrollment.Course?.Title}'! 🎉");
                return true;
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return false; }
            }

        // ══════════════════════════════════════════════════════
        //  GetEnrollmentTrends
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// Groups enrollments by year and month.
        /// Returns monthly enrollment counts for trend analysis.
        /// Key = "YYYY-MM", Value = count.
        /// </summary>
        public Dictionary<string, int> GetEnrollmentTrends()
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                return ctx.Enrollments
                    .AsEnumerable()   // ← add this
                    .GroupBy(e => new
                        {
                        Year = e.EnrolledDate.Year,
                        Month = e.EnrolledDate.Month
                        })
                    .OrderBy(g => g.Key.Year)
                    .ThenBy(g => g.Key.Month)
                    .ToDictionary(
                        g => $"{g.Key.Year}-{g.Key.Month:D2}",
                        g => g.Count()
                    );
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return new Dictionary<string, int>(); }
            }

        // ── DISPLAY HELPERS ──────────────────────────────────

        public void DisplayEnrollmentTrends(Dictionary<string, int> trends)
            {
            if (!trends.Any()) { Console.WriteLine("  No enrollment data."); return; }
            Console.WriteLine("\n  Monthly Enrollment Trends:");
            Console.WriteLine($"  {"Month",-12}{"New Enrollments"}");
            Console.WriteLine("  " + new string('─', 28));
            foreach (var kv in trends)
                {
                string bar = new string('█', Math.Min(kv.Value, 40));
                Console.WriteLine($"  {kv.Key,-12}{kv.Value,5}  {bar}");
                }
            }
        }
    }