using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RIOT
{
    class ItemDetailRow
    {
        private string id;
        private string name;
        private string img;
        private string description;

        public ItemDetailRow(string id, string name, string description, string img)
        {
            this.id = id;
            this.name = name;
            this.img = img;
            this.description = description;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("ID: {0} , Name: {1} ", id, name);
            sb.AppendLine();
            sb.AppendFormat("Full img path: {0}", img); 
            sb.AppendLine();
            sb.AppendLine("Description:");
            sb.AppendLine(description); 

            return base.ToString();
        }

        public string get_sql()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("(");
            sb.Append("'" + this.id + "', ");
            sb.Append("'" + this.name.Replace("'", "") + "', ");
            sb.Append("'" + this.img + "', ");
            sb.Append("'" + this.description.Replace("'","") + "'");
            sb.Append(")");

            return sb.ToString();
        }

        internal void submit_to_db()
        {
            MySQLConn myConn = new MySQLConn();
            string query = "insert into item_details values " + get_sql();
            var cmd = new MySqlCommand(query, myConn.conn);
            cmd.ExecuteNonQuery();
            myConn.conn.Close();
        }

        internal void submit_tag_row(string tag)
        {
            MySQLConn myConn = new MySQLConn();
            string query = "insert into item_tags values ( " + this.id + ", '" + tag.Replace("'", "") + "' ) ";
            var cmd = new MySqlCommand(query, myConn.conn);
            cmd.ExecuteNonQuery();
            myConn.conn.Close();
        }
    }
}
