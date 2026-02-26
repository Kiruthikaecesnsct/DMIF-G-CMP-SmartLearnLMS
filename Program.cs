using System;
using System.Collections.Generic;

namespace SmartLearnLMS
{
    class Program
    {
        // ── Session State ──────────────────────────────────────
        static bool isLoggedIn = false;
        static string currentUser = "";
        static string currentRole = "";

        // ── Data Storage (Dictionary approach - Week 1) ────────
        static Dictionary<string, string> users = new Dictionary<string, string>();
        static Dictionary<string, string> emails = new Dictionary<string, string>();
        static Dictionary<string, string> roles = new Dictionary<string, string>();

        // ── Course Data ────────────────────────────────────────
        static string[] courses = {
            "C# Programming Fundamentals",
            "Introduction to SQL Server",
            "Web Development with ASP.NET Core",
            "Advanced C# Techniques",
            "Database Design Principles",
            "RESTful API Development",
            "Entity Framework Core",
            "Front-End Development with React",
            "Cloud Computing with Azure",
            "Software Testing and Quality Assurance"
        };

        // ══════════════════════════════════════════════════════
        //  ENTRY POINT
        // ══════════════════════════════════════════════════════
        static void Main(string[] args)
        {
            bool running = true;

            while (running)
            {
                if (!isLoggedIn)
                {
                    running = ShowLoginMenu();
                }
                else
                {
                    switch (currentRole)
                    {
                        case "Student": ShowStudentDashboard(); break;
                        case "Instructor": ShowInstructorDashboard(); break;
                        case "Admin": ShowAdminDashboard(); break;
                    }
                }
            }
        }

        // ══════════════════════════════════════════════════════
        //  MAIN LOGIN MENU
        // ══════════════════════════════════════════════════════
        static bool ShowLoginMenu()
        {
            Console.Clear();
            Console.WriteLine("================================");
            Console.WriteLine("    Welcome to SmartLearn LMS   ");
            Console.WriteLine("================================");
            Console.WriteLine("  1. Login");
            Console.WriteLine("  2. Register");
            Console.WriteLine("  3. Browse Courses");
            Console.WriteLine("  4. Exit");
            Console.WriteLine("================================");
            Console.Write("  Enter your choice: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1": Login(); break;
                case "2": RegisterUser(); break;
                case "3": BrowseCourses(); break;
                case "4":
                    Console.WriteLine("\n  Thank you for using SmartLearn!");
                    return false; // stop the app
                default:
                    Console.WriteLine("  Invalid choice. Please enter 1, 2, 3, or 4.");
                    Console.ReadKey();
                    break;
            }
            return true;
        }

        // ══════════════════════════════════════════════════════
        //  REGISTER
        // ══════════════════════════════════════════════════════
        static void RegisterUser()
        {
            Console.Clear();
            Console.WriteLine("================================");
            Console.WriteLine("           REGISTER             ");
            Console.WriteLine("================================");

            // Get and validate username
            Console.Write("  Enter Username : ");
            string username = Console.ReadLine();
            if (!ValidateUsername(username)) { Console.ReadKey(); return; }

            // Get and validate email
            Console.Write("  Enter Email    : ");
            string email = Console.ReadLine();
            if (!ValidateEmail(email)) { Console.ReadKey(); return; }

            // Check duplicate email
            if (emails.ContainsValue(email))
            {
                Console.WriteLine("  Email already registered!");
                Console.ReadKey();
                return;
            }

            // Get and validate password
            Console.Write("  Enter Password : ");
            string password = Console.ReadLine();
            if (!ValidatePassword(password)) { Console.ReadKey(); return; }

            // Get and validate role
            Console.Write("  Enter Role (Student/Instructor/Admin): ");
            string role = Console.ReadLine();
            if (role != "Student" && role != "Instructor" && role != "Admin")
            {
                Console.WriteLine("  Invalid role. Must be Student, Instructor, or Admin.");
                Console.ReadKey();
                return;
            }

            // Store in all three dictionaries
            users.Add(username, password);
            emails.Add(username, email);
            roles.Add(username, role);

            Console.WriteLine($"\n  Registration successful! Welcome, {username}!");
            Console.ReadKey();
        }

