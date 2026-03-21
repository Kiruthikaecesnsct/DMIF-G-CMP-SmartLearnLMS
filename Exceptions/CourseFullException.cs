using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Week_1.Exceptions
    {
    public class CourseFullException : EnrollmentException
        {
        public int MaxCapacity { get; set; }
        public int CurrentEnrollments { get; set; }

        public CourseFullException(int studentId, int courseId, int maxCapacity, int currentEnrollments)
            : base($"Course is at full capacity ({currentEnrollments}/{maxCapacity})", studentId, courseId)
            {
            MaxCapacity = maxCapacity;
            CurrentEnrollments = currentEnrollments;
            }
        }
    }
