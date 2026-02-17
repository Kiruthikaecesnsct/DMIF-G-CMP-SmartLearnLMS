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
        public List<string> EnrolledStudentUsernames { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public Course()
        {
            EnrolledStudentUsernames = new List<string>();
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
