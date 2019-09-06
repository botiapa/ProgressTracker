using System.Windows;
using MySql.Data.MySqlClient;

namespace ProgressTracker
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

        private void btn_connect_click(object sender, RoutedEventArgs e)
        {
            try
            {
                string conString = "datasource=https://remotemysql.com;username=TMrArME2SE;password=kJTKB2snGQ";
                MySqlConnection conn = new MySqlConnection(conString);
            }
        }
    }
}
