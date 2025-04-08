using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace LibraryManagementSystem
{
    public partial class IssueBooks : UserControl
    {
        MySqlConnection connect = new MySqlConnection(
            "Server=localhost;Port=3306;Database=loginform;Uid=root;Pwd=mubeen123;");

        public IssueBooks()
        {
            InitializeComponent();
            displayBookIssueData();
            DataBookTitle();
        }

        public void refreshData()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)refreshData);
                return;
            }
            displayBookIssueData();
            DataBookTitle();
        }

        public void displayBookIssueData()
        {
            DataIssueBooks dib = new DataIssueBooks();
            List<DataIssueBooks> listData = dib.IssueBooksData();
            dataGridView1.DataSource = listData;
        }

        private void bookIssue_addBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(bookIssue_id.Text) ||
                string.IsNullOrEmpty(bookIssue_name.Text) ||
                string.IsNullOrEmpty(bookIssue_contact.Text) ||
                string.IsNullOrEmpty(bookIssue_email.Text) ||
                string.IsNullOrEmpty(bookIssue_bookTitle.Text) ||
                string.IsNullOrEmpty(bookIssue_author.Text) ||
                bookIssue_issueDate.Value == null ||
                bookIssue_returnDate.Value == null ||
                string.IsNullOrEmpty(bookIssue_status.Text)) //||
                //bookIssue_picture.Image == null)
            {
                MessageBox.Show("Please fill all blank fields", "Error Message",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                connect.Open();
                DateTime today = DateTime.Today;

                string insertData = "INSERT INTO issues " +
                    "(issue_id, full_name, contact, email, book_title, author, status, issue_date, return_date, date_insert) " +
                    "VALUES(@issueID, @fullname, @contact, @email, @bookTitle, @author, @status, @issueDate, @returnDate, @dateInsert)";

                using (MySqlCommand cmd = new MySqlCommand(insertData, connect))
                {
                    cmd.Parameters.AddWithValue("@issueID", bookIssue_id.Text.Trim());
                    cmd.Parameters.AddWithValue("@fullname", bookIssue_name.Text.Trim());
                    cmd.Parameters.AddWithValue("@contact", bookIssue_contact.Text.Trim());
                    cmd.Parameters.AddWithValue("@email", bookIssue_email.Text.Trim());
                    cmd.Parameters.AddWithValue("@bookTitle", bookIssue_bookTitle.Text.Trim());
                    cmd.Parameters.AddWithValue("@author", bookIssue_author.Text.Trim());
                    cmd.Parameters.AddWithValue("@status", bookIssue_status.Text.Trim());
                    cmd.Parameters.AddWithValue("@issueDate", bookIssue_issueDate.Value.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@returnDate", bookIssue_returnDate.Value.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@dateInsert", today.ToString("yyyy-MM-dd"));

                    cmd.ExecuteNonQuery();

                    // Update book status to 'Issued'
                    string updateBookStatus = "UPDATE books SET status = 'Issued' WHERE book_title = @title AND author = @author";
                    using (MySqlCommand updateCmd = new MySqlCommand(updateBookStatus, connect))
                    {
                        updateCmd.Parameters.AddWithValue("@title", bookIssue_bookTitle.Text.Trim());
                        updateCmd.Parameters.AddWithValue("@author", bookIssue_author.Text.Trim());
                        updateCmd.ExecuteNonQuery();
                    }

                    displayBookIssueData();
                    DataBookTitle(); // Refresh available books list
                    MessageBox.Show("Issued successfully!", "Information Message",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    clearFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error Message",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                connect.Close();
            }
        }

        public void clearFields()
        {
            bookIssue_id.Text = "";
            bookIssue_name.Text = "";
            bookIssue_contact.Text = "";
            bookIssue_email.Text = "";
            bookIssue_bookTitle.SelectedIndex = -1;
            bookIssue_author.SelectedIndex = -1;
            bookIssue_status.SelectedIndex = -1;
            bookIssue_picture.Image = null;
        }

        public void DataBookTitle()
        {
            try
            {
                connect.Open();
                string selectData = "SELECT id, book_title FROM books WHERE status = 'Available' AND date_delete IS NULL";

                using (MySqlCommand cmd = new MySqlCommand(selectData, connect))
                {
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable table = new DataTable();
                    adapter.Fill(table);

                    bookIssue_bookTitle.DataSource = table;
                    bookIssue_bookTitle.DisplayMember = "book_title";
                    bookIssue_bookTitle.ValueMember = "id";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error Message",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                connect.Close();
            }
        }

        private void bookIssue_bookTitle_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (bookIssue_bookTitle.SelectedValue != null &&
                bookIssue_bookTitle.SelectedValue.ToString() != "System.Data.DataRowView")
            {
                try
                {
                    connect.Open();
                    int selectID = Convert.ToInt32(bookIssue_bookTitle.SelectedValue);

                    string selectData = "SELECT * FROM books WHERE id = @id";
                    using (MySqlCommand cmd = new MySqlCommand(selectData, connect))
                    {
                        cmd.Parameters.AddWithValue("@id", selectID);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                bookIssue_author.Text = reader["author"].ToString();
                                string imagePath = reader["image"].ToString();

                                if (!string.IsNullOrEmpty(imagePath) && System.IO.File.Exists(imagePath))
                                {
                                    bookIssue_picture.Image = Image.FromFile(imagePath);
                                }
                                else
                                {
                                    bookIssue_picture.Image = null;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error Message",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    connect.Close();
                }
            }
        }

        private void dataGridView1_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                bookIssue_id.Text = row.Cells[1].Value.ToString();
                bookIssue_name.Text = row.Cells[2].Value.ToString();
                bookIssue_contact.Text = row.Cells[3].Value.ToString();
                bookIssue_email.Text = row.Cells[4].Value.ToString();
                bookIssue_bookTitle.Text = row.Cells[5].Value.ToString();
                bookIssue_author.Text = row.Cells[6].Value.ToString();
                bookIssue_issueDate.Value = Convert.ToDateTime(row.Cells[7].Value);
                bookIssue_returnDate.Value = Convert.ToDateTime(row.Cells[8].Value);
                bookIssue_status.Text = row.Cells[9].Value.ToString();
            }
        }

        private void bookIssue_updateBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(bookIssue_id.Text))
            {
                MessageBox.Show("Please select an issue to update", "Error Message",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult check = MessageBox.Show("Update Issue ID: " + bookIssue_id.Text + "?",
                "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (check == DialogResult.Yes)
            {
                try
                {
                    connect.Open();
                    DateTime today = DateTime.Today;

                    string updateData = "UPDATE issues SET full_name = @fullName, contact = @contact, " +
                        "email = @email, book_title = @bookTitle, author = @author, status = @status, " +
                        "issue_date = @issueDate, return_date = @returnDate, date_update = @dateUpdate " +
                        "WHERE issue_id = @issueID";

                    using (MySqlCommand cmd = new MySqlCommand(updateData, connect))
                    {
                        cmd.Parameters.AddWithValue("@fullName", bookIssue_name.Text.Trim());
                        cmd.Parameters.AddWithValue("@contact", bookIssue_contact.Text.Trim());
                        cmd.Parameters.AddWithValue("@email", bookIssue_email.Text.Trim());
                        cmd.Parameters.AddWithValue("@bookTitle", bookIssue_bookTitle.Text.Trim());
                        cmd.Parameters.AddWithValue("@author", bookIssue_author.Text.Trim());
                        cmd.Parameters.AddWithValue("@status", bookIssue_status.Text.Trim());
                        cmd.Parameters.AddWithValue("@issueDate", bookIssue_issueDate.Value.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@returnDate", bookIssue_returnDate.Value.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@dateUpdate", today.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@issueID", bookIssue_id.Text.Trim());

                        cmd.ExecuteNonQuery();

                        displayBookIssueData();
                        MessageBox.Show("Updated successfully!", "Information Message",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        clearFields();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error Message",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    connect.Close();
                }
            }
        }

        private void bookIssue_deleteBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(bookIssue_id.Text))
            {
                MessageBox.Show("Please select an issue to delete", "Error Message",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult check = MessageBox.Show("Delete Issue ID: " + bookIssue_id.Text + "?",
                "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (check == DialogResult.Yes)
            {
                try
                {
                    connect.Open();
                    DateTime today = DateTime.Today;

                    // Soft delete
                    string updateData = "UPDATE issues SET date_delete = @dateDelete WHERE issue_id = @issueID";

                    // If you want to hard delete instead, use:
                    // string deleteData = "DELETE FROM issues WHERE issue_id = @issueID";

                    using (MySqlCommand cmd = new MySqlCommand(updateData, connect))
                    {
                        cmd.Parameters.AddWithValue("@dateDelete", today.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@issueID", bookIssue_id.Text.Trim());

                        cmd.ExecuteNonQuery();

                        // Update book status back to 'Available'
                        string updateBookStatus = "UPDATE books SET status = 'Available' " +
                            "WHERE book_title = @title AND author = @author";
                        using (MySqlCommand updateCmd = new MySqlCommand(updateBookStatus, connect))
                        {
                            updateCmd.Parameters.AddWithValue("@title", bookIssue_bookTitle.Text.Trim());
                            updateCmd.Parameters.AddWithValue("@author", bookIssue_author.Text.Trim());
                            updateCmd.ExecuteNonQuery();
                        }

                        displayBookIssueData();
                        DataBookTitle(); // Refresh available books list
                        MessageBox.Show("Deleted successfully!", "Information Message",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        clearFields();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error Message",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    connect.Close();
                }
            }
        }

        private void bookIssue_clearBtn_Click(object sender, EventArgs e)
        {
            clearFields();
        }
    }
}