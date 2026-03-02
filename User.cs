using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Week_1
{
    // Abstract: A generic "User" doesn't exist in SmartLearn.
    // Everyone is a Student, Instructor, or Admin.
    public abstract class User : IReportable
    {
        public string Username { get; set; }

        // Validated Email property
        private string _email;
        public string Email
        {
            get => _email;
            set
            {
                if (string.IsNullOrWhiteSpace(value) || !value.Contains("@"))
                {
                    Console.WriteLine("✗ Invalid email — must contain '@'");
                    return;
                }
                _email = value;
            }
        }

        // Validated Password property (min 8 chars, at least 1 number)
        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                if (string.IsNullOrWhiteSpace(value) || value.Length < 8 || !Regex.IsMatch(value, @"\d"))
                {
                    Console.WriteLine("✗ Password must be at least 8 characters and contain at least 1 number");
                    return;
                }
                _password = value;
            }
        }

        public string Role { get; set; }
        public DateTime DateRegistered { get; set; }
        public bool IsActive { get; set; }

        private List<string> _notificationHistory = new List<string>();

        protected User(string username, string password, string email, string role)
        {
            Username = username;
            _password = password;   // Set directly to bypass constructor-time validation
            _email = email;
            Role = role;
            DateRegistered = DateTime.Now;
            IsActive = true;
        }

        // ── Abstract: every subclass MUST implement these ─────────────────────
        public abstract void DisplayDashboard();
        public abstract string GetUserType();

        // ── Shared concrete methods ────────────────────────────────────────────
        public void DisplayInfo()
        {
            Console.WriteLine($"  Username   : {Username}");
            Console.WriteLine($"  Email      : {_email}");
            Console.WriteLine($"  Role       : {GetUserType()}");
            Console.WriteLine($"  Registered : {DateRegistered:dd MMM yyyy}");
            Console.WriteLine($"  Active     : {IsActive}");
        }

        public bool ValidatePassword(string inputPassword) => _password == inputPassword;

        // ── Notification support (shared by all users) ────────────────────────
        public void SendNotification(string message)
        {
            _notificationHistory.Add($"[{DateTime.Now:HH:mm}] {message}");
            Console.WriteLine($"  🔔 Notification: {message}");
        }

        public List<string> GetNotificationHistory() => _notificationHistory;

        // ── IReportable ───────────────────────────────────────────────────────
        public virtual string GenerateReport()
        {
            return $"Report | User: {Username} | Role: {GetUserType()} | Registered: {DateRegistered:dd MMM yyyy} | Active: {IsActive}";
        }

        public void DisplayReport()
        {
            Console.WriteLine("\n--- User Report ---");
            Console.WriteLine(GenerateReport());
        }
    }
}