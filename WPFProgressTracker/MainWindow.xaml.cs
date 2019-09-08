using APIProgressTracker;
using APIProgressTracker.JSONObjects;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Animation;
using WPFProgressTracker.Controls;

namespace WPFProgressTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Storyboard NewTaskStoryboard;
        Storyboard NewTaskStoryboardReverse;

        public MainWindow()
        {
            InitializeComponent();
            NewTaskStoryboard = (Storyboard)Resources["NewTaskStoryboard"];
            NewTaskStoryboardReverse = (Storyboard)Resources["NewTaskStoryboardReverse"];
        }

        private void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            ProgressTrackerAPI.Init();
            UpdateUI();
        }

        public void UpdateUI()
        {
            MessageHolder.Children.Clear();
            var messages = ProgressTrackerAPI.GetMessages();

            foreach (var msg in messages)
                MessageHolder.Children.Add(new MessageControl(msg));
        }

        private void NewTaskButtonClicked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (NewTaskBorder.Width != 0)
                NewTaskStoryboardReverse.Begin();
            else
                NewTaskStoryboard.Begin();
        }

        private void onSendButtonClicked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            NewTaskStoryboardReverse.Begin();
            var msg = new Message(TitleTextBox.Text, ContentsTextBox.Text, 0, new Author("me", Properties.Resources.testpic));
            ProgressTrackerAPI.SendNewMessage(msg);
            UpdateUI();

            TitleTextBox.Text = string.Empty;
            ContentsTextBox.Text = string.Empty;
        }
    }
}
