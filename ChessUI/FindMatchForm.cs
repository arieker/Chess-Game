using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessUI
{
    public partial class FindMatchForm : Form
    {
        // In future remove these names, it's for testing, or just keep them here and rewrite the entire list every update (it should update on open)
        List<string> onlineUsernames = new List<string>
        {
            "Nayeon",
            "Jeongyeon",
            "Momo",
            "Sana",
            "Jihyo",
            "Mina",
            "Dahyun",
            "Chaeyoung",
            "Tzuyu"
        };

        public FindMatchForm()
        {
            InitializeComponent();
            // Set onlineUsernames to the updated list of online users
            UpdateOnlineUserList(onlineUsernames); // Init the listbox with values
        }
        public void UpdateOnlineUserList(List<string> usernames)
        {
            onlineUsersListBox.Items.Clear();
            onlineUsersListBox.Items.AddRange(usernames.ToArray());
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            // Set onlineUsernames to the updated list of online users
            UpdateOnlineUserList(onlineUsernames);
        }

        private void sendMatchRequestButton_Click(object sender, EventArgs e)
        {
            // Code to send match request to person who was selected inside of the listbox
            // Make sure to check that they are online
            // This might get buried, but make sure being "online" doesn't just mean that they are online, but not inside of a match. We can simplify things by making being in a match = offline
        }
    }
}
