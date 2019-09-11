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
    /// Логика взаимодействия для FindByNameWindow.xaml
    /// </summary>
    public partial class FindByNameWindow : Window
    {
        public SqlDataAdapter sda;
        public DataTable dt;

        public FindByNameWindow(string partOfName)
        {
            InitializeComponent();

            FillDatagrid(partOfName);
        }

        private void FillDatagrid(string partOfName)
        {
            string loadAllOrders = $"SELECT TOP 100 * FROM dbo.products WHERE prd_name LIKE '%{partOfName}%';";
            SqlCommand cmd = new SqlCommand(loadAllOrders, LoginView.connection);
            sda = new SqlDataAdapter(cmd);
            dt = new DataTable();
            sda.Fill(dt);

            dataGrid_FindByName.DataContext = dt.DefaultView;
        }

        // Добавление товара по id в окно заказа
        public event Action<string> ProdId;
        public void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataRowView dataRowView = (DataRowView)dataGrid_FindByName.SelectedItem;
            if (ProdId != null)
                ProdId(dataRowView.Row[0].ToString());
            this.Close();
        }
    }
}
