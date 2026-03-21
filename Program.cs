
using System.Data.SqlClient;
using System.Diagnostics.Metrics;
using System.Text.Json;
using Week_1;
using Week_1.Data;
using Week_1.Interfaces;
using Week_1.Logging;
using Week_1.Services;
using Week_1.Validators;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Net.WebRequestMethods;


// The connection string — tells your C# code exactly WHERE to find the database
// Server = your SQL Server instance name (check SSMS for yours)
// Database = which database to use
// Trusted_Connection = use your Windows login (no username/password needed)
string connectionString = @"Server=localhost\SQLEXPRESS;
                            Database=SmartLearnDB;
                            Trusted_Connection=True;";







//===========================================================================================================================//


//SESSION-6 - ADO.NET & Entity Framework Core - Connecting C# to SQL Server//

//Add package ----dotnet add package System.Data.SqlClient

//check on AdNetDemo.cs for code

//var adoDemo = new AdoNetDemo();
//adoDemo.GetAllStudentsFromDatabase();
//adoDemo.AddStudentToDatabase("newstudent", "new@email.com", "pass123");



//========================================================================//



//// ── Seed data is now in the database via migrations ──────────────────────────
var studentService = new StudentService();
var courseService = new CourseService();
var enrollmentService = new EnrollmentService();

// Register a new student — goes straight to the database
// Wrap in try/catch in case testuser already exists from a previous run
Student newStudent;
try
    {
    newStudent = studentService.RegisterStudent("testuser", "test@email.com", "pass123");
    }
catch
    {
    // If already exists just log in instead
    newStudent = studentService.Login("testuser", "pass123");
    Console.WriteLine($"Student already exists, logged in as: {newStudent?.Username}");
    }

// Login — queries the database
var loggedIn = studentService.Login("testuser", "pass123");
Console.WriteLine($"Logged in as: {loggedIn?.Username}");

// Browse courses — now seeded so this will show 6 courses
var allCourses = courseService.GetAllCourses();
Console.WriteLine($"\nTotal courses in database: {allCourses.Count}");
foreach (var c in allCourses)
    Console.WriteLine($"  - {c.Title}");

// Search — runs WHERE clause in SQL Server
var results = courseService.SearchCourses("programming");
Console.WriteLine($"\nFound {results.Count} programming courses");

// Enroll — creates a row in the Enrollments table
// CourseId 1 = C# Basics (from seed data)
if (newStudent != null)
    {
    enrollmentService.EnrollStudent(newStudent.StudentId, 1);

    // Load student WITH their courses — uses .Include() now that [NotMapped] is removed
    studentService.ShowStudentWithCourses(newStudent.StudentId);

    // Update progress — UPDATE statement runs in the database
    studentService.UpdateProgress(newStudent.StudentId, 75);

    Console.WriteLine("\n✅ All database operations completed successfully!");
    Console.WriteLine("Close the program and run again — testuser will still be in the database!");
    }

// ── Week 7 Demo ───────────────────────────────────────────────────────────────
var instructorService = new InstructorService();
var analyticsService  = new AnalyticsService();

// Student dashboard — loads courses + instructor names via Include chain
if (newStudent != null)
    studentService.StudentDashboard(newStudent.StudentId);

// System analytics
analyticsService.GetSystemWideStatistics();
analyticsService.GetTopPerformers(5);
analyticsService.GetCategoryPopularity();

// Enroll a second course to demo drop / complete
enrollmentService.EnrollStudent(newStudent.StudentId, 2);
enrollmentService.MarkAsCompleted(1); // enrollmentId = 1 from earlier run

//-----------------------------------------------------------------------------
// ── Week 8 Demo ───────────────────────────────────────────────────────────────
Console.WriteLine("\n\n╔════════════════════════════════════╗");
Console.WriteLine("║         WEEK 8 - LIVE DEMO          ║");
Console.WriteLine("╚════════════════════════════════════╝");

// 1. VALIDATION DEMO — show InputValidator catching bad input
Console.WriteLine("\n--- INPUT VALIDATION ---");
Console.WriteLine(InputValidator.IsValidEmail("notanemail")
    ? "valid" : "❌ Bad email caught!");

InputValidator.IsValidUsername("a", out string usernameErr);
Console.WriteLine($"❌ Short username caught: {usernameErr}");

InputValidator.IsStrongPassword("weakpass", out string passErr);
Console.WriteLine($"❌ Weak password caught: {passErr}");

// 2. EXCEPTION DEMO — show custom exceptions in action
Console.WriteLine("\n--- CUSTOM EXCEPTIONS ---");
var enrollmentService2 = new EnrollmentService();
enrollmentService2.EnrollStudent(9999, 1);       // student doesn't exist
enrollmentService2.EnrollStudent(newStudent.StudentId, 9999); // course doesn't exist

// 3. LOGGER DEMO — show a log file being created
Console.WriteLine("\n--- LOGGER DEMO ---");
Logger.Info("Demo", "App started successfully");
Logger.Warning("Demo", "This is a warning example");
Logger.Error("Demo", "This is an error example");

// 4. AUDIT TRAIL DEMO
Console.WriteLine("\n--- AUDIT TRAIL ---");
using (var db = new SmartLearnDbContext())
    {
    AuditService.LogAction(db, "DemoAction", "Demo",
        newStudent?.Username ?? "testuser", newStudent?.StudentId,
        new { Note = "Week 8 demo audit entry" });

    var recentLogs = AuditService.GetRecentActivity(db, 5);
    Console.WriteLine($"✅ Audit trail has {recentLogs.Count} recent entries");
    }
//Add - Migration Week8AuditLog
// Remove the down (drop) lines from the migration file itself
//Update-Database