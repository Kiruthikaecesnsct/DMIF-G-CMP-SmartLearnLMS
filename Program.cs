//string userName;
//int age;
//Console.WriteLine("Enter your name:");
//userName = Console.ReadLine();
//Console.WriteLine("Hello, " + userName);


//Arithmetic Operators
//int a ;
//int b;

//Console.WriteLine("Enter the value of first number");
//a=int.Parse(Console.ReadLine());

//Console.WriteLine("Enter the value of Second number");
//b = int.Parse(Console.ReadLine());
//Console.WriteLine($"{a} + {b} = {a + b}");    // 22
//Console.WriteLine($"{a} - {b} = {a - b}");    // 12
//Console.WriteLine($"{a} * {b} = {a * b}");    // 85
//Console.WriteLine($"{a} / {b} = {a / b}");    // 3   (integer division)
//Console.WriteLine($"{a} % {b} = {a % b}");    // 2   (remainder)


////Comparison Operators & Logical operators
//int age = 17;
//bool hasId = true;

//Console.WriteLine(age >= 18);                // false
//Console.WriteLine(age == 18);                // false
//Console.WriteLine(age != 20);                // true

//// Logical operators
//Console.WriteLine(age >= 13 && age <= 19);   // true  (teenager)
//Console.WriteLine(age < 13 || hasId);        // true
//Console.WriteLine(!hasId);                   // false

////Even & Odd check

//int number = 42;

//if (number % 2 == 0)
//{
//    Console.WriteLine($"{number} is EVEN");
//}
//else
//{
//    Console.WriteLine($"{number} is ODD");
//}
//string type = (number % 2 == 0) ? "even" : "odd";
//Console.WriteLine(type);


////Grade evaluation with switchcase
//Console.Write("Enter grade letter (A/B/C/D/F): ");
//string grade = Console.ReadLine().ToUpper();

//switch (grade)
//{
//    case "A":
//        Console.WriteLine("Excellent! Keep it up.");
//        break;
//    case "B":
//        Console.WriteLine("Good work.");
//        break;
//    case "C":
//        Console.WriteLine("Satisfactory.");
//        break;
//    case "D":
//        Console.WriteLine("Pass – needs improvement.");
//        break;
//    case "F":
//        Console.WriteLine("Failed. Try harder next time.");
//        break;
//    default:
//        Console.WriteLine("Invalid grade entered.");
//        break;
//}


//User student1 = new User();
//student1.Username = "alice123";
//student1.Password = "pass123";
//student1.Email = "alice@email.com";
//student1.Role = "Student";

//User student1 = new User("alice123", "pass123", "alice@email.com", "Student");
//student1.DisplayInfo();
//using Week_1;

//class Program
//{
//    static void Main(string[] args)
//    {
//        // Example 1: Creating a single user
//        Console.WriteLine("=== Example 1: Creating a User ===");
//        User student1 = new User("alice123", "pass123", "alice@email.com", "Student");
//        student1.DisplayInfo();
//        Console.WriteLine();

//        // Example 2: Testing password validation
//        Console.WriteLine("=== Example 2: Password Validation ===");
//        Console.WriteLine("Enter password:");
//        string input = Console.ReadLine();

//        if (student1.ValidatePassword(input))
//        {
//            Console.WriteLine("Correct!");
//        }
//        else
//        {
//            Console.WriteLine("Wrong password!");
//        }
//        Console.WriteLine();

//        // Example 3: Creating multiple users in a list
//        Console.WriteLine("=== Example 3: Multiple Users ===");
//        List<User> users = new List<User>();

//        users.Add(new User("alice123", "pass123", "alice@email.com", "Student"));
//        users.Add(new User("bob456", "teach123", "bob@email.com", "Instructor"));
//        users.Add(new User("admin", "admin123", "admin@email.com", "Admin"));

//        foreach (User user in users)
//        {
//            user.DisplayInfo();
//            Console.WriteLine("---");
//        }
//        Console.WriteLine();

//        // Example 4: Finding a user by username
//        Console.WriteLine("=== Example 4: Finding User ===");
//        User found = users.Find(u => u.Username == "alice123");

//        if (found != null)
//        {
//            found.DisplayInfo();
//        }
//        Console.WriteLine();

//        // Example 5: Creating and testing a Course
//        Console.WriteLine("=== Example 5: Creating Course ===");
//        Course course = new Course(1, "C# Basics", "Learn C#", "Prof Bob", 30, 0);
//        course.DisplayInfo();
//        Console.WriteLine();

//        // Example 6: Creating and testing an Enrollment
//        Console.WriteLine("=== Example 6: Creating Enrollment ===");
//        User student = new User("alice123", "pass123", "alice@email.com", "Student");
//        Course csBasics = new Course(1, "C# Basics", "Learn C#", "Prof Bob", 30, 0);
//        Enrollment enrollment = new Enrollment(1, student.Username, csBasics.CourseId, DateTime.Now, 0, false);

//        enrollment.DisplayInfo();
//        Console.WriteLine("\n--- Updating Progress ---");
//        enrollment.UpdateProgress(50);
//        enrollment.DisplayInfo();

//        Console.WriteLine("\nPress any key to exit...");
//        Console.ReadKey();
//    }
//}
using Week_1;
// Test inheritance
Student student = new Student("alice", "pass123", "alice@email.com");
Console.WriteLine(student.Username);
student.DisplayInfo();


//// Test enrollment
//student.EnrollInCourse(101);
//student.EnrollInCourse(102);
//student.EnrollInCourse(101);

//// Test polymorphism
//List<User> allUsers = new List<User>();
//allUsers.Add(new Student("alice", "pass1", "alice@email.com"));
//allUsers.Add(new Instructor("bob", "pass2", "bob@email.com"));
//allUsers.Add(new Admin("admin", "pass3", "admin@email.com"));

//foreach (User user in allUsers)
//{
//    user.DisplayInfo();
//    Console.WriteLine("---");
//}