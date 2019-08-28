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

        private void datePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
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
            OrderView orderEdit = new OrderView();
            orderEdit.Show();

            //orderEdit.Show();

            //DataGridRow dgr = dataGridNewOrders.ItemContainerGenerator.ContainerFromItem(dataGridNewOrders.SelectedItem) as DataGridRow;
            //DataRowView dr = (DataRowView)dgr.Item;

            //string ord_id = dr[0].ToString();
            //string ord_date_created = dr[6].ToString();

            //OrderWindow orderEdit = new OrderWindow(ord_id, ord_date_created);
            //MessageBox.Show("You Clicked : \r\nName : " + ProductName + "\r\nDescription : " + ProductDescription);
        }
    }
}
