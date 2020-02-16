using System;
using System.Data;
using System.Data.SqlClient;
using FluorineFx;

namespace org.zhangyafei
{
    [RemotingService("http://www.zhang-yafei.com")]
    public class Employees_paged
    {
        private String dbhost = "localhost";
        private String dbport = "1433";
        private String dbname = "myDatabase";
        private String dbuser = "sa";
        private String dbpass = "verysecret";


        // 該方法用於獲取資料庫雇員記錄
        /**
         * @param  int         offset
		 * @param  int         limit
         * @return DataTable
         */
        public DataTable getEmployeesInfo(int offset, int limit)
        {
            DataTable employeesDT = new DataTable();

            // [01]==================================================
            // 根據參數構造查詢字串
            String sql = " SELECT * " +
                         " FROM (SELECT TOP  " + limit + " * " +
                         "         FROM (SELECT TOP  " +
                                         (limit + offset) + " * " +
                         "                 FROM employees " +
                         "             ORDER BY age ASC) AS foo " +
                         "         ORDER BY age DESC) bar " +
                         " ORDER BY age ";

            // [02]==================================================
            // 創建一個新的連接，查詢資料
            SqlConnection oConn = new SqlConnection();
            oConn.ConnectionString = "Server = " + dbhost +
                                    ";Initial Catalog = " + dbname +
                                    ";UID=" + dbuser +
                                    ";PWD=" + dbpass + ";";
            try
            {
                // [03]創建一個DataAdapter，並設置查詢命令
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = new SqlCommand(sql, oConn);
                // 使用DataAdapter填充DataTable
                adapter.Fill(employeesDT);
            }
            catch (Exception e)
            {
                throw new Exception("資料庫查詢出錯");
            }
            finally
            {
                try
                {
                    // 顯式的關閉對象
                    oConn.Close();
                }
                catch (Exception e2) { }
            }

            // [04]==================================================
            // 返回結果
            return employeesDT;
        }
    }
}
