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
    /// Assignment 8 — CourseService with exception handling, validation,
    /// logging, and optimised queries (AsNoTracking, projection).
    /// </summary>
    public class CourseService
        {
        private static readonly Logger _log = Logger.Instance;
        private static readonly AuditService _audit = new();
        private readonly BusinessRuleValidator _rules = new();

        // ══════════════════════════════════════════════════════════════
        //  SearchCourses
        // ══════════════════════════════════════════════════════════════

        /// <summary>
        /// Searches courses by keyword (title/description), optional category,
        /// and optional difficulty. Uses AsNoTracking for read performance.
        /// </summary>
        public List<CourseEntity> SearchCourses(string keyword, string category, string difficulty)
            {
            _log.Debug("CourseService",
                       $"SearchCourses: keyword='{keyword}' cat='{category}' diff='{difficulty}'");
            try
                {
                using var ctx = new SmartLearnDbContext();
                var query = ctx.Courses
                    .AsNoTracking()
                    .Include(c => c.Instructor)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(keyword))
                    query = query.Where(c =>
                        c.Title.Contains(keyword) ||
                        (c.Description != null && c.Description.Contains(keyword)));

                if (!string.IsNullOrWhiteSpace(category))
                    query = query.Where(c => c.Category == category);

                if (!string.IsNullOrWhiteSpace(difficulty))
                    query = query.Where(c => c.DifficultyLevel == difficulty);

                var results = query.OrderBy(c => c.Title).ToList();
                _log.Info("CourseService", $"SearchCourses returned {results.Count} result(s).");
                return results;
                }
            catch (Exception ex)
                {
                _log.Error("CourseService", $"SearchCourses failed: {ex.Message}", null, ex);
                Console.WriteLine($"  ✗ Error: {ex.Message}");
                return new List<CourseEntity>();
                }
            }

        // ══════════════════════════════════════════════════════════════
        //  GetPopularCourses
        // ══════════════════════════════════════════════════════════════

        /// <summary>Returns the top N courses by enrollment count.</summary>
        public List<CourseEntity> GetPopularCourses(int topN)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                return ctx.Courses
                    .AsNoTracking()
                    .Include(c => c.Instructor)
                    .Include(c => c.Ratings)
                    .OrderByDescending(c => c.CurrentEnrollments)
                    .Take(topN)
                    .ToList();
                }
            catch (Exception ex)
                {
                _log.Error("CourseService", $"GetPopularCourses failed: {ex.Message}", null, ex);
                return new List<CourseEntity>();
                }
            }

        // ══════════════════════════════════════════════════════════════
        //  GetCourseWithFullDetails
        // ══════════════════════════════════════════════════════════════

        /// <summary>
        /// Loads a course with all related data: instructor, modules, lessons,
        /// enrollments with students, and ratings with students.
        /// Throws <see cref="CourseNotFoundException"/> if not found.
        /// </summary>
        public CourseEntity GetCourseWithFullDetails(int courseId)
            {
            _log.Info("CourseService", $"GetCourseWithFullDetails: courseId={courseId}");
            try
                {
                using var ctx = new SmartLearnDbContext();
                var course = ctx.Courses
                    .AsNoTracking()
                    .Include(c => c.Instructor)
                    .Include(c => c.Modules).ThenInclude(m => m.Lessons)
                    .Include(c => c.Enrollments).ThenInclude(e => e.Student)
                    .Include(c => c.Ratings).ThenInclude(r => r.Student)
                    .FirstOrDefault(c => c.CourseId == courseId);

                if (course == null)
                    throw new CourseNotFoundException(courseId);

                DisplayFullCourseDetails(course);
                return course;
                }
            catch (CourseNotFoundException ex)
                {
                _log.Warning("CourseService", ex.Message);
                Console.WriteLine($"  ✗ {ex.Message}");
                return null;
                }
            catch (Exception ex)
                {
                _log.Error("CourseService", $"GetCourseWithFullDetails failed: {ex.Message}", null, ex);
                Console.WriteLine($"  ✗ Error: {ex.Message}");
                return null;
                }
            }

        // ══════════════════════════════════════════════════════════════
        //  GetRecommendedCourses
        // ══════════════════════════════════════════════════════════════

        /// <summary>
        /// Recommends courses the student is not enrolled in, matching
        /// categories from their current/past enrolments, ordered by avg rating.
        /// </summary>
        public List<CourseEntity> GetRecommendedCourses(int studentId)
            {
            _log.Info("CourseService", $"GetRecommendedCourses: studentId={studentId}");
            try
                {
                using var ctx = new SmartLearnDbContext();

                var enrolledIds = ctx.Enrollments
                    .AsNoTracking()
                    .Where(e => e.StudentId == studentId)
                    .Select(e => e.CourseId)
                    .ToHashSet();

                var categories = ctx.Enrollments
                    .AsNoTracking()
                    .Include(e => e.Course)
                    .Where(e => e.StudentId == studentId)
                    .Select(e => e.Course.Category)
                    .Distinct()
                    .ToList();

                if (!categories.Any())
                    {
                    // No enrollments — return top-rated overall
                    return ctx.Courses
                        .AsNoTracking()
                        .Include(c => c.Instructor)
                        .Include(c => c.Ratings)
                        .Where(c => !enrolledIds.Contains(c.CourseId))
                        .OrderByDescending(c => c.Ratings.Any()
                            ? c.Ratings.Average(r => r.Rating) : 0)
                        .Take(10).ToList();
                    }

                return ctx.Courses
                    .AsNoTracking()
                    .Include(c => c.Instructor)
                    .Include(c => c.Ratings)
                    .Where(c => !enrolledIds.Contains(c.CourseId)
                             && categories.Contains(c.Category))
                    .OrderByDescending(c => c.Ratings.Any()
                        ? c.Ratings.Average(r => r.Rating) : 0)
                    .ToList();
                }
            catch (Exception ex)
                {
                _log.Error("CourseService", $"GetRecommendedCourses failed: {ex.Message}", null, ex);
                return new List<CourseEntity>();
                }
            }

        // ── Display helpers ────────────────────────────────────────────

        /// <summary>Displays popular courses in a formatted ranked table.</summary>
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
                string title = c.Title.Length > 32 ? c.Title[..29] + "..." : c.Title;
                Console.WriteLine($"  {rank++,-6}{title,-34}{c.Instructor?.Username ?? "N/A",-22}" +
                                  $"{c.CurrentEnrollments,-10}{rating}");
                }
            }

        /// <summary>Displays recommended courses in a formatted table.</summary>
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
                string title = c.Title.Length > 32 ? c.Title[..29] + "..." : c.Title;
                Console.WriteLine($"  {title,-34}{c.Category,-20}{c.DifficultyLevel,-14}{rating}");
                }
            }

        private static void DisplayFullCourseDetails(CourseEntity c)
            {
            Console.WriteLine($"\n╔══════════════════════════════════════════════════╗");
            string ttl = c.Title.Length > 32 ? c.Title[..29] + "..." : c.Title;
            Console.WriteLine($"║  COURSE DETAILS: {ttl,-33}║");
            Console.WriteLine($"╚══════════════════════════════════════════════════╝");
            Console.WriteLine($"  ID         : {c.CourseId}");
            Console.WriteLine($"  Category   : {c.Category}  |  Difficulty: {c.DifficultyLevel}");
            Console.WriteLine($"  Instructor : {c.Instructor?.Username ?? c.InstructorName ?? "N/A"}");
            Console.WriteLine($"  Enrolled   : {c.CurrentEnrollments}/{c.MaxCapacity}");
            Console.WriteLine($"  Description: {c.Description ?? "N/A"}");

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

            Console.WriteLine($"\n  Enrolled Students ({c.Enrollments?.Count ?? 0}):");
            if (c.Enrollments?.Any() == true)
                foreach (var e in c.Enrollments.OrderByDescending(e => e.ProgressPercent).Take(5))
                    Console.WriteLine($"    • {e.Student?.Username ?? "N/A",-22} {e.ProgressPercent,3}%  ({e.Status})");

            double avgRating = c.Ratings?.Any() == true ? c.Ratings.Average(r => r.Rating) : 0;
            Console.WriteLine($"\n  Ratings ({c.Ratings?.Count ?? 0}) — Avg: " +
                              $"{(avgRating > 0 ? $"{avgRating:F2} ★" : "None")}");
            if (c.Ratings?.Any() == true)
                foreach (var r in c.Ratings.OrderByDescending(r => r.RatingDate).Take(3))
                    {
                    string review = r.ReviewText?.Length > 40
                        ? r.ReviewText[..37] + "..." : (r.ReviewText ?? "");
                    Console.WriteLine($"    {r.Rating} ★  {r.Student?.Username ?? "N/A",-18}  \"{review}\"");
                    }
            }
        }
    }