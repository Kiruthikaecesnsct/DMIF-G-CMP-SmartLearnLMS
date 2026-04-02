-- ============================================================
-- SmartLearnDB — Full Setup Script
-- Assignment 5: Part 2 & 3
-- ============================================================

-- STEP 1: Create the database
--CREATE DATABASE SmartLearn;
--GO
USE SmartLearn;
--GO

---- ============================================================
---- PART 2, TASK 2: TABLE CREATION
---- ============================================================

---- 1. Users Table
--CREATE TABLE Users (
--    UserId       INT           IDENTITY(1,1) PRIMARY KEY,
--    Username     NVARCHAR(50)  NOT NULL UNIQUE,
--    Email        NVARCHAR(100) NOT NULL UNIQUE,
--    PasswordHash NVARCHAR(255) NOT NULL,
--    UserType     NVARCHAR(20)  NOT NULL CHECK (UserType IN ('Student','Instructor','Admin')),
--    CreatedDate  DATETIME      NOT NULL DEFAULT GETDATE(),
--    LastLoginDate DATETIME     NULL
--);

---- 2. Courses Table
--CREATE TABLE Courses (
--    CourseId          INT           IDENTITY(1,1) PRIMARY KEY,
--    Title             NVARCHAR(100) NOT NULL,
--    Description       NVARCHAR(500) NULL,
--    Category          NVARCHAR(50)  NOT NULL,
--    DifficultyLevel   NVARCHAR(20)  NOT NULL DEFAULT 'Beginner'
--                                    CHECK (DifficultyLevel IN ('Beginner','Intermediate','Advanced')),
--    MaxCapacity       INT           NOT NULL DEFAULT 30,
--    CurrentEnrollments INT          NOT NULL DEFAULT 0,
--    InstructorId      INT           NOT NULL,
--    CreatedDate       DATETIME      NOT NULL DEFAULT GETDATE(),
--    CONSTRAINT FK_Courses_Instructor FOREIGN KEY (InstructorId) REFERENCES Users(UserId)
--);

---- 3. Enrollments Table (Many-to-Many: Students ↔ Courses)
--CREATE TABLE Enrollments (
--    EnrollmentId    INT          IDENTITY(1,1) PRIMARY KEY,
--    StudentId       INT          NOT NULL,
--    CourseId        INT          NOT NULL,
--    EnrolledDate    DATETIME     NOT NULL DEFAULT GETDATE(),
--    ProgressPercent INT          NOT NULL DEFAULT 0 CHECK (ProgressPercent BETWEEN 0 AND 100),
--    CompletionDate  DATETIME     NULL,
--    Status          NVARCHAR(20) NOT NULL DEFAULT 'Active'
--                                 CHECK (Status IN ('Active','Completed','Dropped')),
--    CONSTRAINT FK_Enrollments_Student FOREIGN KEY (StudentId) REFERENCES Users(UserId),
--    CONSTRAINT FK_Enrollments_Course  FOREIGN KEY (CourseId)  REFERENCES Courses(CourseId),
--    CONSTRAINT UQ_Student_Course      UNIQUE (StudentId, CourseId)  -- prevents double-enrollment
--);

---- 4. CourseRatings Table
--CREATE TABLE CourseRatings (
--    RatingId    INT           IDENTITY(1,1) PRIMARY KEY,
--    CourseId    INT           NOT NULL,
--    StudentId   INT           NOT NULL,
--    Rating      INT           NOT NULL CHECK (Rating BETWEEN 1 AND 5),
--    ReviewText  NVARCHAR(1000) NULL,
--    RatedDate   DATETIME      NOT NULL DEFAULT GETDATE(),
--    CONSTRAINT FK_Ratings_Course   FOREIGN KEY (CourseId)   REFERENCES Courses(CourseId),
--    CONSTRAINT FK_Ratings_Student  FOREIGN KEY (StudentId)  REFERENCES Users(UserId)
--);
--GO

---- ============================================================
---- PART 3, TASK 1: INSERT SAMPLE DATA
---- ============================================================

