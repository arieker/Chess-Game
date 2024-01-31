using MySql.Data.MySqlClient;
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

// PLEASE READ
// Try to name all forms like so MainForm, LoginForm, ChessBoardForm
// Name every object of forms in lowercase with _ before "form" login_form, chessboard_form
// This variable naming makes it extremely easy to differentiate quickly, and is very good practice

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
                logoutToolStripMenuItem.Visible = true; // Logout Visible
            }
        }

        private void registerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RegisterForm register_form = new RegisterForm();

            if (register_form.ShowDialog() == DialogResult.OK)
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
                logoutToolStripMenuItem.Visible = true; // Logout Visible
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

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Program.currentUser.Logout();
        }

        private void logoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.currentUser.Logout();
            MessageBox.Show("Successfully logged out");

            // UI Changes after Successful Logout
            loginToolStripMenuItem.Visible = true; // Login Visible
            registerToolStripMenuItem.Visible = true; // Register Visible
            findMatchToolStripMenuItem.Visible = false; // Play Online Invisible
            editOrViewProfileToolStripMenuItem.Visible = false; // Edit/View Profile Invisible
            logoutToolStripMenuItem.Visible = false; // Logout Invisible
        }

        // Temporary function and button, this should be deleted in the future, or not merged with master
        private void openBoardButton_Click(object sender, EventArgs e)
        {
            ChessBoardForm chessboard_form = new ChessBoardForm();

            chessboard_form.ShowDialog();
        }
    }
}