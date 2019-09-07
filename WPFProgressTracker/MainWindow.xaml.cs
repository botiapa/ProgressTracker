using APIProgressTracker;
using APIProgressTracker.JSON;
using APIProgressTracker.JSONObjects;
using System;
using System.Windows;

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
            ProgressTrackerAPI.Init();
            //MySqlQuery getfirstinfo = ProgressTrackerAPI.SQLQuery("");
            Message testmsg = new Message("Test title", "LOREM IPSUM HÁT MIZSENI LOREM IPSUM HÁT MIZSENI LOREM IPSUM HÁT MIZSENI", 34, new Author("Faszjancsi", ""));
            mc_1.Title.Text = testmsg.Title;
            mc_1.Description.Text = testmsg.TextContents;

            double percent = mc_1.ActualWidth * ((100 - (double)testmsg.ProgressPercent) / 100);
            mc_1.Progress.Margin = new Thickness(0, 0, percent, 0);

            MessageBox.Show(MessageJsonHelper.toJson(testmsg) + "\n" + percent);
        }
    }
}
