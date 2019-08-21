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
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public static string connectionString;
        public static SqlConnection connection;

        public LoginWindow()
        {
            InitializeComponent();
            // получаем строку подключения из app.config
            connectionString = ConfigurationManager.ConnectionStrings["IShopConnection"].ConnectionString;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            connection = new SqlConnection(connectionString);
            string checkUserSelect = $"SELECT pos_id FROM dbo.employees WHERE emp_username = '{TBLogin.Text}' AND emp_pass = '{TBPassword.Password.ToString()}';";

            try
            {
                connection.Open();

                SqlCommand checkUser = new SqlCommand(checkUserSelect, connection);
                int answer = (int)checkUser.ExecuteScalar();

                if (answer == 2)
                {
                    var ok = new ManagerWindow();
                    ok.Show();
                    this.Close();
                }
                else if (answer == 3)
                {
                    var ok = new ContentWindow();
                    ok.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show(this, "Неправильное имя пользователя или пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (SqlException ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (connection != null)
                    connection.Close();
            }
        }
    }
}
