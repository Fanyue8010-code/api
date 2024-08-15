using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;

namespace zym_api.Helper
{
    public class SQLHelper
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        private static string strconn = @"server=49.233.191.59,1443\ZYM;dataBase=zym_test;uid=zym;pwd=!QAZxsw23edc";

        public SQLHelper(string conn)
        {
            //strconn = conn;
            strconn = @"server=49.233.191.59,1443\ZYM;dataBase=zym_test;uid=zym;pwd=!QAZxsw23edc";
        }

        /// <summary>
        /// 执行增删改SQL语句
        /// </summary>
        /// <param name="cmdText">SQL语句</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string cmdText)
        {
            using (SqlConnection conn = new SqlConnection(strconn))
            {
                conn.Open();
                return ExecuteNonQuery(conn, cmdText);
            }
        }

        /// <summary>
        /// 执行增删改SQL语句
        /// </summary>
        /// <param name="conn">SqlConnection</param>
        /// <param name="cmdText">SQL语句<</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(SqlConnection conn, string cmdText)
        {
            int res;
            using (SqlCommand cmd = new SqlCommand(cmdText, conn))
            {
                cmd.CommandType = CommandType.Text;
                res = cmd.ExecuteNonQuery();
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            return res;
        }

        /// <summary>
        /// 执行查询SQL语句
        /// </summary>
        /// <param name="cmdText">SQL语句</param>
        /// <returns></returns>
        public static DataTable ExecuteDataTable(string cmdText)
        {
            using (SqlConnection conn = new SqlConnection(strconn))
            {
                conn.Open();
                return ExecuteDataTable(conn, cmdText);
            }
        }

        /// <summary>
        /// 执行查询SQL语句
        /// </summary>
        /// <param name="conn">SqlConnection</param>
        /// <param name="cmdText">SQL语句</param>
        /// <returns></returns>
        private static DataTable ExecuteDataTable(SqlConnection conn, string cmdText)
        {
            DataTable dt = new DataTable();
            using (SqlCommand cmd = new SqlCommand(cmdText, conn))
            {
                cmd.CommandType = CommandType.Text;
                using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                {
                    sda.Fill(dt);
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                        conn.Dispose();
                    }
                }
            }
            return dt;
        }

        /// <summary>
        /// 执行查询SQL语句
        /// </summary>
        /// <param name="cmdText">SQL语句</param>
        /// <returns></returns>
        public static DataTable ExecuteQuery(string cmdText)
        {
            using (SqlConnection conn = new SqlConnection(strconn))
            {
                conn.Open();
                return ExecuteQuery(conn, cmdText);
            }
        }

        /// <summary>
        /// 执行查询SQL语句
        /// </summary>
        /// <param name="conn">SqlConnection</param>
        /// <param name="cmdText">SQL语句</param>
        /// <returns></returns>
        public static DataTable ExecuteQuery(SqlConnection conn, string cmdText)
        {
            DataTable dt = new DataTable();
            using (SqlCommand cmd = new SqlCommand(cmdText, conn))
            {
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    dt.Load(sdr);
                    sdr.Close();
                    sdr.Dispose();
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                        conn.Dispose();
                    }
                }
            }
            return dt;
        }
    }
}