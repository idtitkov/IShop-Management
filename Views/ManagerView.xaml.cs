using IShop_Management.Models;
using IShop_Management.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
    /// Логика взаимодействия для ManagerView.xaml
    /// </summary>
    public partial class ManagerView : Window
    {
        OrderViewModel orderViewModel;
        public ManagerView()
        {
            InitializeComponent();

            LoadOrders();
        }

        public void LoadOrders()
        {
            orderViewModel = new OrderViewModel();
            DataContext = orderViewModel;
        }

        public void datePicker_SelectedDateChanged(object sender, RoutedEventArgs e)
        {
            dataGridNewOrders.ItemsSource = null;
            dataGridActiveOrders.ItemsSource = null;
            dataGridDeliveredOrders.ItemsSource = null;
            dataGridAllOrders.ItemsSource = null;

            orderViewModel.Refresh();

            dataGridNewOrders.ItemsSource = orderViewModel.NewOrders;
            dataGridActiveOrders.ItemsSource = orderViewModel.ActiveOrders;
            dataGridDeliveredOrders.ItemsSource = orderViewModel.DeliveredOrders;
            dataGridAllOrders.ItemsSource = orderViewModel.AllOrders;
        }

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid grid = sender as DataGrid;
            DataGridRow dgr = grid.ItemContainerGenerator.ContainerFromItem(grid.SelectedItem) as DataGridRow;

            if (dgr != null)
            {
                Order ord = (Order)dgr.Item;

                OrderView orderEdit = new OrderView(ord, orderViewModel, this);

                orderEdit.Show();
            }
        }
    }
}
