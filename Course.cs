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
                    Console.WriteLine("  ✗ Title must be 1-100 characters");
                    return;
                }
                _title = value;
            }
        }

        public string Description { get; set; }
        public string InstructorName { get; set; }
        public string Category { get; set; }
        public int CurrentEnrollments { get; set; }

        private List<int> _ratings = new List<int>();
        private List<string> _reviews = new List<string>();

        protected Course(int id, string title, string description, string instructorName, string category)
        {
            CourseId = id;
            _title = title;
            Description = description;
            InstructorName = instructorName;
            Category = category;
            CurrentEnrollments = 0;
        }

        // ── Abstract methods ──
        public abstract bool CanEnroll(Student student);
        public abstract void DisplayCourseInfo();
        public abstract string GetCourseType();
        public abstract int GetAvailableSeats();

        // ── IEnrollable ──
        public void Enroll(Student student)
        {
            if (!CanEnroll(student))
            {
                Console.WriteLine("  ✗ Cannot enroll - course is full!");
                return;
            }
            CurrentEnrollments++;
            student.EnrollInCourse(CourseId);
            if (student is INotifiable notifiable)
                notifiable.SendNotification($"You have been enrolled in '{Title}'.");
            Console.WriteLine($"  ✓ {student.Username} enrolled in '{Title}'!");
        }

        public void Drop(Student student)
        {
            if (CurrentEnrollments > 0) CurrentEnrollments--;
            Console.WriteLine($"  ✓ {student.Username} has been dropped from '{Title}'.");
        }

        public void Drop(string username)
        {
            if (CurrentEnrollments > 0) CurrentEnrollments--;
            Console.WriteLine($"  ✓ Dropped from '{Title}'.");
        }

        // ── ISearchable ──
        public bool MatchesSearch(string keyword)
        {
            return (Title ?? "").Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                   (Description ?? "").Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                   (Category ?? "").Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                   (InstructorName ?? "").Contains(keyword, StringComparison.OrdinalIgnoreCase);
        }

        public string GetSearchSummary()
        {
            return $"[{GetCourseType()}] {Title} | {Category} | {InstructorName}";
        }

        // ── IRatable ──
        public void AddRating(int stars, string review)
        {
            if (stars < 1 || stars > 5)
            {
                Console.WriteLine("  ✗ Rating must be 1-5 stars.");
                return;
            }
            _ratings.Add(stars);
            _reviews.Add(review);
            Console.WriteLine($"  ✓ Rating added: {stars}⭐");
        }

        public double GetAverageRating() => _ratings.Count > 0 ? _ratings.Average() : 0;

        public int GetTotalRatings() => _ratings.Count;
    }
}