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
//using Week_1;
//// Test inheritance
//Student student = new Student("alice", "pass123", "alice@email.com");
//Console.WriteLine(student.Username);
//student.DisplayInfo();


//// Test enrollment
//student.EnrollInCourse(101);
//student.EnrollInCourse(102);
//student.EnrollInCourse(101);

// Test polymorphism
using Week_1;
List<User> allUsers = new List<User>();
allUsers.Add(new Student("alice", "pass1", "alice@email.com"));
allUsers.Add(new Instructor("bob", "pass2", "bob@email.com"));
allUsers.Add(new Admin("admin", "pass3", "admin@email.com"));

//foreach (User user in allUsers)
//{
//    user.DisplayInfo();
//    Console.WriteLine("---");
//}

//ABSTRACT EXAMPLE
//using Week_1;
//User user = new User("test", "pass", "test@email.com");


// THE ARRAY MOVING NIGHTMARE-Code 1 — The Array Problem
//using Week_1;

//Course[] courses = new Course[10]; // Only 10 spaces!
//courses[0] = new Course { Title = "C# Basics" };
//courses[1] = new Course { Title = "OOP Magic" };

//// Need space for course #11? You have to "move houses"!
//Course[] biggerHouse = new Course[20];          // Get a bigger place
//Array.Copy(courses, biggerHouse, courses.Length); // Pack and move EVERYTHING
//courses = biggerHouse;                            // New address



//Code 2 — Array vs List Side-by-Side
//// NEW WAY - List<T> (magical)
//List<Course> courses = new List<Course>(); // No size needed!
//courses.Add(new Course { Title = "C# Basics" });
//courses.Add(new Course { Title = "OOP Mastery" });
//courses.Add(new Course { Title = "LINQ Wizardry" });
//courses.Add(new Course { Title = "Keep adding..." });
//courses.Add(new Course { Title = "...it never gets full!" });

//Console.WriteLine($"We have {courses.Count} courses!"); // IT counts for us!


//Code 3 — List<T> Superpowers (All Key Methods)
//List<Student> students = new List<Student>();

//// SUPERPOWER #1: Add to the end
//students.Add(new Student("alice", "pass123", "alice@email.com"));
//students.Add(new Student("bob", "pass123", "bob@email.com"));
//students.Add(new Student("charlie", "pass123", "charlie@email.com"));
//Console.WriteLine($"We have {students.Count} students");

//// SUPERPOWER #2: Insert at a specific position
//students.Insert(0, new Student("zara", "pass123", "zara@email.com"));
//Console.WriteLine($"First student is now: {students[0].Username}"); // zara!

//// SUPERPOWER #3: Find the FIRST match
//Student alice = students.Find(s => s.Username == "alice");
//Console.WriteLine($"Found: {alice?.Username}");

//// SUPERPOWER #4: Find ALL matches
//List<Student> active = students.FindAll(s => s.EnrolledCourseIds.Count > 0);
//Console.WriteLine($"Active students: {active.Count}");

//// SUPERPOWER #5: Check if something exists
//bool hasAlice = students.Contains(alice);
//Console.WriteLine($"Do we have Alice? {hasAlice}");

//// SUPERPOWER #6: Remove items
//students.RemoveAt(0); // Remove by index
//Console.WriteLine($"After removing first: {students.Count} students");

//// SUPERPOWER #7: Sort
//students.Sort((a, b) => a.Username.CompareTo(b.Username));
//Console.WriteLine("Sorted alphabetically!");
//foreach (var s in students)
//{
//    Console.WriteLine($"  - {s.Username}");
//}



////Code 4 — Dictionary Basics
//// Create a phonebook: Username (string) → Student object
//Dictionary<string, Student> studentBook = new Dictionary<string, Student>();

//// Add entries: key = username, value = Student object
//studentBook.Add("alice123", new Student("alice123", "pass", "alice@email.com"));
//studentBook.Add("bob456", new Student("bob456", "pass", "bob@email.com"));
//studentBook.Add("charlie789", new Student("charlie789", "pass", "charlie@email.com"));

//// INSTANT lookup — no looping needed!
//Student alice = studentBook["alice123"];
//Console.WriteLine($"Found instantly: {alice.Username}");

//// Check before looking up (avoids errors)
//if (studentBook.ContainsKey("zara999"))
//{
//    Student zara = studentBook["zara999"];
//}
//else
//{
//    Console.WriteLine("Zara not found");
//}

//// BEST PRACTICE: TryGetValue (safe lookup)
//if (studentBook.TryGetValue("bob456", out Student bob))
//{
//    Console.WriteLine($"Found Bob: {bob.Email}");
//}

//// Loop through all KEYS (usernames)
//foreach (string username in studentBook.Keys)
//{
//    Console.WriteLine($"Username: {username}");
//}

//// Loop through all VALUES (student objects)
//foreach (Student student in studentBook.Values)
//{
//    Console.WriteLine($"Student: {student.Username}");
//}

//// Loop through BOTH key and value
//foreach (KeyValuePair<string, Student> pair in studentBook)
//{
//    Console.WriteLine($"Key: {pair.Key} → Value: {pair.Value.Username}");
//}




