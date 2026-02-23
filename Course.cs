using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Week_1
{
    public class Course : IEnrollable, ISearchable

    {
        public int MaxStudents { get; set; }
        public int Id { get; set; }
        public List<string> EnrolledStudentUsernames { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }       // ← add this
        public string Difficulty { get; set; }     // ← add this
        public int CurrentEnrollments { get; set; }

        public Course()
        {
            EnrolledStudentUsernames = new List<string>();
        }
        public static List<Course> GetAllCourses()
        {
            return new List<Course>
    {
        new Course { Id = 1, Title = "C# Basics",          Category = "Programming", Difficulty = "Beginner",     CurrentEnrollments = 120 },
        new Course { Id = 2, Title = "Advanced LINQ",      Category = "Programming", Difficulty = "Advanced",     CurrentEnrollments = 45  },
        new Course { Id = 3, Title = "Web Design 101",     Category = "Design",      Difficulty = "Beginner",     CurrentEnrollments = 98  },
        new Course { Id = 4, Title = "UI/UX Principles",   Category = "Design",      Difficulty = "Intermediate", CurrentEnrollments = 60  },
        new Course { Id = 5, Title = "Data Science Intro", Category = "Data",        Difficulty = "Intermediate", CurrentEnrollments = 75  },
        new Course { Id = 6, Title = "Machine Learning",   Category = "Data",        Difficulty = "Advanced",     CurrentEnrollments = 30  },
    };
        }

        public void Enroll(Student student)
        {
            if (!CanEnroll(student))
            {
                Console.WriteLine("✗ Cannot enroll - course full!");
                return;
            }

            EnrolledStudentUsernames.Add(student.Username);
            Console.WriteLine($"✓ {student.Username} enrolled!");
        }

        public void Drop(Student student)
        {
            EnrolledStudentUsernames.Remove(student.Username);
            Console.WriteLine($"✓ {student.Username} dropped!");
        }

        public bool CanEnroll(Student student)
        {
            return EnrolledStudentUsernames.Count < MaxStudents;
        }

        public int GetAvailableSeats()
        {
            return MaxStudents - EnrolledStudentUsernames.Count;
        }
        public bool MatchesSearch(string keyword)
        {
            return Title.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                   Description.Contains(keyword, StringComparison.OrdinalIgnoreCase);
        }

        public string GetSearchSummary()
        {
            return $"{Title} - {Description}";
        }

    }

}
