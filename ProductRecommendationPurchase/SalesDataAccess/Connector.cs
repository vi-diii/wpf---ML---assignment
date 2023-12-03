using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;


namespace SalesDataAccess
{
    public class Connector
    {
        public string ConnectionString { get;set; }
        public SqlConnection Connection { get;set; }
        public Connector() 
        {
            ConnectionString = "server=DESKTOP-B2E1TM7;database=Midterm;uid=sa;pwd=123456;TrustServerCertificate=true";
            Connection = new SqlConnection(ConnectionString);
        }
        public Connector(string connectionString)
        {
            ConnectionString = connectionString;
            Connection = new SqlConnection(ConnectionString);
        }
        public Connector(string server, string database, string uid, string pwd)
        {
            ConnectionString = "server=" + server + ";database=" + database + ";uid=" + uid + ";pwd=" + pwd;
            Connection = new SqlConnection(ConnectionString);
        }
        public void Open()
        {
            if (Connection == null) { Connection = new SqlConnection(ConnectionString); }
            if (Connection.State == ConnectionState.Closed) Connection.Open();
        }
        public void Close() { if (Connection != null && Connection.State == ConnectionState.Open ) { Connection.Close(); } }
        public SqlDataReader Query(string query)
        {
            SqlCommand cmd = new SqlCommand(query, Connection);
            SqlDataReader r = cmd.ExecuteReader();
            return r;
        }
        public SqlDataReader Query(string query,  SqlParameter[] parameters)
        {
            SqlCommand cmd = new SqlCommand(query, Connection);
            cmd.Parameters.AddRange(parameters);
            SqlDataReader reader = cmd.ExecuteReader();
            return reader;
        }
    }
}
