using SmartLearnLMS;
using System;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Week_1
{
    public class TextCourse : Course
    {
        public List<string> ReadingMaterials { get; set; }
        public int PageCount { get; set; }

        public TextCourse(int id, string title, string description, int pageCount)
            : base(id, title, description)
        {
            PageCount = pageCount;
            ReadingMaterials = new List<string>();
        }

        public override void DisplayCourseInfo()
        {
            Console.WriteLine($"  [TEXT COURSE] {Title}");
            Console.WriteLine($"  Description : {Description}");
            Console.WriteLine($"  Pages       : {PageCount}");
            Console.WriteLine($"  Materials   : {ReadingMaterials.Count} item(s)");
            Console.WriteLine($"  Rating      : {GetAverageRating():F1} / 5  ({GetTotalRatings()} reviews)");
        }

        public override bool CanEnroll() => true;

        public override int GetEstimatedHours() => PageCount / 30; // ~30 pages/hour
    }
}