using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace LibraryManagementSystem
{
    public partial class RegisterForm : Form
    {
        MySqlConnection connect = new MySqlConnection(
            "Server=localhost;Port=3306;Database=loginform;Uid=root;Pwd=mubeen123;");

        public RegisterForm()
        {
            InitializeComponent();

            // Initialize button styles
            register_btn.FlatStyle = FlatStyle.Flat;
            register_btn.FlatAppearance.BorderSize = 0;
            register_btn.BackColor = Color.FromArgb(91, 17, 102); // Purple color
            register_btn.ForeColor = Color.White;
            register_btn.Cursor = Cursors.Hand;

            // Assign event handlers
            register_btn.MouseEnter += register_btn_MouseEnter;
            register_btn.MouseLeave += register_btn_MouseLeave;

            signIn_btn.FlatStyle = FlatStyle.Flat;
            signIn_btn.FlatAppearance.BorderSize = 0;
            signIn_btn.BackColor = Color.FromArgb(91, 17, 102);
            signIn_btn.ForeColor = Color.White;
            signIn_btn.Cursor = Cursors.Hand;

            signIn_btn.MouseEnter += signIn_btn_MouseEnter;
            signIn_btn.MouseLeave += signIn_btn_MouseLeave;
        }

        private void register_btn_MouseEnter(object sender, EventArgs e)
        {
            register_btn.BackColor = Color.Indigo;
            register_btn.FlatAppearance.MouseOverBackColor = Color.Indigo;
        }

        private void register_btn_MouseLeave(object sender, EventArgs e)
        {
            register_btn.BackColor = Color.FromArgb(91, 17, 102);
            register_btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(91, 17, 102);
        }

        private void signIn_btn_Click(object sender, EventArgs e)
        {
            LoginForm lForm = new LoginForm();
            lForm.Show();
            this.Hide();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void register_showPass_CheckedChanged(object sender, EventArgs e)
        {
            register_password.PasswordChar = register_showPass.Checked ? '\0' : '*';
        }

        private void register_btn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(register_email.Text) ||
                string.IsNullOrWhiteSpace(register_username.Text) ||
                string.IsNullOrWhiteSpace(register_password.Text))
            {
                MessageBox.Show("Please fill all blank fields", "Error Message",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                connect.Open();

                // Check if username exists
                string checkUsername = "SELECT COUNT(*) FROM users WHERE username = @username";
                using (MySqlCommand checkCMD = new MySqlCommand(checkUsername, connect))
                {
                    checkCMD.Parameters.AddWithValue("@username", register_username.Text.Trim());
                    int count = Convert.ToInt32(checkCMD.ExecuteScalar());

                    if (count >= 1)
                    {
                        MessageBox.Show($"{register_username.Text.Trim()} is already taken",
                            "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                // Register new user
                string insertData = @"INSERT INTO users (email, username, password, date_register) 
                                    VALUES(@email, @username, @password, @date)";

                using (MySqlCommand insertCMD = new MySqlCommand(insertData, connect))
                {
                    insertCMD.Parameters.AddWithValue("@email", register_email.Text.Trim());
                    insertCMD.Parameters.AddWithValue("@username", register_username.Text.Trim());
                    insertCMD.Parameters.AddWithValue("@password", register_password.Text.Trim());
                    insertCMD.Parameters.AddWithValue("@date", DateTime.Today.ToString("yyyy-MM-dd"));

                    insertCMD.ExecuteNonQuery();

                    MessageBox.Show("Registered successfully!", "Information Message",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Redirect to login
                    LoginForm lForm = new LoginForm();
                    lForm.Show();
                    this.Hide();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database error: {ex.Message}", "Error Message",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (connect.State == ConnectionState.Open)
                    connect.Close();
            }
        }

        private void signIn_btn_MouseEnter(object sender, EventArgs e)
        {
            signIn_btn.BackColor = Color.Indigo;
            signIn_btn.FlatAppearance.MouseOverBackColor = Color.Indigo;
        }

        private void signIn_btn_MouseLeave(object sender, EventArgs e)
        {
            signIn_btn.BackColor = Color.FromArgb(91, 17, 102);
            signIn_btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(91, 17, 102);
        }
    }
}