using System;
using APIProgressTracker.JSON;
using MySql.Data.MySqlClient;
using Newtonsoft;
using Newtonsoft.Json;

namespace APIProgressTracker
{
    public abstract class ProgressTrackerAPI
    {
        private const string SERVER = "remotemysql.com";
        private const string DATABASE = "progresstracker";
        private const string UID = "TMrArME2SE";
        private const string PASSWORD = "kJTKB2snGQ";
        public static MySqlConnection sqlConnection;

        ProgressTrackerAPI()
        {

        }

        public static void Init()
        {
            //TODO: Connect
            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder();
            builder.Server = SERVER;
            builder.UserID = UID;
            builder.Password = PASSWORD;
            builder.Database = DATABASE;

            string connString = builder.ToString();

            builder = null;

            try
            {
                Console.WriteLine("Connect");
                sqlConnection = new MySqlConnection(connString);
                Console.WriteLine("It should work");
            }
            catch(MySqlException e)
            {
                Console.WriteLine(e);
            }
        }
    
        #region Actual API calls
        public void SendNewMessage(Message message)
        {
            var obj = JsonConvert.SerializeObject(message);
            string buzievagy = "Perhaps";
            switch(buzievagy)
            {
                case "MEHET MINDED":
                    Console.Write("yeees");
                    break;

                case "No Homo Erectus":
                    Console.Write("sad boy");
                    break;
            }
        }
        #endregion
    }
}
