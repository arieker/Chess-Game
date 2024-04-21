using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

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
                viewLastPlayedGameToolStripMenuItem.Visible = true; // View Last Played Game Visible
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
                viewLastPlayedGameToolStripMenuItem.Visible = true; // View Last Played Game Visible
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
            ProfileForm profile_form = new ProfileForm(Program.currentUser);

            profile_form.ShowDialog();
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
            viewLastPlayedGameToolStripMenuItem.Visible = false; // View Last Played Game Invisible
            editOrViewProfileToolStripMenuItem.Visible = false; // Edit/View Profile Invisible
            logoutToolStripMenuItem.Visible = false; // Logout Invisible
        }

        // Temporary function and button, this should be deleted in the future, or not merged with master
        private void openBoardButton_Click(object sender, EventArgs e)
        {
            ChessBoardForm chessboard_form = new ChessBoardForm();

            chessboard_form.ShowDialog();
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.wikihow.com/Play-Chess");
        }

        private void viewLastPlayedGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string gameString = "";
                MySqlConnection con;

                using (con = new MySqlConnection())
                {
                    // I'm assuming this entire code will be rewritten in the future so just keep this in mind:

                    // If username is unique, then take whatever password, and register the user as a new account
                    // If username is not unique say an account already exists, password is invalid.
                    // In a dream world, you can tell the user that the username is unique as they type it like a real website. If this is possible that'd be cool :) but this form is your baby, I won't touch it too much
                    // p.s. make sure u limit the length of usernames to something respectable and don't let them use characters that might cause issues like 漢字 even tho they prob wont cause issues
                    con.ConnectionString = ConfigurationManager.ConnectionStrings["users"].ConnectionString;
                    try
                    {
                        con.Open();
                    }
                    catch
                    {

                    }

                    MySqlCommand command = new MySqlCommand("SELECT * FROM users WHERE username = '" + Program.currentUser.getUsername() + "'", con);
                    MySqlDataReader dr = command.ExecuteReader();
                    if (dr.Read())
                    {
                        gameString = dr[8].ToString();
                    }
                    con.Close();
                    MessageBox.Show(gameString, "Most Recent Game: ");
                }
            }
            catch (SqlException er)
            {
                Console.WriteLine(er.ToString());
            }
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            Image kingblack = Image.FromFile(@"../../Assets\king_black.png");
            Image queenblack = Image.FromFile(@"../../Assets\queen_black.png");
            Image kingwhite = Image.FromFile(@"../../Assets\king_white.png");
            Image queenwhite = Image.FromFile(@"../../Assets\queen_white.png");
            Graphics g = e.Graphics;
            Brush blueBrush = Brushes.CornflowerBlue;
            Brush whiteBrush = Brushes.GhostWhite;
            g.FillRectangle(blueBrush, 0, 20, 200, 200);
            g.FillRectangle(whiteBrush, 0, 220, 200, 200);
            g.FillRectangle(whiteBrush, 200, 20, 200, 200);
            g.FillRectangle(blueBrush, 200, 220, 200, 200);
            g.DrawImage(kingblack, new Rectangle(0, 20, 200, 200));
            g.DrawImage(queenwhite, new Rectangle(0, 220, 200, 200));
            g.DrawImage(queenblack, new Rectangle(200, 20, 200, 200));
            g.DrawImage(kingwhite, new Rectangle(200, 220, 200, 200));
        }
    }
}