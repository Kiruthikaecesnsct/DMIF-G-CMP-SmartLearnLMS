using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Week_1
    {
    /// <summary>
    /// EF Core DbContext for SmartLearn LMS.
    /// Assignment 7 — Part 1: Complete Relationship Configuration
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
            // ══════════════════════════════════════════════════════
            //  PART 1 TASK 1: CORE ENTITY RELATIONSHIP CONFIGURATION
            // ══════════════════════════════════════════════════════

            // Unique constraints on Users
            modelBuilder.Entity<UserEntity>()
                .HasIndex(u => u.Username).IsUnique();
            modelBuilder.Entity<UserEntity>()
                .HasIndex(u => u.Email).IsUnique();

            // Enrollment: unique student+course pair (prevent double-enrollment)
            modelBuilder.Entity<EnrollmentEntity>()
                .HasIndex(e => new { e.StudentId, e.CourseId }).IsUnique();

            // Instructor → Courses (One-to-Many)
            // DeleteBehavior.Restrict: cannot delete instructor who has courses
            modelBuilder.Entity<CourseEntity>()
                .HasOne(c => c.Instructor)
                .WithMany(u => u.TaughtCourses)
                .HasForeignKey(c => c.InstructorId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            // Course → Enrollments (One-to-Many)
            // DeleteBehavior.Cascade: deleting a course removes its enrollments
            modelBuilder.Entity<EnrollmentEntity>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Student → Enrollments (One-to-Many)
            // DeleteBehavior.Cascade: deleting a student removes their enrollments
            modelBuilder.Entity<EnrollmentEntity>()
                .HasOne(e => e.Student)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            // ══════════════════════════════════════════════════════
            //  PART 1 TASK 2: COURSE MODULE HIERARCHY
            // ══════════════════════════════════════════════════════

            // Course → Modules (One-to-Many, Cascade delete)
            modelBuilder.Entity<CourseModule>()
                .HasOne(m => m.Course)
                .WithMany(c => c.Modules)
                .HasForeignKey(m => m.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Module → Lessons (One-to-Many, Cascade delete)
            modelBuilder.Entity<Lesson>()
                .HasOne(l => l.Module)
                .WithMany(m => m.Lessons)
                .HasForeignKey(l => l.ModuleId)
                .OnDelete(DeleteBehavior.Cascade);

            // ══════════════════════════════════════════════════════
            //  PART 1 TASK 3: COURSE RATING SYSTEM
            // ══════════════════════════════════════════════════════

            // Unique constraint: one student can only rate a course once
            modelBuilder.Entity<CourseRating>()
                .HasIndex(r => new { r.CourseId, r.StudentId }).IsUnique();

            // Course → Ratings (One-to-Many)
            modelBuilder.Entity<CourseRating>()
                .HasOne(r => r.Course)
                .WithMany(c => c.Ratings)
                .HasForeignKey(r => r.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Student → Ratings (One-to-Many)
            // Use NoAction to avoid multiple cascade paths with Enrollments
            modelBuilder.Entity<CourseRating>()
                .HasOne(r => r.Student)
                .WithMany(u => u.Ratings)
                .HasForeignKey(r => r.StudentId)
                .OnDelete(DeleteBehavior.NoAction);
            }
        }
    }