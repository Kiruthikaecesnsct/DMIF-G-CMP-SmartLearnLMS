using Microsoft.EntityFrameworkCore;
using Week_1;
using Week_1.Logging;

namespace Week_1.Data
    {
    public class SmartLearnDbContext : DbContext
        {
        public DbSet<Student> Students { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }

        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
            optionsBuilder.UseSqlServer(@"Server=localhost\SQLEXPRESS;Database=SmartLearnDB;Trusted_Connection=True;TrustServerCertificate=True;");
            }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
            // ── One-to-Many: Instructor → Courses ──────────────────────────────
            // One instructor teaches many courses
            // RESTRICT: cannot delete instructor if they have courses
            modelBuilder.Entity<Course>()
            .HasOne(c => c.Instructor)
            .WithMany(i => i.Courses)
            .HasForeignKey(c => c.InstructorId)
            .IsRequired(false)                  // ← add this line
            .OnDelete(DeleteBehavior.Restrict);

            // ── One-to-Many: Course → Enrollments ──────────────────────────────
            // One course has many enrollments
            // CASCADE: delete course → delete its enrollments automatically
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // ── One-to-Many: Student → Enrollments ─────────────────────────────
            // One student has many enrollments
            // CASCADE: delete student → delete their enrollments automatically
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
            }
        }
    }