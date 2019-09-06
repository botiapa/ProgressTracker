using APIProgressTracker.JSONObjects;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace APIProgressTracker.JSON
{
    public class Message
    {
        public int UID;
        public string Title;
        public string TextContents;
        public string Image;
        public short ProgressPercent;
        public Author Author;
    }
}
