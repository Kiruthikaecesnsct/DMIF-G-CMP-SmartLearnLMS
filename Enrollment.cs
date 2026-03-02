using System;

namespace Week_1
{
    public class Enrollment
    {
        public int EnrollmentId { get; set; }
        public string StudentUsername { get; set; }
        public int CourseId { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public int Progress { get; set; }
        public bool IsCompleted { get; set; }

        public Enrollment(int id, string username, int courseId, DateTime date, int progress, bool completed)
        {
            EnrollmentId = id;
            StudentUsername = username;
            CourseId = courseId;
            EnrollmentDate = date;
            Progress = progress;
            IsCompleted = completed;
        }
    }
}