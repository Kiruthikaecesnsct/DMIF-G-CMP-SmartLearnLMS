using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Week_1.Data;

namespace Week_1.Services
    {
    public class EnrollmentService
        {
        // THIS IS FOR ADO.NET DEMO

        //private NotificationService notificationService = new NotificationService();

        //public void EnrollStudent(Student student, Course course)
        //    {
        //    if (course.CanEnroll(student))
        //        {
        //        course.Enroll(student);
        //        student.EnrolledCourseIds.Add(course.Id);  // uses your Id, not CourseId
        //        notificationService.NotifyEnrollment(student, course.Title);
        //        }
        //    else
        //        {
        //        Console.WriteLine($"❌ Cannot enroll: {course.Title} is full.");
        //        }
        //    }

        private NotificationService notificationService = new NotificationService();

        // Enroll a student into a course using database IDs
        public void EnrollStudent(int studentId, int courseId)
            {
            using (var db = new SmartLearnDbContext())
                {
                try
                    {
                    // Check if already enrolled
                    bool alreadyEnrolled = db.Enrollments
                        .Any(e => e.StudentId == studentId && e.CourseId == courseId);

                    if (alreadyEnrolled)
                        {
                        Console.WriteLine("❌ Already enrolled in this course!");
                        return;
                        }

                    // Check course exists and has space
                    var course = db.Courses.Find(courseId);
                    if (course == null)
                        {
                        Console.WriteLine("❌ Course not found!");
                        return;
                        }

                    if (course.CurrentEnrollments >= course.MaxStudents)
                        {
                        Console.WriteLine($"❌ Cannot enroll — {course.Title} is full!");
                        return;
                        }

                    // Get the student
                    var student = db.Students.Find(studentId);
                    if (student == null)
                        {
                        Console.WriteLine("❌ Student not found!");
                        return;
                        }

                    // Create the enrollment record
                    // Use object initializer — do NOT use the old constructor
                    // so StudentId and CourseId foreign keys are set correctly
                    var enrollment = new Enrollment
                        {
                        StudentId = studentId,
                        CourseId = courseId,
                        EnrolledDate = DateTime.Now,
                        ProgressPercent = 0,
                        Status = "Active"
                        };

                    db.Enrollments.Add(enrollment);

                    // Update enrollment count on the course
                    course.CurrentEnrollments++;

                    // One SaveChanges saves BOTH changes in one transaction
                    db.SaveChanges();

                    Console.WriteLine($"✅ {student.Username} enrolled in {course.Title}!");

                    // Notify the student
                    notificationService.NotifyEnrollment(student, course.Title);
                    }
                catch (Exception ex)
                    {
                    Console.WriteLine($"❌ Enrollment error: {ex.Message}");
                    if (ex.InnerException != null)
                        Console.WriteLine($"   Details: {ex.InnerException.Message}");
                    }
                }
            }

        // Update progress using enrollment ID
        public void UpdateEnrollmentProgress(int enrollmentId, int percentage)
            {
            using (var db = new SmartLearnDbContext())
                {
                var enrollment = db.Enrollments.Find(enrollmentId);
                if (enrollment != null)
                    {
                    enrollment.UpdateProgress(percentage);
                    db.SaveChanges();

                    var student = db.Students.Find(enrollment.StudentId);
                    if (student != null)
                        notificationService.NotifyProgress(student, percentage);
                    }
                }
            }
        
    // Drop a student from a course
        public void DropCourse(int studentId, int courseId)
            {
            using (var db = new SmartLearnDbContext())
                {
                var enrollment = db.Enrollments
                    .FirstOrDefault(e => e.StudentId == studentId && e.CourseId == courseId);

                if (enrollment == null)
                    { Console.WriteLine("❌ Enrollment not found!"); return; }

                enrollment.Drop();
                db.SaveChanges();
                Console.WriteLine("✅ Course dropped successfully.");
                }
            }

        // Mark an enrollment as completed
        public void MarkAsCompleted(int enrollmentId)
            {
            using (var db = new SmartLearnDbContext())
                {
                var enrollment = db.Enrollments.Find(enrollmentId);
                if (enrollment == null)
                    { Console.WriteLine("❌ Enrollment not found!"); return; }

                enrollment.MarkComplete();
                db.SaveChanges();

                var student = db.Students.Find(enrollment.StudentId);
                if (student != null)
                    notificationService.NotifyProgress(student, 100);

                Console.WriteLine("🎉 Enrollment marked as completed!");
                }
            }
        }
    }
