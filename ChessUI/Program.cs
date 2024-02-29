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
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
