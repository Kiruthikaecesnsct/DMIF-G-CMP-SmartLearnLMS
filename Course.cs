using System;

namespace Week_1
{
    public class Course
    {
        public int CourseId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string InstructorName { get; set; }
        public int MaxStudents { get; set; }
        public int CurrentEnrollments { get; set; }
        public string Category { get; set; }

        public Course(int id, string title, string desc, string instructor, int max, int current, string category)
        {
            CourseId = id;
            Title = title;
            Description = desc;
            InstructorName = instructor;
            MaxStudents = max;
            CurrentEnrollments = current;
            Category = category;
        }

        public bool CanEnroll() => CurrentEnrollments < MaxStudents;

        public void IncrementEnrollment()
        {
            if (CanEnroll()) CurrentEnrollments++;
        }

        public void DecrementEnrollment()
        {
            if (CurrentEnrollments > 0) CurrentEnrollments--;
        }

        public void DisplayInfo()
        {
            Console.WriteLine($"[{CourseId}] {Title} ({Category})");
            Console.WriteLine($"   Instructor: {InstructorName}");
            Console.WriteLine($"   Slots: {CurrentEnrollments}/{MaxStudents}");
        }
    }
}