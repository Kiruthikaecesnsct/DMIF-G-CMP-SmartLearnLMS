using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Week_1.Data;

namespace Week_1.Services
    {
    public class CourseService
        {
        // OLD WAY — reading from a JSON file (delete this!)
        // string json = File.ReadAllText("courses.json");
        // var courses = JsonSerializer.Deserialize<List<Course>>(json);

        // NEW WAY — reading from the database
        public List<Course> GetAllCourses()
            {
            using (var db = new SmartLearnDbContext())
                {
                return db.Courses.ToList();
                }
            }

        // Search courses using LINQ + EF Core — replaces your SearchService JSON logic
        public List<Course> SearchCourses(string keyword)
            {
            using (var db = new SmartLearnDbContext())
                {
                // EF Core converts this LINQ into a SQL WHERE clause
                // The search happens IN the database — much faster than loading all then filtering
                return db.Courses
                    .Where(c => c.Title.Contains(keyword) || c.Description.Contains(keyword))
                    .ToList();
                }
            }

        // Get courses sorted and filtered — replaces your LINQ on in-memory lists
        public List<Course> GetCoursesByCategory(string category)
            {
            using (var db = new SmartLearnDbContext())
                {
                return db.Courses
                    .Where(c => c.Category == category)
                    .OrderBy(c => c.Title) // EF Core translates this to ORDER BY
                    .ToList();
                }
            }

        // Complex query: find the most popular courses
        public List<Course> GetMostPopularCourses(int topN)
            {
            using (var db = new SmartLearnDbContext())
                {
                return db.Courses
                    .OrderByDescending(c => c.CurrentEnrollments)
                    .Take(topN) // EF Core translates this to TOP N in SQL
                    .ToList();
                }
            }

        // Pagination — replaces your Skip/Take on in-memory lists
        // Now Skip/Take happens in the database — only fetches what you need
        public List<Course> GetCoursesPaged(int pageNumber, int pageSize)
            {
            using (var db = new SmartLearnDbContext())
                {
                return db.Courses
                    .OrderBy(c => c.Title)
                    .Skip((pageNumber - 1) * pageSize) // EF Core → OFFSET in SQL
                    .Take(pageSize)                    // EF Core → FETCH NEXT in SQL
                    .ToList();
                }
            }
        }
    }
