/******************************************************************
 * YSFUser.cs 該類用於用戶管理：用戶註冊和資訊修改
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
         * 該方法用於獲取用戶資訊
         * 
         * @param  String    userID
         * @param  String    userPwd
         * @return Hashtable
         */
        public Hashtable getUserInfo(String userID,
                                     String userPwd)
        {
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
            String sql = "SELECT * FROM userlist WHERE "
                        + " userID = @userID AND userPass = @userPwd";

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

        /**
         * 該方法用於新用戶註冊
         * 
         * @param String    userName 姓名
         * @param String    userSex 性別
         * @param String    userBirth 出生日期
         * @param String    userEMail 電子郵件
         * @param String    userTel 電話
         * @param String    userProvince 省份
         * @param String    userADD 居住地址
         * @param String    userZIP 郵遞區號
         * @param String    userID 用戶ID
         * @param String    userPwd 密碼
         * @return String
         */
        public String newUser(String userName, String userSex,
                                String userBirth, String userEmail,
                                String userTel, String userProvince,
                                String userADD, String userZIP,
                                String userID, String userPwd)
        {
            // 首先復核一下用戶名和密碼
            if (userID.Equals("") || userPwd.Equals(""))
            {
                throw new Exception("出錯");
            }

            Boolean isExisted = this.checkUser(userID);
            if (isExisted)
            {
                return "該用戶名已經存在";
            }

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

            String sql = "INSERT INTO userlist (userName, userSex,"
                   + "userBirth,userEMail,userTel,userProvince,"
                   + "userZIP,userADD,userID,roles,userPass)"
                   + "VALUES (@userName, @userSex,"
                   + "@userBirth,@userEMail,@userTel,@userProvince,"
                   + "@userZIP,@userADD,@userID,@roles,@userPwd)";
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

        /**
         * 該方法用於更新用戶資訊
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

            // 首先復核一下用戶名和密碼
            if (userID.Equals("") || userPwd.Equals(""))
            {
                throw new Exception("出錯");
            }

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

            String sql = "UPDATE userlist SET userName= @userName," + "userSex= @userSex,"
                 + "userBirth= @userBirth," + "userEMail = @userEMail," + "userTel = @userTel,"
                 + "userProvince= @userProvince," + "userZIP= @userZIP," + "userADD= @userADD,"
                 + "userPass= @userPwd" + " WHERE userID = @userID2 " + " AND userPass= @userPwd2";
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
                throw new Exception("資料庫查詢出錯" + e.Message);
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
