using System;

namespace Week_1
{
    public abstract class User
    {
        public string Username { get; set; }

        private string _email;
        public string Email
        {
            get => _email;
            set
            {
                if (string.IsNullOrWhiteSpace(value) || !value.Contains("@"))
                {
                    Console.WriteLine("  ✗ Invalid email - must contain @");
                    return;
                }
                _email = value;
            }
        }

        private string _password;
        public string Password
        {
            get => _password;
            private set
            {
                if (value.Length < 8)
                {
                    Console.WriteLine("  ✗ Password must be at least 8 characters");
                    return;
                }
                if (!value.Any(char.IsDigit))
                {
                    Console.WriteLine("  ✗ Password must contain at least 1 number");
                    return;
                }
                _password = value;
            }
        }

        public DateTime DateRegistered { get; private set; }
        public bool IsActive { get; set; }

        protected User(string username, string password, string email)
        {
            Username = username;
            _password = password;   // direct set to bypass validation for seeded data
            _email = email;
            DateRegistered = DateTime.Now;
            IsActive = true;
        }

        public bool ValidatePassword(string inputPassword) => _password == inputPassword;

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