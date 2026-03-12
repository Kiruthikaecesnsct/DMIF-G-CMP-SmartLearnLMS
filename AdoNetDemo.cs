using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Week_1
    {
    public class AdoNetDemo
        {
        private string connectionString = @"Server=localhost\SQLEXPRESS;
                                            Database=SmartLearnDB;
                                            Trusted_Connection=True;";
        //CONCEPT 2 — Reading Data with ADO.NET -Add this method.Walk through every single step — this is the most important part of ADO.NET teaching:
        // ADO.NET way to read students from the database
        // Think of it like a phone call — 5 steps: pick up phone, dial, speak, listen, hang up
        public void GetAllStudentsFromDatabase()
            {
            using (SqlConnection connection = new SqlConnection(connectionString))
                {
                try
                    {
                    connection.Open();
                    Console.WriteLine("✅ Connected to database!");

                    string sqlQuery = "SELECT UserId, Username, Email FROM Users WHERE UserType = 'Student'";
                    SqlCommand command = new SqlCommand(sqlQuery, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    Console.WriteLine("\n📚 STUDENTS IN DATABASE:");
                    Console.WriteLine("─────────────────────────────────");

                    while (reader.Read())
                        {
                        int userId = reader.GetInt32(0);
                        string username = reader.GetString(1);
                        string email = reader.GetString(2);
                        Console.WriteLine($"ID: {userId} | {username} ({email})");
                        }

                    reader.Close();
                    Console.WriteLine("─────────────────────────────────\n");
                    }
                catch (Exception ex)
                    {
                    Console.WriteLine($"❌ Error: {ex.Message}");
                    }
                }
            }
        //CONCEPT 3 — Inserting Data WITH Parameters (Security Warning Moment) .This is where you teach SQL injection.Students need to feel why parameters matter.
        public void AddStudentToDatabase(string username, string email, string password)
            {
            using (SqlConnection connection = new SqlConnection(connectionString))
                {
                try
                    {
                    connection.Open();

                    string sqlQuery = @"INSERT INTO Users (Username, Email, PasswordHash, UserType)
                                       VALUES (@Username, @Email, @Password, 'Student')";

                    SqlCommand command = new SqlCommand(sqlQuery, connection);
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Password", password);

                    int rowsAffected = command.ExecuteNonQuery();
                    Console.WriteLine($"✅ Student added! Rows affected: {rowsAffected}");
                    }
                catch (Exception ex)
                    {
                    Console.WriteLine($"❌ Error: {ex.Message}");
                    }
                }
            }

        // 🚫 NEVER DO THIS — SQL INJECTION VULNERABILITY
        // If username = "'; DROP TABLE Users; --"
        // Your ENTIRE Users table gets deleted by a hacker!

       // string dangerousQuery = $"INSERT INTO Users VALUES ('{username}', '{email}', '{password}')";

        // ALWAYS USE PARAMETERS WITH @ SYMBOLS — always, no exceptions
        }
    }
