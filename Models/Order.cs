using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IShop_Management.Models
{
    public class Order : INotifyPropertyChanged
    {
        private int ord_id;
        private string ord_name;
        private string ord_tel;
        private string ord_address;
        private string ord_comments;
        private DateTime ord_date_created;
        private int ord_status;
        private int cur_id;
        private DateTime? ord_date_delivereed;
        public int Ord_id
        {
            get { return ord_id; }
            set
            {
                ord_id = value;
                OnPropertyChanged("Ord_id");
            }
        }
        public string Ord_name
        {
            get { return ord_name; }
            set
            {
                ord_name = value;
                OnPropertyChanged("Ord_name");
            }
        }
        public string Ord_tel
        {
            get { return ord_tel; }
            set
            {
                ord_tel = value;
                OnPropertyChanged("Ord_tel");
            }
        }
        public string Ord_address
        {
            get { return ord_address; }
            set
            {
                ord_address = value;
                OnPropertyChanged("Ord_address");
            }
        }
        public string Ord_comments
        {
            get { return ord_comments; }
            set
            {
                ord_comments = value;
                OnPropertyChanged("Ord_comments");
            }
        }
        public DateTime Ord_date_created
        {
            get { return ord_date_created; }
            set
            {
                ord_date_created = value;
                OnPropertyChanged("Ord_date_created");
            }
        }
        public int Ord_status
        {
            get { return ord_status; }
            set
            {
                ord_status = value;
                OnPropertyChanged("Ord_status");
            }
        }
        public int Cur_id
        {
            get { return cur_id; }
            set
            {
                cur_id = value;
                OnPropertyChanged("Cur_id");
            }
        }
        public DateTime? Ord_date_delivereed
        {
            get { return ord_date_delivereed; }
            set
            {
                ord_date_delivereed = value;
                OnPropertyChanged("Ord_date_delivereed");
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
