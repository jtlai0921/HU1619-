/******************************************************************
 * YSFLogin.cs ԓ���춲�ԃ�Y�ώ죬���F�Ñ���䛣��@ȡ�Ñ���ɫ
 * 
 * @author zhang-yafei.com
 * @version 1.0.0.128 2009/2/20
 * @since .NET CLR 2.0
 *****************************************************************/
using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Reflection;
using FluorineFx;

namespace com.ysf
{
    [RemotingService("http://www.zhang-yafei.com")]
    public class YSFLogin
    {
        private String dbhost = "localhost";
        private String dbport = "1433";
        private String dbname = "ysf";
        private String dbuser = "sa";
        private String dbpass = "verysecret";

        /**
         * ԓ�����Á���C�Ñ����
         * 
         * @param String    userID
         * @param String    userPwd
         * @return String
         */
        public String login(String userID, String userPwd)
        {
            String logined = "false";
            // ��춱������Y�ώ��е��ܴa�ѽ����D�Q�����ĵ��ִ�
            // ���ԣ����@�YҲ��ԓ���D�Q�����M��ƥ��
            // ���Ȍ��ܴa�����D�Q��MD5����            
            try
            {
                userPwd = encodeString(userPwd);
            }
            catch (Exception e)
            {
                throw new Exception("�ܴa����ʧ��!");
            }

            // ��ԃ�Y�ώ죬�����Ƿ���ƥ����Ñ�����
            // ������ھͷ���ԓ�Ñ���id
            // ��t�����ִ�false
            String sql = "SELECT roles FROM userlist WHERE " +
                         " userID = @userID AND userPass = @userPwd";

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
            oParam = new SqlParameter("@userID", SqlDbType.Char, 50);
            oCmd.Parameters.Add(oParam);
            oParam.Value = userID;
            oParam = new SqlParameter("@userPwd", SqlDbType.Char, 50);
            oCmd.Parameters.Add(oParam);
            oParam.Value = userPwd;

            try
            {
                oConn.Open();
            }
            catch (Exception e)
            {
                throw new Exception("�Y�ώ��ԃ���e");
            }

            // ������ԃ�Y�����췵��ֵ
            // ʹ��ExecuteReader�������в�ԃ
            // �K����ԃ�Y���x�oһ��SqlDataReader
            SqlDataReader oReader = null;
            try
            {
                oReader = oCmd.ExecuteReader();
                // �쿴�Y���Ƿ��գ��Ķ��_���Ñ��Ƿ����
                if (oReader.Read())
                {
                    logined = oReader.GetString(
                                   oReader.GetOrdinal("roles"));
                }
            }
            catch (Exception e)
            {
                return "false";
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
            return logined;
        }

        // ԓ������춼����ִ���ͨ���Ǽ����ܴa
        /**
        * @param  String userPwd
        * @return String
        */
        private String encodeString(String userPwd)
        {
            String result = "";

            // ���ܴa�����D�Q��MD5����	   
            try
            {
                MD5 md5Hasher = MD5.Create();
                byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(userPwd));
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
                result = sBuilder.ToString();
            }
            catch (TargetInvocationException e)
            {
                throw new Exception("�ܴa����ʧ��!");
            }

            return result;
        }
    }
}
