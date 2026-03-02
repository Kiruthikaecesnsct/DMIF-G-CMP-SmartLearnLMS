using System;
using System.Collections.Generic;

namespace Week_1
{
    public static class SearchEngine
    {
        public static List<ISearchable> Search(List<ISearchable> items, string keyword)
        {
            List<ISearchable> results = new List<ISearchable>();
            foreach (ISearchable item in items)
            {
                if (item.MatchesSearch(keyword))
                    results.Add(item);
            }
            return results;
        }

        public static void DisplayResults(List<ISearchable> results)
        {
            if (results.Count == 0)
            {
                Console.WriteLine("  No results found.");
                return;
            }
            Console.WriteLine($"╔════════════════════════════════╗");
            Console.WriteLine($"║  Found {results.Count} result(s)               ║");
            Console.WriteLine($"╚════════════════════════════════╝");
            foreach (ISearchable item in results)
                Console.WriteLine($"  • {item.GetSearchSummary()}");
        }
    }
}