
using System.Diagnostics.Metrics;
using System.Text.Json;
using Week_1;
using Week_1.Interfaces;
using Week_1.Services;
using static System.Net.WebRequestMethods;
using System.Data.SqlClient;
using Week_1.Data;


// The connection string — tells your C# code exactly WHERE to find the database
// Server = your SQL Server instance name (check SSMS for yours)
// Database = which database to use
// Trusted_Connection = use your Windows login (no username/password needed)
string connectionString = @"Server=localhost\SQLEXPRESS;
                            Database=SmartLearn;
                            Trusted_Connection=True;";





//// ─── Shared Data (ONE place, used everywhere) ─────────────────────────────────
//List<Student> students = Student.GetAllStudents();
//List<Course> courses = Course.GetAllCourses();


////SESSION-5//
////====================================//
//var enrollmentService = new EnrollmentService();
//var notificationService = new NotificationService();
//var searchService = new SearchService();

//// Enroll alice in C# Basics — also auto-notifies her
//var alice = students.First(s => s.Username == "alice");
//var csharpCourse = courses.First(c => c.Title == "C# Basics");
//enrollmentService.EnrollStudent(alice, csharpCourse);

//// Add a rating to C# Basics
//csharpCourse.AddRating(5, "Amazing course, really clear explanations!");
//csharpCourse.AddRating(4, "Good content, pace was a bit fast.");
//Console.WriteLine($"\nC# Basics average rating: {csharpCourse.AverageRating:F1}⭐");

//// Search courses by keyword
//var programmingCourses = searchService.Search(courses, "programming");
//Console.WriteLine($"\nFound {programmingCourses.Count} courses matching 'programming':");
//foreach (var c in programmingCourses)
//    Console.WriteLine($"  - {c.GetSearchSummary()}");

//// Notify all students at once
//var notifiableStudents = students.Cast<INotifiable>().ToList();
//notificationService.NotifyAll(notifiableStudents, "New courses added this week!");

//// Check alice's notifications
//Console.WriteLine($"\nAlice has {alice.UnreadNotificationCount} unread notifications:");
//foreach (var note in alice.GetNotificationHistory())
//    Console.WriteLine($"  {note}");

//// Generate a report for alice
//Console.WriteLine($"\n{alice.GenerateReport()}");


//================== SOLID Principles= One class, one job. Show the before/after using SmartLearn.========================//

//S — Single Responsibility Principle//

//BEFORE — violates SRP

//public class Student : User
//    {
//    public void EnrollInCourse(int courseId) { }  // ✅ Student concern

//    public void SaveToDatabase() { }              // ❌ Database concern
//    public void SendWelcomeEmail() { }            // ❌ Email concern
//    public void GenerateProgressReport() { }      // ❌ Reporting concern
//    }


//AFTER — follows SRP

//// Student handles student behaviour only
//public class Student : User, ISearchable, INotifiable, IReportable
//    {
//    public void EnrollInCourse(int courseId) { }
//    }

//// Separate class for file/database work
//public class StudentRepository
//    {
//    public void SaveToFile(List<Student> students) { }
//    public List<Student> LoadFromFile() { }
//    }

//// Separate service for notifications
//public class NotificationService
//    {
//    public void NotifyEnrollment(INotifiable user, string courseName) { }
//    }

//===========================================================================================================================//

//I — Interface Segregation Principle==Do not force classes to implement methods they will never use.//

//BAD — fat interface


//public interface IUser
//    {
//    void Login();
//    void Logout();
//    void TeachCourse();    // Students don't teach!
//    void EnrollInCourse(); // Instructors don't enroll!
//    void ManageSystem();   // Only admins do this!
//    }


//GOOD — small, focused interfaces


//public interface IAuthenticatable { void Login(); void Logout(); }
//public interface ITeacher { void TeachCourse(); }
//public interface ILearner { void EnrollInCourse(); }
//public interface IAdministrator { void ManageSystem(); }

//// Each class only takes what makes sense
//public class Student : User, IAuthenticatable, ILearner, INotifiable, ISearchable { }
//public class Instructor : User, IAuthenticatable, ITeacher, INotifiable { }
//public class Admin : User, IAuthenticatable, IAdministrator { }


//===========================================================================================================================//

//D — Dependency Inversion Principle==Depend on abstractions (interfaces), not concrete classes. This is what lets you swap JSON → SQL Server next week without breaking anything.//

//BEFORE — locked to JSON forever

//public class DataPersistenceService
//    {
//    // Hard-coded! Can never switch without rewriting this entire class
//    private JsonStorage storage = new JsonStorage();

//    public void SaveAllData(List<Student> students)
//        {
//        storage.SaveStudents(students); // Only works with JSON
//        }
//    }


//AFTER — swap storage with one line change


//// The interface — any storage can implement this
//public interface IStorage
//    {
//    void SaveStudents(List<Student> students);
//    List<Student> LoadStudents();
//    }

//// What you have now
//public class JsonStorage : IStorage
//    {
//    public void SaveStudents(List<Student> students) { /* writes .json */ }
//    public List<Student> LoadStudents() { /* reads .json */ }
//    }

//// What you'll build next week
//public class SqlStorage : IStorage
//    {
//    public void SaveStudents(List<Student> students) { /* writes to SQL Server */ }
//    public List<Student> LoadStudents() { /* reads from SQL Server */ }
//    }

//// Service depends on the INTERFACE — doesn't care which one
//public class DataPersistenceService
//    {
//    private IStorage storage;

//    public DataPersistenceService(IStorage storage)
//        {
//        this.storage = storage; // Inject whichever you want
//        }

//    public void SaveAllData(List<Student> students)
//        {
//        storage.SaveStudents(students); // Works with JSON or SQL!
//        }
//    }

//// In Program.cs — switch from JSON to SQL with ONE line:
//IStorage storage = new JsonStorage();    // Today
//// IStorage storage = new SqlStorage(); // Next week — nothing else changes!

//var persistenceService = new DataPersistenceService(storage);

//===========================================================================================================================//


//SESSION-6 - ADO.NET & Entity Framework Core - Connecting C# to SQL Server//

//Add package ----dotnet add package System.Data.SqlClient

//check on AdNetDemo.cs for code

//var adoDemo = new AdoNetDemo();
//adoDemo.GetAllStudentsFromDatabase();
//adoDemo.AddStudentToDatabase("newstudent", "new@email.com", "pass123");



//========================================================================//



// ── Seed data is now in the database via migrations ──────────────────────────
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