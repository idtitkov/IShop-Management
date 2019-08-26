using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Логика взаимодействия для ManagerWindow.xaml
    /// </summary>
    public partial class ManagerWindow : Window
    {
        public ManagerWindow()
        {
            InitializeComponent();

            FillDataGrid();
        }

        private void FillDataGrid()
        {
            using (LoginView.connection)
            {
                string showNewOrders = $"SELECT * FROM dbo.orders;";
                SqlCommand cmd = new SqlCommand(showNewOrders, LoginView.connection);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("orders");
                sda.Fill(dt);

                DGManager.ItemsSource = dt.DefaultView;
            }
        }

        private void DataGrid_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            DataGridRow dgr = DGManager.ItemContainerGenerator.ContainerFromItem(DGManager.SelectedItem) as DataGridRow;
            DataRowView dr = (DataRowView)dgr.Item;

            string ord_id = dr[0].ToString();
            string ord_date_created = dr[6].ToString();

            OrderWindow orderEdit = new OrderWindow(ord_id, ord_date_created);
            orderEdit.Show();

            //MessageBox.Show("You Clicked : \r\nName : " + ProductName + "\r\nDescription : " + ProductDescription);

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //OrderWindow orderEdit = new OrderWindow();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {

            IShop_Management.IShopDataSetOrders iShopDataSetOrders = ((IShop_Management.IShopDataSetOrders)(this.FindResource("iShopDataSetOrders")));
            // Загрузить данные в таблицу orders. Можно изменить этот код как требуется.
            IShop_Management.IShopDataSetOrdersTableAdapters.ordersTableAdapter iShopDataSetOrdersordersTableAdapter = new IShop_Management.IShopDataSetOrdersTableAdapters.ordersTableAdapter();
            iShopDataSetOrdersordersTableAdapter.Fill(iShopDataSetOrders.orders);
            System.Windows.Data.CollectionViewSource ordersViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("ordersViewSource")));
            ordersViewSource.View.MoveCurrentToFirst();
        }
    }
}
