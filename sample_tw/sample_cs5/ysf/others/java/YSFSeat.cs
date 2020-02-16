/******************************************************************
 * YSFSeat.cs ԓ���춲�ԃ�Y�ώ죬�@ȡ��λ�A�����YӍ���K���Ԍ��F�A����λ
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
         * ԓ������춫@ȡָ�����ڃ��ѽ��A������λ
         * 
         * @param  String      fDate
         * @param  String      tDate
         * @return XmlDocument
         */
        public XmlDocument searchSeat(String fDate, String tDate)
        {

            String sql = "SELECT * FROM seatorder WHERE "
                         + " fReserve >= @fDate AND tReserve <= @tDate";
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
                throw new Exception("�Y�ώ��ԃ���e");
            }

            // ������ԃ�Y�����췵��ֵ
            SqlDataReader oReader = null;
            // ����һ��DOM��춷��ز�ԃ�Y��
            XmlDocument doc = new XmlDocument();
            // ��Ԫ��������ęn
            XmlElement root = doc.CreateElement("seat");
            doc.AppendChild(root);
            try
            {
                oReader = oCmd.ExecuteReader();
                // ʹ��oReader.Read()�������Ƿ����ӛ�
                // ��ѽY��������һ��Hashtable
                while (oReader.Read())
                {
                    // ����һ��seat_num���c���K��ԓ���c���x����
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
                throw new Exception("�Y�ώ��ԃ���e");
            }
            finally
            {
                try
                {
                    // �@ʽ���P�]����
                    oReader.Close();
                    oConn.Close();
                }
                catch (Exception e2) { }
            }

            // ���ؽY��
            return doc;
        }

        /**
         * ԓ��������A��ָ�����ڃȵ���λ
         * 
         * @param String      seat_num
         * @param String      fDate
         * @param String      tDate
         * @return Boolean
         */
        public Boolean reserveSeat(String id,
                                   String fDate, String tDate)
        {
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

            SqlDataReader oReader = null;

            String sql = "INSERT INTO seatorder "
                         + " (id, fReserve, tReserve, reserved) VALUES"
                         + " (@id, @fDate, @tDate, 1)";
            // ����һ��SqlCommand����Á팢��ԃ������l�ͽo�Y�ώ�
            SqlCommand oCmd = oConn.CreateCommand();
            oCmd.CommandText = sql;
            oCmd.CommandType = CommandType.Text;
            oCmd.Transaction = transaction;
            // ���x����
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
                // ���x����
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
                // �쿴�������g�Ƿ�������һ���Ñ�ͬ�r�M����Ʋ���
                // ����������ĕr�g����ǰ�����N�͕��Ѓ����Y�ϴ���
                // �@��ζ��һ����λ�ɂ����Aӆ������Ҫȡ��INSERT
                // �K�����YӍҪ���Ñ����xһ����λ
                if (oReader.Read())
                {
                    if (oReader.GetInt32(0) > 1)
                    {
                        try
                        {
                            // �P�]SqlDataReader
                            oReader.Close();
                            // �؝L��
                            transaction.Rollback();
                            // �Y�ώ����ʧ�����ѽ��؝L�գ��]���Y�ϱ�����
                            return false;
                        }
                        catch (Exception e2)
                        {
                            Console.WriteLine("�����e�`���ջ؝L���e" + e2.Message);
                        }
                    }
                    else
                    {
                        // �P�]SqlDataReader
                        oReader.Close();
                        // ��������ղ����������_���t�ύ��
                        transaction.Commit();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("�Y�ώ��ԃ���e" + e.Message);
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
            // ����]�г��e�ͷ���true
            return true;
        }
    }
}
