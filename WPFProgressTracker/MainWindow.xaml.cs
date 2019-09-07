using APIProgressTracker;
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
            ProgressTrackerAPI.Init();
        }

        private void btn_test_click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Bigd");
            ProgressTrackerAPI.Init();
            
        }
    }
}
