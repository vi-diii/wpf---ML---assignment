using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using SalesDataModel;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;

namespace SalesDataAccess
{
    public class CustomerConnector: Connector
    {
        public List<Customer> GetAllCustomer()
        {
           List<Customer> result = new List<Customer>();
            string sql = "select * from Customers";
            Open();
            SqlDataReader reader = Query(sql);
            while (reader.Read())
            {
                Customer customer = new Customer();
                customer.CusTomerId = (uint)reader.GetInt32(0);
                customer.ContactName = reader.GetString(1);
               result.Add(customer);
            }
            reader.Close();
            Close();
            return result;
        }
          public Customer GetCustomerById(int id)
        {
            string sql = "select * from Customers where CustomerID=@customerId";
            SqlParameter parameter = new SqlParameter("@customerId", SqlDbType.Int);
            parameter.Value = id;
            Customer customer = null;
            Open();
            SqlDataReader reader = Query(sql, new[] {parameter});
            if (reader.Read())
            {
                 customer = new Customer();
                customer.CusTomerId =(uint)reader.GetInt32(0);
                customer.ContactName = reader.GetString(1);
                
                
            }
            reader.Close();
            Close ();
            return customer;
        }
        public Customer Login(string userName, string passWord)
        {
            string sql = "select * from [Customers] where UserName=@userName and Password=@passWord ";
            Open();
            SqlParameter prUser = new SqlParameter("@userName", SqlDbType.NVarChar);
            prUser.Value = userName;
            SqlParameter prPassword = new SqlParameter("@passWord", SqlDbType.NVarChar);
            prPassword.Value = passWord;
            
            SqlDataReader reader = Query(sql, new SqlParameter[] { prUser, prPassword});
            if (reader.Read())
            {
               Customer user = new Customer();
                user.CusTomerId = (uint)reader.GetInt32(0);
                

                user.Password = reader.IsDBNull(5) ? "" : reader.GetString(5);
                user.Username = reader.IsDBNull(4) ? "" : reader.GetString(4);
                

                return user;
            }
            Close();
            return null;
        }
    }
}
