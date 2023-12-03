using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using MovieModel;

namespace MovieDataAccess
{
    public class UserConnector :Connector
    {
        public List<User> GetAllUsers()
        {
            List<User> users = new List<User>();
            string sql = "select * from [User]";
            open();
            SqlDataReader reader = Query(sql);
            while (reader.Read())
            {
                User us = new User();
                us.UserId = reader.GetInt32(0);
                us.UserName = reader.GetString(1);
                users.Add(us);
            }
            reader.Close();
            close();
            return users;
        }
        public User GetUser(int userId)
        {
            string sql = "select * from [User] where UserId = @userId";
            SqlParameter parUserId = new SqlParameter("@userId", SqlDbType.Int);
            parUserId.Value = userId;
            User us = null;
            open();
            SqlDataReader r = Query(sql, new[] {parUserId});
            if (r.Read())
            {
                us = new User();
                us.UserId = r.GetInt32(0);
                us.UserName = r.GetString(1);
            }
            r.Close();
            close ();
            return us;
           
        }
    }
}
