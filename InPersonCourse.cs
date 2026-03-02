using System;

namespace Week_1
{
    public class InPersonCourse : Course
    {
        private int _maxStudents;
        public int MaxStudents
        {
            get => _maxStudents;
            set
            {
                if (value <= 0)
                {
                    Console.WriteLine("  ✗ Max students must be greater than 0");
                    return;
                }
                _maxStudents = value;
            }
        }

        public string RoomNumber { get; set; }
        public string Building { get; set; }

        public InPersonCourse(int id, string title, string description, string instructorName, string category, int maxStudents, string roomNumber, string building)
            : base(id, title, description, instructorName, category)
        {
            _maxStudents = maxStudents;
            RoomNumber = roomNumber;
            Building = building;
        }

        public override bool CanEnroll(Student student) => CurrentEnrollments < MaxStudents;

        public override int GetAvailableSeats() => MaxStudents - CurrentEnrollments;

        public override string GetCourseType() => "In-Person";

        public override void DisplayCourseInfo()
        {
            string status = CanEnroll(null) ? "✓ Open for Enrollment" : "✗ Full";
            Console.WriteLine($"╔═══════════════════════════════════════╗");
            Console.WriteLine($"║  IN-PERSON: {Title,-27}║");
            Console.WriteLine($"╚═══════════════════════════════════════╝");
            Console.WriteLine($"  Course ID       : {CourseId}");
            Console.WriteLine($"  Instructor      : {InstructorName}");
            Console.WriteLine($"  Category        : {Category}");
            Console.WriteLine($"  Location        : {Building}, Room {RoomNumber}");
            Console.WriteLine($"  Capacity        : {CurrentEnrollments}/{MaxStudents}");
            Console.WriteLine($"  Seats Remaining : {GetAvailableSeats()}");
            Console.WriteLine($"  Rating          : {GetAverageRating():F1}⭐ ({GetTotalRatings()} reviews)");
            Console.WriteLine($"  Status          : {status}");
        }
    }
}