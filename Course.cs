// Course.cs — complete answer (show AFTER students try)
namespace SmartLearnLMS
{
    public class Course
    {
        public int CourseId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string InstructorName { get; set; }
        public int MaxStudents { get; set; }
        public int CurrentEnrollments { get; set; }

        public Course(int id, string title, string description,
                      string instructor, int maxStudents, int currentEnrollments)
        {
            CourseId = id;
            Title = title;
            Description = description;
            InstructorName = instructor;
            MaxStudents = maxStudents;
            CurrentEnrollments = currentEnrollments;
        }

        public bool CanEnroll()
        {
            return CurrentEnrollments < MaxStudents;
        }

        public void DisplayInfo()
        {
            Console.WriteLine($"  ID         : {CourseId}");
            Console.WriteLine($"  Title      : {Title}");
            Console.WriteLine($"  Instructor : {InstructorName}");
            Console.WriteLine($"  Enrolled   : {CurrentEnrollments}/{MaxStudents}");
            Console.WriteLine($"  Available  : {(CanEnroll() ? "Yes" : "Full")}");
        }
    }
}