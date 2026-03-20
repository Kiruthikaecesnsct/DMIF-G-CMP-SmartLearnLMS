using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Week_1
    {
    // ══════════════════════════════════════════════════════
    //  PART 1 TASK 2: COURSE MODULE HIERARCHY
    // ══════════════════════════════════════════════════════

    /// <summary>
    /// Represents a module (section) within a course.
    /// A Course contains many CourseModules; each module contains many Lessons.
    /// </summary>
    public class CourseModule
        {
        [Key]
        public int ModuleId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        [MaxLength(150)]
        public string Title { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        /// <summary>Display order within the course (1, 2, 3…).</summary>
        public int OrderIndex { get; set; }

        /// <summary>Estimated hours to complete this module.</summary>
        [Column(TypeName = "decimal(5,2)")]
        public decimal DurationHours { get; set; }

        // ── Navigation properties ──
        [ForeignKey(nameof(CourseId))]
        public virtual CourseEntity Course { get; set; }

        public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
        }

    // ══════════════════════════════════════════════════════
    //  PART 1 TASK 2: LESSON ENTITY
    // ══════════════════════════════════════════════════════

    /// <summary>
    /// Represents an individual lesson inside a CourseModule.
    /// </summary>
    public class Lesson
        {
        [Key]
        public int LessonId { get; set; }

        [Required]
        public int ModuleId { get; set; }

        [Required]
        [MaxLength(150)]
        public string Title { get; set; }

        [MaxLength(2000)]
        public string Content { get; set; }

        [MaxLength(500)]
        public string VideoUrl { get; set; }

        /// <summary>Display order within the module (1, 2, 3…).</summary>
        public int OrderIndex { get; set; }

        /// <summary>Estimated minutes to complete this lesson.</summary>
        public int DurationMinutes { get; set; }

        // ── Navigation property ──
        [ForeignKey(nameof(ModuleId))]
        public virtual CourseModule Module { get; set; }
        }

    // ══════════════════════════════════════════════════════
    //  PART 1 TASK 3: COURSE RATING ENTITY
    // ══════════════════════════════════════════════════════

    /// <summary>
    /// Represents a student's rating and review for a course.
    /// A student can only rate a course once (unique index on CourseId + StudentId).
    /// </summary>
    public class CourseRating
        {
        [Key]
        public int RatingId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        public int StudentId { get; set; }

        /// <summary>Star rating: 1 (lowest) to 5 (highest).</summary>
        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string ReviewText { get; set; }

        public DateTime RatingDate { get; set; } = DateTime.Now;

        // ── Navigation properties ──
        [ForeignKey(nameof(CourseId))]
        public virtual CourseEntity Course { get; set; }

        [ForeignKey(nameof(StudentId))]
        public virtual UserEntity Student { get; set; }
        }
    }