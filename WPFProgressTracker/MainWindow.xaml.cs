using APIProgressTracker;
using APIProgressTracker.JSON;
using APIProgressTracker.JSONObjects;
using System;
using System.Windows;
using WPFProgressTracker.Controls;

namespace WPFProgressTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MessageControl_Loaded(object sender, RoutedEventArgs e)
        {
            
            //MySqlQuery getfirstinfo = ProgressTrackerAPI.SQLQuery("");
            /*mc_1.Title.Text = testmsg.Title; // FIXME
            mc_1.Description.Text = testmsg.TextContents;

            double percent = mc_1.ActualWidth * ((100 - (double)testmsg.ProgressPercent) / 100);
            mc_1.Progress.Margin = new Thickness(0, 0, percent, 0);*/

            //MessageBox.Show(MessageJsonHelper.toJson(testmsg) + "\n" + percent);
        }

        private void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            ProgressTrackerAPI.Init();
            Message testmsg = new Message("Test title", "LOREM IPSUM HÁT MIZSENI LOREM IPSUM HÁT MIZSENI LOREM IPSUM HÁT MIZSENI", 34, new Author("Faszjancsi", "http://fanaru.com/futurama/image/69754-futurama-zoidberg-avatar.jpg"));

            var msgControl = new MessageControl();
            msgControl.Title = testmsg.Title;
            msgControl.Description = testmsg.TextContents;
            msgControl.AvatarUrl = testmsg.Author.ImageUrl;
            msgControl.Progress = testmsg.ProgressPercent;

            MessageHolder.Children.Add(msgControl);
        }
    }
}
