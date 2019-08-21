using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace IShop_Management.Views
{
    /// <summary>
    /// Логика взаимодействия для OrderWindow.xaml
    /// </summary>
    public partial class OrderWindow : Window
    {
        public OrderWindow(string a, string b)
        {
            InitializeComponent();

            LoadOrderInfo(a, b);

        }

        public void LoadOrderInfo(string oderNumber, string orderDateTime)
        {
            OderNumber.Text = oderNumber;
            OrderDateTime.Text = orderDateTime;
        }
    }
}
