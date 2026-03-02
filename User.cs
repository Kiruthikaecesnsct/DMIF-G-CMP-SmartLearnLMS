using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace Week_1
{
    public abstract class User
    {
        public string Username { get; set; }

        // Store as plain property so JSON can serialize it
        public string Email { get; set; }

        // Password stored as plain property for JSON (validated on registration)
        public string Password { get; set; }

        public DateTime DateRegistered { get; set; }
        public bool IsActive { get; set; }

        // ── Parameterless constructor for JSON ──
        protected User()
        {
            DateRegistered = DateTime.Now;
            IsActive = true;
        }

        protected User(string username, string password, string email)
        {
            Username = username;
            Password = password;
            Email = email;
            DateRegistered = DateTime.Now;
            IsActive = true;
        }

        public string GetPasswordForSave() => Password;

        public bool ValidatePassword(string inputPassword) => Password == inputPassword;

        public virtual void DisplayInfo()
        {
            Console.WriteLine($"  Username   : {Username}");
            Console.WriteLine($"  Email      : {Email}");
            Console.WriteLine($"  Registered : {DateRegistered:d}");
            Console.WriteLine($"  Status     : {(IsActive ? "Active" : "Inactive")}");
        }

        public abstract void DisplayDashboard();
        public abstract string GetUserType();
    }
}