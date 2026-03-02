
using System.Text.Json;
using Week_1;
using static System.Net.WebRequestMethods;



// THE ARRAY MOVING NIGHTMARE-Code 1 — The Array Problem
//using Week_1;

//Course[] courses = new Course[10]; // Only 10 spaces!
//courses[0] = new Course { Title = "C# Basics" };
//courses[1] = new Course { Title = "OOP Magic" };

//// Need space for course #11? You have to "move houses"!
//Course[] biggerHouse = new Course[20];          // Get a bigger place
//Array.Copy(courses, biggerHouse, courses.Length); // Pack and move EVERYTHING
//courses = biggerHouse;                            // New address



//Code 2 — Array vs List Side-by-Side
//// NEW WAY - List<T> (magical)
//List<Course> courses = new List<Course>(); // No size needed!
//courses.Add(new Course { Title = "C# Basics" });
//courses.Add(new Course { Title = "OOP Mastery" });
//courses.Add(new Course { Title = "LINQ Wizardry" });
//courses.Add(new Course { Title = "Keep adding..." });
//courses.Add(new Course { Title = "...it never gets full!" });

//Console.WriteLine($"We have {courses.Count} courses!"); // IT counts for us!


//Code 3 — List<T> Superpowers (All Key Methods)
//List<Student> students = new List<Student>();

//// SUPERPOWER #1: Add to the end
//students.Add(new Student("alice", "pass123", "alice@email.com"));
//students.Add(new Student("bob", "pass123", "bob@email.com"));
//students.Add(new Student("charlie", "pass123", "charlie@email.com"));
//Console.WriteLine($"We have {students.Count} students");

//// SUPERPOWER #2: Insert at a specific position
//students.Insert(0, new Student("zara", "pass123", "zara@email.com"));
//Console.WriteLine($"First student is now: {students[0].Username}"); // zara!

//// SUPERPOWER #3: Find the FIRST match
//Student alice = students.Find(s => s.Username == "alice");
//Console.WriteLine($"Found: {alice?.Username}");

//// SUPERPOWER #4: Find ALL matches
//List<Student> active = students.FindAll(s => s.EnrolledCourseIds.Count > 0);
//Console.WriteLine($"Active students: {active.Count}");

//// SUPERPOWER #5: Check if something exists
//bool hasAlice = students.Contains(alice);
//Console.WriteLine($"Do we have Alice? {hasAlice}");

//// SUPERPOWER #6: Remove items
//students.RemoveAt(0); // Remove by index
//Console.WriteLine($"After removing first: {students.Count} students");

//// SUPERPOWER #7: Sort
//students.Sort((a, b) => a.Username.CompareTo(b.Username));
//Console.WriteLine("Sorted alphabetically!");
//foreach (var s in students)
//{
//    Console.WriteLine($"  - {s.Username}");
//}



////Code 4 — Dictionary Basics
//// Create a phonebook: Username (string) → Student object
//Dictionary<string, Student> studentBook = new Dictionary<string, Student>();

//// Add entries: key = username, value = Student object
//studentBook.Add("alice123", new Student("alice123", "pass", "alice@email.com"));
//studentBook.Add("bob456", new Student("bob456", "pass", "bob@email.com"));
//studentBook.Add("charlie789", new Student("charlie789", "pass", "charlie@email.com"));

//// INSTANT lookup — no looping needed!
//Student alice = studentBook["alice123"];
//Console.WriteLine($"Found instantly: {alice.Username}");

//// Check before looking up (avoids errors)
//if (studentBook.ContainsKey("zara999"))
//{
//    Student zara = studentBook["zara999"];
//}
//else
//{
//    Console.WriteLine("Zara not found");
//}

//// BEST PRACTICE: TryGetValue (safe lookup)
//if (studentBook.TryGetValue("bob456", out Student bob))
//{
//    Console.WriteLine($"Found Bob: {bob.Email}");
//}

//// Loop through all KEYS (usernames)
//foreach (string username in studentBook.Keys)
//{
//    Console.WriteLine($"Username: {username}");
//}

//// Loop through all VALUES (student objects)
//foreach (Student student in studentBook.Values)
//{
//    Console.WriteLine($"Student: {student.Username}");
//}

//// Loop through BOTH key and value
//foreach (KeyValuePair<string, Student> pair in studentBook)
//{
//    Console.WriteLine($"Key: {pair.Key} → Value: {pair.Value.Username}");
//}






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

