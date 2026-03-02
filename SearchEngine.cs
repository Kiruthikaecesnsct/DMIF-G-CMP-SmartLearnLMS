using System;
using System.Collections.Generic;
using System.Linq;

namespace Week_1
{
    public static class SearchEngine
    {
        // ── Part 2 Step 1: Search Courses ──
        public static List<Course> SearchCourses(List<Course> courses, string keyword)
        {
            return courses
                .Where(c => c.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                            c.Description.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                .OrderBy(c => c.Title)
                .ToList();
        }

        // ── Part 2 Step 2: Search Students ──
        public static List<Student> SearchStudents(List<User> users, string keyword)
        {
            return users
                .OfType<Student>()
                .Where(s => s.Username.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                            (s.Email ?? "").Contains(keyword, StringComparison.OrdinalIgnoreCase))
                .OrderBy(s => s.Username)
                .ToList();
        }

        // ── Part 2 Step 3: Filter by Category (sorted by enrollment, highest first) ──
        public static List<Course> FilterCoursesByCategory(List<Course> courses, string category)
        {
            return courses
                .Where(c => c.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(c => c.CurrentEnrollments)
                .ToList();
        }

        // ── Part 2 Step 4: Search Instructors ──
        public static List<Instructor> SearchInstructors(List<User> users, string keyword)
        {
            return users
                .OfType<Instructor>()
                .Where(i => i.Username.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                            (i.Email ?? "").Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                            (i.Department ?? "").Contains(keyword, StringComparison.OrdinalIgnoreCase))
                .OrderBy(i => i.Username)
                .ToList();
        }

        // ── Universal ISearchable Search ──
        public static List<ISearchable> Search(List<ISearchable> items, string keyword)
        {
            return items.Where(item => item.MatchesSearch(keyword)).ToList();
        }

        // ── Part 4: Display Search Results (Course table) ──
        public static void DisplaySearchResults(List<Course> results)
        {
            if (results.Count == 0)
            {
                Console.WriteLine("  No courses found.");
                return;
            }
            Console.WriteLine($"╔════════════════════════════════════════════════════╗");
            Console.WriteLine($"║  {results.Count} Course(s) Found                              ║");
            Console.WriteLine($"╠══════╦══════════════════════════════╦══════════════╣");
            Console.WriteLine($"║  ID  ║  Title                       ║  Category    ║");
            Console.WriteLine($"╠══════╬══════════════════════════════╬══════════════╣");
            foreach (Course c in results)
            {
                string title = c.Title.Length > 28 ? c.Title.Substring(0, 25) + "..." : c.Title.PadRight(28);
                string cat = (c.Category ?? "").Length > 12 ? c.Category.Substring(0, 9) + "..." : (c.Category ?? "").PadRight(12);
                Console.WriteLine($"║ {c.CourseId,4} ║ {title} ║ {cat} ║");
            }
            Console.WriteLine($"╚══════╩══════════════════════════════╩══════════════╝");
        }

        // ── Part 4: Display Student List ──
        public static void DisplayStudentList(List<Student> students)
        {
            if (students.Count == 0)
            {
                Console.WriteLine("  No students found.");
                return;
            }
            Console.WriteLine($"╔════════════════════════════════════════════════╗");
            Console.WriteLine($"║  {students.Count} Student(s) Found                          ║");
            Console.WriteLine($"╠══════════════════╦═══════════════════╦═════════╣");
            Console.WriteLine($"║  Username         ║  Email            ║ Courses ║");
            Console.WriteLine($"╠══════════════════╬═══════════════════╬═════════╣");
            foreach (Student s in students)
            {
                string uname = s.Username.PadRight(17);
                string email = (s.Email ?? "").Length > 17 ? s.Email.Substring(0, 14) + "..." : (s.Email ?? "").PadRight(17);
                Console.WriteLine($"║ {uname} ║ {email} ║ {s.EnrolledCourseIds.Count,7} ║");
            }
            Console.WriteLine($"╚══════════════════╩═══════════════════╩═════════╝");
        }

        // ── ISearchable display ──
        public static void DisplayResults(List<ISearchable> results)
        {
            if (results.Count == 0)
            {
                Console.WriteLine("  No results found.");
                return;
            }
            Console.WriteLine($"╔════════════════════════════════╗");
            Console.WriteLine($"║  Found {results.Count} result(s)                ║");
            Console.WriteLine($"╚════════════════════════════════╝");
            foreach (ISearchable item in results)
                Console.WriteLine($"  • {item.GetSearchSummary()}");
        }
    }
}