using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Week_1.Exceptions
    {
    public class EnrollmentException : Exception
        {
        public int StudentId { get; set; }
        public int CourseId { get; set; }

        public EnrollmentException(string message, int studentId, int courseId)
            : base(message)
            {
            StudentId = studentId;
            CourseId = courseId;
            }
        }
    }
