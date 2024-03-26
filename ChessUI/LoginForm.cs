﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql;
using MySql.Data.MySqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Threading;

namespace ChessUI
{
    public partial class LoginForm : Form
    {


        public LoginForm()
        {
            InitializeComponent();
            okButton.Enabled = false;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        static bool Login(System.Windows.Forms.TextBox user, System.Windows.Forms.TextBox pass)
        {
            try
            {
                string username = user.Text;
                string password = pass.Text.GetHashCode().ToString();
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
                        MessageBox.Show("Login connection failed. Please try again.");
                        return false;
                    }
                    MySqlDataAdapter da = new MySqlDataAdapter(new MySqlCommand("SELECT COUNT(*) FROM users WHERE username='" + username + "' AND password='" + password + "'", con));
                    DataTable loginTable = new DataTable();
                    da.Fill(loginTable);

                    // checks if username and password holds a valid entry
                    if (loginTable.Rows[0][0].ToString() == "1")
                    {

                        // set currentuser static variable
                        MySqlCommand command = new MySqlCommand("SELECT * FROM users WHERE username = '" + username + "'", con);
                        MySqlDataReader dr = command.ExecuteReader();
                        if (dr.Read())
                        {
                            if (dr[7].ToString() == "Online")
                            {
                                MessageBox.Show("This user is already logged in.");
                                return false;
                            }
                            User currentUser = new User(dr[0].ToString(), dr[2].ToString(), Int32.Parse(dr[3].ToString()), Int32.Parse(dr[4].ToString()), Int32.Parse(dr[5].ToString()), dr[6].ToString(), dr[7].ToString());
                            Program.currentUser = currentUser;
                        }
                        dr.Close();
                        MySqlCommand onlinecommand = new MySqlCommand("UPDATE `login`.`users` SET `status` = 'Online' WHERE (`username` = '" + username + "');", con);
                        MySqlDataReader dr2 = onlinecommand.ExecuteReader();
                        Program.currentUser.setStatus("Online");
                        MessageBox.Show("Logged in Successfully");
                        Console.Out.WriteLine("Logged in Successfully");
                        con.Close();

                        //establish connection to server
                        int port = 31415;
                        string ip = "127.0.0.1";
                        Socket ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ip), port);
                        ClientSocket.Connect(ep);
                        string serveruser = Program.currentUser.getUsername();
                        Program.currentSocket = ClientSocket;
                        Program.currentSocket.Send(System.Text.Encoding.ASCII.GetBytes(serveruser), 0, serveruser.Length, SocketFlags.None);

                        
                        Program.awaitThread.Start();
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("Invalid username or password");
                        pass.Clear();
                    }
                    con.Close();
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }

            return false; // Catch
        }
        private void okButton_Click(object sender, EventArgs e)
        {
            if (Login(usernameTextBox, passwordTextBox))
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void textBoxes_TextChanged(object sender, EventArgs e)
        {
            if (usernameTextBox.Text.Length > 0 && passwordTextBox.Text.Length > 0)
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
