# SmartLearnDB — Relationship Analysis (Assignment 5, Part 4)

## Task 1: Relationship Types

### One-to-Many
A single record in one table relates to many records in another.
**Example in SmartLearnDB:** One `Instructor` (Users table) → many `Courses`.
One instructor like prof_smith teaches multiple courses, but each course
has exactly one instructor. The foreign key `InstructorId` lives in the
`Courses` table, pointing back to `Users`.

### Many-to-Many
Many records in one table relate to many records in another.
**Example in SmartLearnDB:** `Students` ↔ `Courses`.
One student can enroll in many courses; one course can have many students.
This cannot be represented with a single foreign key — it requires a
**junction table** (`Enrollments`) that holds a row for each unique
student-course pair.

### How Foreign Keys Implement Relationships
A foreign key is a column in one table that references the primary key of
another table. The database engine enforces that you cannot insert a value
into the foreign key column that doesn't exist in the referenced table.
This is called **referential integrity**.

### Why the Enrollments Table Is Necessary
SQL tables cannot natively store a "list" in a column. To model the
many-to-many relationship between Students and Courses, we need a separate
table where each row represents one relationship (one enrollment). The
`Enrollments` table also stores extra data about that relationship —
progress, status, and enrollment date — which neither the student nor the
course table "owns" alone.

---

## Task 2: Relationship Validation

| Relationship | FK Location | References |
|---|---|---|
| Instructor → Courses | `Courses.InstructorId` | `Users.UserId` |
| Student → Enrollments | `Enrollments.StudentId` | `Users.UserId` |
| Course → Enrollments | `Enrollments.CourseId` | `Courses.CourseId` |
| Student → CourseRatings | `CourseRatings.StudentId` | `Users.UserId` |
| Course → CourseRatings | `CourseRatings.CourseId` | `Courses.CourseId` |

### Constraints That Prevent Invalid Relationships
- `FOREIGN KEY` constraints: You cannot enroll a student with an
  ID that doesn't exist in Users, or in a course that doesn't exist
  in Courses. The database will raise an error.
- `NOT NULL` on FK columns: Every enrollment must have a valid student
  AND a valid course — neither can be omitted.

### The UNIQUE Constraint on (StudentId, CourseId)
Without it, the same student could be inserted into a course twice,
creating duplicate enrollment rows and corrupting progress tracking,
billing, and reporting. The `UNIQUE (StudentId, CourseId)` constraint
makes this a hard error at the database level — not just a code check.

### Deleting a Course With Active Enrollments
SQL Server will **reject the DELETE** and raise a foreign key violation
error. The `Enrollments.CourseId` FK references `Courses.CourseId`, so
the course cannot be removed while any enrollment row references it.
To delete a course, you must first delete or reassign all related
enrollments and ratings. This is intentional — it prevents orphaned data.

---

## Task 3: Queries Demonstrating Relationships

See `SmartLearnDB.sql` for all queries. Summary:

- **One-to-Many** (instructor → courses): Query 2 — courses by instructor using INNER JOIN
- **Many-to-Many** (students ↔ courses through enrollments): The multi-JOIN query connecting
  Students → Enrollments → Courses → Instructors (4 tables)
- **Multiple relationship levels**: The 4-table JOIN traverses User → Enrollment → Course → User
  (two different User roles in one query)
- **LEFT JOIN showing missing data**: Courses with ratings query — courses with zero ratings
  still appear with NULL/default values, proving the LEFT JOIN includes unmatched rows