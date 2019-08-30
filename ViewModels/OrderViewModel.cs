using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using IShop_Management.Models;
using IShop_Management.Views;

namespace IShop_Management.ViewModels
{
    public class OrderViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Order> AllOrders { get; set; }
        public ObservableCollection<Order> NewOrders { get; set; }
        public ObservableCollection<Order> ActiveOrders { get; set; }
        public ObservableCollection<Order> CanceledOrders { get; set; }
        public ObservableCollection<Order> DeliveredOrders { get; set; }

        private DateTime _beginDate = DateTime.Today;
        private DateTime _endDate = DateTime.Today;

        public OrderViewModel()
        {
            Refresh();
        }

        public void Refresh()
        {
            AllOrders = null;

            LoadAllOrders();
            LoadNewOrders();
            LoadActiveOrders();
            LoadCanceledOrders();
            LoadDeliveredOrders();
        }

        public void LoadAllOrders()
        {
            string loadAllOrders = $"SELECT * FROM dbo.orders WHERE Ord_date_created >= '{_beginDate}' AND Ord_date_created < '{_endDate.AddDays(1)}';";
            SqlCommand cmd = new SqlCommand(loadAllOrders, LoginView.connection);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);

            AllOrders = new ObservableCollection<Order>();

            foreach (DataRow dr in dt.Rows)
            {
                AllOrders.Add(new Order
                {
                    Ord_id = Convert.ToInt32(dr[0]),
                    Ord_name = dr[1].ToString(),
                    Ord_tel = dr[2].ToString(),
                    Ord_address = dr[3].ToString(),
                    Ord_email = dr[4].ToString(),
                    Ord_comments = dr[5].ToString(),
                    Ord_date_created = Convert.ToDateTime(dr[6]),
                    Ord_status = Convert.ToInt32(dr[7]),
                    Cur_id = Convert.ToInt32(dr[8]),
                    Ord_date_delivered = dr[9] as DateTime?
                });
            }
        }

        private void LoadNewOrders()
        {
            NewOrders = new ObservableCollection<Order>(AllOrders.Where(x => x.Ord_status == 0));
        }

        private void LoadActiveOrders()
        {
            ActiveOrders = new ObservableCollection<Order>(AllOrders.Where(x => x.Ord_status == 1));
        }

        private void LoadCanceledOrders()
        {
            CanceledOrders = new ObservableCollection<Order>(AllOrders.Where(x => x.Ord_status == 2));
        }

        private void LoadDeliveredOrders()
        {
            DeliveredOrders = new ObservableCollection<Order>(AllOrders.Where(x => x.Ord_status == 3));
        }

        // Возможный вариант сохранения
        public void SaveChanges()
        {
        }

        public DateTime BeginDate
        {
            get { return _beginDate; }
            set
            {
                _beginDate = value;
                OnPropertyChanged("BeginDate");
            }
        }

        public DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                _endDate = value;
                OnPropertyChanged("EndDate");
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}