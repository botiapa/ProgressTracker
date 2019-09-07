using APIProgressTracker.JSONObjects;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace APIProgressTracker.JSON
{
    public class Message
    {
        public string UID;
        public string Title;
        public string TextContents;
        public string Image;
        public short ProgressPercent;
        public Author Author;

        public Message(string uid, string title, string textcontent, string image, short progresspercent, Author author)
        {
            UID = uid;
            Title = title;
            TextContents = textcontent;
            Image = image;
            ProgressPercent = progresspercent;
            Author = author;
        }

        public Message(string uid, string title, string textcontent, short progresspercent, Author author)
        {
            UID = uid;
            Title = title;
            TextContents = textcontent;
            ProgressPercent = progresspercent;
            Author = author;
        }

        public Message(string title, string textcontent, short progresspercent, Author author)
        {
            UID = Guid.NewGuid().ToString();
            Title = title;
            TextContents = textcontent;
            ProgressPercent = progresspercent;
            Author = author;
        }

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
