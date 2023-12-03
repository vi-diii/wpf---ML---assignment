using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesDataModel
{
    public class DataEntry
    {
        [KeyType(count:2155)]
        public uint CustomerId { get; set; }
        [KeyType(count:2155)]
        public uint ProductID { get; set; }
        public float Label { get; set; }    

    }
}
