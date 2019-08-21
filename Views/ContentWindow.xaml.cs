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
    /// Логика взаимодействия для ContentWindow.xaml
    /// </summary>
    public partial class ContentWindow : Window
    {
        public ContentWindow()
        {
            InitializeComponent();

            FillDataGrid();
        }

        private void FillDataGrid()
        {
            using (LoginWindow.connection)
            {
                string showNewOrders = $"SELECT * FROM prd_description;";
                SqlCommand cmd = new SqlCommand(showNewOrders, LoginWindow.connection);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("prd_description");
                sda.Fill(dt);

                DGContent.ItemsSource = dt.DefaultView;
            }
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow dgr = DGContent.ItemContainerGenerator.ContainerFromItem(DGContent.SelectedItem) as DataGridRow;
            DataRowView dr = (DataRowView)dgr.Item;

            string prd_name = dr[2].ToString();

            ContentEditWindow orderEdit = new ContentEditWindow(prd_name);
            orderEdit.Show();

            //MessageBox.Show("You Clicked : \r\nName : " + ProductName + "\r\nDescription : " + ProductDescription);

        }
    }
}
