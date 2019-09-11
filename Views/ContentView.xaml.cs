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
    /// Логика взаимодействия для ContentView.xaml
    /// </summary>
    public partial class ContentView : Window
    {
        SqlDataAdapter adapter;
        DataTable mergedDT;
        public ContentView()
        {
            InitializeComponent();

            FillDataGrid(null, null);
            dataGrid_Content.DataContext = mergedDT;
        }

        private void FillDataGrid(int? id, string name)
        {
            SqlCommand cmd = new SqlCommand("dbo.ShowContent", LoginView.connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Prdid", SqlDbType.Int).Value = id ?? Convert.DBNull;
            cmd.Parameters.Add("@Prodname", SqlDbType.VarChar).Value = name;

            adapter = new SqlDataAdapter();
            adapter.SelectCommand = cmd;
            mergedDT = new DataTable();
            adapter.Fill(mergedDT);
        }

        private void dataGridContent_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void SearchById_Click(object sender, RoutedEventArgs e)
        {
            int prodId;
            if (int.TryParse(texbox_ProdId.Text, out prodId))
            {
                prodId = Convert.ToInt32(texbox_ProdId.Text);

                dataGrid_Content.DataContext = null;
                FillDataGrid(prodId, null);
                dataGrid_Content.DataContext = mergedDT;
            }
            else
            {
                texbox_ProdId.Text = null;

                dataGrid_Content.DataContext = null;
                FillDataGrid(null, null);
                dataGrid_Content.DataContext = mergedDT;
            }
            texbox_ProdName.Text = null;
        }

        private void SearchByName_Click(object sender, RoutedEventArgs e)
        {
            texbox_ProdId.Text = null;

            dataGrid_Content.DataContext = null;
            FillDataGrid(null, texbox_ProdName.Text);
            dataGrid_Content.DataContext = mergedDT;
        }
    }
}