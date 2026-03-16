using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Week_1.Services
    {
    /// <summary>
    /// Assignment 6 — Part 3 & 6: EF Core student operations.
    /// Replaces/supplements JSON-based student management.
    /// </summary>
    public class EfStudentService
        {
        // ══════════════════════════════════════════════════════
        //  PART 3 TASK 1: CREATE
        // ══════════════════════════════════════════════════════

        /// <summary>Add a new student to the database.</summary>
        public UserEntity RegisterStudent(string username, string email, string password)
            {
            // Part 8 Task 1: Input validation
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

                // Part 8 Task 2: Duplicate prevention using Any()
                if (ctx.Users.Any(u => u.Username == username))
                    { Console.WriteLine($"  ✗ Username '{username}' already exists."); return null; }
                if (ctx.Users.Any(u => u.Email == email))
                    { Console.WriteLine($"  ✗ Email '{email}' already registered."); return null; }

                var student = new UserEntity
                    {
                    Username = username,
                    Email = email,
                    PasswordHash = password,  // In real apps hash with BCrypt
                    UserType = "Student",
                    CreatedDate = DateTime.Now,
                    IsActive = true
                    };

                ctx.Users.Add(student);
                ctx.SaveChanges();
                Console.WriteLine($"  ✓ Student '{username}' registered. ID: {student.UserId}");
                return student;
                }
            catch (DbUpdateException ex) { Console.WriteLine($"  ✗ DB update error: {ex.InnerException?.Message ?? ex.Message}"); return null; }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return null; }
            }

        // ══════════════════════════════════════════════════════
        //  PART 3 TASK 2: READ
        // ══════════════════════════════════════════════════════

        /// <summary>Get all students using ToList().</summary>
        public List<UserEntity> GetAllStudents()
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                return ctx.Users
                    .Where(u => u.UserType == "Student")
                    .OrderBy(u => u.Username)
                    .ToList();
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return new List<UserEntity>(); }
            }

        /// <summary>Find a student by ID using Find().</summary>
        public UserEntity FindStudent(int userId)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                var u = ctx.Users.Find(userId);
                if (u == null || u.UserType != "Student") return null;
                return u;
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return null; }
            }

        /// <summary>Get students filtered by minimum progress using LINQ Where().</summary>
        public List<UserEntity> GetStudentsByMinProgress(int minProgress)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                // Filter via enrollments — students whose max progress >= minProgress
                return ctx.Users
                    .Where(u => u.UserType == "Student")
                    .Where(u => u.Enrollments.Any(e => e.ProgressPercent >= minProgress))
                    .OrderBy(u => u.Username)
                    .ToList();
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return new List<UserEntity>(); }
            }

        /// <summary>Login — verify username + password from DB.</summary>
        public UserEntity Login(string username, string password)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                var user = ctx.Users.FirstOrDefault(u => u.Username == username && u.PasswordHash == password);
                if (user == null) { Console.WriteLine("  ✗ Invalid username or password."); return null; }

                // Update last login date
                user.LastLoginDate = DateTime.Now;
                ctx.SaveChanges();
                return user;
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return null; }
            }

        // ══════════════════════════════════════════════════════
        //  PART 3 TASK 3: UPDATE
        // ══════════════════════════════════════════════════════

        /// <summary>Update student progress for a specific course.</summary>
        public bool UpdateProgress(int studentId, int courseId, int newProgress)
            {
            if (newProgress < 0 || newProgress > 100)
                { Console.WriteLine("  ✗ Progress must be 0-100."); return false; }

            try
                {
                using var ctx = new SmartLearnDbContext();
                var enr = ctx.Enrollments
                    .FirstOrDefault(e => e.StudentId == studentId && e.CourseId == courseId);
                if (enr == null) { Console.WriteLine("  ✗ Enrollment not found."); return false; }

                enr.ProgressPercent = newProgress;
                if (newProgress == 100)
                    {
                    enr.Status = "Completed";
                    enr.CompletionDate = DateTime.Now;
                    }
                ctx.SaveChanges();
                Console.WriteLine($"  ✓ Progress updated to {newProgress}%");
                return true;
                }
            catch (DbUpdateException ex) { Console.WriteLine($"  ✗ DB update error: {ex.InnerException?.Message ?? ex.Message}"); return false; }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return false; }
            }

        // ══════════════════════════════════════════════════════
        //  PART 3 TASK 4: DELETE
        // ══════════════════════════════════════════════════════

        /// <summary>Delete a student by ID (only if no active enrollments).</summary>
        public bool DeleteStudent(int userId)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                var user = ctx.Users.Find(userId);
                if (user == null) { Console.WriteLine("  ✗ Student not found."); return false; }

                // Check for dependencies
                bool hasEnrollments = ctx.Enrollments.Any(e => e.StudentId == userId);
                if (hasEnrollments)
                    { Console.WriteLine("  ✗ Cannot delete student — has existing enrollments. Remove enrollments first."); return false; }

                ctx.Users.Remove(user);
                ctx.SaveChanges();
                Console.WriteLine($"  ✓ Student '{user.Username}' deleted.");
                return true;
                }
            catch (DbUpdateException ex) { Console.WriteLine($"  ✗ DB error: {ex.InnerException?.Message ?? ex.Message}"); return false; }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return false; }
            }

        // ══════════════════════════════════════════════════════
        //  PART 4 TASK 1: EAGER LOADING with Include()
        // ══════════════════════════════════════════════════════

        /// <summary>Get a student with all their enrollments (Include).</summary>
        public UserEntity GetStudentWithEnrollments(int userId)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                return ctx.Users
                    .Include(u => u.Enrollments)
                    .FirstOrDefault(u => u.UserId == userId && u.UserType == "Student");
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return null; }
            }

        /// <summary>Get a student with enrollments AND their course details (ThenInclude).</summary>
        public UserEntity GetStudentWithEnrollmentsAndCourses(int userId)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                return ctx.Users
                    .Include(u => u.Enrollments)
                        .ThenInclude(e => e.Course)
                    .FirstOrDefault(u => u.UserId == userId && u.UserType == "Student");
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return null; }
            }

        // ══════════════════════════════════════════════════════
        //  PART 4 TASK 2: COMPLEX FILTERED QUERIES
        // ══════════════════════════════════════════════════════

        /// <summary>Get students with progress > threshold, including their enrollments.</summary>
        public List<UserEntity> GetStudentsAboveProgress(int threshold)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                return ctx.Users
                    .Include(u => u.Enrollments)
                        .ThenInclude(e => e.Course)
                    .Where(u => u.UserType == "Student"
                             && u.Enrollments.Any(e => e.ProgressPercent > threshold))
                    .OrderBy(u => u.Username)
                    .ToList();
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return new List<UserEntity>(); }
            }

        /// <summary>Get a student's active enrollments only.</summary>
        public List<EnrollmentEntity> GetActiveEnrollments(int studentId)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                return ctx.Enrollments
                    .Include(e => e.Course)
                    .Where(e => e.StudentId == studentId && e.Status == "Active")
                    .OrderBy(e => e.EnrolledDate)
                    .ToList();
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return new List<EnrollmentEntity>(); }
            }

        // ══════════════════════════════════════════════════════
        //  PART 7 TASK 2: AGGREGATION
        // ══════════════════════════════════════════════════════

        public int GetTotalStudentCount()
            {
            try { using var ctx = new SmartLearnDbContext(); return ctx.Users.Count(u => u.UserType == "Student"); }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return 0; }
            }

        public double GetAverageProgress()
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                return ctx.Enrollments.Any() ? ctx.Enrollments.Average(e => e.ProgressPercent) : 0;
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return 0; }
            }

        /// <summary>Students enrolled in the last N days.</summary>
        public List<UserEntity> GetRecentlyEnrolledStudents(int days)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                var since = DateTime.Now.AddDays(-days);
                return ctx.Users
                    .Include(u => u.Enrollments)
                    .Where(u => u.UserType == "Student"
                             && u.Enrollments.Any(e => e.EnrolledDate >= since))
                    .OrderByDescending(u => u.Enrollments.Max(e => e.EnrolledDate))
                    .ToList();
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return new List<UserEntity>(); }
            }

        // ══════════════════════════════════════════════════════
        //  PART 7 TASK 3: MULTI-CRITERIA FILTER
        // ══════════════════════════════════════════════════════

        /// <summary>Students with progress > threshold enrolled in a specific category.</summary>
        public List<UserEntity> GetStudentsByProgressAndCategory(int minProgress, string category)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                return ctx.Users
                    .Include(u => u.Enrollments).ThenInclude(e => e.Course)
                    .Where(u => u.UserType == "Student"
                             && u.Enrollments.Any(e => e.ProgressPercent > minProgress
                                                    && e.Course.Category == category))
                    .OrderBy(u => u.Username)
                    .ToList();
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return new List<UserEntity>(); }
            }

        // ── DISPLAY HELPER ──
        public void DisplayStudentEntity(UserEntity u)
            {
            Console.WriteLine($"  ID: {u.UserId} | {u.Username} | {u.Email} | Active: {(u.IsActive ? "✓" : "✗")} | Last Login: {u.LastLoginDate?.ToString("g") ?? "Never"}");
            }
        }
    }