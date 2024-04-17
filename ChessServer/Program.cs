using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Configuration;
using MySqlX.XDevAPI;
using System.Media;


namespace ChessServer
{
    class UserSocket
    {
        string username;
        Socket socket;

        public UserSocket(string username, Socket socket)
        {
            this.username = username;
            this.socket = socket;
        }

        public Socket getSocket()
        {
            return this.socket;
        }

        public string getUsername() 
        {
            return this.username;
        }
    }
    class Program
    {
        static Dictionary<string, UserSocket> connectionDict = new Dictionary<string, UserSocket>();
        static int port = 31415;
        static String ip = "127.0.0.1";
        static Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static string player1 = "";
        static string player2 = "";

        static IPEndPoint ep = new IPEndPoint(IPAddress.Any, port);
        static void Main(String[] args)
        {
            Program p = new Program();
            p.checkConnections();

            listener.Bind(ep);
            listener.Listen(100);
            Console.WriteLine("ChessServer is listening");
            // counts how many clients are connected
            while (true)
            {
                Socket ClientSocket = listener.Accept();
                byte[] msg = new byte[1024];
                ClientSocket.Receive(msg);
                string username = Encoding.UTF8.GetString(msg);
                if (username.Length >= 11 && username.Substring(0,11) == "sendRequest")
                {
                    Console.WriteLine("Request received.");
                    string[] inputs = username.Split(' ');
                    p.sendRequest(inputs[1], inputs[2]);
                    continue;
                }
                if (username.Length >= 9 && username.Substring(0, 9) == "startGame")
                {
                    Console.WriteLine("Request received.");
                    string[] inputs = username.Split(' ');
                  
                    p.startGame(inputs[1], inputs[2]);
                    continue;
                }
                if (username.Length >= 4 && username.Substring(0, 4) == "move")
                {
                    // this event means an enemy move happened. we see a "move" at the beginning, and the next 
                    // four elements move an enemy piece on the board.
                    Console.WriteLine(username);
                    string[] inputs = username.Split(' ');

                    string user = inputs[1];
                    if (user == player1)
                    {
                        user = player2;
                    }
                    else
                    {
                        user = player1;
                    }
                    user = user.Replace("\0", String.Empty);

                    int x1 = Int32.Parse(inputs[2]);
                    int y1 = Int32.Parse(inputs[3]);
                    int x2 = Int32.Parse(inputs[4]);
                    int y2 = Int32.Parse(inputs[5]);

                    Socket socket = connectionDict[user].getSocket();

                    string cmd = "move " + x1 + " " + y1 + " " + x2 + " " + y2;
                    socket.Send(System.Text.Encoding.ASCII.GetBytes(cmd), 0, cmd.Length, SocketFlags.None);
                    continue;
                }
                else
                {

                    username = username.Replace("\0", String.Empty);
                    username = username.Replace("\n", String.Empty);
                    username = username.Replace("\n", String.Empty);
                    username = username.Replace("\r", String.Empty);
                    username = username.Replace("\t", String.Empty);

                    Console.WriteLine(username + " connected");
                    UserSocket user = new UserSocket(username, ClientSocket);
                    connectionDict.Add(username, user);
                }
            }
        }

        public void startGame(string player1, string player2)
        {
            Program.player1 = player1;
            Program.player2 = player2;

            Console.Out.WriteLine("Starting game between " + player1 + " " + player2);
            string p1 = player1.Replace("\0", String.Empty);
            string p2 = player2.Replace("\0", String.Empty);

            Socket s1 = connectionDict[p1].getSocket();
            Socket s2 = connectionDict[p2].getSocket();

            // opens sender console
            string command = "openBoard";
            s2.Send(System.Text.Encoding.ASCII.GetBytes(command), 0, command.Length, SocketFlags.None);

            Thread gameThread = new Thread(new ThreadStart(() => Program.gameLoop(p1, p2)));
            gameThread.Start();
        }
        

        public static void gameLoop(string p1, string p2)
        {

            // counts how many clients are connected
            while (true)
            {
                Socket ClientSocket = listener.Accept();
                byte[] msg = new byte[1024];
                ClientSocket.Receive(msg);
                string username = Encoding.UTF8.GetString(msg);

                // this means a player moved, and sent that info to the server. what we should do here is send "move x1 y1 x2 y2" to the client that did NOT send it. 
                if (username.Length >= 4 && username.Substring(0, 4) == "move")
                {
                    // this event means an enemy move happened. we see a "move" at the beginning, and the next 
                    // four elements move an enemy piece on the board.
                    Console.WriteLine(username);
                    string[] inputs = username.Split(' ');

                    string user = inputs[1];
                    if (user == p1)
                    {
                        user = p2;
                    }
                    else
                    {
                        user = p1;
                    }
                    user = user.Replace("\0", String.Empty);

                    int x1 = Int32.Parse(inputs[2]);
                    int y1 = Int32.Parse(inputs[3]);
                    int x2 = Int32.Parse(inputs[4]);
                    int y2 = Int32.Parse(inputs[5]);

                    Socket socket = connectionDict[user].getSocket();

                    string cmd = "move " + x1 + " " + y1 + " " + x2 + " " + y2;
                    socket.Send(System.Text.Encoding.ASCII.GetBytes(cmd), 0, cmd.Length, SocketFlags.None);
                }
            }
        }

        void sendRequest(string receiverUsername, string senderUsername)
        {
            string receiveUser = receiverUsername.Replace("\0", "");
            string sendUser = senderUsername.Replace("\0", "");

            Socket s = connectionDict[receiveUser].getSocket();

            string msg = ("sendRequest " + sendUser);
            s.Send(System.Text.Encoding.ASCII.GetBytes(msg), 0, msg.Length, SocketFlags.None);
        }
        public async Task checkConnections()
        {
            while (true)
            {
                var delay = Task.Delay(1000);
                foreach (UserSocket s in connectionDict.Values)
                {
                    Socket client = s.getSocket();
                    try
                    {
                        byte[] tmp = new byte[1];

                        client.Blocking = false;
                        client.Send(tmp, 0, 0);
                    }
                    catch (SocketException e)
                    {
                        if (e.NativeErrorCode.Equals(10035))
                        {
                            Console.WriteLine("Still Connected, but the Send would block");
                        }
                        else
                        {
                            Console.WriteLine(s.getUsername() + " disconnected.");
                            connectionDict.Remove(s.getUsername());
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
                                    MySqlCommand command = new MySqlCommand("UPDATE `login`.`users` SET `status` = 'Offline' WHERE(`username` = '" + s.getUsername + "');", con);
                                    command.ExecuteReader();

                                    con.Close();
                                }
                            }
                            catch (MySqlException ex)
                            {
                                Console.WriteLine(ex.ToString());
                            }
                        }
                    }
                }
                await (delay);
            }
        }
    }
}