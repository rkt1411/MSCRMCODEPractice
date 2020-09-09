using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateTableInDB
{
    class Program
    {
        
        private static string connetionString;

        static void Main(string[] args)
        {
            string tableName = "STUDENT";
            DataTable table = null;

            connetionString = "Data Source=GHYDMS1055406D;Initial Catalog=MSCRM365;Trusted_Connection=true";

            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = connetionString;
                conn.Open();
           string result =  CreateTABLE(tableName, table);
                Console.WriteLine("Connection Open ! ");


                
            }
            
        }

        public static string CreateTABLE(string DDLG, DataTable table)
        {
            string sqlsc;
            sqlsc = "CREATE TABLE " + DDLG + "(";
            for (int i = 0; i < table.Columns.Count; i++)
            {
                sqlsc += "\n [" + table.Columns[i].ColumnName + "] ";
                string columnType = table.Columns[i].DataType.ToString();
                switch (columnType)
                {
                    case "System.Int32":
                        sqlsc += " int ";
                        break;
                    case "System.Int64":
                        sqlsc += " bigint ";
                        break;
                    case "System.Int16":
                        sqlsc += " smallint";
                        break;
                    case "System.Byte":
                        sqlsc += " tinyint";
                        break;
                    case "System.Decimal":
                        sqlsc += " decimal ";
                        break;
                    case "System.DateTime":
                        sqlsc += " datetime ";
                        break;
                    case "System.String":
                    default:
                        sqlsc += string.Format(" nvarchar({0}) ", table.Columns[i].MaxLength == -1 ? "max" : table.Columns[i].MaxLength.ToString());
                        break;
                }
                if (table.Columns[i].AutoIncrement)
                    sqlsc += " IDENTITY(" + table.Columns[i].AutoIncrementSeed.ToString() + "," + table.Columns[i].AutoIncrementStep.ToString() + ") ";
                if (!table.Columns[i].AllowDBNull)
                    sqlsc += " NOT NULL ";
                sqlsc += ",";
            }
            return sqlsc.Substring(0, sqlsc.Length - 1) + "\n)";

        }
    }
}
