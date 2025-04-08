using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace LibraryManagementSystem
{
    public partial class LoginForm : Form
    {
        // MySQL connection for Docker
        MySqlConnection connect = new MySqlConnection(
            "Server=localhost;Port=3306;Database=loginform;Uid=root;Pwd=mubeen123;");

        public LoginForm()
        {
            InitializeComponent();

            // Initialize button styles
            loginBtn.FlatStyle = FlatStyle.Flat;
            loginBtn.FlatAppearance.BorderSize = 0;
            loginBtn.BackColor = Color.FromArgb(91, 17, 102); // Purple color
            loginBtn.ForeColor = Color.White;
            loginBtn.Cursor = Cursors.Hand;

            // Assign event handlers
            loginBtn.MouseEnter += loginBtn_MouseEnter;
            loginBtn.MouseLeave += loginBtn_MouseLeave;

            signupBtn.FlatStyle = FlatStyle.Flat;
            signupBtn.FlatAppearance.BorderSize = 0;
            signupBtn.BackColor = Color.FromArgb(91, 17, 102);
            signupBtn.ForeColor = Color.White;
            signupBtn.Cursor = Cursors.Hand;

            signupBtn.MouseEnter += signupBtn_MouseEnter;
            signupBtn.MouseLeave += signupBtn_MouseLeave;
        }

        private void label1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void signupBtn_Click(object sender, EventArgs e)
        {
            RegisterForm rForm = new RegisterForm();
            rForm.Show();
            this.Hide();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            login_password.PasswordChar = login_showPass.Checked ? '\0' : '*';
        }

        private void loginBtn_Click(object sender, EventArgs e)
        {
            if (login_username.Text == "" || login_password.Text == "")
            {
                MessageBox.Show("Please fill all blank fields", "Error Message",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (connect.State != ConnectionState.Open)
                {
                    try
                    {
                        connect.Open();
                        string selectData = "SELECT * FROM users WHERE username = @username AND password = @password";

                        using (MySqlCommand cmd = new MySqlCommand(selectData, connect))
                        {
                            cmd.Parameters.AddWithValue("@username", login_username.Text.Trim());
                            cmd.Parameters.AddWithValue("@password", login_password.Text.Trim());

                            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                            DataTable table = new DataTable();
                            adapter.Fill(table);

                            if (table.Rows.Count >= 1)
                            {
                                MessageBox.Show("Login Successfully!", "Information Message",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                                MainForm mForm = new MainForm();
                                mForm.Show();
                                this.Hide();
                            }
                            else
                            {
                                MessageBox.Show("Incorrect Username/Password", "Error Message",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error connecting Database: " + ex.Message, "Error Message",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        connect.Close();
                    }
                }
            }
        }

        private void loginBtn_MouseEnter(object sender, EventArgs e)
        {
            loginBtn.BackColor = Color.Indigo;
            loginBtn.FlatAppearance.MouseOverBackColor = Color.Indigo;
        }

        private void loginBtn_MouseLeave(object sender, EventArgs e)
        {
            loginBtn.BackColor = Color.FromArgb(91, 17, 102);
            loginBtn.FlatAppearance.MouseOverBackColor = Color.FromArgb(91, 17, 102);
        }

        private void signupBtn_MouseEnter(object sender, EventArgs e)
        {
            signupBtn.BackColor = Color.Indigo;
            signupBtn.FlatAppearance.MouseOverBackColor = Color.Indigo;
        }

        private void signupBtn_MouseLeave(object sender, EventArgs e)
        {
            signupBtn.BackColor = Color.FromArgb(91, 17, 102);
            signupBtn.FlatAppearance.MouseOverBackColor = Color.FromArgb(91, 17, 102);
        }
    }
}