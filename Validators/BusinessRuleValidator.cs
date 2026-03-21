using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Week_1.Validators
    {
    using Microsoft.EntityFrameworkCore;
    using Week_1.Data;

    public class BusinessRuleValidator
        {
        private readonly SmartLearnDbContext _db;

        public BusinessRuleValidator(SmartLearnDbContext db)
            {
            _db = db;
            }

        public bool CanEnroll(int studentId, int courseId, out List<string> reasons)
            {
            reasons = new List<string>();

            var student = _db.Students.Find(studentId);
            if (student == null) { reasons.Add("Student not found"); return false; }

            var course = _db.Courses.Include(c => c.Enrollments).FirstOrDefault(c => c.CourseId == courseId);
            if (course == null) { reasons.Add("Course not found"); return false; }

            if (_db.Enrollments.Any(e => e.StudentId == studentId && e.CourseId == courseId))
                {
                reasons.Add("Already enrolled in this course");
                return false;
                }

            if (course.Enrollments.Count >= course.MaxCapacity)
                {
                reasons.Add($"Course is full ({course.Enrollments.Count}/{course.MaxCapacity})");
                return false;
                }

            int activeCount = _db.Enrollments.Count(e => e.StudentId == studentId && e.Status == "Active");
            if (activeCount >= 5)
                {
                reasons.Add($"Maximum active enrollments reached ({activeCount}/5)");
                return false;
                }

            return true;
            }

        public bool CanDropCourse(int enrollmentId, out List<string> reasons)
            {
            reasons = new List<string>();

            var enrollment = _db.Enrollments.Include(e => e.Course).FirstOrDefault(e => e.EnrollmentId == enrollmentId);
            if (enrollment == null) { reasons.Add("Enrollment not found"); return false; }
            if (enrollment.Status == "Completed") { reasons.Add("Cannot drop a completed course"); return false; }
            if (enrollment.Status == "Dropped") { reasons.Add("Course already dropped"); return false; }

            var enrollmentDuration = (DateTime.Now - enrollment.EnrolledDate).Days;
            if (enrollmentDuration > 90) { reasons.Add("Cannot drop course after 90 days of enrollment"); return false; }

            return true;
            }

        public bool CanMarkAsCompleted(int enrollmentId, out List<string> reasons)
            {
            reasons = new List<string>();

            var enrollment = _db.Enrollments.Find(enrollmentId);
            if (enrollment == null) { reasons.Add("Enrollment not found"); return false; }
            if (enrollment.Status == "Completed") { reasons.Add("Already marked as completed"); return false; }
            if (enrollment.ProgressPercent < 100) { reasons.Add($"Progress must be 100% (currently {enrollment.ProgressPercent}%)"); return false; }

            return true;
            }

        public bool CanUpdateProgress(int enrollmentId, double newProgress, out List<string> reasons)
            {
            reasons = new List<string>();

            if (!InputValidator.IsValidProgress(newProgress, out string progressError))
                {
                reasons.Add(progressError);
                return false;
                }

            var enrollment = _db.Enrollments.Find(enrollmentId);
            if (enrollment == null) { reasons.Add("Enrollment not found"); return false; }
            if (enrollment.Status != "Active") { reasons.Add($"Cannot update progress. Status: {enrollment.Status}"); return false; }
            if (newProgress < enrollment.ProgressPercent) { reasons.Add($"New progress ({newProgress}%) cannot be less than current ({enrollment.ProgressPercent}%)"); return false; }

            return true;
            }

        public bool CanCreateCourse(int instructorId, string title, out List<string> reasons)
            {
            reasons = new List<string>();

            var instructor = _db.Instructors.Find(instructorId);
            if (instructor == null) { reasons.Add("Instructor not found"); return false; }

            if (!InputValidator.IsValidCourseTitle(title, out string titleError)) { reasons.Add(titleError); return false; }

            if (_db.Courses.Any(c => c.Title.ToLower() == title.ToLower()))
                {
                reasons.Add("A course with this title already exists");
                return false;
                }

            int instructorCourseCount = _db.Courses.Count(c => c.InstructorId == instructorId);
            if (instructorCourseCount >= 10) { reasons.Add($"Instructor has reached maximum course limit ({instructorCourseCount}/10)"); return false; }

            return true;
            }
        }
    }
