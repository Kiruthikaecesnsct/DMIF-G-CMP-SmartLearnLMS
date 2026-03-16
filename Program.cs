using System;
using System.Collections.Generic;
using System.Linq;
using Week_1;
using Week_1.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

// ═══════════════════════════════════════════════════════════════════════════════
//  SmartLearn LMS — Assignment 6 (Full DB Edition)
//  ALL data operations go through EF Core / ADO.NET.
//  JSON persistence has been completely removed.
//  In-memory lists are gone — every read/write hits SmartLearnDbContext.
// ═══════════════════════════════════════════════════════════════════════════════

namespace SmartLearnLMS
    {
    class Program
        {
        // ── Currently logged-in user (held in memory only for session) ──
        static UserEntity currentUser = null;

        // ── Services (DI — created once) ──
        static readonly AdoNetService adoService = new AdoNetService();
        static readonly EfStudentService efStudents = new EfStudentService();
        static readonly EfCourseService efCourses = new EfCourseService();
        static readonly EfEnrollmentService efEnrollments = new EfEnrollmentService();
        static readonly EfInstructorService efInstructors = new EfInstructorService();
        static readonly EfAdminService efAdmin = new EfAdminService();

        // ── Original Assignment 5 services (still used for notifications/reports/search) ──
        static readonly NotificationService notifService = new NotificationService();
        static readonly ReportService reportService = new ReportService();
        static readonly SearchService searchService = new SearchService();
        static readonly EnrollmentService enrollSvc = new EnrollmentService();

        // ════════════════════════════════════════════════════════
        //  ENTRY POINT
        // ════════════════════════════════════════════════════════
        static void Main(string[] args)
            {
            Console.Clear();
            Console.WriteLine("  Initialising SmartLearn LMS...");

            // Verify DB connection
            bool dbOk = adoService.TestConnection();
            if (!dbOk)
                {
                Console.WriteLine("\n  ✗ Cannot reach SQL Server.");
                Console.WriteLine("  Make sure SmartLearn DB is running and connection string is correct.");
                Console.WriteLine("  Press any key to exit.");
                Console.ReadKey();
                return;
                }

            // Seed courses on first run (only if Courses table is empty)
            SeedCoursesIfEmpty();

            bool running = true;
            while (running)
                running = ShowMainMenu();
            }

        // ════════════════════════════════════════════════════════
        //  DB SEED — runs once when Courses table is empty
        // ════════════════════════════════════════════════════════
        static void SeedCoursesIfEmpty()
            {
            try
                {
                using var ctx = new SmartLearnDbContext();
                if (ctx.Courses.Any()) return;

                Console.WriteLine("  Seeding initial course data...");
                var seed = new List<CourseEntity>
                {
                    new() { Title="C# Fundamentals",         Description="Learn C# from scratch",            Category="Programming",     DifficultyLevel="Beginner",     CourseType="Online",   MaxCapacity=999, InstructorName="Prof. Smith" },
                    new() { Title="Python for Beginners",    Description="Intro to Python programming",      Category="Programming",     DifficultyLevel="Beginner",     CourseType="Online",   MaxCapacity=999, InstructorName="Prof. Johnson" },
                    new() { Title="Web Development Basics",  Description="HTML, CSS and JS fundamentals",    Category="Web Development", DifficultyLevel="Beginner",     CourseType="Online",   MaxCapacity=999, InstructorName="Prof. Garcia" },
                    new() { Title="Data Structures",         Description="Arrays, Lists, Trees and more",    Category="Computer Science",DifficultyLevel="Intermediate", CourseType="Online",   MaxCapacity=999, InstructorName="Prof. Smith" },
                    new() { Title="Machine Learning Intro",  Description="Basics of ML and AI concepts",     Category="Data Science",    DifficultyLevel="Intermediate", CourseType="Online",   MaxCapacity=999, InstructorName="Prof. Lee" },
                    new() { Title="Database Design Workshop",Description="Relational DB and SQL",            Category="Database",        DifficultyLevel="Intermediate", CourseType="InPerson", MaxCapacity=25,  InstructorName="Prof. Johnson" },
                    new() { Title="Network Security Lab",    Description="Practical cybersecurity skills",   Category="Security",        DifficultyLevel="Advanced",     CourseType="InPerson", MaxCapacity=20,  InstructorName="Prof. Brown" },
                    new() { Title="Mobile App Development",  Description="Build iOS and Android apps",       Category="Mobile",          DifficultyLevel="Intermediate", CourseType="InPerson", MaxCapacity=30,  InstructorName="Prof. Garcia" },
                    new() { Title="Full-Stack Development",  Description="Frontend + Backend full stack",    Category="Web Development", DifficultyLevel="Advanced",     CourseType="Hybrid",   MaxCapacity=30,  InstructorName="Prof. Garcia" },
                    new() { Title="Cloud Computing",         Description="AWS, Azure and cloud concepts",    Category="Cloud",           DifficultyLevel="Intermediate", CourseType="Hybrid",   MaxCapacity=25,  InstructorName="Prof. Lee" },
                };
                ctx.Courses.AddRange(seed);
                ctx.SaveChanges();
                Console.WriteLine($"  ✓ {seed.Count} courses seeded.");
                }
            catch (Exception ex) { Console.WriteLine($"  ⚠ Seed warning: {ex.Message}"); }
            }

        // ════════════════════════════════════════════════════════
        //  MAIN MENU
        // ════════════════════════════════════════════════════════
        static bool ShowMainMenu()
            {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════╗");
            Console.WriteLine("║    Welcome to SmartLearn LMS   ║");
            Console.WriteLine("║      (Database Edition)        ║");
            Console.WriteLine("╚════════════════════════════════╝");
            Console.WriteLine("  [1] Register");
            Console.WriteLine("  [2] Login");
            Console.WriteLine("  [3] Browse All Courses");
            Console.WriteLine("  [4] Search Courses");
            Console.WriteLine("  [5] Analytics & Reports");
            Console.WriteLine("  [6] Exit");
            Console.WriteLine("════════════════════════════════");
            Console.Write("  Choice: ");

            switch (Console.ReadLine())
                {
                case "1": RegisterUser(); break;
                case "2": LoginUser(); break;
                case "3": BrowseAllCourses(); break;
                case "4": SearchCourses(); break;
                case "5": ShowAnalyticsMenu(); break;
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

        // ════════════════════════════════════════════════════════
        //  REGISTER  — writes directly to DB
        // ════════════════════════════════════════════════════════
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

            // Validation
            if (string.IsNullOrWhiteSpace(username) || username.Length < 3)
                { Console.WriteLine("  ✗ Username must be at least 3 characters."); Console.ReadKey(); return; }
            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
                { Console.WriteLine("  ✗ Invalid email — must contain @"); Console.ReadKey(); return; }
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
                { Console.WriteLine("  ✗ Password must be at least 8 characters."); Console.ReadKey(); return; }
            if (!password.Any(char.IsDigit))
                { Console.WriteLine("  ✗ Password must contain at least 1 digit."); Console.ReadKey(); return; }
            if (role != "Student" && role != "Instructor" && role != "Admin")
                { Console.WriteLine("  ✗ Role must be: Student, Instructor, or Admin"); Console.ReadKey(); return; }

            // Write to DB — duplicate check is inside CreateUser
            var created = efAdmin.CreateUser(username, email, password, role);
            if (created != null)
                Console.WriteLine($"\n  ✓ Registered as {role}! Welcome, {username}! (DB ID: {created.UserId})");
            Console.ReadKey();
            }

        // ════════════════════════════════════════════════════════
        //  LOGIN  — queries DB, updates LastLoginDate
        // ════════════════════════════════════════════════════════
        static void LoginUser()
            {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════╗");
            Console.WriteLine("║              LOGIN             ║");
            Console.WriteLine("╚════════════════════════════════╝");

            Console.Write("  Username : "); string username = Console.ReadLine();
            Console.Write("  Password : "); string password = Console.ReadLine();

            try
                {
                using var ctx = new SmartLearnDbContext();
                var user = ctx.Users.FirstOrDefault(u => u.Username == username && u.PasswordHash == password);
                if (user == null)
                    { Console.WriteLine("  ✗ Invalid username or password."); Console.ReadKey(); return; }
                if (!user.IsActive)
                    { Console.WriteLine("  ✗ Account is deactivated. Contact admin."); Console.ReadKey(); return; }

                // Update last login
                user.LastLoginDate = DateTime.Now;
                ctx.SaveChanges();

                currentUser = user;
                Console.WriteLine($"\n  ✓ Welcome back, {user.Username}! [{user.UserType}]");
                Console.ReadKey();
                RunDashboard();
                }
            catch (Exception ex) { Console.WriteLine($"  ✗ Login error: {ex.Message}"); Console.ReadKey(); }
            }

        // ════════════════════════════════════════════════════════
        //  DASHBOARD ROUTER
        // ════════════════════════════════════════════════════════
        static void RunDashboard()
            {
            bool open = true;
            while (open)
                {
                Console.Clear();
                PrintDashboardHeader();
                Console.Write("  Choice: ");
                string choice = Console.ReadLine();

                open = currentUser.UserType switch
                    {
                        "Student" => HandleStudentChoice(choice),
                        "Instructor" => HandleInstructorChoice(choice),
                        "Admin" => HandleAdminChoice(choice),
                        _ => false
                        };
                }
            }

        static void PrintDashboardHeader()
            {
            switch (currentUser.UserType)
                {
                case "Student":
                    Console.WriteLine("╔════════════════════════════════╗");
                    Console.WriteLine("║        STUDENT DASHBOARD       ║");
                    Console.WriteLine("╚════════════════════════════════╝");
                    Console.WriteLine($"  Welcome, {currentUser.Username}!");
                    Console.WriteLine($"  Last Login: {currentUser.LastLoginDate?.ToString("g") ?? "First time"}");
                    Console.WriteLine();
                    Console.WriteLine("  [1]  Browse & Enroll in Courses");
                    Console.WriteLine("  [2]  My Enrolled Courses");
                    Console.WriteLine("  [3]  Update My Progress");
                    Console.WriteLine("  [4]  My Profile & Report");
                    Console.WriteLine("  [5]  Drop a Course");
                    Console.WriteLine("  [6]  Search Courses");
                    Console.WriteLine("  [7]  My Active Courses");
                    Console.WriteLine("  [8]  Students Above Progress %");
                    Console.WriteLine("  [9]  Filter by Progress + Category");
                    Console.WriteLine("  [10] ADO.NET — View My Enrollments");
                    Console.WriteLine("  [11] ADO.NET — View All Students");
                    Console.WriteLine("  [12] ADO.NET — View All Courses");
                    Console.WriteLine("  [0]  Logout");
                    break;

                case "Instructor":
                    Console.WriteLine("╔════════════════════════════════╗");
                    Console.WriteLine("║      INSTRUCTOR DASHBOARD      ║");
                    Console.WriteLine("╚════════════════════════════════╝");
                    Console.WriteLine($"  Welcome, {currentUser.Username}!");
                    Console.WriteLine($"  Last Login: {currentUser.LastLoginDate?.ToString("g") ?? "First time"}");
                    Console.WriteLine();
                    Console.WriteLine("  [1]  My Courses");
                    Console.WriteLine("  [2]  Create New Course");
                    Console.WriteLine("  [3]  View Student Roster (all students)");
                    Console.WriteLine("  [4]  View Enrollments for My Course");
                    Console.WriteLine("  [5]  Search Courses");
                    Console.WriteLine("  [6]  Top Courses by Enrollment");
                    Console.WriteLine("  [7]  Enrollments by Category");
                    Console.WriteLine("  [8]  Change Enrollment Status");
                    Console.WriteLine("  [9]  My Profile (with all courses)");
                    Console.WriteLine("  [10] My Courses + Students (deep load)");
                    Console.WriteLine("  [11] ADO.NET — Direct Course Insert");
                    Console.WriteLine("  [12] ADO.NET — View All Students");
                    Console.WriteLine("  [0]  Logout");
                    break;

                case "Admin":
                    Console.WriteLine("╔════════════════════════════════╗");
                    Console.WriteLine("║        ADMIN DASHBOARD         ║");
                    Console.WriteLine("╚════════════════════════════════╝");
                    Console.WriteLine($"  Welcome, Admin {currentUser.Username}!");
                    Console.WriteLine($"  Last Login: {currentUser.LastLoginDate?.ToString("g") ?? "First time"}");
                    Console.WriteLine();
                    Console.WriteLine("  [1]  Manage Users");
                    Console.WriteLine("  [2]  Manage Courses");
                    Console.WriteLine("  [3]  Manage Enrollments");
                    Console.WriteLine("  [4]  System Analytics Dashboard");
                    Console.WriteLine("  [5]  Aggregation Report");
                    Console.WriteLine("  [6]  ADO.NET Operations");
                    Console.WriteLine("  [7]  Advanced Queries");
                    Console.WriteLine("  [0]  Logout");
                    break;
                }
            Console.WriteLine("════════════════════════════════");
            }

        // ════════════════════════════════════════════════════════
        //  STUDENT HANDLERS
        // ════════════════════════════════════════════════════════
        static bool HandleStudentChoice(string choice)
            {
            Console.Clear();
            switch (choice)
                {
                // ── [1] Browse & Enroll ──────────────────────
                case "1":
                    BrowseAndEnroll();
                    break;

                // ── [2] My Enrolled Courses ──────────────────
                case "2":
                    Console.WriteLine($"  === {currentUser.Username}'s Enrolled Courses ===\n");
                    var myEnrollments = efEnrollments.GetStudentEnrollments(currentUser.UserId);
                    if (!myEnrollments.Any())
                        Console.WriteLine("  No courses enrolled yet.");
                    else
                        {
                        Console.WriteLine($"  {"Course",-36}{"Progress",-12}{"Status",-14}Enrolled");
                        Console.WriteLine("  " + new string('─', 74));
                        foreach (var e in myEnrollments)
                            Console.WriteLine($"  {e.Course?.Title ?? "N/A",-36}{e.ProgressPercent + "%",-12}{e.Status,-14}{e.EnrolledDate:d}");
                        }
                    Console.ReadKey();
                    break;

                // ── [3] Update Progress ──────────────────────
                case "3":
                    UpdateMyProgress();
                    break;

                // ── [4] My Profile & Report ──────────────────
                case "4":
                    DisplayStudentReport(currentUser);
                    Console.ReadKey();
                    break;

                // ── [5] Drop a Course ────────────────────────
                case "5":
                    DropCourse();
                    break;

                // ── [6] Search Courses ───────────────────────
                case "6":
                    SearchCourses();
                    Console.ReadKey();
                    break;

                // ── [7] My Active Courses ────────────────────
                case "7":
                    Console.WriteLine($"  === {currentUser.Username}'s Active Courses ===\n");
                    var active = efStudents.GetActiveEnrollments(currentUser.UserId);
                    efEnrollments.DisplayEnrollmentList(active);
                    Console.ReadKey();
                    break;

                // ── [8] Students Above Progress % ────────────
                case "8":
                    Console.Write("  Minimum progress %: ");
                    if (int.TryParse(Console.ReadLine(), out int minP))
                        {
                        var res = efStudents.GetStudentsAboveProgress(minP);
                        Console.WriteLine($"\n  Students with progress > {minP}%: {res.Count}");
                        foreach (var s in res) efStudents.DisplayStudentEntity(s);
                        }
                    Console.ReadKey();
                    break;

                // ── [9] Filter by Progress + Category ────────
                case "9":
                    Console.Write("  Min progress % : "); int.TryParse(Console.ReadLine(), out int mp);
                    Console.Write("  Category       : "); string mpCat = Console.ReadLine();
                    var filtered = efStudents.GetStudentsByProgressAndCategory(mp, mpCat);
                    Console.WriteLine($"\n  Results ({filtered.Count}):");
                    foreach (var s in filtered) efStudents.DisplayStudentEntity(s);
                    Console.ReadKey();
                    break;

                // ── [10] ADO.NET — My Enrollments ────────────
                case "10":
                    Console.WriteLine($"  ── My Enrollments via ADO.NET (ID: {currentUser.UserId}) ──\n");
                    adoService.GetEnrollmentsByStudent(currentUser.UserId);
                    Console.ReadKey();
                    break;

                // ── [11] ADO.NET — All Students ──────────────
                case "11":
                    Console.WriteLine("  ── All Students (ADO.NET) ──");
                    adoService.DisplayStudents(adoService.GetAllStudentsAdo());
                    Console.ReadKey();
                    break;

                // ── [12] ADO.NET — All Courses ───────────────
                case "12":
                    Console.WriteLine("  ── All Courses (ADO.NET) ──");
                    adoService.DisplayCourses(adoService.GetAllCoursesAdo());
                    Console.ReadKey();
                    break;

                // ── [0] Logout ────────────────────────────────
                case "0":
                    currentUser = null;
                    Console.WriteLine("  ✓ Logged out.");
                    Console.ReadKey();
                    return false;

                default:
                    Console.WriteLine("  Invalid choice.");
                    Console.ReadKey();
                    break;
                }
            return true;
            }

        static void BrowseAndEnroll()
            {
            // Show all courses from DB
            var courses = efCourses.GetAllCourses();
            if (!courses.Any()) { Console.WriteLine("  No courses available."); Console.ReadKey(); return; }

            DisplayCourseTable(courses);

            Console.Write("\n  Enter Course ID to enroll (0 = cancel): ");
            if (!int.TryParse(Console.ReadLine(), out int courseId) || courseId == 0) return;

            efEnrollments.EnrollStudent(currentUser.UserId, courseId);
            Console.ReadKey();
            }

        static void UpdateMyProgress()
            {
            var myEnr = efEnrollments.GetStudentEnrollments(currentUser.UserId);
            if (!myEnr.Any()) { Console.WriteLine("  No courses enrolled yet."); Console.ReadKey(); return; }

            Console.WriteLine($"  === Update Progress — {currentUser.Username} ===\n");
            foreach (var e in myEnr)
                Console.WriteLine($"  [{e.CourseId}] {e.Course?.Title ?? "N/A",-36} {e.ProgressPercent}%  ({e.Status})");

            Console.Write("\n  Enter Course ID  : ");
            if (!int.TryParse(Console.ReadLine(), out int cId)) { Console.WriteLine("  ✗ Invalid."); Console.ReadKey(); return; }
            Console.Write("  New Progress (0-100): ");
            if (!int.TryParse(Console.ReadLine(), out int newProg)) { Console.WriteLine("  ✗ Invalid."); Console.ReadKey(); return; }

            bool ok = efEnrollments.UpdateEnrollmentProgress(currentUser.UserId, cId, newProg);
            if (ok && newProg >= 75)
                Console.WriteLine(newProg == 100 ? "  🎉 Congratulations! Course completed!" : $"  Great progress! You're {newProg}% through!");
            Console.ReadKey();
            }

        static void DropCourse()
            {
            var myEnr = efEnrollments.GetStudentEnrollments(currentUser.UserId)
                            .Where(e => e.Status == "Active").ToList();
            if (!myEnr.Any()) { Console.WriteLine("  No active courses to drop."); Console.ReadKey(); return; }

            Console.WriteLine("  === Your Active Courses ===\n");
            foreach (var e in myEnr)
                Console.WriteLine($"  [{e.CourseId}] {e.Course?.Title ?? "N/A"} — {e.ProgressPercent}%");

            Console.Write("\n  Enter Course ID to drop (0 = cancel): ");
            if (!int.TryParse(Console.ReadLine(), out int dropId) || dropId == 0) return;

            efEnrollments.DropCourse(currentUser.UserId, dropId);
            Console.ReadKey();
            }

        // ════════════════════════════════════════════════════════
        //  INSTRUCTOR HANDLERS
        // ════════════════════════════════════════════════════════
        static bool HandleInstructorChoice(string choice)
            {
            Console.Clear();
            switch (choice)
                {
                // ── [1] My Courses ───────────────────────────
                case "1":
                    Console.WriteLine($"  === {currentUser.Username}'s Courses ===\n");
                    var myCourses = efCourses.GetAllCourses()
                        .Where(c => c.InstructorName == currentUser.Username ||
                                    c.InstructorId == currentUser.UserId).ToList();
                    if (!myCourses.Any()) Console.WriteLine("  No courses assigned yet.");
                    else DisplayCourseTable(myCourses);
                    Console.ReadKey();
                    break;

                // ── [2] Create New Course ────────────────────
                case "2":
                    Console.WriteLine("  === Create New Course ===\n");
                    Console.Write("  Title       : "); string ct = Console.ReadLine();
                    Console.Write("  Description : "); string cd = Console.ReadLine();
                    Console.Write("  Category    : "); string cc = Console.ReadLine();
                    Console.Write("  Difficulty (Beginner/Intermediate/Advanced): "); string cdif = Console.ReadLine();
                    Console.Write("  Course Type (Online/InPerson/Hybrid): "); string ctype = Console.ReadLine();
                    Console.Write("  Max Capacity: "); int.TryParse(Console.ReadLine(), out int cap);
                    efCourses.CreateCourse(ct, cd, cc, cdif, cap, currentUser.Username, currentUser.UserId);
                    Console.ReadKey();
                    break;

                // ── [3] Student Roster ───────────────────────
                case "3":
                    Console.WriteLine("  === All Registered Students ===\n");
                    var allStudents = efStudents.GetAllStudents();
                    efAdmin.DisplayUserList(allStudents);
                    Console.ReadKey();
                    break;

                // ── [4] Enrollments for My Course ────────────
                case "4":
                    Console.Write("  Enter Course ID: ");
                    if (int.TryParse(Console.ReadLine(), out int enrCourseId))
                        {
                        var ces = efEnrollments.GetCourseEnrollments(enrCourseId);
                        Console.WriteLine($"\n  Enrollments for Course {enrCourseId}:");
                        efEnrollments.DisplayEnrollmentList(ces);
                        }
                    Console.ReadKey();
                    break;

                // ── [5] Search Courses ───────────────────────
                case "5":
                    SearchCourses();
                    Console.ReadKey();
                    break;

                // ── [6] Top Courses by Enrollment ────────────
                case "6":
                    Console.WriteLine("  === Top 5 Courses by Enrollment ===\n");
                    DisplayCourseTable(efCourses.GetTopCoursesByEnrollment(5));
                    Console.ReadKey();
                    break;

                // ── [7] Enrollments by Category ──────────────
                case "7":
                    Console.WriteLine("  === Enrollments by Category (GroupBy) ===\n");
                    var catGrp = efCourses.GetEnrollmentsByCategory();
                    Console.WriteLine($"  {"Category",-26}{"Total Enrollments"}");
                    Console.WriteLine("  " + new string('─', 44));
                    foreach (var kv in catGrp)
                        Console.WriteLine($"  {kv.Key,-26}{kv.Value}");
                    Console.ReadKey();
                    break;

                // ── [8] Change Enrollment Status ─────────────
                case "8":
                    Console.Write("  Enrollment ID : "); int.TryParse(Console.ReadLine(), out int enrId);
                    Console.Write("  New Status (Active/Completed/Dropped): "); string newSt = Console.ReadLine();
                    efEnrollments.ChangeEnrollmentStatus(enrId, newSt);
                    Console.ReadKey();
                    break;

                // ── [9] My Profile ───────────────────────────
                case "9":
                    var instrProfile = efInstructors.GetInstructorWithCourses(currentUser.UserId);
                    efInstructors.DisplayInstructorWithCourses(instrProfile);
                    Console.ReadKey();
                    break;

                // ── [10] My Courses + Students (deep) ────────
                case "10":
                    var deep = efInstructors.GetInstructorWithCoursesAndStudents(currentUser.UserId);
                    efInstructors.DisplayInstructorWithCoursesAndStudents(deep);
                    Console.ReadKey();
                    break;

                // ── [11] ADO.NET — Direct Insert ─────────────
                case "11":
                    Console.WriteLine("  === ADO.NET: Insert Course Directly ===\n");
                    Console.Write("  Title       : "); string at = Console.ReadLine();
                    Console.Write("  Category    : "); string ac = Console.ReadLine();
                    Console.Write("  Difficulty  : "); string ad = Console.ReadLine();
                    Console.Write("  Max Capacity: "); int.TryParse(Console.ReadLine(), out int am);
                    adoService.InsertCourseAdo(at, ac, ad, am, currentUser.Username);
                    Console.ReadKey();
                    break;

                // ── [12] ADO.NET — All Students ──────────────
                case "12":
                    Console.WriteLine("  ── All Students (ADO.NET) ──");
                    adoService.DisplayStudents(adoService.GetAllStudentsAdo());
                    Console.ReadKey();
                    break;

                // ── [0] Logout ────────────────────────────────
                case "0":
                    currentUser = null;
                    Console.WriteLine("  ✓ Logged out.");
                    Console.ReadKey();
                    return false;

                default:
                    Console.WriteLine("  Invalid choice.");
                    Console.ReadKey();
                    break;
                }
            return true;
            }

        // ════════════════════════════════════════════════════════
        //  ADMIN HANDLERS
        // ════════════════════════════════════════════════════════
        static bool HandleAdminChoice(string choice)
            {
            Console.Clear();
            switch (choice)
                {
                case "1": AdminManageUsers(); break;
                case "2": AdminManageCourses(); break;
                case "3": AdminManageEnrollments(); break;
                case "4":
                    efAdmin.DisplaySystemAnalytics();
                    Console.ReadKey();
                    break;
                case "5":
                    efAdmin.DisplayAggregationReport();
                    Console.ReadKey();
                    break;
                case "6": AdminAdoNetMenu(); break;
                case "7": AdminAdvancedQueries(); break;
                case "0":
                    currentUser = null;
                    Console.WriteLine("  ✓ Logged out.");
                    Console.ReadKey();
                    return false;
                default:
                    Console.WriteLine("  Invalid choice.");
                    Console.ReadKey();
                    break;
                }
            return true;
            }

        // ── Admin: Manage Users sub-menu ──
        static void AdminManageUsers()
            {
            bool back = false;
            while (!back)
                {
                Console.Clear();
                Console.WriteLine("╔════════════════════════════════════════╗");
                Console.WriteLine("║          ADMIN — MANAGE USERS          ║");
                Console.WriteLine("╚════════════════════════════════════════╝");
                Console.WriteLine("  [1]  View All Users");
                Console.WriteLine("  [2]  View by Type (Student/Instructor/Admin)");
                Console.WriteLine("  [3]  Find User by ID");
                Console.WriteLine("  [4]  Find User by Username");
                Console.WriteLine("  [5]  Create New User");
                Console.WriteLine("  [6]  Toggle Active Status");
                Console.WriteLine("  [7]  Reset Password");
                Console.WriteLine("  [8]  Delete User");
                Console.WriteLine("  [9]  User Full Details (with enrollments)");
                Console.WriteLine("  [10] All Instructors");
                Console.WriteLine("  [11] Instructor with Courses");
                Console.WriteLine("  [12] Instructor with Courses + Students");
                Console.WriteLine("  [0]  Back");
                Console.WriteLine("════════════════════════════════════════");
                Console.Write("  Choice: ");
                string ch = Console.ReadLine();
                Console.Clear();

                switch (ch)
                    {
                    case "1":
                        Console.WriteLine("  ── All Users ──");
                        efAdmin.DisplayUserList(efAdmin.GetAllUsers());
                        break;

                    case "2":
                        Console.Write("  Type (Student/Instructor/Admin): ");
                        efAdmin.DisplayUserList(efAdmin.GetUsersByType(Console.ReadLine()));
                        break;

                    case "3":
                        Console.Write("  User ID: ");
                        if (int.TryParse(Console.ReadLine(), out int fuId))
                            {
                            var u = efAdmin.FindUserById(fuId);
                            if (u == null) Console.WriteLine("  ✗ Not found.");
                            else efAdmin.DisplayUserList(new List<UserEntity> { u });
                            }
                        break;

                    case "4":
                        Console.Write("  Username: ");
                        var found = efAdmin.FindUserByUsername(Console.ReadLine());
                        if (found == null) Console.WriteLine("  ✗ Not found.");
                        else efAdmin.DisplayUserList(new List<UserEntity> { found });
                        break;

                    case "5":
                        Console.Write("  Username : "); string cuu = Console.ReadLine();
                        Console.Write("  Email    : "); string cue = Console.ReadLine();
                        Console.Write("  Password : "); string cup = Console.ReadLine();
                        Console.Write("  Type (Student/Instructor/Admin): "); string cut = Console.ReadLine();
                        efAdmin.CreateUser(cuu, cue, cup, cut);
                        break;

                    case "6":
                        Console.Write("  User ID            : "); int.TryParse(Console.ReadLine(), out int togId);
                        Console.Write("  Active? (true/false): "); bool.TryParse(Console.ReadLine(), out bool togActive);
                        efAdmin.SetUserActiveStatus(togId, togActive);
                        break;

                    case "7":
                        Console.Write("  User ID      : "); int.TryParse(Console.ReadLine(), out int rpId);
                        Console.Write("  New Password : "); string rpPwd = Console.ReadLine();
                        efAdmin.ResetPassword(rpId, rpPwd);
                        break;

                    case "8":
                        Console.Write("  User ID to delete: ");
                        if (int.TryParse(Console.ReadLine(), out int delId))
                            efAdmin.DeleteUser(delId);
                        break;

                    case "9":
                        Console.Write("  User ID: ");
                        if (int.TryParse(Console.ReadLine(), out int fdId))
                            efAdmin.DisplayUserFullDetails(efAdmin.GetUserWithFullDetails(fdId));
                        break;

                    case "10":
                        Console.WriteLine("  ── All Instructors ──");
                        efInstructors.DisplayInstructorList(efInstructors.GetAllInstructors());
                        break;

                    case "11":
                        Console.Write("  Instructor ID: ");
                        if (int.TryParse(Console.ReadLine(), out int iwcId))
                            efInstructors.DisplayInstructorWithCourses(
                                efInstructors.GetInstructorWithCourses(iwcId));
                        break;

                    case "12":
                        Console.Write("  Instructor ID: ");
                        if (int.TryParse(Console.ReadLine(), out int iwcsId))
                            efInstructors.DisplayInstructorWithCoursesAndStudents(
                                efInstructors.GetInstructorWithCoursesAndStudents(iwcsId));
                        break;

                    case "0": back = true; break;
                    default: Console.WriteLine("  Invalid choice."); break;
                    }
                if (!back) Console.ReadKey();
                }
            }

        // ── Admin: Manage Courses sub-menu ──
        static void AdminManageCourses()
            {
            bool back = false;
            while (!back)
                {
                Console.Clear();
                Console.WriteLine("╔════════════════════════════════════════╗");
                Console.WriteLine("║         ADMIN — MANAGE COURSES         ║");
                Console.WriteLine("╚════════════════════════════════════════╝");
                Console.WriteLine("  [1]  View All Courses");
                Console.WriteLine("  [2]  Search / Filter Courses");
                Console.WriteLine("  [3]  View Courses by Category");
                Console.WriteLine("  [4]  Create New Course");
                Console.WriteLine("  [5]  Update Enrollment Count");
                Console.WriteLine("  [6]  Reassign Course to Instructor");
                Console.WriteLine("  [7]  Delete Course");
                Console.WriteLine("  [8]  Course with All Students (Include)");
                Console.WriteLine("  [9]  Course with Instructor Info");
                Console.WriteLine("  [10] Top Courses by Enrollment");
                Console.WriteLine("  [11] Intermediate Courses with Availability");
                Console.WriteLine("  [0]  Back");
                Console.WriteLine("════════════════════════════════════════");
                Console.Write("  Choice: ");
                string ch = Console.ReadLine();
                Console.Clear();

                switch (ch)
                    {
                    case "1":
                        DisplayCourseTable(efCourses.GetAllCourses());
                        break;

                    case "2":
                        Console.Write("  Keyword  (Enter=any): "); string skw = Console.ReadLine();
                        Console.Write("  Category (Enter=any): "); string scat = Console.ReadLine();
                        DisplayCourseTable(efCourses.SearchCoursesByKeywordAndCategory(skw, scat));
                        break;

                    case "3":
                        Console.Write("  Category: ");
                        DisplayCourseTable(efCourses.GetCoursesByCategory(Console.ReadLine()));
                        break;

                    case "4":
                        Console.Write("  Title       : "); string nt = Console.ReadLine();
                        Console.Write("  Description : "); string nd = Console.ReadLine();
                        Console.Write("  Category    : "); string nc = Console.ReadLine();
                        Console.Write("  Difficulty (Beginner/Intermediate/Advanced): "); string ndif = Console.ReadLine();
                        Console.Write("  Max Capacity: "); int.TryParse(Console.ReadLine(), out int ncap);
                        Console.Write("  Instructor Name: "); string nin = Console.ReadLine();
                        efCourses.CreateCourse(nt, nd, nc, ndif, ncap, nin);
                        break;

                    case "5":
                        Console.Write("  Course ID : "); int.TryParse(Console.ReadLine(), out int ucId);
                        Console.Write("  New Count : "); int.TryParse(Console.ReadLine(), out int ucCnt);
                        efCourses.UpdateEnrollmentCount(ucId, ucCnt);
                        break;

                    case "6":
                        Console.Write("  Course ID         : "); int.TryParse(Console.ReadLine(), out int rcCId);
                        Console.Write("  New Instructor ID : "); int.TryParse(Console.ReadLine(), out int rcIId);
                        efAdmin.ReassignCourse(rcCId, rcIId);
                        break;

                    case "7":
                        Console.Write("  Course ID to delete: ");
                        if (int.TryParse(Console.ReadLine(), out int delCId))
                            efAdmin.DeleteCourse(delCId);
                        break;

                    case "8":
                        Console.Write("  Course ID: ");
                        if (int.TryParse(Console.ReadLine(), out int cwsId))
                            {
                            var cws = efCourses.GetCourseWithStudents(cwsId);
                            if (cws == null) { Console.WriteLine("  ✗ Not found."); break; }
                            Console.WriteLine($"\n  [{cws.CourseId}] {cws.Title} | {cws.Category}");
                            Console.WriteLine($"  Enrolled ({cws.Enrollments.Count}):");
                            foreach (var e in cws.Enrollments)
                                Console.WriteLine($"    → {e.Student?.Username ?? "N/A",-24} {e.ProgressPercent,3}%  {e.Status}");
                            }
                        break;

                    case "9":
                        Console.Write("  Course ID: ");
                        if (int.TryParse(Console.ReadLine(), out int cwiId))
                            {
                            var cwi = efCourses.GetCourseWithInstructor(cwiId);
                            if (cwi == null) { Console.WriteLine("  ✗ Not found."); break; }
                            Console.WriteLine($"\n  [{cwi.CourseId}] {cwi.Title}");
                            Console.WriteLine($"  Category    : {cwi.Category}");
                            Console.WriteLine($"  Difficulty  : {cwi.DifficultyLevel}");
                            Console.WriteLine($"  Enrolled    : {cwi.CurrentEnrollments}/{cwi.MaxCapacity}");
                            Console.WriteLine($"  Instructor  : {cwi.Instructor?.Username ?? cwi.InstructorName ?? "N/A"}");
                            }
                        break;

                    case "10":
                        Console.Write("  Top N: "); int.TryParse(Console.ReadLine(), out int topN);
                        if (topN < 1) topN = 5;
                        DisplayCourseTable(efCourses.GetTopCoursesByEnrollment(topN));
                        break;

                    case "11":
                        DisplayCourseTable(efCourses.GetAvailableIntermediateCourses());
                        break;

                    case "0": back = true; break;
                    default: Console.WriteLine("  Invalid choice."); break;
                    }
                if (!back) Console.ReadKey();
                }
            }

        // ── Admin: Manage Enrollments sub-menu ──
        static void AdminManageEnrollments()
            {
            bool back = false;
            while (!back)
                {
                Console.Clear();
                Console.WriteLine("╔════════════════════════════════════════╗");
                Console.WriteLine("║       ADMIN — MANAGE ENROLLMENTS       ║");
                Console.WriteLine("╚════════════════════════════════════════╝");
                Console.WriteLine("  [1]  All Enrollments");
                Console.WriteLine("  [2]  Enroll Student in Course");
                Console.WriteLine("  [3]  Drop Student from Course");
                Console.WriteLine("  [4]  Update Enrollment Progress");
                Console.WriteLine("  [5]  Change Enrollment Status");
                Console.WriteLine("  [6]  Enrollments by Status");
                Console.WriteLine("  [7]  Enrollments for a Course");
                Console.WriteLine("  [8]  Enrollments for a Student");
                Console.WriteLine("  [9]  At-Risk Enrollments (low progress)");
                Console.WriteLine("  [10] Incomplete Active Enrollments");
                Console.WriteLine("  [0]  Back");
                Console.WriteLine("════════════════════════════════════════");
                Console.Write("  Choice: ");
                string ch = Console.ReadLine();
                Console.Clear();

                switch (ch)
                    {
                    case "1":
                        efEnrollments.DisplayEnrollmentList(efAdmin.GetAllEnrollments());
                        break;

                    case "2":
                        Console.Write("  Student ID: "); int.TryParse(Console.ReadLine(), out int esId);
                        Console.Write("  Course ID : "); int.TryParse(Console.ReadLine(), out int ecId);
                        efEnrollments.EnrollStudent(esId, ecId);
                        break;

                    case "3":
                        Console.Write("  Student ID: "); int.TryParse(Console.ReadLine(), out int dsId);
                        Console.Write("  Course ID : "); int.TryParse(Console.ReadLine(), out int dcId);
                        efEnrollments.DropCourse(dsId, dcId);
                        break;

                    case "4":
                        Console.Write("  Student ID   : "); int.TryParse(Console.ReadLine(), out int ups);
                        Console.Write("  Course ID    : "); int.TryParse(Console.ReadLine(), out int upc);
                        Console.Write("  New Progress : "); int.TryParse(Console.ReadLine(), out int upp);
                        efEnrollments.UpdateEnrollmentProgress(ups, upc, upp);
                        break;

                    case "5":
                        Console.Write("  Enrollment ID: "); int.TryParse(Console.ReadLine(), out int csId);
                        Console.Write("  New Status (Active/Completed/Dropped): "); string csSt = Console.ReadLine();
                        efEnrollments.ChangeEnrollmentStatus(csId, csSt);
                        break;

                    case "6":
                        Console.Write("  Status (Active/Completed/Dropped): ");
                        efEnrollments.DisplayEnrollmentList(efAdmin.GetEnrollmentsByStatus(Console.ReadLine()));
                        break;

                    case "7":
                        Console.Write("  Course ID: ");
                        if (int.TryParse(Console.ReadLine(), out int ceId))
                            efEnrollments.DisplayEnrollmentList(efEnrollments.GetCourseEnrollments(ceId));
                        break;

                    case "8":
                        Console.Write("  Student ID: ");
                        if (int.TryParse(Console.ReadLine(), out int seId))
                            efEnrollments.DisplayEnrollmentList(efEnrollments.GetStudentEnrollments(seId));
                        break;

                    case "9":
                        Console.Write("  Progress threshold (default 30): ");
                        int.TryParse(Console.ReadLine(), out int thr);
                        if (thr == 0) thr = 30;
                        efEnrollments.DisplayEnrollmentList(efAdmin.GetAtRiskEnrollments(thr));
                        break;

                    case "10":
                        efEnrollments.DisplayEnrollmentList(efEnrollments.GetIncompleteActiveEnrollments());
                        break;

                    case "0": back = true; break;
                    default: Console.WriteLine("  Invalid choice."); break;
                    }
                if (!back) Console.ReadKey();
                }
            }

        // ── Admin: ADO.NET Operations sub-menu ──
        static void AdminAdoNetMenu()
            {
            bool back = false;
            while (!back)
                {
                Console.Clear();
                Console.WriteLine("╔════════════════════════════════════════╗");
                Console.WriteLine("║        ADMIN — ADO.NET OPERATIONS      ║");
                Console.WriteLine("╚════════════════════════════════════════╝");
                Console.WriteLine("  [1]  Test DB Connection");
                Console.WriteLine("  [2]  Read All Students (SqlDataReader)");
                Console.WriteLine("  [3]  Read All Courses  (SqlDataReader)");
                Console.WriteLine("  [4]  Find User by ID   (parameterised)");
                Console.WriteLine("  [5]  Get Enrollments for Student");
                Console.WriteLine("  [6]  Insert Student    (AddWithValue)");
                Console.WriteLine("  [7]  Insert Course     (AddWithValue)");
                Console.WriteLine("  [8]  Insert Enrollment (AddWithValue)");
                Console.WriteLine("  [9]  Update Progress   (ExecuteNonQuery)");
                Console.WriteLine("  [10] Update Enrollment Count");
                Console.WriteLine("  [11] Delete Enrollment (ExecuteNonQuery)");
                Console.WriteLine("  [0]  Back");
                Console.WriteLine("════════════════════════════════════════");
                Console.Write("  Choice: ");
                string ch = Console.ReadLine();
                Console.Clear();

                switch (ch)
                    {
                    case "1":
                        adoService.TestConnection();
                        break;
                    case "2":
                        adoService.DisplayStudents(adoService.GetAllStudentsAdo());
                        break;
                    case "3":
                        adoService.DisplayCourses(adoService.GetAllCoursesAdo());
                        break;
                    case "4":
                        Console.Write("  User ID: ");
                        if (int.TryParse(Console.ReadLine(), out int fuId))
                            {
                            var u = adoService.FindUserById(fuId);
                            if (u == null) Console.WriteLine("  ✗ Not found.");
                            else Console.WriteLine($"  [{u.UserType}] {u.Username} | {u.Email} | Active: {u.IsActive}");
                            }
                        break;
                    case "5":
                        Console.Write("  Student ID: ");
                        if (int.TryParse(Console.ReadLine(), out int gsId))
                            adoService.GetEnrollmentsByStudent(gsId);
                        break;
                    case "6":
                        Console.Write("  Username : "); string isu = Console.ReadLine();
                        Console.Write("  Email    : "); string ise = Console.ReadLine();
                        Console.Write("  Password : "); string isp = Console.ReadLine();
                        adoService.InsertStudentAdo(isu, ise, isp);
                        break;
                    case "7":
                        Console.Write("  Title       : "); string ict = Console.ReadLine();
                        Console.Write("  Category    : "); string icc = Console.ReadLine();
                        Console.Write("  Difficulty  : "); string icd = Console.ReadLine();
                        Console.Write("  Max Capacity: "); int.TryParse(Console.ReadLine(), out int icap);
                        Console.Write("  Instructor  : "); string icin = Console.ReadLine();
                        adoService.InsertCourseAdo(ict, icc, icd, icap, icin);
                        break;
                    case "8":
                        Console.Write("  Student ID: "); int.TryParse(Console.ReadLine(), out int ies);
                        Console.Write("  Course ID : "); int.TryParse(Console.ReadLine(), out int iec);
                        adoService.InsertEnrollmentAdo(ies, iec);
                        break;
                    case "9":
                        Console.Write("  Student ID  : "); int.TryParse(Console.ReadLine(), out int aps);
                        Console.Write("  Course ID   : "); int.TryParse(Console.ReadLine(), out int apc);
                        Console.Write("  New Progress: "); int.TryParse(Console.ReadLine(), out int app);
                        adoService.UpdateProgressAdo(aps, apc, app);
                        break;
                    case "10":
                        Console.Write("  Course ID : "); int.TryParse(Console.ReadLine(), out int ucId);
                        Console.Write("  New Count : "); int.TryParse(Console.ReadLine(), out int ucCnt);
                        adoService.UpdateCourseEnrollmentCountAdo(ucId, ucCnt);
                        break;
                    case "11":
                        Console.Write("  Enrollment ID: ");
                        if (int.TryParse(Console.ReadLine(), out int deId))
                            adoService.DeleteEnrollmentAdo(deId);
                        break;
                    case "0": back = true; break;
                    default: Console.WriteLine("  Invalid choice."); break;
                    }
                if (!back) Console.ReadKey();
                }
            }

        // ── Admin: Advanced Queries sub-menu ──
        static void AdminAdvancedQueries()
            {
            bool back = false;
            while (!back)
                {
                Console.Clear();
                Console.WriteLine("╔════════════════════════════════════════╗");
                Console.WriteLine("║       ADMIN — ADVANCED QUERIES         ║");
                Console.WriteLine("╚════════════════════════════════════════╝");
                Console.WriteLine("  [1]  Students above progress %");
                Console.WriteLine("  [2]  Students enrolled last N days");
                Console.WriteLine("  [3]  Recently registered users");
                Console.WriteLine("  [4]  Top N students by progress");
                Console.WriteLine("  [5]  Students by progress + category");
                Console.WriteLine("  [6]  Courses with 10+ enrollments");
                Console.WriteLine("  [7]  Instructors by category");
                Console.WriteLine("  [8]  Busy instructors (N+ courses)");
                Console.WriteLine("  [0]  Back");
                Console.WriteLine("════════════════════════════════════════");
                Console.Write("  Choice: ");
                string ch = Console.ReadLine();
                Console.Clear();

                switch (ch)
                    {
                    case "1":
                        Console.Write("  Min progress %: ");
                        if (int.TryParse(Console.ReadLine(), out int minP))
                            {
                            var r = efStudents.GetStudentsAboveProgress(minP);
                            Console.WriteLine($"  Found {r.Count} student(s):");
                            foreach (var s in r) efStudents.DisplayStudentEntity(s);
                            }
                        break;
                    case "2":
                        Console.Write("  Last N days: ");
                        if (int.TryParse(Console.ReadLine(), out int nd))
                            {
                            var r = efStudents.GetRecentlyEnrolledStudents(nd);
                            Console.WriteLine($"  Students enrolled in last {nd} days: {r.Count}");
                            foreach (var s in r) efStudents.DisplayStudentEntity(s);
                            }
                        break;
                    case "3":
                        Console.Write("  Last N days: ");
                        if (int.TryParse(Console.ReadLine(), out int rd))
                            efAdmin.DisplayUserList(efAdmin.GetRecentlyRegisteredUsers(rd));
                        break;
                    case "4":
                        Console.Write("  Top N: "); int.TryParse(Console.ReadLine(), out int tn);
                        if (tn < 1) tn = 5;
                        var topS = efAdmin.GetTopStudentsByProgress(tn);
                        foreach (var s in topS)
                            {
                            double avg = s.Enrollments.Any() ? s.Enrollments.Average(e => e.ProgressPercent) : 0;
                            Console.WriteLine($"  {s.Username,-24} Avg: {avg:F1}%");
                            }
                        break;
                    case "5":
                        Console.Write("  Min progress %: "); int.TryParse(Console.ReadLine(), out int mp);
                        Console.Write("  Category      : "); string mcat = Console.ReadLine();
                        var mf = efStudents.GetStudentsByProgressAndCategory(mp, mcat);
                        Console.WriteLine($"  Results ({mf.Count}):");
                        foreach (var s in mf) efStudents.DisplayStudentEntity(s);
                        break;
                    case "6":
                        DisplayCourseTable(efCourses.GetCoursesWithMinEnrollments(10));
                        break;
                    case "7":
                        Console.Write("  Category: ");
                        efInstructors.DisplayInstructorList(efInstructors.GetInstructorsByCategory(Console.ReadLine()));
                        break;
                    case "8":
                        Console.Write("  Min courses: "); int.TryParse(Console.ReadLine(), out int mc);
                        var busy = efInstructors.GetBusyInstructors(mc);
                        foreach (var bi in busy)
                            Console.WriteLine($"  {bi.Username,-24} — {bi.TaughtCourses.Count} courses");
                        break;
                    case "0": back = true; break;
                    default: Console.WriteLine("  Invalid choice."); break;
                    }
                if (!back) Console.ReadKey();
                }
            }

        // ════════════════════════════════════════════════════════
        //  SHARED UTILITIES
        // ════════════════════════════════════════════════════════

        /// <summary>Browse all courses from DB (used from main menu too).</summary>
        static void BrowseAllCourses()
            {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════╗");
            Console.WriteLine("║           AVAILABLE COURSES            ║");
            Console.WriteLine("╚════════════════════════════════════════╝\n");
            DisplayCourseTable(efCourses.GetAllCourses());
            Console.ReadKey();
            }

        /// <summary>Search courses by keyword and/or category.</summary>
        static void SearchCourses()
            {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════╗");
            Console.WriteLine("║           SEARCH COURSES               ║");
            Console.WriteLine("╚════════════════════════════════════════╝");
            Console.Write("  Keyword  (Enter=any): "); string kw = Console.ReadLine();
            Console.Write("  Category (Enter=any): "); string cat = Console.ReadLine();
            var results = efCourses.SearchCoursesByKeywordAndCategory(kw, cat);
            Console.WriteLine($"\n  {results.Count} course(s) found:");
            DisplayCourseTable(results);
            }

        /// <summary>Analytics menu (available from main menu without login).</summary>
        static void ShowAnalyticsMenu()
            {
            bool back = false;
            while (!back)
                {
                Console.Clear();
                Console.WriteLine("╔════════════════════════════════════════╗");
                Console.WriteLine("║       ANALYTICS & REPORTS              ║");
                Console.WriteLine("╚════════════════════════════════════════╝");
                Console.WriteLine("  [1]  System Analytics Dashboard");
                Console.WriteLine("  [2]  Aggregation Report");
                Console.WriteLine("  [3]  Category Breakdown");
                Console.WriteLine("  [4]  Top 5 Courses by Enrollment");
                Console.WriteLine("  [5]  Enrollments by Category");
                Console.WriteLine("  [0]  Back");
                Console.WriteLine("════════════════════════════════════════");
                Console.Write("  Choice: ");
                string ch = Console.ReadLine();
                Console.Clear();

                switch (ch)
                    {
                    case "1": efAdmin.DisplaySystemAnalytics(); break;
                    case "2": efAdmin.DisplayAggregationReport(); break;
                    case "3":
                        var cats = efCourses.GetEnrollmentsByCategory();
                        Console.WriteLine($"  {"Category",-26}{"Enrollments"}");
                        Console.WriteLine("  " + new string('─', 40));
                        foreach (var kv in cats) Console.WriteLine($"  {kv.Key,-26}{kv.Value}");
                        break;
                    case "4":
                        DisplayCourseTable(efCourses.GetTopCoursesByEnrollment(5));
                        break;
                    case "5":
                        var grp = efCourses.GetEnrollmentsByCategory();
                        Console.WriteLine($"  {"Category",-26}{"Total Enrollments"}");
                        Console.WriteLine("  " + new string('─', 44));
                        foreach (var kv in grp) Console.WriteLine($"  {kv.Key,-26}{kv.Value}");
                        break;
                    case "0": back = true; break;
                    default: Console.WriteLine("  Invalid choice."); break;
                    }
                if (!back) Console.ReadKey();
                }
            }

        // ── Formatted course table ──
        static void DisplayCourseTable(List<CourseEntity> courses)
            {
            if (!courses.Any()) { Console.WriteLine("  No courses found."); return; }
            Console.WriteLine($"\n  {"ID",-6}{"Title",-34}{"Category",-20}{"Difficulty",-14}{"Type",-10}{"Enrolled",-10}Capacity");
            Console.WriteLine("  " + new string('─', 100));
            foreach (var c in courses)
                Console.WriteLine($"  {c.CourseId,-6}{c.Title,-34}{c.Category,-20}{c.DifficultyLevel,-14}{c.CourseType,-10}{c.CurrentEnrollments,-10}{c.MaxCapacity}");
            }

        // ── Student report display ──
        static void DisplayStudentReport(UserEntity u)
            {
            var withDetails = efStudents.GetStudentWithEnrollmentsAndCourses(u.UserId);
            int enrolled = withDetails?.Enrollments?.Count ?? 0;
            double avgProg = withDetails?.Enrollments?.Any() == true
                              ? withDetails.Enrollments.Average(e => e.ProgressPercent) : 0;
            int completed = withDetails?.Enrollments?.Count(e => e.Status == "Completed") ?? 0;

            Console.WriteLine("════════════════════════════════════");
            Console.WriteLine($"STUDENT REPORT: {u.Username}");
            Console.WriteLine("════════════════════════════════════");
            Console.WriteLine($"  DB ID        : {u.UserId}");
            Console.WriteLine($"  Email        : {u.Email}");
            Console.WriteLine($"  Registered   : {u.CreatedDate:d}");
            Console.WriteLine($"  Last Login   : {u.LastLoginDate?.ToString("g") ?? "N/A"}");
            Console.WriteLine($"  Active       : {(u.IsActive ? "✓" : "✗")}");
            Console.WriteLine($"  Enrolled     : {enrolled} course(s)");
            Console.WriteLine($"  Completed    : {completed}");
            Console.WriteLine($"  Avg Progress : {avgProg:F1}%");
            if (withDetails?.Enrollments?.Any() == true)
                {
                Console.WriteLine("\n  Courses:");
                foreach (var e in withDetails.Enrollments)
                    Console.WriteLine($"    [{e.CourseId}] {e.Course?.Title ?? "N/A",-34} {e.ProgressPercent,3}%  {e.Status}");
                }
            Console.WriteLine("════════════════════════════════════");
            }
        }
    }

//to install compatible for version 8.0

//Microsoft.EntityFrameworkCore.SqlServer
//Microsoft.EntityFrameworkCore.Tools
//Microsoft.EntityFrameworkCore.Design
//Microsoft.Data.SqlClient


//Migration commands


//Add-Migration InitialCreate
//Update-Database
//Add-Migration AddLastLoginDate   ← (LastLoginDate already on UserEntity)
//Update-Database