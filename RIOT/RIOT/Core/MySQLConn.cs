using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient; 

namespace RIOT
{
    public class MySQLConn
    {
        string user = "game";
        string pass = "1234";
        string server_add = "localhost";
        string DataBaseName = "riot";

        public MySqlConnection conn;

        public MySQLConn(bool is_use_local_host = true)
        {

            if (is_use_local_host)
            {
                user = "root";
                pass = "";
            }

            string conn_str = "Server=" + server_add + ";" +
                                "Database=" + DataBaseName + ";" +
                                "Uid=" + user + ";" +
                                "Pwd=" + pass + ";";

            conn = new MySqlConnection(conn_str);
            conn.Open();
        }
    }
}
