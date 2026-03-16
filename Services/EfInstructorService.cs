using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Week_1.Services
    {
    /// <summary>
    /// Assignment 6 — EF Core Instructor operations.
    /// Mirrors EfStudentService but for Instructor role.
    /// Covers: CRUD, Include/ThenInclude, filtering, aggregation, validation.
    /// </summary>
    public class EfInstructorService
        {
        // ══════════════════════════════════════════════════════
        //  CREATE
        // ══════════════════════════════════════════════════════

        /// <summary>Register a new instructor in the database.</summary>
        public UserEntity RegisterInstructor(string username, string email, string password, string department = "Computer Science")
            {
            // Input validation
            if (string.IsNullOrWhiteSpace(username) || username.Length < 3)
                { Console.WriteLine("  ✗ Username must be at least 3 characters."); return null; }
            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
                { Console.WriteLine("  ✗ Invalid email format."); return null; }
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
                { Console.WriteLine("  ✗ Password must be at least 8 characters."); return null; }
            if (!password.Any(char.IsDigit))
                { Console.WriteLine("  ✗ Password must contain at least 1 digit."); return null; }

            try
                {
                using var ctx = new SmartLearnDbContext();

                // Duplicate prevention
                if (ctx.Users.Any(u => u.Username == username))
                    { Console.WriteLine($"  ✗ Username '{username}' already exists."); return null; }
                if (ctx.Users.Any(u => u.Email == email))
                    { Console.WriteLine($"  ✗ Email '{email}' already registered."); return null; }

                var instructor = new UserEntity
                    {
                    Username = username,
                    Email = email,
                    PasswordHash = password,
                    UserType = "Instructor",
                    CreatedDate = DateTime.Now,
                    IsActive = true
                    };

                ctx.Users.Add(instructor);
                ctx.SaveChanges();
                Console.WriteLine($"  ✓ Instructor '{username}' registered. ID: {instructor.UserId}");
                return instructor;
                }
            catch (DbUpdateException ex) { Console.WriteLine($"  ✗ DB error: {ex.InnerException?.Message ?? ex.Message}"); return null; }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return null; }
            }

        // ══════════════════════════════════════════════════════
        //  READ
        // ══════════════════════════════════════════════════════

        /// <summary>Get all instructors.</summary>
        public List<UserEntity> GetAllInstructors()
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                return ctx.Users
                    .Where(u => u.UserType == "Instructor")
                    .OrderBy(u => u.Username)
                    .ToList();
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return new List<UserEntity>(); }
            }

        /// <summary>Find a single instructor by their DB ID.</summary>
        public UserEntity FindInstructor(int userId)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                var u = ctx.Users.Find(userId);
                if (u == null || u.UserType != "Instructor") return null;
                return u;
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return null; }
            }

        /// <summary>Part 4 — Instructor with all their courses (Include).</summary>
        public UserEntity GetInstructorWithCourses(int userId)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                return ctx.Users
                    .Include(u => u.TaughtCourses)
                    .FirstOrDefault(u => u.UserId == userId && u.UserType == "Instructor");
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return null; }
            }

        /// <summary>Part 4 — Instructor with courses AND each course's enrolled students.</summary>
        public UserEntity GetInstructorWithCoursesAndStudents(int userId)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                return ctx.Users
                    .Include(u => u.TaughtCourses)
                        .ThenInclude(c => c.Enrollments)
                            .ThenInclude(e => e.Student)
                    .FirstOrDefault(u => u.UserId == userId && u.UserType == "Instructor");
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return null; }
            }

        /// <summary>Search instructors by username or email keyword.</summary>
        public List<UserEntity> SearchInstructors(string keyword)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                return ctx.Users
                    .Where(u => u.UserType == "Instructor" &&
                               (u.Username.Contains(keyword) || u.Email.Contains(keyword)))
                    .OrderBy(u => u.Username)
                    .ToList();
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return new List<UserEntity>(); }
            }

        /// <summary>All instructors who teach a specific course category.</summary>
        public List<UserEntity> GetInstructorsByCategory(string category)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                return ctx.Users
                    .Include(u => u.TaughtCourses)
                    .Where(u => u.UserType == "Instructor"
                             && u.TaughtCourses.Any(c => c.Category == category))
                    .OrderBy(u => u.Username)
                    .ToList();
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return new List<UserEntity>(); }
            }

        // ══════════════════════════════════════════════════════
        //  UPDATE
        // ══════════════════════════════════════════════════════

        /// <summary>Activate or deactivate an instructor account.</summary>
        public bool SetActiveStatus(int userId, bool isActive)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                var u = ctx.Users.Find(userId);
                if (u == null || u.UserType != "Instructor")
                    { Console.WriteLine("  ✗ Instructor not found."); return false; }

                u.IsActive = isActive;
                ctx.SaveChanges();
                Console.WriteLine($"  ✓ Instructor '{u.Username}' is now {(isActive ? "Active" : "Inactive")}.");
                return true;
                }
            catch (DbUpdateException ex) { Console.WriteLine($"  ✗ DB error: {ex.InnerException?.Message ?? ex.Message}"); return false; }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return false; }
            }

        /// <summary>Update an instructor's email.</summary>
        public bool UpdateEmail(int userId, string newEmail)
            {
            if (!newEmail.Contains("@"))
                { Console.WriteLine("  ✗ Invalid email format."); return false; }

            try
                {
                using var ctx = new SmartLearnDbContext();
                var u = ctx.Users.Find(userId);
                if (u == null || u.UserType != "Instructor")
                    { Console.WriteLine("  ✗ Instructor not found."); return false; }

                if (ctx.Users.Any(x => x.Email == newEmail && x.UserId != userId))
                    { Console.WriteLine("  ✗ Email already in use by another user."); return false; }

                u.Email = newEmail;
                ctx.SaveChanges();
                Console.WriteLine($"  ✓ Email updated to '{newEmail}'.");
                return true;
                }
            catch (DbUpdateException ex) { Console.WriteLine($"  ✗ DB error: {ex.InnerException?.Message ?? ex.Message}"); return false; }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return false; }
            }

        // ══════════════════════════════════════════════════════
        //  DELETE
        // ══════════════════════════════════════════════════════

        /// <summary>Delete an instructor — only if they have no courses assigned.</summary>
        public bool DeleteInstructor(int userId)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                var u = ctx.Users.Find(userId);
                if (u == null || u.UserType != "Instructor")
                    { Console.WriteLine("  ✗ Instructor not found."); return false; }

                bool hasCourses = ctx.Courses.Any(c => c.InstructorId == userId);
                if (hasCourses)
                    { Console.WriteLine("  ✗ Cannot delete — instructor has courses assigned. Reassign courses first."); return false; }

                ctx.Users.Remove(u);
                ctx.SaveChanges();
                Console.WriteLine($"  ✓ Instructor '{u.Username}' deleted.");
                return true;
                }
            catch (DbUpdateException ex) { Console.WriteLine($"  ✗ DB error: {ex.InnerException?.Message ?? ex.Message}"); return false; }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return false; }
            }

        // ══════════════════════════════════════════════════════
        //  AGGREGATION & ANALYTICS
        // ══════════════════════════════════════════════════════

        /// <summary>Total number of instructors in DB.</summary>
        public int GetTotalInstructorCount()
            {
            try { using var ctx = new SmartLearnDbContext(); return ctx.Users.Count(u => u.UserType == "Instructor"); }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return 0; }
            }

        /// <summary>Total students enrolled across all courses taught by a specific instructor.</summary>
        public int GetTotalStudentsForInstructor(int instructorId)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                return ctx.Enrollments
                    .Include(e => e.Course)
                    .Where(e => e.Course.InstructorId == instructorId && e.Status == "Active")
                    .Count();
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return 0; }
            }

        /// <summary>Instructors with more than N courses assigned.</summary>
        public List<UserEntity> GetBusyInstructors(int minCourses)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                return ctx.Users
                    .Include(u => u.TaughtCourses)
                    .Where(u => u.UserType == "Instructor"
                             && u.TaughtCourses.Count > minCourses)
                    .OrderByDescending(u => u.TaughtCourses.Count)
                    .ToList();
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return new List<UserEntity>(); }
            }

        /// <summary>Average enrollment count across courses taught by an instructor.</summary>
        public double GetAverageEnrollmentForInstructor(int instructorId)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                var courses = ctx.Courses.Where(c => c.InstructorId == instructorId).ToList();
                if (!courses.Any()) return 0;
                return courses.Average(c => c.CurrentEnrollments);
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return 0; }
            }

        // ══════════════════════════════════════════════════════
        //  DISPLAY HELPERS
        // ══════════════════════════════════════════════════════

        public void DisplayInstructorEntity(UserEntity u)
            {
            Console.WriteLine($"  ID: {u.UserId,-5} | {u.Username,-22} | {u.Email,-34} | Active: {(u.IsActive ? "✓" : "✗")} | Last Login: {u.LastLoginDate?.ToString("g") ?? "Never"}");
            }

        public void DisplayInstructorList(List<UserEntity> instructors)
            {
            if (!instructors.Any()) { Console.WriteLine("  No instructors found."); return; }
            Console.WriteLine($"\n  {"ID",-6}{"Username",-24}{"Email",-34}{"Active",-8}Last Login");
            Console.WriteLine("  " + new string('─', 82));
            foreach (var u in instructors)
                Console.WriteLine($"  {u.UserId,-6}{u.Username,-24}{u.Email,-34}{(u.IsActive ? "✓" : "✗"),-8}{u.LastLoginDate?.ToString("g") ?? "Never"}");
            }

        public void DisplayInstructorWithCourses(UserEntity instr)
            {
            if (instr == null) { Console.WriteLine("  ✗ Instructor not found."); return; }
            Console.WriteLine($"\n  Instructor : {instr.Username}");
            Console.WriteLine($"  Email      : {instr.Email}");
            Console.WriteLine($"  Active     : {(instr.IsActive ? "✓" : "✗")}");
            Console.WriteLine($"  Courses ({instr.TaughtCourses?.Count ?? 0}):");
            if (instr.TaughtCourses == null || !instr.TaughtCourses.Any())
                { Console.WriteLine("    No courses assigned."); return; }
            Console.WriteLine($"  {"ID",-6}{"Title",-36}{"Category",-20}{"Enrolled",-10}Capacity");
            Console.WriteLine("  " + new string('─', 80));
            foreach (var c in instr.TaughtCourses)
                Console.WriteLine($"  {c.CourseId,-6}{c.Title,-36}{c.Category,-20}{c.CurrentEnrollments,-10}{c.MaxCapacity}");
            }

        public void DisplayInstructorWithCoursesAndStudents(UserEntity instr)
            {
            if (instr == null) { Console.WriteLine("  ✗ Instructor not found."); return; }
            Console.WriteLine($"\n  Instructor : {instr.Username} | {instr.Email}");
            if (instr.TaughtCourses == null || !instr.TaughtCourses.Any())
                { Console.WriteLine("    No courses assigned."); return; }

            foreach (var c in instr.TaughtCourses)
                {
                Console.WriteLine($"\n  ── [{c.CourseId}] {c.Title} ({c.Category}) — {c.CurrentEnrollments}/{c.MaxCapacity} enrolled");
                if (c.Enrollments == null || !c.Enrollments.Any())
                    { Console.WriteLine("       No enrollments yet."); continue; }
                foreach (var e in c.Enrollments)
                    Console.WriteLine($"       • {e.Student?.Username ?? "N/A",-22} {e.ProgressPercent,3}%  ({e.Status})");
                }
            }
        }
    }