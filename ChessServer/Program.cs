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

            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ip), port);
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
                }
                else
                {
                    username = username.Replace("\0", "");
                    Console.WriteLine(username + " connected");
                    UserSocket user = new UserSocket(username, ClientSocket);
                    connectionDict.Add(username, user);
                }
            }
        }
        void sendRequest(string receiverUsername, string senderUsername)
        {
            string receiveUser = receiverUsername.Replace("\0", "");
            string sendUser = senderUsername.Replace("\0", "");

            Socket s = connectionDict[receiveUser].getSocket();

            string msg = (sendUser + " would like to play a chess game with you.");
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
                        Console.WriteLine(s.getUsername() + " Connected!");
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