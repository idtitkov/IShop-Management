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
    /// Логика взаимодействия для ContentEditView.xaml
    /// </summary>
    public partial class ContentEditView : Window
    {
        string ProdId;
        string ProdName;
        string ProdCat;
        string ProdDescr;
        public ContentEditView(string ProdId, string ProdName, string ProdCat, string ProdDescr)
        {
            this.ProdId = ProdId;
            this.ProdName = ProdName;
            this.ProdCat = ProdCat;
            this.ProdDescr = ProdDescr;

            InitializeComponent();

            FillWindow();
        }
        private void FillWindow()
        {
            LoadCategories();
            //LoadName();
            //LoadDescription();

            texbox_ProdId.Text = ProdId;
            texbox_ProdName.Text = ProdName;
            if (ProdCat == null)
                cb_Categories.SelectedIndex = 0;
            else
                cb_Categories.SelectedItem = ProdCat;
            texbox_ProdDescr.Text = ProdDescr;
        }

        private void LoadCategories()
        {
            if (LoginView.connection.State == ConnectionState.Closed)
                LoginView.connection.Open();

            string loadCatName = $"SELECT cat_name FROM dbo.prd_categories;";
            SqlCommand sqlLoadCatName = new SqlCommand(loadCatName, LoginView.connection);
            SqlDataReader sqlReader = sqlLoadCatName.ExecuteReader();

            while (sqlReader.Read())
            {
                cb_Categories.Items.Add(sqlReader["cat_name"].ToString());
            }

            sqlReader.Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (LoginView.connection.State == ConnectionState.Closed)
                LoginView.connection.Open();

            // Проверяем есть ли такой товар
            string getLastProdId = $"SELECT MAX(prd_id) FROM dbo.products;";
            SqlCommand sqlGetLastProdId = new SqlCommand(getLastProdId, LoginView.connection);
            int lastProdId = Convert.ToInt32(sqlGetLastProdId.ExecuteScalar());

            // Если нет таких товаров, то записываем новое в базу
            if (Convert.ToInt32(ProdId) > lastProdId)
            {
                string insertProd = @"INSERT INTO dbo.products (prd_name)
                VALUES (@prd_name);";
                SqlCommand SqlinsertProd = new SqlCommand(insertProd, LoginView.connection);
                SqlinsertProd.Parameters.AddWithValue("@prd_name", texbox_ProdName.Text);
                SqlinsertProd.ExecuteNonQuery();

                string insertDescr = @"INSERT INTO dbo.prd_description (prd_id, cat_id, prd_descr)
                VALUES (@prd_id, @cat_id, @prd_descr);";
                SqlCommand SqlInsertDescr = new SqlCommand(insertDescr, LoginView.connection);
                SqlInsertDescr.Parameters.AddWithValue("@prd_id", Convert.ToInt32(ProdId));
                SqlInsertDescr.Parameters.AddWithValue("@cat_id", cb_Categories.SelectedIndex + 1);
                SqlInsertDescr.Parameters.AddWithValue("@prd_descr", texbox_ProdDescr.Text);
                SqlInsertDescr.ExecuteNonQuery();
            }
            // иначе сохраняем текущее
            else
            {
                string updateDescr = "UPDATE dbo.prd_description SET cat_id=@cat_id, prd_descr=@prd_descr WHERE prd_id=@prd_id";
                SqlCommand SqlupdateDescr = new SqlCommand(updateDescr, LoginView.connection);
                SqlupdateDescr.Parameters.AddWithValue("@cat_id", cb_Categories.SelectedIndex + 1);
                SqlupdateDescr.Parameters.AddWithValue("@prd_descr", texbox_ProdDescr.Text);
                SqlupdateDescr.Parameters.AddWithValue("@prd_id", Convert.ToInt32(ProdId));
                SqlupdateDescr.ExecuteNonQuery();

                string updateProd = "UPDATE dbo.products SET prd_name=@prd_name WHERE prd_id=@prd_id";
                SqlCommand SqlupdateProd = new SqlCommand(updateProd, LoginView.connection);
                SqlupdateProd.Parameters.AddWithValue("@prd_name", texbox_ProdName.Text);
                SqlupdateProd.Parameters.AddWithValue("@prd_id", Convert.ToInt32(ProdId));
                SqlupdateProd.ExecuteNonQuery();
            }

            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
