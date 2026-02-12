////string userName;
////int age;
////Console.WriteLine("Enter your name:");
////userName = Console.ReadLine();
////Console.WriteLine("Hello, " + userName);


////Arithmetic Operators
////int a ;
////int b;

////Console.WriteLine("Enter the value of first number");
////a=int.Parse(Console.ReadLine());

////Console.WriteLine("Enter the value of Second number");
////b = int.Parse(Console.ReadLine());
////Console.WriteLine($"{a} + {b} = {a + b}");    // 22
////Console.WriteLine($"{a} - {b} = {a - b}");    // 12
////Console.WriteLine($"{a} * {b} = {a * b}");    // 85
////Console.WriteLine($"{a} / {b} = {a / b}");    // 3   (integer division)
////Console.WriteLine($"{a} % {b} = {a % b}");    // 2   (remainder)


//////Comparison Operators & Logical operators
////int age = 17;
////bool hasId = true;

////Console.WriteLine(age >= 18);                // false
////Console.WriteLine(age == 18);                // false
////Console.WriteLine(age != 20);                // true

////// Logical operators
////Console.WriteLine(age >= 13 && age <= 19);   // true  (teenager)
////Console.WriteLine(age < 13 || hasId);        // true
////Console.WriteLine(!hasId);                   // false

//////Even & Odd check

////int number = 42;

////if (number % 2 == 0)
////{
////    Console.WriteLine($"{number} is EVEN");
////}
////else
////{
////    Console.WriteLine($"{number} is ODD");
////}
////string type = (number % 2 == 0) ? "even" : "odd";
////Console.WriteLine(type);


//////Grade evaluation with switchcase
////Console.Write("Enter grade letter (A/B/C/D/F): ");
////string grade = Console.ReadLine().ToUpper();

////switch (grade)
////{
////    case "A":
////        Console.WriteLine("Excellent! Keep it up.");
////        break;
////    case "B":
////        Console.WriteLine("Good work.");
////        break;
////    case "C":
////        Console.WriteLine("Satisfactory.");
////        break;
////    case "D":
////        Console.WriteLine("Pass – needs improvement.");
////        break;
////    case "F":
////        Console.WriteLine("Failed. Try harder next time.");
////        break;
////    default:
////        Console.WriteLine("Invalid grade entered.");
////        break;
////}


////User student1 = new User();
////student1.Username = "alice123";
////student1.Password = "pass123";
////student1.Email = "alice@email.com";
////student1.Role = "Student";

////User student1 = new User("alice123", "pass123", "alice@email.com", "Student");
////student1.DisplayInfo();

////using Week_1;

////class Program
////{
////    static void Main(string[] args)
////    {
////        // Example 1: Creating a single user
////        Console.WriteLine("=== Example 1: Creating a User ===");
////        User student1 = new User("alice123", "pass123", "alice@email.com", "Student");
////        student1.DisplayInfo();
////        Console.WriteLine();

////        // Example 2: Testing password validation
////        Console.WriteLine("=== Example 2: Password Validation ===");
////        Console.WriteLine("Enter password:");
////        string input = Console.ReadLine();

////        if (student1.ValidatePassword(input))
////        {
////            Console.WriteLine("Correct!");
////        }
////        else
////        {
////            Console.WriteLine("Wrong password!");
////        }
////        Console.WriteLine();

////        // Example 3: Creating multiple users in a list
////        Console.WriteLine("=== Example 3: Multiple Users ===");
////List<User> users = new List<User>();

////users.Add(new User("alice123", "pass123", "alice@email.com", "Student"));
////users.Add(new User("bob456", "teach123", "bob@email.com", "Instructor"));
////users.Add(new User("admin", "admin123", "admin@email.com", "Admin"));

////foreach (User user in users)
////{
////    user.DisplayInfo();
////    Console.WriteLine("---");
////}
////        Console.WriteLine();

////        // Example 4: Finding a user by username
////        Console.WriteLine("=== Example 4: Finding User ===");
////        User found = users.Find(u => u.Username == "alice123");

////        if (found != null)
////        {
////            found.DisplayInfo();
////        }
////        Console.WriteLine();

////        // Example 5: Creating and testing a Course
////        Console.WriteLine("=== Example 5: Creating Course ===");
////        Course course = new Course(1, "C# Basics", "Learn C#", "Prof Bob", 30, 0);
////        course.DisplayInfo();
////        Console.WriteLine();

////        // Example 6: Creating and testing an Enrollment
////        Console.WriteLine("=== Example 6: Creating Enrollment ===");
////        User student = new User("alice123", "pass123", "alice@email.com", "Student");
////        Course csBasics = new Course(1, "C# Basics", "Learn C#", "Prof Bob", 30, 0);
////        Enrollment enrollment = new Enrollment(1, student.Username, csBasics.CourseId, DateTime.Now, 0, false);

////        enrollment.DisplayInfo();
////        Console.WriteLine("\n--- Updating Progress ---");
////        enrollment.UpdateProgress(50);
////        enrollment.DisplayInfo();

////        Console.WriteLine("\nPress any key to exit...");
////        Console.ReadKey();
////    }
////}
//using Week_1;
////Test inheritance
//Student student = new Student("alice", "pass123", "alice@email.com");
//Console.WriteLine(student.Username);
//student.DisplayInfo();


