using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace ChessUI
{
    public partial class ChangeNicknameForm : Form
    {
        public ChangeNicknameForm()
        {
           
            InitializeComponent();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            // Change nickname to what is found in textbox
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
                        MessageBox.Show("Connection failed. Please try again.");
                        return;
                    }
                    MySqlCommand command = new MySqlCommand("UPDATE `login`.`users` SET `nickname` = '" + nicknameTextBox.Text + "' WHERE (`username` = '" + Program.currentUser.getUsername() + "');", con);
                    command.ExecuteReader();
                    Program.currentUser.setNickname(nicknameTextBox.Text);
                    con.Close();
                }
                this.Close();
            }
            catch (SqlException er)
            {
                Console.WriteLine(er.ToString());
            }
        }
    }
}
