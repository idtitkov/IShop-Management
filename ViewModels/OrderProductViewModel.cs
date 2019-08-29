using IShop_Management.Models;
using IShop_Management.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IShop_Management.ViewModels
{
    public class OrderProductViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<OrderProduct> ProductsInOrder { get; set; }
        public SqlDataAdapter sda;
        public DataTable dt;

        public OrderProductViewModel(int ord_id)
        {
            LoadProductsInOrder(ord_id);
        }

        void LoadProductsInOrder(int ord_id)
        {
            string loadAllOrders = $"SELECT * FROM dbo.m2m_orders_products WHERE ord_id = {ord_id};";
            SqlCommand cmd = new SqlCommand(loadAllOrders, LoginView.connection);
            sda = new SqlDataAdapter(cmd);
            dt = new DataTable();
            sda.Fill(dt);

            ProductsInOrder = new ObservableCollection<OrderProduct>();

            foreach (DataRow dr in dt.Rows)
            {
                ProductsInOrder.Add(new OrderProduct
                {
                    Ord_id = Convert.ToInt32(dr[0]),
                    Prd_id = Convert.ToInt32(dr[1]),
                    Op_qty = Convert.ToInt32(dr[2])

                });
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

