using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Week_1.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Week_1.Services
    {
    //public class StudentService
    //    {
    //    // READ — Get all students from database
    //    // Compare to ADO.NET: that was 20 lines. This is 4 lines. Same result.
    //    public List<Student> GetAllStudents()
    //        {
    //        using (var db = new SmartLearnDbContext())
    //            {
    //            // .ToList() tells EF Core: execute the query and give me all rows
    //            return db.Students.ToList();
    //            }
    //        }

    //    // CREATE — Register a new student
    //    public Student RegisterStudent(string username, string email, string password)
    //        {
    //        using (var db = new SmartLearnDbContext())
    //            {
    //            // Check if username already exists — Any() returns true/false
    //            if (db.Students.Any(s => s.Username == username))
    //                {
    //                throw new Exception("Username already exists!");
    //                }

    //            // Create the student object just like you always have
    //            var student = new Student
    //                {
    //                Username = username,
    //                Email = email,
    //                Password = password, // hash this in a real app!
    //                ProgressPercentage = 0
    //                };

    //            // Tell EF Core to track this new student
    //            db.Students.Add(student);

    //            // THIS is when the SQL INSERT actually happens
    //            // EF Core writes all the SQL for you — you never type INSERT INTO
    //            db.SaveChanges();

    //            Console.WriteLine($"✅ Student {username} added! Their new ID: {student.StudentId}");
    //            return student;
    //            }
    //        }

    //    // READ — Login (find by username + password)
    //    public Student Login(string username, string password)
    //        {
    //        using (var db = new SmartLearnDbContext())
    //            {
    //            // FirstOrDefault = return first match, or null if not found
    //            // This is LINQ working with EF Core — s => s.Username == username is the filter
    //            return db.Students
    //                .FirstOrDefault(s => s.Username == username && s.Password == password);
    //            }
    //        }

    //    // READ — Find one student by ID
    //    public Student FindStudentById(int id)
    //        {
    //        using (var db = new SmartLearnDbContext())
    //            {
    //            // .Find() is the fastest way to find by primary key
    //            return db.Students.Find(id);
    //            }
    //        }

    //    // UPDATE — Change a student's progress
    //    public void UpdateProgress(int studentId, double newProgress)
    //        {
    //        using (var db = new SmartLearnDbContext())
    //            {
    //            var student = db.Students.Find(studentId);
    //            if (student != null)
    //                {
    //                // Just change the property like normal C# — EF Core tracks this change
    //                student.ProgressPercentage = newProgress;

    //                // SaveChanges() detects the change and runs UPDATE SQL automatically
    //                db.SaveChanges();
    //                Console.WriteLine("✅ Progress updated!");
    //                }
    //            }
    //        }

    //    // DELETE — Remove a student
    //    public void DeleteStudent(int studentId)
    //        {
    //        using (var db = new SmartLearnDbContext())
    //            {
    //            var student = db.Students.Find(studentId);
    //            if (student != null)
    //                {
    //                // Tell EF Core to remove it
    //                db.Students.Remove(student);

    //                // SaveChanges() runs the DELETE SQL
    //                db.SaveChanges();
    //                Console.WriteLine("✅ Student deleted!");
    //                }
    //            }
    //        }

    //    //CONCEPT 7 — Loading Related Data with Include() .This is the most impressive part of EF Core.

    //    // Load a student AND all their courses in ONE database call
    //    // Without .Include(), student.Enrollments would be empty — EF Core is lazy by default
    //    public void ShowStudentWithCourses(int studentId)
    //        {
    //        using (var db = new SmartLearnDbContext())
    //            {
    //            var student = db.Students
    //                .Include(s => s.Enrollments)        // Load the enrollments list
    //                .ThenInclude(e => e.Course)          // For each enrollment, also load its course
    //                .FirstOrDefault(s => s.StudentId == studentId);

    //            if (student != null)
    //                {
    //                Console.WriteLine($"\n📚 {student.Username}'s Courses:");
    //                Console.WriteLine("─────────────────────────────────");

    //                foreach (var enrollment in student.Enrollments)
    //                    {
    //                    Console.WriteLine($"📖 {enrollment.Course.Title}");
    //                    Console.WriteLine($"   Progress: {enrollment.ProgressPercentage}%");
    //                    Console.WriteLine($"   Status: {enrollment.Status}");
    //                    }
    //                }
    //            }
    //        }

    //    // Load a course AND all its enrolled students
    //    public void ShowCourseWithStudents(int courseId)
    //        {
    //        using (var db = new SmartLearnDbContext())
    //            {
    //            var course = db.Courses
    //                .Include(c => c.Enrollments)
    //                .ThenInclude(e => e.Student)
    //                .FirstOrDefault(c => c.CourseId == courseId);

    //            if (course != null)
    //                {
    //                Console.WriteLine($"\n👥 Students in {course.Title}:");
    //                foreach (var enrollment in course.Enrollments)
    //                    {
    //                    Console.WriteLine($"  • {enrollment.Student.Username} — {enrollment.ProgressPercentage}% complete");
    //                    }
    //                }
    //            }
    //        }
    //    }


    public class StudentService
        {
        // READ — Get all students
        public List<Student> GetAllStudents()
            {
            using (var db = new SmartLearnDbContext())
                {
                return db.Students.ToList();
                }
            }

        // CREATE — Register new student
        public Student RegisterStudent(string username, string email, string password)
            {
            using (var db = new SmartLearnDbContext())
                {
                if (db.Students.Any(s => s.Username == username))
                    throw new Exception("Username already exists!");

                var student = new Student
                    {
                    Username = username,
                    Email = email,
                    Password = password,
                    ProgressPercentage = 0,
                    DateRegistered = DateTime.Now,
                    IsActive = true
                    };

                db.Students.Add(student);
                db.SaveChanges();

                Console.WriteLine($"✅ Student {username} registered! ID: {student.StudentId}");
                return student;
                }
            }

        // READ — Login
        public Student? Login(string username, string password)
            {
            using (var db = new SmartLearnDbContext())
                {
                return db.Students
                    .FirstOrDefault(s => s.Username == username && s.Password == password);
                }
            }

        // READ — Find by ID
        public Student? FindStudentById(int id)
            {
            using (var db = new SmartLearnDbContext())
                {
                return db.Students.Find(id);
                }
            }

        // UPDATE — Change progress
        public void UpdateProgress(int studentId, double newProgress)
            {
            using (var db = new SmartLearnDbContext())
                {
                var student = db.Students.Find(studentId);
                if (student != null)
                    {
                    student.ProgressPercentage = newProgress;
                    db.SaveChanges();
                    Console.WriteLine($"✅ Progress updated to {newProgress}%");
                    }
                }
            }

        // DELETE — Remove student
        public void DeleteStudent(int studentId)
            {
            using (var db = new SmartLearnDbContext())
                {
                var student = db.Students.Find(studentId);
                if (student != null)
                    {
                    db.Students.Remove(student);
                    db.SaveChanges();
                    Console.WriteLine("✅ Student deleted!");
                    }
                }
            }

        // READ with relationships — load student WITH their enrolled courses
        // .Include() and .ThenInclude() now work because [NotMapped] was removed
        public void ShowStudentWithCourses(int studentId)
            {
            using (var db = new SmartLearnDbContext())
                {
                // Load student → their Enrollments → each Enrollment's Course
                var student = db.Students
                    .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                    .FirstOrDefault(s => s.StudentId == studentId);

                if (student != null)
                    {
                    Console.WriteLine($"\n📚 {student.Username}'s Enrolled Courses:");
                    Console.WriteLine("─────────────────────────────────");

                    if (!student.Enrollments.Any())
                        {
                        Console.WriteLine("  No enrollments yet.");
                        return;
                        }

                    foreach (var enrollment in student.Enrollments)
                        {
                        Console.WriteLine($"📖 {enrollment.Course?.Title ?? "Unknown Course"}");
                        Console.WriteLine($"   Progress: {enrollment.ProgressPercentage}%");
                        Console.WriteLine($"   Status: {enrollment.Status}");
                        Console.WriteLine($"   Enrolled On: {enrollment.EnrollmentDate.ToShortDateString()}");
                        }
                    Console.WriteLine("─────────────────────────────────");
                    }
                else
                    {
                    Console.WriteLine("❌ Student not found.");
                    }
                }
            }
        }
    }
