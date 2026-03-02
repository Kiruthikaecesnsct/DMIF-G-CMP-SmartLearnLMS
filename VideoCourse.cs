using SmartLearnLMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Week_1;


    namespace Week_1
    {
        public class VideoCourse : Course
        {
            public int VideoDurationMinutes { get; set; }
            public string StreamingUrl { get; set; }

            public VideoCourse(int id, string title, string description, int duration, string url = "")
                : base(id, title, description)
            {
                VideoDurationMinutes = duration;
                StreamingUrl = url;
            }

            public override void DisplayCourseInfo()
            {
                Console.WriteLine($"  [VIDEO COURSE] {Title}");
                Console.WriteLine($"  Description : {Description}");
                Console.WriteLine($"  Duration    : {VideoDurationMinutes} mins");
                Console.WriteLine($"  Stream URL  : {(string.IsNullOrEmpty(StreamingUrl) ? "N/A" : StreamingUrl)}");
                Console.WriteLine($"  Rating      : {GetAverageRating():F1} / 5  ({GetTotalRatings()} reviews)");
            }

            public override bool CanEnroll() => true; // Always open

            public override int GetEstimatedHours() => VideoDurationMinutes / 60;
        }
    }


