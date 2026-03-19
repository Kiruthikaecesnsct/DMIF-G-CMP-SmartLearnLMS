using Microsoft.EntityFrameworkCore;
using Week_1.Data;

namespace Week_1.Services
    {
    public class AnalyticsService
        {
        public void GetSystemWideStatistics()
            {
            using (var db = new SmartLearnDbContext())
                {
                var totalStudents = db.Students.Count();
                var totalInstructors = db.Instructors.Count();
                var totalCourses = db.Courses.Count();
                var totalEnrollments = db.Enrollments.Count();
                var activeEnrollments = db.Enrollments.Count(e => e.Status == "Active");
                var completed = db.Enrollments.Count(e => e.Status == "Completed");
                var avgProgress = db.Enrollments.Any()
                                        ? db.Enrollments.Average(e => e.ProgressPercent)
                                        : 0;

                Console.WriteLine("\n╔════════════════════════════════════╗");
                Console.WriteLine("║     SMARTLEARN SYSTEM ANALYTICS     ║");
                Console.WriteLine("╚════════════════════════════════════╝\n");

                Console.WriteLine("👥 USER STATISTICS:");
                Console.WriteLine($"   Students:    {totalStudents}");
                Console.WriteLine($"   Instructors: {totalInstructors}\n");

                Console.WriteLine("📚 COURSE STATISTICS:");
                Console.WriteLine($"   Total Courses:      {totalCourses}");
                Console.WriteLine($"   Total Enrollments:  {totalEnrollments}");
                Console.WriteLine($"   Active:             {activeEnrollments}");
                Console.WriteLine($"   Completed:          {completed}\n");

                Console.WriteLine("📊 ENGAGEMENT:");
                Console.WriteLine($"   System-wide Avg Progress:         {avgProgress:F1}%");
                Console.WriteLine($"   Avg Enrollments per Student:      {(double)totalEnrollments / totalStudents:F1}");
                Console.WriteLine($"   Avg Students per Course:          {(double)totalEnrollments / totalCourses:F1}");
                }
            }

        public void GetTopPerformers(int topN = 10)
            {
            using (var db = new SmartLearnDbContext())
                {
                var top = db.Students
                    .Include(s => s.Enrollments)
                    .Where(s => s.Enrollments.Any())
                    .Select(s => new
                        {
                        Student = s,
                        AvgProgress = s.Enrollments.Average(e => e.ProgressPercent),
                        CompletedCount = s.Enrollments.Count(e => e.Status == "Completed"),
                        TotalEnrollments = s.Enrollments.Count
                        })
                    .OrderByDescending(x => x.AvgProgress)
                    .ThenByDescending(x => x.CompletedCount)
                    .Take(topN)
                    .ToList();

                Console.WriteLine($"\n🏆 TOP {topN} PERFORMING STUDENTS:");
                int rank = 1;
                foreach (var item in top)
                    {
                    Console.WriteLine($"{rank++}. {item.Student.Username}");
                    Console.WriteLine($"   📊 Avg Progress:  {item.AvgProgress:F1}%");
                    Console.WriteLine($"   ✅ Completed:     {item.CompletedCount}/{item.TotalEnrollments}\n");
                    }
                }
            }

        public void GetCategoryPopularity()
            {
            using (var db = new SmartLearnDbContext())
                {
                // Pull data into memory first — SQL Server can't handle
                // nested aggregates like SelectMany().Average() in one query
                var courses = db.Courses
                    .Include(c => c.Enrollments)
                    .ToList();

                var stats = courses
                    .GroupBy(c => c.Category)
                    .Select(g => new
                        {
                        Category = g.Key,
                        CourseCount = g.Count(),
                        TotalEnrollments = g.Sum(c => c.Enrollments.Count),
                        AvgProgress = g.SelectMany(c => c.Enrollments).Any()
                                           ? g.SelectMany(c => c.Enrollments)
                                               .Average(e => e.ProgressPercent)
                                           : 0
                        })
                    .OrderByDescending(x => x.TotalEnrollments)
                    .ToList();

                Console.WriteLine("\n📊 CATEGORY POPULARITY:");
                foreach (var s in stats)
                    {
                    Console.WriteLine($"📂 {s.Category}");
                    Console.WriteLine($"   Courses:           {s.CourseCount}");
                    Console.WriteLine($"   Total Enrollments: {s.TotalEnrollments}");
                    Console.WriteLine($"   Average Progress:  {s.AvgProgress:F1}%\n");
                    }
                }
            }
        }
    }