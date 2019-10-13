using APIProgressTracker.JSONObjects;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace APIProgressTracker.JSONObjects
{
    public class Message
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string Contents { get; set; }
        public short Progress { get; set; }
        public Author Author { get; set; }
        public DateTime LastModified { get; set; }
    }
}
