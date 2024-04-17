using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mysqlx.Crud;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using Org.BouncyCastle.Cms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Drawing;


namespace ChessUI
{
    public class User
    {
        string username;
        string nickname;
        int wins;
        int losses;
        int draws;
        string status;
        string opendate;
        public User(string username, string nickname, int wins, int losses, int draws, string opendate, string status)
        {
            this.username = username;
            this.nickname = nickname;
            this.wins = wins;
            this.losses = losses;
            this.draws = draws;
            this.status = status;
            this.opendate = opendate;
        }

        public void Login()
        {
            try
            {
                MySqlConnection con;

                using (con = new MySqlConnection())
                {
                    con.ConnectionString = ConfigurationManager.ConnectionStrings["users"].ConnectionString;
                    con.Open();
                    MySqlCommand command = new MySqlCommand("UPDATE `login`.`users` SET `status` = 'Online' WHERE(`username` = '" + this.username + "');", con);
                    command.ExecuteReader();

                    con.Close();
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void Logout()
        {
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
                        return;
                    }
                    MySqlCommand command = new MySqlCommand("UPDATE `login`.`users` SET `status` = 'Offline' WHERE(`username` = '" + this.username + "');", con);
                    command.ExecuteReader();    

                    con.Close();
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        void Register()
        {

        }

        public string getUsername()
        {
            return this.username;
        }

        public string getNickname()
        {
            return this.nickname;
        }

        public int getWins()
        {
            return this.wins;
        }

        public int getLosses()
        {
            return this.losses;
        }

        public int getDraws()
        {
            return this.draws;
        }

        public string getStatus()
        {
            return this.status;
        }

        public string getDate()
        {
            return this.opendate;
        }

        public void setNickname(string name)
        {
            this.nickname = name;
        }

        public void setStatus(string status)
        {
            this.status = status;
        }

    }

    internal static class Program
    {
        public static User currentUser = new User("Guest", "Guest", 0, 0, 0, DateTime.Today.ToString(), "Online");
        public static Socket currentSocket = null;
        public static Thread awaitThread = new Thread(new ThreadStart(() => Program.Await()));
        public static Thread gameThread = new Thread(new ThreadStart(() => Program.doGame()));
        public static ChessBoardForm chessboard_form;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            chessboard_form = new ChessBoardForm();
            Application.Run(new MainForm());
            awaitThread.Abort();
            if (currentSocket != null)
            {
                currentSocket.Close();
            }
        }

        public static void Await()
        {
            while (true)
            {
                int size = 0;
                byte[] serverMsg = new byte[1024];
                try
                {
                    size = Program.currentSocket.Receive(serverMsg);
                }
                catch (SocketException e) 
                {
                    Console.WriteLine("Connection aborted");
                    return;
                }

                string challengeUser = System.Text.Encoding.ASCII.GetString(serverMsg, 0, size);
                if (challengeUser.Length >= 11 && challengeUser.Substring(0, 11) == "sendRequest")
                {
                    string[] inputs = challengeUser.Split(' ');
                    DialogResult result = MessageBox.Show(inputs[1] + " would like to play a chess game with you. (request received by " + Program.currentUser.getUsername() + ")", "Game Request", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        int port = 31415;
                        string ip = "127.0.0.1";
                        Socket ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ip), port);
                        ClientSocket.Connect(ep);
                        string request = "startGame " + Program.currentUser.getUsername() + " " + challengeUser.Substring(12);
                        Console.Out.WriteLine(request);
                        ClientSocket.Send(System.Text.Encoding.ASCII.GetBytes(request), 0, request.Length, SocketFlags.None);

                        // open the game board
                        gameThread.Start();
                        chessboard_form.ShowDialog();

                    }
                    else if (result == DialogResult.No)
                    {
                        // make a game not happen
                    }
                }
                if (challengeUser.Length >= 9 && challengeUser.Substring(0, 9) == "openBoard")
                {
                    gameThread.Start();
                    chessboard_form.ShowDialog();
                }
            }
        }

        public static void doGame()
        {
            while (true)
            {
                int size = 0;
                byte[] serverMsg = new byte[1024];
                try
                {
                    size = Program.currentSocket.Receive(serverMsg);
                }
                catch (SocketException e)
                {
                    Console.WriteLine("Connection aborted");
                    return;
                }

                string challengeUser = System.Text.Encoding.ASCII.GetString(serverMsg, 0, size);
                if (challengeUser.Length >= 4 && challengeUser.Substring(0, 4) == "move")
                {
                    // this event means an enemy move happened. we see a "move" at the beginning, and the next 
                    // four elements move an enemy piece on the board.
                    string[] inputs = challengeUser.Split(' ');

                    int x1 = Int32.Parse(inputs[1]);
                    int y1 = Int32.Parse(inputs[2]);
                    int x2 = Int32.Parse(inputs[3]);
                    int y2 = Int32.Parse(inputs[4]);

                    chessboard_form.movePiece_visual(x1, y1, x2, y2);
                    chessboard_form.boardLogic.move(y1, 7 - x1, y2, 7 - x2);
                }
            }
        }
    }
}