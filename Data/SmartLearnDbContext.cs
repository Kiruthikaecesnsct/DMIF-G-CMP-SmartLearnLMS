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
