using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Week_1.Services
    {
    // ── DTOs for analytics results ──────────────────────────

    public class SystemStats
        {
        public int TotalStudents { get; set; }
        public int TotalInstructors { get; set; }
        public int TotalCourses { get; set; }
        public int TotalEnrollments { get; set; }
        public int ActiveEnrollments { get; set; }
        public int CompletedEnrollments { get; set; }
        public double AvgSystemProgress { get; set; }
        public double AvgEnrollmentsPerStudent { get; set; }
        public double AvgStudentsPerCourse { get; set; }
        }

    public class TopPerformer
        {
        public string Username { get; set; }
        public int UserId { get; set; }
        public double AvgProgress { get; set; }
        public int CompletedCourses { get; set; }
        public int TotalCourses { get; set; }
        }

    public class CategoryStats
        {
        public string Category { get; set; }
        public int CourseCount { get; set; }
        public int TotalEnrollments { get; set; }
        public double AvgProgress { get; set; }
        public double AvgRating { get; set; }
        }

    public class AtRiskStudent
        {
        public string Username { get; set; }
        public int UserId { get; set; }
        public List<(string CourseTitle, int Progress, int DaysEnrolled)> StrugglingCourses { get; set; }
            = new();
        }

    public class CourseCompletionRate
        {
        public string CourseTitle { get; set; }
        public int TotalEnrollments { get; set; }
        public int CompletedEnrollments { get; set; }
        public double CompletionPercentage { get; set; }
        }

    // ══════════════════════════════════════════════════════════
    //  ANALYTICS SERVICE
    // ══════════════════════════════════════════════════════════

    /// <summary>
    /// Assignment 7 — Part 4: AnalyticsService
    /// System-wide statistics, top performers, category popularity,
    /// enrollment trends, instructor rankings, and risk analysis.
    /// </summary>
    public class AnalyticsService
        {
        // ══════════════════════════════════════════════════════
        //  PART 4 TASK 1: CORE ANALYTICS
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// Calculate system-wide statistics:
        ///  – Total students, instructors, courses, enrollments.
        ///  – Active vs completed enrollments.
        ///  – System-wide average progress.
        ///  – Averages: enrollments per student, students per course.
        /// </summary>
        public SystemStats GetSystemWideStatistics()
            {
            try
                {
                using var ctx = new SmartLearnDbContext();

                int totalStudents = ctx.Users.Count(u => u.UserType == "Student");
                int totalInstructors = ctx.Users.Count(u => u.UserType == "Instructor");
                int totalCourses = ctx.Courses.Count();
                int totalEnrollments = ctx.Enrollments.Count();
                int activeEnroll = ctx.Enrollments.Count(e => e.Status == "Active");
                int completedEnroll = ctx.Enrollments.Count(e => e.Status == "Completed");
                double avgProgress = totalEnrollments > 0
                    ? ctx.Enrollments.Average(e => e.ProgressPercent)
                    : 0;
                double avgPerStudent = totalStudents > 0
                    ? (double)totalEnrollments / totalStudents
                    : 0;
                double avgPerCourse = totalCourses > 0
                    ? (double)totalEnrollments / totalCourses
                    : 0;

                return new SystemStats
                    {
                    TotalStudents = totalStudents,
                    TotalInstructors = totalInstructors,
                    TotalCourses = totalCourses,
                    TotalEnrollments = totalEnrollments,
                    ActiveEnrollments = activeEnroll,
                    CompletedEnrollments = completedEnroll,
                    AvgSystemProgress = Math.Round(avgProgress, 2),
                    AvgEnrollmentsPerStudent = Math.Round(avgPerStudent, 2),
                    AvgStudentsPerCourse = Math.Round(avgPerCourse, 2)
                    };
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return new SystemStats(); }
            }

        /// <summary>
        /// Display the system statistics dashboard.
        /// </summary>
        public void DisplaySystemStats()
            {
            var s = GetSystemWideStatistics();
            Console.WriteLine("\n╔══════════════════════════════════════════════╗");
            Console.WriteLine("║        SMARTLEARN — SYSTEM ANALYTICS         ║");
            Console.WriteLine("╠══════════════════════════════════════════════╣");
            Console.WriteLine($"║  Students          : {s.TotalStudents,-25}║");
            Console.WriteLine($"║  Instructors       : {s.TotalInstructors,-25}║");
            Console.WriteLine($"║  Courses           : {s.TotalCourses,-25}║");
            Console.WriteLine($"║  Total Enrollments : {s.TotalEnrollments,-25}║");
            Console.WriteLine($"║    Active          : {s.ActiveEnrollments,-25}║");
            Console.WriteLine($"║    Completed       : {s.CompletedEnrollments,-25}║");
            Console.WriteLine($"║  Avg Progress      : {s.AvgSystemProgress + "%",-25}║");
            Console.WriteLine($"║  Enroll/Student    : {s.AvgEnrollmentsPerStudent,-25}║");
            Console.WriteLine($"║  Students/Course   : {s.AvgStudentsPerCourse,-25}║");
            Console.WriteLine("╚══════════════════════════════════════════════╝");
            }

        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Rank top N students by average progress.
        /// Secondary sort: number of completed courses.
        /// </summary>
        public List<TopPerformer> GetTopPerformers(int topN)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                return ctx.Users
                    .Include(u => u.Enrollments)
                    .Where(u => u.UserType == "Student" && u.Enrollments.Any())
                    .AsEnumerable()   // Switch to client-side for complex aggregation
                    .Select(u => new TopPerformer
                        {
                        UserId = u.UserId,
                        Username = u.Username,
                        AvgProgress = Math.Round(u.Enrollments.Average(e => e.ProgressPercent), 2),
                        CompletedCourses = u.Enrollments.Count(e => e.Status == "Completed"),
                        TotalCourses = u.Enrollments.Count
                        })
                    .OrderByDescending(p => p.AvgProgress)
                    .ThenByDescending(p => p.CompletedCourses)
                    .Take(topN)
                    .ToList();
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return new List<TopPerformer>(); }
            }

        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Category popularity: count courses, total enrollments,
        /// average progress, and average rating per category.
        /// Ordered by total enrollments descending.
        /// </summary>
        public List<CategoryStats> GetCategoryPopularity()
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                return ctx.Courses
                    .Include(c => c.Enrollments)
                    .Include(c => c.Ratings)
                    .AsEnumerable()
                    .GroupBy(c => c.Category)
                    .Select(g => new CategoryStats
                        {
                        Category = g.Key,
                        CourseCount = g.Count(),
                        TotalEnrollments = g.Sum(c => c.CurrentEnrollments),
                        AvgProgress = g.SelectMany(c => c.Enrollments).Any()
                            ? Math.Round(g.SelectMany(c => c.Enrollments).Average(e => e.ProgressPercent), 2)
                            : 0,
                        AvgRating = g.SelectMany(c => c.Ratings).Any()
                            ? Math.Round(g.SelectMany(c => c.Ratings).Average(r => r.Rating), 2)
                            : 0
                        })
                    .OrderByDescending(c => c.TotalEnrollments)
                    .ToList();
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return new List<CategoryStats>(); }
            }

        // ══════════════════════════════════════════════════════
        //  PART 4 TASK 2: TREND ANALYSIS
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// Group enrollments by year and month.
        /// Returns data suitable for graphing enrollment growth.
        /// Key = "YYYY-MM", Value = (NewEnrollments, UniqueStudents).
        /// </summary>
        public Dictionary<string, (int NewEnrollments, int UniqueStudents)> GetEnrollmentTrendsByMonth()
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                return ctx.Enrollments
                    .AsEnumerable() 
                    .GroupBy(e => new { e.EnrolledDate.Year, e.EnrolledDate.Month })
                    .OrderBy(g => g.Key.Year)
                    .ThenBy(g => g.Key.Month)
                    .ToDictionary(
                        g => $"{g.Key.Year}-{g.Key.Month:D2}",
                        g => (
                            NewEnrollments: g.Count(),
                            UniqueStudents: g.Select(e => e.StudentId).Distinct().Count()
                        )
                    );
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return new Dictionary<string, (int, int)>(); }
            }

        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Rank instructors by total unique student count.
        /// Includes average rating across all courses and number of courses taught.
        /// </summary>
        public void GetInstructorRankings()
            {
            try
                {
                using var ctx = new SmartLearnDbContext();

                var rankings = ctx.Users
                    .Include(u => u.TaughtCourses)
                        .ThenInclude(c => c.Enrollments)
                    .Include(u => u.TaughtCourses)
                        .ThenInclude(c => c.Ratings)
                    .Where(u => u.UserType == "Instructor")
                    .AsEnumerable()
                    .Select(u =>
                    {
                        var allEnrollments = u.TaughtCourses.SelectMany(c => c.Enrollments).ToList();
                        var allRatings = u.TaughtCourses.SelectMany(c => c.Ratings).ToList();
                        return new
                            {
                            u.Username,
                            CourseCount = u.TaughtCourses.Count,
                            UniqueStudents = allEnrollments.Select(e => e.StudentId).Distinct().Count(),
                            AvgRating = allRatings.Any()
                                ? Math.Round(allRatings.Average(r => r.Rating), 2) : 0.0
                            };
                    })
                    .OrderByDescending(r => r.UniqueStudents)
                    .ToList();

                Console.WriteLine("\n  Instructor Rankings:");
                Console.WriteLine($"  {"Rank",-6}{"Instructor",-24}{"Courses",-10}{"Students",-12}Avg Rating");
                Console.WriteLine("  " + new string('─', 62));
                int rank = 1;
                foreach (var r in rankings)
                    Console.WriteLine($"  {rank++,-6}{r.Username,-24}{r.CourseCount,-10}{r.UniqueStudents,-12}{(r.AvgRating > 0 ? $"{r.AvgRating:F2} ★" : "N/A")}");
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); }
            }

        // ══════════════════════════════════════════════════════
        //  PART 4 TASK 3: RISK ANALYSIS
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// Find students with active enrollments where:
        ///  – Progress &lt; 30%
        ///  – Enrolled more than 2 weeks ago.
        /// Groups by student and shows which courses they're struggling with.
        /// </summary>
        public List<AtRiskStudent> GetStudentsAtRisk()
            {
            var twoWeeksAgo = DateTime.Now.AddDays(-14);
            try
                {
                using var ctx = new SmartLearnDbContext();

                var atRiskEnrollments = ctx.Enrollments
                    .Include(e => e.Student)
                    .Include(e => e.Course)
                    .Where(e => e.Status == "Active"
                             && e.ProgressPercent < 30
                             && e.EnrolledDate <= twoWeeksAgo)
                    .AsEnumerable()
                    .GroupBy(e => e.StudentId)
                    .Select(g => new AtRiskStudent
                        {
                        UserId = g.Key,
                        Username = g.First().Student?.Username ?? $"ID:{g.Key}",
                        StrugglingCourses = g.Select(e => (
                            e.Course?.Title ?? $"ID:{e.CourseId}",
                            e.ProgressPercent,
                            (int)(DateTime.Now - e.EnrolledDate).TotalDays
                        )).ToList()
                        })
                    .OrderByDescending(s => s.StrugglingCourses.Count)
                    .ToList();

                return atRiskEnrollments;
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return new List<AtRiskStudent>(); }
            }

        // ─────────────────────────────────────────────────────

        /// <summary>
        /// For each course, calculate completion rate:
        ///   (Completed enrollments / Total enrollments) * 100
        /// Ordered by completion rate ascending to identify problematic courses.
        /// </summary>
        public List<CourseCompletionRate> GetCourseCompletionRates()
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                return ctx.Courses
                    .Include(c => c.Enrollments)
                    .AsEnumerable()
                    .Where(c => c.Enrollments.Any())   // Exclude courses with no enrollments
                    .Select(c =>
                    {
                        int total = c.Enrollments.Count;
                        int completed = c.Enrollments.Count(e => e.Status == "Completed");
                        return new CourseCompletionRate
                            {
                            CourseTitle = c.Title,
                            TotalEnrollments = total,
                            CompletedEnrollments = completed,
                            CompletionPercentage = Math.Round((double)completed / total * 100, 2)
                            };
                    })
                    .OrderBy(r => r.CompletionPercentage)
                    .ToList();
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return new List<CourseCompletionRate>(); }
            }

        // ── DISPLAY HELPERS ──────────────────────────────────

        public void DisplayTopPerformers(List<TopPerformer> performers)
            {
            if (!performers.Any()) { Console.WriteLine("  No data."); return; }
            Console.WriteLine($"\n  {"Rank",-6}{"Student",-24}{"Avg Progress",-14}{"Completed",-12}Total");
            Console.WriteLine("  " + new string('─', 62));
            int rank = 1;
            foreach (var p in performers)
                Console.WriteLine($"  {rank++,-6}{p.Username,-24}{p.AvgProgress + "%",-14}{p.CompletedCourses,-12}{p.TotalCourses}");
            }

        public void DisplayCategoryPopularity(List<CategoryStats> stats)
            {
            if (!stats.Any()) { Console.WriteLine("  No data."); return; }
            Console.WriteLine($"\n  {"Category",-22}{"Courses",-10}{"Enrolled",-10}{"Avg Prog",-10}Avg Rating");
            Console.WriteLine("  " + new string('─', 65));
            foreach (var s in stats)
                Console.WriteLine($"  {s.Category,-22}{s.CourseCount,-10}{s.TotalEnrollments,-10}{s.AvgProgress + "%",-10}{(s.AvgRating > 0 ? $"{s.AvgRating:F2} ★" : "N/A")}");
            }

        public void DisplayAtRiskStudents(List<AtRiskStudent> students)
            {
            if (!students.Any()) { Console.WriteLine("  ✓ No at-risk students found!"); return; }
            Console.WriteLine($"\n  ⚠  At-Risk Students ({students.Count}):");
            foreach (var s in students)
                {
                Console.WriteLine($"\n  Student: {s.Username}");
                foreach (var (title, progress, days) in s.StrugglingCourses)
                    Console.WriteLine($"    • {title,-36} {progress,3}%  (enrolled {days} days ago)");
                }
            }

        public void DisplayCourseCompletionRates(List<CourseCompletionRate> rates)
            {
            if (!rates.Any()) { Console.WriteLine("  No data."); return; }
            Console.WriteLine($"\n  {"Course",-36}{"Total",-8}{"Completed",-12}Completion %");
            Console.WriteLine("  " + new string('─', 65));
            foreach (var r in rates)
                Console.WriteLine($"  {r.CourseTitle[..Math.Min(r.CourseTitle.Length, 34)],-36}{r.TotalEnrollments,-8}{r.CompletedEnrollments,-12}{r.CompletionPercentage:F1}%");
            }

        public void DisplayEnrollmentTrends(Dictionary<string, (int NewEnrollments, int UniqueStudents)> trends)
            {
            if (!trends.Any()) { Console.WriteLine("  No enrollment data."); return; }
            Console.WriteLine($"\n  {"Month",-12}{"New Enrollments",-18}Unique Students");
            Console.WriteLine("  " + new string('─', 44));
            foreach (var kv in trends)
                Console.WriteLine($"  {kv.Key,-12}{kv.Value.NewEnrollments,-18}{kv.Value.UniqueStudents}");
            }
        }
    }