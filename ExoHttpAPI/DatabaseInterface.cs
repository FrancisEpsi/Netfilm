using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace ExoHttpAPI
{
    public class DatabaseInterface
    {
        private const string mysql_host = "127.0.0.1";
        private const string mysql_port = "3306";
        private const string mysql_database = "netfilm";
        private const string mysql_user = "backend";
        private const string mysql_password = "dotnet33";

        public DatabaseInterface() {
            
        }

        private MySqlConnection Connect()
        {
            string connectionString = "Server="+mysql_host+";Database="+mysql_database+";port="+mysql_port+";User Id="+mysql_user+";password="+mysql_password;
            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                conn.Open();
                return conn;
            } catch
            {
                return null;
            }
        }

        public bool INSERT_INTO(string sqlRequest)
        {
            MySqlConnection conn = Connect();
            if (conn == null) { return false; }
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = sqlRequest;
            try
            {
                cmd.ExecuteNonQuery();
                conn.Close();
                return true;
            } catch
            {
                return false;
            }
        }

        public MySqlDataReader SELECT(string sqlRequest)
        {
            MySqlConnection conn = Connect();
            if (conn == null) { return null; }
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = sqlRequest;
            try
            {
                MySqlDataReader response = cmd.ExecuteReader();
                return response;
            }
            catch
            {
                return null;
            }
        }
    }
}
