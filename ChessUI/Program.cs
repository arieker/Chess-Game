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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using System.Runtime.InteropServices;


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
        public void addLoss()
        {
            this.losses++;
        }

        public void addDraw()
        {
            this.draws++;
        }

        public void addWin()
        {
            this.losses++;
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
                        string ip = "44.221.170.210";
                        Socket ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ip), port);
                        ClientSocket.Connect(ep);
                        string request = "startGame " + Program.currentUser.getUsername() + " " + challengeUser.Substring(12);
                        Console.Out.WriteLine(request);
                        ClientSocket.Send(System.Text.Encoding.ASCII.GetBytes(request), 0, request.Length, SocketFlags.None);

                        // open the game board
                        chessboard_form = new ChessBoardForm();
                        gameThread = new Thread(new ThreadStart(() => Program.doGame()));
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
                    chessboard_form = new ChessBoardForm();
                    gameThread = new Thread(new ThreadStart(() => Program.doGame()));
                    gameThread.Start();
                    chessboard_form.ShowDialog();
                }
            }
        }

        public static void doGame()
        {
            bool gameOver = false;
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
                        string ip = "44.221.170.210";
                        Socket ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ip), port);
                        ClientSocket.Connect(ep);
                        string request = "startGame " + Program.currentUser.getUsername() + " " + challengeUser.Substring(12);
                        Console.Out.WriteLine(request);
                        ClientSocket.Send(System.Text.Encoding.ASCII.GetBytes(request), 0, request.Length, SocketFlags.None);

                        // open the game board
                        chessboard_form = new ChessBoardForm();

                        chessboard_form.ShowDialog();

                    }
                    else if (result == DialogResult.No)
                    {
                        // make a game not happen
                    }
                }
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

                    Winner check = chessboard_form.boardLogic.winner();

                    if (check != Winner.NoneYet)
                    {
                        // this means that this player lost

                        int port = 31415;
                        string ip = "44.221.170.210";
                        Socket cs = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ip), port);
                        cs.Connect(ep);
                        string request = "end " + Program.currentUser.getUsername();
                        cs.Send(System.Text.Encoding.ASCII.GetBytes(request), 0, request.Length, SocketFlags.None);
                    }
                }
                if (challengeUser.Length >= 9 && challengeUser.Substring(0, 9) == "openBoard")
                {
                    chessboard_form = new ChessBoardForm();
                    chessboard_form.ShowDialog();
                }
                if (challengeUser.Length >= 7 && challengeUser.Substring(0, 7) == "drawEnd")
                {
                    MessageBox.Show("Draw Accepted. Ending Game");
                    try
                    {
                        string update = chessboard_form.boardLogic.getAllMoves();
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
                            MySqlCommand onlinecommand = new MySqlCommand("UPDATE `login`.`users` SET `recent` = '" + update + "' WHERE (`username` = '" + Program.currentUser.getUsername() + "');", con);
                            MySqlDataReader dr2 = onlinecommand.ExecuteReader();
                            con.Close();
                        }
                    }
                    catch (SqlException er)
                    {
                        Console.WriteLine(er.ToString());
                    }
                }
                if (challengeUser.Length >= 4 && challengeUser.Substring(0, 4) == "draw" && challengeUser != "drawAccept" && challengeUser != "drawEnd")
                {
                    DialogResult result = MessageBox.Show("A draw has been offered. Accept?","Game Request", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        int port = 31415;
                        string ip = "44.221.170.210";
                        Socket ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ip), port);
                        ClientSocket.Connect(ep);
                        string request = "drawAccept " + Program.currentUser.getUsername();
                        Program.currentUser.addDraw();
                        ClientSocket.Send(System.Text.Encoding.ASCII.GetBytes(request), 0, request.Length, SocketFlags.None);
                        try
                        {
                            string update = chessboard_form.boardLogic.getAllMoves();
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
                                MySqlCommand onlinecommand = new MySqlCommand("UPDATE `login`.`users` SET `recent` = '" + update + "' WHERE (`username` = '" + Program.currentUser.getUsername() + "');", con);
                                MySqlDataReader dr2 = onlinecommand.ExecuteReader();
                                con.Close();
                            }
                        }
                        catch (SqlException er)
                        {
                            Console.WriteLine(er.ToString());
                        }
                    }
                    else if (result == DialogResult.No)
                    {
                        
                    }
                    
                    //gameThread.Abort();
                    break;
                }
               
                    if (challengeUser.Length >= 3 && challengeUser.Substring(0, 3) == "end")
                    {
                        string[] inputs = challengeUser.Split(' ');
                        string loser = inputs[1];
                        loser = loser.Replace("\0", String.Empty);

                        chessboard_form.gameTimer.Stop();
                        if (chessboard_form.boardLogic.winner() == Winner.NoneYet)
                        {
                            MessageBox.Show(loser + " Forfeits");
                        }
                        else
                        {
                             MessageBox.Show(chessboard_form.boardLogic.winner() + " Wins!");
                        }

                    string update = chessboard_form.boardLogic.getAllMoves();
                    // this account lost
                    if (loser == Program.currentUser.getUsername()) 
                    {
                        int losses = 0;
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
                                if (!gameOver)
                                {
                                    gameOver = true;
                                    MySqlDataAdapter da = new MySqlDataAdapter(new MySqlCommand("SELECT COUNT(*) FROM users WHERE username='" + Program.currentUser.getUsername() + "'", con));
                                    DataTable loginTable = new DataTable();
                                    da.Fill(loginTable);

                                    if (loginTable.Rows[0][0].ToString() == "1")
                                    {

                                        // set currentuser static variable
                                        MySqlCommand command = new MySqlCommand("SELECT * FROM users WHERE username = '" + loser + "'", con);
                                        MySqlDataReader dr = command.ExecuteReader();
                                        if (dr.Read())
                                        {
                                            losses = Int32.Parse(dr[4].ToString());
                                        }
                                        dr.Close();
                                    }
                                    MySqlCommand onlinecommand = new MySqlCommand("UPDATE `login`.`users` SET `losses` = '" + (losses+1) + "' WHERE (`username` = '" + Program.currentUser.getUsername() + "');", con);
                                    Program.currentUser.addLoss();
                                    MySqlDataReader dr2 = onlinecommand.ExecuteReader();
                                }
                                con.Close();
                            }
                        }
                        catch (SqlException er)
                        {
                            Console.WriteLine(er.ToString());
                        }
                    }
                    // this account won
                    else
                    {
                        int wins = 0;
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
     
                                }
                                MySqlCommand command = new MySqlCommand("SELECT * FROM users WHERE username = '" + Program.currentUser.getUsername() + "'", con);
                                MySqlDataReader dr = command.ExecuteReader();
                                if (dr.Read())
                                {
                                    wins = Int32.Parse(dr[3].ToString());

                                }
                                dr.Close();
                                MySqlCommand onlinecommand = new MySqlCommand("UPDATE `login`.`users` SET `wins` = '" + (wins + 1) + "' WHERE (`username` = '" + Program.currentUser.getUsername() + "');", con);
                                MySqlDataReader dr2 = onlinecommand.ExecuteReader();
                                con.Close();
                            }
                        }
                        catch (SqlException er)
                        {
                            Console.WriteLine(er.ToString());
                        }
                    }
                    //updates string to database
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
                            MySqlCommand onlinecommand = new MySqlCommand("UPDATE `login`.`users` SET `recent` = '" + update + "' WHERE (`username` = '" + Program.currentUser.getUsername() + "');", con);
                            MySqlDataReader dr2 = onlinecommand.ExecuteReader();
                            con.Close();
                        }
                    }
                    catch (SqlException er)
                    {
                        Console.WriteLine(er.ToString());
                    }
                    //gameThread.Abort();
                }
            }
        }
    }
}