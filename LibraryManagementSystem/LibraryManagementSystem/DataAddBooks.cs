using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace LibraryManagementSystem
{
    class DataAddBooks
    {
        MySqlConnection connect = new MySqlConnection(
            "Server=localhost;Port=3306;Database=loginform;Uid=root;Pwd=mubeen123;");

        public int ID { get; set; }
        public string BookTitle { get; set; }
        public string Author { get; set; }
        public string Published { get; set; }
        public string Image { get; set; }
        public string Status { get; set; }

        public List<DataAddBooks> addBooksData()
        {
            List<DataAddBooks> listData = new List<DataAddBooks>();

            try
            {
                connect.Open();
                string selectData = "SELECT * FROM books WHERE date_delete IS NULL";

                using (MySqlCommand cmd = new MySqlCommand(selectData, connect))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DataAddBooks dab = new DataAddBooks
                            {
                                ID = Convert.ToInt32(reader["id"]),
                                BookTitle = reader["book_title"].ToString(),
                                Author = reader["author"].ToString(),
                                Published = Convert.ToDateTime(reader["published_date"]).ToString("yyyy-MM-dd"),
                                Image = reader["image"].ToString(),
                                Status = reader["status"].ToString()
                            };
                            listData.Add(dab);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Database Error: " + ex.Message);
            }
            finally
            {
                connect.Close();
            }

            return listData;
        }
    }
}