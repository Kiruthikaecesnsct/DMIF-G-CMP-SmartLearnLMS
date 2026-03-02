using System;
using System.Collections.Generic;
using System.Linq;

namespace Week_1
{
    public static class Analytics
    {
        // ══════════════════════════════════════════════════════
        //  PART 3 STEP 1: Student Analytics
        // ══════════════════════════════════════════════════════

        public static List<Student> GetTopStudents(List<User> users, int count)
        {
            return users
                .OfType<Student>()
                .Where(s => s.CourseProgress.Count > 0)
                .OrderByDescending(s => s.CourseProgress.Values.Average())
                .Take(count)
                .ToList();
        }

        public static List<Student> GetActiveStudents(List<User> users)
        {
            return users
                .OfType<Student>()
                .Where(s => s.EnrolledCourseIds.Count > 0)
                .ToList();
        }

        // Returns dictionary: "Low"/"Medium"/"High" → List<Student>
        public static Dictionary<string, List<Student>> GetStudentsByPerformance(List<User> users)
        {
            var students = users.OfType<Student>().ToList();

            var high = students.Where(s => s.CourseProgress.Count > 0 && s.CourseProgress.Values.Average() >= 80).ToList();
            var medium = students.Where(s => s.CourseProgress.Count > 0 && s.CourseProgress.Values.Average() >= 50 && s.CourseProgress.Values.Average() < 80).ToList();
            var low = students.Where(s => s.CourseProgress.Count == 0 || s.CourseProgress.Values.Average() < 50).ToList();

            return new Dictionary<string, List<Student>>
            {
                { "High",   high   },
                { "Medium", medium },
                { "Low",    low    }
            };
        }

        public static double CalculateSystemAverageProgress(List<User> users)
        {
            var students = users.OfType<Student>()
                                .Where(s => s.CourseProgress.Count > 0)
                                .ToList();
            if (students.Count == 0) return 0;
            return students.Average(s => s.CourseProgress.Values.Average());
        }

        // ══════════════════════════════════════════════════════
        //  PART 3 STEP 2: Course Analytics
        // ══════════════════════════════════════════════════════

        public static List<Course> GetPopularCourses(List<Course> courses, int count)
        {
            return courses
                .OrderByDescending(c => c.CurrentEnrollments)
                .Take(count)
                .ToList();
        }

        public static List<Course> GetCoursesNeedingStudents(List<Course> courses)
        {
            return courses
                .Where(c => c.CurrentEnrollments < 5)
                .OrderBy(c => c.CurrentEnrollments)
                .ToList();
        }

        public static List<Course> GetHighestRatedCourses(List<Course> courses, int count)
        {
            return courses
                .Where(c => c.GetTotalRatings() > 0)
                .OrderByDescending(c => c.GetAverageRating())
                .Take(count)
                .ToList();
        }

        public static List<Course> GetCoursesByInstructor(List<Course> courses, string instructorName)
        {
            return courses
                .Where(c => c.InstructorName.Equals(instructorName, StringComparison.OrdinalIgnoreCase))
                .OrderBy(c => c.Title)
                .ToList();
        }

        // ══════════════════════════════════════════════════════
        //  PART 3 STEP 3: System-Wide Analytics
        // ══════════════════════════════════════════════════════

