using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Week_1
    {
    public interface IAuditable
        {
        DateTime CreatedDate { get; set; }
        DateTime ModifiedDate { get; set; }
        void UpdateModifiedDate();
        }
    }
