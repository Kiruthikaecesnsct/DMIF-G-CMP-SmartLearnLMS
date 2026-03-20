-- ============================================================
-- SmartLearn LMS — Week 7 SQL Queries
-- Assignment 7 — Part 2: Advanced SQL Queries
-- ============================================================

USE SmartLearnDB;
GO

-- ============================================================
-- TASK 1: STUDENT PERFORMANCE QUERIES
-- ============================================================

-- ── Query 1: High-Performing Students
-- Find all students with overall completion rate > 75%
-- Shows: student name, total enrollments, completed courses, completion rate
SELECT
    u.Username                                              AS StudentName,
    COUNT(e.EnrollmentId)                                   AS TotalEnrollments,
    SUM(CASE WHEN e.Status = 'Completed' THEN 1 ELSE 0 END) AS CompletedCourses,
    ROUND(
        CAST(SUM(CASE WHEN e.Status = 'Completed' THEN 1 ELSE 0 END) AS FLOAT)
        / COUNT(e.EnrollmentId) * 100, 2
    )                                                       AS CompletionRate
FROM Enrollments e
INNER JOIN Users u ON e.StudentId = u.UserId
WHERE u.UserType = 'Student'
GROUP BY u.UserId, u.Username
HAVING
    ROUND(
        CAST(SUM(CASE WHEN e.Status = 'Completed' THEN 1 ELSE 0 END) AS FLOAT)
        / COUNT(e.EnrollmentId) * 100, 2
    ) > 75
ORDER BY CompletionRate DESC;
GO

-- ── Query 2: Students Who Completed ALL Enrolled Courses
-- Every single enrollment for these students has Status = 'Completed'
-- Shows: student name, total completed courses, average progress
SELECT
    u.Username                      AS StudentName,
    COUNT(e.EnrollmentId)           AS TotalCoursesCompleted,
    AVG(e.ProgressPercent)          AS AverageProgress
FROM Enrollments e
INNER JOIN Users u ON e.StudentId = u.UserId
WHERE u.UserType = 'Student'
GROUP BY u.UserId, u.Username
HAVING MIN(CASE WHEN e.Status = 'Completed' THEN 1 ELSE 0 END) = 1
   -- MIN = 1 means every row was 'Completed'
ORDER BY TotalCoursesCompleted DESC;
GO

-- ── Query 3: Top 5 Students by Completed Courses
-- Ranks students by number of completed courses; includes average rating given
SELECT TOP 5
    ROW_NUMBER() OVER (ORDER BY COUNT(CASE WHEN e.Status = 'Completed' THEN 1 END) DESC) AS Rank,
    u.Username                                                AS StudentName,
    COUNT(CASE WHEN e.Status = 'Completed' THEN 1 END)       AS CompletedCourses,
    ROUND(AVG(CAST(cr.Rating AS FLOAT)), 2)                   AS AvgRatingGiven
FROM Enrollments e
INNER JOIN Users u ON e.StudentId = u.UserId
LEFT  JOIN CourseRatings cr ON cr.StudentId = u.UserId
WHERE u.UserType = 'Student'
GROUP BY u.UserId, u.Username
ORDER BY CompletedCourses DESC;
GO

-- ============================================================
-- TASK 2: INSTRUCTOR ANALYTICS QUERIES
-- ============================================================

-- ── Query 4: Instructors Ranked by Total Unique Student Count
-- Shows: instructor name, total courses taught, total unique students, avg course rating
SELECT
    u.Username                                  AS InstructorName,
    COUNT(DISTINCT c.CourseId)                  AS TotalCoursesTaught,
    COUNT(DISTINCT e.StudentId)                 AS TotalUniqueStudents,
    ROUND(AVG(CAST(cr.Rating AS FLOAT)), 2)     AS AvgCourseRating
