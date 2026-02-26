
using System;
using System.Collections.Generic;
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
            // Seed courses
            courseList.Add(new Course(1, "C# Basics", "Learn C#", "Prof Bob", 30, 0));
            courseList.Add(new Course(2, "SQL Server", "Databases", "Prof Sara", 25, 5));
            courseList.Add(new Course(3, "ASP.NET Core", "Web Dev", "Prof John", 20, 10));
            courseList.Add(new Course(4, "Advanced C#", "Deep C#", "Prof Bob", 15, 0));
            courseList.Add(new Course(5, "Entity Framework", "EF Core ORM", "Prof Sara", 20, 3));

            bool running = true;
            while (running)
                running = ShowMainMenu();
        }

        // ══════════════════════════════════════════════════════
        //  MAIN MENU
        // ══════════════════════════════════════════════════════
        static bool ShowMainMenu()
        {
            Console.Clear();
            Console.WriteLine("================================");
            Console.WriteLine("    Welcome to SmartLearn LMS   ");
            Console.WriteLine("================================");
            Console.WriteLine("  1. Register");
            Console.WriteLine("  2. Login");
            Console.WriteLine("  3. View All Users   (test)");
            Console.WriteLine("  4. View All Courses (test)");
            Console.WriteLine("  5. Exit");
            Console.WriteLine("================================");
            Console.Write("  Choice: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1": RegisterUser(); break;
                case "2": LoginUser(); break;
                case "3": ShowAllUsers(); break;
                case "4": ShowAllCourses(); break;
                case "5":
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
        //  REGISTER — creates correct TYPE based on role
        // ══════════════════════════════════════════════════════
        static void RegisterUser()
        {
            Console.Clear();
            Console.WriteLine("================================");
            Console.WriteLine("           REGISTER             ");
            Console.WriteLine("================================");

            Console.Write("  Username : "); string username = Console.ReadLine();
            Console.Write("  Email    : "); string email = Console.ReadLine();
            Console.Write("  Password : "); string password = Console.ReadLine();
            Console.Write("  Role (Student/Instructor/Admin): ");
            string role = Console.ReadLine();

            // Validate
            if (string.IsNullOrWhiteSpace(username) || username.Length < 3)
            {
                Console.WriteLine("  Username must be at least 3 characters.");
                Console.ReadKey(); return;
            }
            if (!email.Contains("@") || !email.Contains("."))
            {
                Console.WriteLine("  Invalid email.");
                Console.ReadKey(); return;
            }
            if (password.Length < 6)
            {
                Console.WriteLine("  Password must be at least 6 characters.");
                Console.ReadKey(); return;
            }
            if (role != "Student" && role != "Instructor" && role != "Admin")
            {
                Console.WriteLine("  Invalid role. Must be Student, Instructor, or Admin.");
                Console.ReadKey(); return;
            }

            // Check duplicate username
            User existing = userList.Find(u => u.Username == username);
            if (existing != null)
            {
                Console.WriteLine("  Username already exists!");
                Console.ReadKey(); return;
            }

            // ── KEY CHANGE FROM WEEK 1 ──
            // Instead of: users.Add(username, password) + emails.Add + roles.Add
            // Now: create the RIGHT TYPE based on role
            if (role == "Student")
                userList.Add(new Student(username, password, email));
            else if (role == "Instructor")
                userList.Add(new Instructor(username, password, email));
            else if (role == "Admin")
                userList.Add(new Admin(username, password, email));

            Console.WriteLine($"\n  Registered successfully as {role}! Welcome, {username}!");
            Console.ReadKey();
        }

        // ══════════════════════════════════════════════════════
        //  LOGIN — finds User object, routes to correct dashboard
        // ══════════════════════════════════════════════════════
        static void LoginUser()
        {
            Console.Clear();
            Console.WriteLine("================================");
            Console.WriteLine("            LOGIN               ");
            Console.WriteLine("================================");

            Console.Write("  Username : "); string username = Console.ReadLine();
            Console.Write("  Password : "); string password = Console.ReadLine();

            // Find user in list
            User found = userList.Find(u => u.Username == username);

            if (found == null)
            {
                Console.WriteLine("  User not found!"); Console.ReadKey(); return;
            }
            if (!found.ValidatePassword(password))
            {
                Console.WriteLine("  Wrong password!"); Console.ReadKey(); return;
            }

            currentUser = found;
            Console.WriteLine($"\n  Welcome, {found.Username}!");
            Console.ReadKey();

            // ── KEY CHANGE FROM WEEK 1 ──
            // Instead of: switch (currentRole) { case "Student": ... }
            // Now: check the actual TYPE of the object
            if (currentUser is Student student)
                ShowStudentDashboard(student);
            else if (currentUser is Instructor instructor)
                ShowInstructorDashboard(instructor);
            else if (currentUser is Admin admin)
                ShowAdminDashboard(admin);
        }

        // ══════════════════════════════════════════════════════
        //  STUDENT DASHBOARD — receives Student object directly
        // ══════════════════════════════════════════════════════
        static void ShowStudentDashboard(Student student)
        {
            bool open = true;
            while (open)
            {
                Console.Clear();
                Console.WriteLine("================================");
                Console.WriteLine("       STUDENT DASHBOARD        ");
                Console.WriteLine($"  Welcome, {student.Username}!");
                Console.WriteLine("================================");
                Console.WriteLine("  1. My Profile");
                Console.WriteLine("  2. Browse Courses");
                Console.WriteLine("  3. Enroll in Course");
                Console.WriteLine("  4. My Enrolled Courses");
                Console.WriteLine("  5. Update My Progress");
                Console.WriteLine("  6. Logout");
                Console.WriteLine("================================");
                Console.Write("  Choice: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Clear();
                        student.DisplayInfo();
                        Console.ReadKey();
                        break;

                    case "2":
                        Console.Clear();
                        ShowAllCourses();
                        Console.ReadKey();
                        break;

                    case "3":
                        Console.Clear();
                        ShowAllCourses();
                        Console.Write("  Enter Course ID to enroll: ");
                        int enrollId;
                        if (int.TryParse(Console.ReadLine(), out enrollId))
                        {
                            Course selected = courseList.Find(c => c.CourseId == enrollId);
                            if (selected == null)
                                Console.WriteLine("  Course not found.");
                            else if (!selected.CanEnroll())
                                Console.WriteLine("  Course is full!");
                            else
                            {
                                // Call Student's own method — direct OOP call
                                student.EnrollInCourse(enrollId);
                                selected.CurrentEnrollments++;

                                // Also create Enrollment object
                                enrollments.Add(new Enrollment(
                                    enrollments.Count + 1,
                                    student.Username,
                                    enrollId,
                                    DateTime.Now,
                                    0,
                                    false
                                ));
                            }
                        }
                        else
                            Console.WriteLine("  Invalid ID.");
                        Console.ReadKey();
                        break;

                    case "4":
                        Console.Clear();
                        Console.WriteLine($"  === {student.Username}'s Courses ===\n");
                        if (student.EnrolledCourseIds.Count == 0)
                        {
                            Console.WriteLine("  No courses enrolled yet.");
                        }
                        else
                        {
                            // Call Student's own method
                            foreach (int id in student.EnrolledCourseIds)
                            {
                                Course c = courseList.Find(co => co.CourseId == id);
                                int progress = student.CourseProgress.ContainsKey(id)
                                               ? student.CourseProgress[id] : 0;
                                Console.WriteLine($"  Course  : {c?.Title ?? "ID " + id}");
                                Console.WriteLine($"  Progress: {progress}%");
                                Console.WriteLine("  ---");
                            }
                        }
                        Console.ReadKey();
                        break;

                    case "5":
                        Console.Clear();
                        if (student.EnrolledCourseIds.Count == 0)
                        {
                            Console.WriteLine("  No courses enrolled yet.");
                            Console.ReadKey(); break;
                        }
                        Console.Write("  Enter Course ID : ");
                        int cId;
                        if (!int.TryParse(Console.ReadLine(), out cId))
                        {
                            Console.WriteLine("  Invalid ID."); Console.ReadKey(); break;
                        }
                        Console.Write("  Enter Progress % (0-100): ");
                        int prog;
                        if (!int.TryParse(Console.ReadLine(), out prog))
                        {
                            Console.WriteLine("  Invalid number."); Console.ReadKey(); break;
                        }
                        // Call Student's own UpdateProgress method
                        if (student.CourseProgress.ContainsKey(cId))
                        {
                            student.CourseProgress[cId] = prog;
                            Console.WriteLine($"  Progress updated to {prog}%");
                            if (prog >= 100) Console.WriteLine("  Course completed!");
                        }
                        else
                            Console.WriteLine("  Not enrolled in this course.");
                        Console.ReadKey();
                        break;

                    case "6":
                        currentUser = null;
                        Console.WriteLine("  Logged out.");
                        Console.ReadKey();
                        open = false;
                        break;

                    default:
                        Console.WriteLine("  Invalid choice.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        // ══════════════════════════════════════════════════════
        //  INSTRUCTOR DASHBOARD — receives Instructor object
        // ══════════════════════════════════════════════════════
        static void ShowInstructorDashboard(Instructor instructor)
        {
            bool open = true;
            while (open)
            {
                Console.Clear();
                Console.WriteLine("================================");
                Console.WriteLine("     INSTRUCTOR DASHBOARD       ");
                Console.WriteLine($"  Welcome, {instructor.Username}!");
                Console.WriteLine("================================");
                Console.WriteLine("  1. My Profile");
                Console.WriteLine("  2. Add Course to My List");
                Console.WriteLine("  3. My Courses");
                Console.WriteLine("  4. Remove Course from My List");
                Console.WriteLine("  5. Browse All Courses");
                Console.WriteLine("  6. View All Students");
                Console.WriteLine("  7. Logout");
                Console.WriteLine("================================");
                Console.Write("  Choice: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Clear();
                        instructor.DisplayInfo();
                        Console.WriteLine($"  Teaching {instructor.CourseIds.Count} course(s).");
                        Console.ReadKey();
                        break;

                    case "2":
                        Console.Clear();
                        ShowAllCourses();
                        Console.Write("  Enter Course ID to add: ");
                        int addId;
                        if (int.TryParse(Console.ReadLine(), out addId))
                        {
                            Course c = courseList.Find(co => co.CourseId == addId);
                            if (c == null)
                                Console.WriteLine("  Course not found.");
                            else
                                instructor.AddCourse(addId); // Instructor's own method
                        }
                        Console.ReadKey();
                        break;

                    case "3":
                        Console.Clear();
                        Console.WriteLine($"  === {instructor.Username}'s Courses ===\n");
                        if (instructor.CourseIds.Count == 0)
                        {
                            Console.WriteLine("  No courses added yet.");
                        }
                        else
                        {
                            instructor.ShowMyCourses(); // Instructor's own method
                            Console.WriteLine();
                            // Also show course names
                            foreach (int id in instructor.CourseIds)
                            {
                                Course c = courseList.Find(co => co.CourseId == id);
                                if (c != null)
                                {
                                    Console.WriteLine($"  [{id}] {c.Title}");
                                    Console.WriteLine($"       Enrolled: {c.CurrentEnrollments}/{c.MaxStudents}");
                                }
                            }
                        }
                        Console.ReadKey();
                        break;

                    case "4":
                        Console.Clear();
                        instructor.ShowMyCourses();
                        Console.Write("  Enter Course ID to remove: ");
                        int removeId;
                        if (int.TryParse(Console.ReadLine(), out removeId))
                            instructor.RemoveCourse(removeId); // Instructor's own method
                        Console.ReadKey();
                        break;

                    case "5":
                        Console.Clear();
                        ShowAllCourses();
                        Console.ReadKey();
                        break;

                    case "6":
                        Console.Clear();
                        Console.WriteLine("  === All Students ===\n");
                        bool found = false;
                        foreach (User user in userList)
                        {
                            if (user is Student s)
                            {
                                s.DisplayInfo();
                                Console.WriteLine($"  Enrolled in {s.EnrolledCourseIds.Count} course(s).");
                                Console.WriteLine("  ---");
                                found = true;
                            }
                        }
                        if (!found) Console.WriteLine("  No students registered yet.");
                        Console.ReadKey();
                        break;

                    case "7":
                        currentUser = null;
                        Console.WriteLine("  Logged out.");
                        Console.ReadKey();
                        open = false;
                        break;

                    default:
                        Console.WriteLine("  Invalid choice.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        // ══════════════════════════════════════════════════════
        //  ADMIN DASHBOARD — receives Admin object
        // ══════════════════════════════════════════════════════
        static void ShowAdminDashboard(Admin admin)
        {
            bool open = true;
            while (open)
            {
                Console.Clear();
                Console.WriteLine("================================");
                Console.WriteLine("        ADMIN DASHBOARD         ");
                Console.WriteLine($"  Welcome, {admin.Username}!");
                Console.WriteLine("================================");
                Console.WriteLine("  1. My Profile & Permissions");
                Console.WriteLine("  2. View All Users");
                Console.WriteLine("  3. View All Students");
                Console.WriteLine("  4. View All Instructors");
                Console.WriteLine("  5. View All Courses");
                Console.WriteLine("  6. System Stats");
                Console.WriteLine("  7. Logout");
                Console.WriteLine("================================");
                Console.Write("  Choice: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Clear();
                        admin.DisplayInfo();
                        admin.DisplayPermissions(); // Admin's own method
                        Console.ReadKey();
                        break;

                    case "2":
                        Console.Clear();
                        Console.WriteLine("  === All Users ===\n");
                        if (userList.Count == 0)
                        {
                            Console.WriteLine("  No users yet.");
                        }
                        else
                        {
                            // POLYMORPHISM — same loop, each shows its own DisplayInfo
                            foreach (User user in userList)
                            {
                                // Show type label
                                string type = user is Student ? "[STUDENT]"
                                            : user is Instructor ? "[INSTRUCTOR]"
                                            : user is Admin ? "[ADMIN]"
                                            : "[USER]";
                                Console.WriteLine($"  {type}");
                                user.DisplayInfo();
                                Console.WriteLine("  ---");
                            }
                        }
                        Console.ReadKey();
                        break;

                    case "3":
                        Console.Clear();
                        Console.WriteLine("  === All Students ===\n");
                        bool hasStudents = false;
                        foreach (User user in userList)
                        {
                            if (user is Student s)
                            {
                                s.DisplayInfo();
                                Console.WriteLine($"  Enrolled: {s.EnrolledCourseIds.Count} course(s)");
                                Console.WriteLine("  ---");
                                hasStudents = true;
                            }
                        }
                        if (!hasStudents) Console.WriteLine("  No students yet.");
                        Console.ReadKey();
                        break;

                    case "4":
                        Console.Clear();
                        Console.WriteLine("  === All Instructors ===\n");
                        bool hasInstructors = false;
                        foreach (User user in userList)
                        {
                            if (user is Instructor ins)
                            {
                                ins.DisplayInfo();
                                Console.WriteLine($"  Teaching: {ins.CourseIds.Count} course(s)");
                                Console.WriteLine("  ---");
                                hasInstructors = true;
                            }
                        }
                        if (!hasInstructors) Console.WriteLine("  No instructors yet.");
                        Console.ReadKey();
                        break;

                    case "5":
                        Console.Clear();
                        ShowAllCourses();
                        Console.ReadKey();
                        break;

                    case "6":
                        Console.Clear();
                        Console.WriteLine("  === System Stats ===\n");
                        int studentCount = 0;
                        int instructorCount = 0;
                        int adminCount = 0;
                        foreach (User user in userList)
                        {
                            if (user is Student) studentCount++;
                            else if (user is Instructor) instructorCount++;
                            else if (user is Admin) adminCount++;
                        }
                        Console.WriteLine($"  Total Users      : {userList.Count}");
                        Console.WriteLine($"  Students         : {studentCount}");
                        Console.WriteLine($"  Instructors      : {instructorCount}");
                        Console.WriteLine($"  Admins           : {adminCount}");
                        Console.WriteLine($"  Total Courses    : {courseList.Count}");
                        Console.WriteLine($"  Total Enrollments: {enrollments.Count}");
                        Console.ReadKey();
                        break;

                    case "7":
                        currentUser = null;
                        Console.WriteLine("  Logged out.");
                        Console.ReadKey();
                        open = false;
                        break;

                    default:
                        Console.WriteLine("  Invalid choice.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        // ══════════════════════════════════════════════════════
        //  SHARED UTILITIES
        // ══════════════════════════════════════════════════════
        static void ShowAllUsers()
        {
            Console.Clear();
            Console.WriteLine("  === All Users ===\n");
            if (userList.Count == 0)
            {
                Console.WriteLine("  No users yet.");
                Console.ReadKey(); return;
            }
            foreach (User user in userList)
            {
                string type = user is Student ? "[STUDENT]"
                            : user is Instructor ? "[INSTRUCTOR]"
                            : user is Admin ? "[ADMIN]"
                            : "[USER]";
                Console.WriteLine($"  {type}");
                user.DisplayInfo();
                Console.WriteLine("  ---");
            }
            Console.ReadKey();
        }

        static void ShowAllCourses()
        {
            Console.WriteLine("  === Available Courses ===\n");
            foreach (Course course in courseList)
            {
                course.DisplayInfo();
                Console.WriteLine("  ---");
            }
        }
    }
}