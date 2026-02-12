using System;

namespace Week_1
{
    public class Enrollment
    {
        public string StudentUsername { get; set; }
        public int CourseId { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public int ProgressPercentage { get; set; }
        public bool IsCompleted { get; set; }

        public Enrollment(string studentUsername, int courseId)
        {
            StudentUsername = studentUsername;
            CourseId = courseId;
            EnrollmentDate = DateTime.Now;
            ProgressPercentage = 0;
            IsCompleted = false;
        }

        public void UpdateProgress(int percentage)
        {
            ProgressPercentage = Math.Clamp(percentage, 0, 100);
            if (ProgressPercentage == 100) IsCompleted = true;
        }

        public void MarkComplete()
        {
            IsCompleted = true;
            ProgressPercentage = 100;
        }
    }
}