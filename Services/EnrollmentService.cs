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
    /// Assignment 8 — EnrollmentService with full exception handling,
    /// custom exceptions, validation, audit logging, and AsNoTracking optimisation.
    /// </summary>
    public class EnrollmentService
        {
        private static readonly Logger _log = Logger.Instance;
        private static readonly AuditService _audit = new();
        private readonly BusinessRuleValidator _rules = new();

        // ══════════════════════════════════════════════════════════════
        //  EnrollStudent
        // ══════════════════════════════════════════════════════════════

        /// <summary>
        /// Enrols a student in a course after validating business rules.
        /// Throws <see cref="AlreadyEnrolledException"/> or
        /// <see cref="CourseFullException"/> on violations.
        /// </summary>
        public EnrollmentEntity EnrollStudent(int studentId, int courseId)
            {
            _log.Info("EnrollmentService",
                      $"EnrollStudent called: student={studentId} course={courseId}");

            // Business rule pre-check (also validates student/course exist)
            if (!_rules.CanEnroll(studentId, courseId, out string reason))
                {
                _log.Warning("EnrollmentService", $"Enroll denied: {reason}");
                Console.WriteLine($"  ✗ {reason}");
                return null;
                }

            try
                {
                using var ctx = new SmartLearnDbContext();

                // Re-fetch inside transaction scope for safety
                var student = ctx.Users.Find(studentId);
                var course = ctx.Courses.Find(courseId);

                // Confirm still valid (race-condition guard)
                if (ctx.Enrollments.Any(e => e.StudentId == studentId && e.CourseId == courseId))
                    throw new AlreadyEnrolledException(studentId, courseId);

                if (course.CurrentEnrollments >= course.MaxCapacity)
                    throw new CourseFullException(studentId, courseId,
                                                  course.MaxCapacity, course.CurrentEnrollments);

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

                Console.WriteLine($"  ✓ '{student.Username}' enrolled in '{course.Title}'! " +
                                  $"Enrollment ID: {enrollment.EnrollmentId}");

                _log.Info("EnrollmentService",
                          $"'{student.Username}' enrolled in '{course.Title}' (ID {enrollment.EnrollmentId}).",
                          student.Username);

                _audit.LogAction("StudentEnrolled", "Enrollment",
                    username: student.Username, userId: studentId,
                    details: new() { ["courseId"] = courseId, ["courseTitle"] = course.Title });

                return enrollment;
                }
            catch (AlreadyEnrolledException ex)
                {
                _log.Warning("EnrollmentService", ex.Message);
                Console.WriteLine($"  ✗ {ex.Message}");
                return null;
                }
            catch (CourseFullException ex)
                {
                _log.Warning("EnrollmentService", ex.Message);
                Console.WriteLine($"  ✗ {ex.Message}");
                return null;
                }
            catch (DbUpdateException ex)
                {
                bool isDuplicate = ex.InnerException?.Message?.Contains("UNIQUE") == true;
                string msg = isDuplicate
                    ? "Already enrolled (database constraint)."
                    : $"Database error: {ex.InnerException?.Message ?? ex.Message}";
                _log.Error("EnrollmentService", msg, null, ex);
                Console.WriteLine($"  ✗ {msg}");
                return null;
                }
            catch (Exception ex)
                {
                _log.Error("EnrollmentService", $"EnrollStudent failed: {ex.Message}", null, ex);
                Console.WriteLine($"  ✗ Unexpected error: {ex.Message}");
                return null;
                }
            }

        // ══════════════════════════════════════════════════════════════
        //  DropCourse
        // ══════════════════════════════════════════════════════════════

        /// <summary>
        /// Drops a student from a course, validating the drop deadline rule.
        /// </summary>
        public bool DropCourse(int studentId, int courseId)
            {
            _log.Info("EnrollmentService",
                      $"DropCourse called: student={studentId} course={courseId}");

            if (!_rules.CanDropCourse(studentId, courseId, out string reason))
                {
                _log.Warning("EnrollmentService", $"Drop denied: {reason}");
                Console.WriteLine($"  ✗ {reason}");
                return false;
                }

            try
                {
                using var ctx = new SmartLearnDbContext();

                var enrollment = ctx.Enrollments
                    .Include(e => e.Course)
                    .FirstOrDefault(e => e.StudentId == studentId && e.CourseId == courseId);

                if (enrollment == null)
                    { Console.WriteLine("  ✗ Enrollment not found."); return false; }

                var student = ctx.Users.Find(studentId);
                enrollment.Status = "Dropped";
                if (enrollment.Course != null && enrollment.Course.CurrentEnrollments > 0)
                    enrollment.Course.CurrentEnrollments--;

                ctx.SaveChanges();
                Console.WriteLine("  ✓ Course dropped successfully.");

                _log.Info("EnrollmentService",
                          $"'{student?.Username}' dropped course '{enrollment.Course?.Title}'.",
                          student?.Username);
                _audit.LogAction("CourseDrop", "Enrollment",
                    username: student?.Username, userId: studentId,
                    details: new() { ["courseId"] = courseId });
                return true;
                }
            catch (Exception ex)
                {
                _log.Error("EnrollmentService", $"DropCourse failed: {ex.Message}", null, ex);
                Console.WriteLine($"  ✗ Error dropping course: {ex.Message}");
                return false;
                }
            }

        // ══════════════════════════════════════════════════════════════
        //  MarkAsCompleted
        // ══════════════════════════════════════════════════════════════

        /// <summary>
        /// Marks an enrollment as Completed. Validates progress is 100%.
        /// </summary>
        public bool MarkAsCompleted(int enrollmentId)
            {
            _log.Info("EnrollmentService", $"MarkAsCompleted: enrollmentId={enrollmentId}");

            if (!_rules.CanMarkAsCompleted(enrollmentId, out string reason))
                {
                _log.Warning("EnrollmentService", $"MarkAsCompleted denied: {reason}");
                Console.WriteLine($"  ✗ {reason}");
                return false;
                }

            try
                {
                using var ctx = new SmartLearnDbContext();

                var enrollment = ctx.Enrollments
                    .Include(e => e.Student)
                    .Include(e => e.Course)
                    .FirstOrDefault(e => e.EnrollmentId == enrollmentId);

                if (enrollment == null)
                    { Console.WriteLine("  ✗ Enrollment not found."); return false; }

                enrollment.Status = "Completed";
                enrollment.ProgressPercent = 100;
                enrollment.CompletionDate = DateTime.Now;
                ctx.SaveChanges();

                Console.WriteLine($"  ✓ '{enrollment.Student?.Username}' completed " +
                                  $"'{enrollment.Course?.Title}'! 🎉");

                _log.Info("EnrollmentService",
                          $"Enrollment {enrollmentId} marked completed.",
                          enrollment.Student?.Username);
                _audit.LogAction("CourseCompleted", "Enrollment",
                    username: enrollment.Student?.Username,
                    userId: enrollment.StudentId,
                    details: new()
                        {
                        ["courseId"] = enrollment.CourseId,
                        ["courseTitle"] = enrollment.Course?.Title
                        });
                return true;
                }
            catch (Exception ex)
                {
                _log.Error("EnrollmentService", $"MarkAsCompleted failed: {ex.Message}", null, ex);
                Console.WriteLine($"  ✗ Error: {ex.Message}");
                return false;
                }
            }

        // ══════════════════════════════════════════════════════════════
        //  GetEnrollmentTrends
        // ══════════════════════════════════════════════════════════════

        /// <summary>Groups enrollments by year-month for trend analysis.</summary>
        public Dictionary<string, int> GetEnrollmentTrends()
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                return ctx.Enrollments
                    .AsNoTracking()
                    .AsEnumerable()
                    .GroupBy(e => new { e.EnrolledDate.Year, e.EnrolledDate.Month })
                    .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                    .ToDictionary(
                        g => $"{g.Key.Year}-{g.Key.Month:D2}",
                        g => g.Count()
                    );
                }
            catch (Exception ex)
                {
                _log.Error("EnrollmentService", $"GetEnrollmentTrends failed: {ex.Message}", null, ex);
                return new Dictionary<string, int>();
                }
            }

        // ── Display helper ─────────────────────────────────────────────

        /// <summary>Prints a monthly enrollment trend bar chart to the console.</summary>
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