using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace APIProgressTracker
{
    public static class ProgressTrackerAPI
    {
        #region Init process
        private const string SERVER = "remotemysql.com";
        private const string DATABASE = "TMrArME2SE";
        private const string UID = "TMrArME2SE";
        private const string PASSWORD = "kJTKB2snGQ";
        private const uint PORT = 3306;
        public static MySqlConnection sqlConnection;

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
                sqlConnection = new MySqlConnection(connString);
                sqlConnection.Open();
                Console.WriteLine("Connected and opened connection");

            }
            catch(MySqlException e)
            {
                Console.WriteLine(e);
            }
        }
        #endregion

        #region MySql thingies
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

        public static bool SQLExecute(string query)
        {
            MySqlCommand executeCmd = new MySqlCommand(query, sqlConnection);
            var executer = executeCmd.ExecuteNonQuery();

            return true;
        }
        #endregion

        #region Actual API calls
        public static void SendNewMessage(JSON.Message message)
        {
            var obj = JsonConvert.SerializeObject(message);
        }
        #endregion

        #region Base64 decoder
        public static BitmapImage Base64StringToBitmap(string base64String)
        {
            byte[] imgBytes = Convert.FromBase64String(base64String);

            BitmapImage bitmapImage = new BitmapImage();
            MemoryStream ms = new MemoryStream(imgBytes);
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = ms;
            bitmapImage.EndInit();

            //Image object
            return bitmapImage;
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
