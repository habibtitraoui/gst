using System.Data.SqlClient;

namespace gst
{
    internal class DB_CONNECT
    {
        private SqlConnection connection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\SUPERMARKET.mdf;Integrated Security=True;Connect Timeout=30");
        public SqlConnection GetCon()
        {
            return connection;
        }
        public void Opencon()
        {
            if (connection.State == System.Data.ConnectionState.Closed)
            {
                connection.Open();

            }
        }
        public void Closecon()
        {
            if (connection.State == System.Data.ConnectionState.Open)
            {
                connection.Close();
            }
        }
    }
}
