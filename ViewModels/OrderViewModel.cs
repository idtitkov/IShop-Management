using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IShop_Management.Models;
using IShop_Management.Views;

namespace IShop_Management.ViewModels
{
    class OrderViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Order> AllOrders { get; set; }
        public ObservableCollection<Order> NewOrders { get; set; }
        public ObservableCollection<Order> ActiveOrders { get; set; }
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
            LoadDeliveredOrders();
        }

        public void LoadAllOrders()
        {
            string loadAllOrders = $"SELECT * FROM dbo.orders WHERE Ord_date_created >= '{_beginDate}' AND Ord_date_created <= '{_endDate}';";
            SqlCommand cmd = new SqlCommand(loadAllOrders, LoginView.connection);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            sda.Fill(ds, "orders");

            if (AllOrders == null)
                AllOrders = new ObservableCollection<Order>();

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                AllOrders.Add(new Order
                {
                    Ord_id = Convert.ToInt32(dr[0]),
                    Cli_id = dr[1] as int?,
                    Ord_name = dr[2].ToString(),
                    Ord_tel = dr[3].ToString(),
                    Ord_address = dr[4].ToString(),
                    Ord_comments = dr[5].ToString(),
                    Ord_date_created = Convert.ToDateTime(dr[6]),
                    Ord_is_new = Convert.ToBoolean(dr[7]),
                    Ord_is_active = Convert.ToBoolean(dr[8]),
                    Ord_is_delivered = Convert.ToBoolean(dr[9]),
                    Cur_id = dr[10] as int?,
                    Ord_date_delivereed = dr[4] as DateTime?
                });
            }
        }
        private void LoadNewOrders()
        {
            NewOrders = new ObservableCollection<Order>(AllOrders.Where(x => x.Ord_is_new == true));
        }
        private void LoadActiveOrders()
        {
            ActiveOrders = new ObservableCollection<Order>(AllOrders.Where(x => x.Ord_is_active == true));
        }
        private void LoadDeliveredOrders()
        {
            DeliveredOrders = new ObservableCollection<Order>(AllOrders.Where(x => x.Ord_is_delivered == true));
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