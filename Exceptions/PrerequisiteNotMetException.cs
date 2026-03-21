using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Week_1.Exceptions
    {
    public class PrerequisiteNotMetException : EnrollmentException
        {
        public List<string> MissingPrerequisites { get; set; }

        public PrerequisiteNotMetException(int studentId, int courseId, List<string> missing)
            : base($"Prerequisites not met. Required: {string.Join(", ", missing)}", studentId, courseId)
            {
            MissingPrerequisites = missing;
            }
        }
    }