---- Insert Users (3 Instructors, 1 Admin, 10 Students = 14 users)
--INSERT INTO Users (Username, Email, PasswordHash, UserType) VALUES
--('prof_smith',   'smith@uni.edu',       'hash_smith',   'Instructor'),
--('prof_johnson', 'johnson@uni.edu',     'hash_johnson', 'Instructor'),
--('prof_garcia',  'garcia@uni.edu',      'hash_garcia',  'Instructor'),
--('admin1',       'admin@smartlearn.com','hash_admin',   'Admin'),
--('alice',        'alice@email.com',     'hash_alice',   'Student'),
--('bob',          'bob@email.com',       'hash_bob',     'Student'),
--('charlie',      'charlie@email.com',   'hash_charlie', 'Student'),
--('diana',        'diana@email.com',     'hash_diana',   'Student'),
--('eve',          'eve@email.com',       'hash_eve',     'Student'),
--('frank',        'frank@email.com',     'hash_frank',   'Student'),
--('grace',        'grace@email.com',     'hash_grace',   'Student'),
--('henry',        'henry@email.com',     'hash_henry',   'Student'),
--('iris',         'iris@email.com',      'hash_iris',    'Student'),
--('jack',         'jack@email.com',      'hash_jack',    'Student');

---- Insert Courses (15 courses, linked to instructor IDs 1-3)
--INSERT INTO Courses (Title, Description, Category, DifficultyLevel, MaxCapacity, CurrentEnrollments, InstructorId) VALUES
--('C# Fundamentals',          'Learn C# from scratch',            'Programming',      'Beginner',     30, 25, 1),
--('Advanced C#',              'LINQ, async, patterns',            'Programming',      'Advanced',     25, 18, 1),
--('Python for Beginners',     'Intro to Python',                  'Programming',      'Beginner',     30, 20, 2),
--('Data Science with Python', 'Pandas, NumPy, visualization',     'Data Science',     'Intermediate', 25, 15, 2),
--('Machine Learning Basics',  'Intro to ML concepts',             'Data Science',     'Intermediate', 20, 12, 2),
--('Web Dev Fundamentals',     'HTML, CSS, JavaScript',            'Web Development',  'Beginner',     35, 30, 3),
--('React & Node.js',          'Full-stack JavaScript',            'Web Development',  'Advanced',     25, 22, 3),
--('SQL Server Mastery',       'T-SQL and database design',        'Database',         'Intermediate', 20, 14, 1),
--('Data Structures',          'Arrays, Trees, Graphs',            'Computer Science', 'Intermediate', 30, 28, 1),
--('Algorithms',               'Sorting, searching, complexity',   'Computer Science', 'Advanced',     25, 16, 1),
--('Cloud Computing AWS',      'AWS core services',                'Cloud',            'Intermediate', 20, 10, 2),
--('DevOps Basics',            'CI/CD, Docker, pipelines',         'DevOps',           'Intermediate', 20,  8, 3),
--('Mobile Dev Android',       'Kotlin Android development',       'Mobile',           'Intermediate', 15,  6, 3),
--('Cybersecurity Essentials', 'Network security basics',          'Security',         'Beginner',     25, 19, 2),
--('UX Design Principles',     'User experience fundamentals',     'Design',           'Beginner',     30, 11, 3);

