using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SalesDataModel;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Data.SqlClient;

namespace SalesDataAccess
{
    public class ProductConnector : Connector
    {
        public List<Product> GetAllProducts()
        {
            List<Product> products = new List<Product>();
            string sql = "select * from Products";
            Open();
            SqlDataReader reader = Query(sql);
            while (reader.Read())
            {
                Product product = new Product();
                product.ProductId = (uint)reader.GetInt32(0);
                product.ProductName= reader.GetString(1);
            products.Add(product);
            }
            reader.Close();
            Close();
            return products;
        }
        public Product GetProduct(int id)
        {
            string sql = "select * from Products where ProductID=@productId";
            SqlParameter par = new SqlParameter("@productId",SqlDbType.Int);
            par.Value = id;
            Product product = null;
            Open();
            SqlDataReader reader = Query(sql, new[] {par});
            if (reader.Read())
            {
                product = new Product();
                product.ProductId = (uint)reader.GetInt32(0);
                product.ProductName = reader.GetString(1);
            }
            reader.Close();
            Close();
            return product;
        }
    }
}
