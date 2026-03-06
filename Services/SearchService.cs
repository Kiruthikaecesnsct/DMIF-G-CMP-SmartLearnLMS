using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Week_1.Services
    {
    /// <summary>
    /// Single Responsibility: handles all search logic.
    /// Generic — works with any ISearchable collection.
    /// </summary>
    public class SearchService
        {
        // Generic search — works on ANY ISearchable list
        public List<T> Search<T>(IEnumerable<T> items, string keyword) where T : ISearchable
            {
            if (string.IsNullOrWhiteSpace(keyword)) return items.ToList();
            return items
                .Where(item => item.MatchesSearch(keyword))
                .ToList();
            }

        public void DisplayResults<T>(List<T> results, string keyword) where T : ISearchable
            {
            Console.WriteLine($"\n🔍 Search results for '{keyword}': {results.Count} found");
            if (results.Count == 0)
                {
                Console.WriteLine("  No results found.");
                return;
                }
            foreach (var item in results)
                Console.WriteLine($"  • {item.GetSearchSummary()}");
            }
        }
    }
