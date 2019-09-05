using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IShop_Management.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для MainMenuControl.xaml
    /// </summary>
    public partial class MainMenuControl : UserControl
    {
        public MainMenuControl()
        {
            InitializeComponent();
        }

        public void menuItemExit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoginView.connection.Close();
            }
            catch (Exception)
            {
            }
            finally
            {
                Environment.Exit(0);
            }

        }

        private void menuItemAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutBox aboutBox = new AboutBox();
            aboutBox.Show();
        }

        private void helpItemAbout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("IShopHelp.html");
            }
            catch (Exception)
            {
                MessageBox.Show(Window.GetWindow(this), "Не удается найти файл справки.\nПоместите файл \"IShopHelp.html\" в папку с программой.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
