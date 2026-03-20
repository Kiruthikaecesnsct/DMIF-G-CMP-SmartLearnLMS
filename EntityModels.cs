using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Week_1
    {
    // ══════════════════════════════════════════════════════
    //  USER ENTITY  (already existed — adding Ratings nav)
    // ══════════════════════════════════════════════════════

    /// <summary>
    /// DB-mapped user entity (Student, Instructor, or Admin).
    /// Assignment 7: Added Ratings navigation property.
    /// </summary>
    public class UserEntity
        {
        [Key]
        public int UserId { get; set; }

        [Required, MaxLength(50)]
        public string Username { get; set; }

        [Required, MaxLength(100)]
        public string Email { get; set; }

        [Required, MaxLength(255)]
        public string PasswordHash { get; set; }

        [Required, MaxLength(20)]
        public string UserType { get; set; }   // "Student" | "Instructor" | "Admin"

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? LastLoginDate { get; set; }
        public bool IsActive { get; set; } = true;

        // ── Navigation properties ──
        public virtual ICollection<EnrollmentEntity> Enrollments { get; set; } = new List<EnrollmentEntity>();
        public virtual ICollection<CourseEntity> TaughtCourses { get; set; } = new List<CourseEntity>();

        // Assignment 7 — Part 1 Task 3
        public virtual ICollection<CourseRating> Ratings { get; set; } = new List<CourseRating>();
        }

    // ══════════════════════════════════════════════════════
    //  COURSE ENTITY  (already existed — adding Modules & Ratings nav)
    // ══════════════════════════════════════════════════════

    /// <summary>
    /// DB-mapped course entity.
    /// Assignment 7: Added Modules and Ratings navigation properties.
    /// </summary>
    public class CourseEntity
        {
        [Key]
        public int CourseId { get; set; }

        [Required, MaxLength(100)]
        public string Title { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [Required, MaxLength(50)]
        public string Category { get; set; }

        [Required, MaxLength(20)]
        public string DifficultyLevel { get; set; }  // Beginner | Intermediate | Advanced

        public int MaxCapacity { get; set; } = 30;
        public int CurrentEnrollments { get; set; } = 0;

        [MaxLength(100)]
        public string InstructorName { get; set; }

        public int? InstructorId { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // ── Navigation properties ──
        [ForeignKey(nameof(InstructorId))]
        public virtual UserEntity Instructor { get; set; }

        public virtual ICollection<EnrollmentEntity> Enrollments { get; set; } = new List<EnrollmentEntity>();

        // Assignment 7 — Part 1 Task 2
        public virtual ICollection<CourseModule> Modules { get; set; } = new List<CourseModule>();

        // Assignment 7 — Part 1 Task 3
        public virtual ICollection<CourseRating> Ratings { get; set; } = new List<CourseRating>();
        }

    // ══════════════════════════════════════════════════════
    //  ENROLLMENT ENTITY  (unchanged from Assignment 6)
    // ══════════════════════════════════════════════════════

    /// <summary>DB-mapped enrollment record linking a student to a course.</summary>
    public class EnrollmentEntity
        {
        [Key]
        public int EnrollmentId { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        public int CourseId { get; set; }

        public DateTime EnrolledDate { get; set; } = DateTime.Now;

        [Range(0, 100)]
        public int ProgressPercent { get; set; } = 0;

        public DateTime? CompletionDate { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "Active";  // Active | Completed | Dropped

        // ── Navigation properties ──
        [ForeignKey(nameof(StudentId))]
        public virtual UserEntity Student { get; set; }

        [ForeignKey(nameof(CourseId))]
        public virtual CourseEntity Course { get; set; }
        }
    }