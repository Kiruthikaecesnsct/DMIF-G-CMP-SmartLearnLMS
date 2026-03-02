using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Week_1
{
    public interface IRatable
    {
        void AddRating(int stars, string review);
        double GetAverageRating();
        int GetTotalRatings();
    }
}
