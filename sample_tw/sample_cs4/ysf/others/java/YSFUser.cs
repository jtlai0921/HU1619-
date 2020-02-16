/******************************************************************
 * YSFUser.cs ԓ�����Ñ������Ñ��]�Ժ��YӍ�޸�
 * 
 * @author zhang-yafei.com
 * @version 1.0.0.128 2009/2/20
 * @since .NET CLR 2.0
 *****************************************************************/
using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using System.Reflection;
using FluorineFx;

namespace com.ysf
{
    [RemotingService("http://www.zhang-yafei.com")]
    public class YSFUser
    {
        private String dbhost = "localhost";
        private String dbport = "1433";
        private String dbname = "ysf";
        private String dbuser = "sa";
        private String dbpass = "verysecret";

        /**
         * ԓ������춫@ȡ�Ñ��YӍ
         * 
         * @param  String    userID
         * @param  String    userPwd
         * @return Hashtable
         */
        public Hashtable getUserInfo(String userID,
                                     String userPwd)
        {
            Hashtable result = null;          // ԓ׃����춶��x���ص��Y��

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

            // ��ԃ�Y�ώ췵��ԓ�Ñ����]���YӍ
            String sql = "SELECT * FROM userlist WHERE "
                        + " userID = @userID AND userPass = @userPwd";

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

            // [03]==================================================
            // ������ԃ�Y�����췵��ֵ
            // ʹ��ExecuteReader�������в�ԃ���K����ԃ�Y���x�oһ��SqlDataReader
            SqlDataReader oReader = null;
            try
            {
                oReader = oCmd.ExecuteReader();
                // ʹ��oReader.Read()�������Ƿ����ӛ�
                // ��ѽY��������һ��Hashtable
                if (oReader.Read())
                {
                    result = new Hashtable();
                    result.Add("userName", oReader.GetString(oReader.GetOrdinal("userName")).Trim());
                    result.Add("userBirth",
                            oReader.GetString(oReader.GetOrdinal("userBirth")).Trim());
                    result.Add("userTel",
                            oReader.GetString(oReader.GetOrdinal("userTel")).Trim());
                    result.Add("userAdd",
                            oReader.GetString(oReader.GetOrdinal("userAdd")).Trim());
                    result.Add("userID",
                            oReader.GetString(oReader.GetOrdinal("userID")).Trim());
                    result.Add("userSex",
                            oReader.GetString(oReader.GetOrdinal("userSex")).Trim());
                    result.Add("userEMail",
                            oReader.GetString(oReader.GetOrdinal("userEMail")).Trim());
                    result.Add("userProvince",
                            oReader.GetString(oReader.GetOrdinal("userProvince")).Trim());
                    result.Add("userZIP",
                            oReader.GetString(oReader.GetOrdinal("userZIP")).Trim());
                    result.Add("userPass",
                            oReader.GetString(oReader.GetOrdinal("userPass")).Trim());
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
            return result;
        }

        /**
         * ԓ����������Ñ��]��
         * 
         * @param String    userName ����
         * @param String    userSex �Ԅe
         * @param String    userBirth ��������
         * @param String    userEMail ����]��
         * @param String    userTel �Ԓ
         * @param String    userProvince ʡ��
         * @param String    userADD ��ס��ַ
         * @param String    userZIP �]�f�^̖
         * @param String    userID �Ñ�ID
         * @param String    userPwd �ܴa
         * @return String
         */
        public String newUser(String userName, String userSex,
                                String userBirth, String userEmail,
                                String userTel, String userProvince,
                                String userADD, String userZIP,
                                String userID, String userPwd)
        {
            // ���ȏͺ�һ���Ñ������ܴa
            if (userID.Equals("") || userPwd.Equals(""))
            {
                throw new Exception("���e");
            }

            Boolean isExisted = this.checkUser(userID);
            if (isExisted)
            {
                return "ԓ�Ñ����ѽ�����";
            }

            // ���Ȍ��ܴa�����D�Q��MD5����            
            try
            {
                userPwd = encodeString(userPwd);
            }
            catch (Exception e)
            {
                throw new Exception("�ܴa����ʧ��!");
            }

            // ���Ñ����YӍ�����Y�ώ�
            String result = "false";          // ԓ׃����춶��x���ص��Y��

            String sql = "INSERT INTO userlist (userName, userSex,"
                   + "userBirth,userEMail,userTel,userProvince,"
                   + "userZIP,userADD,userID,roles,userPass)"
                   + "VALUES (@userName, @userSex,"
                   + "@userBirth,@userEMail,@userTel,@userProvince,"
                   + "@userZIP,@userADD,@userID,@roles,@userPwd)";
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
            oParam = new SqlParameter("@userName", SqlDbType.Char, 50);
            oCmd.Parameters.Add(oParam);
            oParam.Value = userName;
            oParam = new SqlParameter("@userSex", SqlDbType.Char, 50);
            oCmd.Parameters.Add(oParam);
            oParam.Value = userSex;
            oParam = new SqlParameter("@userBirth", SqlDbType.Char, 50);
            oCmd.Parameters.Add(oParam);
            oParam.Value = userBirth;
            oParam = new SqlParameter("@userEmail", SqlDbType.Char, 50);
            oCmd.Parameters.Add(oParam);
            oParam.Value = userEmail;
            oParam = new SqlParameter("@userTel", SqlDbType.Char, 50);
            oCmd.Parameters.Add(oParam);
            oParam.Value = userTel;
            oParam = new SqlParameter("@userProvince", SqlDbType.Char, 50);
            oCmd.Parameters.Add(oParam);
            oParam.Value = userProvince;
            oParam = new SqlParameter("@userZIP", SqlDbType.Char, 50);
            oCmd.Parameters.Add(oParam);
            oParam.Value = userZIP;
            oParam = new SqlParameter("@userADD", SqlDbType.Char, 50);
            oCmd.Parameters.Add(oParam);
            oParam.Value = userADD;
            oParam = new SqlParameter("@userID", SqlDbType.Char, 50);
            oCmd.Parameters.Add(oParam);
            oParam.Value = userID;
            oParam = new SqlParameter("@roles", SqlDbType.Char, 50);
            oCmd.Parameters.Add(oParam);
            oParam.Value = "user";
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

            // ����SQL INSERT
            int row = 0;
            try
            {
                row = oCmd.ExecuteNonQuery();
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

            // �鿴�Ƿ�ɹ��Ը�׃result׃����ֵ
            if (row > 0)
            {
                result = "true";
            }

            // ���ؽY��
            return result;
        }

        /**
         * ԓ������춸����Ñ��YӍ
         * 
         * @param String    userName
         * @param String    userSex
         * @param String    userBirth
         * @param String    userEMail
         * @param String    userTel
         * @param String    userProvince
         * @param String    userADD
         * @param String    userZIP
         * @param String    userID
         * @param String    userPwd
         * @return String
         */
        public String updateUserInfo(String userName, String userSex,
                                       String userBirth, String userEmail,
                                       String userTel, String userProvince,
                                       String userADD, String userZIP,
                                       String userID, String userPwd)
        {

            // ���ȏͺ�һ���Ñ������ܴa
            if (userID.Equals("") || userPwd.Equals(""))
            {
                throw new Exception("���e");
            }

            // ���Ȍ��ܴa�����D�Q��MD5����            
            try
            {
                userPwd = encodeString(userPwd);
            }
            catch (Exception e)
            {
                throw new Exception("�ܴa����ʧ��!");
            }

            // ���Ñ����YӍ�����Y�ώ�
            String result = "false";          // ԓ׃����춶��x���ص��Y��

            String sql = "UPDATE userlist SET userName= @userName," + "userSex= @userSex,"
                 + "userBirth= @userBirth," + "userEMail = @userEMail," + "userTel = @userTel,"
                 + "userProvince= @userProvince," + "userZIP= @userZIP," + "userADD= @userADD,"
                 + "userPass= @userPwd" + " WHERE userID = @userID2 " + " AND userPass= @userPwd2";
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
            oParam = new SqlParameter("@userName", SqlDbType.Char, 50);
            oCmd.Parameters.Add(oParam);
            oParam.Value = userName;
            oParam = new SqlParameter("@userSex", SqlDbType.Char, 50);
            oCmd.Parameters.Add(oParam);
            oParam.Value = userSex;
            oParam = new SqlParameter("@userBirth", SqlDbType.Char, 50);
            oCmd.Parameters.Add(oParam);
            oParam.Value = userBirth;
            oParam = new SqlParameter("@userEmail", SqlDbType.Char, 50);
            oCmd.Parameters.Add(oParam);
            oParam.Value = userEmail;
            oParam = new SqlParameter("@userTel", SqlDbType.Char, 50);
            oCmd.Parameters.Add(oParam);
            oParam.Value = userTel;
            oParam = new SqlParameter("@userProvince", SqlDbType.Char, 50);
            oCmd.Parameters.Add(oParam);
            oParam.Value = userProvince;
            oParam = new SqlParameter("@userZIP", SqlDbType.Char, 50);
            oCmd.Parameters.Add(oParam);
            oParam.Value = userZIP;
            oParam = new SqlParameter("@userADD", SqlDbType.Char, 50);
            oCmd.Parameters.Add(oParam);
            oParam.Value = userADD;
            oParam = new SqlParameter("@userPwd", SqlDbType.Char, 32);
            oCmd.Parameters.Add(oParam);
            oParam.Value = userPwd;
            oParam = new SqlParameter("@userID2", SqlDbType.Char, 50);
            oCmd.Parameters.Add(oParam);
            oParam.Value = userID;
            oParam = new SqlParameter("@userPwd2", SqlDbType.Char, 32);
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

            // ����SQL INSERT
            int row = 0;
            try
            {
                row = oCmd.ExecuteNonQuery();
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

            // �鿴�Ƿ�ɹ��Ը�׃result׃����ֵ
            if (row > 0)
            {
                result = "true";
            }

            // ���ؽY��
            return result;
        }

        // ԓ������춏ͺ��Ñ��Ƿ���ڣ��@��һ��˽�з���
        /**
         * @param  String userID  �Ñ�ID
         * @return Boolean
         */
        private Boolean checkUser(String userID)
        {
            Boolean isExisted = false;           // ԓ׃����춶��x���ص��Y��
            String sql = "SELECT user_id FROM users WHERE " +
                         " user_id = @userID";

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
                    isExisted = true;
                }
            }
            catch (Exception e)
            {
                return false;
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
            return isExisted;
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