        public static List<Course> GetCoursesByCategory(List<Course> courses, string category)
        {
            return courses
                .Where(c => c.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(c => c.GetAverageRating())
                .ToList();
        }

        public static string GetMostEnrolledCategory(List<Course> courses)
        {
            if (courses.Count == 0) return "N/A";
            return courses
                .GroupBy(c => c.Category)
                .OrderByDescending(g => g.Sum(c => c.CurrentEnrollments))
                .Select(g => g.Key)
                .FirstOrDefault() ?? "N/A";
        }

        public static bool HasStudentCompletedAny(List<User> users)
        {
            return users
                .OfType<Student>()
                .Any(s => s.CourseProgress.Values.Any(progress => progress >= 100));
        }

        public static bool AreAllCoursesFilled(List<Course> courses)
        {
            var limited = courses
                .Where(c => c is InPersonCourse || c is HybridCourse)
                .ToList();
            if (limited.Count == 0) return false;
            return limited.All(c => !c.CanEnroll(null));
        }

        // ══════════════════════════════════════════════════════
        //  PART 4: Display Methods
        // ══════════════════════════════════════════════════════

        public static void DisplayAnalytics(List<User> users, List<Course> courses)
        {
            var students = users.OfType<Student>().ToList();
            var instructors = users.OfType<Instructor>().ToList();
            var admins = users.OfType<Admin>().ToList();

            int activeCount = students.Count(s => s.EnrolledCourseIds.Count > 0);
            double avgProg = CalculateSystemAverageProgress(users);
            string topCat = GetMostEnrolledCategory(courses);
            bool anyComplete = HasStudentCompletedAny(users);
            bool allFull = AreAllCoursesFilled(courses);

            Console.WriteLine("╔════════════════════════════════════╗");
            Console.WriteLine("║       SMARTLEARN ANALYTICS         ║");
            Console.WriteLine("╠════════════════════════════════════╣");
            Console.WriteLine($"║  Total Users        : {users.Count,-13}║");
            Console.WriteLine($"║  Students           : {students.Count,-13}║");
            Console.WriteLine($"║  Instructors        : {instructors.Count,-13}║");
            Console.WriteLine($"║  Admins             : {admins.Count,-13}║");
            Console.WriteLine($"║  Active Students    : {activeCount,-13}║");
            Console.WriteLine($"║  Total Courses      : {courses.Count,-13}║");
            Console.WriteLine($"║  Avg Progress       : {avgProg:F1}%-{"",-10}║");
            Console.WriteLine($"║  Top Category       : {topCat,-13}║");
            Console.WriteLine($"║  Any Completions    : {(anyComplete ? "Yes" : "No"),-13}║");
            Console.WriteLine($"║  All Limited Full   : {(allFull ? "Yes" : "No"),-13}║");
            Console.WriteLine("╠════════════════════════════════════╣");
            Console.WriteLine("║  🏆 Top 3 Courses by Enrollment    ║");
            Console.WriteLine("╠════════════════════════════════════╣");
            var topCourses = GetPopularCourses(courses, 3);
            foreach (Course c in topCourses)
            {
                string line = $"  [{c.CourseId}] {c.Title} ({c.CurrentEnrollments})";
                Console.WriteLine($"║  {line,-34}║");
            }
            Console.WriteLine("╠════════════════════════════════════╣");
            Console.WriteLine("║  📊 Performance Groups             ║");
            Console.WriteLine("╠════════════════════════════════════╣");
            var groups = GetStudentsByPerformance(users);
            Console.WriteLine($"║  🟢 High  (80%+)    : {groups["High"].Count,-13}║");
            Console.WriteLine($"║  🟡 Medium(50-79%)  : {groups["Medium"].Count,-13}║");
            Console.WriteLine($"║  🔴 Low   (<50%)    : {groups["Low"].Count,-13}║");
            Console.WriteLine("╚════════════════════════════════════╝");
        }

        public static void DisplayCategoryBreakdown(List<Course> courses)
        {
            var categories = courses
                .GroupBy(c => c.Category)
                .OrderByDescending(g => g.Sum(c => c.CurrentEnrollments))
                .ToList();

            Console.WriteLine("╔════════════════════════════════════╗");
            Console.WriteLine("║       CATEGORY BREAKDOWN           ║");
            Console.WriteLine("╠══════════════════╦════════╦════════╣");
            Console.WriteLine("║  Category         ║ Courses║ Enroll ║");
            Console.WriteLine("╠══════════════════╬════════╬════════╣");
            foreach (var g in categories)
            {
                string cat = g.Key.Length > 17 ? g.Key.Substring(0, 14) + "..." : g.Key.PadRight(17);
                int totalEnroll = g.Sum(c => c.CurrentEnrollments);
                Console.WriteLine($"║ {cat} ║ {g.Count(),6} ║ {totalEnroll,6} ║");
            }
            Console.WriteLine("╚══════════════════╩════════╩════════╝");
        }

        // ── BONUS: Recommend courses ──
        public static List<Course> GetRecommendedCourses(List<Course> courses, Student student)
        {
            return courses
                .Where(c => !student.EnrolledCourseIds.Contains(c.CourseId))
                .Where(c => c.CanEnroll(student))
                .OrderByDescending(c => c.GetAverageRating())
                .Take(5)
                .ToList();
        }

        // ── BONUS: Pagination ──
        public static List<Course> GetCoursesPaged(List<Course> courses, int pageNumber, int pageSize)
        {
            return courses
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }
    }
}