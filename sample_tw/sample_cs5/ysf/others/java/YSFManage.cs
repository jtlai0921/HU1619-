/******************************************************************
 * YSFManage.cs 該類用於查詢資料庫，獲取菜肴的資訊，並可以實現管理
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
    public class YSFManage
    {
        private String dbhost = "localhost";
        private String dbport = "1433";
        private String dbname = "ysf";
        private String dbuser = "sa";
        private String dbpass = "verysecret";

        /**
         * 該方法用於獲取所有資料
         * 
         * @param  String    userID
         * @param  String    userPwd
         * @return DataTable
         */
        public DataTable getSQLData(string userID, string userPwd, string sqlStr)
        {
            String logined = "false";
            //首先驗證用戶
            if (!string.IsNullOrEmpty(userID) && !string.IsNullOrEmpty(userPwd))
            {
                YSFLogin ysfLoginClass = new YSFLogin();
                logined = ysfLoginClass.login(userID, userPwd);
                if (logined != "admin")
                {
                    // 必須是管理員，即角色名為admin
                    throw new Exception("沒有授權的訪問！");
                }
            }
            else
            {
                throw new Exception("沒有授權的訪問！");
            }

            DataTable productTable = new DataTable();    // 定義返回的資料
            // [01]==================================================
            // 根據參數構造查詢字串
            String sql = "SELECT * FROM productDetail";

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

            // [04]==================================================
            // 返回結果
            return productTable;
        }

        /**
         * 該方法用於保存資料,執行RDBMSResolver
         * 
         * @param  String    userID
         * @param  String    userPwd
         * @param  String    deltaData
         * @return XmlDocument
         */
        public XmlDocument saveSQLData(String userID, String userPwd, String deltaData)
        {
            String logined = "false";
            //首先驗證用戶
            if (!string.IsNullOrEmpty(userID) && !string.IsNullOrEmpty(userPwd))
            {
                YSFLogin ysfLoginClass = new YSFLogin();
                logined = ysfLoginClass.login(userID, userPwd);
                if (logined != "admin")
                {
                    // 必須是管理員，即角色名為admin
                    throw new Exception("沒有授權的訪問！");
                }
            }
            else
            {
                throw new Exception("沒有授權的訪問！");
            }

            // 創建一個新的連接
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

            // 初始化操作字串
            string SelectString = "";
            string updateString = "";
            string insertString = "";
            string deleteString = "";
            // 創建一個DOC用於返回ResultPacket
            XmlDocument resultDoc = new XmlDocument();

            // ===== 下麵開始解析UpdatePacket =====
            XmlDocument deltaDoc = new XmlDocument();
            try
            {
                deltaDoc.LoadXml(deltaData);
            }
            catch (Exception e)
            {
                throw new Exception("delta數據不合法");
            }
            XmlElement rootNode = deltaDoc.DocumentElement;
            // 首先獲取根節點的屬性值
            string tableName = rootNode.GetAttribute("tableName");
            string nullValue = rootNode.GetAttribute("nullValue");
            string transID = rootNode.GetAttribute("transID");

            // ===== 下麵先初始化ResultPacket =====
            // 例如<results_packet transID="1234567890"/>
            resultDoc.LoadXml("<?xml version=\"1.0\" encoding=\"UTF-8\" ?><results_packet></results_packet>");
            //為根節點添加屬性
            XmlElement resultRootElement = resultDoc.CreateElement("results_packet");
            XmlElement resultRootNode = resultDoc.DocumentElement;
            XmlAttribute attr = resultRootNode.SetAttributeNode("transID", "");
            attr.Value = transID;

            XmlNodeList nodes = default(XmlNodeList);

            // ===== 下面就開始解析UpdatePacket獲取其中的資料操作資料庫了 =====
            // ===== 首先處理update節點獲取資料更新資料庫 =====
            nodes = rootNode.GetElementsByTagName("update");
            foreach (XmlElement node in nodes)
            {
                // 遍曆update節點
                string operationId = node.GetAttribute("id");
                XmlNodeList fields = node.GetElementsByTagName("field");
                string updateClause = "";
                string whereClause = "";
                string kname = "";
                // 查找主鍵
                foreach (XmlElement field in fields)
                {
                    if (field.GetAttribute("key") == "true")
                    {
                        kname = field.GetAttribute("name");
                        string ktype = field.GetAttribute("type");
                        string kvalue = field.GetAttribute("oldValue");
                        string k_newValue = field.GetAttribute("newValue");
                        if (string.IsNullOrEmpty(k_newValue))
                        {
                            k_newValue = kvalue;
                        }
                        // 處理資料類型
                        if (ktype == "Boolean" | ktype == "Number")
                        {
                            if (string.IsNullOrEmpty(updateClause))
                            {
                                updateClause = kname + " = " + k_newValue;
                            }
                            else
                            {
                                updateClause = kname + " = " + k_newValue + " , " + updateClause;
                            }
                            whereClause = " where " + kname + " = " + kvalue;
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(updateClause))
                            {
                                updateClause = kname + " = '" + k_newValue + "'";
                            }
                            else
                            {
                                updateClause = kname + " = '" + k_newValue + "' , " + updateClause;
                            }
                            whereClause = " WHERE " + kname + " = '" + kvalue + "'";
                        }
                    }
                    if (field.GetAttribute("key") == "false")
                    {
                        string fname = field.GetAttribute("name");
                        string ftype = field.GetAttribute("type");
                        string fvalue = field.GetAttribute("oldValue");
                        string f_value = field.GetAttribute("newValue");
                        // 處理資料類型
                        if (ftype == "Boolean" | ftype == "Number")
                        {
                            if (string.IsNullOrEmpty(updateClause))
                            {
                                updateClause = fname + " = " + f_value;
                            }
                            else
                            {
                                updateClause = fname + " = " + f_value + " , " + updateClause;
                            }
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(updateClause))
                            {
                                updateClause = fname + " = '" + f_value + "'";
                            }
                            else
                            {
                                updateClause = fname + " = '" + f_value + "' , " + updateClause;
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(whereClause))
                {
                    updateString = "UPDATE " + tableName + " SET " + updateClause + whereClause;
                }
                else if (string.IsNullOrEmpty(whereClause))
                {
                    updateString = "UPDATE " + tableName + " SET " + updateClause;
                }
                Console.WriteLine(updateString);

                SqlCommand oCmd = new SqlCommand(updateString, oConn);
                try
                {
                    oCmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    // =====創建結果包包含的出錯資訊節點=====
                    //<operation op="update" id="23523454646">
                    // <field name="id" msg="Invalid field value" />
                    //</operation>
                    XmlElement updateNode = resultDoc.CreateElement("operation");
                    updateNode.SetAttribute("op", "update");
                    updateNode.SetAttribute("id", operationId);

                    XmlElement fieldNode = resultDoc.CreateElement("field");
                    fieldNode.SetAttribute("name", kname);
                    fieldNode.SetAttribute("msg", ex.Message);

                    updateNode.AppendChild(fieldNode);
                    resultRootNode.AppendChild(updateNode);
                }
            }

            // ===== 開始處理delete節點獲取資料刪除記錄 =====
            nodes = rootNode.GetElementsByTagName("delete");
            foreach (XmlElement node in nodes)
            {
                string operationId = node.GetAttribute("id");
                XmlNodeList fields = node.GetElementsByTagName("field");
                string deleteClause = "";
                string whereClause = "";
                string kname = "";
                // 遍曆delete節點
                foreach (XmlElement field in fields)
                {
                    // 查找主鍵
                    if (field.GetAttribute("key") == "true")
                    {
                        kname = field.GetAttribute("name");
                        string ktype = field.GetAttribute("type");
                        string kvalue = field.GetAttribute("oldValue");
                        // 處理資料類型
                        if (ktype == "Boolean" | ktype == "Number")
                        {
                            whereClause = " WHERE " + kname + " = " + kvalue;
                        }
                        else
                        {
                            whereClause = " WHERE " + kname + " = '" + kvalue + "'";
                        }
                    }
                    if (field.GetAttribute("key") == "false")
                    {
                        string fname = field.GetAttribute("name");
                        string ftype = field.GetAttribute("type");
                        string fvalue = field.GetAttribute("oldValue");
                        // 處理資料類型
                        if (ftype == "Boolean" | ftype == "Number")
                        {
                            whereClause = " WHERE " + fname + " = " + fvalue;
                        }
                        else
                        {
                            whereClause = " WHERE " + fname + " = '" + fvalue + "'";
                        }
                    }
                }
                deleteString = "DELETE FROM " + tableName + whereClause;
                SqlCommand oCmd = new SqlCommand(deleteString, oConn);
                try
                {
                    oCmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    // =====創建結果包包含的出錯資訊節點=====
                    //<operation op="delete" id="12313534115" msg="Record not found" />
                    XmlElement deleteNode = resultDoc.CreateElement("operation");
                    deleteNode.SetAttribute("op", "delete");
                    deleteNode.SetAttribute("id", operationId);
                    deleteNode.SetAttribute("msg", ex.Message);
                    resultRootNode.AppendChild(deleteNode);
                }
            }

            // ===== 開始處理insert節點獲取資料插入新記錄 =====
            nodes = rootNode.GetElementsByTagName("insert");
            foreach (XmlElement node in nodes)
            {
                string operationId = node.GetAttribute("id");
                XmlNodeList fields = node.GetElementsByTagName("field");
                string insertField = "";
                string insertValue = "";
                string whereClause = "";
                string kname = "";
                string kvalue = "";
                foreach (XmlElement field in fields)
                {
                    // 查找主鍵
                    if (field.GetAttribute("key") == "true")
                    {
                        kname = field.GetAttribute("name");
                        string ktype = field.GetAttribute("type");
                        kvalue = field.GetAttribute("newValue");
                        if (kvalue == nullValue)
                        {
                            kvalue = "";
                        }
                        // 處理資料類型
                        if (ktype == "Boolean" | ktype == "Number")
                        {
                            if (string.IsNullOrEmpty(insertField))
                            {
                                insertField = kname;
                                insertValue = kvalue;
                            }
                            else
                            {
                                insertField = kname + " , " + insertField;
                                insertValue = kvalue + " , " + insertValue;
                            }
                            whereClause = " WHERE " + kname + " = " + kvalue;
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(insertField))
                            {
                                insertField = kname;
                                insertValue = "'" + kvalue + "'";
                            }
                            else
                            {
                                insertField = kname + " , " + insertField;
                                insertValue = "'" + kvalue + "' , " + insertValue;
                            }
                            whereClause = " WHERE " + kname + " = '" + kvalue + "'";
                        }
                    }
                    if (field.GetAttribute("key") == "false")
                    {
                        string fname = field.GetAttribute("name");
                        string ftype = field.GetAttribute("type");
                        string fvalue = field.GetAttribute("newValue");
                        if (fvalue == nullValue)
                        {
                            fvalue = "";
                        }
                        //處理資料類型
                        if (ftype == "Boolean" | ftype == "Number")
                        {
                            if (string.IsNullOrEmpty(insertField))
                            {
                                insertField = fname;
                                insertValue = fvalue;
                            }
                            else
                            {
                                insertField = fname + " , " + insertField;
                                insertValue = fvalue + " , " + insertValue;
                            }
                            whereClause = " WHERE " + fname + " = " + fvalue;
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(insertField))
                            {
                                insertField = fname;
                                insertValue = "'" + fvalue + "'";
                            }
                            else
                            {
                                insertField = fname + " , " + insertField;
                                insertValue = "'" + fvalue + "' , " + insertValue;
                            }
                        }
                    }
                }
                //注意，在這裏你也可以先查詢是否存在記錄，然後執行插入操作
                //這樣可以避免主鍵重複
                insertString = "INSERT INTO " + tableName + " ( " +
                               insertField + " ) VALUES ( " + insertValue + " )";
                SqlCommand oCmd = new SqlCommand(insertString, oConn);

                try
                {
                    oCmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    // =====創建結果包包含的出錯資訊節點=====
                    //<operation op="insert" id="345342436346">
                    // <field name="id" curValue="666666666" />
                    //</operation>
                    XmlElement insertNode = resultDoc.CreateElement("operation");
                    insertNode.SetAttribute("op", "insert");
                    insertNode.SetAttribute("id", operationId);

                    XmlElement fieldNode = resultDoc.CreateElement("field");
                    fieldNode.SetAttribute("name", kname);
                    fieldNode.SetAttribute("curValue", kvalue);
                    fieldNode.SetAttribute("msg", ex.Message);
                    insertNode.AppendChild(fieldNode);
                    resultRootNode.AppendChild(insertNode);
                }
            }

            try
            {
                oConn.Close();
            }
            catch (Exception e) { }

            //返回結果包                
            return resultDoc;
        }
    }
}
