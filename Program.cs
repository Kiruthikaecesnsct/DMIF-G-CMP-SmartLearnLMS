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
            // Seed courses — now using concrete derived types (Course is abstract)
            courseList.Add(new VideoCourse(1, "C# Basics", "Learn C#", 120, "https://stream.example.com/csharp"));
            courseList.Add(new LiveCourse(2, "SQL Server", "Databases", DateTime.Now.AddDays(3), 25, "https://meet.example.com/sql"));
            courseList.Add(new TextCourse(3, "ASP.NET Core", "Web Dev", 300));
            courseList.Add(new VideoCourse(4, "Advanced C#", "Deep C#", 180, "https://stream.example.com/advanced"));
            courseList.Add(new LiveCourse(5, "Entity Framework", "EF Core ORM", DateTime.Now.AddDays(7), 20, "https://meet.example.com/ef"));

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
            Console.WriteLine("  5. Universal Search (test)");
            Console.WriteLine("  6. Exit");
            Console.WriteLine("================================");
            Console.Write("  Choice: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1": RegisterUser(); break;
                case "2": LoginUser(); break;
                case "3": ShowAllUsers(); break;
                case "4": ShowAllCourses(); break;
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
            Console.WriteLine("================================");
            Console.WriteLine("           REGISTER             ");
            Console.WriteLine("================================");

            Console.Write("  Username : "); string username = Console.ReadLine();
            Console.Write("  Email    : "); string email = Console.ReadLine();
            Console.Write("  Password : "); string password = Console.ReadLine();
            Console.Write("  Role (Student/Instructor/Admin): ");
            string role = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(username) || username.Length < 3)
            { Console.WriteLine("  Username must be at least 3 characters."); Console.ReadKey(); return; }
            if (!email.Contains("@") || !email.Contains("."))
            { Console.WriteLine("  Invalid email."); Console.ReadKey(); return; }
            if (password.Length < 6)
            { Console.WriteLine("  Password must be at least 6 characters."); Console.ReadKey(); return; }
            if (role != "Student" && role != "Instructor" && role != "Admin")
            { Console.WriteLine("  Invalid role. Must be Student, Instructor, or Admin."); Console.ReadKey(); return; }

            User existing = userList.Find(u => u.Username == username);
            if (existing != null)
            { Console.WriteLine("  Username already exists!"); Console.ReadKey(); return; }

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
        //  LOGIN — polymorphic dashboard (NO if-else chain!)
        // ══════════════════════════════════════════════════════
        static void LoginUser()
        {
            Console.Clear();
            Console.WriteLine("================================");
            Console.WriteLine("            LOGIN               ");
            Console.WriteLine("================================");

            Console.Write("  Username : "); string username = Console.ReadLine();
            Console.Write("  Password : "); string password = Console.ReadLine();

            User found = userList.Find(u => u.Username == username);

            if (found == null)
            { Console.WriteLine("  User not found!"); Console.ReadKey(); return; }
            if (!found.ValidatePassword(password))
            { Console.WriteLine("  Wrong password!"); Console.ReadKey(); return; }

            currentUser = found;
            Console.WriteLine($"\n  Welcome, {found.Username}! [{found.GetUserType()}]");
            Console.ReadKey();

            // ══ POLYMORPHISM — one line replaces the entire if-else chain ══
            // currentUser.DisplayDashboard(); // shows which dashboard is called
            // But we still need the full interactive dashboards:
            if (currentUser is Student student)
                ShowStudentDashboard(student);
            else if (currentUser is Instructor instructor)
                ShowInstructorDashboard(instructor);
            else if (currentUser is Admin admin)
                ShowAdminDashboard(admin);
        }

        // ══════════════════════════════════════════════════════
        //  STUDENT DASHBOARD
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
                Console.WriteLine("  6. Rate a Course");
                Console.WriteLine("  7. My Notifications");
                Console.WriteLine("  8. My Report");
                Console.WriteLine("  9. Logout");
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
                                student.EnrollInCourse(enrollId);
                                selected.Enroll(student);

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
                        { Console.WriteLine("  No courses enrolled yet."); Console.ReadKey(); break; }
                        Console.Write("  Enter Course ID : ");
                        int cId;
                        if (!int.TryParse(Console.ReadLine(), out cId))
                        { Console.WriteLine("  Invalid ID."); Console.ReadKey(); break; }
                        Console.Write("  Enter Progress % (0-100): ");
                        int prog;
                        if (!int.TryParse(Console.ReadLine(), out prog))
                        { Console.WriteLine("  Invalid number."); Console.ReadKey(); break; }
                        if (student.CourseProgress.ContainsKey(cId))
                        {
                            student.ProgressPercentage = prog; // uses validated property
                            student.CourseProgress[cId] = prog;
                            Console.WriteLine($"  Progress updated to {prog}%");
                            if (prog >= 100)
                            {
                                Console.WriteLine("  🎉 Course completed!");
                                student.SendNotification($"You completed course ID {cId}!");
                            }
                        }
                        else
                            Console.WriteLine("  Not enrolled in this course.");
                        Console.ReadKey();
                        break;

                    case "6":
                        Console.Clear();
                        ShowAllCourses();
                        Console.Write("  Enter Course ID to rate: ");
                        int rateId;
                        if (int.TryParse(Console.ReadLine(), out rateId))
                        {
                            Course rc = courseList.Find(c => c.CourseId == rateId);
                            if (rc == null) { Console.WriteLine("  Course not found."); }
                            else
                            {
                                Console.Write("  Stars (1-5): ");
                                int stars;
                                int.TryParse(Console.ReadLine(), out stars);
                                Console.Write("  Review: ");
                                string rev = Console.ReadLine();
                                rc.AddRating(stars, rev);
                            }
                        }
                        Console.ReadKey();
                        break;

                    case "7":
                        Console.Clear();
                        Console.WriteLine($"  === Notifications for {student.Username} ===\n");
                        var notifs = student.GetNotificationHistory();
                        if (notifs.Count == 0) Console.WriteLine("  No notifications.");
                        else foreach (string n in notifs) Console.WriteLine($"  {n}");
                        Console.ReadKey();
                        break;

                    case "8":
                        Console.Clear();
                        student.DisplayReport();
                        Console.ReadKey();
                        break;

                    case "9":
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
        //  INSTRUCTOR DASHBOARD
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
                Console.WriteLine("  7. My Notifications");
                Console.WriteLine("  8. My Report");
                Console.WriteLine("  9. Logout");
                Console.WriteLine("================================");
                Console.Write("  Choice: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Clear();
                        instructor.DisplayInfo();
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
                            if (c == null) Console.WriteLine("  Course not found.");
                            else instructor.AddCourse(addId);
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
                            foreach (int id in instructor.CourseIds)
                            {
                                Course c = courseList.Find(co => co.CourseId == id);
                                if (c != null)
                                {
                                    c.DisplayCourseInfo();
                                    Console.WriteLine("  ---");
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
                            instructor.RemoveCourse(removeId);
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
                        Console.Clear();
                        Console.WriteLine($"  === Notifications for {instructor.Username} ===\n");
                        var notifs = instructor.GetNotificationHistory();
                        if (notifs.Count == 0) Console.WriteLine("  No notifications.");
                        else foreach (string n in notifs) Console.WriteLine($"  {n}");
                        Console.ReadKey();
                        break;

                    case "8":
                        Console.Clear();
                        instructor.DisplayReport();
                        Console.ReadKey();
                        break;

                    case "9":
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
        //  ADMIN DASHBOARD
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
                Console.WriteLine("  7. My Report");
                Console.WriteLine("  8. Logout");
                Console.WriteLine("================================");
                Console.Write("  Choice: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Clear();
                        admin.DisplayInfo();
                        admin.DisplayPermissions();
                        Console.ReadKey();
                        break;

                    case "2":
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
                        int studentCount = 0, instructorCount = 0, adminCount = 0;
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
                        Console.Clear();
                        admin.DisplayReport();
                        Console.ReadKey();
                        break;

                    case "8":
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
            if (userList.Count == 0) { Console.WriteLine("  No users yet."); Console.ReadKey(); return; }
            foreach (User user in userList)
            {
                Console.WriteLine($"  [{user.GetUserType()}]");
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
                course.DisplayCourseInfo();
                Console.WriteLine($"  Est. Hours: {course.GetEstimatedHours()}h | Rating: {course.GetAverageRating():F1}/5");
                Console.WriteLine("  ---");
            }
        }

        // ══════════════════════════════════════════════════════
        //  UNIVERSAL SEARCH — ISearchable polymorphism demo
        // ══════════════════════════════════════════════════════
        static void UniversalSearch()
        {
            Console.Clear();
            Console.WriteLine("================================");
            Console.WriteLine("       UNIVERSAL SEARCH         ");
            Console.WriteLine("================================");
            Console.Write("  Enter keyword: ");
            string keyword = Console.ReadLine();

            List<ISearchable> searchableItems = new List<ISearchable>();
            foreach (Course c in courseList) searchableItems.Add(c);
            foreach (User u in userList) { if (u is ISearchable s) searchableItems.Add(s); }

            Console.WriteLine($"\n  Results for '{keyword}':\n");
            bool any = false;
            foreach (ISearchable item in searchableItems)
            {
                if (item.MatchesSearch(keyword))
                {
                    Console.WriteLine($"  ✓ {item.GetSearchSummary()}");
                    any = true;
                }
            }
            if (!any) Console.WriteLine("  No results found.");
            Console.ReadKey();
        }
    }
}