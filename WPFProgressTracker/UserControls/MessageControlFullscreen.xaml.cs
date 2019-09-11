﻿using APIProgressTracker;
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

        public int ID;
        public Message Data;

        Storyboard hoverStoryboard;

        public MessageControlFullscreen()
        {
            InitializeComponent();
            DataContext = this;
        }

        public MessageControlFullscreen(int ID, Message message)
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
            if (ProgressTrackerAPI.DeleteMessage(ID))
            {
                this.Visibility = System.Windows.Visibility.Collapsed; //FIXME
            }
        }
    }
}
