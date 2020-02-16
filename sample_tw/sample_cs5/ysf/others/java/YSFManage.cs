/******************************************************************
 * YSFManage.cs ԓ���춲�ԃ�Y�ώ죬�@ȡ���ȵ��YӍ���K���Ԍ��F����
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
         * ԓ������춫@ȡ�����Y��
         * 
         * @param  String    userID
         * @param  String    userPwd
         * @return DataTable
         */
        public DataTable getSQLData(string userID, string userPwd, string sqlStr)
        {
            String logined = "false";
            //������C�Ñ�
            if (!string.IsNullOrEmpty(userID) && !string.IsNullOrEmpty(userPwd))
            {
                YSFLogin ysfLoginClass = new YSFLogin();
                logined = ysfLoginClass.login(userID, userPwd);
                if (logined != "admin")
                {
                    // ����ǹ���T������ɫ����admin
                    throw new Exception("�]���ڙ���L����");
                }
            }
            else
            {
                throw new Exception("�]���ڙ���L����");
            }

            DataTable productTable = new DataTable();    // ���x���ص��Y��
            // [01]==================================================
            // �������������ԃ�ִ�
            String sql = "SELECT * FROM productDetail";

            // [02]==================================================
            // ����һ���µ��B�ӣ���ԃ�Y��
            SqlConnection oConn = new SqlConnection();
            oConn.ConnectionString = "Server = " + dbhost +
                                    ";Initial Catalog = " + dbname +
                                    ";UID=" + dbuser +
                                    ";PWD=" + dbpass + ";";
            try
            {
                // [03]����һ��DataAdapter���K�O�ò�ԃ����
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = new SqlCommand(sql, oConn);
                // ʹ��DataAdapter���DataTable
                adapter.Fill(productTable);
            }
            catch (Exception e)
            {
                throw new Exception("�Y�ώ��ԃ���e");
            }
            finally
            {
                try
                {
                    // �@ʽ���P�]����
                    oConn.Close();
                }
                catch (Exception e2) { }
            }

            // [04]==================================================
            // ���ؽY��
            return productTable;
        }

        /**
         * ԓ������춱����Y��,����RDBMSResolver
         * 
         * @param  String    userID
         * @param  String    userPwd
         * @param  String    deltaData
         * @return XmlDocument
         */
        public XmlDocument saveSQLData(String userID, String userPwd, String deltaData)
        {
            String logined = "false";
            //������C�Ñ�
            if (!string.IsNullOrEmpty(userID) && !string.IsNullOrEmpty(userPwd))
            {
                YSFLogin ysfLoginClass = new YSFLogin();
                logined = ysfLoginClass.login(userID, userPwd);
                if (logined != "admin")
                {
                    // ����ǹ���T������ɫ����admin
                    throw new Exception("�]���ڙ���L����");
                }
            }
            else
            {
                throw new Exception("�]���ڙ���L����");
            }

            // ����һ���µ��B��
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
                throw new Exception("�Y�ώ��ԃ���e");
            }

            // ��ʼ�������ִ�
            string SelectString = "";
            string updateString = "";
            string insertString = "";
            string deleteString = "";
            // ����һ��DOC��춷���ResultPacket
            XmlDocument resultDoc = new XmlDocument();

            // ===== ���I�_ʼ����UpdatePacket =====
            XmlDocument deltaDoc = new XmlDocument();
            try
            {
                deltaDoc.LoadXml(deltaData);
            }
            catch (Exception e)
            {
                throw new Exception("delta�������Ϸ�");
            }
            XmlElement rootNode = deltaDoc.DocumentElement;
            // ���ȫ@ȡ�����c�Č���ֵ
            string tableName = rootNode.GetAttribute("tableName");
            string nullValue = rootNode.GetAttribute("nullValue");
            string transID = rootNode.GetAttribute("transID");

            // ===== ���I�ȳ�ʼ��ResultPacket =====
            // ����<results_packet transID="1234567890"/>
            resultDoc.LoadXml("<?xml version=\"1.0\" encoding=\"UTF-8\" ?><results_packet></results_packet>");
            //������c��ӌ���
            XmlElement resultRootElement = resultDoc.CreateElement("results_packet");
            XmlElement resultRootNode = resultDoc.DocumentElement;
            XmlAttribute attr = resultRootNode.SetAttributeNode("transID", "");
            attr.Value = transID;

            XmlNodeList nodes = default(XmlNodeList);

            // ===== ������_ʼ����UpdatePacket�@ȡ���е��Y�ϲ����Y�ώ��� =====
            // ===== ����̎��update���c�@ȡ�Y�ϸ����Y�ώ� =====
            nodes = rootNode.GetElementsByTagName("update");
            foreach (XmlElement node in nodes)
            {
                // ���update���c
                string operationId = node.GetAttribute("id");
                XmlNodeList fields = node.GetElementsByTagName("field");
                string updateClause = "";
                string whereClause = "";
                string kname = "";
                // �������I
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
                        // ̎���Y�����
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
                        // ̎���Y�����
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
                    // =====�����Y���������ĳ��e�YӍ���c=====
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

            // ===== �_ʼ̎��delete���c�@ȡ�Y�τh��ӛ� =====
            nodes = rootNode.GetElementsByTagName("delete");
            foreach (XmlElement node in nodes)
            {
                string operationId = node.GetAttribute("id");
                XmlNodeList fields = node.GetElementsByTagName("field");
                string deleteClause = "";
                string whereClause = "";
                string kname = "";
                // ���delete���c
                foreach (XmlElement field in fields)
                {
                    // �������I
                    if (field.GetAttribute("key") == "true")
                    {
                        kname = field.GetAttribute("name");
                        string ktype = field.GetAttribute("type");
                        string kvalue = field.GetAttribute("oldValue");
                        // ̎���Y�����
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
                        // ̎���Y�����
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
                    // =====�����Y���������ĳ��e�YӍ���c=====
                    //<operation op="delete" id="12313534115" msg="Record not found" />
                    XmlElement deleteNode = resultDoc.CreateElement("operation");
                    deleteNode.SetAttribute("op", "delete");
                    deleteNode.SetAttribute("id", operationId);
                    deleteNode.SetAttribute("msg", ex.Message);
                    resultRootNode.AppendChild(deleteNode);
                }
            }

            // ===== �_ʼ̎��insert���c�@ȡ�Y�ϲ�����ӛ� =====
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
                    // �������I
                    if (field.GetAttribute("key") == "true")
                    {
                        kname = field.GetAttribute("name");
                        string ktype = field.GetAttribute("type");
                        kvalue = field.GetAttribute("newValue");
                        if (kvalue == nullValue)
                        {
                            kvalue = "";
                        }
                        // ̎���Y�����
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
                        //̎���Y�����
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
                //ע�⣬���@�Y��Ҳ�����Ȳ�ԃ�Ƿ����ӛ䛣�Ȼ����в������
                //�@�ӿ��Ա������I���}
                insertString = "INSERT INTO " + tableName + " ( " +
                               insertField + " ) VALUES ( " + insertValue + " )";
                SqlCommand oCmd = new SqlCommand(insertString, oConn);

                try
                {
                    oCmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    // =====�����Y���������ĳ��e�YӍ���c=====
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

            //���ؽY����                
            return resultDoc;
        }
    }
}
