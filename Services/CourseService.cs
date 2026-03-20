using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Week_1.Services
    {
    /// <summary>
    /// Assignment 7 — Part 3 Task 3: CourseService
    /// Search, popular courses, full details, and course recommendations.
    /// </summary>
    public class CourseService
        {
        // ══════════════════════════════════════════════════════
        //  SearchCourses
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// Search courses by keyword (in title or description),
        /// optional category, and optional difficulty level.
        /// Includes instructor details and enrollment count.
        /// All parameters are nullable — pass null to skip that filter.
        /// </summary>
        public List<CourseEntity> SearchCourses(string keyword, string category, string difficulty)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                var query = ctx.Courses
                    .Include(c => c.Instructor)
                    .AsQueryable();

                // Keyword filter: title or description
                if (!string.IsNullOrWhiteSpace(keyword))
                    query = query.Where(c =>
                        c.Title.Contains(keyword) ||
                        (c.Description != null && c.Description.Contains(keyword)));

                // Optional category filter
                if (!string.IsNullOrWhiteSpace(category))
                    query = query.Where(c => c.Category == category);

                // Optional difficulty filter
                if (!string.IsNullOrWhiteSpace(difficulty))
                    query = query.Where(c => c.DifficultyLevel == difficulty);

                return query.OrderBy(c => c.Title).ToList();
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return new List<CourseEntity>(); }
            }

        // ══════════════════════════════════════════════════════
        //  GetPopularCourses
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// Returns top N courses sorted by enrollment count.
        /// Includes instructor name, category, enrollment count, and average rating.
        /// </summary>
        public List<CourseEntity> GetPopularCourses(int topN)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                return ctx.Courses
                    .Include(c => c.Instructor)
                    .Include(c => c.Ratings)
                    .OrderByDescending(c => c.CurrentEnrollments)
                    .Take(topN)
                    .ToList();
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return new List<CourseEntity>(); }
            }

        // ══════════════════════════════════════════════════════
        //  GetCourseWithFullDetails
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// Load a course with ALL related data:
        ///  – Instructor
        ///  – Modules → Lessons
        ///  – Enrollments → Student
        ///  – Ratings → Student
        /// Uses multiple Include and ThenInclude statements.
        /// Displays the complete course structure.
        /// </summary>
        public CourseEntity GetCourseWithFullDetails(int courseId)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();

                var course = ctx.Courses
                    // Branch 1: Instructor
                    .Include(c => c.Instructor)
                    // Branch 2: Modules → Lessons
                    .Include(c => c.Modules)
                        .ThenInclude(m => m.Lessons)
                    // Branch 3: Enrollments → Student
                    .Include(c => c.Enrollments)
                        .ThenInclude(e => e.Student)
                    // Branch 4: Ratings → Student
                    .Include(c => c.Ratings)
                        .ThenInclude(r => r.Student)
                    .FirstOrDefault(c => c.CourseId == courseId);

                if (course == null) { Console.WriteLine("  ✗ Course not found."); return null; }

                DisplayFullCourseDetails(course);
                return course;
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return null; }
            }

        // ══════════════════════════════════════════════════════
        //  GetRecommendedCourses
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// Recommends courses the student is NOT enrolled in,
        /// matching categories they have completed or are currently taking.
        /// Orders by average rating descending.
        /// </summary>
        public List<CourseEntity> GetRecommendedCourses(int studentId)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();

                // Courses the student is currently enrolled in
                var enrolledCourseIds = ctx.Enrollments
                    .Where(e => e.StudentId == studentId)
                    .Select(e => e.CourseId)
                    .ToHashSet();

                // Categories of those enrolled courses
                var relevantCategories = ctx.Enrollments
                    .Include(e => e.Course)
                    .Where(e => e.StudentId == studentId)
                    .Select(e => e.Course.Category)
                    .Distinct()
                    .ToList();

                if (!relevantCategories.Any())
                    {
                    // Student has no enrollments — return top-rated courses overall
                    return ctx.Courses
                        .Include(c => c.Instructor)
                        .Include(c => c.Ratings)
                        .Where(c => !enrolledCourseIds.Contains(c.CourseId))
                        .OrderByDescending(c => c.Ratings.Any()
                            ? c.Ratings.Average(r => r.Rating) : 0)
                        .Take(10)
                        .ToList();
                    }

                // Courses NOT enrolled, in matching categories, ordered by avg rating
                return ctx.Courses
                    .Include(c => c.Instructor)
                    .Include(c => c.Ratings)
                    .Where(c => !enrolledCourseIds.Contains(c.CourseId)
                             && relevantCategories.Contains(c.Category))
                    .OrderByDescending(c => c.Ratings.Any()
                        ? c.Ratings.Average(r => r.Rating) : 0)
                    .ToList();
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return new List<CourseEntity>(); }
            }

        // ── DISPLAY HELPERS ──────────────────────────────────

        public void DisplayPopularCourses(List<CourseEntity> courses)
            {
            if (!courses.Any()) { Console.WriteLine("  No courses found."); return; }
            Console.WriteLine($"\n  {"Rank",-6}{"Title",-34}{"Instructor",-22}{"Enrolled",-10}{"Avg Rating"}");
            Console.WriteLine("  " + new string('─', 80));
            int rank = 1;
            foreach (var c in courses)
                {
                double avg = c.Ratings?.Any() == true ? c.Ratings.Average(r => r.Rating) : 0;
                string rating = avg > 0 ? $"{avg:F2} ★" : "No ratings";
                Console.WriteLine($"  {rank++,-6}{c.Title[..Math.Min(c.Title.Length, 32)],-34}{c.Instructor?.Username ?? "N/A",-22}{c.CurrentEnrollments,-10}{rating}");
                }
            }

        public void DisplayRecommendedCourses(List<CourseEntity> courses)
            {
            if (!courses.Any()) { Console.WriteLine("  No recommendations found."); return; }
            Console.WriteLine($"\n  Recommended Courses ({courses.Count}):");
            Console.WriteLine($"  {"Title",-34}{"Category",-20}{"Difficulty",-14}Avg Rating");
            Console.WriteLine("  " + new string('─', 80));
            foreach (var c in courses)
                {
                double avg = c.Ratings?.Any() == true ? c.Ratings.Average(r => r.Rating) : 0;
                string rating = avg > 0 ? $"{avg:F2} ★" : "No ratings";
                Console.WriteLine($"  {c.Title[..Math.Min(c.Title.Length, 32)],-34}{c.Category,-20}{c.DifficultyLevel,-14}{rating}");
                }
            }

        private void DisplayFullCourseDetails(CourseEntity c)
            {
            Console.WriteLine($"\n╔══════════════════════════════════════════════════╗");
            Console.WriteLine($"║  COURSE DETAILS: {c.Title[..Math.Min(c.Title.Length, 32)],-33}║");
            Console.WriteLine($"╚══════════════════════════════════════════════════╝");
            Console.WriteLine($"  ID          : {c.CourseId}");
            Console.WriteLine($"  Category    : {c.Category}  |  Difficulty: {c.DifficultyLevel}");
            Console.WriteLine($"  Instructor  : {c.Instructor?.Username ?? c.InstructorName ?? "N/A"}");
            Console.WriteLine($"  Enrolled    : {c.CurrentEnrollments}/{c.MaxCapacity}");
            Console.WriteLine($"  Description : {c.Description ?? "N/A"}");

            // Modules & Lessons
            Console.WriteLine($"\n  Modules ({c.Modules?.Count ?? 0}):");
            if (c.Modules?.Any() == true)
                foreach (var m in c.Modules.OrderBy(m => m.OrderIndex))
                    {
                    Console.WriteLine($"    [{m.OrderIndex}] {m.Title}  ({m.DurationHours}h)");
                    if (m.Lessons?.Any() == true)
                        foreach (var l in m.Lessons.OrderBy(l => l.OrderIndex))
                            Console.WriteLine($"         • {l.Title} ({l.DurationMinutes} min)");
                    }
            else
                Console.WriteLine("    No modules yet.");

            // Enrollments
            Console.WriteLine($"\n  Enrolled Students ({c.Enrollments?.Count ?? 0}):");
            if (c.Enrollments?.Any() == true)
                foreach (var e in c.Enrollments.OrderByDescending(e => e.ProgressPercent).Take(5))
                    Console.WriteLine($"    • {e.Student?.Username ?? "N/A",-22} {e.ProgressPercent,3}%  ({e.Status})");

            // Ratings
            double avgRating = c.Ratings?.Any() == true ? c.Ratings.Average(r => r.Rating) : 0;
            Console.WriteLine($"\n  Ratings ({c.Ratings?.Count ?? 0}) — Avg: {(avgRating > 0 ? $"{avgRating:F2} ★" : "None")}");
            if (c.Ratings?.Any() == true)
                foreach (var r in c.Ratings.OrderByDescending(r => r.RatingDate).Take(3))
                    Console.WriteLine($"    {r.Rating} ★  {r.Student?.Username ?? "N/A",-18}  \"{r.ReviewText?.Substring(0, Math.Min(r.ReviewText.Length, 40))}\"");
            }
        }
    }