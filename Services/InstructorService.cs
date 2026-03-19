using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Week_1.Data;

namespace Week_1.Services
    {
    public class InstructorService
        {
        public void InstructorDashboard(int instructorId)
            {
            using (var db = new SmartLearnDbContext())
                {
                var instructor = db.Instructors
                    .Include(i => i.Courses)
                    .ThenInclude(c => c.Enrollments)
                    .ThenInclude(e => e.Student)
                    .FirstOrDefault(i => i.InstructorId == instructorId);

                if (instructor == null) return;

                Console.WriteLine("\n╔════════════════════════════════════╗");
                Console.WriteLine($"║  {instructor.Username}'s Teaching Dashboard");
                Console.WriteLine("╚════════════════════════════════════╝\n");

                Console.WriteLine($"📚 Total Courses: {instructor.Courses.Count}");
                Console.WriteLine($"👥 Total Students: {instructor.Courses.Sum(c => c.Enrollments.Count)}\n");

                foreach (var course in instructor.Courses)
                    {
                    Console.WriteLine($"📖 {course.Title} ({course.Category})");
                    Console.WriteLine($"   👥 Enrolled: {course.Enrollments.Count}/{course.MaxCapacity}");

                    if (course.Enrollments.Any())
                        {
                        var avgProgress = course.Enrollments.Average(e => e.ProgressPercent);
                        Console.WriteLine($"   📊 Class Average: {avgProgress:F1}%");

                        var topStudents = course.Enrollments
                            .OrderByDescending(e => e.ProgressPercent)
                            .Take(3)
                            .Select(e => $"{e.Student.Username} ({e.ProgressPercent}%)");

                        Console.WriteLine($"   ⭐ Top Students: {string.Join(", ", topStudents)}");
                        }
                    Console.WriteLine();
                    }
                }
            }

        public void GetCourseAnalytics(int courseId)
            {
            using (var db = new SmartLearnDbContext())
                {
                var course = db.Courses
                    .Include(c => c.Instructor)
                    .Include(c => c.Enrollments)
                    .ThenInclude(e => e.Student)
                    .FirstOrDefault(c => c.CourseId == courseId);

                if (course == null) return;

                Console.WriteLine($"\n📊 ANALYTICS: {course.Title}");
                Console.WriteLine("═══════════════════════════════════════\n");
                Console.WriteLine($"👨‍🏫 Instructor: {course.Instructor?.Username ?? "TBA"}");
                Console.WriteLine($"📚 Category: {course.Category}");
                Console.WriteLine($"🎯 Difficulty: {course.DifficultyLevel}");
                Console.WriteLine($"👥 Capacity: {course.Enrollments.Count}/{course.MaxCapacity}\n");

                if (course.Enrollments.Any())
                    {
                    var avgProgress = course.Enrollments.Average(e => e.ProgressPercent);
                    var completionRate = (double)course.Enrollments.Count(e => e.Status == "Completed")
                                         / course.Enrollments.Count * 100;
                    var dropoutRate = (double)course.Enrollments.Count(e => e.Status == "Dropped")
                                         / course.Enrollments.Count * 100;

                    Console.WriteLine("📈 PERFORMANCE METRICS:");
                    Console.WriteLine($"   Average Progress:  {avgProgress:F1}%");
                    Console.WriteLine($"   Completion Rate:   {completionRate:F1}%");
                    Console.WriteLine($"   Dropout Rate:      {dropoutRate:F1}%\n");

                    var beginners = course.Enrollments.Count(e => e.ProgressPercent < 25);
                    var intermediate = course.Enrollments.Count(e => e.ProgressPercent >= 25 && e.ProgressPercent < 75);
                    var advanced = course.Enrollments.Count(e => e.ProgressPercent >= 75);

                    Console.WriteLine("📊 PROGRESS DISTRIBUTION:");
                    Console.WriteLine($"   🔴 Just Started (0–25%):    {beginners}");
                    Console.WriteLine($"   🟡 In Progress  (25–75%):   {intermediate}");
                    Console.WriteLine($"   🟢 Almost Done  (75–100%):  {advanced}");
                    }
                }
            }

        public void GetTopStudentsInCourse(int courseId, int topN = 5)
            {
            using (var db = new SmartLearnDbContext())
                {
                var course = db.Courses
                    .Include(c => c.Enrollments)
                    .ThenInclude(e => e.Student)
                    .FirstOrDefault(c => c.CourseId == courseId);

                if (course == null) return;

                Console.WriteLine($"\n⭐ Top {topN} Students in {course.Title}:");
                var top = course.Enrollments
                    .OrderByDescending(e => e.ProgressPercent)
                    .Take(topN);

                int rank = 1;
                foreach (var e in top)
                    {
                    Console.WriteLine($"{rank++}. {e.Student.Username} — {e.ProgressPercent}%");
                    }
                }
            }

        public void GetStudentsAtRisk()
            {
            using (var db = new SmartLearnDbContext())
                {
                var atRisk = db.Students
                    .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                    .Where(s => s.Enrollments.Any(e =>
                        e.Status == "Active" &&
                        e.ProgressPercent < 30 &&
                        (DateTime.Now - e.EnrolledDate).Days > 14))
                    .ToList();

                Console.WriteLine("\n⚠️  STUDENTS AT RISK:");
                foreach (var student in atRisk)
                    {
                    Console.WriteLine($"👤 {student.Username}");
                    foreach (var e in student.Enrollments.Where(e => e.Status == "Active" && e.ProgressPercent < 30))
                        {
                        Console.WriteLine($"   • {e.Course.Title} — {e.ProgressPercent}% in {(DateTime.Now - e.EnrolledDate).Days} days");
                        }
                    }
                Console.WriteLine($"\nTotal at risk: {atRisk.Count}");
                }
            }
        }
    }
