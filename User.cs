using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace Week_1
    {
    public abstract class User : IAuditable
        {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime DateRegistered { get; set; }
        public bool IsActive { get; set; }

        // ── IAuditable ──
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public void UpdateModifiedDate() => ModifiedDate = DateTime.Now;

        protected User()
            {
            DateRegistered = DateTime.Now;
            IsActive = true;
            CreatedDate = DateTime.Now;
            ModifiedDate = DateTime.Now;
            }

        protected User(string username, string password, string email)
            {
            Username = username;
            Password = password;
            Email = email;
            DateRegistered = DateTime.Now;
            IsActive = true;
            CreatedDate = DateTime.Now;
            ModifiedDate = DateTime.Now;
            }

        public string GetPasswordForSave() => Password;
        public bool ValidatePassword(string inputPassword) => Password == inputPassword;

        public virtual void DisplayInfo()
            {
            Console.WriteLine($"  Username   : {Username}");
            Console.WriteLine($"  Email      : {Email}");
            Console.WriteLine($"  Registered : {DateRegistered:d}");
            Console.WriteLine($"  Status     : {(IsActive ? "Active" : "Inactive")}");
            Console.WriteLine($"  Created    : {CreatedDate:g}");
            Console.WriteLine($"  Modified   : {ModifiedDate:g}");
            }

        public abstract void DisplayDashboard();
        public abstract string GetUserType();
        }
    }