FROM Users u
INNER JOIN Courses     c  ON c.InstructorId = u.UserId
LEFT  JOIN Enrollments e  ON e.CourseId     = c.CourseId
LEFT  JOIN CourseRatings cr ON cr.CourseId  = c.CourseId
WHERE u.UserType = 'Instructor'
GROUP BY u.UserId, u.Username
ORDER BY TotalUniqueStudents DESC;
GO

-- ── Query 5: Instructor Performance by Category
-- Shows: instructor name, category, number of courses, avg student progress, avg rating
SELECT
    u.Username                              AS InstructorName,
    c.Category,
    COUNT(DISTINCT c.CourseId)              AS NumberOfCourses,
    ROUND(AVG(CAST(e.ProgressPercent AS FLOAT)), 2) AS AvgStudentProgress,
    ROUND(AVG(CAST(cr.Rating AS FLOAT)), 2) AS AvgRating
FROM Users u
INNER JOIN Courses       c  ON c.InstructorId = u.UserId
LEFT  JOIN Enrollments   e  ON e.CourseId     = c.CourseId
LEFT  JOIN CourseRatings cr ON cr.CourseId    = c.CourseId
WHERE u.UserType = 'Instructor'
GROUP BY u.UserId, u.Username, c.Category
ORDER BY u.Username, c.Category;
GO

-- ============================================================
-- TASK 3: COURSE ANALYTICS QUERIES
-- ============================================================

-- ── Query 6: Highly-Rated Courses (avg rating > 4.0)
-- LEFT JOIN ensures courses with no ratings still appear (with NULL avg)
-- Shows: course title, instructor name, avg rating, total ratings, enrollment count
SELECT
    c.Title                                       AS CourseTitle,
    u.Username                                    AS InstructorName,
    ROUND(AVG(CAST(cr.Rating AS FLOAT)), 2)       AS AvgRating,
    COUNT(cr.RatingId)                            AS TotalRatings,
    c.CurrentEnrollments                          AS EnrollmentCount
FROM Courses c
LEFT  JOIN CourseRatings cr ON cr.CourseId    = c.CourseId
INNER JOIN Users         u  ON c.InstructorId = u.UserId
GROUP BY c.CourseId, c.Title, u.Username, c.CurrentEnrollments
HAVING AVG(CAST(cr.Rating AS FLOAT)) > 4.0
ORDER BY AvgRating DESC;
GO

-- ── Query 7: Courses with Low Enrollment (no new enrollment in last 30 days)
-- Shows: course title, category, last enrollment date, current student count
SELECT
    c.Title                         AS CourseTitle,
    c.Category,
    MAX(e.EnrolledDate)             AS LastEnrollmentDate,
    c.CurrentEnrollments            AS CurrentStudentCount
FROM Courses c
LEFT JOIN Enrollments e ON e.CourseId = c.CourseId
GROUP BY c.CourseId, c.Title, c.Category, c.CurrentEnrollments
HAVING MAX(e.EnrolledDate) < DATEADD(DAY, -30, GETDATE())
    OR MAX(e.EnrolledDate) IS NULL
ORDER BY LastEnrollmentDate ASC;
GO

-- ── Query 8: Category Performance Analysis
-- Shows: category, total courses, total enrollments, avg progress, avg rating
-- Ordered by total enrollments descending
SELECT
    c.Category,
    COUNT(DISTINCT c.CourseId)                      AS TotalCourses,
    SUM(c.CurrentEnrollments)                       AS TotalEnrollments,
    ROUND(AVG(CAST(e.ProgressPercent AS FLOAT)), 2) AS AvgProgress,
    ROUND(AVG(CAST(cr.Rating AS FLOAT)), 2)         AS AvgRating
FROM Courses c
LEFT JOIN Enrollments   e  ON e.CourseId  = c.CourseId
LEFT JOIN CourseRatings cr ON cr.CourseId = c.CourseId
GROUP BY c.Category
ORDER BY TotalEnrollments DESC;
GO

-- ============================================================
-- TASK 4: TREND ANALYSIS QUERIES
-- ============================================================

