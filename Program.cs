using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Week_1;

namespace SmartLearnLMS
    {
    class Program
        {
        // ── Core collections ──
        static List<User> userList = new List<User>();
        static List<Course> courseList = new List<Course>();
        static List<Enrollment> enrollments = new List<Enrollment>();
        static User currentUser = null;

        // ── Dictionary lookups ──
        static Dictionary<int, Course> courseById = new Dictionary<int, Course>();
        static Dictionary<string, User> userByName = new Dictionary<string, User>();

        // ── JSON file paths — both use the same folder, consistent names ──
        static readonly string DataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        static readonly string UsersFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "users.json");
        static readonly string CoursesFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "courses.json");

        // ══════════════════════════════════════════════════════
        //  ENTRY POINT
        // ══════════════════════════════════════════════════════
        static void Main(string[] args)
            {
            LoadAllData();       // load persisted data first
            InitializeCourses(); // seed courses only if nothing was loaded

            bool running = true;
            while (running)
                running = ShowMainMenu();

            SaveAllData();       // save on clean exit
            }

        // ══════════════════════════════════════════════════════
        //  SYNC HELPERS — every Add goes through these so
        //  List<T> and Dictionary stay in sync always
        // ══════════════════════════════════════════════════════
        static void AddUser(User u)
            {
            userList.Add(u);
            userByName[u.Username] = u;
            }

        static void AddCourse(Course c)
            {
            courseList.Add(c);
            courseById[c.CourseId] = c;
            }

        // ══════════════════════════════════════════════════════
        //  SAMPLE DATA  — only runs when courseList is empty
        //  (i.e. first ever launch, no courses.json yet)
        // ══════════════════════════════════════════════════════
        static void InitializeCourses()
            {
            if (courseList.Count > 0) return;

            AddCourse(new OnlineCourse(101, "C# Fundamentals", "Learn C# from scratch", "Prof. Smith", "Programming", 450));
            AddCourse(new OnlineCourse(102, "Python for Beginners", "Intro to Python programming", "Prof. Johnson", "Programming", 360));
            AddCourse(new OnlineCourse(103, "Web Development Basics", "HTML, CSS and JS fundamentals", "Prof. Garcia", "Web Development", 540));
            AddCourse(new OnlineCourse(104, "Data Structures", "Arrays, Lists, Trees and more", "Prof. Smith", "Computer Science", 600));
            AddCourse(new OnlineCourse(105, "Machine Learning Intro", "Basics of ML and AI concepts", "Prof. Lee", "Data Science", 720));

            AddCourse(new InPersonCourse(201, "Database Design Workshop", "Relational DB and SQL", "Prof. Johnson", "Database", 25, "B-101", "Engineering Building"));
            AddCourse(new InPersonCourse(202, "Network Security Lab", "Practical cybersecurity skills", "Prof. Brown", "Security", 20, "C-205", "CS Building"));
            AddCourse(new InPersonCourse(203, "Mobile App Development", "Build iOS and Android apps", "Prof. Garcia", "Mobile", 30, "A-301", "Tech Center"));

            AddCourse(new HybridCourse(301, "Full-Stack Development", "Frontend + Backend full stack", "Prof. Garcia", "Web Development", 30, 720, "C-201", "CS Building"));
            AddCourse(new HybridCourse(302, "Cloud Computing", "AWS, Azure and cloud concepts", "Prof. Lee", "Cloud", 25, 600, "D-101", "Engineering Building"));

            // Save seed data immediately so courses.json exists from the start
            SaveAllData();
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
            Console.WriteLine("  [6] Analytics & LINQ Demo");
            Console.WriteLine("  [7] Exit");
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
                case "6": ShowAnalyticsMenu(); break;
                case "7":
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

            // ── Validation ──
            if (string.IsNullOrWhiteSpace(username) || username.Length < 3)
                { Console.WriteLine("  ✗ Username must be at least 3 characters."); Console.ReadKey(); return; }

            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
                { Console.WriteLine("  ✗ Invalid email - must contain @"); Console.ReadKey(); return; }

            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
                { Console.WriteLine("  ✗ Password must be at least 8 characters"); Console.ReadKey(); return; }

            if (!password.Any(char.IsDigit))
                { Console.WriteLine("  ✗ Password must contain at least 1 number"); Console.ReadKey(); return; }

            if (role != "Student" && role != "Instructor" && role != "Admin")
                { Console.WriteLine("  ✗ Role must be: Student, Instructor, or Admin"); Console.ReadKey(); return; }

            if (userByName.ContainsKey(username))
                { Console.WriteLine("  ✗ Username already exists!"); Console.ReadKey(); return; }

            // ── Create user ──
            User newUser;
            if (role == "Student") newUser = new Student(username, password, email);
            else if (role == "Instructor") newUser = new Instructor(username, password, email);
            else newUser = new Admin(username, password, email);

            // ── Add to collections ──
            AddUser(newUser);

            // ── Save immediately so it persists even if app crashes ──
            SaveAllData();

            Console.WriteLine($"\n  ✓ Registered as {role}! Welcome, {username}!");
            Console.WriteLine($"  ✓ Saved to: {UsersFile}");
            Console.ReadKey();
            }

        // ══════════════════════════════════════════════════════
        //  LOGIN
        // ══════════════════════════════════════════════════════
        static void LoginUser()
            {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════╗");
            Console.WriteLine("║              LOGIN             ║");
            Console.WriteLine("╚════════════════════════════════╝");

            Console.Write("  Username : "); string username = Console.ReadLine();
            Console.Write("  Password : "); string password = Console.ReadLine();

            // Dictionary lookup — O(1), instant
            if (!userByName.TryGetValue(username, out User found))
                { Console.WriteLine("  ✗ User not found!"); Console.ReadKey(); return; }

            if (!found.ValidatePassword(password))
                { Console.WriteLine("  ✗ Wrong password!"); Console.ReadKey(); return; }

            currentUser = found;
            Console.WriteLine($"\n  ✓ Welcome back, {found.Username}! [{found.GetUserType()}]");
            Console.ReadKey();

            RunDashboard(currentUser);
            }

        // ══════════════════════════════════════════════════════
        //  DASHBOARD RUNNER
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

                if (user is Student s) open = HandleStudentChoice(s, choice);
                else if (user is Instructor i) open = HandleInstructorChoice(i, choice);
                else if (user is Admin a) open = HandleAdminChoice(a, choice);
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
                            courseById.TryGetValue(id, out Course c);
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
            if (!int.TryParse(Console.ReadLine(), out int enrollId) || enrollId == 0) return;

            if (!courseById.TryGetValue(enrollId, out Course selected))
                { Console.WriteLine("  ✗ Course not found."); Console.ReadKey(); return; }
            if (!selected.CanEnroll(student))
                { Console.WriteLine("  ✗ Course is full!"); Console.ReadKey(); return; }
            if (student.EnrolledCourseIds.Contains(enrollId))
                { Console.WriteLine("  ✗ Already enrolled in this course."); Console.ReadKey(); return; }

            selected.Enroll(student);
            enrollments.Add(new Enrollment(enrollments.Count + 1, student.Username, enrollId, DateTime.Now, 0, false));
            SaveAllData(); // save after enrollment
            Console.ReadKey();
            }

        static void UpdateProgress(Student student)
            {
            if (student.EnrolledCourseIds.Count == 0)
                { Console.WriteLine("  No courses enrolled yet."); Console.ReadKey(); return; }

            Console.WriteLine($"  === Update Progress for {student.Username} ===\n");
            foreach (int id in student.EnrolledCourseIds)
                {
                courseById.TryGetValue(id, out Course c);
                int prog = student.CourseProgress.ContainsKey(id) ? student.CourseProgress[id] : 0;
                Console.WriteLine($"  [{id}] {c?.Title ?? "Unknown"} — {prog}%");
                }

            Console.Write("\n  Enter Course ID: ");
            if (!int.TryParse(Console.ReadLine(), out int cId)) { Console.WriteLine("  ✗ Invalid ID."); Console.ReadKey(); return; }
            Console.Write("  Enter Progress % (0-100): ");
            if (!int.TryParse(Console.ReadLine(), out int newProg)) { Console.WriteLine("  ✗ Invalid number."); Console.ReadKey(); return; }

            if (!student.CourseProgress.ContainsKey(cId))
                { Console.WriteLine("  ✗ Not enrolled in this course."); Console.ReadKey(); return; }

            student.ProgressPercentage = newProg;
            student.CourseProgress[cId] = newProg;
            Console.WriteLine($"  ✓ Progress updated to {newProg}%");
            if (newProg >= 100)
                {
                Console.WriteLine("  🎉 Course completed!");
                student.SendNotification($"You completed Course ID {cId}!");
                }
            SaveAllData(); // save after progress update
            Console.ReadKey();
            }

        static void RateCourse(Student student)
            {
            BrowseCourses();
            Console.Write("  Enter Course ID to rate: ");
            if (!int.TryParse(Console.ReadLine(), out int rId)) { Console.ReadKey(); return; }
            if (!courseById.TryGetValue(rId, out Course rc))
                { Console.WriteLine("  ✗ Course not found."); Console.ReadKey(); return; }
            Console.Write("  Stars (1-5): ");
            int.TryParse(Console.ReadLine(), out int stars);
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
                            if (courseById.TryGetValue(id, out Course c)) c.DisplayCourseInfo();
                            Console.WriteLine();
                            }
                        }
                    Console.ReadKey();
                    return true;

                case "2":
                    Console.Clear();
                    BrowseCourses();
                    Console.Write("  Enter Course ID to add to your list: ");
                    if (int.TryParse(Console.ReadLine(), out int addId))
                        {
                        if (!courseById.ContainsKey(addId)) Console.WriteLine("  ✗ Course not found.");
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
                        if (u is Student s) { s.DisplayInfo(); Console.WriteLine("  ---"); any = true; }
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
                        foreach (User u in userList)
                            {
                            Console.WriteLine($"  [{u.GetUserType()}]");
                            u.DisplayInfo();
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
            foreach (User u in userList)
                {
                Console.WriteLine($"  [{u.GetUserType()}]");
                u.DisplayInfo();
                Console.WriteLine("  ---");
                }
            Console.ReadKey();
            }

        static void BrowseCourses()
            {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════╗");
            Console.WriteLine("║       AVAILABLE COURSES        ║");
            Console.WriteLine("╚════════════════════════════════╝\n");
            foreach (Course c in courseList) { c.DisplayCourseInfo(); Console.WriteLine(); }
            }

        static void UniversalSearch()
            {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════╗");
            Console.WriteLine("║        UNIVERSAL SEARCH        ║");
            Console.WriteLine("╚════════════════════════════════╝");
            Console.Write("  Enter keyword: ");
            string keyword = Console.ReadLine();

            var searchable = new List<ISearchable>();
            foreach (Course c in courseList) searchable.Add(c);
            foreach (User u in userList) { if (u is ISearchable s) searchable.Add(s); }

            var results = SearchEngine.Search(searchable, keyword);
            Console.WriteLine();
            SearchEngine.DisplayResults(results);
            Console.ReadKey();
            }

        // ══════════════════════════════════════════════════════
        // ██  PART 2 – LINQ SEARCH METHODS
        // ══════════════════════════════════════════════════════
        static List<Course> SearchCourses(string keyword) =>
            courseList
                .Where(c => c.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                            (c.Description ?? "").Contains(keyword, StringComparison.OrdinalIgnoreCase))
                .OrderBy(c => c.Title).ToList();

        static List<Student> SearchStudents(string keyword) =>
            userList.OfType<Student>()
                .Where(s => s.Username.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                            (s.Email ?? "").Contains(keyword, StringComparison.OrdinalIgnoreCase))
                .OrderBy(s => s.Username).ToList();

        static List<Course> FilterCoursesByCategory(string category) =>
            courseList
                .Where(c => c.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(c => c.CurrentEnrollments).ToList();

        static List<Instructor> SearchInstructors(string keyword) =>
            userList.OfType<Instructor>()
                .Where(i => i.Username.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                            (i.Email ?? "").Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                            (i.Department ?? "").Contains(keyword, StringComparison.OrdinalIgnoreCase))
                .OrderBy(i => i.Username).ToList();

        // ══════════════════════════════════════════════════════
        // ██  PART 3 – LINQ ANALYTICS
        // ══════════════════════════════════════════════════════
        static List<Student> GetTopStudents(int count) =>
            userList.OfType<Student>()
                .OrderByDescending(s => s.CourseProgress.Count > 0 ? s.CourseProgress.Values.Average() : 0)
                .Take(count).ToList();

        static List<Student> GetActiveStudents() =>
            userList.OfType<Student>().Where(s => s.EnrolledCourseIds.Count >= 1).ToList();

        static Dictionary<string, List<Student>> GetStudentsByPerformance() =>
            userList.OfType<Student>()
                .GroupBy(s => {
                    double avg = s.CourseProgress.Count > 0 ? s.CourseProgress.Values.Average() : 0;
                    return avg < 50 ? "Low" : avg < 80 ? "Medium" : "High";
                })
                .ToDictionary(g => g.Key, g => g.ToList());

        static double CalculateSystemAverageProgress()
            {
            var all = userList.OfType<Student>().SelectMany(s => s.CourseProgress.Values).ToList();
            return all.Count > 0 ? all.Average() : 0;
            }

        static List<Course> GetPopularCourses(int count) =>
            courseList.OrderByDescending(c => c.CurrentEnrollments).Take(count).ToList();

        static List<Course> GetCoursesNeedingStudents() =>
            courseList.Where(c => c.CurrentEnrollments < 5).ToList();

        static List<Course> GetHighestRatedCourses(int count) =>
            courseList.OrderByDescending(c => c.GetAverageRating()).Take(count).ToList();

        static List<Course> GetCoursesByInstructor(string name) =>
            courseList.Where(c => c.InstructorName.Equals(name, StringComparison.OrdinalIgnoreCase)).ToList();

        static List<Course> GetCoursesByCategory(string category) =>
            courseList.Where(c => c.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(c => c.GetAverageRating()).ToList();

        static string GetMostEnrolledCategory() =>
            courseList.Any()
                ? courseList.GroupBy(c => c.Category)
                    .OrderByDescending(g => g.Sum(c => c.CurrentEnrollments)).First().Key
                : "N/A";

        static bool HasStudentCompletedAny() =>
            userList.OfType<Student>().Any(s => s.CourseProgress.Values.Any(p => p >= 100));

        static bool AreAllCoursesFilled()
            {
            var limited = courseList.Where(c => c is InPersonCourse || c is HybridCourse).ToList();
            return limited.Any() && limited.All(c => c.GetAvailableSeats() == 0);
            }

        // ── Advanced LINQ ──
        static void ShowStudentsByPerformanceLevel()
            {
            var groups = GetStudentsByPerformance();
            Console.WriteLine("\n  ── Student Performance Levels ──");
            foreach (string level in new[] { "High", "Medium", "Low" })
                {
                if (!groups.ContainsKey(level)) { Console.WriteLine($"  {level,-8}: (none)"); continue; }
                var list = groups[level];
                Console.WriteLine($"  {level,-8}: {list.Count} student(s)");
                foreach (var s in list) Console.WriteLine($"           • {s.Username}");
                }
            }

        static void ShowCourseStatistics()
            {
            if (!courseList.Any()) { Console.WriteLine("  No courses yet."); return; }
            Console.WriteLine("\n  ── Course Statistics ──");
            Console.WriteLine($"  Total courses  : {courseList.Count}");
            Console.WriteLine($"  Avg enrollment : {courseList.Average(c => c.CurrentEnrollments):F1}");
            var hi = courseList.OrderByDescending(c => c.CurrentEnrollments).First();
            var lo = courseList.OrderBy(c => c.CurrentEnrollments).First();
            Console.WriteLine($"  Most enrolled  : {hi.Title} ({hi.CurrentEnrollments})");
            Console.WriteLine($"  Least enrolled : {lo.Title} ({lo.CurrentEnrollments})");
            Console.WriteLine("  By category:");
            courseList.GroupBy(c => c.Category).OrderBy(g => g.Key).ToList()
                .ForEach(g => Console.WriteLine(
                    $"    {g.Key,-22}→ {g.Sum(c => c.CurrentEnrollments)} enrollments, {g.Count()} course(s)"));
            }

        static void ShowPaginatedCourses(int pageNumber, int pageSize)
            {
            var sorted = courseList.OrderBy(c => c.Title).ToList();
            int total = sorted.Count;
            int totalPages = (int)Math.Ceiling((double)total / pageSize);
            pageNumber = Math.Max(1, Math.Min(pageNumber, totalPages));
            var page = sorted.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            Console.WriteLine($"\n  ── Courses  Page {pageNumber} of {totalPages}  ({page.Count}/{total} shown) ──");
            foreach (var c in page)
                Console.WriteLine($"  • [{c.CourseId}] {c.Title} ({c.GetCourseType()})");
            }

        // ══════════════════════════════════════════════════════
        // ██  PART 4 – DISPLAY METHODS
        // ══════════════════════════════════════════════════════
        static void DisplaySearchResults(List<Course> results)
            {
            if (!results.Any()) { Console.WriteLine("  No courses found."); return; }
            Console.WriteLine($"\n  {"ID",-6}{"Title",-32}{"Type",-12}{"Category",-18}{"Enrolled",-10}Rating");
            Console.WriteLine("  " + new string('─', 80));
            foreach (var c in results)
                Console.WriteLine($"  {c.CourseId,-6}{c.Title,-32}{c.GetCourseType(),-12}{c.Category,-18}{c.CurrentEnrollments,-10}{c.GetAverageRating():F1}⭐");
            }

        static void DisplayStudentList(List<Student> students)
            {
            if (!students.Any()) { Console.WriteLine("  No students found."); return; }
            Console.WriteLine($"\n  {"Username",-20}{"Email",-28}{"Courses",-9}Avg Progress");
            Console.WriteLine("  " + new string('─', 65));
            foreach (var s in students)
                {
                double avg = s.CourseProgress.Count > 0 ? s.CourseProgress.Values.Average() : 0;
                Console.WriteLine($"  {s.Username,-20}{s.Email,-28}{s.EnrolledCourseIds.Count,-9}{avg:F1}%");
                }
            }

        static void DisplayAnalytics()
            {
            Console.WriteLine("\n╔══════════════════════════════════════════╗");
            Console.WriteLine("║       SYSTEM ANALYTICS DASHBOARD        ║");
            Console.WriteLine("╚══════════════════════════════════════════╝");
            int sc = userList.OfType<Student>().Count();
            int ic = userList.OfType<Instructor>().Count();
            Console.WriteLine($"  Total Users           : {userList.Count}  (Students:{sc}  Instructors:{ic})");
            Console.WriteLine($"  Total Courses         : {courseList.Count}");
            Console.WriteLine($"  Total Enrollments     : {enrollments.Count}");
            Console.WriteLine($"  System Avg Progress   : {CalculateSystemAverageProgress():F1}%");
            Console.WriteLine($"  Most Enrolled Category: {GetMostEnrolledCategory()}");
            Console.WriteLine($"  Any 100% Completions  : {(HasStudentCompletedAny() ? "Yes ✓" : "No")}");
            Console.WriteLine($"  All Limited Seats Full : {(AreAllCoursesFilled() ? "Yes" : "No")}");
            ShowCourseStatistics();
            ShowStudentsByPerformanceLevel();
            }

        static void DisplayCategoryBreakdown()
            {
            Console.WriteLine("\n  ── Courses by Category ──");
            foreach (string cat in courseList.Select(c => c.Category).Distinct().OrderBy(x => x))
                {
                var courses = GetCoursesByCategory(cat);
                Console.WriteLine($"\n  📂 {cat} ({courses.Count} course(s))");
                foreach (var c in courses)
                    Console.WriteLine($"      [{c.CourseId}] {c.Title,-30} {c.CurrentEnrollments} enrolled  {c.GetAverageRating():F1}⭐");
                }
            }

        // ══════════════════════════════════════════════════════
        // ██  ANALYTICS MENU
        // ══════════════════════════════════════════════════════
        static void ShowAnalyticsMenu()
            {
            bool back = false;
            while (!back)
                {
                Console.Clear();
                Console.WriteLine("╔════════════════════════════════════════╗");
                Console.WriteLine("║       ANALYTICS & LINQ DEMO            ║");
                Console.WriteLine("╚════════════════════════════════════════╝");
                Console.WriteLine("  [1]  System Analytics Dashboard");
                Console.WriteLine("  [2]  Category Breakdown");
                Console.WriteLine("  [3]  Search Courses by Keyword");
                Console.WriteLine("  [4]  Search Students by Keyword");
                Console.WriteLine("  [5]  Filter Courses by Category");
                Console.WriteLine("  [6]  Search Instructors by Keyword");
                Console.WriteLine("  [7]  Top 3 Students by Progress");
                Console.WriteLine("  [8]  Popular Courses (Top 5)");
                Console.WriteLine("  [9]  Highest-Rated Courses (Top 5)");
                Console.WriteLine("  [10] Courses by Instructor Name");
                Console.WriteLine("  [11] Paginated Course Browser");
                Console.WriteLine("  [12] Student Performance Levels");
                Console.WriteLine("  [0]  Back");
                Console.WriteLine("════════════════════════════════════════");
                Console.Write("  Choice: ");
                string ch = Console.ReadLine();
                Console.Clear();

                switch (ch)
                    {
                    case "1": DisplayAnalytics(); break;
                    case "2": DisplayCategoryBreakdown(); break;
                    case "3":
                        Console.Write("  Keyword: ");
                        DisplaySearchResults(SearchCourses(Console.ReadLine())); break;
                    case "4":
                        Console.Write("  Keyword: ");
                        DisplayStudentList(SearchStudents(Console.ReadLine())); break;
                    case "5":
                        Console.Write("  Category: ");
                        DisplaySearchResults(FilterCoursesByCategory(Console.ReadLine())); break;
                    case "6":
                        Console.Write("  Keyword: ");
                        var ir = SearchInstructors(Console.ReadLine());
                        if (!ir.Any()) Console.WriteLine("  No instructors found.");
                        else foreach (var i in ir) { i.DisplayInfo(); Console.WriteLine("  ---"); }
                        break;
                    case "7":
                        Console.WriteLine("  ── Top 3 Students ──");
                        DisplayStudentList(GetTopStudents(3)); break;
                    case "8":
                        Console.WriteLine("  ── Top 5 Popular Courses ──");
                        DisplaySearchResults(GetPopularCourses(5)); break;
                    case "9":
                        Console.WriteLine("  ── Top 5 Highest-Rated Courses ──");
                        DisplaySearchResults(GetHighestRatedCourses(5)); break;
                    case "10":
                        Console.Write("  Instructor name: ");
                        DisplaySearchResults(GetCoursesByInstructor(Console.ReadLine())); break;
                    case "11":
                        Console.Write("  Page number: ");
                        int.TryParse(Console.ReadLine(), out int pg);
                        Console.Write("  Page size  : ");
                        int.TryParse(Console.ReadLine(), out int ps);
                        if (ps < 1) ps = 3;
                        ShowPaginatedCourses(pg, ps); break;
                    case "12":
                        ShowStudentsByPerformanceLevel(); break;
                    case "0":
                        back = true; break;
                    default:
                        Console.WriteLine("  Invalid choice."); break;
                    }
                if (!back) Console.ReadKey();
                }
            }

        // ══════════════════════════════════════════════════════
        // ██  FILE PERSISTENCE
        //
        //  HOW IT WORKS:
        //  - SaveAllData() builds a List of plain dictionaries
        //    (one per user / course) and serializes to JSON.
        //  - Password is readable via User.Password public getter.
        //    On load it is passed back through the constructor
        //    which stores it directly (same bypass the seeded
        //    data uses — no validation runs on load).
        //  - No DTOs needed — we use Dictionary<string,object>
        //    to write and JsonElement to read back.
        //  - Both files are always written together so they
        //    never go out of sync.
        // ══════════════════════════════════════════════════════
        static void SaveAllData()
            {
            try
                {
                // Make sure the Data folder exists
                Directory.CreateDirectory(DataDir);

                var opts = new JsonSerializerOptions { WriteIndented = true };

                // ── Build user records ──────────────────────────
                var usersData = new List<Dictionary<string, object>>();

                foreach (User u in userList)
                    {
                    var record = new Dictionary<string, object>
                        {
                        ["Type"] = u.GetUserType(),  // "Student" | "Instructor" | "Admin"
                        ["Username"] = u.Username,
                        ["Password"] = u.Password,       // public getter — always readable
                        ["Email"] = u.Email ?? ""
                        };

                    if (u is Student s)
                        {
                        record["EnrolledCourseIds"] = s.EnrolledCourseIds;
                        record["CourseProgress"] = s.CourseProgress;
                        }
                    else if (u is Instructor ins)
                        {
                        record["CourseIds"] = ins.CourseIds;
                        }
                    // Admin has no extra fields beyond the base ones

                    usersData.Add(record);
                    }

                File.WriteAllText(UsersFile, JsonSerializer.Serialize(usersData, opts));

                // ── Build course records ────────────────────────
                var coursesData = new List<Dictionary<string, object>>();

                foreach (Course c in courseList)
                    {
                    var record = new Dictionary<string, object>
                        {
                        ["Type"] = c.GetCourseType(), // "Online" | "In-Person" | "Hybrid"
                        ["CourseId"] = c.CourseId,
                        ["Title"] = c.Title,
                        ["Description"] = c.Description ?? "",
                        ["InstructorName"] = c.InstructorName,
                        ["Category"] = c.Category,
                        ["CurrentEnrollments"] = c.CurrentEnrollments
                        };

                    if (c is OnlineCourse oc)
                        {
                        record["VideoDurationMinutes"] = oc.VideoDurationMinutes;
                        }
                    else if (c is InPersonCourse ip)
                        {
                        record["MaxStudents"] = ip.MaxStudents;
                        record["RoomNumber"] = ip.RoomNumber;
                        record["Building"] = ip.Building;
                        }
                    else if (c is HybridCourse hc)
                        {
                        record["MaxStudents"] = hc.MaxStudents;
                        record["RoomNumber"] = hc.RoomNumber;
                        record["Building"] = hc.Building;
                        record["OnlineVideoDuration"] = hc.OnlineVideoDuration;
                        }

                    coursesData.Add(record);
                    }

                File.WriteAllText(CoursesFile, JsonSerializer.Serialize(coursesData, opts));
                }
            catch (Exception ex)
                {
                Console.WriteLine($"\n  ✗ Save error: {ex.Message}");
                Console.ReadKey();
                }
            }

        static void LoadAllData()
            {
            try
                {
                // ── Load users ──────────────────────────────────
                if (File.Exists(UsersFile))
                    {
                    string json = File.ReadAllText(UsersFile);
                    var elements = JsonSerializer.Deserialize<List<JsonElement>>(json);

                    foreach (var el in elements)
                        {
                        // Read the three fields every user has
                        string type = el.GetProperty("Type").GetString();
                        string username = el.GetProperty("Username").GetString();
                        string password = el.GetProperty("Password").GetString();
                        string email = el.GetProperty("Email").GetString();

                        if (type == "Student")
                            {
                            var s = new Student(username, password, email);

                            // Restore enrolled course IDs
                            if (el.TryGetProperty("EnrolledCourseIds", out JsonElement ids))
                                foreach (var idEl in ids.EnumerateArray())
                                    s.EnrolledCourseIds.Add(idEl.GetInt32());

                            // Restore progress dictionary
                            if (el.TryGetProperty("CourseProgress", out JsonElement prog))
                                foreach (var kv in prog.EnumerateObject())
                                    s.CourseProgress[int.Parse(kv.Name)] = kv.Value.GetInt32();

                            AddUser(s);
                            }
                        else if (type == "Instructor")
                            {
                            var ins = new Instructor(username, password, email);

                            if (el.TryGetProperty("CourseIds", out JsonElement cids))
                                foreach (var idEl in cids.EnumerateArray())
                                    ins.CourseIds.Add(idEl.GetInt32());

                            AddUser(ins);
                            }
                        else if (type == "Admin")
                            {
                            AddUser(new Admin(username, password, email));
                            }
                        }

                    Console.WriteLine($"  📂 Loaded {userList.Count} user(s) from users.json");
                    }

                // ── Load courses ────────────────────────────────
                if (File.Exists(CoursesFile))
                    {
                    string json = File.ReadAllText(CoursesFile);
                    var elements = JsonSerializer.Deserialize<List<JsonElement>>(json);

                    foreach (var el in elements)
                        {
                        string type = el.GetProperty("Type").GetString();
                        int id = el.GetProperty("CourseId").GetInt32();
                        string title = el.GetProperty("Title").GetString();
                        string desc = el.GetProperty("Description").GetString();
                        string instr = el.GetProperty("InstructorName").GetString();
                        string cat = el.GetProperty("Category").GetString();
                        int enr = el.GetProperty("CurrentEnrollments").GetInt32();

                        Course c = null;

                        if (type == "Online")
                            {
                            int dur = el.GetProperty("VideoDurationMinutes").GetInt32();
                            c = new OnlineCourse(id, title, desc, instr, cat, dur);
                            }
                        else if (type == "In-Person")
                            {
                            int max = el.GetProperty("MaxStudents").GetInt32();
                            string room = el.GetProperty("RoomNumber").GetString();
                            string bldg = el.GetProperty("Building").GetString();
                            c = new InPersonCourse(id, title, desc, instr, cat, max, room, bldg);
                            }
                        else if (type == "Hybrid")
                            {
                            int max = el.GetProperty("MaxStudents").GetInt32();
                            string room = el.GetProperty("RoomNumber").GetString();
                            string bldg = el.GetProperty("Building").GetString();
                            int ovd = el.GetProperty("OnlineVideoDuration").GetInt32();
                            c = new HybridCourse(id, title, desc, instr, cat, max, ovd, room, bldg);
                            }

                        if (c != null)
                            {
                            c.CurrentEnrollments = enr;
                            AddCourse(c);
                            }
                        }

                    Console.WriteLine($"  📂 Loaded {courseList.Count} course(s) from courses.json");
                    }

                // Pause so user can see what was loaded
                if (userList.Count > 0 || courseList.Count > 0)
                    {
                    Console.WriteLine("  Press any key to continue...");
                    Console.ReadKey();
                    }
                }
            catch (Exception ex)
                {
                // If ANYTHING goes wrong during load, wipe bad state and start fresh
                Console.WriteLine($"\n  ✗ Load error: {ex.Message}");
                Console.WriteLine("  Deleting corrupted files and starting fresh...");

                // Clear any partial state
                userList.Clear();
                courseList.Clear();
                userByName.Clear();
                courseById.Clear();

                // Delete bad files so next run starts clean
                try { if (File.Exists(UsersFile)) File.Delete(UsersFile); } catch { }
                try { if (File.Exists(CoursesFile)) File.Delete(CoursesFile); } catch { }

                Console.WriteLine("  Press any key to continue...");
                Console.ReadKey();
                }
            }
        }
    }