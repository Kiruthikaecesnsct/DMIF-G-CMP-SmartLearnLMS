using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Week_1;
using Week_1.Services;

// ─── Shared Data ─────────────────────────────────────────────────────────────
List<Student> students = Student.GetAllStudents();
List<Course> courses = Course.GetAllCourses();
List<User> users = new List<User>(students);

// Add some instructors and admins for analytics
users.Add(new Instructor("prof_smith", "pass", "smith@uni.edu") { Department = "Computer Science" });
users.Add(new Instructor("prof_johnson", "pass", "johnson@uni.edu") { Department = "Data Science" });
users.Add(new Admin("admin1", "adminpass", "admin@smartlearn.com"));

// ─── Dependency Injection: Create service instances once ──────────────────────
// We use interface types on the left to demonstrate DI principle.
// To swap an implementation, you only change these lines — nothing else.
EnrollmentService enrollmentService = new EnrollmentService();
NotificationService notificationService = new NotificationService();
SearchService searchService = new SearchService();
ReportService reportService = new ReportService();

// ─── Demo: EnrollmentService ──────────────────────────────────────────────────
Console.WriteLine("========== ENROLLMENT SERVICE DEMO ==========\n");

Student alice = students.First(s => s.Username == "alice");
Course csharpCourse = courses.First(c => c.CourseId == 101);

Console.WriteLine($"Checking availability for '{csharpCourse.Title}':");
enrollmentService.DisplayEnrollmentStatus(csharpCourse);
enrollmentService.EnrollStudent(csharpCourse, alice);

// ─── Demo: NotificationService ────────────────────────────────────────────────
Console.WriteLine("\n========== NOTIFICATION SERVICE DEMO ==========\n");

notificationService.NotifyEnrollment(alice, csharpCourse.Title);
notificationService.NotifyProgressMilestone(alice, alice.ProgressPercentage);

// Notify ALL students at once (demonstrates NotifyAll)
Console.WriteLine("\nBroadcast to all students:");
notificationService.NotifyAll(students, "📢 New courses available this semester!");

// Show history for alice
Console.WriteLine("\nAlice's notification history:");
notificationService.DisplayHistory(alice);

// ─── Demo: SearchService ──────────────────────────────────────────────────────
Console.WriteLine("\n========== SEARCH SERVICE DEMO ==========\n");

// Search courses by keyword
string keyword = "python";
List<Course> courseResults = searchService.Search(courses, keyword);
searchService.DisplayResults(courseResults, keyword);

// Search students by keyword — same service, different type (generic!)
string studentKeyword = "alice";
List<Student> studentResults = searchService.Search(students, studentKeyword);
searchService.DisplayResults(studentResults, studentKeyword);

// ─── Demo: ReportService ──────────────────────────────────────────────────────
Console.WriteLine("\n========== REPORT SERVICE DEMO ==========\n");

// Display report for a student
reportService.DisplayReport(alice);

// Display report for a course
reportService.DisplayReport(csharpCourse);

// Display all student reports
Console.WriteLine("\n--- All Student Reports ---");
reportService.DisplayAllReports(students.Take(2).Cast<IReportable>());

// ─── LINQ Demos ───────────────────────────────────────────────────────────────
Demo_GroupBy();
Demo_Aggregates();
Demo_Pagination();
Demo_DistinctAndSelectMany();

// ─── Analytics ────────────────────────────────────────────────────────────────
Console.WriteLine("\n========== ANALYTICS ==========\n");
Analytics.DisplayAnalytics(users, courses);
Analytics.DisplayCategoryBreakdown(courses);

// ─── File Persistence ─────────────────────────────────────────────────────────
string projectFolder = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\"));
string dataFolder = Path.Combine(projectFolder, "Data");
Directory.CreateDirectory(dataFolder);

SaveAllData();

// ── Local helper methods (same as before) ─────────────────────────────────────

void Demo_GroupBy()
    {
    Console.WriteLine("========== GROUP BY DEMO ==========\n");
    var grouped = students.GroupBy(s =>
    {
        if (s.ProgressPercentage >= 80) return "High Performer";
        if (s.ProgressPercentage >= 50) return "Medium Performer";
        return "Needs Support";
    });
    foreach (var g in grouped)
        {
        Console.WriteLine($"{g.Key}: {g.Count()} students");
        foreach (var s in g) Console.WriteLine($"  - {s.Username}: {s.ProgressPercentage}%");
        Console.WriteLine();
        }

    var byCategory = courses.GroupBy(c => c.Category);
    foreach (var g in byCategory)
        {
        Console.WriteLine($"{g.Key} ({g.Count()} courses):");
        foreach (var c in g) Console.WriteLine($"  - {c.Title}");
        Console.WriteLine();
        }
    }

void Demo_Aggregates()
    {
    Console.WriteLine("========== AGGREGATES DEMO ==========\n");
    if (!students.Any() || !courses.Any()) { Console.WriteLine("⚠️  No data.\n"); return; }
    Console.WriteLine($"Total Students   : {students.Count}");
    Console.WriteLine($"Total Courses    : {courses.Count}");
    Console.WriteLine($"Average Progress : {students.Average(s => s.ProgressPercentage):F1}%");
    Console.WriteLine($"Total Enrollments: {courses.Sum(c => c.CurrentEnrollments)}");
    Console.WriteLine($"Highest Progress : {students.Max(s => s.ProgressPercentage)}%");
    Console.WriteLine($"Lowest Progress  : {students.Min(s => s.ProgressPercentage)}%");
    Console.WriteLine($"Most Popular     : {courses.OrderByDescending(c => c.CurrentEnrollments).First().Title}\n");
    }

void Demo_Pagination()
    {
    Console.WriteLine("========== PAGINATION DEMO ==========\n");
    int pageSize = 3, page = 1;
    var paged = courses.OrderBy(c => c.Title).Skip((page - 1) * pageSize).Take(pageSize).ToList();
    int total = (int)Math.Ceiling(courses.Count / (double)pageSize);
    Console.WriteLine($"Page {page} of {total}:");
    paged.ForEach(c => Console.WriteLine($"  - {c.Title}"));
    Console.WriteLine();
    }

void Demo_DistinctAndSelectMany()
    {
    Console.WriteLine("========== DISTINCT & SELECTMANY DEMO ==========\n");
    var categories = courses.Select(c => c.Category).Distinct().OrderBy(x => x).ToList();
    Console.WriteLine("Categories:");
    categories.ForEach(cat => Console.WriteLine($"  - {cat}"));
    var allIds = students.SelectMany(s => s.EnrolledCourseIds).Distinct().ToList();
    Console.WriteLine($"\nUnique enrolled course IDs: {allIds.Count}\n");
    }

void SaveAllData()
    {
    try
        {
        string sf = Path.Combine(dataFolder, "students.json");
        string cf = Path.Combine(dataFolder, "courses.json");
        File.WriteAllText(sf, JsonSerializer.Serialize(students, new JsonSerializerOptions { WriteIndented = true }));
        File.WriteAllText(cf, JsonSerializer.Serialize(courses, new JsonSerializerOptions { WriteIndented = true }));
        Console.WriteLine($"✅ Saved {students.Count} students and {courses.Count} courses.");
        }
    catch (Exception ex) { Console.WriteLine($"❌ Save error: {ex.Message}"); }
    }