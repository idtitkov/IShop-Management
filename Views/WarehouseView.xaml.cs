using IShop_Management.Models;
using IShop_Management.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

            // Обновление окна по таймеру
            var dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(WindowActivated);
            dispatcherTimer.Interval = new TimeSpan(0, 5, 0);
            dispatcherTimer.Start();
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

        // Поиск по номеру телефона
        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            var filteredActiveOrders = new ObservableCollection<Order>(from item in orderViewModel.ActiveOrders where item.Ord_tel.Contains(texbox_Search.Text) select item);
            dgWActiveOrders.ItemsSource = filteredActiveOrders;

            var filteredCanceledOrders = new ObservableCollection<Order>(from item in orderViewModel.CanceledOrders where item.Ord_tel.Contains(texbox_Search.Text) select item);
            dgWCanceledOrders.ItemsSource = filteredCanceledOrders;

            var filteredDeliveredOrders = new ObservableCollection<Order>(from item in orderViewModel.NewOrders where item.Ord_tel.Contains(texbox_Search.Text) select item);
            dgWDeliveredOrders.ItemsSource = filteredDeliveredOrders;

            var filteredAllOrders = new ObservableCollection<Order>(from item in orderViewModel.AllOrders where item.Ord_tel.Contains(texbox_Search.Text) select item);
            dgWAllOrders.ItemsSource = filteredAllOrders;
        }

        private void WindowActivated(object sender, EventArgs e)
        {
            LoadOrders();
        }


    }
}