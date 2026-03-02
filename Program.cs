using System;
using System.Collections.Generic;
using System.Linq;
using Week_1;

namespace SmartLearnLMS
{
    class Program
    {
        static List<User> userList = new List<User>();
        static List<Course> courseList = new List<Course>();
        static List<Enrollment> enrollments = new List<Enrollment>();
        static User currentUser = null;

        static void Main(string[] args)
        {
            InitializeCourses();

            bool running = true;
            while (running)
                running = ShowMainMenu();
        }

        // ══════════════════════════════════════════════════════
        //  SAMPLE DATA
        // ══════════════════════════════════════════════════════
        static void InitializeCourses()
        {
            // 5 Online Courses (IDs 101-105)
            courseList.Add(new OnlineCourse(101, "C# Fundamentals", "Learn C# from scratch", "Prof. Smith", "Programming", 450));
            courseList.Add(new OnlineCourse(102, "Python for Beginners", "Intro to Python programming", "Prof. Johnson", "Programming", 360));
            courseList.Add(new OnlineCourse(103, "Web Development Basics", "HTML, CSS and JS fundamentals", "Prof. Garcia", "Web Development", 540));
            courseList.Add(new OnlineCourse(104, "Data Structures", "Arrays, Lists, Trees and more", "Prof. Smith", "Computer Science", 600));
            courseList.Add(new OnlineCourse(105, "Machine Learning Intro", "Basics of ML and AI concepts", "Prof. Lee", "Data Science", 720));

            // 3 In-Person Courses (IDs 201-203)
            courseList.Add(new InPersonCourse(201, "Database Design Workshop", "Relational DB and SQL", "Prof. Johnson", "Database", 25, "B-101", "Engineering Building"));
            courseList.Add(new InPersonCourse(202, "Network Security Lab", "Practical cybersecurity skills", "Prof. Brown", "Security", 20, "C-205", "CS Building"));
            courseList.Add(new InPersonCourse(203, "Mobile App Development", "Build iOS and Android apps", "Prof. Garcia", "Mobile", 30, "A-301", "Tech Center"));

            // 2 Hybrid Courses (IDs 301-302)
            courseList.Add(new HybridCourse(301, "Full-Stack Development", "Frontend + Backend full stack", "Prof. Garcia", "Web Development", 30, 720, "C-201", "CS Building"));
            courseList.Add(new HybridCourse(302, "Cloud Computing", "AWS, Azure and cloud concepts", "Prof. Lee", "Cloud", 25, 600, "D-101", "Engineering Building"));
        }

        // ══════════════════════════════════════════════════════
        //  MAIN MENU
        // ══════════════════════════════════════════════════════
        static bool ShowMainMenu()
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════╗");
            Console.WriteLine("║    Welcome to SmartLearn LMS   ║");
            Console.WriteLine("╚════════════════════════════════╝");
            Console.WriteLine("  [1] Register");
            Console.WriteLine("  [2] Login");
            Console.WriteLine("  [3] View All Users   (test)");
            Console.WriteLine("  [4] View All Courses (test)");
            Console.WriteLine("  [5] Universal Search (test)");
            Console.WriteLine("  [6] Exit");
            Console.WriteLine("════════════════════════════════");
            Console.Write("  Choice: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1": RegisterUser(); break;
                case "2": LoginUser(); break;
                case "3": ShowAllUsers(); break;
                case "4": BrowseCourses(); break;
                case "5": UniversalSearch(); break;
                case "6":
                    Console.WriteLine("  Goodbye!");
                    return false;
                default:
                    Console.WriteLine("  Invalid choice.");
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
            Console.WriteLine("╔════════════════════════════════╗");
            Console.WriteLine("║            REGISTER            ║");
            Console.WriteLine("╚════════════════════════════════╝");

            Console.Write("  Username : "); string username = Console.ReadLine();
            Console.Write("  Email    : "); string email = Console.ReadLine();
            Console.Write("  Password : "); string password = Console.ReadLine();
            Console.Write("  Role (Student/Instructor/Admin): "); string role = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(username) || username.Length < 3)
            { Console.WriteLine("  ✗ Username must be at least 3 characters."); Console.ReadKey(); return; }
            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
            { Console.WriteLine("  ✗ Invalid email - must contain @"); Console.ReadKey(); return; }
            if (password.Length < 8)
            { Console.WriteLine("  ✗ Password must be at least 8 characters"); Console.ReadKey(); return; }
            if (!password.Any(char.IsDigit))
            { Console.WriteLine("  ✗ Password must contain at least 1 number"); Console.ReadKey(); return; }
            if (role != "Student" && role != "Instructor" && role != "Admin")
            { Console.WriteLine("  ✗ Invalid role."); Console.ReadKey(); return; }

