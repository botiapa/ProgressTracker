using APIProgressTracker;
using APIProgressTracker.JSONObjects;
using APIProgressTracker.JSONObjects.WS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace WPFProgressTracker
{
    class WebSocketHandler
    {
        const string TYPE_NEWMESSAGE = "CREATE";
        const string TYPE_UPDATEMESSAGE = "UPDATE";
        const string TYPE_DELETEMESSAGE = "DELETE";

        GenericWebSocket ws;
        MainWindow main;

        public WebSocketHandler(GenericWebSocket WS, MainWindow main)
        {
            ws = WS;
            this.main = main;
            ws.OnMessageReceived += onMessageReceived;
        }

        private void onMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            var baseMsg = JsonConvert.DeserializeObject<BaseWSJsonMessage>(e.message);
            switch(baseMsg.type)
            {
                case TYPE_NEWMESSAGE:
                    newMessageReceived(baseMsg.message.ToObject<Message>());
                    break;

                case TYPE_UPDATEMESSAGE:
                    updatedMessageReceived(baseMsg.message.ToObject<Message>());
                    break;

                case TYPE_DELETEMESSAGE:
                    deletedMessageReceived(baseMsg.message.ToObject<DeletedMessage>());
                    break;

                default:
                    Console.WriteLine("UNKNOWN WS TYPE RECEIVED");
                    break;
            }
        }

        void newMessageReceived(Message msg)
        {

        }

        void updatedMessageReceived(Message msg)
        {

        }

        void deletedMessageReceived(DeletedMessage msg)
        {

        }
    }
}
