using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Week_1.Services
    {
    /// <summary>
    /// Assignment 6 — EF Core Admin operations.
    /// Covers full platform management: user CRUD, course oversight,
    /// enrollment management, system analytics, and validation.
    /// </summary>
    public class EfAdminService
        {
        // ══════════════════════════════════════════════════════
        //  USER MANAGEMENT — CREATE
        // ══════════════════════════════════════════════════════

        /// <summary>Create any user type (Student / Instructor / Admin) from admin panel.</summary>
        public UserEntity CreateUser(string username, string email, string password, string userType)
            {
            var validTypes = new[] { "Student", "Instructor", "Admin" };
            if (!validTypes.Contains(userType))
                { Console.WriteLine("  ✗ UserType must be: Student, Instructor, or Admin."); return null; }
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

                if (ctx.Users.Any(u => u.Username == username))
                    { Console.WriteLine($"  ✗ Username '{username}' already exists."); return null; }
                if (ctx.Users.Any(u => u.Email == email))
                    { Console.WriteLine($"  ✗ Email '{email}' already registered."); return null; }

                var user = new UserEntity
                    {
                    Username = username,
                    Email = email,
                    PasswordHash = password,
                    UserType = userType,
                    CreatedDate = DateTime.Now,
                    IsActive = true
                    };

                ctx.Users.Add(user);
                ctx.SaveChanges();
                Console.WriteLine($"  ✓ [{userType}] '{username}' created. ID: {user.UserId}");
                return user;
                }
            catch (DbUpdateException ex) { Console.WriteLine($"  ✗ DB error: {ex.InnerException?.Message ?? ex.Message}"); return null; }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return null; }
            }

        // ══════════════════════════════════════════════════════
        //  USER MANAGEMENT — READ
        // ══════════════════════════════════════════════════════

        /// <summary>Get ALL users regardless of type.</summary>
        public List<UserEntity> GetAllUsers()
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                return ctx.Users.OrderBy(u => u.UserType).ThenBy(u => u.Username).ToList();
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return new List<UserEntity>(); }
            }

        /// <summary>Get all users of a specific type.</summary>
        public List<UserEntity> GetUsersByType(string userType)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                return ctx.Users
                    .Where(u => u.UserType == userType)
                    .OrderBy(u => u.Username)
                    .ToList();
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return new List<UserEntity>(); }
            }

        /// <summary>Find any user by DB ID using Find().</summary>
        public UserEntity FindUserById(int userId)
            {
            try { using var ctx = new SmartLearnDbContext(); return ctx.Users.Find(userId); }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return null; }
            }

        /// <summary>Find any user by username.</summary>
        public UserEntity FindUserByUsername(string username)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                return ctx.Users.FirstOrDefault(u => u.Username == username);
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return null; }
            }

        /// <summary>Part 4 — Get student with all enrollments and course details.</summary>
        public UserEntity GetUserWithFullDetails(int userId)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                return ctx.Users
                    .Include(u => u.Enrollments)
                        .ThenInclude(e => e.Course)
                    .Include(u => u.TaughtCourses)
                        .ThenInclude(c => c.Enrollments)
                    .FirstOrDefault(u => u.UserId == userId);
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return null; }
            }

        // ══════════════════════════════════════════════════════
        //  USER MANAGEMENT — UPDATE
        // ══════════════════════════════════════════════════════

        /// <summary>Toggle active/inactive status for any user.</summary>
        public bool SetUserActiveStatus(int userId, bool isActive)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                var user = ctx.Users.Find(userId);
                if (user == null) { Console.WriteLine("  ✗ User not found."); return false; }

                user.IsActive = isActive;
                ctx.SaveChanges();
                Console.WriteLine($"  ✓ '{user.Username}' ({user.UserType}) is now {(isActive ? "Active" : "Inactive")}.");
                return true;
                }
            catch (DbUpdateException ex) { Console.WriteLine($"  ✗ DB error: {ex.InnerException?.Message ?? ex.Message}"); return false; }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return false; }
            }

        /// <summary>Reset a user's password.</summary>
        public bool ResetPassword(int userId, string newPassword)
            {
            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 8)
                { Console.WriteLine("  ✗ New password must be at least 8 characters."); return false; }
            if (!newPassword.Any(char.IsDigit))
                { Console.WriteLine("  ✗ Password must contain at least 1 digit."); return false; }

            try
                {
                using var ctx = new SmartLearnDbContext();
                var user = ctx.Users.Find(userId);
                if (user == null) { Console.WriteLine("  ✗ User not found."); return false; }

                user.PasswordHash = newPassword;
                ctx.SaveChanges();
                Console.WriteLine($"  ✓ Password reset for '{user.Username}'.");
                return true;
                }
            catch (DbUpdateException ex) { Console.WriteLine($"  ✗ DB error: {ex.InnerException?.Message ?? ex.Message}"); return false; }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return false; }
            }

        // ══════════════════════════════════════════════════════
        //  USER MANAGEMENT — DELETE
        // ══════════════════════════════════════════════════════

        /// <summary>Delete any user — with full dependency checks.</summary>
        public bool DeleteUser(int userId)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                var user = ctx.Users.Find(userId);
                if (user == null) { Console.WriteLine("  ✗ User not found."); return false; }

                // Prevent deleting users with active enrollments (students)
                if (ctx.Enrollments.Any(e => e.StudentId == userId))
                    { Console.WriteLine("  ✗ Cannot delete — user has enrollment records. Remove enrollments first."); return false; }

                // Prevent deleting instructors with courses
                if (ctx.Courses.Any(c => c.InstructorId == userId))
                    { Console.WriteLine("  ✗ Cannot delete — instructor has courses assigned. Reassign courses first."); return false; }

                ctx.Users.Remove(user);
                ctx.SaveChanges();
                Console.WriteLine($"  ✓ User '{user.Username}' ({user.UserType}) deleted.");
                return true;
                }
            catch (DbUpdateException ex) { Console.WriteLine($"  ✗ DB error: {ex.InnerException?.Message ?? ex.Message}"); return false; }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return false; }
            }

        // ══════════════════════════════════════════════════════
        //  COURSE MANAGEMENT
        // ══════════════════════════════════════════════════════

        /// <summary>Delete a course — only if it has no active enrollments.</summary>
        public bool DeleteCourse(int courseId)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                var course = ctx.Courses.Find(courseId);
                if (course == null) { Console.WriteLine("  ✗ Course not found."); return false; }

                bool hasActiveEnrollments = ctx.Enrollments
                    .Any(e => e.CourseId == courseId && e.Status == "Active");
                if (hasActiveEnrollments)
                    { Console.WriteLine("  ✗ Cannot delete — course has active enrollments."); return false; }

                ctx.Courses.Remove(course);
                ctx.SaveChanges();
                Console.WriteLine($"  ✓ Course '{course.Title}' deleted.");
                return true;
                }
            catch (DbUpdateException ex) { Console.WriteLine($"  ✗ DB error: {ex.InnerException?.Message ?? ex.Message}"); return false; }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return false; }
            }

        /// <summary>Reassign a course to a different instructor.</summary>
        public bool ReassignCourse(int courseId, int newInstructorId)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();

                // FK validation — verify new instructor exists
                bool instrExists = ctx.Users.Any(u => u.UserId == newInstructorId && u.UserType == "Instructor");
                if (!instrExists)
                    { Console.WriteLine("  ✗ New instructor not found or is not an Instructor role."); return false; }

                var course = ctx.Courses.Find(courseId);
                if (course == null) { Console.WriteLine("  ✗ Course not found."); return false; }

                var newInstr = ctx.Users.Find(newInstructorId);
                course.InstructorId = newInstructorId;
                course.InstructorName = newInstr!.Username;
                ctx.SaveChanges();
                Console.WriteLine($"  ✓ Course '{course.Title}' reassigned to '{newInstr.Username}'.");
                return true;
                }
            catch (DbUpdateException ex) { Console.WriteLine($"  ✗ DB error: {ex.InnerException?.Message ?? ex.Message}"); return false; }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return false; }
            }

        // ══════════════════════════════════════════════════════
        //  ENROLLMENT MANAGEMENT
        // ══════════════════════════════════════════════════════

        /// <summary>Get ALL enrollments across the platform with full details.</summary>
        public List<EnrollmentEntity> GetAllEnrollments()
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                return ctx.Enrollments
                    .Include(e => e.Student)
                    .Include(e => e.Course)
                    .OrderByDescending(e => e.EnrolledDate)
                    .ToList();
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return new List<EnrollmentEntity>(); }
            }

        /// <summary>Get enrollments filtered by status.</summary>
        public List<EnrollmentEntity> GetEnrollmentsByStatus(string status)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                return ctx.Enrollments
                    .Include(e => e.Student)
                    .Include(e => e.Course)
                    .Where(e => e.Status == status)
                    .OrderByDescending(e => e.EnrolledDate)
                    .ToList();
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return new List<EnrollmentEntity>(); }
            }

        // ══════════════════════════════════════════════════════
        //  SYSTEM ANALYTICS (Part 7)
        // ══════════════════════════════════════════════════════

        /// <summary>Full platform analytics dashboard.</summary>
        public void DisplaySystemAnalytics()
            {
            try
                {
                using var ctx = new SmartLearnDbContext();

                int totalUsers = ctx.Users.Count();
                int totalStudents = ctx.Users.Count(u => u.UserType == "Student");
                int totalInstructors = ctx.Users.Count(u => u.UserType == "Instructor");
                int totalAdmins = ctx.Users.Count(u => u.UserType == "Admin");
                int activeStudents = ctx.Users.Count(u => u.UserType == "Student" && u.IsActive);
                int totalCourses = ctx.Courses.Count();
                int totalEnrollments = ctx.Enrollments.Count();
                int activeEnr = ctx.Enrollments.Count(e => e.Status == "Active");
                int completedEnr = ctx.Enrollments.Count(e => e.Status == "Completed");
                int droppedEnr = ctx.Enrollments.Count(e => e.Status == "Dropped");
                double avgProgress = ctx.Enrollments.Any() ? ctx.Enrollments.Average(e => e.ProgressPercent) : 0;
                int maxEnr = ctx.Courses.Any() ? ctx.Courses.Max(c => c.CurrentEnrollments) : 0;
                int minEnr = ctx.Courses.Any() ? ctx.Courses.Min(c => c.CurrentEnrollments) : 0;

                string topCategory = ctx.Courses
                    .GroupBy(c => c.Category)
                    .OrderByDescending(g => g.Sum(c => c.CurrentEnrollments))
                    .Select(g => g.Key)
                    .FirstOrDefault() ?? "N/A";

                Console.WriteLine("╔════════════════════════════════════════╗");
                Console.WriteLine("║    SMARTLEARN — DB SYSTEM ANALYTICS    ║");
                Console.WriteLine("╠════════════════════════════════════════╣");
                Console.WriteLine($"║  Total Users        : {totalUsers,-17}║");
                Console.WriteLine($"║  Students           : {totalStudents,-17}║");
                Console.WriteLine($"║  Instructors        : {totalInstructors,-17}║");
                Console.WriteLine($"║  Admins             : {totalAdmins,-17}║");
                Console.WriteLine($"║  Active Students    : {activeStudents,-17}║");
                Console.WriteLine("╠════════════════════════════════════════╣");
                Console.WriteLine($"║  Total Courses      : {totalCourses,-17}║");
                Console.WriteLine($"║  Total Enrollments  : {totalEnrollments,-17}║");
                Console.WriteLine($"║  Active Enrollments : {activeEnr,-17}║");
                Console.WriteLine($"║  Completed          : {completedEnr,-17}║");
                Console.WriteLine($"║  Dropped            : {droppedEnr,-17}║");
                Console.WriteLine("╠════════════════════════════════════════╣");
                Console.WriteLine($"║  Avg Progress       : {avgProgress:F1}%{"",-14}║");
                Console.WriteLine($"║  Max Enrollments    : {maxEnr,-17}║");
                Console.WriteLine($"║  Min Enrollments    : {minEnr,-17}║");
                Console.WriteLine($"║  Top Category       : {topCategory,-17}║");
                Console.WriteLine("╠════════════════════════════════════════╣");
                Console.WriteLine("║  Enrollments by Category:              ║");

                var catGroups = ctx.Courses
                    .GroupBy(c => c.Category)
                    .OrderByDescending(g => g.Sum(c => c.CurrentEnrollments))
                    .Select(g => new { Category = g.Key, Total = g.Sum(c => c.CurrentEnrollments) })
                    .ToList();

                foreach (var g in catGroups)
                    {
                    string line = $"  {g.Category,-20}: {g.Total}";
                    Console.WriteLine($"║  {line,-38}║");
                    }
                Console.WriteLine("╚════════════════════════════════════════╝");
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error generating analytics: {ex.Message}"); }
            }

        /// <summary>Part 7 Task 1 — Top N students by progress.</summary>
        public List<UserEntity> GetTopStudentsByProgress(int count)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                return ctx.Users
                    .Include(u => u.Enrollments)
                    .Where(u => u.UserType == "Student" && u.Enrollments.Any())
                    .OrderByDescending(u => u.Enrollments.Average(e => e.ProgressPercent))
                    .Take(count)
                    .ToList();
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return new List<UserEntity>(); }
            }

        /// <summary>Part 7 Task 1 — Students enrolled in the last N days.</summary>
        public List<UserEntity> GetRecentlyRegisteredUsers(int days)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                var since = DateTime.Now.AddDays(-days);
                return ctx.Users
                    .Where(u => u.CreatedDate >= since)
                    .OrderByDescending(u => u.CreatedDate)
                    .ToList();
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return new List<UserEntity>(); }
            }

        /// <summary>Part 7 Task 3 — Active enrollments with progress less than threshold.</summary>
        public List<EnrollmentEntity> GetAtRiskEnrollments(int progressThreshold = 30)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                return ctx.Enrollments
                    .Include(e => e.Student)
                    .Include(e => e.Course)
                    .Where(e => e.Status == "Active" && e.ProgressPercent < progressThreshold)
                    .OrderBy(e => e.ProgressPercent)
                    .ToList();
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return new List<EnrollmentEntity>(); }
            }

        /// <summary>Part 7 Task 2 — Count, Average, Max, Min aggregation summary.</summary>
        public void DisplayAggregationReport()
            {
            try
                {
                using var ctx = new SmartLearnDbContext();

                Console.WriteLine("\n  ── Aggregation Report ──");
                Console.WriteLine($"  Total Students       : {ctx.Users.Count(u => u.UserType == "Student")}");
                Console.WriteLine($"  Total Instructors    : {ctx.Users.Count(u => u.UserType == "Instructor")}");
                Console.WriteLine($"  Total Courses        : {ctx.Courses.Count()}");
                Console.WriteLine($"  Total Enrollments    : {ctx.Enrollments.Count()}");

                if (ctx.Enrollments.Any())
                    {
                    Console.WriteLine($"  Avg Progress         : {ctx.Enrollments.Average(e => e.ProgressPercent):F1}%");
                    Console.WriteLine($"  Max Progress (any)   : {ctx.Enrollments.Max(e => e.ProgressPercent)}%");
                    Console.WriteLine($"  Min Progress (any)   : {ctx.Enrollments.Min(e => e.ProgressPercent)}%");
                    }

                if (ctx.Courses.Any())
                    {
                    var maxCourse = ctx.Courses.OrderByDescending(c => c.CurrentEnrollments).First();
                    var minCourse = ctx.Courses.OrderBy(c => c.CurrentEnrollments).First();
                    Console.WriteLine($"  Most Enrolled Course : {maxCourse.Title} ({maxCourse.CurrentEnrollments})");
                    Console.WriteLine($"  Least Enrolled Course: {minCourse.Title} ({minCourse.CurrentEnrollments})");
                    }

                Console.WriteLine("\n  Enrollments by Category (GroupBy):");
                var groups = ctx.Courses
                    .GroupBy(c => c.Category)
                    .Select(g => new { g.Key, Total = g.Sum(c => c.CurrentEnrollments), Count = g.Count() })
                    .OrderByDescending(g => g.Total)
                    .ToList();

                Console.WriteLine($"  {"Category",-26}{"Courses",-10}{"Enrollments",-14}");
                Console.WriteLine("  " + new string('─', 50));
                foreach (var g in groups)
                    Console.WriteLine($"  {g.Key,-26}{g.Count,-10}{g.Total,-14}");
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); }
            }

        // ══════════════════════════════════════════════════════
        //  DISPLAY HELPERS
        // ══════════════════════════════════════════════════════

        public void DisplayUserList(List<UserEntity> users)
            {
            if (!users.Any()) { Console.WriteLine("  No users found."); return; }
            Console.WriteLine($"\n  {"ID",-6}{"Type",-14}{"Username",-24}{"Email",-34}{"Active",-8}Last Login");
            Console.WriteLine("  " + new string('─', 96));
            foreach (var u in users)
                Console.WriteLine($"  {u.UserId,-6}{u.UserType,-14}{u.Username,-24}{u.Email,-34}{(u.IsActive ? "✓" : "✗"),-8}{u.LastLoginDate?.ToString("g") ?? "Never"}");
            }

        public void DisplayUserFullDetails(UserEntity u)
            {
            if (u == null) { Console.WriteLine("  ✗ User not found."); return; }
            Console.WriteLine($"\n  ─── User Details ───────────────────────");
            Console.WriteLine($"  ID           : {u.UserId}");
            Console.WriteLine($"  Username     : {u.Username}");
            Console.WriteLine($"  Email        : {u.Email}");
            Console.WriteLine($"  Type         : {u.UserType}");
            Console.WriteLine($"  Active       : {(u.IsActive ? "✓ Yes" : "✗ No")}");
            Console.WriteLine($"  Created      : {u.CreatedDate:g}");
            Console.WriteLine($"  Last Login   : {u.LastLoginDate?.ToString("g") ?? "Never"}");

            if (u.Enrollments != null && u.Enrollments.Any())
                {
                Console.WriteLine($"\n  Enrollments ({u.Enrollments.Count}):");
                foreach (var e in u.Enrollments)
                    Console.WriteLine($"    → [{e.CourseId}] {e.Course?.Title ?? "N/A",-34} {e.ProgressPercent,3}%  {e.Status}");
                }

            if (u.TaughtCourses != null && u.TaughtCourses.Any())
                {
                Console.WriteLine($"\n  Teaches ({u.TaughtCourses.Count} course(s)):");
                foreach (var c in u.TaughtCourses)
                    Console.WriteLine($"    → [{c.CourseId}] {c.Title,-34} {c.CurrentEnrollments}/{c.MaxCapacity} enrolled");
                }
            }
        }
    }