            if (userList.Find(u => u.Username == username) != null)
            { Console.WriteLine("  ✗ Username already exists!"); Console.ReadKey(); return; }

            if (role == "Student")
                userList.Add(new Student(username, password, email));
            else if (role == "Instructor")
                userList.Add(new Instructor(username, password, email));
            else
                userList.Add(new Admin(username, password, email));

            Console.WriteLine($"\n  ✓ Registered as {role}! Welcome, {username}!");
            Console.ReadKey();
        }

        // ══════════════════════════════════════════════════════
        //  LOGIN — pure polymorphism, zero if-else type checks
        // ══════════════════════════════════════════════════════
        static void LoginUser()
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════╗");
            Console.WriteLine("║              LOGIN             ║");
            Console.WriteLine("╚════════════════════════════════╝");

            Console.Write("  Username : "); string username = Console.ReadLine();
            Console.Write("  Password : "); string password = Console.ReadLine();

            User found = userList.Find(u => u.Username == username);
            if (found == null) { Console.WriteLine("  ✗ User not found!"); Console.ReadKey(); return; }
            if (!found.ValidatePassword(password)) { Console.WriteLine("  ✗ Wrong password!"); Console.ReadKey(); return; }

            currentUser = found;
            Console.WriteLine($"\n  ✓ Welcome, {found.Username}! [{found.GetUserType()}]");
            Console.ReadKey();