---- Insert Enrollments (30+ student-course relationships)
---- Students have UserId 5-14; Courses have CourseId 1-15
--INSERT INTO Enrollments (StudentId, CourseId, ProgressPercent, Status) VALUES
--(5,  1, 85, 'Active'),    -- alice   → C# Fundamentals
--(5,  6, 100,'Completed'), -- alice   → Web Dev (completed)
--(5,  9, 60, 'Active'),    -- alice   → Data Structures
--(6,  1, 45, 'Active'),    -- bob     → C# Fundamentals
--(6,  3, 30, 'Active'),    -- bob     → Python
--(7,  1, 92, 'Active'),    -- charlie → C# Fundamentals
--(7,  2, 75, 'Active'),    -- charlie → Advanced C#
--(7,  9, 88, 'Active'),    -- charlie → Data Structures
--(8,  4, 78, 'Active'),    -- diana   → Data Science
--(8,  5, 65, 'Active'),    -- diana   → ML Basics
--(8,  6, 100,'Completed'), -- diana   → Web Dev (completed)
--(9,  3, 30, 'Active'),    -- eve     → Python
--(9,  6, 20, 'Active'),    -- eve     → Web Dev
--(10, 7, 55, 'Active'),    -- frank   → React & Node
--(10, 8, 70, 'Active'),    -- frank   → SQL Server
--(10, 11,40, 'Active'),    -- frank   → Cloud
--(11, 1, 95, 'Active'),    -- grace   → C# Fundamentals
--(11, 2, 80, 'Active'),    -- grace   → Advanced C#
--(11, 8, 90, 'Active'),    -- grace   → SQL Server
--(11, 9, 85, 'Active'),    -- grace   → Data Structures
--(12, 3, 50, 'Active'),    -- henry   → Python
--(12, 4, 45, 'Active'),    -- henry   → Data Science
--(12, 14,60, 'Active'),    -- henry   → Cybersecurity
--(13, 6, 100,'Completed'), -- iris    → Web Dev (completed)
--(13, 7, 88, 'Active'),    -- iris    → React & Node
--(13, 12,35, 'Active'),    -- iris    → DevOps
--(14, 9, 72, 'Active'),    -- jack    → Data Structures
--(14, 10,55, 'Active'),    -- jack    → Algorithms
--(14, 13,25, 'Active'),    -- jack    → Mobile Dev
--(5,  4, 50, 'Active'),    -- alice   → Data Science
--(6,  14,15, 'Active'),    -- bob     → Cybersecurity
--(7,  6, 100,'Completed'); -- charlie → Web Dev (completed)

---- Insert CourseRatings (20+ ratings)
--INSERT INTO CourseRatings (CourseId, StudentId, Rating, ReviewText) VALUES
--(1, 5,  5, 'Excellent course! Very clear explanations.'),
--(1, 7,  5, 'Best C# course I have taken.'),
--(1, 11, 4, 'Great content, could use more exercises.'),
--(1, 6,  3, 'Good but moves a bit fast.'),
--(6, 5,  5, 'Perfect introduction to web development.'),
--(6, 8,  5, 'Loved every lesson, very practical.'),
--(6, 13, 4, 'Solid course with good projects.'),
--(9, 7,  5, 'Data structures explained brilliantly.'),
--(9, 11, 5, 'Very thorough and well-paced.'),
--(9, 14, 4, 'Good depth, challenging assignments.'),
--(4, 8,  4, 'Great intro to data science.'),
--(4, 12, 3, 'Decent, but needs more real datasets.'),
--(5, 8,  4, 'Good ML fundamentals coverage.'),
--(7, 10, 5, 'Full-stack in one course — amazing!'),
--(7, 13, 5, 'Hands-on and relevant to industry.'),
--(8, 10, 4, 'SQL explained very well.'),
--(8, 11, 5, 'Best database course available.'),
--(2, 7,  4, 'Advanced topics well explained.'),
--(2, 11, 5, 'LINQ section was outstanding.'),
--(14,12, 4, 'Good security foundations.'),
--(11,10, 3, 'Cloud content is a bit outdated.');
--GO

-- ============================================================
-- PART 3, TASK 2 & 3: SELECT QUERIES + JOINS + AGGREGATES
-- ============================================================

