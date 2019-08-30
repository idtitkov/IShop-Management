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

            LoadOrders();
        }

        public void LoadOrders()
        {
            orderViewModel = new OrderViewModel();
            DataContext = orderViewModel;
        }

        public void datePicker_SelectedDateChanged(object sender, RoutedEventArgs e)
        {
            dgWActiveOrders.ItemsSource = null;
            dgWDeliveredOrders.ItemsSource = null;
            dgWAllOrders.ItemsSource = null;

            orderViewModel.Refresh();

            dgWActiveOrders.ItemsSource = orderViewModel.ActiveOrders;
            dgWDeliveredOrders.ItemsSource = orderViewModel.DeliveredOrders;
            dgWAllOrders.ItemsSource = orderViewModel.AllOrders;
        }

        private void dgWarehouse_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid grid = sender as DataGrid;
            DataGridRow dgr = grid.ItemContainerGenerator.ContainerFromItem(grid.SelectedItem) as DataGridRow;

            if (dgr != null)
            {
                Order ord = (Order)dgr.Item;

                //OrderView orderEdit = new OrderView(ord, orderViewModel, this);

                //orderEdit.Show();
            }
        }

        private void ButtonNewOrder_Click(object sender, RoutedEventArgs e)
        {
            // Получение номера заказа
            string getNewOrdId = $"SELECT MAX(ord_id) FROM dbo.orders;";
            SqlCommand sqlGetNewOrdId = new SqlCommand(getNewOrdId, LoginView.connection);
            if (LoginView.connection.State == ConnectionState.Closed)
                LoginView.connection.Open();

            // Открытие пустого заказа с новым номером
            Order ord = new Order();
            ord.Ord_id = Convert.ToInt32(sqlGetNewOrdId.ExecuteScalar());
            ord.Ord_id++;
            ord.Ord_date_created = DateTime.Now;

            //OrderView orderEdit = new OrderView(ord, orderViewModel, this);
            //orderEdit.Show();
        }


    }
}