using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Week_1.Services
    {
    /// <summary>
    /// Assignment 6 — Part 6 Task 3: New EF Core EnrollmentService.
    /// Covers all enrollment CRUD with Include(), validation, and capacity checks.
    /// </summary>
    public class EfEnrollmentService
        {
        // ══════════════════════════════════════════════════════
        //  ENROLL STUDENT
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// Enroll a student in a course.
        /// Validates: student exists, course exists, not already enrolled, capacity check.
        /// </summary>
        public EnrollmentEntity EnrollStudent(int studentId, int courseId)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();

                // Part 8 Task 4: FK validation
                var student = ctx.Users.Find(studentId);
                if (student == null || student.UserType != "Student")
                    { Console.WriteLine("  ✗ Student not found."); return null; }

                var course = ctx.Courses.Find(courseId);
                if (course == null)
                    { Console.WriteLine("  ✗ Course not found."); return null; }

                // Part 8 Task 2: Duplicate prevention
                bool alreadyEnrolled = ctx.Enrollments.Any(e => e.StudentId == studentId && e.CourseId == courseId);
                if (alreadyEnrolled)
                    { Console.WriteLine($"  ✗ '{student.Username}' is already enrolled in '{course.Title}'."); return null; }

                // Capacity check
                if (course.CurrentEnrollments >= course.MaxCapacity)
                    { Console.WriteLine($"  ✗ Course '{course.Title}' is at full capacity ({course.MaxCapacity})."); return null; }

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
                if (ex.InnerException?.Message.Contains("UQ_") == true || ex.InnerException?.Message.Contains("UNIQUE") == true)
                    Console.WriteLine("  ✗ Already enrolled in this course (DB constraint).");
                else
                    Console.WriteLine($"  ✗ DB error: {ex.InnerException?.Message ?? ex.Message}");
                return null;
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return null; }
            }

        // ══════════════════════════════════════════════════════
        //  DROP COURSE
        // ══════════════════════════════════════════════════════

        public bool DropCourse(int studentId, int courseId)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                var enr = ctx.Enrollments
                    .FirstOrDefault(e => e.StudentId == studentId && e.CourseId == courseId);
                if (enr == null) { Console.WriteLine("  ✗ Enrollment not found."); return false; }

                var course = ctx.Courses.Find(courseId);
                ctx.Enrollments.Remove(enr);
                if (course != null && course.CurrentEnrollments > 0)
                    course.CurrentEnrollments--;
                ctx.SaveChanges();
                Console.WriteLine("  ✓ Course dropped successfully.");
                return true;
                }
            catch (DbUpdateException ex) { Console.WriteLine($"  ✗ DB error: {ex.InnerException?.Message ?? ex.Message}"); return false; }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return false; }
            }

        public bool DeleteEnrollment(int enrollmentId)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                var enr = ctx.Enrollments.Find(enrollmentId);
                if (enr == null) { Console.WriteLine("  ✗ Enrollment not found."); return false; }

                var course = ctx.Courses.Find(enr.CourseId);
                ctx.Enrollments.Remove(enr);
                if (course != null && course.CurrentEnrollments > 0)
                    course.CurrentEnrollments--;
                ctx.SaveChanges();
                Console.WriteLine("  ✓ Enrollment deleted.");
                return true;
                }
            catch (DbUpdateException ex) { Console.WriteLine($"  ✗ DB error: {ex.InnerException?.Message ?? ex.Message}"); return false; }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return false; }
            }

        // ══════════════════════════════════════════════════════
        //  READ ENROLLMENTS
        // ══════════════════════════════════════════════════════

        /// <summary>Get all enrollments for a student, including course details.</summary>
        public List<EnrollmentEntity> GetStudentEnrollments(int studentId)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                return ctx.Enrollments
                    .Include(e => e.Course)
                    .Where(e => e.StudentId == studentId)
                    .OrderBy(e => e.EnrolledDate)
                    .ToList();
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return new List<EnrollmentEntity>(); }
            }

        /// <summary>Get all enrollments for a course, including student details.</summary>
        public List<EnrollmentEntity> GetCourseEnrollments(int courseId)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                return ctx.Enrollments
                    .Include(e => e.Student)
                    .Where(e => e.CourseId == courseId)
                    .OrderBy(e => e.Student.Username)
                    .ToList();
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return new List<EnrollmentEntity>(); }
            }

        // ══════════════════════════════════════════════════════
        //  UPDATE PROGRESS
        // ══════════════════════════════════════════════════════

        public bool UpdateEnrollmentProgress(int studentId, int courseId, int newProgress)
            {
            if (newProgress < 0 || newProgress > 100)
                { Console.WriteLine("  ✗ Progress must be 0-100."); return false; }

            try
                {
                using var ctx = new SmartLearnDbContext();
                var enr = ctx.Enrollments
                    .FirstOrDefault(e => e.StudentId == studentId && e.CourseId == courseId);
                if (enr == null) { Console.WriteLine("  ✗ Enrollment not found."); return false; }

                enr.ProgressPercent = newProgress;
                if (newProgress == 100) { enr.Status = "Completed"; enr.CompletionDate = DateTime.Now; }
                ctx.SaveChanges();
                Console.WriteLine($"  ✓ Progress updated to {newProgress}%{(newProgress == 100 ? " — Course Completed! 🎉" : "")}");
                return true;
                }
            catch (DbUpdateException ex) { Console.WriteLine($"  ✗ DB error: {ex.InnerException?.Message ?? ex.Message}"); return false; }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return false; }
            }

        public bool ChangeEnrollmentStatus(int enrollmentId, string newStatus)
            {
            var validStatuses = new[] { "Active", "Completed", "Dropped" };
            if (!validStatuses.Contains(newStatus))
                { Console.WriteLine("  ✗ Status must be: Active, Completed, or Dropped."); return false; }

            try
                {
                using var ctx = new SmartLearnDbContext();
                var enr = ctx.Enrollments.Find(enrollmentId);
                if (enr == null) { Console.WriteLine("  ✗ Enrollment not found."); return false; }

                enr.Status = newStatus;
                if (newStatus == "Completed") enr.CompletionDate = DateTime.Now;
                ctx.SaveChanges();
                Console.WriteLine($"  ✓ Status changed to '{newStatus}'.");
                return true;
                }
            catch (DbUpdateException ex) { Console.WriteLine($"  ✗ DB error: {ex.InnerException?.Message ?? ex.Message}"); return false; }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return false; }
            }

        // ══════════════════════════════════════════════════════
        //  PART 7 TASK 3: MULTI-CRITERIA
        // ══════════════════════════════════════════════════════

        /// <summary>Active enrollments with progress less than 100%.</summary>
        public List<EnrollmentEntity> GetIncompleteActiveEnrollments()
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                return ctx.Enrollments
                    .Include(e => e.Student)
                    .Include(e => e.Course)
                    .Where(e => e.Status == "Active" && e.ProgressPercent < 100)
                    .OrderBy(e => e.ProgressPercent)
                    .ToList();
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return new List<EnrollmentEntity>(); }
            }

        // ── DISPLAY HELPERS ──
        public void DisplayEnrollmentList(List<EnrollmentEntity> enrollments)
            {
            if (!enrollments.Any()) { Console.WriteLine("  No enrollments found."); return; }
            Console.WriteLine($"\n  {"ID",-6}{"Student",-22}{"Course",-36}{"Progress",-10}{"Status",-12}Enrolled");
            Console.WriteLine("  " + new string('─', 96));
            foreach (var e in enrollments)
                {
                string student = e.Student?.Username ?? $"ID:{e.StudentId}";
                string course = e.Course?.Title ?? $"ID:{e.CourseId}";
                if (course.Length > 34) course = course.Substring(0, 31) + "...";
                Console.WriteLine($"  {e.EnrollmentId,-6}{student,-22}{course,-36}{e.ProgressPercent + "%",-10}{e.Status,-12}{e.EnrolledDate:d}");
                }
            }
        }
    }