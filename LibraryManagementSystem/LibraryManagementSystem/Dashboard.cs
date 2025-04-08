using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace LibraryManagementSystem
{
    public partial class Dashboard : UserControl
    {
        MySqlConnection connect = new MySqlConnection(
            "Server=localhost;Port=3306;Database=loginform;Uid=root;Pwd=mubeen123;");

        public Dashboard()
        {
            InitializeComponent();
            displayAB();
            displayIB();
            displayRB();
        }

        public void refreshData()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)refreshData);
                return;
            }
            displayAB();
            displayIB();
            displayRB();
        }

        public void displayAB()
        {
            try
            {
                connect.Open();
                string selectData = "SELECT COUNT(id) FROM books " +
                    "WHERE status = 'Available' AND date_delete IS NULL";

                using (MySqlCommand cmd = new MySqlCommand(selectData, connect))
                {
                    object result = cmd.ExecuteScalar();
                    dashboard_AB.Text = result?.ToString() ?? "0";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error counting available books: " + ex.Message,
                    "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (connect.State == ConnectionState.Open)
                {
                    connect.Close();
                }
            }
        }

        public void displayIB()
        {
            try
            {
                connect.Open();
                string selectData = "SELECT COUNT(id) FROM issues " +
                    "WHERE date_delete IS NULL";

                using (MySqlCommand cmd = new MySqlCommand(selectData, connect))
                {
                    object result = cmd.ExecuteScalar();
                    dashboard_IB.Text = result?.ToString() ?? "0";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error counting issued books: " + ex.Message,
                    "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (connect.State == ConnectionState.Open)
                {
                    connect.Close();
                }
            }
        }

        public void displayRB()
        {
            try
            {
                connect.Open();
                string selectData = "SELECT COUNT(id) FROM issues " +
                    "WHERE status = 'Return' AND date_delete IS NULL";

                using (MySqlCommand cmd = new MySqlCommand(selectData, connect))
                {
                    object result = cmd.ExecuteScalar();
                    dashboard_RB.Text = result?.ToString() ?? "0";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error counting returned books: " + ex.Message,
                    "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
}