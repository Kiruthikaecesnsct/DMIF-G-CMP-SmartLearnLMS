using System;
using Week_1.Exceptions;
using Week_1.Logging;
using Week_1.Validators;

namespace Week_1.Tests
    {
    // ══════════════════════════════════════════════════════════════════
    //  ASSIGNMENT 8 — PART 1 TASK 3 + PART 2 TASK 4: Test Cases
    // ══════════════════════════════════════════════════════════════════

    /// <summary>
    /// Standalone test runner that exercises error scenarios, custom exceptions,
    /// and validation rules. Call <see cref="RunAll"/> from Program.cs for verification.
    /// </summary>
    public static class SmartLearnTests
        {
        private static readonly Logger _log = Logger.Instance;
        private static int _pass;
        private static int _fail;

        /// <summary>Runs all test categories and prints a summary.</summary>
        public static void RunAll()
            {
            _pass = _fail = 0;
            Console.WriteLine("\n╔════════════════════════════════════════════╗");
            Console.WriteLine("║     SMARTLEARN TEST SUITE (Assignment 8)   ║");
            Console.WriteLine("╚════════════════════════════════════════════╝\n");

            RunInputValidationTests();
            RunBusinessRuleTests();
            RunExceptionTests();

            Console.WriteLine($"\n══ Results: {_pass} passed  |  {_fail} failed ══\n");
            }

        // ── Input Validation Tests ─────────────────────────────────────

        private static void RunInputValidationTests()
            {
            Console.WriteLine("── Input Validation Tests ─────────────────");

            // Email
            AssertValid(InputValidator.ValidateEmail("alice@example.com", out _),
                        "Valid email accepted");
            AssertInvalid(InputValidator.ValidateEmail("notanemail", out _),
                          "No @ rejected");
            AssertInvalid(InputValidator.ValidateEmail("bad@nodot", out _),
                          "Missing TLD rejected");
            AssertInvalid(InputValidator.ValidateEmail("", out _),
                          "Empty email rejected");

            // Username
            AssertValid(InputValidator.ValidateUsername("alice123", out _),
                        "Valid username accepted");
            AssertInvalid(InputValidator.ValidateUsername("ab", out _),
                          "Too-short username rejected");
            AssertInvalid(InputValidator.ValidateUsername("this_username_is_way_too_long_for_sure", out _),
                          "Too-long username rejected");
            AssertInvalid(InputValidator.ValidateUsername("bad name!", out _),
                          "Special chars rejected");

            // Password
            AssertValid(InputValidator.ValidatePassword("Secure@123", out _),
                        "Valid password accepted");
            AssertInvalid(InputValidator.ValidatePassword("short1@A", out _),
                          "Just-under-8-char password rejected");
            AssertInvalid(InputValidator.ValidatePassword("alllowercase1!", out _),
                          "No uppercase rejected");
            AssertInvalid(InputValidator.ValidatePassword("ALLUPPERCASE1!", out _),
                          "No lowercase rejected");
            AssertInvalid(InputValidator.ValidatePassword("NoDigitHere!", out _),
                          "No digit rejected");
            AssertInvalid(InputValidator.ValidatePassword("NoSpecial123", out _),
                          "No special char rejected");

            // Progress
            AssertValid(InputValidator.ValidateProgress(0, out _), "0% valid");
            AssertValid(InputValidator.ValidateProgress(100, out _), "100% valid");
            AssertInvalid(InputValidator.ValidateProgress(-1, out _), "Negative progress rejected");
            AssertInvalid(InputValidator.ValidateProgress(101, out _), ">100% rejected");

            // Course title
            AssertValid(InputValidator.ValidateCourseTitle("Intro to C#", out _),
                        "Valid title accepted");
            AssertInvalid(InputValidator.ValidateCourseTitle("abc", out _),
                          "Too-short title rejected");
            AssertInvalid(InputValidator.ValidateCourseTitle(new string('x', 101), out _),
                          "Too-long title rejected");
            }

        // ── Business Rule Tests (DB-independent logic only) ────────────

        private static void RunBusinessRuleTests()
            {
            Console.WriteLine("\n── Business Rule Tests (offline) ──────────");

            // We only test the rule validator's response to obviously invalid IDs
            // (avoids needing a live DB in the test runner).
            var bv = new BusinessRuleValidator();

            AssertInvalid(bv.CanEnroll(-1, -1, out _),
                          "CanEnroll: negative IDs rejected");
            AssertInvalid(bv.CanDropCourse(-1, -1, out _),
                          "CanDropCourse: negative IDs rejected");
            AssertInvalid(bv.CanMarkAsCompleted(-1, out _),
                          "CanMarkAsCompleted: negative ID rejected");
            AssertInvalid(bv.CanUpdateProgress(-1, -1, 50, out _),
                          "CanUpdateProgress: negative IDs rejected");
            AssertInvalid(bv.CanUpdateProgress(1, 1, 150, out _),
                          "CanUpdateProgress: >100 rejected before DB check");
            }

        // ── Custom Exception Tests ─────────────────────────────────────

        private static void RunExceptionTests()
            {
            Console.WriteLine("\n── Custom Exception Tests ──────────────────");

            // CourseFullException carries context
            TestThrows<CourseFullException>("CourseFullException thrown & caught", () =>
            {
                throw new CourseFullException(1, 101, 30, 30);
            });

            // AlreadyEnrolledException
            TestThrows<AlreadyEnrolledException>("AlreadyEnrolledException thrown & caught", () =>
            {
                throw new AlreadyEnrolledException(1, 101);
            });

            // PrerequisiteNotMetException
            TestThrows<PrerequisiteNotMetException>("PrerequisiteNotMetException thrown & caught", () =>
            {
                throw new PrerequisiteNotMetException(1, 101, 99);
            });

            // DuplicateUsernameException
            TestThrows<DuplicateUsernameException>("DuplicateUsernameException thrown & caught", () =>
            {
                throw new DuplicateUsernameException("alice");
            });

            // DuplicateEmailException
            TestThrows<DuplicateEmailException>("DuplicateEmailException thrown & caught", () =>
            {
                throw new DuplicateEmailException("alice@example.com");
            });

            // InvalidCredentialsException
            TestThrows<InvalidCredentialsException>("InvalidCredentialsException thrown & caught", () =>
            {
                throw new InvalidCredentialsException("alice");
            });

            // CourseNotFoundException
            TestThrows<CourseNotFoundException>("CourseNotFoundException thrown & caught", () =>
            {
                throw new CourseNotFoundException(999);
            });

            // InvalidProgressUpdateException — context properties
            try
                {
                var ex = new InvalidProgressUpdateException(1, 101, 70, 50);
                bool contextOk = ex.OldProgress == 70 && ex.NewProgress == 50;
                Assert(contextOk, "InvalidProgressUpdateException properties correct");
                }
            catch (Exception ex) { Assert(false, $"InvalidProgressUpdateException property check: {ex.Message}"); }

            // EnrollmentException is base of CourseFullException
            try
                {
                EnrollmentException caught = null;
                try { throw new CourseFullException(1, 101, 30, 30); }
                catch (EnrollmentException ex) { caught = ex; }
                Assert(caught != null, "CourseFullException caught as EnrollmentException (polymorphism)");
                Assert(caught.StudentId == 1, "EnrollmentException.StudentId preserved");
                Assert(caught.CourseId == 101, "EnrollmentException.CourseId preserved");
                }
            catch (Exception ex) { Assert(false, $"Polymorphism test: {ex.Message}"); }
            }

        // ── Helpers ────────────────────────────────────────────────────

        private static void AssertValid(bool result, string label)
            => Assert(result, label);

        private static void AssertInvalid(bool result, string label)
            => Assert(!result, label);

        private static void Assert(bool condition, string label)
            {
            if (condition)
                {
                _pass++;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"  ✓ PASS  {label}");
                }
            else
                {
                _fail++;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"  ✗ FAIL  {label}");
                }
            Console.ResetColor();
            }

        private static void TestThrows<TException>(string label, Action action)
            where TException : Exception
            {
            try
                {
                action();
                Assert(false, $"{label} — expected {typeof(TException).Name}, nothing thrown");
                }
            catch (TException)
                {
                Assert(true, label);
                }
            catch (Exception ex)
                {
                Assert(false, $"{label} — wrong exception: {ex.GetType().Name}");
                }
            }
        }
    }