using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Week_1.Validators
    {
    public static class InputValidator
        {
        public static bool IsValidEmail(string email)
            {
            if (string.IsNullOrWhiteSpace(email)) return false;
            try
                {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email && email.Contains("@") && email.Contains(".");
                }
            catch { return false; }
            }

        public static bool IsValidUsername(string username, out string errorMessage)
            {
            errorMessage = string.Empty;
            if (string.IsNullOrWhiteSpace(username)) { errorMessage = "Username cannot be empty"; return false; }
            if (username.Length < 3) { errorMessage = "Username must be at least 3 characters"; return false; }
            if (username.Length > 20) { errorMessage = "Username cannot exceed 20 characters"; return false; }
            if (!System.Text.RegularExpressions.Regex.IsMatch(username, @"^[a-zA-Z0-9_-]+$"))
                {
                errorMessage = "Username can only contain letters, numbers, underscore, and hyphen";
                return false;
                }
            return true;
            }

        public static bool IsStrongPassword(string password, out string errorMessage)
            {
            errorMessage = string.Empty;
            if (string.IsNullOrWhiteSpace(password)) { errorMessage = "Password cannot be empty"; return false; }
            if (password.Length < 8) { errorMessage = "Password must be at least 8 characters"; return false; }
            if (!password.Any(char.IsUpper)) { errorMessage = "Password must contain at least one uppercase letter"; return false; }
            if (!password.Any(char.IsLower)) { errorMessage = "Password must contain at least one lowercase letter"; return false; }
            if (!password.Any(char.IsDigit)) { errorMessage = "Password must contain at least one number"; return false; }
            if (!password.Any(ch => !char.IsLetterOrDigit(ch))) { errorMessage = "Password must contain at least one special character (!@#$%^&*)"; return false; }
            return true;
            }

        public static bool IsValidProgress(double progress, out string errorMessage)
            {
            errorMessage = string.Empty;
            if (progress < 0) { errorMessage = "Progress cannot be negative"; return false; }
            if (progress > 100) { errorMessage = "Progress cannot exceed 100%"; return false; }
            return true;
            }

        public static bool IsValidCourseTitle(string title, out string errorMessage)
            {
            errorMessage = string.Empty;
            if (string.IsNullOrWhiteSpace(title)) { errorMessage = "Course title cannot be empty"; return false; }
            if (title.Length < 5) { errorMessage = "Course title must be at least 5 characters"; return false; }
            if (title.Length > 100) { errorMessage = "Course title cannot exceed 100 characters"; return false; }
            return true;
            }

        public static bool IsValidEnrollmentDate(DateTime enrollmentDate, out string errorMessage)
            {
            errorMessage = string.Empty;
            if (enrollmentDate > DateTime.Now) { errorMessage = "Enrollment date cannot be in the future"; return false; }
            if (enrollmentDate < DateTime.Now.AddYears(-5)) { errorMessage = "Enrollment date seems too old (more than 5 years ago)"; return false; }
            return true;
            }
        }
    }
