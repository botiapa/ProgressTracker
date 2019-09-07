using APIProgressTracker.JSON;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace APIProgressTracker.JSONObjects
{
    public abstract class MessageJsonHelper
    {
        public static Message fromJson(string json)
        {
            return JsonConvert.DeserializeObject<Message>(json);
        }

        public static string toJson(Message message)
        {
            return JsonConvert.SerializeObject(message);
        }
    }
}
