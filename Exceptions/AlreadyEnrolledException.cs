using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Week_1.Exceptions
    {
    public class AlreadyEnrolledException : EnrollmentException
        {
        public DateTime OriginalEnrollmentDate { get; set; }

        public AlreadyEnrolledException(int studentId, int courseId, DateTime enrollmentDate)
            : base("Student is already enrolled in this course", studentId, courseId)
            {
            OriginalEnrollmentDate = enrollmentDate;
            }
        }
    }
