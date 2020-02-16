/******************************************************************
 * YSFSeat.cs 該類用於查詢資料庫，獲取座位預定的資訊，並可以實現預定座位
 * 
 * @author zhang-yafei.com
 * @version 1.0.0.128 2009/2/20
 * @since .NET CLR 2.0
 *****************************************************************/
using System;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using FluorineFx;

namespace com.ysf
{
    [RemotingService("http://www.zhang-yafei.com")]
    public class YSFSeat
    {
        private String dbhost = "localhost";
        private String dbport = "1433";
        private String dbname = "ysf";
        private String dbuser = "sa";
        private String dbpass = "verysecret";

        /**
         * 該方法用於獲取指定日期內已經預定的座位
         * 
         * @param  String      fDate
         * @param  String      tDate
         * @return XmlDocument
         */
        public XmlDocument searchSeat(String fDate, String tDate)
        {

            String sql = "SELECT * FROM seatorder WHERE "
                         + " fReserve >= @fDate AND tReserve <= @tDate";
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
            oParam = new SqlParameter("@fDate", SqlDbType.SmallDateTime);
            oCmd.Parameters.Add(oParam);
            oParam.Value = fDate;
            oParam = new SqlParameter("@tDate", SqlDbType.SmallDateTime);
            oCmd.Parameters.Add(oParam);
            oParam.Value = tDate;
            try
            {
                oConn.Open();
            }
            catch (Exception e)
            {
                throw new Exception("資料庫查詢出錯");
            }

            // 根據查詢結果構造返回值
            SqlDataReader oReader = null;
            // 創建一個DOM用於返回查詢結果
            XmlDocument doc = new XmlDocument();
            // 根元素添加上文檔
            XmlElement root = doc.CreateElement("seat");
            doc.AppendChild(root);
            try
            {
                oReader = oCmd.ExecuteReader();
                // 使用oReader.Read()方法看是否存在記錄
                // 遍曆結果集構造一個Hashtable
                while (oReader.Read())
                {
                    // 創建一個seat_num節點，並為該節點定義屬性
                    XmlElement seat = doc.CreateElement("seat_num");
                    String id = oReader.GetString(oReader.GetOrdinal("id")).Trim();
                    String fReserve = oReader.GetDateTime(oReader.GetOrdinal("fReserve")).ToString().Trim();
                    String tReserve = oReader.GetDateTime(oReader.GetOrdinal("tReserve")).ToString().Trim();
                    String reserved = oReader.GetBoolean(oReader.GetOrdinal("reserved")).ToString().ToLower();
                    seat.SetAttribute("id", id);
                    seat.SetAttribute("fDate", fReserve);
                    seat.SetAttribute("tDate", tReserve);
                    root.AppendChild(seat);
                    XmlText textNode = doc.CreateTextNode(reserved);
                    seat.AppendChild(textNode);
                }
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
                    oReader.Close();
                    oConn.Close();
                }
                catch (Exception e2) { }
            }

            // 返回結果
            return doc;
        }

        /**
         * 該方法用於預定指定日期內的座位
         * 
         * @param String      seat_num
         * @param String      fDate
         * @param String      tDate
         * @return Boolean
         */
        public Boolean reserveSeat(String id,
                                   String fDate, String tDate)
        {
            // 創建一個新的連接，查詢資料
            SqlConnection oConn = new SqlConnection();
            oConn.ConnectionString = "Server = " + dbhost +
                                     ";Initial Catalog = " + dbname +
                                     ";UID=" + dbuser +
                                     ";PWD=" + dbpass + ";";
            try
            {
                oConn.Open();
            }
            catch (Exception e)
            {
                throw new Exception("資料庫查詢出錯");
            }
            // 啟動一個事務操作
            SqlTransaction transaction = oConn.BeginTransaction();

            SqlDataReader oReader = null;

            String sql = "INSERT INTO seatorder "
                         + " (id, fReserve, tReserve, reserved) VALUES"
                         + " (@id, @fDate, @tDate, 1)";
            // 創建一個SqlCommand物件用來將查詢和命令發送給資料庫
            SqlCommand oCmd = oConn.CreateCommand();
            oCmd.CommandText = sql;
            oCmd.CommandType = CommandType.Text;
            oCmd.Transaction = transaction;
            // 定義參數
            SqlParameter oParam;
            oParam = new SqlParameter("@id", SqlDbType.Char);
            oCmd.Parameters.Add(oParam);
            oParam.Value = id;
            oParam = new SqlParameter("@fDate", SqlDbType.SmallDateTime);
            oCmd.Parameters.Add(oParam);
            oParam.Value = fDate;
            oParam = new SqlParameter("@tDate", SqlDbType.SmallDateTime);
            oCmd.Parameters.Add(oParam);
            oParam.Value = tDate;
            try
            {
                oCmd.ExecuteNonQuery();

                String sql2 = "SELECT COUNT(*) as row FROM seatorder WHERE "
                      + " seatorder.id = @id AND seatorder.fReserve >= @fDate "
                      + " AND seatorder.tReserve <= @tDate;";
                SqlCommand oCmd2 = oConn.CreateCommand();
                oCmd2.CommandText = sql2;
                oCmd2.CommandType = CommandType.Text;
                oCmd2.Transaction = transaction;
                // 定義參數
                SqlParameter oParam2;
                oParam2 = new SqlParameter("@id", SqlDbType.Char);
                oCmd2.Parameters.Add(oParam2);
                oParam2.Value = id;
                oParam2 = new SqlParameter("@fDate", SqlDbType.SmallDateTime);
                oCmd2.Parameters.Add(oParam2);
                oParam2.Value = fDate;
                oParam2 = new SqlParameter("@tDate", SqlDbType.SmallDateTime);
                oCmd2.Parameters.Add(oParam2);
                oParam2.Value = tDate;

                oReader = oCmd2.ExecuteReader();
                // 察看操作期間是否已有另一個用戶同時進行類似操作
                // 如果它操作的時間更靠前，那麼就會有兩行資料存在
                // 這意味著一個座位兩個人預訂，所以要取消INSERT
                // 並返回資訊要求用戶另選一個座位
                if (oReader.Read())
                {
                    if (oReader.GetInt32(0) > 1)
                    {
                        try
                        {
                            // 關閉SqlDataReader
                            oReader.Close();
                            // 回滾事務
                            transaction.Rollback();
                            // 資料庫插入失敗，已經回滾事務，沒有資料被插入
                            return false;
                        }
                        catch (Exception e2)
                        {
                            Console.WriteLine("致命錯誤：事務回滾出錯" + e2.Message);
                        }
                    }
                    else
                    {
                        // 關閉SqlDataReader
                        oReader.Close();
                        // 如果整個事務操作執行正確，則提交事務
                        transaction.Commit();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("資料庫查詢出錯" + e.Message);
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
            // 如果沒有出錯就返回true
            return true;
        }
    }
}
