using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
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
    /// Логика взаимодействия для ContentEditWindow.xaml
    /// </summary>
    public partial class ContentEditWindow : Window
    {
        public ContentEditWindow(string prd_name)
        {
            InitializeComponent();

            LoadContentInfo(prd_name);
        }

        public void LoadContentInfo(string p)
        {
            JsonResult jsonResult = new JsonResult();
            //DataTable dt = (DataTable)JsonConvert.DeserializeObject(p, (typeof(DataTable)));
            //var result = JsonConvert.DeserializeObject<List<JsonResult>>(p);
            //DataTable dt = (DataTable)JsonConvert.DeserializeObject(p, (typeof(JsonResult)));
            //DataTable dt = (DataTable)JsonConvert.DeserializeObject<JsonResult>(p);
            //string json = JsonConvert.SerializeObject(jsonResult);


            //DGContent.ItemsSource = dt.DefaultView;
        }
    }

    public class JsonResult
    {
        public string key { get; set; }
        public string value { get; set; }
    }
}
