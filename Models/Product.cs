using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IShop_Management.Models
{
    public class Product
    {
        public int Prd_id { get; set; }
        public string Prd_name { get; set; }
        public int Prd_qty { get; set; }
        public double Prd_price_out { get; set; }
    }
}
