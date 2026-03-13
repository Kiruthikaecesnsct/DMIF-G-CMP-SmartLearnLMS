using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Week_1;
using Week_1.Services;

namespace SmartLearnLMS
    {
    class Program
        {
        // ── Core collections ──
        static List<User> userList = new List<User>();
        static List<Course> courseList = new List<Course>();
        static List<Enrollment> enrollments = new List<Enrollment>();
        static User currentUser = null;

        // ── Dictionary lookups (O(1) access) ──
        static Dictionary<int, Course> courseById = new Dictionary<int, Course>();
        static Dictionary<string, User> userByName = new Dictionary<string, User>();

        // ── JSON file paths ──
        static readonly string DataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        static readonly string UsersFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "users.json");
        static readonly string CoursesFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "courses.json");

        // ══════════════════════════════════════════════════════
        //  DEPENDENCY INJECTION
        //  Services are created ONCE here and passed where needed.
        //  To swap an implementation, only change these four lines.
        // ══════════════════════════════════════════════════════
        static readonly EnrollmentService enrollmentService = new EnrollmentService();
        static readonly NotificationService notificationService = new NotificationService();
        static readonly SearchService searchService = new SearchService();
        static readonly ReportService reportService = new ReportService();

        // ══════════════════════════════════════════════════════
        //  ENTRY POINT
        // ══════════════════════════════════════════════════════
        static void Main(string[] args)
            {
            LoadAllData();       // load persisted data first
            InitializeCourses(); // seed only when courseList is empty

            bool running = true;
            while (running)
                running = ShowMainMenu();

            SaveAllData();       // save on clean exit
            }

        // ══════════════════════════════════════════════════════
        //  SYNC HELPERS — keep List<T> + Dictionary in sync
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
        //  SAMPLE DATA — only when list is empty (first launch)
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

            SaveAllData(); // persist seed data immediately
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
            Console.WriteLine("  [7] Services Demo (Assignment 5)");
            Console.WriteLine("  [8] Exit");
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
                case "7": ShowServicesDemo(); break;
                case "8":
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
                { Console.WriteLine("  ✗ Invalid email — must contain @"); Console.ReadKey(); return; }
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
                { Console.WriteLine("  ✗ Password must be at least 8 characters"); Console.ReadKey(); return; }
            if (!password.Any(char.IsDigit))
                { Console.WriteLine("  ✗ Password must contain at least 1 number"); Console.ReadKey(); return; }
            if (role != "Student" && role != "Instructor" && role != "Admin")
                { Console.WriteLine("  ✗ Role must be: Student, Instructor, or Admin"); Console.ReadKey(); return; }
            if (userByName.ContainsKey(username))
                { Console.WriteLine("  ✗ Username already exists!"); Console.ReadKey(); return; }

            User newUser;
            if (role == "Student") newUser = new Student(username, password, email);
            else if (role == "Instructor") newUser = new Instructor(username, password, email);
            else newUser = new Admin(username, password, email);

            AddUser(newUser);
            SaveAllData(); // auto-save immediately

            Console.WriteLine($"\n  ✓ Registered as {role}! Welcome, {username}!");
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
                        foreach (int id in student.EnrolledCourseIds)
                            {
                            courseById.TryGetValue(id, out Course c);
                            int prog = student.CourseProgress.ContainsKey(id) ? student.CourseProgress[id] : 0;
                            Console.WriteLine($"  [{id}] {c?.Title ?? "Unknown"} — Progress: {prog}%");
                            }
                    Console.ReadKey();
                    return true;

                case "3":
                    Console.Clear();
                    UpdateProgress(student);
                    return true;

                case "4":
                    Console.Clear();
                    // Use ReportService (DI) instead of calling DisplayInfo directly
                    reportService.DisplayReport(student);
                    Console.ReadKey();
                    return true;

                case "5":
                    Console.Clear();
                    RateCourse(student);
                    return true;

                case "6":
                    Console.Clear();
                    Console.WriteLine($"  === Notifications for {student.Username} ===\n");
                    // Use NotificationService (DI)
                    notificationService.DisplayHistory(student);
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
            if (student.EnrolledCourseIds.Contains(enrollId))
                { Console.WriteLine("  ✗ Already enrolled in this course."); Console.ReadKey(); return; }

            // Use EnrollmentService (DI) — works on IEnrollable, not Course specifically
            bool ok = enrollmentService.EnrollStudent(selected, student);
            if (ok)
                {
                // Use NotificationService (DI) — works on INotifiable
                notificationService.NotifyEnrollment(student, selected.Title);
                enrollments.Add(new Enrollment(enrollments.Count + 1, student.Username, enrollId, DateTime.Now, 0, false));
                SaveAllData();
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
                courseById.TryGetValue(id, out Course c);
                int prog = student.CourseProgress.ContainsKey(id) ? student.CourseProgress[id] : 0;
                Console.WriteLine($"  [{id}] {c?.Title ?? "Unknown"} — {prog}%");
                }

            Console.Write("\n  Enter Course ID: ");
            if (!int.TryParse(Console.ReadLine(), out int cId))
                { Console.WriteLine("  ✗ Invalid ID."); Console.ReadKey(); return; }
            Console.Write("  Enter Progress % (0-100): ");
            if (!int.TryParse(Console.ReadLine(), out int newProg))
                { Console.WriteLine("  ✗ Invalid number."); Console.ReadKey(); return; }

            if (!student.CourseProgress.ContainsKey(cId))
                { Console.WriteLine("  ✗ Not enrolled in this course."); Console.ReadKey(); return; }

            student.ProgressPercentage = newProg;
            student.CourseProgress[cId] = newProg;
            Console.WriteLine($"  ✓ Progress updated to {newProg}%");

            // Use NotificationService milestone method (DI)
            notificationService.NotifyProgressMilestone(student, newProg);

            SaveAllData();
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
                    else foreach (int id in instructor.CourseIds)
                            {
                            if (courseById.TryGetValue(id, out Course c)) c.DisplayCourseInfo();
                            Console.WriteLine();
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
                        if (u is Student s) { s.DisplayInfo(); Console.WriteLine("  ---"); any = true; }
                    if (!any) Console.WriteLine("  No students registered.");
                    Console.ReadKey();
                    return true;

                case "4":
                    Console.Clear();
                    Console.WriteLine("  [Grade Assignments — Coming in Week 6]");
                    Console.ReadKey();
                    return true;

                case "5":
                    Console.Clear();
                    Console.WriteLine($"  === Notifications for {instructor.Username} ===\n");
                    notificationService.DisplayHistory(instructor);
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
                    else foreach (User u in userList)
                            {
                            Console.WriteLine($"  [{u.GetUserType()}]");
                            u.DisplayInfo();
                            Console.WriteLine("  ---");
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
                    int st = 0, ins = 0, adm = 0;
                    foreach (User u in userList)
                        {
                        if (u is Student) st++;
                        else if (u is Instructor) ins++;
                        else if (u is Admin) adm++;
                        }
                    Console.WriteLine($"  Total Users      : {userList.Count}");
                    Console.WriteLine($"  Students         : {st}");
                    Console.WriteLine($"  Instructors      : {ins}");
                    Console.WriteLine($"  Admins           : {adm}");
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

            // Use SearchService (DI) — generic, works on any ISearchable
            var courseResults = searchService.Search(courseList, keyword);
            var studentResults = searchService.Search(userList.OfType<Student>().ToList(), keyword);

            searchService.DisplayResults(courseResults, keyword + " (courses)");
            searchService.DisplayResults(studentResults, keyword + " (students)");
            Console.ReadKey();
            }

        // ══════════════════════════════════════════════════════
        // ██  ASSIGNMENT 5 SERVICES DEMO  (menu option 7)
        //     Shows DI, interface polymorphism, and SRP in action
        // ══════════════════════════════════════════════════════
        static void ShowServicesDemo()
            {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════╗");
            Console.WriteLine("║   ASSIGNMENT 5 — SERVICES DEMO         ║");
            Console.WriteLine("╚════════════════════════════════════════╝\n");

            // ── Use seeded students or live registered ones ──
            var demoStudents = userList.OfType<Student>().ToList();
            if (demoStudents.Count == 0)
                {
                Console.WriteLine("  No students registered yet. Register a student first.\n");
                Console.ReadKey(); return;
                }
            Student demo = demoStudents.First();
            Course c101 = courseById.ContainsKey(101) ? courseById[101] : courseList.First();

            // ── EnrollmentService demo ──────────────────────
            Console.WriteLine("══ EnrollmentService ══");
            enrollmentService.DisplayEnrollmentStatus(c101);
            bool enrolled = enrollmentService.EnrollStudent(c101, demo);
            if (enrolled) SaveAllData();

            // ── NotificationService demo ────────────────────
            Console.WriteLine("\n══ NotificationService ══");
            notificationService.NotifyEnrollment(demo, c101.Title);
            notificationService.NotifyProgressMilestone(demo, 50);
            notificationService.NotifyProgressMilestone(demo, 75);
            notificationService.NotifyProgressMilestone(demo, 100);

            Console.WriteLine($"\n  Broadcasting to {demoStudents.Count} student(s):");
            notificationService.NotifyAll(demoStudents, "📢 New semester courses are now live!");

            Console.WriteLine($"\n  Notification history for {demo.Username}:");
            notificationService.DisplayHistory(demo);

            // ── SearchService demo (generic!) ───────────────
            Console.WriteLine("\n══ SearchService (generic — same service, different types) ══");
            var cResults = searchService.Search(courseList, "python");
            searchService.DisplayResults(cResults, "python");

            var sResults = searchService.Search(demoStudents, demo.Username.Substring(0, 2));
            searchService.DisplayResults(sResults, demo.Username.Substring(0, 2));

            // ── ReportService demo ──────────────────────────
            Console.WriteLine("\n══ ReportService ══");
            reportService.DisplayReport(demo);     // Student report
            reportService.DisplayReport(c101);     // Course report

            // Show all student reports (first 2)
            Console.WriteLine("  ── All Student Reports (first 2) ──");
            reportService.DisplayAllReports(demoStudents.Take(2).Cast<IReportable>());

            Console.WriteLine("\n  ✓ Services demo complete. Press any key...");
            Console.ReadKey();
            }

        // ══════════════════════════════════════════════════════
        // ██  PART 2 — LINQ SEARCH METHODS
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
        // ██  PART 3 — LINQ ANALYTICS (delegates to Analytics.cs)
        // ══════════════════════════════════════════════════════
        static void ShowStudentsByPerformanceLevel()
            {
            var groups = Analytics.GetStudentsByPerformance(userList);
            Console.WriteLine("\n  ── Student Performance Levels ──");
            foreach (string level in new[] { "High", "Medium", "Low" })
                {
                var list = groups.ContainsKey(level) ? groups[level] : new List<Student>();
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
        // ██  PART 4 — DISPLAY METHODS
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
            // Delegates to Analytics static class (from Assignment 4)
            Analytics.DisplayAnalytics(userList, courseList);
            }

        static void DisplayCategoryBreakdown()
            => Analytics.DisplayCategoryBreakdown(courseList);

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
                        DisplayStudentList(Analytics.GetTopStudents(userList, 3)); break;
                    case "8":
                        Console.WriteLine("  ── Top 5 Popular Courses ──");
                        DisplaySearchResults(Analytics.GetPopularCourses(courseList, 5)); break;
                    case "9":
                        Console.WriteLine("  ── Top 5 Highest-Rated Courses ──");
                        DisplaySearchResults(Analytics.GetHighestRatedCourses(courseList, 5)); break;
                    case "10":
                        Console.Write("  Instructor name: ");
                        DisplaySearchResults(Analytics.GetCoursesByInstructor(courseList, Console.ReadLine())); break;
                    case "11":
                        Console.Write("  Page number: "); int.TryParse(Console.ReadLine(), out int pg);
                        Console.Write("  Page size  : "); int.TryParse(Console.ReadLine(), out int ps);
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
        // ══════════════════════════════════════════════════════
        static void SaveAllData()
            {
            try
                {
                Directory.CreateDirectory(DataDir);
                var opts = new JsonSerializerOptions { WriteIndented = true };

                // ── Users ──
                var usersData = new List<Dictionary<string, object>>();
                foreach (User u in userList)
                    {
                    var r = new Dictionary<string, object>
                        {
                        ["Type"] = u.GetUserType(),
                        ["Username"] = u.Username,
                        ["Password"] = u.Password,
                        ["Email"] = u.Email ?? ""
                        };
                    if (u is Student s)
                        { r["EnrolledCourseIds"] = s.EnrolledCourseIds; r["CourseProgress"] = s.CourseProgress; }
                    else if (u is Instructor ins)
                        { r["CourseIds"] = ins.CourseIds; }
                    usersData.Add(r);
                    }
                File.WriteAllText(UsersFile, JsonSerializer.Serialize(usersData, opts));

                // ── Courses ──
                var coursesData = new List<Dictionary<string, object>>();
                foreach (Course c in courseList)
                    {
                    var r = new Dictionary<string, object>
                        {
                        ["Type"] = c.GetCourseType(),
                        ["CourseId"] = c.CourseId,
                        ["Title"] = c.Title,
                        ["Description"] = c.Description ?? "",
                        ["InstructorName"] = c.InstructorName,
                        ["Category"] = c.Category,
                        ["CurrentEnrollments"] = c.CurrentEnrollments
                        };
                    if (c is OnlineCourse oc)
                        r["VideoDurationMinutes"] = oc.VideoDurationMinutes;
                    else if (c is InPersonCourse ip)
                        { r["MaxStudents"] = ip.MaxStudents; r["RoomNumber"] = ip.RoomNumber; r["Building"] = ip.Building; }
                    else if (c is HybridCourse hc)
                        { r["MaxStudents"] = hc.MaxStudents; r["RoomNumber"] = hc.RoomNumber; r["Building"] = hc.Building; r["OnlineVideoDuration"] = hc.OnlineVideoDuration; }
                    coursesData.Add(r);
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
                if (File.Exists(UsersFile))
                    {
                    var els = JsonSerializer.Deserialize<List<System.Text.Json.JsonElement>>(File.ReadAllText(UsersFile));
                    foreach (var el in els)
                        {
                        string type = el.GetProperty("Type").GetString();
                        string user = el.GetProperty("Username").GetString();
                        string pass = el.GetProperty("Password").GetString();
                        string mail = el.GetProperty("Email").GetString();

                        if (type == "Student")
                            {
                            var s = new Student(user, pass, mail);
                            if (el.TryGetProperty("EnrolledCourseIds", out var ids))
                                foreach (var id in ids.EnumerateArray())
                                    s.EnrolledCourseIds.Add(id.GetInt32());
                            if (el.TryGetProperty("CourseProgress", out var prog))
                                foreach (var kv in prog.EnumerateObject())
                                    s.CourseProgress[int.Parse(kv.Name)] = kv.Value.GetInt32();
                            AddUser(s);
                            }
                        else if (type == "Instructor")
                            {
                            var i = new Instructor(user, pass, mail);
                            if (el.TryGetProperty("CourseIds", out var cids))
                                foreach (var id in cids.EnumerateArray())
                                    i.CourseIds.Add(id.GetInt32());
                            AddUser(i);
                            }
                        else if (type == "Admin")
                            AddUser(new Admin(user, pass, mail));
                        }
                    Console.WriteLine($"  📂 Loaded {userList.Count} user(s).");
                    }

                if (File.Exists(CoursesFile))
                    {
                    var els = JsonSerializer.Deserialize<List<System.Text.Json.JsonElement>>(File.ReadAllText(CoursesFile));
                    foreach (var el in els)
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
                            c = new OnlineCourse(id, title, desc, instr, cat, el.GetProperty("VideoDurationMinutes").GetInt32());
                        else if (type == "In-Person")
                            c = new InPersonCourse(id, title, desc, instr, cat,
                                    el.GetProperty("MaxStudents").GetInt32(),
                                    el.GetProperty("RoomNumber").GetString(),
                                    el.GetProperty("Building").GetString());
                        else if (type == "Hybrid")
                            c = new HybridCourse(id, title, desc, instr, cat,
                                    el.GetProperty("MaxStudents").GetInt32(),
                                    el.GetProperty("OnlineVideoDuration").GetInt32(),
                                    el.GetProperty("RoomNumber").GetString(),
                                    el.GetProperty("Building").GetString());

                        if (c != null) { c.CurrentEnrollments = enr; AddCourse(c); }
                        }
                    Console.WriteLine($"  📂 Loaded {courseList.Count} course(s).");
                    }

                if (userList.Count > 0 || courseList.Count > 0)
                    { Console.WriteLine("  Press any key to continue..."); Console.ReadKey(); }
                }
            catch (Exception ex)
                {
                Console.WriteLine($"\n  ✗ Load error: {ex.Message} — starting fresh.");
                userList.Clear(); courseList.Clear();
                userByName.Clear(); courseById.Clear();
                try { if (File.Exists(UsersFile)) File.Delete(UsersFile); } catch { }
                try { if (File.Exists(CoursesFile)) File.Delete(CoursesFile); } catch { }
                Console.ReadKey();
                }
            }
        }
    }