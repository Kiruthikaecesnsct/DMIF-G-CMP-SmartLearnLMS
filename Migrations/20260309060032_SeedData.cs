using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Week_1.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastLoginDate",
                table: "Students");

            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "CourseId", "AverageRating", "Category", "CurrentEnrollments", "Description", "Difficulty", "InstructorId", "MaxStudents", "Title" },
                values: new object[,]
                {
                    { 1, 0.0, "Programming", 0, "Introduction to C# programming", "Beginner", 1, 200, "C# Basics" },
                    { 2, 0.0, "Programming", 0, "Deep dive into LINQ queries", "Advanced", 1, 100, "Advanced LINQ" },
                    { 3, 0.0, "Design", 0, "HTML and CSS fundamentals", "Beginner", 1, 150, "Web Design 101" },
                    { 4, 0.0, "Design", 0, "User interface design principles", "Intermediate", 1, 80, "UI/UX Principles" },
                    { 5, 0.0, "Data", 0, "Introduction to data science", "Intermediate", 1, 120, "Data Science Intro" },
                    { 6, 0.0, "Data", 0, "ML algorithms and applications", "Advanced", 1, 60, "Machine Learning" }
                });

            migrationBuilder.InsertData(
                table: "Instructors",
                columns: new[] { "InstructorId", "DateRegistered", "Email", "IsActive", "Password", "Username" },
                values: new object[] { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "instructor@smartlearn.com", true, "pass123", "admin_instructor" });

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_StudentId",
                table: "Enrollments",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_Students_StudentId",
                table: "Enrollments",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "StudentId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_Students_StudentId",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_StudentId",
                table: "Enrollments");

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "CourseId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "CourseId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "CourseId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "CourseId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "CourseId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "CourseId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Instructors",
                keyColumn: "InstructorId",
                keyValue: 1);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLoginDate",
                table: "Students",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
