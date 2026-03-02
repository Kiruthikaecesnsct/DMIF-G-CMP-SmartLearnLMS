using System;
using System.Text.Json.Serialization;

namespace Week_1
{
    public class OnlineCourse : Course
    {
        public int VideoDurationMinutes { get; set; }
        public string StreamingUrl { get; set; }

        // ── Parameterless constructor for JSON ──
        public OnlineCourse() : base() { }

        public OnlineCourse(int id, string title, string description,
                            string instructorName, string category, int videoDurationMinutes)
            : base(id, title, description, instructorName, category)
        {
            VideoDurationMinutes = videoDurationMinutes;
            StreamingUrl = "https://smartlearn.com/stream/" + id;
        }

        public override bool CanEnroll(Student student) => true;
        public override int GetAvailableSeats() => int.MaxValue;
        public override string GetCourseType() => "Online";

        public override void DisplayCourseInfo()
        {
            double hours = Math.Round((double)VideoDurationMinutes / 60, 1);
            Console.WriteLine($"╔═══════════════════════════════════════╗");
            Console.WriteLine($"║  ONLINE: {Title,-30}║");
            Console.WriteLine($"╚═══════════════════════════════════════╝");
            Console.WriteLine($"  Course ID       : {CourseId}");
            Console.WriteLine($"  Instructor      : {InstructorName}");
            Console.WriteLine($"  Category        : {Category}");
            Console.WriteLine($"  Duration        : {VideoDurationMinutes} minutes ({hours} hours)");
            Console.WriteLine($"  Capacity        : Unlimited");
            Console.WriteLine($"  Current Students: {CurrentEnrollments}");
            Console.WriteLine($"  Rating          : {GetAverageRating():F1}⭐ ({GetTotalRatings()} reviews)");
            Console.WriteLine($"  Status          : ✓ Open for Enrollment");
        }
    }
}