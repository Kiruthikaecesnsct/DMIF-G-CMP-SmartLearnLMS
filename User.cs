using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//public class User
//{

//}



//public class User
//{
//    public string Username { get; set; }
//    public string Password { get; set; }
//    public string Email { get; set; }
//    public string Role { get; set; }
//}




public class User
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    //public string Role { get; set; }
    public DateTime DateRegistered { get; set; }
    public bool IsActive { get; set; }

    public User(string username, string password, string email/*, string role*/)
    {
        Username = username;
        Password = password;
        Email = email;
        //Role = role;
        DateRegistered = DateTime.Now;
        IsActive = true;
    }

    public void DisplayInfo()
    {
        Console.WriteLine($"Username: {Username}");
        Console.WriteLine($"Email: {Email}");
        Console.WriteLine($"Registered: {DateRegistered}");
        Console.WriteLine($"Active: {IsActive}");
    }
    public bool ValidatePassword(string inputPassword)
    {
        return Password == inputPassword;
    }


}

