using APIProgressTracker;
using APIProgressTracker.JSON;
using Newtonsoft.Json;
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
