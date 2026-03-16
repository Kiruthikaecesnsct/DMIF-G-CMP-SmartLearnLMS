using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Week_1.Services
    {
    /// <summary>
    /// Assignment 6 — Part 3 & 6: EF Core course operations.
    /// </summary>
    public class EfCourseService
        {
        // ══════════════════════════════════════════════════════
        //  CREATE
        // ══════════════════════════════════════════════════════

        public CourseEntity CreateCourse(string title, string description, string category,
            string difficulty, int maxCapacity, string instructorName, int? instructorId = null)
            {
            // Validation
            if (string.IsNullOrWhiteSpace(title) || title.Length > 100)
                { Console.WriteLine("  ✗ Title must be 1-100 characters."); return null; }
            if (string.IsNullOrWhiteSpace(category))
                { Console.WriteLine("  ✗ Category is required."); return null; }
            if (maxCapacity <= 0)
                { Console.WriteLine("  ✗ Capacity must be a positive number."); return null; }

            var validDifficulties = new[] { "Beginner", "Intermediate", "Advanced" };
            if (!validDifficulties.Contains(difficulty))
                { Console.WriteLine("  ✗ Difficulty must be: Beginner, Intermediate, or Advanced."); return null; }

            try
                {
                using var ctx = new SmartLearnDbContext();

                // Verify instructor exists if provided
                if (instructorId.HasValue)
                    {
                    bool instrExists = ctx.Users.Any(u => u.UserId == instructorId.Value && u.UserType == "Instructor");
                    if (!instrExists)
                        { Console.WriteLine("  ✗ Instructor not found. Verify instructor ID."); return null; }
                    }

                var course = new CourseEntity
                    {
                    Title = title,
                    Description = description ?? "",
                    Category = category,
                    DifficultyLevel = difficulty,
                    MaxCapacity = maxCapacity,
                    CurrentEnrollments = 0,
                    InstructorName = instructorName,
                    InstructorId = instructorId,
                    CreatedDate = DateTime.Now
                    };

                ctx.Courses.Add(course);
                ctx.SaveChanges();
                Console.WriteLine($"  ✓ Course '{title}' created. ID: {course.CourseId}");
                return course;
                }
            catch (DbUpdateException ex) { Console.WriteLine($"  ✗ DB error: {ex.InnerException?.Message ?? ex.Message}"); return null; }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return null; }
            }

        // ══════════════════════════════════════════════════════
        //  READ
        // ══════════════════════════════════════════════════════

        public List<CourseEntity> GetAllCourses()
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                return ctx.Courses.OrderBy(c => c.Title).ToList();
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return new List<CourseEntity>(); }
            }

        public CourseEntity FindCourse(int courseId)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                return ctx.Courses.Find(courseId);
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return null; }
            }

        /// <summary>Search courses by keyword in title or description.</summary>
        public List<CourseEntity> SearchCourses(string keyword)
            {
            if (string.IsNullOrWhiteSpace(keyword)) return GetAllCourses();
            try
                {
                using var ctx = new SmartLearnDbContext();
                return ctx.Courses
                    .Where(c => c.Title.Contains(keyword) || (c.Description != null && c.Description.Contains(keyword)))
                    .OrderBy(c => c.Title)
                    .ToList();
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return new List<CourseEntity>(); }
            }

        /// <summary>Search courses by keyword AND category (multi-criteria).</summary>
        public List<CourseEntity> SearchCoursesByKeywordAndCategory(string keyword, string category)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                var q = ctx.Courses.AsQueryable();
                if (!string.IsNullOrWhiteSpace(keyword))
                    q = q.Where(c => c.Title.Contains(keyword) || (c.Description != null && c.Description.Contains(keyword)));
                if (!string.IsNullOrWhiteSpace(category))
                    q = q.Where(c => c.Category == category);
                return q.OrderBy(c => c.Title).ToList();
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return new List<CourseEntity>(); }
            }

        public List<CourseEntity> GetCoursesByCategory(string category)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                return ctx.Courses
                    .Where(c => c.Category == category)
                    .OrderByDescending(c => c.CurrentEnrollments)
                    .ToList();
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return new List<CourseEntity>(); }
            }

        /// <summary>Part 4: Get a course with all enrolled students (Include).</summary>
        public CourseEntity GetCourseWithStudents(int courseId)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                return ctx.Courses
                    .Include(c => c.Enrollments)
                        .ThenInclude(e => e.Student)
                    .FirstOrDefault(c => c.CourseId == courseId);
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return null; }
            }

        /// <summary>Part 4: Get a course with instructor info.</summary>
        public CourseEntity GetCourseWithInstructor(int courseId)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                return ctx.Courses
                    .Include(c => c.Instructor)
                    .FirstOrDefault(c => c.CourseId == courseId);
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return null; }
            }

        /// <summary>Part 4: Courses in a category with instructor info.</summary>
        public List<CourseEntity> GetCoursesByCategoryWithInstructor(string category)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                return ctx.Courses
                    .Include(c => c.Instructor)
                    .Where(c => c.Category == category)
                    .OrderByDescending(c => c.CurrentEnrollments)
                    .ToList();
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return new List<CourseEntity>(); }
            }

        // ══════════════════════════════════════════════════════
        //  UPDATE
        // ══════════════════════════════════════════════════════

        public bool UpdateEnrollmentCount(int courseId, int newCount)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                var course = ctx.Courses.Find(courseId);
                if (course == null) { Console.WriteLine("  ✗ Course not found."); return false; }
                if (newCount < 0 || newCount > course.MaxCapacity)
                    { Console.WriteLine($"  ✗ Count must be 0-{course.MaxCapacity}."); return false; }

                course.CurrentEnrollments = newCount;
                ctx.SaveChanges();
                Console.WriteLine($"  ✓ Enrollment count updated to {newCount}.");
                return true;
                }
            catch (DbUpdateException ex) { Console.WriteLine($"  ✗ DB error: {ex.InnerException?.Message ?? ex.Message}"); return false; }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return false; }
            }

        // ══════════════════════════════════════════════════════
        //  PART 7 TASK 1: FILTERING & SORTING
        // ══════════════════════════════════════════════════════

        /// <summary>Top N courses by enrollment count.</summary>
        public List<CourseEntity> GetTopCoursesByEnrollment(int count)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                return ctx.Courses
                    .OrderByDescending(c => c.CurrentEnrollments)
                    .Take(count)
                    .ToList();
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return new List<CourseEntity>(); }
            }

        /// <summary>Courses with more than N enrollments including student details.</summary>
        public List<CourseEntity> GetCoursesWithMinEnrollments(int min)
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                return ctx.Courses
                    .Include(c => c.Enrollments).ThenInclude(e => e.Student)
                    .Where(c => c.CurrentEnrollments > min)
                    .OrderByDescending(c => c.CurrentEnrollments)
                    .ToList();
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return new List<CourseEntity>(); }
            }

        /// <summary>Part 7 Task 3: Intermediate difficulty courses with available capacity.</summary>
        public List<CourseEntity> GetAvailableIntermediateCourses()
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                return ctx.Courses
                    .Where(c => c.DifficultyLevel == "Intermediate"
                             && c.CurrentEnrollments < c.MaxCapacity)
                    .OrderBy(c => c.Title)
                    .ToList();
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return new List<CourseEntity>(); }
            }

        // ══════════════════════════════════════════════════════
        //  PART 7 TASK 2: AGGREGATION
        // ══════════════════════════════════════════════════════

        public int GetMaxEnrollments()
            {
            try { using var ctx = new SmartLearnDbContext(); return ctx.Courses.Any() ? ctx.Courses.Max(c => c.CurrentEnrollments) : 0; }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return 0; }
            }

        public int GetMinEnrollments()
            {
            try { using var ctx = new SmartLearnDbContext(); return ctx.Courses.Any() ? ctx.Courses.Min(c => c.CurrentEnrollments) : 0; }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return 0; }
            }

        /// <summary>Enrollment count grouped by category.</summary>
        public Dictionary<string, int> GetEnrollmentsByCategory()
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                return ctx.Courses
                    .GroupBy(c => c.Category)
                    .OrderByDescending(g => g.Sum(c => c.CurrentEnrollments))
                    .ToDictionary(g => g.Key, g => g.Sum(c => c.CurrentEnrollments));
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return new Dictionary<string, int>(); }
            }

        // ── DISPLAY HELPER ──
        public void DisplayCourseEntity(CourseEntity c)
            {
            Console.WriteLine($"  [{c.CourseId}] {c.Title} | {c.Category} | {c.DifficultyLevel} | {c.CurrentEnrollments}/{c.MaxCapacity} | Instructor: {c.InstructorName ?? "N/A"}");
            }

        public void DisplayCourseList(List<CourseEntity> courses)
            {
            if (!courses.Any()) { Console.WriteLine("  No courses found."); return; }
            Console.WriteLine($"\n  {"ID",-6}{"Title",-36}{"Category",-20}{"Difficulty",-14}{"Enrolled",-10}Capacity");
            Console.WriteLine("  " + new string('─', 95));
            foreach (var c in courses)
                Console.WriteLine($"  {c.CourseId,-6}{c.Title,-36}{c.Category,-20}{c.DifficultyLevel,-14}{c.CurrentEnrollments,-10}{c.MaxCapacity}");
            }
        }
    }