using APIProgressTracker;
using APIProgressTracker.JSONObjects;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFProgressTracker.Misc;

namespace WPFProgressTracker.UserControls
{
    /// <summary>
    /// Interaction logic for MessageControlFullscreenxaml.xaml
    /// </summary>
    public partial class MessageControlFullscreen : UserControl
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public BitmapImage Avatar { get; set; }
        public double Progress { get; set; }

        public string ID;
        public Message Data;

        Storyboard hoverStoryboard;

        public MessageControlFullscreen()
        {
            InitializeComponent();
            DataContext = this;
        }

        public MessageControlFullscreen(Message message)
        {
            InitializeComponent();
            DataContext = this;

            Title = message.Title;
            Description = message.Contents;
            if (!String.IsNullOrWhiteSpace(message.Author.ImageUrl))
                Avatar.UriSource = new Uri(message.Author.ImageUrl); // Set the image source if it's not empty or null
            Progress = message.Progress;

            this.ID = message.ID;
            Data = message;
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
    }
}