            // ══ ONE POLYMORPHIC CALL — no if-else needed ══
            RunDashboard(currentUser);
        }

        // ══════════════════════════════════════════════════════
        //  DASHBOARD RUNNER — fully polymorphic
        // ══════════════════════════════════════════════════════
        static void RunDashboard(User user)
        {
            bool open = true;
            while (open)
            {
                Console.Clear();
                user.DisplayDashboard();
                Console.WriteLine();
                Console.Write("  Choice: ");
                string choice = Console.ReadLine();

                if (user is Student student)
                    open = HandleStudentChoice(student, choice);
                else if (user is Instructor instructor)
                    open = HandleInstructorChoice(instructor, choice);
                else if (user is Admin admin)
                    open = HandleAdminChoice(admin, choice);
            }
        }

        // ══════════════════════════════════════════════════════
        //  STUDENT HANDLERS
        // ══════════════════════════════════════════════════════
        static bool HandleStudentChoice(Student student, string choice)
        {
            switch (choice)
            {
                case "1":
                    Console.Clear();
                    BrowseAndEnroll(student);
                    return true;

                case "2":
                    Console.Clear();
                    Console.WriteLine($"  === {student.Username}'s Enrolled Courses ===\n");
                    if (student.EnrolledCourseIds.Count == 0)
                        Console.WriteLine("  No courses enrolled yet.");
                    else
                    {
                        foreach (int id in student.EnrolledCourseIds)
                        {
                            Course c = courseList.Find(co => co.CourseId == id);
                            int prog = student.CourseProgress.ContainsKey(id) ? student.CourseProgress[id] : 0;
                            Console.WriteLine($"  [{id}] {c?.Title ?? "Unknown"} — Progress: {prog}%");
                        }
                    }
                    Console.ReadKey();
                    return true;

                case "3":
                    Console.Clear();
                    UpdateProgress(student);
                    return true;

                case "4":
                    Console.Clear();
                    Console.WriteLine($"  === Statistics for {student.Username} ===\n");
                    student.DisplayInfo();
                    Console.ReadKey();
                    return true;

                case "5":
                    Console.Clear();
                    RateCourse(student);
                    return true;

                case "6":
                    Console.Clear();
                    Console.WriteLine($"  === Notifications for {student.Username} ===\n");
                    var notifs = student.GetNotificationHistory();
                    if (notifs.Count == 0) Console.WriteLine("  No notifications.");
                    else foreach (string n in notifs) Console.WriteLine($"  🔔 {n}");
                    Console.ReadKey();
                    return true;

                case "7":
                    currentUser = null;
                    Console.WriteLine("  ✓ Logged out.");
                    Console.ReadKey();
                    return false;

                default:
                    Console.WriteLine("  Invalid choice.");
                    Console.ReadKey();
                    return true;
            }
        }

        static void BrowseAndEnroll(Student student)
        {
            BrowseCourses();
            Console.Write("  Enter Course ID to enroll (0 to cancel): ");
            int enrollId;
            if (!int.TryParse(Console.ReadLine(), out enrollId) || enrollId == 0) return;

            Course selected = courseList.Find(c => c.CourseId == enrollId);
            if (selected == null)
                Console.WriteLine("  ✗ Course not found.");
            else if (!selected.CanEnroll(student))
                Console.WriteLine("  ✗ Course is full!");
            else if (student.EnrolledCourseIds.Contains(enrollId))
                Console.WriteLine("  ✗ Already enrolled in this course.");
            else
            {
                selected.Enroll(student);
                enrollments.Add(new Enrollment(enrollments.Count + 1, student.Username, enrollId, DateTime.Now, 0, false));
            }
            Console.ReadKey();
        }

        static void UpdateProgress(Student student)
        {
            if (student.EnrolledCourseIds.Count == 0)
            { Console.WriteLine("  No courses enrolled yet."); Console.ReadKey(); return; }

            Console.WriteLine($"  === Update Progress for {student.Username} ===\n");
            foreach (int id in student.EnrolledCourseIds)
            {
                Course c = courseList.Find(co => co.CourseId == id);
                int prog = student.CourseProgress.ContainsKey(id) ? student.CourseProgress[id] : 0;
                Console.WriteLine($"  [{id}] {c?.Title ?? "Unknown"} — {prog}%");
            }

            Console.Write("\n  Enter Course ID: ");
            int cId;
            if (!int.TryParse(Console.ReadLine(), out cId)) { Console.WriteLine("  ✗ Invalid ID."); Console.ReadKey(); return; }
            Console.Write("  Enter Progress % (0-100): ");
            int prog2;
            if (!int.TryParse(Console.ReadLine(), out prog2)) { Console.WriteLine("  ✗ Invalid number."); Console.ReadKey(); return; }

            if (student.CourseProgress.ContainsKey(cId))
            {
                student.ProgressPercentage = prog2;
                student.CourseProgress[cId] = prog2;
                Console.WriteLine($"  ✓ Progress updated to {prog2}%");
                if (prog2 >= 100)
                {
                    Console.WriteLine("  🎉 Course completed!");
                    student.SendNotification($"You completed Course ID {cId}!");
                }
            }
            else
                Console.WriteLine("  ✗ Not enrolled in this course.");
            Console.ReadKey();
        }

        static void RateCourse(Student student)
        {
            BrowseCourses();
            Console.Write("  Enter Course ID to rate: ");
            int rId;
            if (!int.TryParse(Console.ReadLine(), out rId)) { Console.ReadKey(); return; }
            Course rc = courseList.Find(c => c.CourseId == rId);
            if (rc == null) { Console.WriteLine("  ✗ Course not found."); Console.ReadKey(); return; }
            Console.Write("  Stars (1-5): ");
            int stars;
            int.TryParse(Console.ReadLine(), out stars);
            Console.Write("  Review: ");
            string review = Console.ReadLine();
            rc.AddRating(stars, review);
            Console.ReadKey();
        }

        // ══════════════════════════════════════════════════════
        //  INSTRUCTOR HANDLERS
        // ══════════════════════════════════════════════════════
        static bool HandleInstructorChoice(Instructor instructor, string choice)
        {
            switch (choice)
            {
                case "1":
                    Console.Clear();
                    Console.WriteLine($"  === {instructor.Username}'s Courses ===\n");
                    if (instructor.CourseIds.Count == 0) Console.WriteLine("  No courses added.");
                    else
                    {
                        foreach (int id in instructor.CourseIds)
                        {
                            Course c = courseList.Find(co => co.CourseId == id);
                            c?.DisplayCourseInfo();
                            Console.WriteLine();
                        }
                    }
                    Console.ReadKey();
                    return true;

                case "2":
                    Console.Clear();
                    BrowseCourses();
                    Console.Write("  Enter Course ID to add to your list: ");
                    int addId;
                    if (int.TryParse(Console.ReadLine(), out addId))
                    {
                        Course c = courseList.Find(co => co.CourseId == addId);
                        if (c == null) Console.WriteLine("  ✗ Course not found.");
                        else instructor.AddCourse(addId);
                    }
                    Console.ReadKey();
                    return true;

                case "3":
                    Console.Clear();
                    Console.WriteLine("  === Student Roster ===\n");
                    bool any = false;
                    foreach (User u in userList)
                    {
                        if (u is Student s)
                        {
                            s.DisplayInfo();
                            Console.WriteLine("  ---");
                            any = true;
                        }
                    }
                    if (!any) Console.WriteLine("  No students registered.");
                    Console.ReadKey();
                    return true;

                case "4":
                    Console.Clear();
                    Console.WriteLine("  [Grade Assignments — Coming in Week 4]");
                    Console.ReadKey();
                    return true;

                case "5":
                    Console.Clear();
                    Console.WriteLine($"  === Notifications for {instructor.Username} ===\n");
                    var notifs = instructor.GetNotificationHistory();
                    if (notifs.Count == 0) Console.WriteLine("  No notifications.");
                    else foreach (string n in notifs) Console.WriteLine($"  🔔 {n}");
                    Console.ReadKey();
                    return true;

                case "6":
                    currentUser = null;
                    Console.WriteLine("  ✓ Logged out.");
                    Console.ReadKey();
                    return false;

                default:
                    Console.WriteLine("  Invalid choice.");
                    Console.ReadKey();
                    return true;
            }
        }

        // ══════════════════════════════════════════════════════
        //  ADMIN HANDLERS
        // ══════════════════════════════════════════════════════
        static bool HandleAdminChoice(Admin admin, string choice)
        {
            switch (choice)
            {
                case "1":
                    Console.Clear();
                    Console.WriteLine("  === All Users ===\n");
                    if (userList.Count == 0) Console.WriteLine("  No users yet.");
                    else
                    {
                        foreach (User user in userList)
                        {
                            Console.WriteLine($"  [{user.GetUserType()}]");
                            user.DisplayInfo();
                            Console.WriteLine("  ---");
                        }
                    }
                    Console.ReadKey();
                    return true;

                case "2":
                    Console.Clear();
                    BrowseCourses();
                    Console.ReadKey();
                    return true;

                case "3":
                    Console.Clear();
                    Console.WriteLine("  === System Report ===\n");
                    int students = 0, instructors = 0, admins = 0;
                    foreach (User u in userList)
                    {
                        if (u is Student) students++;
                        else if (u is Instructor) instructors++;
                        else if (u is Admin) admins++;
                    }
                    Console.WriteLine($"  Total Users      : {userList.Count}");
                    Console.WriteLine($"  Students         : {students}");
                    Console.WriteLine($"  Instructors      : {instructors}");
                    Console.WriteLine($"  Admins           : {admins}");
                    Console.WriteLine($"  Total Courses    : {courseList.Count}");
                    Console.WriteLine($"  Total Enrollments: {enrollments.Count}");
                    Console.ReadKey();
                    return true;

                case "4":
                    Console.Clear();
                    Console.WriteLine("  === System Settings ===");
                    admin.DisplayInfo();
                    Console.ReadKey();
                    return true;

                case "5":
                    currentUser = null;
                    Console.WriteLine("  ✓ Logged out.");
                    Console.ReadKey();
                    return false;

                default:
                    Console.WriteLine("  Invalid choice.");
                    Console.ReadKey();
                    return true;
            }
        }

        // ══════════════════════════════════════════════════════
        //  SHARED UTILITIES
        // ══════════════════════════════════════════════════════
        static void ShowAllUsers()
        {
            Console.Clear();
            Console.WriteLine("  === All Users ===\n");
            if (userList.Count == 0) { Console.WriteLine("  No users yet."); Console.ReadKey(); return; }
            foreach (User user in userList)
            {
                Console.WriteLine($"  [{user.GetUserType()}]");
                user.DisplayInfo();
                Console.WriteLine("  ---");
            }
            Console.ReadKey();
        }

        static void BrowseCourses()
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════╗");
            Console.WriteLine("║       AVAILABLE COURSES        ║");
            Console.WriteLine("╚════════════════════════════════╝");
            Console.WriteLine();
            foreach (Course course in courseList)
            {
                course.DisplayCourseInfo();
                Console.WriteLine();
            }
        }

        static void UniversalSearch()
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════╗");
            Console.WriteLine("║        UNIVERSAL SEARCH        ║");
            Console.WriteLine("╚════════════════════════════════╝");
            Console.Write("  Enter keyword: ");
            string keyword = Console.ReadLine();

            List<ISearchable> searchable = new List<ISearchable>();
            foreach (Course c in courseList) searchable.Add(c);
            foreach (User u in userList) { if (u is ISearchable s) searchable.Add(s); }

            List<ISearchable> results = SearchEngine.Search(searchable, keyword);
            Console.WriteLine();
            SearchEngine.DisplayResults(results);
            Console.ReadKey();
        }
    }
}