using System;
using System.Collections.Generic;
using System.Configuration;
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
    /// Логика взаимодействия для LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        public static string connectionString;
        public static SqlConnection connection;
        public LoginView()
        {
            InitializeComponent();
            // получаем строку подключения из app.config
            connectionString = ConfigurationManager.ConnectionStrings["IShopConnection"].ConnectionString;
        }

        // обработка нажатия на кнопку логина
        private void buttonLogin_Click(object sender, RoutedEventArgs e)
        {
            connection = new SqlConnection(connectionString);
            string checkUserTypeSelect = $"SELECT pos_id FROM dbo.employees WHERE emp_username = '{textBoxLogin.Text}' AND emp_pass = '{passwordBoxLogin.Password.ToString()}';";
            int? userType = null;

            try
            {
                connection.Open();

                SqlCommand checkCredentials = new SqlCommand(checkUserTypeSelect, connection);
                userType = (int?)checkCredentials.ExecuteScalar();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(this, ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            finally
            {
                if (connection != null)
                    connection.Close();
            }

            // выбор рабочего окна в зависимости от типа пользователя
            Window okWindow = null;
            switch (userType)
            {
                case 1:
                    //okWindow = new DirectorView();
                    okWindow.Show();
                    this.Close();
                    break;
                case 2:
                    okWindow = new ManagerView();
                    okWindow.Show();
                    this.Close();
                    break;
                case 3:
                    okWindow = new ContentWindow();
                    okWindow.Show();
                    this.Close();
                    break;
                case 4:
                    //okWindow = new BuyerView();
                    okWindow.Show();
                    this.Close();
                    break;
                case 5:
                    okWindow = new WarehouseView();
                    okWindow.Show();
                    this.Close();
                    break;
                default:
                    MessageBox.Show(this, "Неправильное имя пользователя или пароль.\nПовторите попытку или обратитесь к администратору.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;
            }

        }
    }
}
