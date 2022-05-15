using MySql.Data.MySqlClient;

namespace ExoHttpAPI
{
    /// <summary>
    /// Classe permettant de facilier la manipulation de la base de donnée (une sorte d'ORM Custom)
    /// </summary>
    public class DatabaseInterface
    {
        //Spécifiez les informations sur la base de donnée ici:
        private const string mysql_host = "127.0.0.1";
        private const string mysql_port = "21832";
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

        public bool EXECUTE_REQUEST(string sqlRequest)
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
