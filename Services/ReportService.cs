using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Week_1.Services
    {
    /// <summary>
    /// Single Responsibility: handles all report generation and display.
    /// Works against IReportable — not tied to any specific class.
    /// </summary>
    public class ReportService
        {
        public string GenerateReport(IReportable reportable)
            {
            return reportable.GenerateReport();
            }

        public void DisplayReport(IReportable reportable)
            {
            Console.WriteLine(reportable.GenerateReport());
            }

        public void DisplayAllReports(IEnumerable<IReportable> reportables)
            {
            foreach (var r in reportables)
                {
                Console.WriteLine(r.GenerateReport());
                Console.WriteLine();
                }
            }

        public void SaveReport(IReportable reportable, string filePath)
            {
            try
                {
                File.WriteAllText(filePath, reportable.GenerateReport());
                Console.WriteLine($"  ✓ Report saved to {filePath}");
                }
            catch (Exception ex)
                {
                Console.WriteLine($"  ✗ Failed to save report: {ex.Message}");
                }
            }
        }
    }
