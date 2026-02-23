
using System.Text.Json;
using Week_1;
using static System.Net.WebRequestMethods;

// ─── Shared Data (ONE place, used everywhere) ─────────────────────────────────
List<Student> students = Student.GetAllStudents();
List<Course> courses = Course.GetAllCourses();

// ─── LINQ Demo Functions ──────────────────────────────────────────────────────

void Demo_GroupBy()
{
    Console.WriteLine("========== GROUP BY DEMO ==========\n");

    var studentsByPerformance = students
        .GroupBy(s =>
        {
            if (s.ProgressPercentage >= 80) return "High Performer";
            if (s.ProgressPercentage >= 50) return "Medium Performer";
            return "Needs Support";
        })
        .ToList();

    Console.WriteLine("📊 STUDENT PERFORMANCE REPORT\n");
    foreach (var group in studentsByPerformance)
    {
        Console.WriteLine($"{group.Key}: {group.Count()} students");
        foreach (var student in group)
            Console.WriteLine($"  - {student.Username}: {student.ProgressPercentage}%");
        Console.WriteLine();
    }

    var coursesByCategory = courses.GroupBy(c => c.Category).ToList();

    Console.WriteLine("📚 COURSES BY CATEGORY\n");
    foreach (var categoryGroup in coursesByCategory)
    {
        Console.WriteLine($"{categoryGroup.Key} ({categoryGroup.Count()} courses):");
        foreach (var course in categoryGroup)
            Console.WriteLine($"  - {course.Title}");
        Console.WriteLine();
    }
}

void Demo_Aggregates()
{
    Console.WriteLine("========== AGGREGATES DEMO ==========\n");

    if (students.Count == 0 || courses.Count == 0)
    {
        Console.WriteLine("⚠️  No data available for analytics.\n");
        return;
    }

    int totalStudents = students.Count;
    int totalCourses = courses.Count;
    double averageProgress = students.Average(s => s.ProgressPercentage);
    int totalEnrollments = courses.Sum(c => c.CurrentEnrollments);
    double highestProgress = students.Max(s => s.ProgressPercentage);
    double lowestProgress = students.Min(s => s.ProgressPercentage);

    Course mostPopular = courses
        .OrderByDescending(c => c.CurrentEnrollments)
        .First();

    Console.WriteLine("🎛️  SMARTLEARN DASHBOARD");
    Console.WriteLine($"Total Students:      {totalStudents}");
    Console.WriteLine($"Total Courses:       {totalCourses}");
    Console.WriteLine($"Average Progress:    {averageProgress:F1}%");
    Console.WriteLine($"Total Enrollments:   {totalEnrollments}");
    Console.WriteLine($"Highest Progress:    {highestProgress}%");
    Console.WriteLine($"Lowest Progress:     {lowestProgress}%");
    Console.WriteLine($"Most Popular Course: {mostPopular.Title} ({mostPopular.CurrentEnrollments} students)\n");
}

void Demo_Pagination()
{
    Console.WriteLine("========== PAGINATION DEMO ==========\n");

    if (courses.Count == 0)
    {
        Console.WriteLine("⚠️  No courses available.\n");
        return;
    }

    int pageSize = 3;
    int pageNumber = 1;

    var coursesForPage = courses
        .OrderBy(c => c.Title)
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToList();

    int totalPages = (int)Math.Ceiling(courses.Count / (double)pageSize);

    Console.WriteLine($"📄 Page {pageNumber} of {totalPages}\n");
    foreach (var course in coursesForPage)
        Console.WriteLine($"- {course.Title}");

    Console.WriteLine($"\nShowing {coursesForPage.Count} of {courses.Count} courses\n");
}

void Demo_DistinctAndSelectMany()
{
    Console.WriteLine("========== DISTINCT & SELECTMANY DEMO ==========\n");

    if (courses.Count == 0 || students.Count == 0)
    {
        Console.WriteLine("⚠️  No data available.\n");
        return;
    }

    var categories = courses
        .Select(c => c.Category)
        .Distinct()
        .OrderBy(cat => cat)
        .ToList();

    Console.WriteLine("🏷️  ALL COURSE CATEGORIES:");
    foreach (var category in categories)
        Console.WriteLine($"- {category}");

    var allEnrolledCourseIds = students
        .SelectMany(s => s.EnrolledCourseIds)
        .Distinct()
        .ToList();

    Console.WriteLine($"\n✅ Total unique courses with enrollments: {allEnrolledCourseIds.Count}\n");
}

// ─── File Persistence Functions ───────────────────────────────────────────────
string projectFolder = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\"));
string dataFolder = Path.Combine(projectFolder, "Data");
void SaveStudents()
{
    try
    {
        string filePath = Path.Combine(dataFolder, "students.json");
        string json = JsonSerializer.Serialize(students, new JsonSerializerOptions { WriteIndented = true });
        System.IO.File.WriteAllText(filePath, json);
        Console.WriteLine($"✅ Saved {students.Count} students to {filePath}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error saving students: {ex.Message}");
    }
}

void SaveCourses()
{
    try
    {
        string filePath = Path.Combine(dataFolder, "courses.json");
        string json = JsonSerializer.Serialize(courses, new JsonSerializerOptions { WriteIndented = true });
        System.IO.File.WriteAllText(filePath, json);
        Console.WriteLine($"✅ Saved {courses.Count} courses to {filePath}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error saving courses: {ex.Message}");
    }
}

void SaveAllData()
{
    Directory.CreateDirectory("Data");
    Console.WriteLine("💾 Saving all data...");
    SaveStudents();
    SaveCourses();
    Console.WriteLine("✅ All data saved!\n");
}

void LoadStudents()
{
    try
    {
        string filePath = Path.Combine(dataFolder, "students.json");

        if (!System.IO.File.Exists(filePath))
        {
            Console.WriteLine("ℹ️  No students file found. Starting with default data.");
            return;
        }

        string json = System.IO.File.ReadAllText(filePath);
        var loaded = JsonSerializer.Deserialize<List<Student>>(json)!;

        if (loaded.Count == 0)
        {
            Console.WriteLine("ℹ️  Students file is empty. Starting with default data.");
            students = Student.GetAllStudents();
            return;
        }

        students = loaded;
        Console.WriteLine($"✅ Loaded {students.Count} students from {filePath}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error loading students: {ex.Message}");
        students = Student.GetAllStudents();
    }
}

void LoadCourses()
{
    try
    {
        string filePath = Path.Combine(dataFolder, "courses.json");

        if (!System.IO.File.Exists(filePath))
        {
            Console.WriteLine("ℹ️  No courses file found. Starting with default data.");
            return;
        }

        string json = System.IO.File.ReadAllText(filePath);
        var loaded = JsonSerializer.Deserialize<List<Course>>(json)!;

        if (loaded.Count == 0)
        {
            Console.WriteLine("ℹ️  Courses file is empty. Starting with default data.");
            courses = Course.GetAllCourses();
            return;
        }

        courses = loaded;
        Console.WriteLine($"✅ Loaded {courses.Count} courses from {filePath}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error loading courses: {ex.Message}");
        courses = Course.GetAllCourses();
    }
}

void LoadAllData()
{
    Console.WriteLine("📂 Loading saved data...\n");
    LoadStudents();
    LoadCourses();
    Console.WriteLine("\n✅ All data loaded!\n");
}

// ─── Main Flow ────────────────────────────────────────────────────────────────

LoadAllData();

Demo_GroupBy();
Demo_Aggregates();
Demo_Pagination();
Demo_DistinctAndSelectMany();

SaveAllData();

