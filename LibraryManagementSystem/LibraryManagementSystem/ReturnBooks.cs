using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace LibraryManagementSystem
{
    public partial class ReturnBooks : UserControl
    {
        MySqlConnection connect = new MySqlConnection(
            "Server=localhost;Port=3306;Database=loginform;Uid=root;Pwd=mubeen123;");

        public ReturnBooks()
        {
            InitializeComponent();
            displayIssuedBooksData();
        }

        public void refreshData()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)refreshData);
                return;
            }
            displayIssuedBooksData();
        }

        private void returnBooks_returnBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(returnBooks_issueID.Text) ||
                string.IsNullOrEmpty(returnBooks_name.Text) ||
                string.IsNullOrEmpty(returnBooks_contact.Text) ||
                string.IsNullOrEmpty(returnBooks_email.Text) ||
                string.IsNullOrEmpty(returnBooks_bookTitle.Text) ||
                string.IsNullOrEmpty(returnBooks_author.Text) ||
                bookIssue_issueDate.Value == null)
            {
                MessageBox.Show("Please select item first", "Error Message",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult check = MessageBox.Show($"Are you sure that Issue ID: {returnBooks_issueID.Text.Trim()} is returned already?",
                "Confirmation Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (check == DialogResult.Yes)
            {
                try
                {
                    connect.Open();
                    DateTime today = DateTime.Today;

                    // Start transaction for atomic operations
                    using (MySqlTransaction transaction = connect.BeginTransaction())
                    {
                        try
                        {
                            // Update issue status
                            string updateIssue = "UPDATE issues SET status = @status, date_update = @dateUpdate " +
                                "WHERE issue_id = @issueID";

                            using (MySqlCommand cmd = new MySqlCommand(updateIssue, connect, transaction))
                            {
                                cmd.Parameters.AddWithValue("@status", "Return");
                                cmd.Parameters.AddWithValue("@dateUpdate", today.ToString("yyyy-MM-dd"));
                                cmd.Parameters.AddWithValue("@issueID", returnBooks_issueID.Text.Trim());
                                cmd.ExecuteNonQuery();
                            }

                            // Update book status to Available
                            string updateBook = "UPDATE books SET status = 'Available' " +
                                "WHERE book_title = @title AND author = @author";

                            using (MySqlCommand cmd = new MySqlCommand(updateBook, connect, transaction))
                            {
                                cmd.Parameters.AddWithValue("@title", returnBooks_bookTitle.Text.Trim());
                                cmd.Parameters.AddWithValue("@author", returnBooks_author.Text.Trim());
                                cmd.ExecuteNonQuery();
                            }

                            // Commit transaction if both updates succeed
                            transaction.Commit();

                            displayIssuedBooksData();
                            MessageBox.Show("Returned successfully!", "Information Message",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            clearFields();
                        }
                        catch
                        {
                            // Rollback if any error occurs
                            transaction.Rollback();
                            throw;
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
                    if (connect.State == ConnectionState.Open)
                    {
                        connect.Close();
                    }
                }
            }
        }

        public void displayIssuedBooksData()
        {
            DataIssueBooks dib = new DataIssueBooks();
            List<DataIssueBooks> listData = dib.ReturnIssueBooksData();
            dataGridView1.DataSource = listData;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                returnBooks_issueID.Text = row.Cells[1].Value?.ToString() ?? "";
                returnBooks_name.Text = row.Cells[2].Value?.ToString() ?? "";
                returnBooks_contact.Text = row.Cells[3].Value?.ToString() ?? "";
                returnBooks_email.Text = row.Cells[4].Value?.ToString() ?? "";
                returnBooks_bookTitle.Text = row.Cells[5].Value?.ToString() ?? "";
                returnBooks_author.Text = row.Cells[6].Value?.ToString() ?? "";

                if (DateTime.TryParse(row.Cells[7].Value?.ToString(), out DateTime issueDate))
                {
                    bookIssue_issueDate.Value = issueDate;
                }
            }
        }

        public void clearFields()
        {
            returnBooks_issueID.Text = "";
            returnBooks_name.Text = "";
            returnBooks_contact.Text = "";
            returnBooks_email.Text = "";
            returnBooks_bookTitle.Text = "";
            returnBooks_author.Text = "";
        }

        private void returnBooks_clearBtn_Click(object sender, EventArgs e)
        {
            clearFields();
        }
    }
}