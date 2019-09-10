using System;
using System.Collections.Generic;
using System.IO;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using APIProgressTracker.JSONObjects;

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

        public static object Init()
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
                return true;
            }
            catch(MySqlException e)
            {
                Console.WriteLine(e);
                return e;
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
        public static IEnumerable<KeyValuePair<int, Message>> GetMessages()
        {
            MySqlQuery msgs = SQLQuery("SELECT * FROM progresstracker");

            foreach (List<object> row in msgs.objects)
            {
                Message msg = JsonConvert.DeserializeObject<Message>(row[1].ToString());
                yield return new KeyValuePair<int, Message>(Convert.ToInt32(row[0]), msg);
            }
        }

        /// <summary>
        /// Adds a new message to the database
        /// </summary>
        /// <returns>True on success</returns>
        public static bool SendNewMessage(Message message)
        {
            var json = JsonConvert.SerializeObject(message);
            var comm = sqlConnection.CreateCommand();
            comm.CommandText = ("INSERT INTO progresstracker(`progress`, `last_modified`) VALUES(?json, ?lastmodified)");
            comm.Parameters.AddWithValue("?json", json);
            comm.Parameters.AddWithValue("?lastmodified", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            return comm.ExecuteNonQuery() > 0 ? true : false; // Return true if more than one row was affected. (It actually added the new row)
        }

        /// <summary>
        /// Deletes a message from the database
        /// </summary>
        /// <returns>True on success</returns>
        public static bool DeleteMessage(int id)
        {
            var comm = sqlConnection.CreateCommand();
            comm.CommandText = ("DELETE FROM `progresstracker` WHERE `id` = ?ID");
            comm.Parameters.AddWithValue("?ID", id);
            return comm.ExecuteNonQuery() > 0 ? true : false; // Return true if more than one row was affected. (It actually deleted the row)
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
