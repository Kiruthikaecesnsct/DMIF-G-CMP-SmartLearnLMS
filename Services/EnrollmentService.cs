using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Week_1.Services
    {
    /// <summary>
    /// Single Responsibility: handles all enrollment logic.
    /// Works against IEnrollable — not tied to Course specifically.
    /// </summary>
    public class EnrollmentService
        {
        public bool EnrollStudent(IEnrollable enrollable, Student student)
            {
            if (!enrollable.CanEnroll(student))
                {
                Console.WriteLine("  ✗ Enrollment failed: course is full or student not eligible.");
                return false;
                }
            enrollable.Enroll(student);
            return true;
            }

        public void DropStudent(IEnrollable enrollable, Student student)
            {
            enrollable.Drop(student);
            }

        public bool CheckAvailability(IEnrollable enrollable)
            {
            return enrollable.GetAvailableSeats() > 0;
            }

        public void DisplayEnrollmentStatus(IEnrollable enrollable)
            {
            int seats = enrollable.GetAvailableSeats();
            Console.WriteLine(seats > 0
                ? $"  ✓ Available — {seats} seat(s) remaining."
                : "  ✗ No seats available.");
            }
        }
    }
