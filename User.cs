using System;
using System.Collections.Generic;

namespace Week_1
{
    public abstract class User
    {
        public string Username { get; set; }
        private string Password { get; set; }
        public string Email { get; set; }

        public User(string username, string password, string email)
        {
            Username = username;
            Password = password;
            Email = email;
        }

        public bool ValidatePassword(string inputPassword)
        {
            return Password == inputPassword;
        }

        // Abstract methods — every user type MUST implement these
        public abstract void DisplayInfo();
        public abstract void DisplayDashboard();
        public abstract string GetUserType();
    }
}