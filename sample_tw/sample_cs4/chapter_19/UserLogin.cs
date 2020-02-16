/****************************************************************
 * UserLogin.cs
 * ԓ���춲�ԃ�Y�ώ죬���F�Ñ����
 * @author   zhang-yafei.com
 * @version  1.0.0.128  2009/2/20
 * @since    .NET CLR 2.0
 *****************************************************************/

using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Security.Cryptography;
using System.Web.SessionState;
using System.Text;
using System.Reflection;
using FluorineFx;
using FluorineFx.Context;

namespace org.zhangyafei
{
    [RemotingService("http://www.zhang-yafei.com")]
    public class UserLogin
    {
        private String dbhost = "localhost";
        private String dbport = "1433";
        private String dbname = "userManage";
        private String dbuser = "sa";
        private String dbpass = "verysecret";


        // ԓ������춲�ԃ�Y�ώ죬�鿴�Ƿ���ƥ����Ñ�
        /**
         * @param  String  userID
         * @param  String  userPwd
         * @param  String  checkingCode
         * @return String
         */
        public String login(String userID,
                            String userPwd,
                            String checkingCode)
        {
            String user;
            // ����Session
            ISessionState session = FluorineContext.Current.Session;
            // �z����C�a�Ƿ�ƥ��
            // ʹ���Ñ���f�ą����cSession�б�����ִ��M�Ќ���
            // �����ƥ��ͷ���"checkingCodeFalse"
            Object s_checkingCode = session["checkingCode"];
            if (s_checkingCode == null)
            {
                return "checkingCodeFalse";
            }
            else
            {
                // ���D�Q��С���M��ƥ��
                String s_checkingCode_lower =
                                   s_checkingCode.ToString().ToLower();
                if (!(checkingCode.ToLower()).Equals(
                                              s_checkingCode_lower))
                {
                    return "checkingCodeFalse";
                }
            }

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
            String sql = "SELECT user_id FROM users WHERE " +
                         " user_id = @userID AND user_pwd = @userPwd";

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
            oParam = new SqlParameter("@userID", SqlDbType.Char, 8);
            oCmd.Parameters.Add(oParam);
            oParam.Value = userID;
            oParam = new SqlParameter("@userPwd", SqlDbType.Char, 32);
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
                // ���O�����ص�ֵ��false
                user = "false";
                // �쿴�Y���Ƿ��գ��Ķ��_���Ñ��Ƿ����
                if (oReader.Read())
                {
                    user =
                                oReader.GetString(
                                   oReader.GetOrdinal("user_id"));
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
            return user;
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
