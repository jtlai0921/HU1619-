/******************************************************************
 * YSFFood.cs ԓ���춲�ԃ�Y�ώ죬�@ȡ���ȵ��YӍ���K���Fӆُ����
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
         * ԓ�����������еĲ���Ʒ�N
         * 
         * @return String[]
         */
        public String[] getProductionKinds()
        {
            DataTable productTable = new DataTable();

            // �����ԃ�ִ�
            String sql = "SELECT DISTINCT productKind FROM productdetail";

            // ����һ���µ��B�ӣ���ԃ�Y��
            SqlConnection oConn = new SqlConnection();
            oConn.ConnectionString = "Server = " + dbhost +
                                    ";Initial Catalog = " + dbname +
                                    ";UID=" + dbuser +
                                    ";PWD=" + dbpass + ";";
            try
            {
                // ����һ��DataAdapter���K�O�ò�ԃ����
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

            // ���ؽY��
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
         * ԓ��������Ʒ�N���ز���
         * 
         * @param String    product_kind
         * @return String[]
         */
        public String[] getProductionName(String product_kind)
        {
            DataTable productTable = new DataTable();

            // �����ԃ�ִ�
            String sql = "SELECT productName FROM productdetail WHERE "
                         + " productKind = @productKind";

            // ����һ���µ��B�ӣ���ԃ�Y��
            SqlConnection oConn = new SqlConnection();
            oConn.ConnectionString = "Server = " + dbhost +
                                    ";Initial Catalog = " + dbname +
                                    ";UID=" + dbuser +
                                    ";PWD=" + dbpass + ";";
            // ����һ��SqlCommand����Á팢��ԃ������l�ͽo�Y�ώ�
            SqlCommand oCmd = oConn.CreateCommand();
            oCmd.CommandText = sql;
            oCmd.CommandType = CommandType.Text;
            // ���x����
            SqlParameter oParam;
            oParam = new SqlParameter("@productKind", SqlDbType.Char, 8);
            oCmd.Parameters.Add(oParam);
            oParam.Value = product_kind;
            try
            {
                // ����һ��DataAdapter���K�O�ò�ԃ����
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = oCmd;
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

            // ���ؽY��
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
         * ԓ�������ز���Ԕ�����YӍ
         * 
         * @param String      product_name
         * @return DataTable   
         */
        public DataTable getProductionDetail(String product_name)
        {
            DataTable productTable = new DataTable();

            // �����ԃ�ִ�
            String sql = "SELECT * FROM productdetail WHERE "
                         + " productName = @productName";

            // ����һ���µ��B�ӣ���ԃ�Y��
            SqlConnection oConn = new SqlConnection();
            oConn.ConnectionString = "Server = " + dbhost +
                                    ";Initial Catalog = " + dbname +
                                    ";UID=" + dbuser +
                                    ";PWD=" + dbpass + ";";
            // ����һ��SqlCommand����Á팢��ԃ������l�ͽo�Y�ώ�
            SqlCommand oCmd = oConn.CreateCommand();
            oCmd.CommandText = sql;
            oCmd.CommandType = CommandType.Text;
            // ���x����
            SqlParameter oParam;
            oParam = new SqlParameter("@productName", SqlDbType.Char, 8);
            oCmd.Parameters.Add(oParam);
            oParam.Value = product_name;
            try
            {
                // ����һ��DataAdapter���K�O�ò�ԃ����
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = oCmd;
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

            return productTable;
        }

        /**
         * ԓ��������Aӆ��ُ�I����
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
            //������C�Ñ�
            if (!string.IsNullOrEmpty(userID) && !string.IsNullOrEmpty(userPwd))
            {
                YSFLogin ysfLoginClass = new YSFLogin();
                logined = ysfLoginClass.login(userID, userPwd);
                if (logined != "user")
                {
                    // ��횽�ɫ����user
                    throw new Exception("�]���ڙ���L����");
                }
            }
            else
            {
                throw new Exception("�]���ڙ���L����");
            }

            // ����һ���µ��B�ӣ���ԃ�Y��
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
            // ����һ���ղ���
            SqlTransaction transaction = oConn.BeginTransaction();

            // ͨ�^��C�������M��ӆُ���������Ƚ����Y��
            // �@��һ��XML�n�Ľ�����
            XmlDocument orderDoc = new XmlDocument();
            try
            {
                orderDoc.LoadXml(order_xml);
            }
            catch (Exception e)
            {
                throw new Exception("order xml�������Ϸ�");
            }

            // �@��RootԪ��
            XmlNode rootNode = orderDoc.DocumentElement;
            XmlNode nd = default(XmlNode);
            XmlAttributeCollection nnm = default(XmlAttributeCollection);
            if (rootNode.HasChildNodes)
            {
                // �@��RootԪ�ص��ӹ��c�б�
                XmlNodeList nlst = rootNode.ChildNodes;
                try
                {
                    // ��ѹ��c��
                    int i = 0;
                    for (i = 0; i <= nlst.Count - 1; i++)
                    {
                        // �@ȡfood���c���ӹ��c
                        nd = nlst.Item(i);
                        // �@ȡfood���c�ӹ��c�Č���ӳ��
                        nnm = nd.Attributes;
                        // �@ȡ�ӹ��c�Č���food&num��ֵ
                        // �K�������c����SQL�Z��
                        String sql = "INSERT INTO footorder "
                                     + "(userName,foodName,foodNum) VALUES ('" + userID
                                     + "','" + nnm.Item(1).Value + "','"
                                     + nnm.Item(0).Value + "' )";
                        //�����Z��sqlStmt
                        SqlCommand oCmd = oConn.CreateCommand();
                        oCmd.CommandText = sql;
                        oCmd.ExecuteNonQuery();
                    }
                    // ��������ղ����������_���t�ύ��
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    try
                    {
                        // ��������Y�ϲ���ʧ�����t����ǰ�؝L
                        transaction.Rollback();
                        throw new Exception("�Y�ώ����ʧ�����ѽ��؝L�գ��]���Y�ϱ����룡����");
                    }
                    catch (Exception e2)
                    {
                        Console.WriteLine("�����e�`���ջ؝L���e" + e2.Message);
                    }
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
            }
            // ����]�г��e�ͷ���true
            return true;
        }
    }
}
