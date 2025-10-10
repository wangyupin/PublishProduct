using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITYHUB
{
    public class DBHelper
    {
        public static void ExecuteSqlTranPARM(SqlConnection sqlconn, string SqlSyntax, SqlTransaction sqltran,  params SqlParameter[] Parameters)
        {
            try
            {
                using (SqlCommand cmd = new System.Data.SqlClient.SqlCommand(SqlSyntax, sqlconn, sqltran))
                {
                    cmd.CommandTimeout = 8888;
                    cmd.Parameters.AddRange(Parameters);
                    //cmd.Connection.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 透過SqlParameter 執行 UPDATE、INSERT 和 DELETE 陳述式，傳回值是受命令影響的資料列數目
        /// Author：Rex  2021/6/23
        /// </summary>
        /// <code>
        /* int iRET = CITYHUB.DBHelper.ExecutePRM()
         * 
        */
        /// </code>
        public static int ExecuteSqlPARM(string ConnectionString, string SqlSyntax, params SqlParameter[] Parameters)
        {
            #region -- check parameters

            #endregion

            #region -- initial variables
            int iRET = 0;
            #endregion

            #region -- biz
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new System.Data.SqlClient.SqlCommand(SqlSyntax, conn))
                {
                    cmd.CommandTimeout = 8888;
                    cmd.Parameters.AddRange(Parameters);
                    cmd.Connection.Open();
                    iRET = cmd.ExecuteNonQuery();
                }
            }
            #endregion

            #region result
            return iRET;
            #endregion
        }

        /// <summary>
        /// 透過SqlParameter取得運算結果之用法。
        /// Author：Rex  2021/6/23
        /// </summary>
        /// <code>
        /* DataSet dataSet = CITYHUB.DBHelper.GetDataSetByPRM()
         *  DataSet ds = CITYHUB.DBHelper.GetDataSetByPRM(TB_DB_ConnectionString.Text, sql ,Parameters);
             DGV_DB_DataSHow.DataSource = ds.Tables[0];
        */
        /// </code>
        static public DataSet GetDataSetBySqlPARM(string ConnectionString, string SqlSyntax, params SqlParameter[] Parameters)
        {
            #region -- check parameters

            #endregion

            #region -- initial variables
            DataSet result = new DataSet();
            #endregion

            #region -- biz  
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(SqlSyntax, conn))
                {
                    cmd.Parameters.AddRange(Parameters);
                    conn.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.SelectCommand.CommandTimeout = 8888;
                    adapter.Fill(result);
                    conn.Close();
                }
            }
            #endregion

            #region result
            return result;
            #endregion
        }


        /// <summary>
        /// 執行SP，並取回DataSet後，再使用lamba取得運算結果之用法。
        /// Author：Moon 2021/6/23
        /// </summary>
        /// <example>
        /// 取回SP所傳回之資料，再Lamba處理。
        /// <code>
        /* DataSet dataSet = CITYHUB.DBHelper.GetDataSetBySP(DBConnStr.BPM, "dbo.USP_QRY_CALENDAR",
                new SqlParameter("@O_MSG", "") { Direction = ParameterDirection.Output }, //回傳用的變數
                new SqlParameter("@START_DATE", value.START_DATE),
                new SqlParameter("@END_DATE", value.END_DATE),
                new SqlParameter("@QRY_IP", value.QRY_IP));
           if (dataSet.Tables.Count > 0) {
                    CalendarWorkDayCount = dataSet.Tables[0].AsEnumerable()
                        .Where(r => r.Field<string>("isHoliday") == "0").Count();
            }
        */
        /// </code>
        /// </example>
        /// <param name="ConnectionString">連線字串</param>
        /// <param name="SPName">SP名稱</param>
        /// <param name="Parameters">SqlParameter集合</param>
        /// <returns>DataSet</returns>
        static public DataSet GetDataSetBySP(string ConnectionString, string SPName, params SqlParameter[] Parameters)
        {
            #region -- check parameters

            #endregion

            #region -- initial variables
            DataSet result = new DataSet();
            #endregion

            #region -- biz
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new System.Data.SqlClient.SqlCommand(SPName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(Parameters);
                    conn.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(result);
                    conn.Close();
                }
            }
            #endregion

            #region result
            return result;
            #endregion

        }

        /// <summary>
        /// 執行預儲程式，execute類。
        /// Author：Moon 2021/6/23
        /// </summary>
        /// <example>
        /// 
        /// <code>
        /* int iRET  = CITYHUB.DBHelper.ExecuteSP(DBConnStr.BPM, "dbo.USP_QRY_CALENDAR",
                new SqlParameter("@START_DATE", value.START_DATE),
                new SqlParameter("@END_DATE", value.END_DATE),
                new SqlParameter("@QRY_IP", value.QRY_IP));
           if (dataSet.Tables.Count > 0) {
                    CalendarWorkDayCount = dataSet.Tables[0].AsEnumerable()
                        .Where(r => r.Field<string>("isHoliday") == "0").Count();
            }
        */
        /// </code>
        /// </example>
        /// <param name="ConnectionString">連線字串</param>
        /// <param name="SPName">SP名稱</param>
        /// <param name="Parameters">SqlParameter集合</param>
        /// <returns>int</returns>
        public static int ExecuteSP(string ConnectionString, string SPName, params SqlParameter[] Parameters)
        {
            #region -- check parameters

            #endregion

            #region -- initial variables
            int iRET = 0;
            #endregion

            #region -- biz
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new System.Data.SqlClient.SqlCommand(SPName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(Parameters);
                    cmd.Connection.Open();
                    iRET = cmd.ExecuteNonQuery();
                    cmd.Connection.Close();
                }
            }
            #endregion

            #region result
            ///對 UPDATE、INSERT 和 DELETE 陳述式而言，傳回值是受命令影響的資料列數目。 對其他類型的陳述式而言，傳回值為 -1。
            ///REF：https://docs.microsoft.com/zh-tw/dotnet/api/system.data.sqlclient.sqlcommand.executenonquery?view=dotnet-plat-ext-5.0
            return iRET;
            #endregion

         }



        public static DataTable ExecuteSqlGetDataTable(string ConnectionString, string sql)
        {
            DataTable result = null;
            try
            {
                using (SqlConnection cnn = new SqlConnection(ConnectionString))
                {
                    //2.開啟資料庫
                    cnn.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(sql, cnn))
                    {
                        try
                        {
                            da.SelectCommand.CommandTimeout = 8888;
                            //以下會null!!!
                            //da.InsertCommand.CommandTimeout = 300;
                            //da.UpdateCommand.CommandTimeout = 300;
                            //da.DeleteCommand.CommandTimeout = 300;
                            DataSet ds = new DataSet();
                            da.Fill(ds, "Data");
                            result = ds.Tables[0];
                        }
                        catch (Exception ex)
                        {
                            //可使用多個catch捕捉各種錯誤。
                            throw ex;
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                 throw ex;
            }
        }
    }
}
