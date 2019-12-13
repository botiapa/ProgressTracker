using APIProgressTracker;
using APIProgressTracker.JSONObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
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
        const string WS_ADDRESS = "ws://progress-tracker-server.herokuapp.com";
        //const string WS_ADDRESS = "ws://localhost";
        const string HASH_FILE = "login.hash";

        Storyboard NewTaskStoryboard;
        Storyboard NewTaskStoryboardReverse;

        DateTime newestTimestamp = DateTime.MinValue;

        /// <summary>
        /// The key is the ID of the message, and the value is the contents
        /// </summary>
        public Dictionary<string, MessageControl> MessageControls = new Dictionary<string, MessageControl>();
        public GenericWebSocket ws;
        public Author loggedInAuthor;

        public MainWindow()
        {
            InitializeComponent();
            NewTaskStoryboard = (Storyboard)Resources["NewTaskStoryboard"];
            NewTaskStoryboardReverse = (Storyboard)Resources["NewTaskStoryboardReverse"];
        }

        private async void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            string hash = null;
            if(File.Exists(HASH_FILE))
            {
                var file = File.ReadAllText(HASH_FILE);
                if (await ProgressTrackerAPI.IsHashValid(file))
                    hash = file;
                else
                    File.Delete(HASH_FILE);
            }
            if(hash == null)
            {
                hash = await loginProcess();
                var sw = File.CreateText(HASH_FILE);
                await sw.WriteAsync(hash);
                sw.Close();
            }

            ProgressTrackerAPI.hash = hash;
            loggedInAuthor = await ProgressTrackerAPI.GetAuthorInfo();

            LoginPanel.Visibility = Visibility.Collapsed;
            MainPanel.Visibility = Visibility.Visible;

            ws = new GenericWebSocket(new Uri(WS_ADDRESS + "/" + hash));
            ws.OnWebSocketConnected += onWSConnected;
            await ws.Connect();
            new WebSocketHandler(ws, this);
        }

        private async void onWSConnected(object? sender, EventArgs e)
        {
            await Dispatcher.BeginInvoke(async () =>
            {
                await ReloadUI();
            });
            
        }

        public async Task ReloadUI()
        {
            MessageHolder.Children.Clear();
            MessageControls.Clear();
            var messages = await ProgressTrackerAPI.GetMessages(newestTimestamp);

            foreach(var msg in messages)
            {
                var mc = new MessageControl(msg);
                MessageHolder.Children.Add(mc);
                MessageControls.Add(msg.ID, mc);
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

        CancellationTokenSource waitingLoginTokenSource = new CancellationTokenSource();
        private void onLoginButtonClicked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(!waitingLoginTokenSource.Token.IsCancellationRequested)
                waitingLoginTokenSource.Cancel();
        }

        private async Task<string> loginProcess()
        {
            while(true)
            {
                await Task.Run(() => waitingLoginTokenSource.Token.WaitHandle.WaitOne());
                UsernameInput.IsReadOnly = true;
                PasswordInput.Focusable = false;
                PasswordInput.IsHitTestVisible = false;
                string login = null;
                try
                {
                    login = await ProgressTrackerAPI.LoginAsync(UsernameInput.Text, PasswordInput.Password.ToString());
                }
                catch (TaskCanceledException) { }
                catch(HttpRequestException) { }

                if (login != null)
                    return login;
                else
                    MessageBox.Show("Login failed. Please try again.", "Login failed", MessageBoxButton.OK, MessageBoxImage.Error);
                UsernameInput.Clear();
                PasswordInput.Clear();
                UsernameInput.IsReadOnly = false;
                PasswordInput.Focusable = true;
                PasswordInput.IsHitTestVisible = true;
                waitingLoginTokenSource = new CancellationTokenSource();
            }
        }
    }
}
