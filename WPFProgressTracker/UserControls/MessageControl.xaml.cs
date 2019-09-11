﻿using APIProgressTracker.JSONObjects;
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

        public int ID;
        public Message Data;

        Storyboard hoverStoryboard;

        public MessageControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        public MessageControl(int ID, Message message)
        {
            InitializeComponent();
            DataContext = this;

            Title = message.Title;
            Description = message.TextContents;
            Avatar = ImageHelper.Base64StringToBitmap(message.Author.ImageUrl);
            Progress = message.ProgressPercent;

            this.ID = ID;
            Data = message;
        }

        private void onDeleteButtonClicked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(ProgressTrackerAPI.DeleteMessage(ID))
            {
                this.Visibility = System.Windows.Visibility.Collapsed; //FIXME
            }
        }

        private void onMessageControlClicked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            var mcFullscreen = new MessageControlFullscreen(ID, Data);
            mainWindow.OverlayContents.Children.Add(mcFullscreen);
            mainWindow.Overlay.Visibility = Visibility.Visible;
        }
    }
}
