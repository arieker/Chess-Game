using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ChessUI
{
    public partial class FindMatchForm : Form
    {
        List<String> onlineUsernames = new List<String>();
        public FindMatchForm()
        {
            InitializeComponent();
            // Set onlineUsernames to the updated list of online users
            UpdateOnlineUserList(onlineUsernames); // Init the listbox with values
        }
        public void UpdateOnlineUserList(List<string> usernames)
        {
            MySqlConnection con;

            using (con = new MySqlConnection())
            {
                con.ConnectionString = ConfigurationManager.ConnectionStrings["users"].ConnectionString;
                con.Open();
                string query = "SELECT * FROM users WHERE status = 'Online' AND username != '" + Program.currentUser.getUsername() + "'";
                using (MySqlCommand command = new MySqlCommand(query, con))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            onlineUsernames.Add(reader.GetString(0));
                        }
                    }
                }
                con.Close();
            }
            onlineUsersListBox.Items.Clear();
            onlineUsersListBox.Items.AddRange(usernames.ToArray());
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            // Set onlineUsernames to the updated list of online users
            viewProfileButton.Enabled = false;
            onlineUsernames.Clear();
            UpdateOnlineUserList(onlineUsernames);
        }

        private void sendMatchRequestButton_Click(object sender, EventArgs e)
        {
            String username;
            if (onlineUsersListBox.SelectedItem != null)
            {
                username = onlineUsersListBox.SelectedItem.ToString();
            }
            else
            {
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
                        MessageBox.Show("Connection failed. Please try again.");
                        return;
                    }
                    MySqlDataAdapter da = new MySqlDataAdapter(new MySqlCommand("SELECT COUNT(*) FROM users WHERE username='" + username + "'", con));
                    DataTable loginTable = new DataTable();
                    da.Fill(loginTable);

                    if (loginTable.Rows[0][0].ToString() == "1")
                    {
                        MySqlCommand command = new MySqlCommand("SELECT * FROM users WHERE username = '" + username + "'", con);
                        MySqlDataReader dr = command.ExecuteReader();
                        if (dr.Read())
                        {
                            User viewUser = new User(dr[0].ToString(), dr[2].ToString(), Int32.Parse(dr[3].ToString()), Int32.Parse(dr[4].ToString()), Int32.Parse(dr[5].ToString()), dr[6].ToString(), dr[7].ToString());
                            ProfileForm profile_form = new ProfileForm(viewUser);

                            profile_form.ShowDialog();
                        }
                        dr.Close();
                        MySqlCommand onlinecommand = new MySqlCommand("UPDATE `login`.`users` SET `status` = 'Online' WHERE (`username` = '" + username + "');", con);
                        MySqlDataReader dr2 = onlinecommand.ExecuteReader();
                        con.Close();
                    }
                    con.Close();
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        private void onlineUsersListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // this is literally just to enable it once one of them is selected
            viewProfileButton.Enabled = true;
        }
    }
}
