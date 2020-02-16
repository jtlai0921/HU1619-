/****************************************************************
 * UserLogin.cs
 * 該類用於查詢資料庫，實現用戶登錄
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


        // 該方法用於查詢資料庫，查看是否有匹配的用戶
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

            // 由於保存在資料庫中的密碼已經是轉換為密文的字串
            // 所以，在這裏也應該作轉換才能進行匹配
            // 首先將密碼明文轉換為MD5密文            
            try
            {
                userPwd = encodeString(userPwd);
            }
            catch (Exception e)
            {
                throw new Exception("密碼加密失敗!");
            }

            // 查詢資料庫，查找是否有匹配的用戶存在
            // 如果存在就返回該用戶的id
            // 否則返回字串false
            String sql = "SELECT user_id FROM users WHERE " +
                         " user_id = @userID AND user_pwd = @userPwd";

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
                throw new Exception("資料庫查詢出錯");
            }

            // 根據查詢結果構造返回值
            // 使用ExecuteReader方法執行查詢
            // 並將查詢結果賦給一個SqlDataReader
            SqlDataReader oReader = null;
            try
            {
                oReader = oCmd.ExecuteReader();
                // 先設定返回的值為false
                user = "false";
                // 察看結果是否為空，從而確定用戶是否存在
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
                    // 顯式的關閉對象
                    oReader.Close();
                    oConn.Close();
                }
                catch (Exception e2) { }
            }

            // 返回結果
            return user;
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
