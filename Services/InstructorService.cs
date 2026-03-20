using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Week_1.Services
    {
    /// <summary>
    /// Assignment 7 — Part 3 Task 2: InstructorService
    /// Provides dashboard, analytics, and at-risk detection for instructors.
    /// </summary>
    public class InstructorService
        {
        // ══════════════════════════════════════════════════════
        //  GetInstructorDashboard
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// Display all courses taught by the instructor with:
        ///  – Enrollment count, capacity, average student progress.
        ///  – Top 3 performing students per course.
        /// Uses ThenInclude() chains to load courses → enrollments → students.
        /// </summary>
        public void GetInstructorDashboard(int instructorId)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();

                var instructor = ctx.Users
                    .Include(u => u.TaughtCourses)
                        .ThenInclude(c => c.Enrollments)
                            .ThenInclude(e => e.Student)
                    .FirstOrDefault(u => u.UserId == instructorId && u.UserType == "Instructor");

                if (instructor == null) { Console.WriteLine("  ✗ Instructor not found."); return; }

                Console.WriteLine($"\n╔══════════════════════════════════════════════╗");
                Console.WriteLine($"║  INSTRUCTOR DASHBOARD — {instructor.Username,-20}║");
                Console.WriteLine($"╚══════════════════════════════════════════════╝");

                if (!instructor.TaughtCourses.Any())
                    { Console.WriteLine("  No courses assigned yet."); return; }

                foreach (var course in instructor.TaughtCourses.OrderBy(c => c.Title))
                    {
                    var enrollments = course.Enrollments.ToList();
                    double avgProgress = enrollments.Any()
                        ? enrollments.Average(e => e.ProgressPercent)
                        : 0;

                    Console.WriteLine($"\n  ── [{course.CourseId}] {course.Title}");
                    Console.WriteLine($"     Category  : {course.Category} | Difficulty: {course.DifficultyLevel}");
                    Console.WriteLine($"     Enrolled  : {course.CurrentEnrollments}/{course.MaxCapacity}  |  Avg Progress: {avgProgress:F1}%");

                    // Top 3 performing students
                    var top3 = enrollments
                        .OrderByDescending(e => e.ProgressPercent)
                        .Take(3)
                        .ToList();

                    if (top3.Any())
                        {
                        Console.WriteLine("     Top Students:");
                        foreach (var e in top3)
                            Console.WriteLine($"       • {e.Student?.Username ?? "N/A",-22} {e.ProgressPercent,3}%  ({e.Status})");
                        }
                    else
                        Console.WriteLine("     No enrollments yet.");
                    }
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); }
            }

        // ══════════════════════════════════════════════════════
        //  GetCourseAnalytics
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// Comprehensive analytics for a single course:
        ///  – Total enrollments, avg progress, completion rate, dropout rate, avg rating.
        ///  – Progress distribution: 0-25%, 25-75%, 75-100%.
        /// </summary>
        public void GetCourseAnalytics(int courseId)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();

                var course = ctx.Courses
                    .Include(c => c.Enrollments)
                        .ThenInclude(e => e.Student)
                    .Include(c => c.Ratings)
                    .FirstOrDefault(c => c.CourseId == courseId);

                if (course == null) { Console.WriteLine("  ✗ Course not found."); return; }

                var enrollments = course.Enrollments.ToList();
                int total = enrollments.Count;

                if (total == 0)
                    { Console.WriteLine($"  [{course.CourseId}] {course.Title} — No enrollments yet."); return; }

                int completed = enrollments.Count(e => e.Status == "Completed");
                int dropped = enrollments.Count(e => e.Status == "Dropped");
                double avgProg = enrollments.Average(e => e.ProgressPercent);
                double completionRate = (double)completed / total * 100;
                double dropoutRate = (double)dropped / total * 100;
                double avgRating = course.Ratings.Any()
                    ? course.Ratings.Average(r => r.Rating)
                    : 0;

                // Progress distribution buckets
                int low = enrollments.Count(e => e.ProgressPercent < 25);
                int mid = enrollments.Count(e => e.ProgressPercent >= 25 && e.ProgressPercent < 75);
                int high = enrollments.Count(e => e.ProgressPercent >= 75);

                Console.WriteLine($"\n  ── Course Analytics: {course.Title} ──────────────");
                Console.WriteLine($"  Category       : {course.Category}  |  Difficulty: {course.DifficultyLevel}");
                Console.WriteLine($"  Total Enrolled : {total}  (Capacity: {course.MaxCapacity})");
                Console.WriteLine($"  Avg Progress   : {avgProg:F1}%");
                Console.WriteLine($"  Completion Rate: {completionRate:F1}%  ({completed} completed)");
                Console.WriteLine($"  Dropout Rate   : {dropoutRate:F1}%  ({dropped} dropped)");
                Console.WriteLine($"  Avg Rating     : {(avgRating > 0 ? $"{avgRating:F2} ★ ({course.Ratings.Count} ratings)" : "No ratings yet")}");
                Console.WriteLine($"\n  Progress Distribution:");
                Console.WriteLine($"    0–25%  : {low,3} students");
                Console.WriteLine($"    25–75% : {mid,3} students");
                Console.WriteLine($"    75–100%: {high,3} students");
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); }
            }

        // ══════════════════════════════════════════════════════
        //  GetTopStudentsInCourse
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// Returns the top N students ranked by progress percentage for a given course.
        /// Includes student name, progress, status, and enrollment date.
        /// </summary>
        public List<EnrollmentEntity> GetTopStudentsInCourse(int courseId, int topN)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                return ctx.Enrollments
                    .Include(e => e.Student)
                    .Where(e => e.CourseId == courseId)
                    .OrderByDescending(e => e.ProgressPercent)
                    .Take(topN)
                    .ToList();
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return new List<EnrollmentEntity>(); }
            }

        // ══════════════════════════════════════════════════════
        //  GetStudentsAtRisk
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// Finds all students across the instructor's courses who are at risk:
        ///  – Progress &lt; 30% AND enrolled more than 2 weeks ago.
        /// Groups results by student and shows which courses they're struggling with.
        /// </summary>
        public void GetStudentsAtRisk(int instructorId)
            {
            var twoWeeksAgo = DateTime.Now.AddDays(-14);
            try
                {
                using var ctx = new SmartLearnDbContext();

                var atRisk = ctx.Enrollments
                    .Include(e => e.Student)
                    .Include(e => e.Course)
                    .Where(e => e.Course.InstructorId == instructorId
                             && e.Status == "Active"
                             && e.ProgressPercent < 30
                             && e.EnrolledDate <= twoWeeksAgo)
                    .OrderBy(e => e.Student.Username)
                    .ThenBy(e => e.ProgressPercent)
                    .ToList();

                if (!atRisk.Any())
                    { Console.WriteLine("  ✓ No at-risk students found."); return; }

                Console.WriteLine($"\n  ⚠  At-Risk Students ({atRisk.Count} enrollments):");

                // Group by student for a clearer report
                var grouped = atRisk
                    .GroupBy(e => e.Student?.Username ?? $"ID:{e.StudentId}")
                    .OrderBy(g => g.Key);

                foreach (var group in grouped)
                    {
                    Console.WriteLine($"\n  Student: {group.Key}");
                    foreach (var e in group)
                        {
                        int days = (int)(DateTime.Now - e.EnrolledDate).TotalDays;
                        Console.WriteLine($"    • {e.Course?.Title ?? $"ID:{e.CourseId}",-36} {e.ProgressPercent,3}%  ({days} days since enroll)");
                        }
                    }
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); }
            }

        // ── DISPLAY HELPERS ──────────────────────────────────

        public void DisplayTopStudents(List<EnrollmentEntity> enrollments)
            {
            if (!enrollments.Any()) { Console.WriteLine("  No students found."); return; }
            Console.WriteLine($"\n  {"Rank",-6}{"Student",-24}{"Progress",-10}{"Status",-12}Enrolled");
            Console.WriteLine("  " + new string('─', 65));
            int rank = 1;
            foreach (var e in enrollments)
                Console.WriteLine($"  {rank++,-6}{e.Student?.Username ?? "N/A",-24}{e.ProgressPercent + "%",-10}{e.Status,-12}{e.EnrolledDate:d}");
            }
        }
    }