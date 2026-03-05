using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Week_1.Interfaces;

namespace Week_1.Services
    {
    public class SearchService
        {
        public List<T> Search<T>(List<T> items, string keyword) where T : ISearchable
            {
            return items
                .Where(item => item.MatchesSearch(keyword))
                .ToList();
            }
        }
    }
