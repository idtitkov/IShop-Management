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
    /// Логика взаимодействия для OrderWindow.xaml
    /// </summary>
    public partial class OrderView : Window
    {
        Order order;
        OrderViewModel orderViewModel;
        ManagerView managerView;
        /*Datasets-Datatables*/
        SqlDataAdapter m2m_orders_products_sda;
        DataTable m2m_orders_products_dt;
        DataTable mergedDT;
        public OrderView(Order order, OrderViewModel orderViewModel, ManagerView managerView)
        {
            InitializeComponent();

            this.order = order;
            DataContext = order;

            this.orderViewModel = orderViewModel;
            this.managerView = managerView;

            this.ResizeMode = ResizeMode.NoResize;

            FillDataGrid();
        }

        void FillOrderContacts()
        {

        }

        public void FillDataGrid()
        {
            string loadAllOrders = $"SELECT * FROM dbo.m2m_orders_products WHERE ord_id = {order.Ord_id};";
            SqlCommand cmd = new SqlCommand(loadAllOrders, LoginView.connection);
            m2m_orders_products_sda = new SqlDataAdapter(cmd);
            m2m_orders_products_dt = new DataTable();
            m2m_orders_products_sda.Fill(m2m_orders_products_dt);

            // деление таблиц и объединенеи

            string loadProductsName = $"SELECT * FROM dbo.products WHERE prd_id IN (SELECT prd_id FROM dbo.m2m_orders_products WHERE ord_id = {order.Ord_id});";
            cmd = new SqlCommand(loadProductsName, LoginView.connection);
            SqlDataAdapter products_sda = new SqlDataAdapter(cmd);
            DataTable products_dt = new DataTable();
            products_sda.Fill(products_dt);

            mergedDT = new DataTable();
            mergedDT.Merge(m2m_orders_products_dt);
            mergedDT.PrimaryKey = new DataColumn[] { mergedDT.Columns["prd_id"] };
            mergedDT.Merge(products_dt);

            dataGridOrderProduct.ItemsSource = mergedDT.DefaultView;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

            SqlCommand update_comm = new SqlCommand();
            update_comm.CommandText = @"UPDATE orders SET
                ord_name = @ord_name,
                ord_tel = @ord_tel,
                ord_address = @ord_address,
                ord_comments = @ord_comments,
                ord_status = @ord_status
                WHERE ord_id = @ord_id;";

            update_comm.Connection = LoginView.connection;

            var update_da = new SqlDataAdapter(update_comm);

            update_da.SelectCommand.Parameters.Add(new SqlParameter("@ord_name", SqlDbType.NVarChar));
            update_da.SelectCommand.Parameters["@ord_name"].Value = OrderName.Text;

            update_da.SelectCommand.Parameters.Add(new SqlParameter("@ord_tel", SqlDbType.NVarChar));
            update_da.SelectCommand.Parameters["@ord_tel"].Value = OrderTel.Text;

            update_da.SelectCommand.Parameters.Add(new SqlParameter("@ord_address", SqlDbType.NVarChar));
            update_da.SelectCommand.Parameters["@ord_address"].Value = OrderAddress.Text;

            update_da.SelectCommand.Parameters.Add(new SqlParameter("@ord_comments", SqlDbType.NVarChar));
            update_da.SelectCommand.Parameters["@ord_comments"].Value = OrderComments.Text;

            update_da.SelectCommand.Parameters.Add(new SqlParameter("@ord_status", SqlDbType.Int));
            update_da.SelectCommand.Parameters["@ord_status"].Value = ComboBoxOrderStatus.SelectedIndex;

            update_da.SelectCommand.Parameters.Add(new SqlParameter("@ord_id", SqlDbType.Int));
            update_da.SelectCommand.Parameters["@ord_id"].Value = Convert.ToInt32(OderNumber.Text);

            var update_ds = new DataSet();
            update_da.Fill(update_ds);

            // Делим и сохраняем деленную таблицу с количеством заказов
            m2m_orders_products_dt = mergedDT.Copy();
            m2m_orders_products_dt.Columns.Remove("prd_name");
            m2m_orders_products_dt.Columns.Remove("prd_qty");
            m2m_orders_products_dt.Columns.Remove("prd_price_out");

            SqlCommandBuilder builder = new SqlCommandBuilder(m2m_orders_products_sda);
            m2m_orders_products_sda.UpdateCommand = builder.GetUpdateCommand();
            m2m_orders_products_sda.Update(m2m_orders_products_dt);

            managerView.datePicker_SelectedDateChanged(sender, e);
            this.Close();
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
