using IShop_Management.Models;
using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace IShop_Management.Views
{
    /// <summary>
    /// Логика взаимодействия для CourierView.xaml
    /// </summary>
    public partial class CourierView : Window, INotifyPropertyChanged
    {
        // Инициализтуем окно информацией их заказа
        Order order;

        // Datasets/Datatables
        SqlDataAdapter m2m_orders_products_sda;
        DataTable m2m_orders_products_dt;
        DataTable mergedDT;

        /*Summ Property*/
        private double _orderCost;

        public CourierView(Order order)
        {
            InitializeComponent();

            this.order = order;
            DataContext = order;
            this.ResizeMode = ResizeMode.NoResize;

            FillDataGrid();
            LoadCouriers();

            mainMenuControl.ExportItem.IsEnabled = true;
            mainMenuControl.ExportItem.Click += ExportItem_Click;
        }

        private void FillDataGrid()
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

        private void LoadCouriers()
        {
            if (LoginView.connection.State == ConnectionState.Closed)
                LoginView.connection.Open();

            string loadCouriers = $"SELECT cur_name FROM dbo.couriers;";
            SqlCommand sqlLoadCouriers = new SqlCommand(loadCouriers, LoginView.connection);
            SqlDataReader sqlReader = sqlLoadCouriers.ExecuteReader();

            while (sqlReader.Read())
            {
                cbCouriers.Items.Add(sqlReader["cur_name"].ToString());
            }

            sqlReader.Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Сохраняем заказ с новым курьером и статусом
            SqlCommand update_comm = new SqlCommand();
            update_comm.CommandText = @"UPDATE orders SET
                ord_address = @ord_address,
                ord_comments = @ord_comments,
                ord_status = @ord_status,
                cur_id = @cur_id,
                ord_date_delivered = @ord_date_delivered
                WHERE ord_id = @ord_id;";

            update_comm.Connection = LoginView.connection;

            var update_da = new SqlDataAdapter(update_comm);

            update_comm.Parameters.AddWithValue("@ord_address", OrderAddress.Text);
            update_comm.Parameters.AddWithValue("@ord_comments", OrderComments.Text);
            update_comm.Parameters.AddWithValue("@ord_status", ComboBoxOrderStatus.SelectedIndex);
            update_comm.Parameters.AddWithValue("@cur_id", cbCouriers.SelectedIndex);
            update_comm.Parameters.AddWithValue("@ord_date_delivered", dpDateDelivered.SelectedDate ?? Convert.DBNull);
            update_comm.Parameters.AddWithValue("@ord_id", OderNumber.Text);

            var update_ds = new DataSet();
            update_da.Fill(update_ds);

            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        public double OrderCost
        {
            get { return _orderCost; }
            set
            {
                _orderCost = value;
                OnPropertyChanged("OrderCost");
            }
        }

        // Экспорт
        private void ExportItem_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Office.Interop.Excel.Application ExcelApp = new Microsoft.Office.Interop.Excel.Application();
            ExcelApp.Application.Workbooks.Add(Type.Missing);
            ExcelApp.Columns.ColumnWidth = 20;

            ExcelApp.Cells.Range["A1:D2"].Borders.Weight = 2d;
            ExcelApp.Cells.Range["A3:B5"].Borders.Weight = 2d;
            ExcelApp.Cells.Range["A7:E7"].Borders.Weight = 3d;

            ExcelApp.Cells[1, 1] = "Имя курьера";
            ExcelApp.Cells[1, 2] = cbCouriers.SelectedValue;

            ExcelApp.Cells[1, 3] = "Номер заказа";
            ExcelApp.Cells[1, 4].NumberFormat = "@";
            ExcelApp.Cells[1, 4] = OderNumber.Text;

            ExcelApp.Cells[2, 1] = "Номер телефона";
            ExcelApp.Cells[2, 2].NumberFormat = "@";
            ExcelApp.Cells[2, 2] = OrderTel.Text;

            ExcelApp.Cells[2, 3] = "Имя заказчика";
            ExcelApp.Cells[2, 4] = OrderName.Text;

            ExcelApp.Cells[3, 1] = "Адрес";
            ExcelApp.Cells[3, 2] = OrderAddress.Text;

            ExcelApp.Cells[4, 1] = "Комментарий";
            ExcelApp.Cells[4, 2] = OrderComments.Text;

            ExcelApp.Cells[5, 1] = "Сумма заказа";
            ExcelApp.Cells[5, 2] = labelOrderCost.Content + " руб.";

            // Заголовок
            for (int i = 0; i < dataGridOrderProduct.Columns.Count - 1; i++)
            {
                ExcelApp.Cells[7, (i + 1)] = dataGridOrderProduct.Columns[i].Header;
                ExcelApp.Cells[7, (i + 1)].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
            }

            // Строки
            int row = 8;
            foreach (DataRowView dr in dataGridOrderProduct.ItemsSource)
            {
                var cell1 = ExcelApp.Cells[row, 1];
                var cell2 = ExcelApp.Cells[row, 5];

                ExcelApp.Cells[row, 1] = dr[1];
                ExcelApp.Cells[row, 2] = dr[3];
                ExcelApp.Cells[row, 3] = dr[5] + " руб.";
                ExcelApp.Cells[row, 4] = dr[2] + " шт.";
                ExcelApp.Cells[row, 5] = dr[6] + " руб.";

                ExcelApp.Range[cell1, cell2].Borders.Weight = 2d;
                ExcelApp.Range[cell1, cell2].WrapText = true;
                ExcelApp.Range[cell1, cell2].Cells.VerticalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;
                ExcelApp.Range[cell1, cell2].Cells.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                row++;
            }

            ExcelApp.Visible = true;
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