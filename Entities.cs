using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Week_1
    {
    // ══════════════════════════════════════════════════════
    //  USER ENTITY  (maps to Users table)
    //  Assignment 6 — Part 2, Task 3
    // ══════════════════════════════════════════════════════
    public class UserEntity
        {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; }

        [Required]
        [MaxLength(20)]
        public string UserType { get; set; }   // "Student" | "Instructor" | "Admin"

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? LastLoginDate { get; set; }   // Part 5 Task 3 — added via migration

        public bool IsActive { get; set; } = true;

        // Navigation — enrollments this user has (as a student)
        public virtual ICollection<EnrollmentEntity> Enrollments { get; set; }
            = new List<EnrollmentEntity>();

        // Navigation — courses this user teaches (as an instructor)
        public virtual ICollection<CourseEntity> TaughtCourses { get; set; }
            = new List<CourseEntity>();
        }

    // ══════════════════════════════════════════════════════
    //  COURSE ENTITY  (maps to Courses table)
    // ══════════════════════════════════════════════════════
    public class CourseEntity
        {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CourseId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        [MaxLength(50)]
        public string Category { get; set; }

        [Required]
        [MaxLength(20)]
        public string DifficultyLevel { get; set; } = "Beginner";

        [Required]
        [MaxLength(20)]
        public string CourseType { get; set; } = "Online";   // Online | InPerson | Hybrid

        public int MaxCapacity { get; set; } = 30;
        public int CurrentEnrollments { get; set; } = 0;

        [MaxLength(100)]
        public string InstructorName { get; set; }  // denormalised display name

        public int? InstructorId { get; set; }      // FK to Users (optional)

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation
        public virtual UserEntity Instructor { get; set; }
        public virtual ICollection<EnrollmentEntity> Enrollments { get; set; }
            = new List<EnrollmentEntity>();
        }

    // ══════════════════════════════════════════════════════
    //  ENROLLMENT ENTITY  (maps to Enrollments table)
    // ══════════════════════════════════════════════════════
    public class EnrollmentEntity
        {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EnrollmentId { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        public int CourseId { get; set; }

        public DateTime EnrolledDate { get; set; } = DateTime.Now;

        [Range(0, 100)]
        public int ProgressPercent { get; set; } = 0;

        public DateTime? CompletionDate { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Active";   // Active | Completed | Dropped

        // Navigation
        public virtual UserEntity Student { get; set; }
        public virtual CourseEntity Course { get; set; }
        }
    }