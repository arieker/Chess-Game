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
            onlineUsernames.Clear();
            UpdateOnlineUserList(onlineUsernames);
        }

        private void sendMatchRequestButton_Click(object sender, EventArgs e)
        {
            // open a view profile form with the information of the person that is selected in the listbox with this line or some other method
            Debug.WriteLine(onlineUsersListBox.SelectedItem.ToString());
        }

        private void onlineUsersListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // this is literally just to enable it once one of them is selected
            viewProfileButton.Enabled = true;
        }
    }
}
