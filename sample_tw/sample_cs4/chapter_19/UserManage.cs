/****************************************************************
 * UserManage.cs
 * 該類用於用戶管理：用戶註冊和資訊修改
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


        // 該方法用於獲取用戶資訊
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

            Hashtable result = null;          // 該變數用於定義返回的資料

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

            // 查詢資料庫返回該用戶的註冊資訊
            String sql = "SELECT user_id, user_truename, user_email," +
                  "       user_telephone, user_fax," +
                  "       user_birthday, user_gender " +
                  "FROM users WHERE user_id = @userID AND user_pwd = @userPwd";

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

            // [03]==================================================
            // 根據查詢結果構造返回值
            // 使用ExecuteReader方法執行查詢，並將查詢結果賦給一個SqlDataReader
            SqlDataReader oReader = null;
            try
            {
                oReader = oCmd.ExecuteReader();
                // 使用oReader.Read()方法看是否存在記錄
                // 遍曆結果集構造一個Hashtable
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
                throw new Exception("資料庫查詢出錯");
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
            return result;
        }

        // 該方法用於新用戶註冊
        /**
         * @param  String userID        用戶ID
         * @param  String userPwd       密碼
         * @param  String userName      姓名
         * @param  String userEmail     電子郵件
         * @param  String userBirth     出生日期
         * @param  String userSex	    性別
         * @param  String userTel	    電話
         * @param  String userFax       傳真
         * @param  String checkingCode  驗證碼
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

            // 首先復核一下用戶名和密碼
            if (userID.Equals("") || userPwd.Equals(""))
            {
                throw new Exception("出錯");
            }

            // 再次驗證用戶的輸入是否合法
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
                return "existed";     // 該用戶名已經存在
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

            // 將用戶的資訊插入資料庫
            String result = "false";          // 該變數用於定義返回的資料	   
            String sql = "INSERT INTO users (user_id, user_pwd," +
                         "user_truename, user_email," +
                         "user_telephone, user_fax," +
                         "user_birthday, user_gender)" +
                         "VALUES (@userID,@userPwd," +
                         "        @userName,@userEmail," +
                         "        @userTel,@userFax," +
                         "        @userBirth,@userSex)";

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
                throw new Exception("資料庫查詢出錯");
            }

            // 執行SQL INSERT
            int row = 0;
            try
            {
                row = oCmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception("資料庫查詢出錯");
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
                result = "true";
            }

            // 返回結果
            return result;
        }

        // 該方法用於更新用戶資訊
        /**
         * @param  String userID        用戶ID
         * @param  String userPwd       密碼
         * @param  String userName      姓名
         * @param  String userEmail     電子郵件
         * @param  String userBirth     出生日期
         * @param  String userSex	    性別
         * @param  String userTel	    電話
         * @param  String userFax       傳真
         * @param  String checkingCode  驗證碼
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

            // 首先復核一下用戶名和密碼
            if (userID.Equals("") || userPwd.Equals(""))
            {
                throw new Exception("出錯");
            }

            // 再次驗證用戶的輸入是否合法
            if (!this.validInput(userID, userPwd,
                                userName, userEmail,
                                userBirth, userSex,
                                userTel, userFax))
            {
                return "false";
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

            // 更新資料庫中該用戶的註冊資訊
            String result = "false";             // 該變數用於定義返回的資料
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
                throw new Exception("資料庫查詢出錯");
            }

            // 執行SQL UPDATE
            int row = 0;
            try
            {
                row = oCmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception("資料庫查詢出錯");
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
                result = "true";
            }

            // 返回結果
            return result;
        }

        // 該方法用於復核用戶是否存在，這是一個私有方法
        /**
         * @param  String userID  用戶ID
         * @return Boolean
         */
        private Boolean checkUser(String userID)
        {
            Boolean isExisted = false;           // 該變數用於定義返回的資料
            String sql = "SELECT user_id FROM users WHERE " +
                         " user_id = @userID";

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
                // 察看結果是否為空，從而確定用戶是否存在
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
                    // 顯式的關閉對象
                    oReader.Close();
                    oConn.Close();
                }
                catch (Exception e2) { }
            }

            // 返回結果
            return isExisted;
        }

        // 該方法用於驗證用戶輸入資訊
        /**
         * @param  String userID        用戶ID
         * @param  String userPwd       密碼
         * @param  String userName      姓名
         * @param  String userEmail     電子郵件
         * @param  String userBirth     出生日期
         * @param  String userSex	    性別
         * @param  String userTel	    電話
         * @param  String userFax       傳真
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


        // 該方法使用正則運算式驗證字串
        /**
         * @param  String pattern    模式
         * @param  String str        字串
         * @return Boolean
         */
        private Boolean regexMatch(String pattern,
                                   String str)
        {
            Regex re = new Regex(pattern);  //創建Regex實例
            Match m = re.Match(str);
            // 進行匹配測試，並寫出是否匹配成功
            if (m.Success)
            {
                return true;
            }
            return false;
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
