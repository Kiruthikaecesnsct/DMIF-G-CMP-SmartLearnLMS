using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Week_1.Exceptions
    {
    public class InsufficientProgressException : Exception
        {
        public double CurrentProgress { get; set; }
        public double RequiredProgress { get; set; }

        public InsufficientProgressException(double current, double required)
            : base($"Insufficient progress. Current: {current}%, Required: {required}%")
            {
            CurrentProgress = current;
            RequiredProgress = required;
            }
        }
    }
