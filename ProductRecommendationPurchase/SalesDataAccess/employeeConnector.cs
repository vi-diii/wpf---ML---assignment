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
    public class employeeConnector: Connector
    {
        public List<Employee> GetUsers()
        {
            string sql = "select * from [Employees]";
            Open();
            SqlDataReader reader = Query(sql);
            List<Employee> users = new List<Employee>();
            while (reader.Read())
            {
                Employee user = new Employee();
                user.EmployeeId = reader.GetInt32(0);
                user.FirstName = reader.IsDBNull(2) ? "" : reader.GetString(2);
                
                user.Password = reader.IsDBNull(5) ? "" : reader.GetString(5);
                user.UserName = reader.IsDBNull(4) ? "" : reader.GetString(4);
                users.Add(user);
            }
            reader.Close();
            Close();
            return users;
        }
        public Employee GetUser(int Employeeid)
        {
            string sql = "select * from [User] where EmployeeID = @Employeeid";
            Open();
            SqlParameter par = new SqlParameter("@Employeeid", SqlDbType.Int);
            par.Value = Employeeid;
            SqlDataReader reader = Query(sql, new SqlParameter[] { par });
            if (reader.Read())
            {
                Employee user = new Employee();
                user.EmployeeId = reader.GetInt32(0);
                user.FirstName = reader.IsDBNull(2) ? "" : reader.GetString(2);

                user.Password = reader.IsDBNull(5) ? "" : reader.GetString(5);
                user.UserName = reader.IsDBNull(4) ? "" : reader.GetString(4);
                
                return user;
            }
            Close();
            return null;
        }
        public Employee Login(string userName, string passWord, string role)
        {
            string sql = "select * from [Employees] where UserName=@userName and Password=@passWord ";
            Open();
            SqlParameter prUser = new SqlParameter("@userName", SqlDbType.NVarChar);
            prUser.Value = userName;
            SqlParameter prPassword = new SqlParameter("@passWord", SqlDbType.NVarChar);
            prPassword.Value = passWord;
            SqlParameter prRole = new SqlParameter("@role", SqlDbType.NVarChar);
            prPassword.Value = role;
            SqlDataReader reader = Query(sql, new SqlParameter[] { prUser, prPassword, prRole });
            if (reader.Read())
            {
                Employee user = new Employee();
                user.EmployeeId = reader.GetInt32(0);
                user.FirstName = reader.IsDBNull(2) ? "" : reader.GetString(2);

                user.Password = reader.IsDBNull(5) ? "" : reader.GetString(5);
                user.UserName = reader.IsDBNull(4) ? "" : reader.GetString(4);
                user.Role = reader.IsDBNull(3) ? "" : reader.GetString(3);


                return user;
            }
            Close();
            return null;
        }
    }
}
