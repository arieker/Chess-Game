using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace ChessUI
{
    public partial class RegisterForm : Form
    {

        public RegisterForm()
        {
            InitializeComponent();
            okButton.Enabled = false;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            string username = usernameTextBox.Text;
            string password = passwordTextBox.Text;
            string passwordMatch = confirmPasswordTextBox.Text;
            if (password != passwordMatch)
            {
                MessageBox.Show("Passwords do not match.");
                passwordTextBox.Clear();
                confirmPasswordTextBox.Clear();
                return;
            }
            if (username.Contains(' '))
            {
                MessageBox.Show("Username can not have spaces in it.");
                passwordTextBox.Clear();
                confirmPasswordTextBox.Clear();
                return;
            }
            if (username.Length > 11 && username.Substring(0,11) == ("sendRequest"))
            {
                MessageBox.Show("Invalid username or password");
                passwordTextBox.Clear();
                confirmPasswordTextBox.Clear();
                return;
            }
            try
            {
                MySqlConnection con;

                using (con = new MySqlConnection())
                {
                    con.ConnectionString = ConfigurationManager.ConnectionStrings["users"].ConnectionString;
                    try
                    {
                        con.Open();
                    }
                    catch
                    {
                        MessageBox.Show("Connection for registration failed. Please try again.");
                        return;
                    }
                    MySqlCommand command = new MySqlCommand("SELECT * FROM users WHERE username = '" + username + "'", con);
                    MySqlDataReader dr = command.ExecuteReader();
                    if (dr.Read())
                    {
                        MessageBox.Show("This username is already taken.");
                        passwordTextBox.Clear();
                        confirmPasswordTextBox.Clear();
                        usernameTextBox.Clear();
                    }
                    else
                    {
                        dr.Close();
                        MySqlCommand insertcommand = new MySqlCommand("INSERT INTO `login`.`users` (`username`, `password`, `nickname`, `wins`, `losses`, `draws`, `opendate`, `status`) VALUES ('" + username + "', '" + password.GetHashCode() + "', '" + nicknameTextBox.Text + "', '0', '0', '0', '" + DateTime.Now.Date.ToString("dd/MM/yyyy") + "', 'Offline');", con);
                        MySqlDataReader dr2 = insertcommand.ExecuteReader();
                        MessageBox.Show("Account successfully created.");
                        this.Close();
                    }
                    con.Close();
                }
            }
            catch (SqlException er)
            {
                Console.WriteLine(er.ToString());
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBoxes_TextChanged(object sender, EventArgs e)
        {
            if (usernameTextBox.Text.Length > 0 && passwordTextBox.Text.Length > 0 && confirmPasswordTextBox.Text.Length > 0)
            {
                okButton.Enabled = true;
            }
            else
            {
                okButton.Enabled = false;
            }
        }
    }
}
