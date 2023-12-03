using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SalesDataModel;
using System.Data.SqlClient;
using SalesDataModel;
using Microsoft.Data.SqlClient;

namespace SalesDataAccess
{
    public class DataEntryConnector : Connector
    {
        public List<DataEntry> GetDataEntries()
        {
            List < DataEntry > List = new List<DataEntry>();
            string sql = "SELECT c.CustomerID, od.ProductID " +
                "FROM Customers as c, Orders As o, [Order Details] as od " +
                "WHERE c.CustomerID = o.CustomerID " +
                "and o.OrderID = od.OrderID";
            Open();
            SqlDataReader reader = Query(sql);
            while (reader.Read())
            {
                DataEntry entry = new DataEntry();
                entry.CustomerId = (uint)reader.GetInt32(0);
                entry.ProductID = (uint)reader.GetInt32(1);
                List.Add(entry);
            }
            reader.Close();
            Close();
            return List;

        }

    }
}
