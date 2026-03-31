using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Week_1.Exceptions;
using Week_1.Logging;
using Week_1.Validators;

namespace Week_1.Services
    {
    /// <summary>
    /// Assignment 8 — StudentService with full exception handling,
    /// input/business-rule validation, logging, and query optimisation.
    /// </summary>
    public class StudentService
        {
        private static readonly Logger _log = Logger.Instance;
        private static readonly AuditService _audit = new();
        private readonly BusinessRuleValidator _rules = new();

        // ══════════════════════════════════════════════════════════════
        //  GetStudentDashboard
        // ══════════════════════════════════════════════════════════════

        /// <summary>
        /// Loads a student's full dashboard with all enrolled courses,
        /// instructor names, and summary statistics.
        /// Uses AsNoTracking for read-only performance.
        /// </summary>
        public void GetStudentDashboard(int studentId)
            {
            _log.Info("StudentService", "GetStudentDashboard called.", $"ID:{studentId}");
            try
                {
                using var ctx = new SmartLearnDbContext();

                var student = ctx.Users
                    .AsNoTracking()
                    .Include(u => u.Enrollments)
                        .ThenInclude(e => e.Course)
                            .ThenInclude(c => c.Instructor)
                    .FirstOrDefault(u => u.UserId == studentId && u.UserType == "Student");

                if (student == null)
                    {
                    _log.Warning("StudentService", $"Dashboard requested for unknown student {studentId}.");
                    Console.WriteLine("  ✗ Student not found.");
                    return;
                    }

                Console.WriteLine($"\n╔══════════════════════════════════════════════╗");
                Console.WriteLine($"║  STUDENT DASHBOARD — {student.Username,-24}║");
                Console.WriteLine($"╚══════════════════════════════════════════════╝");

                if (!student.Enrollments.Any())
                    { Console.WriteLine("  No enrollments yet."); return; }

                Console.WriteLine($"\n  {"Course",-34}{"Instructor",-22}{"Progress",-10}Status");
                Console.WriteLine("  " + new string('─', 82));

                foreach (var e in student.Enrollments.OrderBy(e => e.EnrolledDate))
                    {
                    string title = TruncateTitle(e.Course?.Title ?? $"ID:{e.CourseId}");
                    string instructor = e.Course?.Instructor?.Username
                                        ?? e.Course?.InstructorName ?? "N/A";
                    Console.WriteLine($"  {title,-34}{instructor,-22}{e.ProgressPercent + "%",-10}{e.Status}");
                    }

                int total = student.Enrollments.Count;
                int completed = student.Enrollments.Count(e => e.Status == "Completed");
                int active = student.Enrollments.Count(e => e.Status == "Active");
                double avgProg = student.Enrollments.Average(e => e.ProgressPercent);

                Console.WriteLine($"\n  ── Summary ──────────────────────────────");
                Console.WriteLine($"  Total Courses   : {total}");
                Console.WriteLine($"  Completed       : {completed}");
                Console.WriteLine($"  Active          : {active}");
                Console.WriteLine($"  Average Progress: {avgProg:F1}%");

                _log.Info("StudentService", $"Dashboard displayed for '{student.Username}'.", student.Username);
                }
            catch (Exception ex)
                {
                _log.Error("StudentService", "GetStudentDashboard failed.", $"ID:{studentId}", ex);
                Console.WriteLine($"  ✗ Error loading dashboard: {ex.Message}");
                }
            }

        // ══════════════════════════════════════════════════════════════
        //  GetStudentCompletedCourses
        // ══════════════════════════════════════════════════════════════

        /// <summary>Returns completed courses for a student, ordered by completion date.</summary>
        public List<EnrollmentEntity> GetStudentCompletedCourses(int studentId)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                return ctx.Enrollments
                    .AsNoTracking()
                    .Include(e => e.Course).ThenInclude(c => c.Instructor)
                    .Where(e => e.StudentId == studentId && e.Status == "Completed")
                    .OrderByDescending(e => e.CompletionDate)
                    .ToList();
                }
            catch (Exception ex)
                {
                _log.Error("StudentService", "GetStudentCompletedCourses failed.", $"ID:{studentId}", ex);
                return new List<EnrollmentEntity>();
                }
            }

        // ══════════════════════════════════════════════════════════════
        //  UpdateCourseProgress
        // ══════════════════════════════════════════════════════════════

        /// <summary>
        /// Updates progress for an enrollment.
        /// Validates input and business rules, throws
        /// <see cref="InvalidProgressUpdateException"/> for illegal updates.
        /// Auto-completes at 100%.
        /// </summary>
        public bool UpdateCourseProgress(int studentId, int courseId, double newProgress)
            {
            _log.Info("StudentService",
                      $"UpdateCourseProgress: student={studentId} course={courseId} new={newProgress}%");

            // Input validation
            if (!InputValidator.ValidateProgress(newProgress, out string inputError))
                {
                _log.Warning("StudentService", $"Invalid progress input: {inputError}", $"ID:{studentId}");
                Console.WriteLine($"  ✗ {inputError}");
                return false;
                }

            // Business rule validation (ensures no decrease)
            if (!_rules.CanUpdateProgress(studentId, courseId, newProgress, out string ruleError))
                {
                _log.Warning("StudentService", $"Progress rule violation: {ruleError}", $"ID:{studentId}");
                Console.WriteLine($"  ✗ {ruleError}");
                return false;
                }

            try
                {
                using var ctx = new SmartLearnDbContext();

                var enrollment = ctx.Enrollments
                    .Include(e => e.Course)
                    .FirstOrDefault(e => e.StudentId == studentId && e.CourseId == courseId);

                if (enrollment == null)
                    throw new InvalidProgressUpdateException(studentId, courseId, 0, newProgress);

                double oldProgress = enrollment.ProgressPercent;
                enrollment.ProgressPercent = (int)newProgress;

                if (newProgress >= 100)
                    {
                    enrollment.ProgressPercent = 100;
                    enrollment.Status = "Completed";
                    enrollment.CompletionDate = DateTime.Now;
                    Console.WriteLine($"  🎉 '{enrollment.Course?.Title}' marked as Completed!");

                    _audit.LogAction("CourseCompleted", "Enrollment",
                        userId: studentId,
                        details: new() { ["courseId"] = courseId, ["courseTitle"] = enrollment.Course?.Title });
                    }

                ctx.SaveChanges();
                Console.WriteLine($"  ✓ Progress updated: {oldProgress}% → {enrollment.ProgressPercent}%.");
                _log.Info("StudentService",
                          $"Progress updated {oldProgress}% → {enrollment.ProgressPercent}% for student {studentId} course {courseId}.");
                return true;
                }
            catch (InvalidProgressUpdateException ex)
                {
                _log.Error("StudentService", ex.Message, $"ID:{studentId}", ex);
                Console.WriteLine($"  ✗ {ex.Message}");
                return false;
                }
            catch (Exception ex)
                {
                _log.Error("StudentService", $"UpdateCourseProgress failed: {ex.Message}", $"ID:{studentId}", ex);
                Console.WriteLine($"  ✗ Error updating progress: {ex.Message}");
                return false;
                }
            }

        // ══════════════════════════════════════════════════════════════
        //  GetStudentAtRiskCourses
        // ══════════════════════════════════════════════════════════════

        /// <summary>Returns active enrollments where progress &lt; 30% and enrolled &gt; 2 weeks ago.</summary>
        public List<EnrollmentEntity> GetStudentAtRiskCourses(int studentId)
            {
            var twoWeeksAgo = DateTime.Now.AddDays(-14);
            try
                {
                using var ctx = new SmartLearnDbContext();
                return ctx.Enrollments
                    .AsNoTracking()
                    .Include(e => e.Course)
                    .Where(e => e.StudentId == studentId
                             && e.Status == "Active"
                             && e.ProgressPercent < 30
                             && e.EnrolledDate <= twoWeeksAgo)
                    .OrderBy(e => e.ProgressPercent)
                    .ToList();
                }
            catch (Exception ex)
                {
                _log.Error("StudentService", "GetStudentAtRiskCourses failed.", $"ID:{studentId}", ex);
                return new List<EnrollmentEntity>();
                }
            }

        // ── Display helpers ────────────────────────────────────────────

        /// <summary>Displays a formatted list of completed enrollments.</summary>
        public void DisplayCompletedCourses(List<EnrollmentEntity> enrollments)
            {
            if (!enrollments.Any()) { Console.WriteLine("  No completed courses."); return; }
            Console.WriteLine($"\n  {"Course",-36}{"Completed",-14}Progress");
            Console.WriteLine("  " + new string('─', 60));
            foreach (var e in enrollments)
                Console.WriteLine($"  {e.Course?.Title ?? $"ID:{e.CourseId}",-36}" +
                                  $"{e.CompletionDate?.ToString("d") ?? "N/A",-14}{e.ProgressPercent}%");
            }

        /// <summary>Displays a formatted list of at-risk enrollments.</summary>
        public void DisplayAtRiskCourses(List<EnrollmentEntity> enrollments)
            {
            if (!enrollments.Any()) { Console.WriteLine("  ✓ No at-risk courses — great job!"); return; }
            Console.WriteLine($"\n  ⚠  At-Risk Courses ({enrollments.Count}):");
            Console.WriteLine($"  {"Course",-36}{"Progress",-10}{"Enrolled",-14}Days Since Enroll");
            Console.WriteLine("  " + new string('─', 75));
            foreach (var e in enrollments)
                {
                int days = (int)(DateTime.Now - e.EnrolledDate).TotalDays;
                Console.WriteLine($"  {e.Course?.Title ?? $"ID:{e.CourseId}",-36}" +
                                  $"{e.ProgressPercent + "%",-10}{e.EnrolledDate:d,-14}{days} days");
                }
            }

        private static string TruncateTitle(string title, int max = 32)
            => title.Length > max ? title[..(max - 3)] + "..." : title;
        }
    }