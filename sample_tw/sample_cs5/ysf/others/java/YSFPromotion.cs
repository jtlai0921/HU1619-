/******************************************************************
 * YSFPromotion.cs ԓ���춲�ԃ�Y�ώ죬�@ȡ���N���ȵ��YӍ
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
         * ԓ�����������еĴ��N����
         * 
         * @return DataTable
         */
        public DataTable getAllPromotion()
        {
            DataTable productTable = new DataTable();

            // �����ԃ�ִ�
            String sql = "SELECT a.*, b.productPic, b.productPicPath FROM "
               + " promotion AS a INNER JOIN productdetail AS b ON "
               + " a.productName=b.productName";

            // ����һ���µ��B�ӣ���ԃ�Y��
            SqlConnection oConn = new SqlConnection();
            oConn.ConnectionString = "Server = " + dbhost +
                                    ";Initial Catalog = " + dbname +
                                    ";UID=" + dbuser +
                                    ";PWD=" + dbpass + ";";
            // ����һ��SqlCommand����Á팢��ԃ������l�ͽo�Y�ώ�
            SqlCommand oCmd = oConn.CreateCommand();
            oCmd.CommandText = sql;
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
         * ԓ�����������ڷ��خ���Ĵ��N����
         * 
         * @param int         week
         * @return DataTable
         */
        public DataTable getPromotionByDate(int week)
        {
            DataTable productTable = new DataTable();

            // �����ԃ�ִ�
            String sql = "SELECT a.*, b.productPic, b.productPicPath FROM "
               + " promotion AS a INNER JOIN productdetail AS b ON "
               + " a.productName=b.productName Where " + " a.promotionDate = @promotionDate";

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
            oParam = new SqlParameter("@promotionDate", SqlDbType.TinyInt);
            oCmd.Parameters.Add(oParam);
            oParam.Value = week;
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
    }
}