-- ── Query 9: Monthly Enrollment Trends
-- Shows enrollment counts grouped by year and month
SELECT
    YEAR(e.EnrolledDate)                         AS EnrollYear,
    MONTH(e.EnrolledDate)                        AS EnrollMonth,
    DATENAME(MONTH, e.EnrolledDate)              AS MonthName,
    COUNT(e.EnrollmentId)                        AS TotalNewEnrollments,
    COUNT(DISTINCT e.StudentId)                  AS UniqueStudentsEnrolled
FROM Enrollments e
GROUP BY
    YEAR(e.EnrolledDate),
    MONTH(e.EnrolledDate),
    DATENAME(MONTH, e.EnrolledDate)
ORDER BY EnrollYear ASC, EnrollMonth ASC;
GO

-- ── Query 10: Course Completion Rate Over Time
-- Uses CASE WHEN for conditional counting
-- Shows: course title, total enrollments, completed enrollments, completion %
SELECT
    c.Title                                               AS CourseTitle,
    COUNT(e.EnrollmentId)                                 AS TotalEnrollments,
    SUM(CASE WHEN e.Status = 'Completed' THEN 1 ELSE 0 END) AS CompletedEnrollments,
    ROUND(
        CAST(SUM(CASE WHEN e.Status = 'Completed' THEN 1 ELSE 0 END) AS FLOAT)
        / NULLIF(COUNT(e.EnrollmentId), 0) * 100, 2
    )                                                     AS CompletionPercentage
FROM Courses c
LEFT JOIN Enrollments e ON e.CourseId = c.CourseId
GROUP BY c.CourseId, c.Title
ORDER BY CompletionPercentage ASC;  -- Low completion first = problematic courses
GO
---------------------------------------------------

USE SmartLearnDB;

---- Tell EF that InitialCreate was already applied
--INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
--VALUES ('20260313110546_InitialCreate', '8.0.0');


--INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
--VALUES ('20260320110723_AddCourseHierarchyAndRatings', '8.0.0');
-------------------------------------------------------------------------------------------------
-- Should show all applied migrations
--SELECT * FROM [__EFMigrationsHistory];

---- Should show all 6 tables exist
--SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES 
--WHERE TABLE_TYPE = 'BASE TABLE'
--ORDER BY TABLE_NAME;

-------------------------------------------------------
--EXEC sp_rename 'CourseRatings.Review', 'ReviewText', 'COLUMN';
---------------------------------------------------------
--ALTER TABLE Courses 
--DROP COLUMN CourseType;
--------------------------------------------------------------
USE SmartLearnDB;

-- Create CourseModules table
CREATE TABLE CourseModules (
    ModuleId      INT           IDENTITY(1,1) PRIMARY KEY,
    CourseId      INT           NOT NULL,
    Title         NVARCHAR(150) NOT NULL,
    Description   NVARCHAR(500) NULL,
    OrderIndex    INT           NOT NULL DEFAULT 1,
    DurationHours DECIMAL(5,2)  NOT NULL DEFAULT 0,
    CONSTRAINT FK_CourseModules_Courses_CourseId 
        FOREIGN KEY (CourseId) REFERENCES Courses(CourseId) ON DELETE CASCADE
);

CREATE INDEX IX_CourseModules_CourseId ON CourseModules(CourseId);

-- Create Lessons table
CREATE TABLE Lessons (
    LessonId        INT            IDENTITY(1,1) PRIMARY KEY,
    ModuleId        INT            NOT NULL,
    Title           NVARCHAR(150)  NOT NULL,
    Content         NVARCHAR(2000) NULL,
    VideoUrl        NVARCHAR(500)  NULL,
    OrderIndex      INT            NOT NULL DEFAULT 1,
    DurationMinutes INT            NOT NULL DEFAULT 0,
    CONSTRAINT FK_Lessons_CourseModules_ModuleId 
        FOREIGN KEY (ModuleId) REFERENCES CourseModules(ModuleId) ON DELETE CASCADE
);

CREATE INDEX IX_Lessons_ModuleId ON Lessons(ModuleId);