﻿using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace CPUFramework
{
     public class SQLUtility
    {
        public static string ConnectionString = "";

        public static SqlCommand GetSqlCommand(string sprocname)
        {
            SqlCommand cmd;
            using (SqlConnection conn = new SqlConnection(SQLUtility.ConnectionString))
            {
               cmd = new SqlCommand(sprocname, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                Debug.Print(cmd.ToString());

                conn.Open();
                SqlCommandBuilder.DeriveParameters(cmd);
            }
            return cmd;
        }

        public static DataTable GetDataTable(SqlCommand cmd)
        {
            Debug.Print("----------" + Environment.NewLine+cmd.CommandText);
            DataTable dt = new();
            using(SqlConnection conn= new SqlConnection(SQLUtility.ConnectionString))
            {
                conn.Open();
                cmd.Connection= conn;
                SqlDataReader dr= cmd.ExecuteReader();
                dt.Load(dr);

            }
            SetAllColumnsAllowNull(dt);
            return dt;
        }

        public static DataTable GetDataTable(string sqlstatement)
        {
            Debug.Print(sqlstatement);
           return GetDataTable(new SqlCommand(sqlstatement));
        }

        public static void ExecuteSQL(string sqlstatment)
        {
            GetDataTable(sqlstatment);
        }

        public static int GetFirstColumnFirstRowValue(string sql)
        {
            int n = 0;

            DataTable dt = GetDataTable(sql);
            if (dt.Rows.Count > 0 && dt.Columns.Count > 0)
            {
                if (dt.Rows[0][0] != DBNull.Value)
                {
                    int.TryParse(dt.Rows[0][0].ToString(), out n);
                }
            }
            return n;
        }

        private static void SetAllColumnsAllowNull(DataTable dt)
        {
            foreach (DataColumn c in dt.Columns)
            {
                c.AllowDBNull = true;
            }
        }

        public static void DebugPrintDataTable(DataTable dt)
        {
            foreach (DataRow r in dt.Rows)
            {
                foreach (DataColumn c in dt.Columns)
                {
                    Debug.Print(c.ColumnName + " = " + r[c.ColumnName].ToString());
                }
            }
        }

    }
}
 