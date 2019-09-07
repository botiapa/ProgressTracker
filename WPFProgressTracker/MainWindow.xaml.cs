using APIProgressTracker;
using APIProgressTracker.JSONObjects;
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

            var messages = ProgressTrackerAPI.GetMessages();

            foreach (var msg in messages)
                MessageHolder.Children.Add(new MessageControl(msg));
        }
    }
}
