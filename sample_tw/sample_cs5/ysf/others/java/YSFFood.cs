/******************************************************************
 * YSFFood.cs 該類用於查詢資料庫，獲取菜肴的資訊，並實現訂購菜肴
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
    public class YSFFood
    {
        private String dbhost = "localhost";
        private String dbport = "1433";
        private String dbname = "ysf";
        private String dbuser = "sa";
        private String dbpass = "verysecret";


        /**
         * 該方法返回所有的菜肴品種
         * 
         * @return String[]
         */
        public String[] getProductionKinds()
        {
            DataTable productTable = new DataTable();

            // 構造查詢字串
            String sql = "SELECT DISTINCT productKind FROM productdetail";

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

            // 返回結果
            int rows = productTable.Rows.Count;
            String[] result = new String[rows];
            for (int i = 0; i < rows; i++)
            {
                Object o = productTable.Rows[i]["productKind"];
                result[i] = o.ToString().Trim();
            }
            return result;
        }

        /**
         * 該方法根據品種返回菜名
         * 
         * @param String    product_kind
         * @return String[]
         */
        public String[] getProductionName(String product_kind)
        {
            DataTable productTable = new DataTable();

            // 構造查詢字串
            String sql = "SELECT productName FROM productdetail WHERE "
                         + " productKind = @productKind";

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
            oParam = new SqlParameter("@productKind", SqlDbType.Char, 8);
            oCmd.Parameters.Add(oParam);
            oParam.Value = product_kind;
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

            // 返回結果
            int rows = productTable.Rows.Count;
            String[] result = new String[rows];
            for (int i = 0; i < rows; i++)
            {
                Object o = productTable.Rows[i]["productName"];
                result[i] = o.ToString().Trim();
            }
            return result;
        }

        /**
         * 該方法返回菜肴詳細的資訊
         * 
         * @param String      product_name
         * @return DataTable   
         */
        public DataTable getProductionDetail(String product_name)
        {
            DataTable productTable = new DataTable();

            // 構造查詢字串
            String sql = "SELECT * FROM productdetail WHERE "
                         + " productName = @productName";

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
            oParam = new SqlParameter("@productName", SqlDbType.Char, 8);
            oCmd.Parameters.Add(oParam);
            oParam.Value = product_name;
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
         * 該方法用於預訂和購買菜肴
         * 
         * @param String    userID
         * @param String    userPwd
         * @param String    order_xml
         * @return Boolean
         */
        public Boolean orderProduction(String userID,
                                       String userPwd,
                                       String order_xml)
        {
            String logined = "false";
            //首先驗證用戶
            if (!string.IsNullOrEmpty(userID) && !string.IsNullOrEmpty(userPwd))
            {
                YSFLogin ysfLoginClass = new YSFLogin();
                logined = ysfLoginClass.login(userID, userPwd);
                if (logined != "user")
                {
                    // 必須角色名為user
                    throw new Exception("沒有授權的訪問！");
                }
            }
            else
            {
                throw new Exception("沒有授權的訪問！");
            }

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

            // 通過驗證，下面進行訂購操作，首先解析資料
            // 獲得一個XML檔的解析器
            XmlDocument orderDoc = new XmlDocument();
            try
            {
                orderDoc.LoadXml(order_xml);
            }
            catch (Exception e)
            {
                throw new Exception("order xml數據不合法");
            }

            // 獲得Root元素
            XmlNode rootNode = orderDoc.DocumentElement;
            XmlNode nd = default(XmlNode);
            XmlAttributeCollection nnm = default(XmlAttributeCollection);
            if (rootNode.HasChildNodes)
            {
                // 獲得Root元素的子節點列表
                XmlNodeList nlst = rootNode.ChildNodes;
                try
                {
                    // 遍曆節點樹
                    int i = 0;
                    for (i = 0; i <= nlst.Count - 1; i++)
                    {
                        // 獲取food節點的子節點
                        nd = nlst.Item(i);
                        // 獲取food節點子節點的屬性映射
                        nnm = nd.Attributes;
                        // 獲取子節點的屬性food&num的值
                        // 並根據節點創建SQL語句
                        String sql = "INSERT INTO footorder "
                                     + "(userName,foodName,foodNum) VALUES ('" + userID
                                     + "','" + nnm.Item(1).Value + "','"
                                     + nnm.Item(0).Value + "' )";
                        //執行語句sqlStmt
                        SqlCommand oCmd = oConn.CreateCommand();
                        oCmd.CommandText = sql;
                        oCmd.ExecuteNonQuery();
                    }
                    // 如果整個事務操作執行正確，則提交事務
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    try
                    {
                        // 如果插入資料操作失敗，則事務向前回滾
                        transaction.Rollback();
                        throw new Exception("資料庫插入失敗，已經回滾事務，沒有資料被插入！！！");
                    }
                    catch (Exception e2)
                    {
                        Console.WriteLine("致命錯誤：事務回滾出錯" + e2.Message);
                    }
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
            }
            // 如果沒有出錯就返回true
            return true;
        }
    }
}
