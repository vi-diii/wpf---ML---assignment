using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesDataModel
{
    public class Product
    {
        public uint ProductId { get; set; }
        public string ProductName { get; set; }
        public int CategoryID { get; set; }
        public float unitPrice { get; set; }
        public int Quantity { get; set; }
    }
}
