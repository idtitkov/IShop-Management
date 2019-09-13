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
    /// Логика взаимодействия для BuyView.xaml
    /// </summary>
    public partial class BuyView : Window
    {
        SqlDataAdapter adapter;
        DataTable mergedDT;
        public BuyView()
        {
            InitializeComponent();

            datePickerBuyStart.SelectedDate = DateTime.Now;
            datePickerBuyEnd.SelectedDate = DateTime.Now;

            FillDataGrid(null, DateTime.Now, DateTime.Now);
            dataGrid_Buy.DataContext = mergedDT;
        }

        // Наполнение таблицы закупок
        private void FillDataGrid(int? id, DateTime? date1, DateTime? date2)
        {
            SqlCommand cmd = new SqlCommand("dbo.ShowPurchaces", LoginView.connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Prdid", SqlDbType.Int).Value = id ?? Convert.DBNull;
            cmd.Parameters.Add("@Purdate1", SqlDbType.DateTime).Value = date1;
            cmd.Parameters.Add("@Purdate2", SqlDbType.DateTime).Value = date2;

            adapter = new SqlDataAdapter();
            adapter.SelectCommand = cmd;
            mergedDT = new DataTable();
            adapter.Fill(mergedDT);
        }

        private void datePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            WindowActivated(sender, e);
        }

        private void dataGrid_Buy_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void NewPurchase_Click(object sender, RoutedEventArgs e)
        {
            BuyEditView buyEditView = new BuyEditView();
            buyEditView.ShowDialog();
        }

        private void WindowActivated(object sender, EventArgs e)
        {
            try
            {
                dataGrid_Buy.DataContext = null;
                FillDataGrid(null, datePickerBuyStart.SelectedDate.Value.Date, datePickerBuyEnd.SelectedDate.Value.Date);
                dataGrid_Buy.DataContext = mergedDT;
            }
            catch (Exception)
            {
            }
        }
    }
}
