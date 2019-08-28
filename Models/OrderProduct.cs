using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IShop_Management.Models
{
    class OrderProduct
    {
        public List<Order> Ord_id { get; set; }
        public List<Product> Prd_id { get; set; }
        public int Op_qty { get; set; }
    }
}
