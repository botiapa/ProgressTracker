﻿using System;
using System.Collections.Generic;
using System.IO;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using APIProgressTracker.JSONObjects;
using System.Security;
using System.Net.Http;
using System.Threading.Tasks;

namespace APIProgressTracker
{
    public static class ProgressTrackerAPI
    {
        static string server = "http://localhost:8000";
        const uint PORT = 8000;

        // URLS
        const string URL_GETMESSAGES = "/message";
        const string URL_CREATEMESSAGE = "/message/create";
        const string URL_UPDATEMESSAGE = "/message/update";
        const string URL_DELETEMESSAGE = "/message/delete";
        const string URL_ACCOUNTLOGIN = "/account/login";
        const string URL_ACCOUNTREGISTER = "/account/register";
        const string URL_ACCOUNTINFO = "/account/info";
        const string URL_ACCOUNTIMAGE= "/account/uploadimage";

        static readonly HttpClient client = new HttpClient();

        public static string hash;

        #region API Calls
        /// <returns>
        /// HASH or null if error
        /// </returns>
        public async static Task<string> LoginAsync(string username, string password)
        {
            var parameters = new Dictionary<string, string>() { { "username", username }, { "password", password } };
            var content = new FormUrlEncodedContent(parameters);
            var result = await client.PostAsync(server + URL_ACCOUNTLOGIN, content);
            if (result.IsSuccessStatusCode)
                return await result.Content.ReadAsStringAsync();
            return null;
        }

        
        public async static Task<List<Message>> GetMessages(DateTime latestTimestamp)
        {
            var result = await client.GetAsync(server + URL_GETMESSAGES);
            if (result.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<List<Message>>(await result.Content.ReadAsStringAsync());
            return null;
        }

        
        /// <summary>
        /// Adds a new message to the database
        /// </summary>
        /// <returns>True on success</returns>
        public async static Task<bool> SendNewMessage(string title, string contents = null, ushort progress = 0)
        {
            var parameters = new Dictionary<string, string>() {{ "hash", hash }, {"Title", title } };
            if (contents != null)
                parameters.Add("Contents", contents);
            if (progress != 0)
                parameters.Add("Progress", progress.ToString());
            var content = new FormUrlEncodedContent(parameters);
            var result = await client.PostAsync(server + URL_CREATEMESSAGE, content);
            return result.IsSuccessStatusCode;
        }

        /// <summary>
        /// Deletes a message from the database
        /// </summary>
        /// <returns>True on success</returns>
        public async static Task<bool> DeleteMessage(string id)
        {
            var parameters = new Dictionary<string, string>() { { "hash", hash }, { "ID", id } };
            var content = new FormUrlEncodedContent(parameters);
            var result = await client.PostAsync(server + URL_DELETEMESSAGE, content);
            return result.IsSuccessStatusCode;
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
