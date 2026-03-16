using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace Week_1.Services
    {
    /// <summary>
    /// Assignment 6 — Part 1: ADO.NET fundamentals.
    /// Direct SQL operations using SqlConnection / SqlCommand / SqlDataReader.
    /// All user inputs use parameterised queries to prevent SQL injection.
    /// </summary>
    public class AdoNetService
        {
        // ── Task 1: Connection string stored as a constant ──
        private const string ConnectionString =
            @"Server=localhost\SQLEXPRESS;Database=SmartLearnDB;Trusted_Connection=True;TrustServerCertificate=True;";

        // ══════════════════════════════════════════════════════
        //  TASK 1: Test Connection
        // ══════════════════════════════════════════════════════
        public bool TestConnection()
            {
            try
                {
                using var conn = new SqlConnection(ConnectionString);
                conn.Open();
                Console.WriteLine($"  ✓ Connected to: {conn.DataSource} | DB: {conn.Database}");
                conn.Close();
                return true;
                }
            catch (SqlException ex)
                {
                Console.WriteLine($"  ✗ SQL Connection failed: {ex.Message}");
                return false;
                }
            catch (Exception ex)
                {
                Console.WriteLine($"  ✗ Connection error: {ex.Message}");
                return false;
                }
            }

        // ══════════════════════════════════════════════════════
        //  TASK 2: READ DATA WITH SqlDataReader
        // ══════════════════════════════════════════════════════

        /// <summary>Retrieve all students from Users table.</summary>
        public List<UserEntity> GetAllStudentsAdo()
            {
            var list = new List<UserEntity>();
            try
                {
                using var conn = new SqlConnection(ConnectionString);
                conn.Open();
                using var cmd = new SqlCommand(
                    "SELECT UserId, Username, Email, UserType, CreatedDate, IsActive, LastLoginDate " +
                    "FROM Users WHERE UserType = 'Student' ORDER BY Username", conn);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                    {
                    list.Add(new UserEntity
                        {
                        UserId = reader.GetInt32(0),
                        Username = reader.GetString(1),
                        Email = reader.GetString(2),
                        UserType = reader.GetString(3),
                        CreatedDate = reader.GetDateTime(4),
                        IsActive = reader.GetBoolean(5),
                        LastLoginDate = reader.IsDBNull(6) ? null : reader.GetDateTime(6)
                        });
                    }
                }
            catch (SqlException ex) { Console.WriteLine($"  ✗ DB error (GetAllStudents): {ex.Message}"); }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); }
            return list;
            }

        /// <summary>Retrieve all courses from Courses table.</summary>
        public List<CourseEntity> GetAllCoursesAdo()
            {
            var list = new List<CourseEntity>();
            try
                {
                using var conn = new SqlConnection(ConnectionString);
                conn.Open();
                using var cmd = new SqlCommand(
                    "SELECT CourseId, Title, Category, DifficultyLevel, CurrentEnrollments, MaxCapacity, InstructorName " +
                    "FROM Courses ORDER BY Title", conn);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                    {
                    list.Add(new CourseEntity
                        {
                        CourseId = reader.GetInt32(0),
                        Title = reader.GetString(1),
                        Category = reader.GetString(2),
                        DifficultyLevel = reader.GetString(3),
                        CurrentEnrollments = reader.GetInt32(4),
                        MaxCapacity = reader.GetInt32(5),
                        InstructorName = reader.IsDBNull(6) ? "" : reader.GetString(6)
                        });
                    }
                }
            catch (SqlException ex) { Console.WriteLine($"  ✗ DB error (GetAllCourses): {ex.Message}"); }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); }
            return list;
            }

        /// <summary>Find a specific user by UserId using parameters.</summary>
        public UserEntity FindUserById(int userId)
            {
            try
                {
                using var conn = new SqlConnection(ConnectionString);
                conn.Open();
                using var cmd = new SqlCommand(
                    "SELECT UserId, Username, Email, UserType, CreatedDate, IsActive, LastLoginDate " +
                    "FROM Users WHERE UserId = @UserId", conn);
                cmd.Parameters.AddWithValue("@UserId", userId);
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                    {
                    return new UserEntity
                        {
                        UserId = reader.GetInt32(0),
                        Username = reader.GetString(1),
                        Email = reader.GetString(2),
                        UserType = reader.GetString(3),
                        CreatedDate = reader.GetDateTime(4),
                        IsActive = reader.GetBoolean(5),
                        LastLoginDate = reader.IsDBNull(6) ? null : reader.GetDateTime(6)
                        };
                    }
                }
            catch (SqlException ex) { Console.WriteLine($"  ✗ DB error (FindUserById): {ex.Message}"); }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); }
            return null;
            }

        /// <summary>Get all enrollments for a specific student.</summary>
        public List<EnrollmentEntity> GetEnrollmentsByStudent(int studentId)
            {
            var list = new List<EnrollmentEntity>();
            try
                {
                using var conn = new SqlConnection(ConnectionString);
                conn.Open();
                using var cmd = new SqlCommand(
                    "SELECT e.EnrollmentId, e.StudentId, e.CourseId, e.EnrolledDate, " +
                    "       e.ProgressPercent, e.Status, c.Title " +
                    "FROM Enrollments e " +
                    "INNER JOIN Courses c ON e.CourseId = c.CourseId " +
                    "WHERE e.StudentId = @StudentId ORDER BY e.EnrolledDate DESC", conn);
                cmd.Parameters.AddWithValue("@StudentId", studentId);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                    {
                    var enr = new EnrollmentEntity
                        {
                        EnrollmentId = reader.GetInt32(0),
                        StudentId = reader.GetInt32(1),
                        CourseId = reader.GetInt32(2),
                        EnrolledDate = reader.GetDateTime(3),
                        ProgressPercent = reader.GetInt32(4),
                        Status = reader.GetString(5)
                        };
                    // Store course title in a temp way for display
                    Console.WriteLine($"  • [{enr.CourseId}] {reader.GetString(6)} — {enr.ProgressPercent}% ({enr.Status})");
                    list.Add(enr);
                    }
                }
            catch (SqlException ex) { Console.WriteLine($"  ✗ DB error (GetEnrollments): {ex.Message}"); }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); }
            return list;
            }

        // ══════════════════════════════════════════════════════
        //  TASK 3: INSERT DATA
        // ══════════════════════════════════════════════════════

        /// <summary>Insert a new student using parameterised query.</summary>
        public bool InsertStudentAdo(string username, string email, string passwordHash)
            {
            try
                {
                using var conn = new SqlConnection(ConnectionString);
                conn.Open();
                using var cmd = new SqlCommand(
                    "INSERT INTO Users (Username, Email, PasswordHash, UserType, CreatedDate, IsActive) " +
                    "VALUES (@Username, @Email, @PasswordHash, 'Student', GETDATE(), 1)", conn);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
                int rows = cmd.ExecuteNonQuery();
                if (rows > 0) Console.WriteLine($"  ✓ Student '{username}' inserted. Rows affected: {rows}");
                return rows > 0;
                }
            catch (SqlException ex) { Console.WriteLine($"  ✗ DB error (InsertStudent): {ex.Message}"); return false; }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return false; }
            }

        /// <summary>Insert a new course using parameterised query.</summary>
        public bool InsertCourseAdo(string title, string category, string difficulty, int maxCapacity, string instructorName)
            {
            try
                {
                using var conn = new SqlConnection(ConnectionString);
                conn.Open();
                using var cmd = new SqlCommand(
                    "INSERT INTO Courses (Title, Category, DifficultyLevel, MaxCapacity, CurrentEnrollments, InstructorName, CreatedDate) " +
                    "VALUES (@Title, @Category, @Difficulty, @MaxCapacity, 0, @InstructorName, GETDATE())", conn);
                cmd.Parameters.AddWithValue("@Title", title);
                cmd.Parameters.AddWithValue("@Category", category);
                cmd.Parameters.AddWithValue("@Difficulty", difficulty);
                cmd.Parameters.AddWithValue("@MaxCapacity", maxCapacity);
                cmd.Parameters.AddWithValue("@InstructorName", instructorName);
                int rows = cmd.ExecuteNonQuery();
                if (rows > 0) Console.WriteLine($"  ✓ Course '{title}' inserted. Rows affected: {rows}");
                return rows > 0;
                }
            catch (SqlException ex) { Console.WriteLine($"  ✗ DB error (InsertCourse): {ex.Message}"); return false; }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return false; }
            }

        /// <summary>Insert a new enrollment record.</summary>
        public bool InsertEnrollmentAdo(int studentId, int courseId)
            {
            try
                {
                using var conn = new SqlConnection(ConnectionString);
                conn.Open();
                using var cmd = new SqlCommand(
                    "INSERT INTO Enrollments (StudentId, CourseId, EnrolledDate, ProgressPercent, Status) " +
                    "VALUES (@StudentId, @CourseId, GETDATE(), 0, 'Active')", conn);
                cmd.Parameters.AddWithValue("@StudentId", studentId);
                cmd.Parameters.AddWithValue("@CourseId", courseId);
                int rows = cmd.ExecuteNonQuery();
                if (rows > 0) Console.WriteLine($"  ✓ Enrollment added. Rows affected: {rows}");
                return rows > 0;
                }
            catch (SqlException ex) { Console.WriteLine($"  ✗ DB error (InsertEnrollment): {ex.Message}"); return false; }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return false; }
            }

        // ══════════════════════════════════════════════════════
        //  TASK 4: UPDATE & DELETE — ExecuteNonQuery
        // ══════════════════════════════════════════════════════

        /// <summary>Update a student's progress percentage.</summary>
        public bool UpdateProgressAdo(int studentId, int courseId, int newProgress)
            {
            if (newProgress < 0 || newProgress > 100)
                { Console.WriteLine("  ✗ Progress must be 0-100."); return false; }

            try
                {
                using var conn = new SqlConnection(ConnectionString);
                conn.Open();
                using var cmd = new SqlCommand(
                    "UPDATE Enrollments SET ProgressPercent = @Progress, " +
                    "Status = CASE WHEN @Progress = 100 THEN 'Completed' ELSE Status END " +
                    "WHERE StudentId = @StudentId AND CourseId = @CourseId", conn);
                cmd.Parameters.AddWithValue("@Progress", newProgress);
                cmd.Parameters.AddWithValue("@StudentId", studentId);
                cmd.Parameters.AddWithValue("@CourseId", courseId);
                int rows = cmd.ExecuteNonQuery();
                Console.WriteLine(rows > 0
                    ? $"  ✓ Progress updated to {newProgress}%. Rows affected: {rows}"
                    : "  ✗ No enrollment found to update.");
                return rows > 0;
                }
            catch (SqlException ex) { Console.WriteLine($"  ✗ DB error (UpdateProgress): {ex.Message}"); return false; }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return false; }
            }

        /// <summary>Update a course's current enrollment count.</summary>
        public bool UpdateCourseEnrollmentCountAdo(int courseId, int newCount)
            {
            try
                {
                using var conn = new SqlConnection(ConnectionString);
                conn.Open();
                using var cmd = new SqlCommand(
                    "UPDATE Courses SET CurrentEnrollments = @Count WHERE CourseId = @CourseId", conn);
                cmd.Parameters.AddWithValue("@Count", newCount);
                cmd.Parameters.AddWithValue("@CourseId", courseId);
                int rows = cmd.ExecuteNonQuery();
                Console.WriteLine(rows > 0
                    ? $"  ✓ Enrollment count updated. Rows affected: {rows}"
                    : "  ✗ Course not found.");
                return rows > 0;
                }
            catch (SqlException ex) { Console.WriteLine($"  ✗ DB error (UpdateCourseCount): {ex.Message}"); return false; }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return false; }
            }

        /// <summary>Delete an enrollment record.</summary>
        public bool DeleteEnrollmentAdo(int enrollmentId)
            {
            try
                {
                using var conn = new SqlConnection(ConnectionString);
                conn.Open();
                using var cmd = new SqlCommand(
                    "DELETE FROM Enrollments WHERE EnrollmentId = @EnrollmentId", conn);
                cmd.Parameters.AddWithValue("@EnrollmentId", enrollmentId);
                int rows = cmd.ExecuteNonQuery();
                Console.WriteLine(rows > 0
                    ? $"  ✓ Enrollment deleted. Rows affected: {rows}"
                    : "  ✗ Enrollment not found.");
                return rows > 0;
                }
            catch (SqlException ex) { Console.WriteLine($"  ✗ DB error (DeleteEnrollment): {ex.Message}"); return false; }
            catch (Exception ex) { Console.WriteLine($"  ✗ Error: {ex.Message}"); return false; }
            }

        // ══════════════════════════════════════════════════════
        //  DISPLAY HELPERS
        // ══════════════════════════════════════════════════════
        public void DisplayStudents(List<UserEntity> students)
            {
            if (students.Count == 0) { Console.WriteLine("  No students found."); return; }
            Console.WriteLine($"\n  {"ID",-6}{"Username",-22}{"Email",-32}{"Created",-14}Active");
            Console.WriteLine("  " + new string('─', 80));
            foreach (var s in students)
                Console.WriteLine($"  {s.UserId,-6}{s.Username,-22}{s.Email,-32}{s.CreatedDate:d,-14}{(s.IsActive ? "✓" : "✗")}");
            }

        public void DisplayCourses(List<CourseEntity> courses)
            {
            if (courses.Count == 0) { Console.WriteLine("  No courses found."); return; }
            Console.WriteLine($"\n  {"ID",-6}{"Title",-36}{"Category",-20}{"Difficulty",-14}{"Enrolled",-10}Capacity");
            Console.WriteLine("  " + new string('─', 95));
            foreach (var c in courses)
                Console.WriteLine($"  {c.CourseId,-6}{c.Title,-36}{c.Category,-20}{c.DifficultyLevel,-14}{c.CurrentEnrollments,-10}{c.MaxCapacity}");
            }
        }
    }