using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace LibraryManagementSystem
{
    public partial class AddBooks : UserControl
    {
        MySqlConnection connect = new MySqlConnection(
            "Server=localhost;Port=3306;Database=loginform;Uid=root;Pwd=mubeen123;");

        public AddBooks()
        {
            InitializeComponent();
            displayBooks();
        }

        public void refreshData()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)refreshData);
                return;
            }
            displayBooks();
        }

        private String imagePath;
        private void addBooks_importBtn_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "Image Files (*.jpg; *.png)|*.jpg;*.png";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    imagePath = dialog.FileName;
                    addBooks_picture.ImageLocation = imagePath;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void addBooks_addBtn_Click(object sender, EventArgs e)
        {
            if (addBooks_picture.Image == null
                || addBooks_bookTitle.Text == ""
                || addBooks_author.Text == ""
                || addBooks_published.Value == null
                || addBooks_status.Text == "")
            {
                MessageBox.Show("Please fill all blank fields", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (connect.State == ConnectionState.Closed)
                {
                    try
                    {
                        DateTime today = DateTime.Today;
                        connect.Open();
                        string insertData = "INSERT INTO books " +
                            "(book_title, author, published_date, status, image, date_insert) " +
                            "VALUES(@bookTitle, @author, @published_date, @status, @image, @dateInsert)";

                        string path = Path.Combine(@"C:\Pictures of LMS",
                            addBooks_bookTitle.Text + "_" + addBooks_author.Text.Trim() + ".jpg");

                        string directoryPath = Path.GetDirectoryName(path);

                        if (!Directory.Exists(directoryPath))
                        {
                            Directory.CreateDirectory(directoryPath);
                        }

                        File.Copy(imagePath, path, true);

                        using (MySqlCommand cmd = new MySqlCommand(insertData, connect))
                        {
                            cmd.Parameters.AddWithValue("@bookTitle", addBooks_bookTitle.Text.Trim());
                            cmd.Parameters.AddWithValue("@author", addBooks_author.Text.Trim());
                            cmd.Parameters.AddWithValue("@published_date", addBooks_published.Value.ToString("yyyy-MM-dd"));
                            cmd.Parameters.AddWithValue("@status", addBooks_status.Text.Trim());
                            cmd.Parameters.AddWithValue("@image", path);
                            cmd.Parameters.AddWithValue("@dateInsert", today.ToString("yyyy-MM-dd"));

                            cmd.ExecuteNonQuery();

                            displayBooks();

                            MessageBox.Show("Added successfully!", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            clearFields();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        connect.Close();
                    }
                }
            }
        }

        public void clearFields()
        {
            addBooks_bookTitle.Text = "";
            addBooks_author.Text = "";
            addBooks_picture.Image = null;
            addBooks_status.SelectedIndex = -1;
            imagePath = null;
        }

        public void displayBooks()
        {
            DataAddBooks dab = new DataAddBooks();
            List<DataAddBooks> listData = dab.addBooksData();
            dataGridView1.DataSource = listData;
        }

        private int bookID = 0;
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                bookID = (int)row.Cells[0].Value;
                addBooks_bookTitle.Text = row.Cells[1].Value.ToString();
                addBooks_author.Text = row.Cells[2].Value.ToString();
                addBooks_published.Text = row.Cells[3].Value.ToString();

                string imagePath = row.Cells[4].Value.ToString();

                if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                {
                    addBooks_picture.Image = Image.FromFile(imagePath);
                }
                else
                {
                    addBooks_picture.Image = null;
                }
                addBooks_status.Text = row.Cells[5].Value.ToString();
            }
        }

        private void addBooks_clearBtn_Click(object sender, EventArgs e)
        {
            clearFields();
        }

        private void addBooks_updateBtn_Click(object sender, EventArgs e)
        {
            if (bookID == 0 || addBooks_bookTitle.Text == "" || addBooks_author.Text == "")
            {
                MessageBox.Show("Please select a book first", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult check = MessageBox.Show("Update Book ID: " + bookID + "?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (check == DialogResult.Yes)
            {
                try
                {
                    connect.Open();
                    string updateData = "UPDATE books SET " +
                        "book_title = @bookTitle, " +
                        "author = @author, " +
                        "published_date = @published, " +
                        "status = @status, " +
                        "date_update = @dateUpdate " +
                        "WHERE id = @id";

                    using (MySqlCommand cmd = new MySqlCommand(updateData, connect))
                    {
                        cmd.Parameters.AddWithValue("@bookTitle", addBooks_bookTitle.Text.Trim());
                        cmd.Parameters.AddWithValue("@author", addBooks_author.Text.Trim());
                        cmd.Parameters.AddWithValue("@published", addBooks_published.Value.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@status", addBooks_status.Text.Trim());
                        cmd.Parameters.AddWithValue("@dateUpdate", DateTime.Today.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@id", bookID);

                        cmd.ExecuteNonQuery();

                        // Update image if changed
                        if (!string.IsNullOrEmpty(imagePath))
                        {
                            string newPath = Path.Combine(@"E:\BS Software Engineering\4th Semester of BSSE\Visual Programming\Practice\New folder\LibraryManagementSystem\Books_Directory",
                                addBooks_bookTitle.Text + "_" + addBooks_author.Text.Trim() + ".jpg");

                            if (File.Exists(newPath)) File.Delete(newPath);
                            File.Copy(imagePath, newPath);

                            // Update image path in database
                            string updateImage = "UPDATE books SET image = @image WHERE id = @id";
                            using (MySqlCommand imgCmd = new MySqlCommand(updateImage, connect))
                            {
                                imgCmd.Parameters.AddWithValue("@image", newPath);
                                imgCmd.Parameters.AddWithValue("@id", bookID);
                                imgCmd.ExecuteNonQuery();
                            }
                        }

                        displayBooks();
                        MessageBox.Show("Updated successfully!", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        clearFields();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    connect.Close();
                }
            }
        }

        private void addBooks_deleteBtn_Click(object sender, EventArgs e)
        {
            if (bookID == 0)
            {
                MessageBox.Show("Please select a book first", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult check = MessageBox.Show("Delete Book ID: " + bookID + "?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (check == DialogResult.Yes)
            {
                try
                {
                    connect.Open();
                    string softDelete = "UPDATE books SET date_delete = @dateDelete WHERE id = @id";

                    using (MySqlCommand cmd = new MySqlCommand(softDelete, connect))
                    {
                        cmd.Parameters.AddWithValue("@dateDelete", DateTime.Today.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@id", bookID);
                        cmd.ExecuteNonQuery();

                        displayBooks();
                        MessageBox.Show("Deleted successfully!", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        clearFields();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    connect.Close();
                }
            }
        }
    }
}