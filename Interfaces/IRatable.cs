using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Week_1.Interfaces
    {
    public interface IRatable
        {
        double AverageRating { get; }
        void AddRating(int rating, string review);
        List<(int rating, string review)> GetReviews();
        }
    }
