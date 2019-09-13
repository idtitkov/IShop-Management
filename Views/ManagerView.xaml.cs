using IShop_Management.Models;
using IShop_Management.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;


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

            orderViewModel = new OrderViewModel();
            DataContext = orderViewModel;

            LoadOrders();

            // Обновление окна по таймеру
            var dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(WindowActivated);
            dispatcherTimer.Interval = new TimeSpan(0, 5, 0);
            dispatcherTimer.Start();
        }

        private void LoadOrders()
        {
            dataGridNewOrders.ItemsSource = null;
            dataGridActiveOrders.ItemsSource = null;
            dataGridCanceledOrders.ItemsSource = null;
            dataGridAllOrders.ItemsSource = null;

            orderViewModel.Refresh();

            dataGridNewOrders.ItemsSource = orderViewModel.NewOrders;
            dataGridActiveOrders.ItemsSource = orderViewModel.ActiveOrders;
            dataGridCanceledOrders.ItemsSource = orderViewModel.CanceledOrders;
            dataGridAllOrders.ItemsSource = orderViewModel.AllOrders;
        }

        private void datePicker_SelectedDateChanged(object sender, RoutedEventArgs e)
        {
            LoadOrders();
        }

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid grid = sender as DataGrid;
            DataGridRow dgr = grid.ItemContainerGenerator.ContainerFromItem(grid.SelectedItem) as DataGridRow;

            if (dgr != null)
            {
                Order ord = (Order)dgr.Item;

                OrderView orderEdit = new OrderView(ord);

                orderEdit.Show();
            }
        }

        // Создание нового заказа
        private void ButtonNewOrder_Click(object sender, RoutedEventArgs e)
        {
            // Получение максимального номера заказа
            string getNewOrdId = $"SELECT IDENT_CURRENT('dbo.orders');";
            SqlCommand sqlGetNewOrdId = new SqlCommand(getNewOrdId, LoginView.connection);
            if (LoginView.connection.State == ConnectionState.Closed)
                LoginView.connection.Open();

            // Открытие пустого заказа с максимальным номером +1
            Order ord = new Order();
            ord.Ord_id = Convert.ToInt32(sqlGetNewOrdId.ExecuteScalar());
            ord.Ord_id++;
            ord.Ord_date_created = DateTime.Now;

            // Открываем новый заказ с новый id
            OrderView orderEdit = new OrderView(ord);
            orderEdit.Show();
        }

        // Поиск заказов
        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            var filteredNewOrders = new ObservableCollection<Order>(from item in orderViewModel.NewOrders where item.Ord_tel.Contains(texbox_Search.Text) select item);
            dataGridNewOrders.ItemsSource = filteredNewOrders;

            var filteredActiveOrders = new ObservableCollection<Order>(from item in orderViewModel.ActiveOrders where item.Ord_tel.Contains(texbox_Search.Text) select item);
            dataGridActiveOrders.ItemsSource = filteredActiveOrders;

            var filteredCanceledOrders = new ObservableCollection<Order>(from item in orderViewModel.CanceledOrders where item.Ord_tel.Contains(texbox_Search.Text) select item);
            dataGridCanceledOrders.ItemsSource = filteredCanceledOrders;

            var filteredAllOrders = new ObservableCollection<Order>(from item in orderViewModel.AllOrders where item.Ord_tel.Contains(texbox_Search.Text) select item);
            dataGridAllOrders.ItemsSource = filteredAllOrders;
        }

        // Обновление заказов при возвращении фокуса в окно
        private void WindowActivated(object sender, EventArgs e)
        {
            LoadOrders();
        }
    }
}