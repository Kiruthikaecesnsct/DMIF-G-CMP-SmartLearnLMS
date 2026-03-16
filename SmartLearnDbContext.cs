using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Week_1
    {
    /// <summary>
    /// EF Core DbContext for SmartLearn LMS.
    /// Assignment 6 — Part 2, Task 2
    /// </summary>
    public class SmartLearnDbContext : DbContext
        {
        // ── DbSet properties (one per entity table) ──
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<CourseEntity> Courses { get; set; }
        public DbSet<EnrollmentEntity> Enrollments { get; set; }

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
            // Unique constraints
            modelBuilder.Entity<UserEntity>()
                .HasIndex(u => u.Username).IsUnique();

            modelBuilder.Entity<UserEntity>()
                .HasIndex(u => u.Email).IsUnique();

            // Enrollment: unique student+course pair (prevent double-enrollment)
            modelBuilder.Entity<EnrollmentEntity>()
                .HasIndex(e => new { e.StudentId, e.CourseId }).IsUnique();

            // Relationships
            modelBuilder.Entity<EnrollmentEntity>()
                .HasOne(e => e.Student)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EnrollmentEntity>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CourseEntity>()
                .HasOne(c => c.Instructor)
                .WithMany(u => u.TaughtCourses)
                .HasForeignKey(c => c.InstructorId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);
            }
        }
    }