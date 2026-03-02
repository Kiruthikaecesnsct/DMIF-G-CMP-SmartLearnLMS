using SmartLearnLMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Week_1;

namespace Week_1
{
    public class VideoCourse : Course, IEnrollable
    {
        public int VideoDurationMinutes { get; set; }

        public VideoCourse(int id, string title, string description, int duration)
            : base(id, title, description)
        {
            VideoDurationMinutes = duration;
        }

        public override void DisplayCourseInfo()
        {
            Console.WriteLine($"[VIDEO] {Title}");
            Console.WriteLine($"Duration: {VideoDurationMinutes} mins");
        }

        public override bool CanEnroll()
        {
            return true; // Always open 
        }

        public override int GetEstimatedHours()
        {
            return VideoDurationMinutes / 60;
        }
    }

}
