using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Week_1
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "CourseType")]
    [JsonDerivedType(typeof(OnlineCourse), "Online")]
    [JsonDerivedType(typeof(InPersonCourse), "InPerson")]
    [JsonDerivedType(typeof(HybridCourse), "Hybrid")]

    public abstract class Course : IEnrollable, ISearchable, IRatable, IAuditable, IReportable
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


        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public void UpdateModifiedDate() => ModifiedDate = DateTime.Now;

        // ── Parameterless constructor for JSON ──
        protected Course()
            {
            _ratings = new List<int>();
            _reviews = new List<string>();
            CreatedDate = DateTime.Now;
            ModifiedDate = DateTime.Now;
            }
        protected Course(int id, string title, string description,
                  string instructorName, string category)
            {
            CourseId = id;
            _title = title;
            Description = description;
            InstructorName = instructorName;
            Category = category;
            CurrentEnrollments = 0;
            _ratings = new List<int>();
            _reviews = new List<string>();
            CreatedDate = DateTime.Now;
            ModifiedDate = DateTime.Now;
            }

        // ── Static sample data factory ──
        public static List<Course> GetAllCourses()
        {
            return new List<Course>
            {
                // Online Courses (101-105)
                new OnlineCourse(101, "C# Fundamentals",
                    "Learn C# from scratch", "Prof. Smith", "Programming", 450),
                new OnlineCourse(102, "Python for Beginners",
                    "Intro to Python programming", "Prof. Johnson", "Programming", 360),
                new OnlineCourse(103, "Web Development Basics",
                    "HTML, CSS and JS fundamentals", "Prof. Garcia", "Web Development", 540),
                new OnlineCourse(104, "Data Structures",
                    "Arrays, Lists, Trees and more", "Prof. Smith", "Computer Science", 600),
                new OnlineCourse(105, "Machine Learning Intro",
                    "Basics of ML and AI concepts", "Prof. Lee", "Data Science", 720),

                // In-Person Courses (201-203)
                new InPersonCourse(201, "Database Design Workshop",
                    "Relational DB and SQL", "Prof. Johnson", "Database", 25, "B-101", "Engineering Building"),
                new InPersonCourse(202, "Network Security Lab",
                    "Practical cybersecurity skills", "Prof. Brown", "Security", 20, "C-205", "CS Building"),
                new InPersonCourse(203, "Mobile App Development",
                    "Build iOS and Android apps", "Prof. Garcia", "Mobile", 30, "A-301", "Tech Center"),

                // Hybrid Courses (301-302)
                new HybridCourse(301, "Full-Stack Development",
                    "Frontend + Backend full stack", "Prof. Garcia", "Web Development", 30, 720, "C-201", "CS Building"),
                new HybridCourse(302, "Cloud Computing",
                    "AWS, Azure and cloud concepts", "Prof. Lee", "Cloud", 25, 600, "D-101", "Engineering Building"),
            };
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
            Console.WriteLine($"  ✓ {student.Username} dropped from '{Title}'.");
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
            if (_ratings == null) _ratings = new List<int>();
            if (_reviews == null) _reviews = new List<string>();
            if (stars < 1 || stars > 5)
            {
                Console.WriteLine("  ✗ Rating must be 1-5 stars.");
                return;
            }
            _ratings.Add(stars);
            _reviews.Add(review);
            Console.WriteLine($"  ✓ Rating added: {stars}⭐");
        }

        public double GetAverageRating()
        {
            if (_ratings == null || _ratings.Count == 0) return 0;
            return _ratings.Average();
        }

        public int GetTotalRatings()
        {
            if (_ratings == null) return 0;
            return _ratings.Count;
        }

     
        // ── IReportable ──
        public string GenerateReport()
            {
            return $"""
                ════════════════════════════════════
                COURSE REPORT: {Title}
                ════════════════════════════════════
                ID             : {CourseId}
                Type           : {GetCourseType()}
                Instructor     : {InstructorName}
                Category       : {Category}
                Enrollments    : {CurrentEnrollments}
                Avg Rating     : {GetAverageRating():F1} ({GetTotalRatings()} ratings)
                Available Seats: {GetAvailableSeats()}
                Created        : {CreatedDate:d}
                ════════════════════════════════════
                """;
            }

        public void DisplayReport() => Console.WriteLine(GenerateReport());
        }
}