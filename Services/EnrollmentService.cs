using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Week_1.Services
    {
    public class EnrollmentService
        {
        private NotificationService notificationService = new NotificationService();

        public void EnrollStudent(Student student, Course course)
            {
            if (course.CanEnroll(student))
                {
                course.Enroll(student);
                student.EnrolledCourseIds.Add(course.Id);  // uses your Id, not CourseId
                notificationService.NotifyEnrollment(student, course.Title);
                }
            else
                {
                Console.WriteLine($"❌ Cannot enroll: {course.Title} is full.");
                }
            }
        }
    }
