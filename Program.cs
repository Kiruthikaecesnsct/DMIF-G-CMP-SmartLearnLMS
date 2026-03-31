using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Week_1;
using Week_1.Exceptions;
using Week_1.Logging;
using Week_1.Services;
using Week_1.Tests;
using Week_1.Validators;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

// ═══════════════════════════════════════════════════════════════════════════════
//  SmartLearn LMS — Assignment 8 (Exception Handling, Validation & Logging)
//  Changes from Assignment 7:
//    • Registration: InputValidator + DuplicateUsername/Email exceptions
//    • Login: InvalidCredentialsException + failed-login audit logging
//    • All Wk8 services injected (logging, validation, custom exceptions)
//    • Student  dashboard: [17] View Audit Log (my activity)
//    • Admin    dashboard: [5] Run Test Suite, [8] View System Audit Log
//    • Main menu: [7] Run Test Suite added
//    • Migration reminder updated for Assignment 8 (AuditLogs table)
// ═══════════════════════════════════════════════════════════════════════════════

namespace SmartLearnLMS
    {
    class Program
        {
        // ── Currently logged-in user ──
        static UserEntity currentUser = null;

        // ── Logging / Audit (NEW Wk8) ──
        static readonly Logger _log = Logger.Instance;
        static readonly AuditService _audit = new();

        // ── Assignment 6 services (unchanged) ──
        static readonly AdoNetService adoService = new();
        static readonly EfStudentService efStudents = new();
        static readonly EfCourseService efCourses = new();
        static readonly EfEnrollmentService efEnrollments = new();
        static readonly EfInstructorService efInstructors = new();
        static readonly EfAdminService efAdmin = new();
        static readonly NotificationService notifService = new();
        static readonly ReportService reportService = new();
        static readonly SearchService searchService = new();

        // ── Assignment 7 / 8 services ──
        static readonly StudentService studentSvc = new();
        static readonly InstructorService instructorSvc = new();
        static readonly CourseService courseSvc = new();
        static readonly EnrollmentService enrollSvc = new();   // Wk8 version
        static readonly AnalyticsService analyticsSvc = new();

        // ════════════════════════════════════════════════════════
        //  ENTRY POINT
        // ════════════════════════════════════════════════════════
        static void Main(string[] args)
            {
            Console.Clear();
            _log.Info("Program", "SmartLearn LMS starting up — Assignment 8.");
            Console.WriteLine("  Initialising SmartLearn LMS — Assignment 8...");
            Console.WriteLine("  ─────────────────────────────────────────────");
            Console.WriteLine("  NOTE: If first run after Assignment 8, run in");
            Console.WriteLine("  Package Manager Console:");
            Console.WriteLine("    Add-Migration AddAuditLogsAndIndexes");
            Console.WriteLine("    Update-Database");
            Console.WriteLine("  ─────────────────────────────────────────────");

            bool dbOk = adoService.TestConnection();
            if (!dbOk)
                {
                Console.WriteLine("\n  ✗ Cannot reach SQL Server.");
                Console.WriteLine("  Make sure SmartLearnDB is running and connection string is correct.");
                Console.WriteLine("  Press any key to exit.");
                Console.ReadKey();
                return;
                }

            SeedCoursesIfEmpty();

            bool running = true;
            while (running)
                running = ShowMainMenu();
            }

        // ════════════════════════════════════════════════════════
        //  DB SEED
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
                    new() { Title="C# Fundamentals",          Description="Learn C# from scratch",            Category="Programming",      DifficultyLevel="Beginner",     MaxCapacity=999, InstructorName="Prof. Smith"   },
                    new() { Title="Python for Beginners",     Description="Intro to Python programming",      Category="Programming",      DifficultyLevel="Beginner",     MaxCapacity=999, InstructorName="Prof. Johnson" },
                    new() { Title="Web Development Basics",   Description="HTML, CSS and JS fundamentals",    Category="Web Development",  DifficultyLevel="Beginner",     MaxCapacity=999, InstructorName="Prof. Garcia"  },
                    new() { Title="Data Structures",          Description="Arrays, Lists, Trees and more",    Category="Computer Science", DifficultyLevel="Intermediate", MaxCapacity=999, InstructorName="Prof. Smith"   },
                    new() { Title="Machine Learning Intro",   Description="Basics of ML and AI concepts",     Category="Data Science",     DifficultyLevel="Intermediate", MaxCapacity=999, InstructorName="Prof. Lee"     },
                    new() { Title="Database Design Workshop", Description="Relational DB and SQL",            Category="Database",         DifficultyLevel="Intermediate", MaxCapacity=25,  InstructorName="Prof. Johnson" },
                    new() { Title="Network Security Lab",     Description="Practical cybersecurity skills",   Category="Security",         DifficultyLevel="Advanced",     MaxCapacity=20,  InstructorName="Prof. Brown"   },
                    new() { Title="Mobile App Development",   Description="Build iOS and Android apps",       Category="Mobile",           DifficultyLevel="Intermediate", MaxCapacity=30,  InstructorName="Prof. Garcia"  },
                    new() { Title="Full-Stack Development",   Description="Frontend + Backend full stack",    Category="Web Development",  DifficultyLevel="Advanced",     MaxCapacity=30,  InstructorName="Prof. Garcia"  },
                    new() { Title="Cloud Computing",          Description="AWS, Azure and cloud concepts",    Category="Cloud",            DifficultyLevel="Intermediate", MaxCapacity=25,  InstructorName="Prof. Lee"     },
                };
                ctx.Courses.AddRange(seed);
                ctx.SaveChanges();
                Console.WriteLine($"  ✓ {seed.Count} courses seeded.");
                _log.Info("Program", $"{seed.Count} courses seeded on startup.");
                }
            catch (Exception ex)
                {
                Console.WriteLine($"  ⚠ Seed warning: {ex.Message}");
                _log.Warning("Program", $"Seed warning: {ex.Message}");
                }
            }

        // ════════════════════════════════════════════════════════
        //  MAIN MENU
        // ════════════════════════════════════════════════════════
        static bool ShowMainMenu()
            {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════╗");
            Console.WriteLine("║    Welcome to SmartLearn LMS   ║");
            Console.WriteLine("║    (Assignment 8 — Wk8 Ed.)    ║");
            Console.WriteLine("╚════════════════════════════════╝");
            Console.WriteLine("  [1] Register");
            Console.WriteLine("  [2] Login");
            Console.WriteLine("  [3] Browse All Courses");
            Console.WriteLine("  [4] Search Courses");
            Console.WriteLine("  [5] Analytics & Reports");
            Console.WriteLine("  [6] Exit");
            Console.WriteLine("  [7] Run Test Suite");           // NEW Wk8
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
                    _log.Info("Program", "Application exiting normally.");
                    Console.WriteLine("  Goodbye!");
                    return false;
                case "7":                      // NEW Wk8
                    SmartLearnTests.RunAll();
                    Console.ReadKey();
                    break;
                default:
                    Console.WriteLine("  Invalid choice.");
                    Console.ReadKey();
                    break;
                }
            return true;
            }

        // ════════════════════════════════════════════════════════
        //  REGISTER  (Wk8: InputValidator + custom exceptions)
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

            // ── Input validation (Wk8) ──
            if (!InputValidator.ValidateUsername(username, out string uErr))
                { Console.WriteLine($"  ✗ {uErr}"); Console.ReadKey(); return; }

            if (!InputValidator.ValidateEmail(email, out string eErr))
                { Console.WriteLine($"  ✗ {eErr}"); Console.ReadKey(); return; }

            if (!InputValidator.ValidatePassword(password, out string pErr))
                { Console.WriteLine($"  ✗ {pErr}"); Console.ReadKey(); return; }

            if (!InputValidator.ValidateRole(role, out string rErr))
                { Console.WriteLine($"  ✗ {rErr}"); Console.ReadKey(); return; }

            try
                {
                // ── Duplicate checks (Wk8 custom exceptions) ──
                using var ctx = new SmartLearnDbContext();
                if (ctx.Users.Any(u => u.Username == username))
                    throw new DuplicateUsernameException(username);
                if (ctx.Users.Any(u => u.Email == email))
                    throw new DuplicateEmailException(email);

                var created = efAdmin.CreateUser(username, email, password, role);
                if (created != null)
                    {
                    Console.WriteLine($"\n  ✓ Registered as {role}! Welcome, {username}! (DB ID: {created.UserId})");
                    _log.Info("Program", $"New user registered: '{username}' [{role}].");
                    _audit.LogAction("UserRegistered", "Auth",
                        username: username, userId: created.UserId,
                        details: new() { ["role"] = role });
                    }
                }
            catch (DuplicateUsernameException ex)
                {
                _log.Warning("Program", ex.Message);
                Console.WriteLine($"  ✗ {ex.Message}");
                }
            catch (DuplicateEmailException ex)
                {
                _log.Warning("Program", ex.Message);
                Console.WriteLine($"  ✗ {ex.Message}");
                }
            catch (Exception ex)
                {
                _log.Error("Program", $"Registration error: {ex.Message}", username, ex);
                Console.WriteLine($"  ✗ Registration failed: {ex.Message}");
                }
            finally
                {
                Console.ReadKey();
                }
            }

        // ════════════════════════════════════════════════════════
        //  LOGIN  (Wk8: InvalidCredentialsException + audit)
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
                var user = ctx.Users.FirstOrDefault(u => u.Username == username
                                                      && u.PasswordHash == password);
                if (user == null)
                    throw new InvalidCredentialsException(username);

                if (!user.IsActive)
                    { Console.WriteLine("  ✗ Account is deactivated. Contact admin."); Console.ReadKey(); return; }

                user.LastLoginDate = DateTime.Now;
                ctx.SaveChanges();
                currentUser = user;

                Console.WriteLine($"\n  ✓ Welcome back, {user.Username}! [{user.UserType}]");
                _log.Info("Program", $"User '{username}' logged in successfully.", username);
                _audit.LogAction("UserLogin", "Auth", username: username, userId: user.UserId);
                Console.ReadKey();
                RunDashboard();
                }
            catch (InvalidCredentialsException ex)
                {
                _log.Warning("Program", $"Failed login attempt for '{username}'.");
                _audit.LogAction("FailedLogin", "Auth",
                    username: username,
                    details: new() { ["reason"] = "InvalidCredentials" });
                Console.WriteLine($"  ✗ {ex.Message}");
                Console.ReadKey();
                }
            catch (Exception ex)
                {
                _log.Error("Program", $"Login error: {ex.Message}", username, ex);
                Console.WriteLine($"  ✗ Login error: {ex.Message}");
                Console.ReadKey();
                }
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
                    Console.WriteLine("  ── Assignment 6 ────────────────");
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
                    Console.WriteLine("  ── Assignment 7 ────────────────");
                    Console.WriteLine("  [13] My Full Dashboard (Wk7)");
                    Console.WriteLine("  [14] My Completed Courses");
                    Console.WriteLine("  [15] My At-Risk Courses");
                    Console.WriteLine("  [16] Recommended Courses For Me");
                    Console.WriteLine("  ── Assignment 8 (NEW) ──────────");
                    Console.WriteLine("  [17] My Audit / Activity Log");
                    Console.WriteLine("  [0]  Logout");
                    break;

                case "Instructor":
                    Console.WriteLine("╔════════════════════════════════╗");
                    Console.WriteLine("║      INSTRUCTOR DASHBOARD      ║");
                    Console.WriteLine("╚════════════════════════════════╝");
                    Console.WriteLine($"  Welcome, {currentUser.Username}!");
                    Console.WriteLine($"  Last Login: {currentUser.LastLoginDate?.ToString("g") ?? "First time"}");
                    Console.WriteLine();
                    Console.WriteLine("  ── Assignment 6 ────────────────");
                    Console.WriteLine("  [1]  My Courses");
                    Console.WriteLine("  [2]  Create New Course");
                    Console.WriteLine("  [3]  View Student Roster");
                    Console.WriteLine("  [4]  Enrollments for My Course");
                    Console.WriteLine("  [5]  Search Courses");
                    Console.WriteLine("  [6]  Top Courses by Enrollment");
                    Console.WriteLine("  [7]  Enrollments by Category");
                    Console.WriteLine("  [8]  Change Enrollment Status");
                    Console.WriteLine("  [9]  My Profile (with courses)");
                    Console.WriteLine("  [10] My Courses + Students (deep)");
                    Console.WriteLine("  [11] ADO.NET — Direct Course Insert");
                    Console.WriteLine("  [12] ADO.NET — All Students");
                    Console.WriteLine("  ── Assignment 7 ────────────────");
                    Console.WriteLine("  [13] My Full Dashboard (Wk7)");
                    Console.WriteLine("  [14] Course Analytics");
                    Console.WriteLine("  [15] Top Students in a Course");
                    Console.WriteLine("  [16] Students At Risk (My Courses)");
                    Console.WriteLine("  ── Assignment 8 (NEW) ──────────");
                    Console.WriteLine("  [17] My Audit / Activity Log");
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
                    Console.WriteLine("  [4]  Assignment 7 Analytics");
                    Console.WriteLine("  [5]  Assignment 6 Analytics");
                    Console.WriteLine("  [6]  ADO.NET Operations");
                    Console.WriteLine("  [7]  Advanced Queries");
                    Console.WriteLine("  ── Assignment 8 (NEW) ──────────");
                    Console.WriteLine("  [8]  System Audit Log");
                    Console.WriteLine("  [9]  Run Test Suite");
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
                case "1": BrowseAndEnroll(); break;

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
                    Console.ReadKey(); break;

                case "3": UpdateMyProgress(); break;

                case "4":
                    DisplayStudentReport(currentUser);
                    Console.ReadKey(); break;

                case "5": DropCourse(); break;

                case "6":
                    SearchCourses();
                    Console.ReadKey(); break;

                case "7":
                    Console.WriteLine($"  === {currentUser.Username}'s Active Courses ===\n");
                    efEnrollments.DisplayEnrollmentList(efStudents.GetActiveEnrollments(currentUser.UserId));
                    Console.ReadKey(); break;

                case "8":
                    Console.Write("  Minimum progress %: ");
                    if (int.TryParse(Console.ReadLine(), out int minP))
                        {
                        var res = efStudents.GetStudentsAboveProgress(minP);
                        Console.WriteLine($"\n  Students with progress > {minP}%: {res.Count}");
                        foreach (var s in res) efStudents.DisplayStudentEntity(s);
                        }
                    Console.ReadKey(); break;

                case "9":
                    Console.Write("  Min progress % : "); int.TryParse(Console.ReadLine(), out int mp);
                    Console.Write("  Category       : "); string mpCat = Console.ReadLine();
                    var filtered = efStudents.GetStudentsByProgressAndCategory(mp, mpCat);
                    Console.WriteLine($"\n  Results ({filtered.Count}):");
                    foreach (var s in filtered) efStudents.DisplayStudentEntity(s);
                    Console.ReadKey(); break;

                case "10":
                    Console.WriteLine($"  ── My Enrollments via ADO.NET (ID: {currentUser.UserId}) ──\n");
                    adoService.GetEnrollmentsByStudent(currentUser.UserId);
                    Console.ReadKey(); break;

                case "11":
                    adoService.DisplayStudents(adoService.GetAllStudentsAdo());
                    Console.ReadKey(); break;

                case "12":
                    adoService.DisplayCourses(adoService.GetAllCoursesAdo());
                    Console.ReadKey(); break;

                // ── Assignment 7 ──
                case "13":
                    studentSvc.GetStudentDashboard(currentUser.UserId);
                    Console.ReadKey(); break;

                case "14":
                    Console.WriteLine($"  === {currentUser.Username}'s Completed Courses ===");
                    var completed = studentSvc.GetStudentCompletedCourses(currentUser.UserId);
                    studentSvc.DisplayCompletedCourses(completed);
                    Console.ReadKey(); break;

                case "15":
                    Console.WriteLine($"  === {currentUser.Username}'s At-Risk Courses ===");
                    var atRisk = studentSvc.GetStudentAtRiskCourses(currentUser.UserId);
                    studentSvc.DisplayAtRiskCourses(atRisk);
                    Console.ReadKey(); break;

                case "16":
                    Console.WriteLine("  === Courses Recommended For You ===");
                    var recs = courseSvc.GetRecommendedCourses(currentUser.UserId);
                    courseSvc.DisplayRecommendedCourses(recs);
                    Console.ReadKey(); break;

                // ── Assignment 8 (NEW) ──
                case "17":
                    Console.WriteLine($"  === {currentUser.Username}'s Audit Log ===");
                    var myLogs = _audit.GetUserActivity(currentUser.UserId, 30);
                    _audit.DisplayAuditLog(myLogs);
                    Console.ReadKey(); break;

                case "0":
                    _log.Info("Program", $"'{currentUser.Username}' logged out.", currentUser.Username);
                    _audit.LogAction("UserLogout", "Auth",
                        username: currentUser.Username, userId: currentUser.UserId);
                    currentUser = null;
                    Console.WriteLine("  ✓ Logged out.");
                    Console.ReadKey();
                    return false;

                default:
                    Console.WriteLine("  Invalid choice.");
                    Console.ReadKey(); break;
                }
            return true;
            }

        static void BrowseAndEnroll()
            {
            var courses = efCourses.GetAllCourses();
            if (!courses.Any()) { Console.WriteLine("  No courses available."); Console.ReadKey(); return; }
            DisplayCourseTable(courses);
            Console.Write("\n  Enter Course ID to enroll (0 = cancel): ");
            if (!int.TryParse(Console.ReadLine(), out int courseId) || courseId == 0) return;
            // Use Wk8 EnrollmentService (validates business rules + logs)
            enrollSvc.EnrollStudent(currentUser.UserId, courseId);
            Console.ReadKey();
            }

        static void UpdateMyProgress()
            {
            var myEnr = efEnrollments.GetStudentEnrollments(currentUser.UserId);
            if (!myEnr.Any()) { Console.WriteLine("  No courses enrolled yet."); Console.ReadKey(); return; }
            Console.WriteLine($"  === Update Progress — {currentUser.Username} ===\n");
            foreach (var e in myEnr)
                Console.WriteLine($"  [{e.CourseId}] {e.Course?.Title ?? "N/A",-36} {e.ProgressPercent}%  ({e.Status})");
            Console.Write("\n  Enter Course ID     : ");
            if (!int.TryParse(Console.ReadLine(), out int cId)) { Console.WriteLine("  ✗ Invalid."); Console.ReadKey(); return; }
            Console.Write("  New Progress (0-100): ");
            if (!int.TryParse(Console.ReadLine(), out int newProg)) { Console.WriteLine("  ✗ Invalid."); Console.ReadKey(); return; }

            // Use Wk8 StudentService (validates no decrease + logs)
            bool ok = studentSvc.UpdateCourseProgress(currentUser.UserId, cId, newProg);
            if (ok && newProg >= 75 && newProg < 100)
                Console.WriteLine($"  Great progress! You're {newProg}% through!");
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
            // Use Wk8 EnrollmentService (validates drop deadline + logs)
            enrollSvc.DropCourse(currentUser.UserId, dropId);
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
                case "1":
                    Console.WriteLine($"  === {currentUser.Username}'s Courses ===\n");
                    var myCourses = efCourses.GetAllCourses()
                        .Where(c => c.InstructorName == currentUser.Username ||
                                    c.InstructorId == currentUser.UserId).ToList();
                    if (!myCourses.Any()) Console.WriteLine("  No courses assigned yet.");
                    else DisplayCourseTable(myCourses);
                    Console.ReadKey(); break;

                case "2":
                    Console.WriteLine("  === Create New Course ===\n");
                    Console.Write("  Title       : "); string ct = Console.ReadLine();
                    if (!InputValidator.ValidateCourseTitle(ct, out string titleErr))
                        { Console.WriteLine($"  ✗ {titleErr}"); Console.ReadKey(); break; }
                    Console.Write("  Description : "); string cd = Console.ReadLine();
                    Console.Write("  Category    : "); string cc = Console.ReadLine();
                    Console.Write("  Difficulty (Beginner/Intermediate/Advanced): "); string cdif = Console.ReadLine();
                    Console.Write("  Max Capacity: "); int.TryParse(Console.ReadLine(), out int cap);
                    efCourses.CreateCourse(ct, cd, cc, cdif, cap, currentUser.Username, currentUser.UserId);
                    _audit.LogAction("CourseCreated", "Course",
                        username: currentUser.Username, userId: currentUser.UserId,
                        details: new() { ["title"] = ct, ["category"] = cc });
                    Console.ReadKey(); break;

                case "3":
                    Console.WriteLine("  === All Registered Students ===\n");
                    efAdmin.DisplayUserList(efStudents.GetAllStudents());
                    Console.ReadKey(); break;

                case "4":
                    Console.Write("  Enter Course ID: ");
                    if (int.TryParse(Console.ReadLine(), out int enrCourseId))
                        efEnrollments.DisplayEnrollmentList(efEnrollments.GetCourseEnrollments(enrCourseId));
                    Console.ReadKey(); break;

                case "5": SearchCourses(); Console.ReadKey(); break;

                case "6":
                    Console.WriteLine("  === Top 5 Courses by Enrollment ===\n");
                    DisplayCourseTable(efCourses.GetTopCoursesByEnrollment(5));
                    Console.ReadKey(); break;

                case "7":
                    Console.WriteLine("  === Enrollments by Category ===\n");
                    var catGrp = efCourses.GetEnrollmentsByCategory();
                    Console.WriteLine($"  {"Category",-26}{"Total Enrollments"}");
                    Console.WriteLine("  " + new string('─', 44));
                    foreach (var kv in catGrp)
                        Console.WriteLine($"  {kv.Key,-26}{kv.Value}");
                    Console.ReadKey(); break;

                case "8":
                    Console.Write("  Enrollment ID : "); int.TryParse(Console.ReadLine(), out int enrId);
                    Console.Write("  New Status (Active/Completed/Dropped): "); string newSt = Console.ReadLine();
                    efEnrollments.ChangeEnrollmentStatus(enrId, newSt);
                    Console.ReadKey(); break;

                case "9":
                    efInstructors.DisplayInstructorWithCourses(
                        efInstructors.GetInstructorWithCourses(currentUser.UserId));
                    Console.ReadKey(); break;

                case "10":
                    efInstructors.DisplayInstructorWithCoursesAndStudents(
                        efInstructors.GetInstructorWithCoursesAndStudents(currentUser.UserId));
                    Console.ReadKey(); break;

                case "11":
                    Console.WriteLine("  === ADO.NET: Insert Course Directly ===\n");
                    Console.Write("  Title       : "); string at = Console.ReadLine();
                    Console.Write("  Category    : "); string ac = Console.ReadLine();
                    Console.Write("  Difficulty  : "); string ad = Console.ReadLine();
                    Console.Write("  Max Capacity: "); int.TryParse(Console.ReadLine(), out int am);
                    adoService.InsertCourseAdo(at, ac, ad, am, currentUser.Username);
                    Console.ReadKey(); break;

                case "12":
                    adoService.DisplayStudents(adoService.GetAllStudentsAdo());
                    Console.ReadKey(); break;

                // ── Assignment 7 ──
                case "13":
                    instructorSvc.GetInstructorDashboard(currentUser.UserId);
                    Console.ReadKey(); break;

                case "14":
                    Console.Write("  Enter Course ID: ");
                    if (int.TryParse(Console.ReadLine(), out int cAId))
                        instructorSvc.GetCourseAnalytics(cAId);
                    Console.ReadKey(); break;

                case "15":
                    Console.Write("  Enter Course ID: ");
                    if (int.TryParse(Console.ReadLine(), out int tsId))
                        {
                        Console.Write("  Top N students : ");
                        int.TryParse(Console.ReadLine(), out int topN);
                        var top = instructorSvc.GetTopStudentsInCourse(tsId, topN > 0 ? topN : 5);
                        instructorSvc.DisplayTopStudents(top);
                        }
                    Console.ReadKey(); break;

                case "16":
                    instructorSvc.GetStudentsAtRisk(currentUser.UserId);
                    Console.ReadKey(); break;

                // ── Assignment 8 (NEW) ──
                case "17":
                    Console.WriteLine($"  === {currentUser.Username}'s Audit Log ===");
                    var iLogs = _audit.GetUserActivity(currentUser.UserId, 30);
                    _audit.DisplayAuditLog(iLogs);
                    Console.ReadKey(); break;

                case "0":
                    _log.Info("Program", $"'{currentUser.Username}' logged out.", currentUser.Username);
                    _audit.LogAction("UserLogout", "Auth",
                        username: currentUser.Username, userId: currentUser.UserId);
                    currentUser = null;
                    Console.WriteLine("  ✓ Logged out.");
                    Console.ReadKey();
                    return false;

                default:
                    Console.WriteLine("  Invalid choice.");
                    Console.ReadKey(); break;
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
                case "1": ShowAdminUserMenu(); break;
                case "2": ShowAdminCourseMenu(); break;
                case "3": ShowAdminEnrollMenu(); break;
                case "4": ShowAdminAnalytics7Menu(); break;
                case "5": efAdmin.DisplaySystemAnalytics(); Console.ReadKey(); break;
                case "6": ShowAdoMenu(); break;
                case "7": ShowAdvancedQueryMenu(); break;

                // ── Assignment 8 (NEW) ──
                case "8":
                    Console.WriteLine("  === System Audit Log (Recent 50) ===");
                    var recent = _audit.GetRecentActivity(50);
                    _audit.DisplayAuditLog(recent);
                    Console.ReadKey(); break;

                case "9":
                    SmartLearnTests.RunAll();
                    Console.ReadKey(); break;

                case "0":
                    _log.Info("Program", $"Admin '{currentUser.Username}' logged out.", currentUser.Username);
                    _audit.LogAction("UserLogout", "Auth",
                        username: currentUser.Username, userId: currentUser.UserId);
                    currentUser = null;
                    Console.WriteLine("  ✓ Logged out.");
                    Console.ReadKey();
                    return false;

                default:
                    Console.WriteLine("  Invalid choice.");
                    Console.ReadKey(); break;
                }
            return true;
            }

        // ── Admin sub-menus (unchanged from Wk7 except case labels) ───

        static void ShowAdminUserMenu()
            {
            bool back = false;
            while (!back)
                {
                Console.Clear();
                Console.WriteLine("╔════════════════════════════════════════╗");
                Console.WriteLine("║           MANAGE USERS                 ║");
                Console.WriteLine("╚════════════════════════════════════════╝");
                Console.WriteLine("  [1]  List All Users");
                Console.WriteLine("  [2]  View User Details");
                Console.WriteLine("  [3]  Deactivate User");
                Console.WriteLine("  [4]  Reactivate User");
                Console.WriteLine("  [5]  Reset Password");
                Console.WriteLine("  [0]  Back");
                Console.WriteLine("════════════════════════════════════════");
                Console.Write("  Choice: ");
                string ch = Console.ReadLine(); Console.Clear();
                switch (ch)
                    {
                    case "1": efAdmin.DisplayUserList(efAdmin.GetAllUsers()); break;
                    case "2":
                        Console.Write("  User ID: "); int.TryParse(Console.ReadLine(), out int uid);
                        var u = efAdmin.FindUserById(uid);              // ← was GetUserById
                        if (u != null) { efAdmin.DisplayUserFullDetails(u); }  // ← was DisplayUserDetails
                        else Console.WriteLine("  ✗ User not found.");
                        break;
                    case "3":
                        Console.Write("  User ID to deactivate: "); int.TryParse(Console.ReadLine(), out int did);
                        efAdmin.SetUserActiveStatus(did, false); break; // ← was DeactivateUser
                    case "4":
                        Console.Write("  User ID to reactivate: "); int.TryParse(Console.ReadLine(), out int rid);
                        efAdmin.SetUserActiveStatus(rid, true); break;  // ← was ReactivateUser
                    case "5":
                        Console.Write("  User ID   : "); int.TryParse(Console.ReadLine(), out int pid);
                        Console.Write("  New pass  : "); string np = Console.ReadLine();
                        efAdmin.ResetPassword(pid, np); break;
                    case "0": back = true; break;
                    default: Console.WriteLine("  Invalid choice."); break;
                    }
                if (!back) Console.ReadKey();
                }
            }

        static void ShowAdminCourseMenu()
            {
            bool back = false;
            while (!back)
                {
                Console.Clear();
                Console.WriteLine("╔════════════════════════════════════════╗");
                Console.WriteLine("║           MANAGE COURSES               ║");
                Console.WriteLine("╚════════════════════════════════════════╝");
                Console.WriteLine("  [1]  List All Courses");
                Console.WriteLine("  [2]  View Course Details (Full)");
                Console.WriteLine("  [3]  Delete Course");
                Console.WriteLine("  [0]  Back");
                Console.Write("  Choice: ");
                string ch = Console.ReadLine(); Console.Clear();
                switch (ch)
                    {
                    case "1": DisplayCourseTable(efCourses.GetAllCourses()); break;
                    case "2":
                        Console.Write("  Course ID: "); int.TryParse(Console.ReadLine(), out int cid);
                        courseSvc.GetCourseWithFullDetails(cid); break;
                    case "3":
                        Console.Write("  Course ID to delete: "); int.TryParse(Console.ReadLine(), out int dcid);
                        efAdmin.DeleteCourse(dcid); break;
                    case "0": back = true; break;
                    default: Console.WriteLine("  Invalid choice."); break;
                    }
                if (!back) Console.ReadKey();
                }
            }

        static void ShowAdminEnrollMenu()
            {
            bool back = false;
            while (!back)
                {
                Console.Clear();
                Console.WriteLine("╔════════════════════════════════════════╗");
                Console.WriteLine("║         MANAGE ENROLLMENTS             ║");
                Console.WriteLine("╚════════════════════════════════════════╝");
                Console.WriteLine("  [1]  All Enrollments");
                Console.WriteLine("  [2]  Enroll Student (validated, Wk8)");
                Console.WriteLine("  [3]  Mark as Completed (validated, Wk8)");
                Console.WriteLine("  [4]  Enrollment Trends");
                Console.WriteLine("  [0]  Back");
                Console.Write("  Choice: ");
                string ch = Console.ReadLine(); Console.Clear();
                switch (ch)
                    {
                    case "1": efEnrollments.DisplayEnrollmentList(efAdmin.GetAllEnrollments()); break;
                    case "2":
                        Console.Write("  Student ID : "); int.TryParse(Console.ReadLine(), out int sid);
                        Console.Write("  Course  ID : "); int.TryParse(Console.ReadLine(), out int cid);
                        enrollSvc.EnrollStudent(sid, cid); break;
                    case "3":
                        Console.Write("  Enrollment ID: "); int.TryParse(Console.ReadLine(), out int eid);
                        enrollSvc.MarkAsCompleted(eid); break;
                    case "4":
                        enrollSvc.DisplayEnrollmentTrends(enrollSvc.GetEnrollmentTrends()); break;
                    case "0": back = true; break;
                    default: Console.WriteLine("  Invalid choice."); break;
                    }
                if (!back) Console.ReadKey();
                }
            }

        static void ShowAdminAnalytics7Menu()
            {
            bool back = false;
            while (!back)
                {
                Console.Clear();
                Console.WriteLine("╔════════════════════════════════════════╗");
                Console.WriteLine("║      ASSIGNMENT 7 ANALYTICS MENU       ║");
                Console.WriteLine("╚════════════════════════════════════════╝");
                Console.WriteLine("  [1]  System Statistics");
                Console.WriteLine("  [2]  Category Popularity");
                Console.WriteLine("  [3]  Top 5 Courses by Enrollment");
                Console.WriteLine("  [4]  Enrollments by Category");
                Console.WriteLine("  [0]  Back");
                Console.Write("  Choice: ");
                string ch = Console.ReadLine(); Console.Clear();
                switch (ch)
                    {
                    case "1": analyticsSvc.DisplaySystemStats(); break;
                    case "2": analyticsSvc.DisplayCategoryPopularity(analyticsSvc.GetCategoryPopularity()); break;
                    case "3": DisplayCourseTable(efCourses.GetTopCoursesByEnrollment(5)); break;
                    case "4":
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

        static void ShowAdoMenu()
            {
            bool back = false;
            while (!back)
                {
                Console.Clear();
                Console.WriteLine("╔════════════════════════════════════════╗");
                Console.WriteLine("║         ADO.NET OPERATIONS             ║");
                Console.WriteLine("╚════════════════════════════════════════╝");
                Console.WriteLine("  [1]  View All Students");
                Console.WriteLine("  [2]  View All Courses");
                Console.WriteLine("  [3]  View Enrollments for Student");
                Console.WriteLine("  [0]  Back");
                Console.Write("  Choice: ");
                string ch = Console.ReadLine(); Console.Clear();
                switch (ch)
                    {
                    case "1": adoService.DisplayStudents(adoService.GetAllStudentsAdo()); break;
                    case "2": adoService.DisplayCourses(adoService.GetAllCoursesAdo()); break;
                    case "3":
                        Console.Write("  Student ID: "); int.TryParse(Console.ReadLine(), out int sid);
                        adoService.GetEnrollmentsByStudent(sid); break;
                    case "0": back = true; break;
                    default: Console.WriteLine("  Invalid choice."); break;
                    }
                if (!back) Console.ReadKey();
                }
            }

        static void ShowAdvancedQueryMenu()
            {
            bool back = false;
            while (!back)
                {
                Console.Clear();
                Console.WriteLine("╔════════════════════════════════════════╗");
                Console.WriteLine("║         ADVANCED QUERIES               ║");
                Console.WriteLine("╚════════════════════════════════════════╝");
                Console.WriteLine("  [1]  Search Courses (keyword+cat+diff)");
                Console.WriteLine("  [2]  Popular Courses (top N)");
                Console.WriteLine("  [3]  Student Recommendations");
                Console.WriteLine("  [4]  Instructor At-Risk Report");
                Console.WriteLine("  [5]  Students by Progress + Category");
                Console.WriteLine("  [6]  Courses with Min 10 Enrollments");
                Console.WriteLine("  [7]  Instructors by Category");
                Console.WriteLine("  [8]  Busy Instructors (by course count)");
                Console.WriteLine("  [0]  Back");
                Console.Write("  Choice: ");
                string ch = Console.ReadLine(); Console.Clear();
                switch (ch)
                    {
                    case "1":
                        Console.Write("  Keyword   : "); string kw = Console.ReadLine();
                        Console.Write("  Category  : "); string cat = Console.ReadLine();
                        Console.Write("  Difficulty: "); string dif = Console.ReadLine();
                        var sr = courseSvc.SearchCourses(kw, cat, dif);
                        DisplayCourseTable(sr); break;
                    case "2":
                        Console.Write("  Top N: "); int.TryParse(Console.ReadLine(), out int n);
                        courseSvc.DisplayPopularCourses(courseSvc.GetPopularCourses(n > 0 ? n : 5)); break;
                    case "3":
                        Console.Write("  Student ID: "); int.TryParse(Console.ReadLine(), out int sid);
                        courseSvc.DisplayRecommendedCourses(courseSvc.GetRecommendedCourses(sid)); break;
                    case "4":
                        Console.Write("  Instructor ID: "); int.TryParse(Console.ReadLine(), out int iid);
                        instructorSvc.GetStudentsAtRisk(iid); break;
                    case "5":
                        Console.Write("  Min progress %: "); int.TryParse(Console.ReadLine(), out int mp2);
                        Console.Write("  Category      : "); string mcat = Console.ReadLine();
                        var mf = efStudents.GetStudentsByProgressAndCategory(mp2, mcat);
                        foreach (var s in mf) efStudents.DisplayStudentEntity(s); break;
                    case "6":
                        DisplayCourseTable(efCourses.GetCoursesWithMinEnrollments(10)); break;
                    case "7":
                        Console.Write("  Category: ");
                        efInstructors.DisplayInstructorList(efInstructors.GetInstructorsByCategory(Console.ReadLine())); break;
                    case "8":
                        Console.Write("  Min courses: "); int.TryParse(Console.ReadLine(), out int mc);
                        var busy = efInstructors.GetBusyInstructors(mc);
                        foreach (var bi in busy) Console.WriteLine($"  {bi.Username,-24} — {bi.TaughtCourses.Count} courses"); break;
                    case "0": back = true; break;
                    default: Console.WriteLine("  Invalid choice."); break;
                    }
                if (!back) Console.ReadKey();
                }
            }

        // ════════════════════════════════════════════════════════
        //  SHARED UTILITIES
        // ════════════════════════════════════════════════════════
        static void BrowseAllCourses()
            {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════╗");
            Console.WriteLine("║           AVAILABLE COURSES            ║");
            Console.WriteLine("╚════════════════════════════════════════╝\n");
            DisplayCourseTable(efCourses.GetAllCourses());
            Console.ReadKey();
            }

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

        static void ShowAnalyticsMenu()
            {
            bool back = false;
            while (!back)
                {
                Console.Clear();
                Console.WriteLine("╔════════════════════════════════════════╗");
                Console.WriteLine("║         ANALYTICS & REPORTS            ║");
                Console.WriteLine("╚════════════════════════════════════════╝");
                Console.WriteLine("  [1]  System Statistics (Wk8)");
                Console.WriteLine("  [2]  Category Popularity");
                Console.WriteLine("  [3]  Top 5 Courses by Enrollment");
                Console.WriteLine("  [4]  Enrollments by Category");
                Console.WriteLine("  [5]  System Analytics (Wk6)");
                Console.WriteLine("  [0]  Back");
                Console.Write("  Choice: ");
                string ch = Console.ReadLine(); Console.Clear();
                switch (ch)
                    {
                    case "1": analyticsSvc.DisplaySystemStats(); break;
                    case "2": analyticsSvc.DisplayCategoryPopularity(analyticsSvc.GetCategoryPopularity()); break;
                    case "3": DisplayCourseTable(efCourses.GetTopCoursesByEnrollment(5)); break;
                    case "4":
                        var grp = efCourses.GetEnrollmentsByCategory();
                        Console.WriteLine($"  {"Category",-26}{"Total Enrollments"}");
                        Console.WriteLine("  " + new string('─', 44));
                        foreach (var kv in grp) Console.WriteLine($"  {kv.Key,-26}{kv.Value}");
                        break;
                    case "5": efAdmin.DisplaySystemAnalytics(); break;
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
            Console.WriteLine($"\n  {"ID",-6}{"Title",-34}{"Category",-20}{"Difficulty",-14}{"Enrolled",-10}Capacity");
            Console.WriteLine("  " + new string('─', 92));
            foreach (var c in courses)
                Console.WriteLine($"  {c.CourseId,-6}{c.Title,-34}{c.Category,-20}{c.DifficultyLevel,-14}{c.CurrentEnrollments,-10}{c.MaxCapacity}");
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

// ─────────────────────────────────────────────────────────────────────────────
//  Package Manager Console commands:
//
//  Install packages (if not already):
//    Install-Package Microsoft.EntityFrameworkCore.SqlServer
//    Install-Package Microsoft.EntityFrameworkCore.Tools
//    Install-Package Microsoft.EntityFrameworkCore.Design
//    Install-Package Microsoft.Data.SqlClient
//
//  Migrations (Assignment 8):
//    Add-Migration AddAuditLogsAndIndexes
//    Update-Database
//Add-Migration FixAuditLogNullableColumns
//Update - Database
// ─────────────────────────────────────────────────────────────────────────────