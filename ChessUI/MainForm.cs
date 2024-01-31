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
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void loginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoginForm login_form = new LoginForm();

            if (login_form.ShowDialog() == DialogResult.OK)
            {
                // The user has successfully logged in
                if (Program.currentUser.getStatus() != "Online")
                {
                    return;
                }

                // UI Changes after Successful Login
                loginToolStripMenuItem.Visible = false; // Login Invisible
                registerToolStripMenuItem.Visible = false; // Register Invisible
                findMatchToolStripMenuItem.Visible = true; // Play Online Visible
                editOrViewProfileToolStripMenuItem.Visible = true; // Edit/View Profile Visible
            }
        }

        private void findMatchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FindMatchForm find_match_form = new FindMatchForm();

            if (find_match_form.ShowDialog() == DialogResult.OK)
            {
                /*
                if (!find_match_form.foundMatch)
                {
                    return;
                }
                */
                // Match was actually found

                // TODO (all): Game starting sequence here this is the actual hard part of the project lol
            }
        }

        private void editOrViewProfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Open a new form where you can view and edit your profile, don't know if we need anything other than a username and password but for more points in this class (I want an A) maybe we should tryhard
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}