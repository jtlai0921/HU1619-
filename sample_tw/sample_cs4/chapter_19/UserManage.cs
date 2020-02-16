/****************************************************************
 * UserManage.cs
 * ԓ�����Ñ������Ñ��]�Ժ��YӍ�޸�
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
using System.Text.RegularExpressions;
using FluorineFx;
using FluorineFx.Context;

namespace org.zhangyafei
{
    [RemotingService("http://www.zhang-yafei.com")]
    public class UserManage
    {
        private String dbhost = "localhost";
        private String dbport = "1433";
        private String dbname = "userManage";
        private String dbuser = "sa";
        private String dbpass = "verysecret";


        // ԓ������춫@ȡ�Ñ��YӍ
        /**
         * @param  String userID
         * @param  String userPwd
         * @param  String checkingCode
         * @return Object
         */
        public Object getUserInfo(String userID,
                                  String userPwd,
                                  String checkingCode)
        {
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
            String sql = "SELECT user_id, user_truename, user_email," +
                  "       user_telephone, user_fax," +
                  "       user_birthday, user_gender " +
                  "FROM users WHERE user_id = @userID AND user_pwd = @userPwd";

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
                    result.Add("user_id", oReader.GetString(oReader.GetOrdinal("user_id")).Trim());
                    result.Add("user_truename",
                            oReader.GetString(oReader.GetOrdinal("user_truename")).Trim());
                    result.Add("user_email",
                            oReader.GetString(oReader.GetOrdinal("user_email")).Trim());
                    result.Add("user_telephone",
                            oReader.GetString(oReader.GetOrdinal("user_telephone")).Trim());
                    result.Add("user_fax",
                            oReader.GetString(oReader.GetOrdinal("user_fax")).Trim());
                    result.Add("user_birthday",
                            oReader.GetDateTime(oReader.GetOrdinal("user_birthday")).ToString("yyyy-MM-dd"));
                    result.Add("user_gender",
                            oReader.GetString(oReader.GetOrdinal("user_gender")).Trim());
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

        // ԓ����������Ñ��]��
        /**
         * @param  String userID        �Ñ�ID
         * @param  String userPwd       �ܴa
         * @param  String userName      ����
         * @param  String userEmail     ����]��
         * @param  String userBirth     ��������
         * @param  String userSex	    �Ԅe
         * @param  String userTel	    �Ԓ
         * @param  String userFax       ����
         * @param  String checkingCode  ��C�a
         * @return String
         */
        public String registerUser(String userID,
                                   String userPwd,
                                   String userName,
                                   String userEmail,
                                   String userBirth,
                                   String userSex,
                                   String userTel,
                                   String userFax,
                                   String checkingCode)
        {
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

            // ���ȏͺ�һ���Ñ������ܴa
            if (userID.Equals("") || userPwd.Equals(""))
            {
                throw new Exception("���e");
            }

            // �ٴ���C�Ñ���ݔ���Ƿ�Ϸ�
            if (!this.validInput(userID, userPwd,
                                userName, userEmail,
                                userBirth, userSex,
                                userTel, userFax))
            {
                return "false";
            }

            Boolean isExisted = this.checkUser(userID);
            if (isExisted)
            {
                return "existed";     // ԓ�Ñ����ѽ�����
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

            // ���Ñ����YӍ�����Y�ώ�
            String result = "false";          // ԓ׃����춶��x���ص��Y��	   
            String sql = "INSERT INTO users (user_id, user_pwd," +
                         "user_truename, user_email," +
                         "user_telephone, user_fax," +
                         "user_birthday, user_gender)" +
                         "VALUES (@userID,@userPwd," +
                         "        @userName,@userEmail," +
                         "        @userTel,@userFax," +
                         "        @userBirth,@userSex)";

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
            oParam = new SqlParameter("@userName", SqlDbType.Char, 20);
            oCmd.Parameters.Add(oParam);
            oParam.Value = userName;
            oParam = new SqlParameter("@userEmail", SqlDbType.Char, 50);
            oCmd.Parameters.Add(oParam);
            oParam.Value = userEmail;
            oParam = new SqlParameter("@userTel", SqlDbType.Char, 20);
            oCmd.Parameters.Add(oParam);
            oParam.Value = userTel;
            oParam = new SqlParameter("@userFax", SqlDbType.Char, 20);
            oCmd.Parameters.Add(oParam);
            oParam.Value = userFax;
            oParam = new SqlParameter("@userBirth", SqlDbType.DateTime);
            oCmd.Parameters.Add(oParam);
            oParam.Value = userBirth;
            oParam = new SqlParameter("@userSex", SqlDbType.Char, 5);
            oCmd.Parameters.Add(oParam);
            oParam.Value = userSex;

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

        // ԓ������춸����Ñ��YӍ
        /**
         * @param  String userID        �Ñ�ID
         * @param  String userPwd       �ܴa
         * @param  String userName      ����
         * @param  String userEmail     ����]��
         * @param  String userBirth     ��������
         * @param  String userSex	    �Ԅe
         * @param  String userTel	    �Ԓ
         * @param  String userFax       ����
         * @param  String checkingCode  ��C�a
         * @return String
         */
        public String updateUserInfo(String userID,
                                     String userPwd,
                                     String userName,
                                     String userEmail,
                                     String userBirth,
                                     String userSex,
                                     String userTel,
                                     String userFax,
                                     String checkingCode)
        {
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

            // ���ȏͺ�һ���Ñ������ܴa
            if (userID.Equals("") || userPwd.Equals(""))
            {
                throw new Exception("���e");
            }

            // �ٴ���C�Ñ���ݔ���Ƿ�Ϸ�
            if (!this.validInput(userID, userPwd,
                                userName, userEmail,
                                userBirth, userSex,
                                userTel, userFax))
            {
                return "false";
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

            // �����Y�ώ���ԓ�Ñ����]���YӍ
            String result = "false";             // ԓ׃����춶��x���ص��Y��
            String sql = "UPDATE users SET user_id= @userID," +
                         "user_pwd= @userPwd," +
                         "user_truename= @userName," +
                         "user_email= @userEmail," +
                         "user_telephone= @userTel," +
                         "user_fax= @userFax," +
                         "user_birthday= @userBirth," +
                         "user_gender= @userSex " +
                         " WHERE user_id = @userID2 " +
                         " AND user_pwd= @userPwd2";


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
            // ���x����
            SqlParameter oParam;
            oParam = new SqlParameter("@userID", SqlDbType.Char, 8);
            oCmd.Parameters.Add(oParam);
            oParam.Value = userID;
            oParam = new SqlParameter("@userPwd", SqlDbType.Char, 32);
            oCmd.Parameters.Add(oParam);
            oParam.Value = userPwd;
            oParam = new SqlParameter("@userName", SqlDbType.Char, 20);
            oCmd.Parameters.Add(oParam);
            oParam.Value = userName;
            oParam = new SqlParameter("@userEmail", SqlDbType.Char, 50);
            oCmd.Parameters.Add(oParam);
            oParam.Value = userEmail;
            oParam = new SqlParameter("@userTel", SqlDbType.Char, 20);
            oCmd.Parameters.Add(oParam);
            oParam.Value = userTel;
            oParam = new SqlParameter("@userFax", SqlDbType.Char, 20);
            oCmd.Parameters.Add(oParam);
            oParam.Value = userFax;
            oParam = new SqlParameter("@userBirth", SqlDbType.DateTime);
            oCmd.Parameters.Add(oParam);
            oParam.Value = userBirth;
            oParam = new SqlParameter("@userSex", SqlDbType.Char, 5);
            oCmd.Parameters.Add(oParam);
            oParam.Value = userSex;
            oParam = new SqlParameter("@userID2", SqlDbType.Char, 8);
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

            // ����SQL UPDATE
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

        // ԓ���������C�Ñ�ݔ���YӍ
        /**
         * @param  String userID        �Ñ�ID
         * @param  String userPwd       �ܴa
         * @param  String userName      ����
         * @param  String userEmail     ����]��
         * @param  String userBirth     ��������
         * @param  String userSex	    �Ԅe
         * @param  String userTel	    �Ԓ
         * @param  String userFax       ����
         * @return Boolean
         */
        private Boolean validInput(String userID,
                                   String userPwd,
                                   String userName,
                                   String userEmail,
                                   String userBirth,
                                   String userSex,
                                   String userTel,
                                   String userFax)
        {
            if (!regexMatch("^[a-zA-Z]{1}([a-zA-Z0-9]|[._]){5,7}",
                               userID))
            {
                return false;
            }

            if (!regexMatch("^\\S{6,8}", userPwd))
            {
                return false;
            }

            if (!regexMatch("^[\u4e00-\u9fa5\uf900-\ufa2d]{1,11}",
                                     userName))
            {
                return false;
            }

            if (!regexMatch("^[a-zA-Z0-9]{1}[\\.a-zA-Z0-9_-]*[a-zA-Z0-9]{1}@[a-zA-Z0-9]+[-]{0,1}[a-zA-Z0-9]+[\\.]{1}[a-zA-Z]+[\\.]{0,1}[a-zA-Z]+", userEmail))
            {
                return false;
            }
            if (!regexMatch("^\\d+", userTel))
            {
                return false;
            }
            if (userFax.Equals("") != true)
            {
                if (!regexMatch("^\\d+", userFax))
                {
                    return false;
                }
            }
            if (!regexMatch("^\\d{4}-\\d{1,2}-\\d{1,2}", userBirth))
            {
                return false;
            }
            return true;
        }


        // ԓ����ʹ�����t�\��ʽ��C�ִ�
        /**
         * @param  String pattern    ģʽ
         * @param  String str        �ִ�
         * @return Boolean
         */
        private Boolean regexMatch(String pattern,
                                   String str)
        {
            Regex re = new Regex(pattern);  //����Regex����
            Match m = re.Match(str);
            // �M��ƥ��yԇ���K�����Ƿ�ƥ��ɹ�
            if (m.Success)
            {
                return true;
            }
            return false;
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
