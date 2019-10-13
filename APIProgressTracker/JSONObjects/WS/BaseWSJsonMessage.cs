using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace APIProgressTracker.JSONObjects
{
    public class BaseWSJsonMessage
    {
        public string type { get; set; }
        public JObject message { get; set; }
    }
}
