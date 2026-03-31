using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Week_1.Exceptions;
using Week_1.Logging;

namespace Week_1.Services
    {
    // ══════════════════════════════════════════════════════════════════
    //  ASSIGNMENT 8: InstructorService (updated)
    // ══════════════════════════════════════════════════════════════════

    /// <summary>
    /// Assignment 8 — InstructorService with full exception handling,
    /// logging, and AsNoTracking on read-only queries.
    /// </summary>
    public class InstructorService
        {
        private static readonly Logger _log = Logger.Instance;
        private static readonly AuditService _audit = new();

        // ── GetInstructorDashboard ─────────────────────────────────────

        /// <summary>
        /// Displays all courses taught by the instructor with enrollment stats
        /// and top-3 performing students per course.
        /// </summary>
        public void GetInstructorDashboard(int instructorId)
            {
            _log.Info("InstructorService", $"GetInstructorDashboard: id={instructorId}");
            try
                {
                using var ctx = new SmartLearnDbContext();

                var instructor = ctx.Users
                    .AsNoTracking()
                    .Include(u => u.TaughtCourses)
                        .ThenInclude(c => c.Enrollments)
                            .ThenInclude(e => e.Student)
                    .FirstOrDefault(u => u.UserId == instructorId && u.UserType == "Instructor");

                if (instructor == null)
                    {
                    _log.Warning("InstructorService",
                                 $"Dashboard requested for unknown instructor {instructorId}.");
                    Console.WriteLine("  ✗ Instructor not found.");
                    return;
                    }

                Console.WriteLine($"\n╔══════════════════════════════════════════════╗");
                Console.WriteLine($"║  INSTRUCTOR DASHBOARD — {instructor.Username,-20}║");
                Console.WriteLine($"╚══════════════════════════════════════════════╝");

                if (!instructor.TaughtCourses.Any())
                    { Console.WriteLine("  No courses assigned yet."); return; }

                foreach (var course in instructor.TaughtCourses.OrderBy(c => c.Title))
                    {
                    var enrollments = course.Enrollments.ToList();
                    double avgProgress = enrollments.Any()
                        ? enrollments.Average(e => e.ProgressPercent) : 0;

                    Console.WriteLine($"\n  ── [{course.CourseId}] {course.Title}");
                    Console.WriteLine($"     Category  : {course.Category} | Difficulty: {course.DifficultyLevel}");
                    Console.WriteLine($"     Enrolled  : {course.CurrentEnrollments}/{course.MaxCapacity}  |  Avg Progress: {avgProgress:F1}%");

                    var top3 = enrollments.OrderByDescending(e => e.ProgressPercent).Take(3).ToList();
                    if (top3.Any())
                        {
                        Console.WriteLine("     Top Students:");
                        foreach (var e in top3)
                            Console.WriteLine($"       • {e.Student?.Username ?? "N/A",-22} " +
                                              $"{e.ProgressPercent,3}%  ({e.Status})");
                        }
                    else
                        Console.WriteLine("     No enrollments yet.");
                    }

                _log.Info("InstructorService",
                          $"Dashboard displayed for '{instructor.Username}'.", instructor.Username);
                _audit.LogAction("InstructorDashboardAccess", "Analytics",
                    username: instructor.Username, userId: instructorId);
                }
            catch (Exception ex)
                {
                _log.Error("InstructorService", $"GetInstructorDashboard failed: {ex.Message}",
                           $"ID:{instructorId}", ex);
                Console.WriteLine($"  ✗ Error: {ex.Message}");
                }
            }

        // ── GetCourseAnalytics ─────────────────────────────────────────

        /// <summary>
        /// Comprehensive analytics for a single course including completion rate,
        /// dropout rate, average rating, and progress distribution buckets.
        /// </summary>
        public void GetCourseAnalytics(int courseId)
            {
            _log.Info("InstructorService", $"GetCourseAnalytics: courseId={courseId}");
            try
                {
                using var ctx = new SmartLearnDbContext();
                var course = ctx.Courses
                    .AsNoTracking()
                    .Include(c => c.Enrollments).ThenInclude(e => e.Student)
                    .Include(c => c.Ratings)
                    .FirstOrDefault(c => c.CourseId == courseId);

                if (course == null)
                    throw new CourseNotFoundException(courseId);

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
                                         ? course.Ratings.Average(r => r.Rating) : 0;

                int low = enrollments.Count(e => e.ProgressPercent < 25);
                int mid = enrollments.Count(e => e.ProgressPercent >= 25 && e.ProgressPercent < 75);
                int high = enrollments.Count(e => e.ProgressPercent >= 75);

                Console.WriteLine($"\n  ── Course Analytics: {course.Title} ──");
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
            catch (CourseNotFoundException ex)
                {
                _log.Warning("InstructorService", ex.Message);
                Console.WriteLine($"  ✗ {ex.Message}");
                }
            catch (Exception ex)
                {
                _log.Error("InstructorService", $"GetCourseAnalytics failed: {ex.Message}", null, ex);
                Console.WriteLine($"  ✗ Error: {ex.Message}");
                }
            }

        // ── GetTopStudentsInCourse ─────────────────────────────────────

        /// <summary>Returns the top N students by progress for a given course.</summary>
        public List<EnrollmentEntity> GetTopStudentsInCourse(int courseId, int topN)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                return ctx.Enrollments
                    .AsNoTracking()
                    .Include(e => e.Student)
                    .Where(e => e.CourseId == courseId)
                    .OrderByDescending(e => e.ProgressPercent)
                    .Take(topN)
                    .ToList();
                }
            catch (Exception ex)
                {
                _log.Error("InstructorService", $"GetTopStudentsInCourse failed: {ex.Message}", null, ex);
                return new List<EnrollmentEntity>();
                }
            }

        // ── GetStudentsAtRisk ──────────────────────────────────────────

        /// <summary>
        /// Finds at-risk students (progress &lt; 30%, enrolled &gt; 2 weeks)
        /// across all of the instructor's courses.
        /// </summary>
        public void GetStudentsAtRisk(int instructorId)
            {
            var twoWeeksAgo = DateTime.Now.AddDays(-14);
            _log.Info("InstructorService", $"GetStudentsAtRisk: instructorId={instructorId}");
            try
                {
                using var ctx = new SmartLearnDbContext();
                var atRisk = ctx.Enrollments
                    .AsNoTracking()
                    .Include(e => e.Student)
                    .Include(e => e.Course)
                    .Where(e => e.Course.InstructorId == instructorId
                             && e.Status == "Active"
                             && e.ProgressPercent < 30
                             && e.EnrolledDate <= twoWeeksAgo)
                    .OrderBy(e => e.Student.Username).ThenBy(e => e.ProgressPercent)
                    .ToList();

                if (!atRisk.Any())
                    { Console.WriteLine("  ✓ No at-risk students found."); return; }

                Console.WriteLine($"\n  ⚠  At-Risk Students ({atRisk.Count} enrollments):");
                foreach (var group in atRisk
                    .GroupBy(e => e.Student?.Username ?? $"ID:{e.StudentId}")
                    .OrderBy(g => g.Key))
                    {
                    Console.WriteLine($"\n  Student: {group.Key}");
                    foreach (var e in group)
                        {
                        int days = (int)(DateTime.Now - e.EnrolledDate).TotalDays;
                        Console.WriteLine($"    • {e.Course?.Title ?? $"ID:{e.CourseId}",-36} " +
                                          $"{e.ProgressPercent,3}%  ({days} days since enroll)");
                        }
                    }
                }
            catch (Exception ex)
                {
                _log.Error("InstructorService", $"GetStudentsAtRisk failed: {ex.Message}", null, ex);
                Console.WriteLine($"  ✗ Error: {ex.Message}");
                }
            }

        // ── Display helper ─────────────────────────────────────────────

        /// <summary>Displays a ranked list of top students for a course.</summary>
        public void DisplayTopStudents(List<EnrollmentEntity> enrollments)
            {
            if (!enrollments.Any()) { Console.WriteLine("  No students found."); return; }
            Console.WriteLine($"\n  {"Rank",-6}{"Student",-24}{"Progress",-10}{"Status",-12}Enrolled");
            Console.WriteLine("  " + new string('─', 65));
            int rank = 1;
            foreach (var e in enrollments)
                Console.WriteLine($"  {rank++,-6}{e.Student?.Username ?? "N/A",-24}" +
                                  $"{e.ProgressPercent + "%",-10}{e.Status,-12}{e.EnrolledDate:d}");
            }
        }

    // ══════════════════════════════════════════════════════════════════
    //  ASSIGNMENT 8: AnalyticsService (merged Wk7 + Wk8)
    // ══════════════════════════════════════════════════════════════════

    // ── DTOs ───────────────────────────────────────────────────────────

    /// <summary>System-wide aggregate statistics.</summary>
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

    /// <summary>Top-performing student summary.</summary>
    public class TopPerformer
        {
        public string Username { get; set; }
        public int UserId { get; set; }
        public double AvgProgress { get; set; }
        public int CompletedCourses { get; set; }
        public int TotalCourses { get; set; }
        }

    /// <summary>Per-category aggregate statistics.</summary>
    public class CategoryStats
        {
        public string Category { get; set; }
        public int CourseCount { get; set; }
        public int TotalEnrollments { get; set; }
        public double AvgProgress { get; set; }
        public double AvgRating { get; set; }
        }

    /// <summary>At-risk student with the courses they are struggling in.</summary>
    public class AtRiskStudent
        {
        public string Username { get; set; }
        public int UserId { get; set; }
        public List<(string CourseTitle, int Progress, int DaysEnrolled)> StrugglingCourses { get; set; }
            = new();
        }

    /// <summary>Completion-rate summary for a single course.</summary>
    public class CourseCompletionRate
        {
        public string CourseTitle { get; set; }
        public int TotalEnrollments { get; set; }
        public int CompletedEnrollments { get; set; }
        public double CompletionPercentage { get; set; }
        }

    /// <summary>
    /// Assignment 7 + 8 — AnalyticsService.
    /// System-wide statistics, top performers, category popularity,
    /// enrollment trends, instructor rankings, risk analysis,
    /// plus Wk8 exception handling and logging throughout.
    /// </summary>
    public class AnalyticsService
        {
        private static readonly Logger _log = Logger.Instance;

        // ══════════════════════════════════════════════════════════════
        //  PART 4 TASK 1: CORE ANALYTICS
        // ══════════════════════════════════════════════════════════════

        /// <summary>
        /// Calculates system-wide statistics: totals, averages, and
        /// active vs completed enrollment breakdown.
        /// </summary>
        public SystemStats GetSystemWideStatistics()
            {
            _log.Info("AnalyticsService", "GetSystemWideStatistics called.");
            try
                {
                using var ctx = new SmartLearnDbContext();

                int totalStudents = ctx.Users.AsNoTracking().Count(u => u.UserType == "Student");
                int totalInstructors = ctx.Users.AsNoTracking().Count(u => u.UserType == "Instructor");
                int totalCourses = ctx.Courses.AsNoTracking().Count();
                int totalEnrollments = ctx.Enrollments.AsNoTracking().Count();
                int activeEnroll = ctx.Enrollments.AsNoTracking().Count(e => e.Status == "Active");
                int completedEnroll = ctx.Enrollments.AsNoTracking().Count(e => e.Status == "Completed");
                double avgProgress = totalEnrollments > 0
                    ? ctx.Enrollments.AsNoTracking().Average(e => e.ProgressPercent) : 0;
                double avgPerStudent = totalStudents > 0
                    ? (double)totalEnrollments / totalStudents : 0;
                double avgPerCourse = totalCourses > 0
                    ? (double)totalEnrollments / totalCourses : 0;

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
            catch (Exception ex)
                {
                _log.Error("AnalyticsService", $"GetSystemWideStatistics failed: {ex.Message}", null, ex);
                return new SystemStats();
                }
            }

        /// <summary>Prints the full system statistics dashboard to the console.</summary>
        public void DisplaySystemStats()
            {
            _log.Info("AnalyticsService", "DisplaySystemStats called.");
            try
                {
                var s = GetSystemWideStatistics();

                Console.WriteLine("\n╔══════════════════════════════════════════════╗");
                Console.WriteLine("║     SMARTLEARN — SYSTEM STATISTICS (Wk8)     ║");
                Console.WriteLine("╠══════════════════════════════════════════════╣");
                Console.WriteLine($"║  Students           : {s.TotalStudents,-25}║");
                Console.WriteLine($"║  Instructors        : {s.TotalInstructors,-25}║");
                Console.WriteLine($"║  Courses            : {s.TotalCourses,-25}║");
                Console.WriteLine($"║  Total Enrollments  : {s.TotalEnrollments,-25}║");
                Console.WriteLine($"║    Active           : {s.ActiveEnrollments,-25}║");
                Console.WriteLine($"║    Completed        : {s.CompletedEnrollments,-25}║");
                Console.WriteLine($"║  Avg Progress       : {s.AvgSystemProgress + "%",-25}║");
                Console.WriteLine($"║  Enroll/Student     : {s.AvgEnrollmentsPerStudent,-25}║");
                Console.WriteLine($"║  Students/Course    : {s.AvgStudentsPerCourse,-25}║");
                Console.WriteLine("╚══════════════════════════════════════════════╝");
                }
            catch (Exception ex)
                {
                _log.Error("AnalyticsService", $"DisplaySystemStats failed: {ex.Message}", null, ex);
                Console.WriteLine($"  ✗ Error: {ex.Message}");
                }
            }

        // ── Top Performers ─────────────────────────────────────────────

        /// <summary>
        /// Ranks the top N students by average progress.
        /// Secondary sort: number of completed courses.
        /// </summary>
        public List<TopPerformer> GetTopPerformers(int topN)
            {
            _log.Info("AnalyticsService", $"GetTopPerformers: topN={topN}");
            try
                {
                using var ctx = new SmartLearnDbContext();
                return ctx.Users
                    .AsNoTracking()
                    .Include(u => u.Enrollments)
                    .Where(u => u.UserType == "Student" && u.Enrollments.Any())
                    .AsEnumerable()
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
            catch (Exception ex)
                {
                _log.Error("AnalyticsService", $"GetTopPerformers failed: {ex.Message}", null, ex);
                return new List<TopPerformer>();
                }
            }

        // ── Category Popularity ────────────────────────────────────────

        /// <summary>
        /// Returns per-category statistics: course count, total enrollments,
        /// average progress, and average rating. Ordered by total enrollments desc.
        /// </summary>
        public List<CategoryStats> GetCategoryPopularity()
            {
            _log.Info("AnalyticsService", "GetCategoryPopularity called.");
            try
                {
                using var ctx = new SmartLearnDbContext();
                return ctx.Courses
                    .AsNoTracking()
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
            catch (Exception ex)
                {
                _log.Error("AnalyticsService", $"GetCategoryPopularity failed: {ex.Message}", null, ex);
                return new List<CategoryStats>();
                }
            }

        // ══════════════════════════════════════════════════════════════
        //  PART 4 TASK 2: TREND ANALYSIS
        // ══════════════════════════════════════════════════════════════

        /// <summary>
        /// Groups enrollments by year-month.
        /// Key = "YYYY-MM", Value = (NewEnrollments, UniqueStudents).
        /// </summary>
        public Dictionary<string, (int NewEnrollments, int UniqueStudents)> GetEnrollmentTrendsByMonth()
            {
            _log.Info("AnalyticsService", "GetEnrollmentTrendsByMonth called.");
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
                        g => (
                            NewEnrollments: g.Count(),
                            UniqueStudents: g.Select(e => e.StudentId).Distinct().Count()
                        )
                    );
                }
            catch (Exception ex)
                {
                _log.Error("AnalyticsService", $"GetEnrollmentTrendsByMonth failed: {ex.Message}", null, ex);
                return new Dictionary<string, (int, int)>();
                }
            }

        /// <summary>
        /// Ranks instructors by total unique student count across all their courses.
        /// Also shows number of courses taught and average rating.
        /// </summary>
        public void GetInstructorRankings()
            {
            _log.Info("AnalyticsService", "GetInstructorRankings called.");
            try
                {
                using var ctx = new SmartLearnDbContext();
                var rankings = ctx.Users
                    .AsNoTracking()
                    .Include(u => u.TaughtCourses).ThenInclude(c => c.Enrollments)
                    .Include(u => u.TaughtCourses).ThenInclude(c => c.Ratings)
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
                    Console.WriteLine($"  {rank++,-6}{r.Username,-24}{r.CourseCount,-10}{r.UniqueStudents,-12}" +
                                      $"{(r.AvgRating > 0 ? $"{r.AvgRating:F2} ★" : "N/A")}");
                }
            catch (Exception ex)
                {
                _log.Error("AnalyticsService", $"GetInstructorRankings failed: {ex.Message}", null, ex);
                Console.WriteLine($"  ✗ Error: {ex.Message}");
                }
            }

        // ══════════════════════════════════════════════════════════════
        //  PART 4 TASK 3: RISK ANALYSIS
        // ══════════════════════════════════════════════════════════════

        /// <summary>
        /// Returns students with active enrollments where progress &lt; 30%
        /// and they enrolled more than 2 weeks ago.
        /// </summary>
        public List<AtRiskStudent> GetStudentsAtRisk()
            {
            var twoWeeksAgo = DateTime.Now.AddDays(-14);
            _log.Info("AnalyticsService", "GetStudentsAtRisk called.");
            try
                {
                using var ctx = new SmartLearnDbContext();
                return ctx.Enrollments
                    .AsNoTracking()
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
                }
            catch (Exception ex)
                {
                _log.Error("AnalyticsService", $"GetStudentsAtRisk failed: {ex.Message}", null, ex);
                return new List<AtRiskStudent>();
                }
            }

        /// <summary>
        /// For each course that has enrollments, calculates the completion rate
        /// (completed / total * 100). Ordered ascending to surface problem courses.
        /// </summary>
        public List<CourseCompletionRate> GetCourseCompletionRates()
            {
            _log.Info("AnalyticsService", "GetCourseCompletionRates called.");
            try
                {
                using var ctx = new SmartLearnDbContext();
                return ctx.Courses
                    .AsNoTracking()
                    .Include(c => c.Enrollments)
                    .AsEnumerable()
                    .Where(c => c.Enrollments.Any())
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
            catch (Exception ex)
                {
                _log.Error("AnalyticsService", $"GetCourseCompletionRates failed: {ex.Message}", null, ex);
                return new List<CourseCompletionRate>();
                }
            }

        // ── Display helpers ────────────────────────────────────────────

        /// <summary>Prints a ranked top-performers table.</summary>
        public void DisplayTopPerformers(List<TopPerformer> performers)
            {
            if (!performers.Any()) { Console.WriteLine("  No data."); return; }
            Console.WriteLine($"\n  {"Rank",-6}{"Student",-24}{"Avg Progress",-14}{"Completed",-12}Total");
            Console.WriteLine("  " + new string('─', 62));
            int rank = 1;
            foreach (var p in performers)
                Console.WriteLine($"  {rank++,-6}{p.Username,-24}{p.AvgProgress + "%",-14}" +
                                  $"{p.CompletedCourses,-12}{p.TotalCourses}");
            }

        /// <summary>Prints a category statistics table.</summary>
        public void DisplayCategoryPopularity(List<CategoryStats> stats)
            {
            if (!stats.Any()) { Console.WriteLine("  No data."); return; }
            Console.WriteLine($"\n  {"Category",-22}{"Courses",-10}{"Enrolled",-10}{"Avg Prog",-10}Avg Rating");
            Console.WriteLine("  " + new string('─', 65));
            foreach (var s in stats)
                Console.WriteLine($"  {s.Category,-22}{s.CourseCount,-10}{s.TotalEnrollments,-10}" +
                                  $"{s.AvgProgress + "%",-10}{(s.AvgRating > 0 ? $"{s.AvgRating:F2} ★" : "N/A")}");
            }

        /// <summary>Prints the at-risk student list with per-course detail.</summary>
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

        /// <summary>Prints course completion rates ordered by percentage.</summary>
        public void DisplayCourseCompletionRates(List<CourseCompletionRate> rates)
            {
            if (!rates.Any()) { Console.WriteLine("  No data."); return; }
            Console.WriteLine($"\n  {"Course",-36}{"Total",-8}{"Completed",-12}Completion %");
            Console.WriteLine("  " + new string('─', 65));
            foreach (var r in rates)
                {
                string title = r.CourseTitle.Length > 34
                    ? r.CourseTitle[..31] + "..." : r.CourseTitle;
                Console.WriteLine($"  {title,-36}{r.TotalEnrollments,-8}" +
                                  $"{r.CompletedEnrollments,-12}{r.CompletionPercentage:F1}%");
                }
            }

        /// <summary>Prints a monthly enrollment trend table.</summary>
        public void DisplayEnrollmentTrends(
            Dictionary<string, (int NewEnrollments, int UniqueStudents)> trends)
            {
            if (!trends.Any()) { Console.WriteLine("  No enrollment data."); return; }
            Console.WriteLine($"\n  {"Month",-12}{"New Enrollments",-18}Unique Students");
            Console.WriteLine("  " + new string('─', 44));
            foreach (var kv in trends)
                Console.WriteLine($"  {kv.Key,-12}{kv.Value.NewEnrollments,-18}{kv.Value.UniqueStudents}");
            }
        }
    }