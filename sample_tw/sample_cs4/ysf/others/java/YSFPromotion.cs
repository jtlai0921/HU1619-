/******************************************************************
 * YSFPromotion.cs 該類用於查詢資料庫，獲取促銷菜肴的資訊
 * 
 * @author zhang-yafei.com
 * @version 1.0.0.128 2009/2/20
 * @since .NET CLR 2.0
 *****************************************************************/
using System;
using System.Data;
using System.Data.SqlClient;
using FluorineFx;

namespace com.ysf
{
    [RemotingService("http://www.zhang-yafei.com")]
    public class YSFPromotion
    {
        private String dbhost = "localhost";
        private String dbport = "1433";
        private String dbname = "ysf";
        private String dbuser = "sa";
        private String dbpass = "verysecret";

        /**
         * 該方法返回所有的促銷菜名
         * 
         * @return DataTable
         */
        public DataTable getAllPromotion()
        {
            DataTable productTable = new DataTable();

            // 構造查詢字串
            String sql = "SELECT a.*, b.productPic, b.productPicPath FROM "
               + " promotion AS a INNER JOIN productdetail AS b ON "
               + " a.productName=b.productName";

            // 創建一個新的連接，查詢資料
            SqlConnection oConn = new SqlConnection();
            oConn.ConnectionString = "Server = " + dbhost +
                                    ";Initial Catalog = " + dbname +
                                    ";UID=" + dbuser +
                                    ";PWD=" + dbpass + ";";
            // 創建一個SqlCommand物件用來將查詢和命令發送給資料庫
            SqlCommand oCmd = oConn.CreateCommand();
            oCmd.CommandText = sql;
            try
            {
                // 創建一個DataAdapter，並設置查詢命令
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = oCmd;
                // 使用DataAdapter填充DataTable
                adapter.Fill(productTable);
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

            return productTable;
        }

        /**
         * 該方法根據日期返回當天的促銷菜名
         * 
         * @param int         week
         * @return DataTable
         */
        public DataTable getPromotionByDate(int week)
        {
            DataTable productTable = new DataTable();

            // 構造查詢字串
            String sql = "SELECT a.*, b.productPic, b.productPicPath FROM "
               + " promotion AS a INNER JOIN productdetail AS b ON "
               + " a.productName=b.productName Where " + " a.promotionDate = @promotionDate";

            // 創建一個新的連接，查詢資料
            SqlConnection oConn = new SqlConnection();
            oConn.ConnectionString = "Server = " + dbhost +
                                    ";Initial Catalog = " + dbname +
                                    ";UID=" + dbuser +
                                    ";PWD=" + dbpass + ";";
            // 創建一個SqlCommand物件用來將查詢和命令發送給資料庫
            SqlCommand oCmd = oConn.CreateCommand();
            oCmd.CommandText = sql;
            oCmd.CommandType = CommandType.Text;
            // 定義參數
            SqlParameter oParam;
            oParam = new SqlParameter("@promotionDate", SqlDbType.TinyInt);
            oCmd.Parameters.Add(oParam);
            oParam.Value = week;
            try
            {
                // 創建一個DataAdapter，並設置查詢命令
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = oCmd;
                // 使用DataAdapter填充DataTable
                adapter.Fill(productTable);
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

            return productTable;
        }
    }
}
