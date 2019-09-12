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

        // Наполнение таблицы описаний
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

        // Редактирование описания
        private void dataGridContent_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataRowView dataRowView = (DataRowView)dataGrid_Content.SelectedItem;
            ContentEditView contentEditView = new ContentEditView(
                dataRowView.Row[0].ToString(),
                dataRowView.Row[1].ToString(),
                dataRowView.Row[2].ToString(),
                dataRowView.Row[3].ToString()
                );
            contentEditView.ShowDialog();
        }

        // Новое описание
        private void NewProduct_Click(object sender, RoutedEventArgs e)
        {
            //string getNewOrdId = $"SELECT MAX(prd_id) FROM dbo.products;";
            string getNewOrdId = $"SELECT IDENT_CURRENT ('dbo.products');";
            SqlCommand sqlGetNewProdId = new SqlCommand(getNewOrdId, LoginView.connection);
            if (LoginView.connection.State == ConnectionState.Closed)
                LoginView.connection.Open();

            ContentEditView contentEditView = new ContentEditView(
                (Convert.ToInt32(sqlGetNewProdId.ExecuteScalar()) + 1).ToString(),
                null,
                null,
                null
                );
            contentEditView.ShowDialog();
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

        // Обноевление окна при возврате из редактирования
        private void WindowActivated(object sender, EventArgs e)
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
                dataGrid_Content.DataContext = null;
                FillDataGrid(null, texbox_ProdName.Text);
                dataGrid_Content.DataContext = mergedDT;
            }
        }

        // Удаление товара
        private void dataGrid_Content_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            if (dg != null)
            {
                DataRowView dataRowView = (DataRowView)dg.SelectedItem;
                int idToDelete = (int)dataRowView.Row[0];
                if (e.Key == Key.Delete)
                {
                    // User is attempting to delete the row
                    var result = MessageBox.Show(
                        "Текущий товар будет удален!\n\nПродолжить?",
                        "Удаление",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question,
                        MessageBoxResult.No);
                    if (result != MessageBoxResult.No)
                    {
                        if (LoginView.connection.State == ConnectionState.Closed)
                            LoginView.connection.Open();

                        string deleteDescr = @"DELETE FROM dbo.prd_description WHERE prd_id=@prd_id;";
                        SqlCommand SqlDeleteDescr = new SqlCommand(deleteDescr, LoginView.connection);
                        SqlDeleteDescr.Parameters.AddWithValue("@prd_id", idToDelete);
                        SqlDeleteDescr.ExecuteNonQuery();

                        string deleteProd = @"DELETE FROM dbo.products WHERE prd_id=@prd_id;";
                        SqlCommand SqlDeleteProd = new SqlCommand(deleteProd, LoginView.connection);
                        SqlDeleteProd.Parameters.AddWithValue("@prd_id", idToDelete);
                        SqlDeleteProd.ExecuteNonQuery();

                        WindowActivated(sender, e);
                    }
                    e.Handled = (result == MessageBoxResult.No);
                }
            }
        }
    }
}