-- ── Query 1: All students enrolled in a specific course (C# Fundamentals = CourseId 1)
SELECT
    u.Username,
    u.Email,
    e.ProgressPercent,
    e.EnrolledDate,
    e.Status
FROM Enrollments e
INNER JOIN Users u ON e.StudentId = u.UserId
WHERE e.CourseId = 1
ORDER BY e.ProgressPercent DESC;

---- ── Query 2: All courses by a specific instructor
--SELECT
--    c.Title,
--    c.Category,
--    c.CurrentEnrollments,
--    c.DifficultyLevel
--FROM Courses c
--INNER JOIN Users u ON c.InstructorId = u.UserId
--WHERE u.Username = 'prof_smith'
--ORDER BY c.Title;

---- ── Query 3: Top 10 students by average progress
--SELECT TOP 10
--    u.Username,
--    u.Email,
--    AVG(e.ProgressPercent) AS AverageProgress,
--    COUNT(e.EnrollmentId)  AS CoursesEnrolled
--FROM Enrollments e
--INNER JOIN Users u ON e.StudentId = u.UserId
--GROUP BY u.UserId, u.Username, u.Email
--ORDER BY AverageProgress DESC;

---- ── Query 4: Courses with average rating > 4.0
--SELECT
--    c.Title,
--    c.Category,
--    ROUND(AVG(CAST(r.Rating AS FLOAT)), 2) AS AvgRating,
--    COUNT(r.RatingId) AS TotalRatings
--FROM CourseRatings r
--INNER JOIN Courses c ON r.CourseId = c.CourseId
--GROUP BY c.CourseId, c.Title, c.Category
--HAVING AVG(CAST(r.Rating AS FLOAT)) > 4.0
--ORDER BY AvgRating DESC;

---- ── Query 5: Enrollment statistics by category
--SELECT
--    c.Category,
--    COUNT(DISTINCT c.CourseId)     AS TotalCourses,
--    SUM(c.CurrentEnrollments)      AS TotalEnrollments,
--    AVG(c.CurrentEnrollments)      AS AvgEnrollmentsPerCourse,
--    MAX(c.CurrentEnrollments)      AS MaxEnrollments,
--    MIN(c.CurrentEnrollments)      AS MinEnrollments
--FROM Courses c
--GROUP BY c.Category
--ORDER BY TotalEnrollments DESC;

---- ── Query 6: Students who completed at least 3 courses
--SELECT
--    u.Username,
--    u.Email,
--    COUNT(e.EnrollmentId) AS CompletedCourses
--FROM Enrollments e
--INNER JOIN Users u ON e.StudentId = u.UserId
--WHERE e.Status = 'Completed'
--GROUP BY u.UserId, u.Username, u.Email
--HAVING COUNT(e.EnrollmentId) >= 3;

---- ── Query 7: Monthly enrollment trends
--SELECT
--    YEAR(e.EnrolledDate)  AS EnrollYear,
--    MONTH(e.EnrolledDate) AS EnrollMonth,
--    DATENAME(MONTH, e.EnrolledDate) AS MonthName,
--    COUNT(e.EnrollmentId) AS TotalEnrollments
--FROM Enrollments e
--GROUP BY YEAR(e.EnrolledDate), MONTH(e.EnrolledDate), DATENAME(MONTH, e.EnrolledDate)
--ORDER BY EnrollYear, EnrollMonth;

---- ── Query 8: Courses with low enrollment (fewer than 5)
--SELECT
--    c.CourseId,
--    c.Title,
--    c.Category,
--    c.CurrentEnrollments,
--    c.MaxCapacity,
--    u.Username AS InstructorName
--FROM Courses c
--INNER JOIN Users u ON c.InstructorId = u.UserId
--WHERE c.CurrentEnrollments < 5
--ORDER BY c.CurrentEnrollments ASC;

---- ============================================================
---- PART 3, TASK 3: JOIN OPERATIONS
---- ============================================================

---- INNER JOIN: Students with their enrolled courses
--SELECT
--    u.Username AS Student,
--    c.Title    AS Course,
--    e.ProgressPercent,
--    e.Status
--FROM Enrollments e
--INNER JOIN Users   u ON e.StudentId = u.UserId
--INNER JOIN Courses c ON e.CourseId  = c.CourseId
--ORDER BY u.Username, c.Title;

---- INNER JOIN: Courses with their instructors
--SELECT
--    c.Title      AS CourseTitle,
--    c.Category,
--    u.Username   AS Instructor,
--    u.Email      AS InstructorEmail
--FROM Courses c
--INNER JOIN Users u ON c.InstructorId = u.UserId
--ORDER BY c.Title;

---- Multiple JOINs: Students → Enrollments → Courses → Instructors (4 tables)
--SELECT
--    s.Username   AS Student,
--    c.Title      AS Course,
--    i.Username   AS Instructor,
--    e.ProgressPercent,
--    r.Rating,
--    r.ReviewText
--FROM Enrollments e
--INNER JOIN Users   s ON e.StudentId    = s.UserId
--INNER JOIN Courses c ON e.CourseId     = c.CourseId
--INNER JOIN Users   i ON c.InstructorId = i.UserId
--LEFT  JOIN CourseRatings r ON r.CourseId = c.CourseId AND r.StudentId = s.UserId
--ORDER BY s.Username, c.Title;

---- LEFT JOIN: All courses with their ratings (includes courses with NO ratings)
--SELECT
--    c.Title,
--    c.Category,
--    COALESCE(CAST(r.Rating AS NVARCHAR), 'No rating') AS Rating,
--    COALESCE(r.ReviewText, 'No review') AS Review
--FROM Courses c
--LEFT JOIN CourseRatings r ON c.CourseId = r.CourseId
--ORDER BY c.Title;

---- JOIN with WHERE: Active enrollments in Programming courses only
--SELECT
--    u.Username,
--    c.Title,
--    e.ProgressPercent
--FROM Enrollments e
--INNER JOIN Users   u ON e.StudentId = u.UserId
--INNER JOIN Courses c ON e.CourseId  = c.CourseId
--WHERE c.Category = 'Programming'
--  AND e.Status   = 'Active'
--ORDER BY e.ProgressPercent DESC;

---- ============================================================
---- PART 3, TASK 4: AGGREGATE FUNCTIONS
---- ============================================================

---- COUNT: Total records per table
--SELECT 'Users'       AS TableName, COUNT(*) AS TotalRows FROM Users
--UNION ALL
--SELECT 'Courses',    COUNT(*) FROM Courses
--UNION ALL
--SELECT 'Enrollments',COUNT(*) FROM Enrollments
--UNION ALL
--SELECT 'Ratings',    COUNT(*) FROM CourseRatings;

---- AVG: Average rating per course
--SELECT c.Title, ROUND(AVG(CAST(r.Rating AS FLOAT)), 2) AS AvgRating
--FROM CourseRatings r
--INNER JOIN Courses c ON r.CourseId = c.CourseId
--GROUP BY c.CourseId, c.Title
--ORDER BY AvgRating DESC;

---- SUM: Total enrollments per instructor
--SELECT u.Username AS Instructor, SUM(c.CurrentEnrollments) AS TotalStudents
--FROM Courses c
--INNER JOIN Users u ON c.InstructorId = u.UserId
--GROUP BY u.UserId, u.Username
--ORDER BY TotalStudents DESC;

---- MIN and MAX: Progress range per course
--SELECT
--    c.Title,
--    MIN(e.ProgressPercent) AS LowestProgress,
--    MAX(e.ProgressPercent) AS HighestProgress,
--    AVG(e.ProgressPercent) AS AvgProgress
--FROM Enrollments e
--INNER JOIN Courses c ON e.CourseId = c.CourseId
--GROUP BY c.CourseId, c.Title
--ORDER BY AvgProgress DESC;

---- GROUP BY + HAVING: Categories with more than 2 courses
--SELECT
--    c.Category,
--    COUNT(c.CourseId) AS CourseCount,
--    SUM(c.CurrentEnrollments) AS TotalEnrollments
--FROM Courses c
--GROUP BY c.Category
--HAVING COUNT(c.CourseId) > 2
--ORDER BY TotalEnrollments DESC;

---- LIKE: Search courses by keyword
--SELECT Title, Category, DifficultyLevel
--FROM Courses
--WHERE Title LIKE '%python%'
--   OR Category LIKE '%science%';

---- ORDER BY: Top 5 most popular courses
--SELECT TOP 5
--    Title,
--    Category,
--    CurrentEnrollments,
--    MaxCapacity
--FROM Courses
--ORDER BY CurrentEnrollments DESC;
--GO
