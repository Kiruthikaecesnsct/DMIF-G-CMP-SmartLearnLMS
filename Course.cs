using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Week_1.Interfaces;



namespace Week_1
{
    public class Course : IEnrollable, IRatable, ISearchable
        {
        // All your existing properties — unchanged
        public int MaxStudents { get; set; }
        public int Id { get; set; }
        public List<string> EnrolledStudentUsernames { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Difficulty { get; set; }
        public int CurrentEnrollments { get; set; }

        // New fields for IRatable
        public double AverageRating { get; private set; }
        private List<(int rating, string review)> reviews = new List<(int, string)>();

        public Course()
            {
            EnrolledStudentUsernames = new List<string>();
            }

        // Your existing GetAllCourses — only change is adding Description values
        // so MatchesSearch doesn't crash on null
        public static List<Course> GetAllCourses()
            {
            return new List<Course>
            {
                new Course { Id = 1, Title = "C# Basics",          Description = "Introduction to C# programming",   Category = "Programming", Difficulty = "Beginner",     CurrentEnrollments = 120, MaxStudents = 200 },
                new Course { Id = 2, Title = "Advanced LINQ",      Description = "Deep dive into LINQ queries",       Category = "Programming", Difficulty = "Advanced",     CurrentEnrollments = 45,  MaxStudents = 100 },
                new Course { Id = 3, Title = "Web Design 101",     Description = "HTML and CSS fundamentals",         Category = "Design",      Difficulty = "Beginner",     CurrentEnrollments = 98,  MaxStudents = 150 },
                new Course { Id = 4, Title = "UI/UX Principles",   Description = "User interface design principles",  Category = "Design",      Difficulty = "Intermediate", CurrentEnrollments = 60,  MaxStudents = 80  },
                new Course { Id = 5, Title = "Data Science Intro", Description = "Introduction to data science",      Category = "Data",        Difficulty = "Intermediate", CurrentEnrollments = 75,  MaxStudents = 120 },
                new Course { Id = 6, Title = "Machine Learning",   Description = "ML algorithms and applications",    Category = "Data",        Difficulty = "Advanced",     CurrentEnrollments = 30,  MaxStudents = 60  },
            };
            }

        // Your existing enrollment methods — unchanged
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

        // Your existing ISearchable — fixed null crash on Description
        public bool MatchesSearch(string keyword)
            {
            string desc = Description ?? "";
            return Title.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                   desc.Contains(keyword, StringComparison.OrdinalIgnoreCase);
            }

        public string GetSearchSummary()
            {
            return $"{Title} - {Description}";
            }

        // NEW — IRatable implementation
        public void AddRating(int rating, string review)
            {
            if (rating < 1 || rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5");

            reviews.Add((rating, review));
            AverageRating = reviews.Average(r => r.rating);
            Console.WriteLine($"⭐ Rating added to {Title}! Average: {AverageRating:F1}");
            }

        public List<(int rating, string review)> GetReviews()
            {
            return reviews;
            }
        }

    }
