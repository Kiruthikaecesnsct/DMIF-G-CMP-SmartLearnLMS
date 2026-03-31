using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Week_1.Exceptions
    {
    // ══════════════════════════════════════════════════════════════════
    //  ASSIGNMENT 8 — PART 1 TASK 1: Custom Exception Hierarchy
    // ══════════════════════════════════════════════════════════════════

    // ── Base Enrollment Exception ──────────────────────────────────────
    /// <summary>Base class for all enrollment-related exceptions.</summary>
    public class EnrollmentException : Exception
        {
        public int StudentId { get; }
        public int CourseId { get; }

        public EnrollmentException(string message, int studentId, int courseId)
            : base(message)
            {
            StudentId = studentId;
            CourseId = courseId;
            }

        public EnrollmentException(string message, int studentId, int courseId, Exception inner)
            : base(message, inner)
            {
            StudentId = studentId;
            CourseId = courseId;
            }
        }

    /// <summary>Thrown when a course has no remaining capacity.</summary>
    public class CourseFullException : EnrollmentException
        {
        public int MaxCapacity { get; }
        public int CurrentEnrollments { get; }

        public CourseFullException(int studentId, int courseId, int maxCapacity, int current)
            : base($"Course {courseId} is at full capacity ({current}/{maxCapacity}).",
                   studentId, courseId)
            {
            MaxCapacity = maxCapacity;
            CurrentEnrollments = current;
            }
        }

    /// <summary>Thrown when a student tries to enroll in a course they are already in.</summary>
    public class AlreadyEnrolledException : EnrollmentException
        {
        public AlreadyEnrolledException(int studentId, int courseId)
            : base($"Student {studentId} is already enrolled in course {courseId}.",
                   studentId, courseId)
            { }
        }

    /// <summary>Thrown when a prerequisite course has not been completed.</summary>
    public class PrerequisiteNotMetException : EnrollmentException
        {
        public int PrerequisiteCourseId { get; }

        public PrerequisiteNotMetException(int studentId, int courseId, int prereqId)
            : base($"Student {studentId} has not completed prerequisite course {prereqId} for course {courseId}.",
                   studentId, courseId)
            {
            PrerequisiteCourseId = prereqId;
            }
        }

    // ── User Exceptions ────────────────────────────────────────────────

    /// <summary>Thrown when a username already exists in the system.</summary>
    public class DuplicateUsernameException : Exception
        {
        public string Username { get; }

        public DuplicateUsernameException(string username)
            : base($"Username '{username}' is already taken.")
            {
            Username = username;
            }
        }

    /// <summary>Thrown when an email address already exists in the system.</summary>
    public class DuplicateEmailException : Exception
        {
        public string Email { get; }

        public DuplicateEmailException(string email)
            : base($"Email '{email}' is already registered.")
            {
            Email = email;
            }
        }

    /// <summary>Thrown when login credentials are invalid.</summary>
    public class InvalidCredentialsException : Exception
        {
        public string Username { get; }

        public InvalidCredentialsException(string username)
            : base($"Invalid username or password for '{username}'.")
            {
            Username = username;
            }
        }

    // ── Course Exceptions ──────────────────────────────────────────────

    /// <summary>Thrown when a requested course does not exist.</summary>
    public class CourseNotFoundException : Exception
        {
        public int CourseId { get; }

        public CourseNotFoundException(int courseId)
            : base($"Course with ID {courseId} was not found.")
            {
            CourseId = courseId;
            }

        public CourseNotFoundException(string title)
            : base($"Course '{title}' was not found.") { }
        }

    /// <summary>Thrown when a progress update value is invalid or decreases.</summary>
    public class InvalidProgressUpdateException : Exception
        {
        public int StudentId { get; }
        public int CourseId { get; }
        public double OldProgress { get; }
        public double NewProgress { get; }

        public InvalidProgressUpdateException(int studentId, int courseId,
                                              double oldProgress, double newProgress)
            : base($"Invalid progress update for student {studentId} in course {courseId}: " +
                   $"{oldProgress}% → {newProgress}% (must be 0-100 and not decrease).")
            {
            StudentId = studentId;
            CourseId = courseId;
            OldProgress = oldProgress;
            NewProgress = newProgress;
            }
        }
    }

