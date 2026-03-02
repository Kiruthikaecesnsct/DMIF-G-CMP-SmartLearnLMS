using SmartLearnLMS;
using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Week_1
{
    public class LiveCourse : Course
    {
        public DateTime SessionTime { get; set; }
        public int MaxStudents { get; set; }
        public int CurrentEnrollments { get; set; }
        public string MeetingLink { get; set; }

        public LiveCourse(int id, string title, string description, DateTime sessionTime, int maxStudents, string meetingLink = "")
            : base(id, title, description)
        {
            SessionTime = sessionTime;
            MaxStudents = maxStudents;
            CurrentEnrollments = 0;
            MeetingLink = meetingLink;
        }

        public override void DisplayCourseInfo()
        {
            Console.WriteLine($"  [LIVE COURSE] {Title}");
            Console.WriteLine($"  Description  : {Description}");
            Console.WriteLine($"  Session Time : {SessionTime:g}");
            Console.WriteLine($"  Seats        : {CurrentEnrollments}/{MaxStudents}");
            Console.WriteLine($"  Meeting Link : {(string.IsNullOrEmpty(MeetingLink) ? "N/A" : MeetingLink)}");
            Console.WriteLine($"  Rating       : {GetAverageRating():F1} / 5  ({GetTotalRatings()} reviews)");
        }

        public override bool CanEnroll() => CurrentEnrollments < MaxStudents;

        public override int GetEstimatedHours() => 20;
    }
}
