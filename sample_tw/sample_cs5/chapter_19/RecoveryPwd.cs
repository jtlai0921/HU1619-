/****************************************************************
 * RecoveryPwd.cs
 * ԓ���춰l���]���o�Ñ�׌�����«@���ܴa
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
using System.Net.Mail;
using FluorineFx;
using FluorineFx.Context;

namespace org.zhangyafei
{
    [RemotingService("http://www.zhang-yafei.com")]
    public class RecoveryPwd
    {
        private String dbhost = "localhost";
        private String dbport = "1433";
        private String dbname = "userManage";
        private String dbuser = "sa";
        private String dbpass = "verysecret";


        // ԓ�����Á�׌�Ñ���ʰ�ܴa
        /**
         * @param  String  userEmail
         * @param  String  checkingCode
         * @return String
         */
        public String getPwd(String userEmail,
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

            // ����6���S�C��Ԫ�������ܴa
            // ���ȶ��x���õ���Ԫ����Ԫ����
            String ConstCode = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            int codeAmount = ConstCode.Length;
            String userPwd = "";
            Random random = new Random();
            for (int i = 0; i <= 5; i++)
            {
                int index = random.Next(codeAmount - 1);
                String rand = ConstCode.Substring(index, 1);
                userPwd += rand;
            }

            Boolean result = false;               // ԓ׃����춶��x���ص��Y��

            // ���ȸ����Y�ώ��ܴa
            try
            {
                result = this.updatePwd(userEmail, userPwd);
            }
            catch (Exception e)
            {
                return "false";                     // �ܴa����ʧ��
            }

            // ����Y�ώ���³ɹ������N�Ͱl���]��
            if (result)
            {
                // �ٙz���]���l�ͳɹ������N����true����t����false
                result = this.sendMail(userEmail, userPwd);
            }
            else
            {
                return "false";
            }

            return result.ToString().ToLower();
        }

        // ԓ�����Á�����Ñ��ܴa
        /**
         * @param  String userEmail
         * @param  String userPwd
         * @return Boolean
         */
        private Boolean updatePwd(String userEmail,
                             String userPwd)
        {
            // ���ܴa�����D�Q��MD5����
            String md_userPwd;

            try
            {
                md_userPwd = encodeString(userPwd);
            }
            catch (Exception e)
            {
                throw new Exception("�ܴa����ʧ��!");
            }

            Boolean result = false;               // ԓ׃����춶��x���ص��Y��

            String sql = "UPDATE users SET user_pwd= @userPwd " +
                         " WHERE user_email = @userEmail";
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
            oParam = new SqlParameter("@userPwd", SqlDbType.Char, 32);
            oCmd.Parameters.Add(oParam);
            oParam.Value = md_userPwd;
            oParam = new SqlParameter("@userEmail", SqlDbType.Char, 50);
            oCmd.Parameters.Add(oParam);
            oParam.Value = userEmail;
            try
            {
                oConn.Open();
            }
            catch (Exception e)
            {
                return false;
            }

            // ����SQL UPDATE
            int row = 0;
            try
            {
                row = oCmd.ExecuteNonQuery();
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
                    oConn.Close();
                }
                catch (Exception e2) { }
            }

            // �鿴�Ƿ�ɹ��Ը�׃result׃����ֵ
            if (row > 0)
            {
                result = true;
            }

            // ���ؽY��
            return result;
        }

        // ԓ�����Á����Ñ��l���]��
        /**
         * @param  String userEmail
         * @param  String userPwd
         * @return Boolean
         */
        private Boolean sendMail(String userEmail, String userPwd)
        {
            String smtpHost = "smtp.tom.com";      // SMTP�ŷ�����ַ
            String smtpAuth = "true";                // SMTP�ŷ����Ƿ���Ҫ��C
            int smtpPort = 25;                  // SMTP�ŷ�����
            String mailAddr = "zhang-yafei@tom.com";  // �]���ַ
            String mailUser = "zhang-yafei";            // �]���Ñ���
            String mailPwd = "verysecret";              // �]���ܴa

            // =====================�����]��===============
            // �ڄ�������rָ���l���˵��]��������˵��]��
            MailMessage oMsg = new MailMessage(mailAddr, userEmail);
            oMsg.Subject = "noreply-RecoveryPwd";
            oMsg.Body = "�@��������ܴa,Ո���Ʊ���:" + userPwd;
            oMsg.BodyEncoding = System.Text.Encoding.UTF8;
            // =====================�M�аl���O��=================
            //ע��SmtpClient���O�ã����C������̖���Ñ������ܴa
            //Ĭ�J��̖��25���@�����Կ���ʡ��
            //��ʹ�ñ���SMTP���Օr�����]�ж��xҪ���Ñ���C�����Ҳ����ʡ��Credentials����
            SmtpClient client = new SmtpClient();
            client.Host = smtpHost;
            client.Port = smtpPort;
            client.Credentials =
            new System.Net.NetworkCredential(mailUser, mailPwd);
            try
            {
                // =====================���аl��=====================
                // �l����Ϣ��������ǰl���]��
                client.Send(oMsg);
            }
            catch (Exception e)
            {
                return false;        // �l���]�����e
            }
            return true;            // �ɹ��l���]��
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
