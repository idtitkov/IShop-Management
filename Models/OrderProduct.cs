using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IShop_Management.Models
{
    public class OrderProduct
    {
        public int Ord_id { get; set; }
        public int Prd_id { get; set; }
        public int Op_qty { get; set; }
    }
}
