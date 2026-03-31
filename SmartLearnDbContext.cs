using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Week_1.Logging;

namespace Week_1
    {
    /// <summary>
    /// EF Core DbContext for SmartLearn LMS.
    /// Assignment 8 — adds AuditLogs DbSet; all relationships unchanged from Wk7.
    /// </summary>
    public class SmartLearnDbContext : DbContext
        {
        // ── DbSet properties ──
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<CourseEntity> Courses { get; set; }
        public DbSet<EnrollmentEntity> Enrollments { get; set; }
        public DbSet<CourseModule> CourseModules { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<CourseRating> CourseRatings { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }  // ← NEW Wk8

        // ── Connection string ──
        private const string ConnectionString =
            @"Server=localhost\SQLEXPRESS;Database=SmartLearnDB;Trusted_Connection=True;TrustServerCertificate=True;";

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlServer(ConnectionString);
            }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
            // ── PART 1 TASK 1: Core Entity Relationships (unchanged from Wk7) ──

            modelBuilder.Entity<UserEntity>()
                .HasIndex(u => u.Username).IsUnique();
            modelBuilder.Entity<UserEntity>()
                .HasIndex(u => u.Email).IsUnique();

            modelBuilder.Entity<EnrollmentEntity>()
                .HasIndex(e => new { e.StudentId, e.CourseId }).IsUnique();

            modelBuilder.Entity<CourseEntity>()
                .HasOne(c => c.Instructor)
                .WithMany(u => u.TaughtCourses)
                .HasForeignKey(c => c.InstructorId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            modelBuilder.Entity<EnrollmentEntity>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EnrollmentEntity>()
                .HasOne(e => e.Student)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            // ── PART 1 TASK 2: Course Module Hierarchy ──

            modelBuilder.Entity<CourseModule>()
                .HasOne(m => m.Course)
                .WithMany(c => c.Modules)
                .HasForeignKey(m => m.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Lesson>()
                .HasOne(l => l.Module)
                .WithMany(m => m.Lessons)
                .HasForeignKey(l => l.ModuleId)
                .OnDelete(DeleteBehavior.Cascade);

            // ── PART 1 TASK 3: Course Rating System ──

            modelBuilder.Entity<CourseRating>()
                .HasIndex(r => new { r.CourseId, r.StudentId }).IsUnique();

            modelBuilder.Entity<CourseRating>()
                .HasOne(r => r.Course)
                .WithMany(c => c.Ratings)
                .HasForeignKey(r => r.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CourseRating>()
                .HasOne(r => r.Student)
                .WithMany(u => u.Ratings)
                .HasForeignKey(r => r.StudentId)
                .OnDelete(DeleteBehavior.NoAction);

            // ── ASSIGNMENT 8: Indexes for performance (Part 4 Task 1) ──

            // Courses: frequently searched columns
            modelBuilder.Entity<CourseEntity>()
                .HasIndex(c => c.Category);
            modelBuilder.Entity<CourseEntity>()
                .HasIndex(c => c.DifficultyLevel);
            modelBuilder.Entity<CourseEntity>()
                .HasIndex(c => c.Title);

            // Enrollments: status filter used in at-risk queries
            modelBuilder.Entity<EnrollmentEntity>()
                .HasIndex(e => e.Status);
            modelBuilder.Entity<EnrollmentEntity>()
                .HasIndex(e => e.StudentId);
            modelBuilder.Entity<EnrollmentEntity>()
                .HasIndex(e => e.CourseId);

            // AuditLogs: time-range and user queries
            modelBuilder.Entity<AuditLog>()
                .HasIndex(a => a.Timestamp);
            modelBuilder.Entity<AuditLog>()
                .HasIndex(a => a.UserId);
            modelBuilder.Entity<AuditLog>()
                .HasIndex(a => a.Action);
            }
        }
    }