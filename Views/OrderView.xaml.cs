﻿using IShop_Management.Models;
using IShop_Management.ViewModels;
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
    /// Логика взаимодействия для OrderWindow.xaml
    /// </summary>
    public partial class OrderView : Window, INotifyPropertyChanged
    {
        Order order;
        OrderViewModel orderViewModel;
        ManagerView managerView;
        /*Datasets-Datatables*/
        SqlDataAdapter m2m_orders_products_sda;
        DataTable m2m_orders_products_dt;
        DataTable mergedDT;

        /*Summ Property*/
        private double _orderCost;
        public double OrderCost
        {
            get { return _orderCost; }
            set
            {
                _orderCost = value;
                OnPropertyChanged("OrderCost");
            }
        }
        public OrderView(Order order, OrderViewModel orderViewModel, ManagerView managerView)
        {
            InitializeComponent();

            this.order = order;
            DataContext = order;
            this.orderViewModel = orderViewModel;
            this.managerView = managerView;
            this.ResizeMode = ResizeMode.NoResize;

            FillDataGrid();

            dataGridOrderProduct.CellEditEnding += dataGridOrderProduct_CellEditEnding;
            dataGridOrderProduct.LostFocus += dataGridOrderProduct_LostFocus;
            dataGridOrderProduct.GotFocus += dataGridOrderProduct_LostFocus;
        }
        public void FillDataGrid()
        {
            // Загрузка с id товаров и количеством
            string loadAllOrders = $"SELECT * FROM dbo.m2m_orders_products WHERE ord_id = {order.Ord_id};";
            SqlCommand cmd = new SqlCommand(loadAllOrders, LoginView.connection);
            m2m_orders_products_sda = new SqlDataAdapter(cmd);
            m2m_orders_products_dt = new DataTable();
            m2m_orders_products_sda.Fill(m2m_orders_products_dt);

            // Загрузка названий
            string loadProductsName = $"SELECT * FROM dbo.products WHERE prd_id IN (SELECT prd_id FROM dbo.m2m_orders_products WHERE ord_id = {order.Ord_id});";
            cmd = new SqlCommand(loadProductsName, LoginView.connection);
            SqlDataAdapter products_sda = new SqlDataAdapter(cmd);
            DataTable products_dt = new DataTable();
            products_sda.Fill(products_dt);

            // Объединение заблиц
            mergedDT = new DataTable();
            mergedDT.Merge(m2m_orders_products_dt);
            mergedDT.PrimaryKey = new DataColumn[] { mergedDT.Columns["prd_id"] };
            mergedDT.Merge(products_dt);

            // Добавление суммы в общую таблицу
            mergedDT.Columns.Add("summ", typeof(double));

            // передача в DataGrid
            dataGridOrderProduct.ItemsSource = mergedDT.DefaultView;

            // Подсчет суммы первый
            foreach (DataRow row in mergedDT.Rows)
            {
                row["summ"] = (int)row["op_qty"] * (decimal)row["prd_price_out"];
            }
            foreach (DataRowView rowView in (DataView)this.dataGridOrderProduct.ItemsSource)
            {
                OrderCost += (double)rowView["summ"];
            }
        }

        // События
        private void AddByProdId_Click(object sender, RoutedEventArgs e)
        {
            int prodId;
            if (!(int.TryParse(textBox_AddProduct.Text, out prodId)))
                return;
            else
                prodId = Convert.ToInt32(textBox_AddProduct.Text);

            string addProdName = $"SELECT prd_name FROM dbo.products WHERE prd_id = {prodId};";
            string addProdQty = $"SELECT prd_qty FROM dbo.products WHERE prd_id = {prodId};";
            string addProdPrice = $"SELECT prd_price_out FROM dbo.products WHERE prd_id = {prodId};";
            SqlCommand sqAddProdName = new SqlCommand(addProdName, LoginView.connection);
            SqlCommand sqladdProdQty = new SqlCommand(addProdQty, LoginView.connection);
            SqlCommand sqAddProdPrice = new SqlCommand(addProdPrice, LoginView.connection);

            if (LoginView.connection.State == ConnectionState.Closed)
                LoginView.connection.Open();

            var row = mergedDT.NewRow();
            row["ord_id"] = Convert.ToInt32(OderNumber.Text);
            row["prd_id"] = prodId;
            row["op_qty"] = 1;
            row["prd_name"] = sqAddProdName.ExecuteScalar();
            row["prd_qty"] = Convert.ToDecimal(sqladdProdQty.ExecuteScalar()); ;
            row["prd_price_out"] = Convert.ToDecimal(sqAddProdPrice.ExecuteScalar());
            row["summ"] = (decimal)row["prd_price_out"];

            try
            {
                mergedDT.Rows.Add(row);
            }
            catch (Exception)
            {
                return;
            }
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем есть ли такой заказ
            string getLastOrdId = $"SELECT MAX(ord_id) FROM dbo.orders;";
            SqlCommand sqlGetNewOrdId = new SqlCommand(getLastOrdId, LoginView.connection);
            if (LoginView.connection.State == ConnectionState.Closed)
                LoginView.connection.Open();

            int lastOrdId = Convert.ToInt32(sqlGetNewOrdId.ExecuteScalar());
            // Если нет заказов с таким номер но записываем пустышку в базу
            if (order.Ord_id > lastOrdId)
            {
                string insert_string = $@"INSERT INTO orders
                (ord_name, ord_tel, ord_address, ord_email, ord_comments, ord_date_created, ord_status, cur_id, ord_date_delivereed)
                VALUES ('', '', '', '', '', '{DateTime.Now}', 0, 0, NULL);";
                SqlCommand insert_comm = new SqlCommand(insert_string, LoginView.connection);
                insert_comm.ExecuteNonQuery();
            }

            // Сохраняем личные данные
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
            m2m_orders_products_dt.Columns.Remove("summ");

            SqlCommandBuilder builder = new SqlCommandBuilder(m2m_orders_products_sda);
            m2m_orders_products_sda.UpdateCommand = builder.GetUpdateCommand();
            m2m_orders_products_sda.Update(m2m_orders_products_dt);

            // Обновляем таблицу заказов
            managerView.datePicker_SelectedDateChanged(sender, e);
            this.Close();
        }
        private void dataGridOrderProduct_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            OrderCost = 0;
            foreach (DataRowView rowView in (DataView)dataGridOrderProduct.ItemsSource)
            {
                rowView["summ"] = (int)rowView["op_qty"] * (decimal)rowView["prd_price_out"];
                OrderCost += (double)rowView["summ"];
            }
        }
        private void dataGridOrderProduct_LostFocus(object sender, RoutedEventArgs e)
        {
            OrderCost = 0;
            foreach (DataRowView rowView in (DataView)dataGridOrderProduct.ItemsSource)
            {
                rowView["summ"] = (int)rowView["op_qty"] * (decimal)rowView["prd_price_out"];
                OrderCost += (double)rowView["summ"];
            }
            e.Handled = true;
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void dataGridOrderProduct_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            if (dg != null)
            {
                DataGridRow dgr = (DataGridRow)(dg.ItemContainerGenerator.ContainerFromIndex(dg.SelectedIndex));
                if (e.Key == Key.Delete && !dgr.IsEditing)
                {
                    // User is attempting to delete the row
                    var result = MessageBox.Show(
                        "Текущая позиция будет удалена из заказа.\n\nПродолжить?",
                        "Удаление",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question,
                        MessageBoxResult.No);
                    e.Handled = (result == MessageBoxResult.No);
                }
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
