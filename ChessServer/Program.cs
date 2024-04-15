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
        static void Main(String[] args)
        {
            Program p = new Program();
            p.checkConnections();
            int port = 31415;
            String ip  = "127.0.0.1";
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint ep = new IPEndPoint(IPAddress.Any, port);
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
                // this means a player moved, and sent that info to the server. what we should do here is send "move x1 y1 x2 y2" to the client that did NOT send it. 
                if (username.Length >= 4 && username.Substring(0, 4) == "move")
                {

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

        public async void startGame(string player1, string player2)
        {
            Console.Out.WriteLine("Starting game between " + player1 + " " + player2);
            string p1 = player1.Replace("\0", String.Empty);
            string p2 = player2.Replace("\0", String.Empty);

            Socket s1 = connectionDict[p1].getSocket();
            Socket s2 = connectionDict[p2].getSocket();

            // opens sender console
            string command = "openBoard";
            s2.Send(System.Text.Encoding.ASCII.GetBytes(command), 0, command.Length, SocketFlags.None);
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