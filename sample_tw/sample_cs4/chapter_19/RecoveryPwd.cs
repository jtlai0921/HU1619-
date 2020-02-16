/****************************************************************
 * RecoveryPwd.cs
 * 該類用於發送郵件給用戶讓他重新獲得密碼
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


        // 該方法用來讓用戶重拾密碼
        /**
         * @param  String  userEmail
         * @param  String  checkingCode
         * @return String
         */
        public String getPwd(String userEmail,
                        String checkingCode)
        {
            // 引用Session
            ISessionState session = FluorineContext.Current.Session;
            // 檢查驗證碼是否匹配
            // 使用用戶傳遞的參數與Session中保存的字串進行對比
            // 如果不匹配就返回"checkingCodeFalse"
            Object s_checkingCode = session["checkingCode"];
            if (s_checkingCode == null)
            {
                return "checkingCodeFalse";
            }
            else
            {
                // 都轉換為小寫進行匹配
                String s_checkingCode_lower =
                                      s_checkingCode.ToString().ToLower();
                if (!(checkingCode.ToLower()).Equals(
                                                   s_checkingCode_lower))
                {
                    return "checkingCodeFalse";
                }
            }

            // 創建6個隨機字元作為新密碼
            // 首先定義可用的字元和字元數量
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

            Boolean result = false;               // 該變數用於定義返回的資料

            // 首先更新資料庫密碼
            try
            {
                result = this.updatePwd(userEmail, userPwd);
            }
            catch (Exception e)
            {
                return "false";                     // 密碼更新失敗
            }

            // 如果資料庫更新成功，那麼就發送郵件
            if (result)
            {
                // 再檢查郵件發送成功，那麼返回true，否則返回false
                result = this.sendMail(userEmail, userPwd);
            }
            else
            {
                return "false";
            }

            return result.ToString().ToLower();
        }

        // 該方法用來更新用戶密碼
        /**
         * @param  String userEmail
         * @param  String userPwd
         * @return Boolean
         */
        private Boolean updatePwd(String userEmail,
                             String userPwd)
        {
            // 將密碼明文轉換為MD5密文
            String md_userPwd;

            try
            {
                md_userPwd = encodeString(userPwd);
            }
            catch (Exception e)
            {
                throw new Exception("密碼加密失敗!");
            }

            Boolean result = false;               // 該變數用於定義返回的資料

            String sql = "UPDATE users SET user_pwd= @userPwd " +
                         " WHERE user_email = @userEmail";
            // 創建一個新的連接，查詢資料
            SqlConnection oConn = new SqlConnection();
            oConn.ConnectionString = "Server = " + dbhost +
                                     ";Initial Catalog = " + dbname +
                                     ";UID=" + dbuser +
                                     ";PWD=" + dbpass + ";";
            // 創建一個SqlCommand物件用來將查詢和命令發送給資料庫
            SqlCommand oCmd = oConn.CreateCommand();
            oCmd.CommandText = sql;
            oCmd.CommandType = CommandType.Text;
            // 定義參數
            // 定義參數
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

            // 執行SQL UPDATE
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
                    // 顯式的關閉對象
                    oConn.Close();
                }
                catch (Exception e2) { }
            }

            // 查看是否成功以改變result變數的值
            if (row > 0)
            {
                result = true;
            }

            // 返回結果
            return result;
        }

        // 該方法用來向用戶發送郵件
        /**
         * @param  String userEmail
         * @param  String userPwd
         * @return Boolean
         */
        private Boolean sendMail(String userEmail, String userPwd)
        {
            String smtpHost = "smtp.tom.com";      // SMTP伺服器地址
            String smtpAuth = "true";                // SMTP伺服器是否需要驗證
            int smtpPort = 25;                  // SMTP伺服器埠
            String mailAddr = "zhang-yafei@tom.com";  // 郵箱地址
            String mailUser = "zhang-yafei";            // 郵箱用戶名
            String mailPwd = "verysecret";              // 郵箱密碼

            // =====================構造郵件===============
            // 在創建對象時指定發信人的郵箱和收信人的郵箱
            MailMessage oMsg = new MailMessage(mailAddr, userEmail);
            oMsg.Subject = "noreply-RecoveryPwd";
            oMsg.Body = "這是你的新密碼,請妥善保管:" + userPwd;
            oMsg.BodyEncoding = System.Text.Encoding.UTF8;
            // =====================進行發送設置=================
            //注意SmtpClient的設置：主機名、埠號、用戶名和密碼
            //默認埠號是25，這個屬性可以省略
            //當使用本地SMTP服務時，因為沒有定義要求用戶驗證，因此也可以省略Credentials屬性
            SmtpClient client = new SmtpClient();
            client.Host = smtpHost;
            client.Port = smtpPort;
            client.Credentials =
            new System.Net.NetworkCredential(mailUser, mailPwd);
            try
            {
                // =====================執行發送=====================
                // 發送消息物件，便是發送郵件
                client.Send(oMsg);
            }
            catch (Exception e)
            {
                return false;        // 發送郵件出錯
            }
            return true;            // 成功發送郵件
        }

        // 該方法用於加密字串，通常是加密密碼
        /**
        * @param  String userPwd
        * @return String
        */
        private String encodeString(String userPwd)
        {
            String result = "";

            // 將密碼明文轉換為MD5密文	   
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
                throw new Exception("密碼加密失敗!");
            }

            return result;
        }
    }
}
