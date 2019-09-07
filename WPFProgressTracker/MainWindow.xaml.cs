using APIProgressTracker;
using APIProgressTracker.JSON;
using APIProgressTracker.JSONObjects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
            MySqlQuery msgs = ProgressTrackerAPI.SQLQuery("SELECT * FROM progresstracker");

            foreach (List<object> card in msgs.objects)
            {
                Message msg = JsonConvert.DeserializeObject<Message>(card[1].ToString());

                MessageHolder.Children.Add(new MessageControl(msg));
            }
        }
    }
}