////Test enrollment
////student.EnrollInCourse(101);
////student.EnrollInCourse(102);
////student.EnrollInCourse(101);

//// Test polymorphism

////List<User> allUsers = new List<User>();
////allUsers.Add(new Student("alice", "pass1", "alice@email.com"));
////allUsers.Add(new Instructor("bob", "pass2", "bob@email.com"));
////allUsers.Add(new Admin("admin", "pass3", "admin@email.com"));
////foreach (User user in allUsers)
////{
////    user.DisplayInfo();
////    Console.WriteLine("---");
////}


using System;
using System.Collections.Generic;
using Week_1;

class Program
{
    // NEW Week 2 Architecture
    static List<User> users = new List<User>();
    static List<Course> courses = new List<Course>();
    static List<Enrollment> enrollments = new List<Enrollment>();

    static bool isLoggedIn = false;
    static User currentUser = null;

    static void Main(string[] args)
    {
        LoadSampleCourses();
        // Add one default admin for testing
        users.Add(new User("admin", "admin123", "admin@smart.com", "Admin"));

        while (true)
        {
            if (!isLoggedIn)
            {
                Console.WriteLine("\n--- SmartLearn Welcome ---");
                Console.WriteLine("1. Register\n2. Login\n3. Exit");
                string choice = Console.ReadLine();

                if (choice == "1") RegisterUser();
                else if (choice == "2") LoginUser();
                else if (choice == "3") break;
            }
            
        }
    }

    static void LoadSampleCourses()
    {
        courses.Clear();
        courses.Add(new Course(1, "C# Fundamentals", "Basics", "Prof. Bob", 2, 0, "Programming"));
        courses.Add(new Course(2, "SQL Mastery", "DB design", "Prof. Jana", 25, 0, "Database"));
        courses.Add(new Course(3, "Web Basics", "HTML/CSS", "Prof. Alice", 30, 0, "Web"));
        courses.Add(new Course(4, "AI Ethics", "Theory", "Dr. Smith", 15, 0, "Philosophy"));
        courses.Add(new Course(5, "Cloud Ops", "Azure/AWS", "Engr. Mike", 10, 0, "IT"));
    }

    static void RegisterUser()
    {
        Console.Write("Username: "); string un = Console.ReadLine();
        if (users.Exists(u => u.Username == un)) { Console.WriteLine("User exists!"); return; }

        Console.Write("Password: "); string pw = Console.ReadLine();
        Console.Write("Email: "); string em = Console.ReadLine();
        Console.Write("Role (Student/Instructor/Admin): "); string ro = Console.ReadLine();

        users.Add(new User(un, pw, em, ro));
        Console.WriteLine("✓ Registration Successful!");
    }

    static void LoginUser()
    {
        Console.Write("Username: "); string un = Console.ReadLine();
        User found = users.Find(u => u.Username == un);

        if (found == null) { Console.WriteLine("User not found."); return; }

        Console.Write("Password: "); string pw = Console.ReadLine();
        if (found.ValidatePassword(pw))
        {
            isLoggedIn = true;
            currentUser = found;
            Console.WriteLine($"\nWelcome {currentUser.Username}! Role: {currentUser.Role}");
            ShowDashboardByRole();
        }
        else Console.WriteLine("Invalid Password.");
    }

    static void ShowDashboardByRole()
    {
        Console.WriteLine($"\n--- {currentUser.Role} Dashboard ---");
        if (currentUser.Role.ToLower() == "student")
        {
            Console.WriteLine("1. Browse & Enroll\n2. My Courses\n3. Logout");
            string choice = Console.ReadLine();
            if (choice == "1") BrowseAndEnrollCourses();
            else if (choice == "2") ShowMyEnrolledCourses();
            else if (choice == "3") Logout();
        }
        else
        {
            Console.WriteLine("1. View Profile\n2. Logout");
            string choice = Console.ReadLine();
            if (choice == "1") currentUser.DisplayInfo();
            else if (choice == "2") Logout();
        }
    }

    static void BrowseAndEnrollCourses()
    {
        Console.WriteLine("\nAvailable Courses:");
        foreach (var c in courses) c.DisplayInfo();

        Console.Write("\nEnter Course ID to enroll: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            Course target = courses.Find(c => c.CourseId == id);
            if (target == null) Console.WriteLine("Invalid ID.");
            else if (!target.CanEnroll()) Console.WriteLine("Course is Full!");
            else if (enrollments.Exists(e => e.CourseId == id && e.StudentUsername == currentUser.Username))
                Console.WriteLine("Already enrolled!");
            else
            {
                enrollments.Add(new Enrollment(currentUser.Username, id));
                target.IncrementEnrollment();
                Console.WriteLine("✓ Successfully enrolled!");
            }
        }
    }


    static void ShowMyEnrolledCourses()
    {
        var myEnrolls = enrollments.FindAll(e => e.StudentUsername == currentUser.Username);
        if (myEnrolls.Count == 0) Console.WriteLine("No enrollments found.");

        foreach (var e in myEnrolls)
        {
            Course c = courses.Find(course => course.CourseId == e.CourseId);
            Console.WriteLine($"- {c.Title} | Progress: {e.ProgressPercentage}%");
        }
    }

    static void Logout()
    {
        isLoggedIn = false;
        currentUser = null;
        Console.WriteLine("Logged out.");
    }
}