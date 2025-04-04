﻿using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;

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
            
            DataTable dt = new();
            using(SqlConnection conn= new SqlConnection(SQLUtility.ConnectionString))
            {
                conn.Open();
                cmd.Connection= conn;
                Debug.Print(GetSQL(cmd));
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

        public static string GetSQL(SqlCommand cmd)
        {
            string val = "";
#if Debug     
            StringBuilder sb= new StringBuilder();
           
            if (cmd.Connection!= null)
            {
                sb.AppendLine($"--{cmd. Connection.DataSource}");
                sb.AppendLine($"use {cmd.Connection.Database}");
                sb.AppendLine("go");

            }
            if (cmd.CommandType == CommandType.StoredProcedure)
            {
                sb.AppendLine($"exec{cmd.CommandText}");
                int paramcount= cmd.Parameters.Count-1;
                int paramnum = 0;
                string comma = ",";
                foreach (SqlParameter p in cmd.Parameters)
                {   if (p.Direction!= ParameterDirection.ReturnValue)
                    {
                        if (paramnum == paramcount)
                        {
                          comma = "";
                        }
                        sb.AppendLine($"{p.ParameterName}={(p.Value == null ? "null" : p.Value.ToString())}{comma}");
                     
                    }
                paramnum++;

                }
            }
            else {
                sb.AppendLine(cmd.CommandText);
            }
            val = sb.ToString();
#endif
            return val;
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
 