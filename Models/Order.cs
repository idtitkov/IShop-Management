using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IShop_Management.Models
{
    class Order
    {
        public int Ord_id { get; set; }
        public int? Cli_id { get; set; }
        public string Ord_name { get; set; }
        public string Ord_tel { get; set; }
        public string Ord_address { get; set; }
        public string Ord_comments { get; set; }
        public DateTime Ord_date_created { get; set; }
        public bool Ord_is_new { get; set; }
        public bool Ord_is_active { get; set; }
        public bool Ord_is_delivered { get; set; }
        public int? Cur_id { get; set; }
        public DateTime? Ord_date_delivereed { get; set; }
    }
}
