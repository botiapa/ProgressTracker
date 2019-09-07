using APIProgressTracker.JSON;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFProgressTracker.Controls
{
    /// <summary>
    /// Interaction logic for MessageControl.xaml
    /// </summary>
    public partial class MessageControl : UserControl
    {

        public string Title { get; set; }
        public string Description { get; set; }
        public string AvatarUrl { get; set; }
        public double Progress { get; set; }

        public MessageControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        public MessageControl(Message message)
        {
            InitializeComponent();
            DataContext = this;

            Title = message.Title;
            Description = message.TextContents;
            AvatarUrl = message.Author.ImageUrl;
            Progress = message.ProgressPercent;
        }
    }
}
