using System;
using System.Collections.Generic;
using System.Windows.Forms;
using APIProgressTracker.JSON;
using MySql.Data.MySqlClient;
using Newtonsoft;
using Newtonsoft.Json;

namespace APIProgressTracker
{
    public abstract class ProgressTrackerAPI
    {
        private const string SERVER = "remotemysql.com";
        private const string DATABASE = "TMrArME2SE";
        private const string UID = "TMrArME2SE";
        private const string PASSWORD = "kJTKB2snGQ";
        private const uint PORT = 3306;
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
            builder.Port = PORT;
            builder.SslMode = MySqlSslMode.None;

            string connString = builder.ToString();

            builder = null;

            try
            {
                MessageBox.Show("Connect");
                sqlConnection = new MySqlConnection(connString);
                sqlConnection.Open();
                MessageBox.Show("It should work");
                MySqlQuery test = SQLQuery("SELECT * FROM progresstracker");
                MessageBox.Show(test.success.ToString() + "\n" + test.objects[0][0] + "\n" + test.objects[0][1]);

            }
            catch(MySqlException e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        public static MySqlQuery SQLQuery(string query)
        {
            MySqlCommand mySqlCommand = new MySqlCommand(query, sqlConnection);
            var reader = mySqlCommand.ExecuteReader();

            List<List<object>> readObjs;
            readObjs = new List<List<object>>();

            bool success = false;

            while (reader.Read())
            {
                List<object> insertList = new List<object>();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    insertList.Add(reader.GetValue(i));
                }

                readObjs.Add(insertList);
                insertList = null;

            }
            success = reader.HasRows;

            reader.Close();

            return new MySqlQuery(readObjs, success);
        }

        public bool SQLExecute(string query)
        {
            MySqlCommand executeCmd = new MySqlCommand(query, sqlConnection);
            var executer = executeCmd.ExecuteNonQuery();

            return true;
        }

        #region Actual API calls
        public void SendNewMessage(JSON.Message message)
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

    public class MySqlQuery
    {
        public List<List<object>> objects;
        public bool success;

        public MySqlQuery(List<List<object>> listIn, bool status)
        {
            this.objects = listIn;
            this.success = status;
        }
    }
}
