using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace ChessUI
{
    public partial class ProfileForm : Form
    {
        String username = "";
        public ProfileForm(User user)
        {
            Debug.Write(user.getWins());
            Debug.Write(user.getLosses());
            Debug.Write(""+ user.getDraws());
            InitializeComponent();


            string username = user.getUsername();

            nicknameTextLabel.Text = user.getNickname();
            usernameTextLabel.Text = user.getUsername();

            openingDateTextLabel.Text = user.getDate();
            if (Program.currentUser == user)
            {
                changeNicknameButton.Enabled = true;
            }
            else
            {
                sendMatchRequestButton.Enabled = true;
                sendMatchRequestButton.Visible = true;
                changeNicknameButton.Visible = false;
            }

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

                    // set currentuser static variable
                    MySqlCommand command = new MySqlCommand("SELECT * FROM users WHERE username = '" + username + "'", con);
                    MySqlDataReader dr = command.ExecuteReader();
                    if (dr.Read())
                    {
                        dataGridView1.Rows[0].Cells[0].Value = Int32.Parse(dr[3].ToString());
                        dataGridView1.Rows[0].Cells[1].Value = Int32.Parse(dr[4].ToString());
                        dataGridView1.Rows[0].Cells[2].Value = Int32.Parse(dr[5].ToString());
                    }
                    dr.Close();
                }
                con.Close();
            }


        }

        private void changeNicknameButton_Click(object sender, EventArgs e)
        {
            ChangeNicknameForm change_nickname_form = new ChangeNicknameForm();

            change_nickname_form.ShowDialog();
            nicknameTextLabel.Text = Program.currentUser.getNickname();
        }

        private void sendMatchRequestButton_Click(object sender, EventArgs e)
        {
            int port = 31415;
            string ip = "44.221.170.210";
            Socket ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ip), port);
            ClientSocket.Connect(ep); 
            string request = "sendRequest " + usernameTextLabel.Text + " " + Program.currentUser.getUsername();
            Console.Out.WriteLine(request);
            ClientSocket.Send(System.Text.Encoding.ASCII.GetBytes(request), 0, request.Length, SocketFlags.None);

        }
    }
}
