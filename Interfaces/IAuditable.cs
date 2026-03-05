using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Week_1.Interfaces
    {
    public interface IAuditable
        {
        DateTime CreatedDate { get; }
        DateTime ModifiedDate { get; set; }
        void UpdateTimestamp();
        }
    }
