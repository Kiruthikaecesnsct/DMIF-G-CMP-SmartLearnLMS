using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Week_1.Interfaces
{
    public interface ISearchable
    {
        bool MatchesSearch(string keyword);
        string GetSearchSummary();
    }

}
