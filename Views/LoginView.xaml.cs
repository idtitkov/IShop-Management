using System.Configuration;
using System.Data.SqlClient;
using System.Windows;

namespace IShop_Management.Views
{
    /// <summary>
    /// Логика взаимодействия для LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        public static string connectionString;
        public static SqlConnection connection;
        private Window okWindow;
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

                // выбор рабочего окна в зависимости от типа пользователя
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
                        okWindow = new ContentView();
                        okWindow.Show();
                        this.Close();
                        break;
                    case 4:
                        okWindow = new BuyView();
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
            catch (SqlException)
            {
                MessageBox.Show(this, "Соединение с сервером отсутствует\nОбратитесь к администратору.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            finally
            {
                if (connection != null)
                    connection.Close();
            }
        }
    }
}
