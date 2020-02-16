using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using FluorineFx;

namespace org.zhangyafei
{
    [RemotingService("http://www.zhang-yafei.com")]
    public class ResultSetTest
    {
        private String dbhost = "localhost";
        private String dbport = "1433";
        private String dbname = "restaurant";
        private String dbuser = "sa";
        private String dbpass = "verysecret";


        // 該方法用於獲取資料庫雇員記錄
        /**
         * @param  String  firstName
         * @param  String  lastName
         * @param  int     age
         * @return DataTable
         */
        public DataTable getRestaurant()
        {
            DataTable restaurant = new DataTable();

            // [01]==================================================
            // 構造查詢字串
            string sql = "SELECT * FROM main";

            // [02]==================================================
            // 創建一個新的連接，查詢資料
            SqlConnection oConn = new SqlConnection();
            oConn.ConnectionString = "Server = " + dbhost +
                                    ";Initial Catalog = " + dbname +
                                    ";UID=" + dbuser +
                                    ";PWD=" + dbpass + ";";
            try
            {
                // 創建一個DataAdapter，並設置查詢命令
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = new SqlCommand(sql, oConn);
                // 使用DataAdapter填充DataTable
                adapter.Fill(restaurant);
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
            return restaurant;
        }
    }
}
