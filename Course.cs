using System;
using System.Collections.Generic;
using System.Linq;

namespace Week_1
{
    public abstract class Course : IEnrollable, ISearchable, IRatable
    {
        public int CourseId { get; set; }

        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                if (string.IsNullOrWhiteSpace(value) || value.Length > 100)
                {
                    Console.WriteLine("  ✗ Title must be non-empty and max 100 chars.");
                    return;
                }
                _title = value;
            }
        }

        public string Description { get; set; }

        // Enrolled student usernames (for IEnrollable)
        public List<string> EnrolledStudentUsernames { get; set; } = new List<string>();

        // Ratings (for IRatable)
        private List<int> ratings = new List<int>();
        private List<string> reviews = new List<string>();

        public Course(int id, string title, string description)
        {
            CourseId = id;
            _title = title;
            Description = description;
        }

        // --- Abstract methods ---
        public abstract void DisplayCourseInfo();
        public abstract bool CanEnroll();
        public abstract int GetEstimatedHours();

        // --- IEnrollable ---
        public void Enroll(Student student)
        {
            if (!CanEnroll(student))
            {
                Console.WriteLine("  ✗ Cannot enroll - course full!");
                return;
            }
            if (!EnrolledStudentUsernames.Contains(student.Username))
            {
                EnrolledStudentUsernames.Add(student.Username);
                Console.WriteLine($"  ✓ {student.Username} enrolled in {Title}!");
            }
            else
                Console.WriteLine("  Already enrolled.");
        }

        public void Drop(Student student)
        {
            EnrolledStudentUsernames.Remove(student.Username);
            Console.WriteLine($"  ✓ {student.Username} dropped from {Title}.");
        }

        public bool CanEnroll(Student student)
        {
            return CanEnroll(); // delegates to abstract method
        }

        public int GetAvailableSeats()
        {
            // Derived live courses override CanEnroll with MaxStudents logic
            // For base, return a large number if not overridden
            return 999;
        }

        // --- IRatable ---
        public void AddRating(int stars, string review)
        {
            if (stars < 1 || stars > 5)
            {
                Console.WriteLine("  ✗ Rating must be 1-5 stars.");
                return;
            }
            ratings.Add(stars);
            reviews.Add(review);
            Console.WriteLine($"  ✓ Rating added: {stars} ⭐");
        }

        public double GetAverageRating() => ratings.Count > 0 ? ratings.Average() : 0;

        public int GetTotalRatings() => ratings.Count;

        // --- ISearchable ---
        public bool MatchesSearch(string keyword)
        {
            return (Title ?? "").Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                   (Description ?? "").Contains(keyword, StringComparison.OrdinalIgnoreCase);
        }

        public string GetSearchSummary() => $"{Title} - {Description}";

        // Keep a DisplayInfo wrapper so existing Program.cs code doesn't break
        public void DisplayInfo()
        {
            DisplayCourseInfo();
        }
    }
}