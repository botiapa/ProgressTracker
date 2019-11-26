using APIProgressTracker.JSONObjects;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using APIProgressTracker;
using WPFProgressTracker.Misc;
using System.Windows.Media.Animation;
using System;
using System.Windows;
using System.Windows.Media;
using WPFProgressTracker.UserControls;
using System.IO;

namespace WPFProgressTracker.Controls
{
    /// <summary>
    /// Interaction logic for MessageControl.xaml
    /// </summary>
    public partial class MessageControl : UserControl
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public BitmapImage Avatar { get; set; }
        public double Progress { get; set; }

        public string ID;

        public Message Data;

        Storyboard hoverStoryboard;

        public MessageControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        public MessageControl(Message message)
        {
            InitializeComponent();
            DataContext = this;

            setContents(message);
            ProgressBar.OnProgressBarUpdate += onProgressBarUpdated;
        }

        private async void onProgressBarUpdated(object sender, EventArgs e)
        {
            var pb = (RoundedProgressBar)sender;
            await ProgressTrackerAPI.UpdateMessage(ID, null, null, (int)pb.Progress);
        }

        public void updateContents(Message message) => setContents(message);

        private void setContents(Message message)
        {
            Title = message.Title;
            Description = message.Contents;
            Console.WriteLine(ProgressTrackerAPI.server);
            
            if (!String.IsNullOrWhiteSpace(message.Author.ImageUrl))
                Avatar = new BitmapImage(new Uri(message.Author.ImageUrl)); // Set the image source if it's not empty or null
            else
                Avatar = new BitmapImage(new Uri(ProgressTrackerAPI.server + "/uploads/defaultAvatar.png"));
            
            Progress = message.Progress;

            Data = message;
            ID = message.ID;

            var main = (MainWindow)Application.Current.MainWindow;
            if (main.loggedInAuthor.ID == message.Author.ID)
            {
                MessageOptions.Visibility = Visibility.Visible;
                ProgressBar.Editable = true;
            }
                
            else
            {
                MessageOptions.Visibility = Visibility.Collapsed;
                ProgressBar.Editable = false;
            }
                
        }

        private async void onDeleteButtonClicked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (await ProgressTrackerAPI.DeleteMessage(ID))
            {
                this.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
                Console.WriteLine("AN ERROR HAS OCCURED. Cannot Delete!");
        }

        private void onCompleteButtonClicked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ProgressBar.Progress = ProgressBar.MaxProgress;
        }

        private void onMessageControlClicked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!MessageOptions.IsMouseOver && !ProgressBar.IsMouseOver)
            {
                var mainWindow = (MainWindow)Application.Current.MainWindow;
                var mcFullscreen = new MessageControlFullscreen(Data);
                mainWindow.OverlayContents.Children.Add(mcFullscreen);
                mainWindow.Overlay.Visibility = Visibility.Visible;
            }
        }
    }
}
