using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql;
using MySql.Data.MySqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace ChessUI
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        static void Login(System.Windows.Forms.TextBox user, System.Windows.Forms.TextBox pass)
        {
            try
            {
                string username = user.Text;
                string password = pass.Text;

                string connstring = "Server=db-mysql-nyc3-70559-do-user-15626248-0.c.db.ondigitalocean.com;Port=25060;Database=login;Uid=doadmin;Pwd=AVNS_vrWwNwI19lugI3fo-6y;";
                MySqlConnection con;

                using (con = new MySqlConnection())
                {
                    con.ConnectionString = connstring;
                    con.Open();
                    MySqlDataAdapter da = new MySqlDataAdapter(new MySqlCommand("SELECT COUNT(*) FROM users WHERE username='" + username + "' AND password='" + password + "'", con));
                    
                    DataTable loginTable = new DataTable();
                    da.Fill(loginTable);
                    if (loginTable.Rows[0][0].ToString() == "1")
                    {
                        MessageBox.Show("Logged in Successfully");
                        
                    }
                    else
                    {
                        MessageBox.Show("Invalid username or password");
                        pass.Clear();
                    }
                    Console.Out.WriteLine("We connected");
                    con.Close();
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        private void okButton_Click(object sender, EventArgs e)
        {
            Login(usernameTextBox, textBox1);
        }

        private void pressEnter(object sender, KeyEventArgs e)
        {
           if (e.KeyCode == Keys.Enter)
            {
                Login(usernameTextBox, textBox1);

                e.Handled = true;
                e.SuppressKeyPress = true;
            }

        }
    }
}
