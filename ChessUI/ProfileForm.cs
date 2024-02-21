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
    public partial class ProfileForm : Form
    {
        public ProfileForm(User user)
        {
            InitializeComponent();
            dataGridView1.ReadOnly = true;
            usernameTextBox.Enabled = false;
            openingDateTextBox.Enabled = false;
            usernameTextBox.SelectedText = user.getUsername();
            openingDateTextBox.SelectedText = user.getDate();
            if (Program.currentUser == user)
            {
                // make the edit nickname button visible
            }
        }
    }
}
