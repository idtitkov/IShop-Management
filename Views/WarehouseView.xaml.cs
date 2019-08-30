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
    public partial class WarehouseView : Window
    {
        OrderViewModel orderViewModel;
        public WarehouseView()
        {
            InitializeComponent();

            orderViewModel = new OrderViewModel();
            DataContext = orderViewModel;

            LoadOrders();
        }

        public void LoadOrders()
        {
            dgWActiveOrders.ItemsSource = null;
            dgWCanceledOrders.ItemsSource = null;
            dgWDeliveredOrders.ItemsSource = null;
            dgWAllOrders.ItemsSource = null;

            orderViewModel.Refresh();

            dgWActiveOrders.ItemsSource = orderViewModel.ActiveOrders;
            dgWCanceledOrders.ItemsSource = orderViewModel.CanceledOrders;
            dgWDeliveredOrders.ItemsSource = orderViewModel.DeliveredOrders;
            dgWAllOrders.ItemsSource = orderViewModel.AllOrders;
        }

        public void datePicker_SelectedDateChanged(object sender, RoutedEventArgs e)
        {
            LoadOrders();
        }

        private void dgWarehouse_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid grid = sender as DataGrid;
            DataGridRow dgr = grid.ItemContainerGenerator.ContainerFromItem(grid.SelectedItem) as DataGridRow;

            if (dgr != null)
            {
                Order ord = (Order)dgr.Item;

                CourierView orderEdit = new CourierView(ord);
                orderEdit.Show();
            }
        }

        private void WindowActivated(object sender, EventArgs e)
        {
            LoadOrders();
        }
    }
}