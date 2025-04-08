using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace LibraryManagementSystem
{
    class DataIssueBooks
    {
        MySqlConnection connect = new MySqlConnection(
            "Server=localhost;Port=3306;Database=loginform;Uid=root;Pwd=mubeen123;");

        public int ID { get; set; }
        public string IssueID { get; set; }
        public string Name { get; set; }
        public string Contact { get; set; }
        public string Email { get; set; }
        public string BookTitle { get; set; }
        public string Author { get; set; }
        public string DateIssue { get; set; }
        public string DateReturn { get; set; }
        public string Status { get; set; }

        public List<DataIssueBooks> IssueBooksData()
        {
            List<DataIssueBooks> listData = new List<DataIssueBooks>();

            try
            {
                connect.Open();
                string selectData = "SELECT * FROM issues WHERE date_delete IS NULL";

                using (MySqlCommand cmd = new MySqlCommand(selectData, connect))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DataIssueBooks dib = new DataIssueBooks
                            {
                                ID = Convert.ToInt32(reader["id"]),
                                IssueID = reader["issue_id"].ToString(),
                                Name = reader["full_name"].ToString(),
                                Contact = reader["contact"].ToString(),
                                Email = reader["email"].ToString(),
                                BookTitle = reader["book_title"].ToString(),
                                Author = reader["author"].ToString(),
                                DateIssue = Convert.ToDateTime(reader["issue_date"]).ToString("yyyy-MM-dd"),
                                DateReturn = Convert.ToDateTime(reader["return_date"]).ToString("yyyy-MM-dd"),
                                Status = reader["status"].ToString()
                            };
                            listData.Add(dib);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving issue books: " + ex.Message);
                // Consider logging the full exception details
            }
            finally
            {
                if (connect.State == ConnectionState.Open)
                {
                    connect.Close();
                }
            }

            return listData;
        }

        public List<DataIssueBooks> ReturnIssueBooksData()
        {
            List<DataIssueBooks> listData = new List<DataIssueBooks>();

            try
            {
                connect.Open();
                string selectData = "SELECT * FROM issues WHERE status = 'Not Return' AND date_delete IS NULL";

                using (MySqlCommand cmd = new MySqlCommand(selectData, connect))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DataIssueBooks dib = new DataIssueBooks
                            {
                                ID = Convert.ToInt32(reader["id"]),
                                IssueID = reader["issue_id"].ToString(),
                                Name = reader["full_name"].ToString(),
                                Contact = reader["contact"].ToString(),
                                Email = reader["email"].ToString(),
                                BookTitle = reader["book_title"].ToString(),
                                Author = reader["author"].ToString(),
                                DateIssue = Convert.ToDateTime(reader["issue_date"]).ToString("yyyy-MM-dd"),
                                DateReturn = Convert.ToDateTime(reader["return_date"]).ToString("yyyy-MM-dd"),
                                Status = reader["status"].ToString()
                            };
                            listData.Add(dib);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving return books: " + ex.Message);
                // Consider logging the full exception details
            }
            finally
            {
                if (connect.State == ConnectionState.Open)
                {
                    connect.Close();
                }
            }

            return listData;
        }
    }
}