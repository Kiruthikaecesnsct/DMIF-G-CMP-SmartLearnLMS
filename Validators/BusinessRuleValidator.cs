using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Week_1.Validators
    {
    // ══════════════════════════════════════════════════════════════════
    //  ASSIGNMENT 8 — PART 2 TASK 2: BusinessRuleValidator
    // ══════════════════════════════════════════════════════════════════

    /// <summary>
    /// Validates business-level rules that depend on the current database
    /// state. Returns <c>true</c> when the operation is allowed, and provides
    /// a detailed failure reason via <paramref name="reason"/> when not.
    /// </summary>
    public class BusinessRuleValidator
        {
        private const int MaxActiveEnrollments = 5;
        private const int MaxDropDays = 90;
        private const int MaxInstructorCourses = 10;

        // ── CanEnroll ──────────────────────────────────────────────────

        /// <summary>
        /// Verifies that a student can enroll in a course:
        /// student exists, course exists, not already enrolled,
        /// course has capacity, and student is under the active-enrollment limit.
        /// </summary>
        public bool CanEnroll(int studentId, int courseId, out string reason)
            {
            reason = string.Empty;
            try
                {
                using var ctx = new SmartLearnDbContext();

                var student = ctx.Users.Find(studentId);
                if (student == null || student.UserType != "Student")
                    { reason = $"Student with ID {studentId} not found."; return false; }

                var course = ctx.Courses.Find(courseId);
                if (course == null)
                    { reason = $"Course with ID {courseId} not found."; return false; }

                bool alreadyEnrolled = ctx.Enrollments
                    .Any(e => e.StudentId == studentId && e.CourseId == courseId);
                if (alreadyEnrolled)
                    { reason = $"'{student.Username}' is already enrolled in '{course.Title}'."; return false; }

                if (course.CurrentEnrollments >= course.MaxCapacity)
                    { reason = $"'{course.Title}' is at full capacity ({course.MaxCapacity}/{course.MaxCapacity})."; return false; }

                int activeCount = ctx.Enrollments
                    .Count(e => e.StudentId == studentId && e.Status == "Active");
                if (activeCount >= MaxActiveEnrollments)
                    { reason = $"Student already has {activeCount} active enrollments (max {MaxActiveEnrollments})."; return false; }

                return true;
                }
            catch (Exception ex)
                { reason = $"Validation error: {ex.Message}"; return false; }
            }

        // ── CanDropCourse ─────────────────────────────────────────────

        /// <summary>
        /// Verifies an enrollment exists, is active, and is within the
        /// allowed drop window (<see cref="MaxDropDays"/> days).
        /// </summary>
        public bool CanDropCourse(int studentId, int courseId, out string reason)
            {
            reason = string.Empty;
            try
                {
                using var ctx = new SmartLearnDbContext();

                var enrollment = ctx.Enrollments
                    .FirstOrDefault(e => e.StudentId == studentId && e.CourseId == courseId);

                if (enrollment == null)
                    { reason = "Enrollment not found."; return false; }

                if (enrollment.Status != "Active")
                    { reason = $"Cannot drop — enrollment status is '{enrollment.Status}'."; return false; }

                int daysSinceEnroll = (int)(DateTime.Now - enrollment.EnrolledDate).TotalDays;
                if (daysSinceEnroll > MaxDropDays)
                    { reason = $"Drop deadline exceeded ({daysSinceEnroll} days since enrollment; max {MaxDropDays})."; return false; }

                return true;
                }
            catch (Exception ex)
                { reason = $"Validation error: {ex.Message}"; return false; }
            }

        // ── CanMarkAsCompleted ────────────────────────────────────────

        /// <summary>
        /// Verifies the enrollment exists and that progress is at 100%.
        /// </summary>
        public bool CanMarkAsCompleted(int enrollmentId, out string reason)
            {
            reason = string.Empty;
            try
                {
                using var ctx = new SmartLearnDbContext();

                var enrollment = ctx.Enrollments.Find(enrollmentId);
                if (enrollment == null)
                    { reason = $"Enrollment {enrollmentId} not found."; return false; }

                if (enrollment.ProgressPercent < 100)
                    { reason = $"Progress is only {enrollment.ProgressPercent}% — must reach 100% to complete."; return false; }

                return true;
                }
            catch (Exception ex)
                { reason = $"Validation error: {ex.Message}"; return false; }
            }

        // ── CanCreateCourse ───────────────────────────────────────────

        /// <summary>
        /// Verifies the instructor exists, the course title is unique,
        /// and the instructor has not exceeded the course limit.
        /// </summary>
        public bool CanCreateCourse(string title, int instructorId, out string reason)
            {
            reason = string.Empty;
            try
                {
                using var ctx = new SmartLearnDbContext();

                var instructor = ctx.Users.Find(instructorId);
                if (instructor == null || instructor.UserType != "Instructor")
                    { reason = $"Instructor with ID {instructorId} not found."; return false; }

                bool titleExists = ctx.Courses
                    .Any(c => c.Title.ToLower() == title.ToLower().Trim());
                if (titleExists)
                    { reason = $"A course named '{title}' already exists."; return false; }

                int instructorCourseCount = ctx.Courses
                    .Count(c => c.InstructorId == instructorId);
                if (instructorCourseCount >= MaxInstructorCourses)
                    { reason = $"Instructor has reached the maximum of {MaxInstructorCourses} courses."; return false; }

                return true;
                }
            catch (Exception ex)
                { reason = $"Validation error: {ex.Message}"; return false; }
            }

        // ── CanUpdateProgress ─────────────────────────────────────────

        /// <summary>
        /// Verifies progress is 0-100, the enrollment exists, and that
        /// the new value is not lower than the current value.
        /// </summary>
        public bool CanUpdateProgress(int studentId, int courseId, double newProgress, out string reason)
            {
            reason = string.Empty;

            if (newProgress < 0 || newProgress > 100)
                { reason = "Progress must be between 0 and 100."; return false; }

            try
                {
                using var ctx = new SmartLearnDbContext();

                var enrollment = ctx.Enrollments
                    .FirstOrDefault(e => e.StudentId == studentId && e.CourseId == courseId);

                if (enrollment == null)
                    { reason = $"No enrollment found for student {studentId} in course {courseId}."; return false; }

                if (newProgress < enrollment.ProgressPercent)
                    { reason = $"Progress cannot decrease from {enrollment.ProgressPercent}% to {newProgress}%."; return false; }

                return true;
                }
            catch (Exception ex)
                { reason = $"Validation error: {ex.Message}"; return false; }
            }
        }
    }