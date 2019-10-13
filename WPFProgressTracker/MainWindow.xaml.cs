using APIProgressTracker;
using APIProgressTracker.JSONObjects;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using WPFProgressTracker.Controls;

namespace WPFProgressTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string WS_ADDRESS = "ws://localhost:8001";

        Storyboard NewTaskStoryboard;
        Storyboard NewTaskStoryboardReverse;

        DateTime newestTimestamp = DateTime.MinValue;

        /// <summary>
        /// The key is the ID of the message, and the value is the contents
        /// </summary>
        Dictionary<string, MessageControl> messageControls = new Dictionary<string, MessageControl>();
        public GenericWebSocket ws;

        public MainWindow()
        {
            InitializeComponent();
            NewTaskStoryboard = (Storyboard)Resources["NewTaskStoryboard"];
            NewTaskStoryboardReverse = (Storyboard)Resources["NewTaskStoryboardReverse"];
        }

        private async void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            var hash = await ProgressTrackerAPI.LoginAsync("alma123", "alma123");
            if (hash != null)
                ProgressTrackerAPI.hash = hash;

            ws = new GenericWebSocket(new Uri(WS_ADDRESS + "/" + hash));
            await ws.Connect();
            new WebSocketHandler(ws, this);

            await ReloadUI();
        }

        public async Task ReloadUI()
        {
            MessageHolder.Children.Clear();
            messageControls.Clear();
            var messages = await ProgressTrackerAPI.GetMessages(newestTimestamp);

            foreach(var msg in messages)
            {
                var mc = new MessageControl(msg);
                MessageHolder.Children.Add(mc);
                messageControls.Add(msg.ID, mc);
            }
                
        }

        private void NewTaskButtonClicked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (NewTaskBorder.Width != 0)
                NewTaskStoryboardReverse.Begin();
            else
                NewTaskStoryboard.Begin();
        }

        private async void onSendButtonClicked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            NewTaskStoryboardReverse.Begin();
            await ProgressTrackerAPI.SendNewMessage(TitleTextBox.Text, ContentsTextBox.Text, 0);

            TitleTextBox.Text = string.Empty;
            ContentsTextBox.Text = string.Empty;
        }

        private void onOverlayClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!OverlayContents.IsMouseOver)
            {
                OverlayContents.Children.Clear();
                Overlay.Visibility = Visibility.Collapsed;
            }
        }

        private async void MainWindowUnloaded(object sender, RoutedEventArgs e)
        {
            if (ws != null)
                await ws.Close();
        }
    }
}
