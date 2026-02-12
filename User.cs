//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

////public class User
////{

////}



////public class User
////{
////    public string Username { get; set; }
////    public string Password { get; set; }
////    public string Email { get; set; }
////    public string Role { get; set; }
////}




//public /*abstract*/ class User
//{
//    public string Username { get; set; }
//    public string Password { get; set; }
//    public string Email { get; set; }
//    //public string Role { get; set; }

//    public DateTime DateRegistered { get; set; }
//    public bool IsActive { get; set; }

//    public string Coursename { get; set; }



//    public User(string username, string password, string email /*string role*/)
//    {
//        Username = username;
//        Password = password;
//        Email = email;
//        //Role = role;
//        DateRegistered = DateTime.Now;
//        IsActive = false;
//    }
//    //public abstract void DisplayDashboard();
//    //public abstract string GetUserType();
//    public void DisplayInfo()
//    {
//        Console.WriteLine($"Username: {Username}");
//        Console.WriteLine($"Email: {Email}");
//        Console.WriteLine($"Registered: {DateRegistered}");
//        Console.WriteLine($"Active: {IsActive}");
//    }
//    public bool ValidatePassword(string inputPassword)
//    {
//        return Password == inputPassword;
//    }


//}


using System;

namespace Week_1
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
        public DateTime DateRegistered { get; set; }

        public User(string username, string password, string email, string role)
        {
            Username = username;
            Password = password;
            Email = email;
            Role = role;
            IsActive = true;
            DateRegistered = DateTime.Now;
        }

        public void DisplayInfo()
        {
            Console.WriteLine($"Username: {Username}");
            Console.WriteLine($"Email: {Email}");
            Console.WriteLine($"Role: {Role}");
            Console.WriteLine($"Status: {(IsActive ? "Active" : "Deactivated")}");
            Console.WriteLine($"Registered: {DateRegistered.ToShortDateString()}");
        }

        public bool ValidatePassword(string inputPassword) => Password == inputPassword;

        public void ChangePassword(string newPassword)
        {
            Password = newPassword;
            Console.WriteLine("✓ Password updated successfully.");
        }

        public void Deactivate()
        {
            IsActive = false;
            Console.WriteLine("Account deactivated.");
        }
    }
}