        // ══════════════════════════════════════════════════════
        //  VALIDATION METHODS
        // ══════════════════════════════════════════════════════
        static bool ValidateUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username) || username.Length < 3)
            {
                Console.WriteLine("  Username must be at least 3 characters.");
                return false;
            }
            if (username.Contains(" "))
            {
                Console.WriteLine("  Username cannot contain spaces.");
                return false;
            }
            if (users.ContainsKey(username))
            {
                Console.WriteLine("  Username already exists!");
                return false;
            }
            return true;
        }

        static bool ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            {
                Console.WriteLine("  Password must be at least 6 characters.");
                return false;
            }
            return true;
        }

        static bool ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@") || !email.Contains("."))
            {
                Console.WriteLine("  Invalid email format.");
                return false;
            }
            return true;
        }

        // ══════════════════════════════════════════════════════
        //  LOGIN
        // ══════════════════════════════════════════════════════
        static void Login()
        {
            Console.Clear();
            Console.WriteLine("================================");
            Console.WriteLine("            LOGIN               ");
            Console.WriteLine("================================");

            Console.Write("  Enter Username : ");
            string username = Console.ReadLine();

            // Check if username exists
            if (!users.ContainsKey(username))
            {
                Console.WriteLine("  User not found!");
                Console.ReadKey();
                return;
            }

            Console.Write("  Enter Password : ");
            string password = Console.ReadLine();

            // Verify password
            if (users[username] != password)
            {
                Console.WriteLine("  Incorrect password!");
                Console.ReadKey();
                return;
            }

            // Set session state
            isLoggedIn = true;
            currentUser = username;
            currentRole = roles[username];

            Console.WriteLine($"\n  Welcome back, {currentUser}! Role: {currentRole}");
            Console.ReadKey();
        }

        // ══════════════════════════════════════════════════════
        //  LOGOUT
        // ══════════════════════════════════════════════════════
        static void Logout()
        {
            isLoggedIn = false;
            currentUser = "";
            currentRole = "";
            Console.WriteLine("\n  Logged out successfully.");
            Console.ReadKey();
        }

        // ══════════════════════════════════════════════════════
        //  COURSE BROWSING
        // ══════════════════════════════════════════════════════
        static void DisplayAllCourses()
        {
            Console.WriteLine("\n  === Available Courses ===");
            for (int i = 0; i < courses.Length; i++)
            {
                Console.WriteLine($"  {i + 1}. {courses[i]}");
            }
            Console.WriteLine();
        }

        static void SearchCourses()
        {
            Console.Write("  Enter search keyword: ");
            string keyword = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(keyword))
            {
                Console.WriteLine("  Keyword cannot be empty.");
                return;
            }

            Console.WriteLine($"\n  === Search Results for '{keyword}' ===");
            int found = 0;

            for (int i = 0; i < courses.Length; i++)
            {
                if (courses[i].ToLower().Contains(keyword.ToLower()))
                {
                    found++;
                    Console.WriteLine($"  {found}. {courses[i]}");
                }
            }

            if (found == 0)
                Console.WriteLine($"  No courses found matching '{keyword}'.");
            else
                Console.WriteLine($"\n  Found {found} matching course(s).");
        }

        static void BrowseCourses()
        {
            bool browsing = true;
            while (browsing)
            {
                Console.Clear();
                Console.WriteLine("================================");
                Console.WriteLine("        BROWSE COURSES          ");
                Console.WriteLine("================================");
                DisplayAllCourses();
                Console.WriteLine("  1. Search by keyword");
                Console.WriteLine("  2. Return to menu");
                Console.Write("  Your choice: ");
                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    SearchCourses();
                    Console.Write("\n  Search again? (y/n): ");
                    string again = Console.ReadLine();
                    if (again?.ToLower() != "y")
                        browsing = false;
                }
                else if (choice == "2")
                {
                    browsing = false;
                }
                else
                {
                    Console.WriteLine("  Invalid choice.");
                    Console.ReadKey();
                }
            }
        }

        // ══════════════════════════════════════════════════════
        //  DASHBOARDS
        // ══════════════════════════════════════════════════════
        static void ShowStudentDashboard()
        {
            bool open = true;
            while (open)
            {
                Console.Clear();
                Console.WriteLine("================================");
                Console.WriteLine($"  STUDENT DASHBOARD");
                Console.WriteLine($"  Welcome, {currentUser}!");
                Console.WriteLine("================================");
                Console.WriteLine("  1. Browse Courses");
                Console.WriteLine("  2. My Enrolled Courses");
                Console.WriteLine("  3. My Progress");
                Console.WriteLine("  4. Take Quiz");
                Console.WriteLine("  5. Logout");
                Console.WriteLine("================================");
                Console.Write("  Your choice: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": BrowseCourses(); break;
                    case "2": ComingSoon(); break;
                    case "3": ComingSoon(); break;
                    case "4": ComingSoon(); break;
                    case "5": Logout(); open = false; break;
                    default:
                        Console.WriteLine("  Invalid choice.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void ShowInstructorDashboard()
        {
            bool open = true;
            while (open)
            {
                Console.Clear();
                Console.WriteLine("================================");
                Console.WriteLine($"  INSTRUCTOR DASHBOARD");
                Console.WriteLine($"  Welcome, {currentUser}!");
                Console.WriteLine("================================");
                Console.WriteLine("  1. My Courses");
                Console.WriteLine("  2. Create New Course");
                Console.WriteLine("  3. View Students");
                Console.WriteLine("  4. Grade Assignments");
                Console.WriteLine("  5. Browse Courses");
                Console.WriteLine("  6. Logout");
                Console.WriteLine("================================");
                Console.Write("  Your choice: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": ComingSoon(); break;
                    case "2": ComingSoon(); break;
                    case "3": ComingSoon(); break;
                    case "4": ComingSoon(); break;
                    case "5": BrowseCourses(); break;
                    case "6": Logout(); open = false; break;
                    default:
                        Console.WriteLine("  Invalid choice.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void ShowAdminDashboard()
        {
            bool open = true;
            while (open)
            {
                Console.Clear();
                Console.WriteLine("================================");
                Console.WriteLine($"  ADMIN DASHBOARD");
                Console.WriteLine($"  Welcome, {currentUser}!");
                Console.WriteLine("================================");
                Console.WriteLine("  1. Manage Users");
                Console.WriteLine("  2. Manage Courses");
                Console.WriteLine("  3. View All Users");
                Console.WriteLine("  4. Reports");
                Console.WriteLine("  5. Settings");
                Console.WriteLine("  6. Browse Courses");
                Console.WriteLine("  7. Logout");
                Console.WriteLine("================================");
                Console.Write("  Your choice: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": ComingSoon(); break;
                    case "2": ComingSoon(); break;
                    case "3": DisplayAllUsers(); break;
                    case "4": ComingSoon(); break;
                    case "5": ComingSoon(); break;
                    case "6": BrowseCourses(); break;
                    case "7": Logout(); open = false; break;
                    default:
                        Console.WriteLine("  Invalid choice.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        // ══════════════════════════════════════════════════════
        //  UTILITY
        // ══════════════════════════════════════════════════════
        static void ComingSoon()
        {
            Console.WriteLine("\n  Feature coming in Week 2. Press any key to continue...");
            Console.ReadKey();
        }

        static void DisplayAllUsers()
        {
            Console.WriteLine("\n  === All Registered Users ===");
            if (users.Count == 0)
            {
                Console.WriteLine("  No users registered yet.");
                Console.ReadKey();
                return;
            }
            foreach (string username in users.Keys)
            {
                Console.WriteLine($"  Username : {username}");
                Console.WriteLine($"  Email    : {emails[username]}");
                Console.WriteLine($"  Role     : {roles[username]}");
                Console.WriteLine("  ----------------------------");
            }
            Console.ReadKey();
        }
    }
}
