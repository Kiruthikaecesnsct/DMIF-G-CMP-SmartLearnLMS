using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Week_1;

namespace Week_1.Data
    {
    public class SmartLearnDbContext : DbContext
        {
        public DbSet<Student> Students { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
            optionsBuilder.UseSqlServer(@"Server=localhost\SQLEXPRESS;
                                          Database=SmartLearnDB;
                                          Trusted_Connection=True;
                                          TrustServerCertificate=True;");
            }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
            // Tell EF Core each class maps to its own separate table
            modelBuilder.Entity<Student>().ToTable("Students");
            modelBuilder.Entity<Instructor>().ToTable("Instructors");
            modelBuilder.Entity<Course>().ToTable("Courses");
            modelBuilder.Entity<Enrollment>().ToTable("Enrollments");

            // Seed data — this inserts default courses into DB automatically
            // when you run Update-Database
            modelBuilder.Entity<Course>().HasData(
                new Course { CourseId = 1, Title = "C# Basics", Description = "Introduction to C# programming", Category = "Programming", Difficulty = "Beginner", CurrentEnrollments = 0, MaxStudents = 200, InstructorId = 1 },
                new Course { CourseId = 2, Title = "Advanced LINQ", Description = "Deep dive into LINQ queries", Category = "Programming", Difficulty = "Advanced", CurrentEnrollments = 0, MaxStudents = 100, InstructorId = 1 },
                new Course { CourseId = 3, Title = "Web Design 101", Description = "HTML and CSS fundamentals", Category = "Design", Difficulty = "Beginner", CurrentEnrollments = 0, MaxStudents = 150, InstructorId = 1 },
                new Course { CourseId = 4, Title = "UI/UX Principles", Description = "User interface design principles", Category = "Design", Difficulty = "Intermediate", CurrentEnrollments = 0, MaxStudents = 80, InstructorId = 1 },
                new Course { CourseId = 5, Title = "Data Science Intro", Description = "Introduction to data science", Category = "Data", Difficulty = "Intermediate", CurrentEnrollments = 0, MaxStudents = 120, InstructorId = 1 },
                new Course { CourseId = 6, Title = "Machine Learning", Description = "ML algorithms and applications", Category = "Data", Difficulty = "Advanced", CurrentEnrollments = 0, MaxStudents = 60, InstructorId = 1 }
            );

            // Seed one default instructor so the InstructorId foreign key is satisfied
            modelBuilder.Entity<Instructor>().HasData(
                new Instructor
                    {
                    InstructorId = 1,
                    Username = "admin_instructor",
                    Password = "pass123",
                    Email = "instructor@smartlearn.com",
                    DateRegistered = new DateTime(2024, 1, 1),
                    IsActive = true
                    }
            );
            }
        }
    }



//using System;
//using System.Collections.Generic;
//using System.Data;
//using Microsoft.Data.SqlClient;   // Modern ADO.NET provider for .NET Core / .NET 5+
//using Week_1;                     // Your model classes (Student, Instructor, Course, Enrollment)

//namespace Week_1.Data
//    {
//    /// <summary>
//    /// ADO.NET Core version of the data access layer.
//    /// Replaces the EF Core DbContext.
//    /// No DbSets, no migrations, no OnModelCreating.
//    /// All operations use raw SQL via SqlConnection/SqlCommand.
//    /// Tables must be created manually (or via a separate SQL script).
//    /// </summary>
//    public class SmartLearnDbContext   // Kept the same class name for minimal code changes in your project
//        {
//        // Same connection string you were using in EF Core
//        private readonly string _connectionString =
//            @"Server=localhost\SQLEXPRESS;
//              Database=SmartLearnDB;
//              Trusted_Connection=True;
//              TrustServerCertificate=True;";

//        /// <summary>
//        /// Returns an open SqlConnection. Caller is responsible for disposing it.
//        /// </summary>
//        public SqlConnection GetConnection()
//            {
//            var connection = new SqlConnection(_connectionString);
//            connection.Open();
//            return connection;
//            }

//        /// <summary>
//        /// Seeds the default Instructor and Courses (same data you had in EF Core HasData).
//        /// Uses IF NOT EXISTS so it is safe to call multiple times.
//        /// </summary>
//        public void SeedData()
//            {
//            using var connection = new SqlConnection(_connectionString);
//            connection.Open();

//            // Seed Instructor (must exist before courses because of FK)
//            const string instructorSeed = @"
//                IF NOT EXISTS (SELECT 1 FROM Instructors WHERE InstructorId = 1)
//                BEGIN
//                    INSERT INTO Instructors 
//                        (InstructorId, Username, Password, Email, DateRegistered, IsActive)
//                    VALUES 
//                        (1, 'admin_instructor', 'pass123', 'instructor@smartlearn.com', '2024-01-01', 1);
//                END";

//            using (var cmd = new SqlCommand(instructorSeed, connection))
//                {
//                cmd.ExecuteNonQuery();
//                }

//            // Seed Courses 
//            const string courseSeed = @"
//                IF NOT EXISTS (SELECT 1 FROM Courses WHERE CourseId = 1)
//                BEGIN
//                    INSERT INTO Courses 
//                        (CourseId, Title, Description, Category, Difficulty, CurrentEnrollments, MaxStudents, InstructorId)
//                    VALUES 
//                        (1, 'C# Basics', 'Introduction to C# programming', 'Programming', 'Beginner', 0, 200, 1),
//                        (2, 'Advanced LINQ', 'Deep dive into LINQ queries', 'Programming', 'Advanced', 0, 100, 1),
//                        (3, 'Web Design 101', 'HTML and CSS fundamentals', 'Design', 'Beginner', 0, 150, 1),
//                        (4, 'UI/UX Principles', 'User interface design principles', 'Design', 'Intermediate', 0, 80, 1),
//                        (5, 'Data Science Intro', 'Introduction to data science', 'Data', 'Intermediate', 0, 120, 1),
//                        (6, 'Machine Learning', 'ML algorithms and applications', 'Data', 'Advanced', 0, 60, 1);
//                END";

//            using (var cmd = new SqlCommand(courseSeed, connection))
//                {
//                cmd.ExecuteNonQuery();
//                }
//            }

//        // ===================================================================
//        // Example helper methods (you can add more as needed)
//        // ===================================================================

//        public List<Course> GetAllCourses()
//            {
//            var courses = new List<Course>();
//            using var connection = new SqlConnection(_connectionString);
//            connection.Open();

//            const string query = "SELECT * FROM Courses ORDER BY CourseId";
//            using var cmd = new SqlCommand(query, connection);
//            using var reader = cmd.ExecuteReader();

//            while (reader.Read())
//                {
//                courses.Add(new Course
//                    {
//                    CourseId = reader.GetInt32("CourseId"),
//                    Title = reader.GetString("Title"),
//                    Description = reader.GetString("Description"),
//                    Category = reader.GetString("Category"),
//                    Difficulty = reader.GetString("Difficulty"),
//                    CurrentEnrollments = reader.GetInt32("CurrentEnrollments"),
//                    MaxStudents = reader.GetInt32("MaxStudents"),
//                    InstructorId = reader.GetInt32("InstructorId")
//                    });
//                }
//            return courses;
//            }

//        // Add similar methods for Student, Instructor, Enrollment, etc.
//        // Example: AddStudent, EnrollStudent, UpdateEnrollmentCount, etc.
//        }
//    }