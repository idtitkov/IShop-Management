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
        public ObservableCollection<Order> DeliveredOrders { get; set; }

        public SqlDataAdapter sda;
        public DataTable dt;

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
            string loadAllOrders = $"SELECT * FROM dbo.orders WHERE Ord_date_created >= '{_beginDate}' AND Ord_date_created < '{_endDate.AddDays(1)}';";
            SqlCommand cmd = new SqlCommand(loadAllOrders, LoginView.connection);
            sda = new SqlDataAdapter(cmd);
            dt = new DataTable();
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
                    Ord_date_delivereed = dr[9] as DateTime?
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

        private void LoadDeliveredOrders()
        {
            DeliveredOrders = new ObservableCollection<Order>(AllOrders.Where(x => x.Ord_status == 2));
        }

        // Возможный вариант сохранения
        public void SaveChanges()
        {
            sda.UpdateCommand = new SqlCommandBuilder(sda).GetUpdateCommand();

            DataTable dt2 = new DataTable();
            dt2 = ToDataTable<Order>(AllOrders);
            dt2.PrimaryKey = new DataColumn[] { dt2.Columns["ord_id"] };
            sda.Update(dt2);
        }

        // Конвертер
        public DataTable ToDataTable<T>(ObservableCollection<T> items)
        {
            var tb = new DataTable(typeof(T).Name);

            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo prop in props)
            {
                Type t = GetCoreType(prop.PropertyType);
                tb.Columns.Add(prop.Name, t);
            }

            foreach (T item in items)
            {
                var values = new object[props.Length];

                for (int i = 0; i < props.Length; i++)
                {
                    values[i] = props[i].GetValue(item, null);
                }

                tb.Rows.Add(values);
            }
            return tb;
        }

        public static bool IsNullable(Type t)
        {
            return !t.IsValueType || (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        public static Type GetCoreType(Type t)
        {
            if (t != null && IsNullable(t))
            {
                if (!t.IsValueType)
                {
                    return t;
                }
                else
                {
                    return Nullable.GetUnderlyingType(t);
                }
            }
            else
            {
                return t;
            }
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