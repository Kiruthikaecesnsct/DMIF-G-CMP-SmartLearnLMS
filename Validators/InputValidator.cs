using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Week_1.Validators
    {
    // ══════════════════════════════════════════════════════════════════
    //  ASSIGNMENT 8 — PART 2 TASK 1: InputValidator
    // ══════════════════════════════════════════════════════════════════

    /// <summary>
    /// Static utility class that validates raw user input before it reaches
    /// the database layer. All methods return <c>true</c> when valid and
    /// populate <paramref name="errorMessage"/> with a human-readable reason
    /// when invalid.
    /// </summary>
    public static class InputValidator
        {
        // ── Email ──────────────────────────────────────────────────────

        /// <summary>
        /// Validates an email address: requires an @ symbol, a domain name,
        /// and a top-level domain extension.
        /// </summary>
        public static bool ValidateEmail(string email, out string errorMessage)
            {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(email))
                { errorMessage = "Email cannot be empty."; return false; }

            // Simple but reliable regex: local@domain.tld
            var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]{2,}$";
            if (!Regex.IsMatch(email.Trim(), pattern, RegexOptions.IgnoreCase))
                { errorMessage = "Email format is invalid (e.g., user@domain.com)."; return false; }

            if (email.Length > 150)
                { errorMessage = "Email must be 150 characters or fewer."; return false; }

            return true;
            }

        // ── Username ───────────────────────────────────────────────────

        /// <summary>
        /// Validates a username: 3–20 characters, letters/digits/underscore/hyphen only.
        /// </summary>
        public static bool ValidateUsername(string username, out string errorMessage)
            {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(username))
                { errorMessage = "Username cannot be empty."; return false; }

            if (username.Length < 3)
                { errorMessage = "Username must be at least 3 characters."; return false; }

            if (username.Length > 20)
                { errorMessage = "Username must be 20 characters or fewer."; return false; }

            if (!Regex.IsMatch(username, @"^[a-zA-Z0-9_\-]+$"))
                { errorMessage = "Username may only contain letters, digits, underscores (_), and hyphens (-)."; return false; }

            return true;
            }

        // ── Password ───────────────────────────────────────────────────

        /// <summary>
        /// Validates a password: minimum 8 characters; requires at least one
        /// uppercase letter, one lowercase letter, one digit, and one special character.
        /// </summary>
        public static bool ValidatePassword(string password, out string errorMessage)
            {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(password))
                { errorMessage = "Password cannot be empty."; return false; }

            if (password.Length < 8)
                { errorMessage = "Password must be at least 8 characters."; return false; }

            if (!password.Any(char.IsUpper))
                { errorMessage = "Password must contain at least one uppercase letter (A-Z)."; return false; }

            if (!password.Any(char.IsLower))
                { errorMessage = "Password must contain at least one lowercase letter (a-z)."; return false; }

            if (!password.Any(char.IsDigit))
                { errorMessage = "Password must contain at least one digit (0-9)."; return false; }

            var specialChars = "!@#$%^&*()_+-=[]{}|;':\",./<>?";
            if (!password.Any(c => specialChars.Contains(c)))
                { errorMessage = "Password must contain at least one special character (!@#$%^&* etc.)."; return false; }

            return true;
            }

        // ── Progress ───────────────────────────────────────────────────

        /// <summary>
        /// Validates that a progress value is between 0 and 100 (inclusive).
        /// </summary>
        public static bool ValidateProgress(double progress, out string errorMessage)
            {
            errorMessage = string.Empty;

            if (progress < 0)
                { errorMessage = "Progress cannot be negative."; return false; }

            if (progress > 100)
                { errorMessage = "Progress cannot exceed 100%."; return false; }

            return true;
            }

        // ── Course Title ───────────────────────────────────────────────

        /// <summary>
        /// Validates a course title: 5–100 characters, not whitespace-only.
        /// </summary>
        public static bool ValidateCourseTitle(string title, out string errorMessage)
            {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(title))
                { errorMessage = "Course title cannot be empty."; return false; }

            if (title.Trim().Length < 5)
                { errorMessage = "Course title must be at least 5 characters."; return false; }

            if (title.Length > 100)
                { errorMessage = "Course title must be 100 characters or fewer."; return false; }

            return true;
            }

        // ── Role ───────────────────────────────────────────────────────

        /// <summary>Validates that a role is one of Student, Instructor, or Admin.</summary>
        public static bool ValidateRole(string role, out string errorMessage)
            {
            errorMessage = string.Empty;
            if (role != "Student" && role != "Instructor" && role != "Admin")
                { errorMessage = "Role must be Student, Instructor, or Admin."; return false; }
            return true;
            }

        // ── Rating ─────────────────────────────────────────────────────

        /// <summary>Validates a star rating is between 1 and 5.</summary>
        public static bool ValidateRating(int stars, out string errorMessage)
            {
            errorMessage = string.Empty;
            if (stars < 1 || stars > 5)
                { errorMessage = "Rating must be between 1 and 5 stars."; return false; }
            return true;
            }
        }
    }