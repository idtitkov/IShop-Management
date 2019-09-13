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
    /// Логика взаимодействия для BuyEditView.xaml
    /// </summary>
    public partial class BuyEditView : Window
    {
        public BuyEditView()
        {
            InitializeComponent();

            LoadPartners();
            GetNewId();
        }

        private void LoadPartners()
        {
            if (LoginView.connection.State == ConnectionState.Closed)
                LoginView.connection.Open();

            string loadCatName = $"SELECT par_name FROM dbo.partners;";
            SqlCommand sqlLoadCatName = new SqlCommand(loadCatName, LoginView.connection);
            SqlDataReader sqlReader = sqlLoadCatName.ExecuteReader();

            while (sqlReader.Read())
            {
                cb_Partners.Items.Add(sqlReader["par_name"].ToString());
            }

            sqlReader.Close();
        }

        // Получаем новый id
        private void GetNewId()
        {
            if (LoginView.connection.State == ConnectionState.Closed)
                LoginView.connection.Open();

            string getLastPurId = $"SELECT MAX(pur_id) FROM dbo.purchase;";
            SqlCommand sqlGetLastPurId = new SqlCommand(getLastPurId, LoginView.connection);
            int lastPurId = Convert.ToInt32(sqlGetLastPurId.ExecuteScalar());
            lastPurId++;

            texbox_ProdId.Text = lastPurId.ToString();
        }

        private void Button_AddProduct_Click(object sender, RoutedEventArgs e)
        {
            if (LoginView.connection.State == ConnectionState.Closed)
                LoginView.connection.Open();

            try
            {
                string getProdName = $"SELECT prd_name FROM dbo.products WHERE prd_id=@prd_id;";
                SqlCommand sqlGetProdName = new SqlCommand(getProdName, LoginView.connection);
                sqlGetProdName.Parameters.AddWithValue("@prd_id", Convert.ToInt32(tb_ProdId.Text));

                texbox_ProdName.Text = sqlGetProdName.ExecuteScalar().ToString();
            }
            catch (Exception)
            {
                texbox_ProdName.Text = null;
            }
        }

        // сохраняем новую накладную
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            double priceIn;
            double priceOut;
            int qty;

            if (double.TryParse(tb_PriceIn.Text, out priceIn)
                && double.TryParse(tb_PriceOut.Text, out priceOut)
                && int.TryParse(tb_Qty.Text, out qty)
                && texbox_ProdName.Text.Length != 0)
            {
                if (LoginView.connection.State == ConnectionState.Closed)
                    LoginView.connection.Open();

                string getProdId = $"SELECT prd_id FROM dbo.products WHERE prd_name=@prd_name;";
                SqlCommand sqlGetProdId = new SqlCommand(getProdId, LoginView.connection);
                sqlGetProdId.Parameters.AddWithValue("@prd_name", texbox_ProdName.Text);
                tb_ProdId.Text = sqlGetProdId.ExecuteScalar().ToString();

                string insertPur = @"INSERT INTO dbo.purchase (pur_date, prd_id, pur_qty, par_id, prd_price_in)
                VALUES (@pur_date, @prd_id, @pur_qty, @par_id, @prd_price_in);";
                SqlCommand SqlinsertProd = new SqlCommand(insertPur, LoginView.connection);
                SqlinsertProd.Parameters.AddWithValue("@pur_date", dp_NewBuy.SelectedDate);
                SqlinsertProd.Parameters.AddWithValue("@prd_id", Convert.ToInt32(tb_ProdId.Text));
                SqlinsertProd.Parameters.AddWithValue("@pur_qty", qty);
                SqlinsertProd.Parameters.AddWithValue("@par_id", cb_Partners.SelectedIndex + 1);
                SqlinsertProd.Parameters.AddWithValue("@prd_price_in", priceIn);
                SqlinsertProd.ExecuteNonQuery();

                string updatePriceIn = @"UPDATE dbo.products SET prd_price_out=@prd_price_out WHERE prd_id=@prd_id;";
                SqlCommand SqlUpdatePriceIn = new SqlCommand(updatePriceIn, LoginView.connection);
                SqlUpdatePriceIn.Parameters.AddWithValue("@prd_price_out", priceOut);
                SqlUpdatePriceIn.Parameters.AddWithValue("@prd_id", Convert.ToInt32(tb_ProdId.Text));
                SqlUpdatePriceIn.ExecuteNonQuery();

                this.Close();
            }
            else
            {
                MessageBox.Show(this, "Проверьте корректность введенных данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
