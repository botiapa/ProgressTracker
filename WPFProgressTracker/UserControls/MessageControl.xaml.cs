using APIProgressTracker.JSON;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using APIProgressTracker;

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
            Avatar = ProgressTrackerAPI.Base64StringToBitmap(message.Author.ImageUrl);
            Progress = message.ProgressPercent;
        }
    }
}
