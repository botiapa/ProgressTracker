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
        }

        public void updateContents(Message message) => setContents(message);

        private void setContents(Message message)
        {
            Title = message.Title;
            Description = message.Contents;
            if (!String.IsNullOrWhiteSpace(message.Author.ImageUrl))
                Avatar.UriSource = new Uri(message.Author.ImageUrl); // Set the image source if it's not empty or null
            Progress = message.Progress;

            Data = message;
            ID = message.ID;
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

        private void onMessageControlClicked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(!DeleteButton.IsMouseOver)
            {
                var mainWindow = (MainWindow)Application.Current.MainWindow;
                var mcFullscreen = new MessageControlFullscreen(Data);
                mainWindow.OverlayContents.Children.Add(mcFullscreen);
                mainWindow.Overlay.Visibility = Visibility.Visible;
            }
        }
    }
}
