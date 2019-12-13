using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace APIProgressTracker
{
    public class GenericWebSocket
    {
        protected Uri address { get; private set; }
        public ClientWebSocket ws { get; protected set; }
        public bool isOpen { get => (ws != null && ws.State == WebSocketState.Open); }
        public CancellationTokenSource receiveTokenSource;

        public GenericWebSocket(Uri address)
        {
            this.address = address;
        }

        public async Task<bool> Connect()
        {
            ws = new ClientWebSocket();
            try
            {
                await ws.ConnectAsync(address, new CancellationToken());
            }
            catch(WebSocketException) { }
            
            if (ws.State == WebSocketState.Open)
            {
                receiveTokenSource = new CancellationTokenSource();
                System.Diagnostics.Debug.WriteLine(String.Format("WebSocket connected with address: {0}", address));
                WebSocketConnected(new EventArgs());

                new Thread(() => waitForMessage(receiveTokenSource.Token)).Start();
                return true;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine(String.Format("WebSocket couldn't connect with address: {0}", address));
                return false;
            }

        }

        public async Task Close(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken token)
        {
            receiveTokenSource.Cancel();
            ws.Abort();
            await ws.CloseAsync(closeStatus, statusDescription, token);
            System.Diagnostics.Debug.WriteLine(String.Format("WebSocket closed with address: {0}", address));
        }

        public async Task Close()
        {
            receiveTokenSource.Cancel();
            await ws.CloseAsync(WebSocketCloseStatus.Empty, String.Empty, new CancellationToken());
            System.Diagnostics.Debug.WriteLine(String.Format("WebSocket closed with address: {0}", address));
        }

        protected virtual async void waitForMessage(CancellationToken receiveToken, int bufferSize = 2048)
        {
            while (!receiveToken.IsCancellationRequested)
            {
                var buffer = new byte[bufferSize];
                var segment = new ArraySegment<byte>(buffer);

                WebSocketReceiveResult msgResult = null;
                string receivedString = String.Empty;
                try
                {
                    do
                    {
                        msgResult = await ws.ReceiveAsync(segment, receiveToken);
                        receivedString += Encoding.UTF8.GetString(buffer.Take(msgResult.Count).ToArray());
                    }
                    while (!msgResult.EndOfMessage);
                }
                catch (OperationCanceledException) { }
                catch (WebSocketException)
                {
                    if(ws.State == WebSocketState.Aborted && ws.CloseStatus == null)
                    {
                        System.Diagnostics.Debug.WriteLine(String.Format("WebSocket connection lost reconnecting"));
                        await Connect();
                        break;
                    }
                }

                if (msgResult != null)
                {                 
                    MessageReceived(receivedString);
                    System.Diagnostics.Debug.WriteLine(String.Format("WebSocket message received: {0}", receivedString));
                }
            }
        }

        /// <summary>
        /// Not needed at this time, but just in case
        /// </summary>
        public async Task<bool> sendMessage(object msg)
        {
            if (isOpen)
            {
                var msgString = JsonConvert.SerializeObject(msg);
                var msgArray = new ArraySegment<byte>(Encoding.UTF8.GetBytes(msgString));

                await ws.SendAsync(msgArray, WebSocketMessageType.Text, true, new CancellationToken());
                System.Diagnostics.Debug.WriteLine("WebSocket message sent: {0}", msgString);
                return true;
            }
            return false;
        }

        public event EventHandler OnWebSocketConnected;
        protected virtual void WebSocketConnected(EventArgs e)
        {
            EventHandler handler = OnWebSocketConnected;
            handler?.Invoke(this, e);
        }

        public delegate void MessageReceivedHandler(object sender, MessageReceivedEventArgs e);
        public event MessageReceivedHandler OnMessageReceived;

        protected virtual void MessageReceived(string msg)
        {
            MessageReceivedHandler handler = OnMessageReceived;
            handler?.Invoke(this, new MessageReceivedEventArgs(msg));
        }

        private static bool isValidJson(string strInput)
        {
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    //Exception in parsing json
                    Console.WriteLine(jex.Message);
                    return false;
                }
                catch (Exception ex) //some other exception
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
    public class MessageReceivedEventArgs : EventArgs
    {
        public string message { get; private set; }

        public MessageReceivedEventArgs(string msg)
        {
            message = msg;
        }
    }
}
