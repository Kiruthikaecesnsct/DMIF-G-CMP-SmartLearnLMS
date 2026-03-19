--SELECT TABLE_NAME, COLUMN_NAME
--FROM INFORMATION_SCHEMA.COLUMNS
--WHERE TABLE_NAME IN ('Students', 'Enrollments', 'Courses', 'Instructors')
--ORDER BY TABLE_NAME, COLUMN_NAME;


-- ?? Check all tables ?????????????????????????????????????????????????????????
--SELECT * FROM Students;
--SELECT * FROM Courses;
--SELECT * FROM Enrollments;
--SELECT * FROM Instructors;
--SELECT * FROM __EFMigrationsHistory;

-- ?? WEEK 7: JOIN queries ??????????????????????????????????????????????????????

-- Students + their enrolled courses (INNER JOIN)
--SELECT s.Username, c.Title, e.ProgressPercent, e.Status
--FROM Enrollments e
--INNER JOIN Students s ON e.StudentId = s.StudentId
--INNER JOIN Courses  c ON e.CourseId  = c.CourseId
--ORDER BY s.Username;

-- All students including those not enrolled (LEFT JOIN)
--SELECT s.Username, c.Title, e.ProgressPercent
--FROM Students s
--LEFT JOIN Enrollments e ON s.StudentId = e.StudentId
--LEFT JOIN Courses     c ON e.CourseId  = c.CourseId
--ORDER BY s.Username;

-- ?? GROUP BY + Aggregates ?????????????????????????????????????????????????????

-- Students per course
--SELECT c.Title, c.Category, COUNT(e.EnrollmentId) AS TotalStudents
--FROM Courses c
--LEFT JOIN Enrollments e ON c.CourseId = e.CourseId
--GROUP BY c.CourseId, c.Title, c.Category
--ORDER BY TotalStudents DESC;

-- Average, min, max progress per course
--SELECT c.Title,
--       COUNT(e.EnrollmentId)  AS TotalEnrollments,
--       AVG(e.ProgressPercent) AS AverageProgress,
--       MIN(e.ProgressPercent) AS LowestProgress,
--       MAX(e.ProgressPercent) AS HighestProgress
--FROM Courses c
--INNER JOIN Enrollments e ON c.CourseId = e.CourseId
--GROUP BY c.CourseId, c.Title
--ORDER BY AverageProgress DESC;

-- Students enrolled in multiple courses (HAVING)
--SELECT s.Username,
--       COUNT(e.CourseId)      AS NumberOfCourses,
--       AVG(e.ProgressPercent) AS OverallProgress
--FROM Students s
--INNER JOIN Enrollments e ON s.StudentId = e.StudentId
--GROUP BY s.StudentId, s.Username
--HAVING COUNT(e.CourseId) > 1
--ORDER BY NumberOfCourses DESC;

-- Top 5 most popular courses
--SELECT TOP 5 c.Title, c.Category,
--       COUNT(e.EnrollmentId)  AS TotalEnrollments,
--       AVG(e.ProgressPercent) AS AvgProgress
--FROM Courses c
--INNER JOIN Enrollments e ON c.CourseId = e.CourseId
--GROUP BY c.CourseId, c.Title, c.Category
--ORDER BY TotalEnrollments DESC;

-- Monthly enrollment trends
--SELECT YEAR(e.EnrolledDate)  AS [Year],
--       MONTH(e.EnrolledDate) AS [Month],
--       COUNT(e.EnrollmentId) AS NewEnrollments
--FROM Enrollments e
--GROUP BY YEAR(e.EnrolledDate), MONTH(e.EnrolledDate)
--ORDER BY [Year] DESC, [Month] DESC;

-- Category performance
--SELECT c.Category,
--       COUNT(DISTINCT e.StudentId) AS TotalStudents,
--       AVG(e.ProgressPercent)      AS AvgProgress
--FROM Courses c
--INNER JOIN Enrollments e ON c.CourseId = e.CourseId
--GROUP BY c.Category
--ORDER BY TotalStudents DESC;

-- Students with completed courses
--SELECT s.Username,
--       COUNT(CASE WHEN e.Status = 'Completed' THEN 1 END) AS CompletedCourses,
--       COUNT(e.EnrollmentId)                               AS TotalEnrollments,
--       AVG(e.ProgressPercent)                              AS AvgProgress
--FROM Students s
--INNER JOIN Enrollments e ON s.StudentId = e.StudentId
--GROUP BY s.StudentId, s.Username
--HAVING COUNT(CASE WHEN e.Status = 'Completed' THEN 1 END) > 0
--ORDER BY CompletedCourses DESC;

-- Courses with low enrollment (fewer than 3 students)
--SELECT c.Title, c.Category, c.Difficulty,
--       COUNT(e.EnrollmentId) AS StudentCount
--FROM Courses c
--LEFT JOIN Enrollments e ON c.CourseId = e.CourseId
--GROUP BY c.CourseId, c.Title, c.Category, c.Difficulty
--HAVING COUNT(e.EnrollmentId) < 3
--ORDER BY StudentCount;

--DELETE FROM __EFMigrationsHistory;

--INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion) VALUES ('20260306120422_InitialCreate', '8.0.0');
--INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion) VALUES ('20260306120730_AddLastLoginDate', '8.0.0');
--INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion) VALUES ('20260309060032_SeedData', '8.0.0');
--INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion) VALUES ('20260309060629_FixNavigationProperties', '8.0.0');

--SELECT TABLE_NAME 
--FROM INFORMATION_SCHEMA.TABLES 
--WHERE TABLE_TYPE = 'BASE TABLE'
--ORDER BY TABLE_NAME;

--SELECT CourseId, Title, InstructorId, MaxStudents, MaxCapacity, CurrentEnrollments
--FROM Courses
--WHERE InstructorId IS NULL 
--   OR MaxStudents IS NULL 
--   OR MaxCapacity IS NULL
--   OR CurrentEnrollments IS NULL;

SELECT UserId, Username FROM Users;
UPDATE Courses SET InstructorId = 1 WHERE InstructorId IS NULL;

--SELECT 
--    fk.name AS ForeignKeyName,
--    COL_NAME(fkc.parent_object_id, fkc.parent_column_id) AS ColumnName,
--    OBJECT_NAME(fkc.referenced_object_id) AS ReferencedTable,
--    COL_NAME(fkc.referenced_object_id, fkc.referenced_column_id) AS ReferencedColumnName
--FROM sys.foreign_keys fk
--INNER JOIN sys.foreign_key_columns fkc 
--    ON fk.object_id = fkc.constraint_object_id
--WHERE OBJECT_NAME(fk.parent_object_id) = 'Courses';

--UPDATE Courses SET InstructorId = 1 WHERE InstructorId IS NULL;



