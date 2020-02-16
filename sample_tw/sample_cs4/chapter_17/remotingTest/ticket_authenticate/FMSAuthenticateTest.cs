/*****************************************************************
 * FMSAuthenticateTest.cs
 * 該類用於處理資料庫操作，使用Ticket方式驗證用戶
 * @author   zhang-yafei.com
 * @version  1.0.0.128  2009/2/20
 * @since    .NET CLR 1.0
 *****************************************************************/

using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using FluorineFx;

[RemotingService("http://www.zhang-yafei.com")]
public class FMSAuthenticateTest
{

    private Boolean logined = false;
    private String dbhost = "localhost";
    private String dbport = "1433";
    private String dbname = "users";
    private String dbuser = "sa";
    private String dbpass = "verysecret";

    // 該方法用來驗證用戶
    /**
     * @param  String  userID
     * @param  String  userPwd
     * @return String[]
     */
    public String[] checkUser(String userID, String userPwd)
    {

        // 構造SQL語句
        String sql = "SELECT * FROM main WHERE " +
                     " username = @userID AND userpwd = @userPwd";

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

        SqlParameter oParam;
        oParam = new SqlParameter("@userID", SqlDbType.Char, 10);
        oCmd.Parameters.Add(oParam);
        oParam.Value = userID;
        oParam = new SqlParameter("@userPwd", SqlDbType.Char, 10);
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
        // 使用ExecuteReader方法執行查詢，並將查詢結果賦給一個SqlDataReader
        SqlDataReader oReader = null;
        try
        {
            oReader = oCmd.ExecuteReader();
            // 檢查是否存在符合條件的記錄
            if (oReader.Read())
            {
                logined = true;
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

        String[] result = null;
        if (logined)
        {
            // 發現該用戶
            // 創建一個UUID並保存到資料庫中
            String token = Guid.NewGuid().ToString();
            // 調用setTicket方法執行保存工作
            if (this.setTicket(userID, token))
            {
                result = new String[2];
                result[0] = "true";
                result[1] = token;
            }
            else
            {
                result = new String[1];
                result[0] = "false";
            }
        }
        else
        {
            result = new String[1];
            result[0] = "false";
        }
        return result;
    }

    // 該方法用來插入Ticket到資料庫
    /**
     * @param  String  userID
     * @param  String  ticket
     * @return Boolean
     */
    private Boolean setTicket(String userID, String ticket)
    {

        // 獲取當前時間
        // 注意定義登錄的延時是2分鐘
        DateTime dateTime = DateTime.Now;
        dateTime = dateTime.AddMinutes(2);

        // 構造SQL語句
        String sql = "INSERT INTO tickets (name, ticket, expire_time)" +
                 "VALUES(@userID,@ticket,@dateTime)";

        // 創建一個新的連接
        SqlConnection oConn = new SqlConnection();
        oConn.ConnectionString = "Server = " + dbhost +
                          ";Initial Catalog = " + dbname +
                          ";UID=" + dbuser +
                          ";PWD=" + dbpass + ";";
        SqlCommand oCmd = oConn.CreateCommand();
        oCmd.CommandType = CommandType.Text;
        oCmd.CommandText = sql;

        SqlParameter oParam;
        oParam = new SqlParameter("@userID", SqlDbType.Char);
        oCmd.Parameters.Add(oParam);
        oParam.Value = userID;

        oParam = new SqlParameter("@ticket", SqlDbType.Char);
        oCmd.Parameters.Add(oParam);
        oParam.Value = ticket;

        oParam = new SqlParameter("@dateTime", SqlDbType.DateTime);
        oCmd.Parameters.Add(oParam);
        oParam.Value = dateTime;

        try
        {
            oConn.Open();
        }
        catch (Exception e)
        {
            throw new Exception("資料庫查詢出錯");
        }

        // 執行SQL INSERT
        int myInsert = 0;
        try
        {
            myInsert = oCmd.ExecuteNonQuery();
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

        // 檢查是否成功插入資料
        if (myInsert > 0)
        {
            return true;
        }

        return false;
    }

    // 該方法用來查找是否存在Ticket
    /**
     * @param  String   ticket
     * @return Boolean
     */
    public Boolean findTicket(String ticket)
    {
        logined = false;

        // 獲取當前時間
        DateTime dateTime = DateTime.Now;

        // 構造SQL語句
        String sql = "SELECT main.username FROM main " +
             " INNER JOIN tickets ON main.username = tickets.name " +
             " WHERE tickets.ticket = @ticket AND tickets.expire_time > @dateTime";

        // 創建一個新的連接，查詢資料
        SqlConnection oConn = new SqlConnection();
        oConn.ConnectionString = "Server = " + dbhost +
                           ";Initial Catalog = " + dbname +
                           ";UID=" + dbuser +
                           ";PWD=" + dbpass + ";";
        // 創建一個SqlCommand物件用來將查詢和命令發送給資料庫	   
        SqlCommand oCmd = oConn.CreateCommand();
        oCmd.CommandType = CommandType.Text;
        oCmd.CommandText = sql;

        SqlParameter oParam;
        oParam = new SqlParameter("@ticket", SqlDbType.Char);
        oCmd.Parameters.Add(oParam);
        oParam.Value = ticket;

        oParam = new SqlParameter("@dateTime", SqlDbType.DateTime);
        oCmd.Parameters.Add(oParam);
        oParam.Value = dateTime;

        try
        {
            oConn.Open();
        }
        catch (Exception e)
        {
            throw new Exception("資料庫查詢出錯");
        }

        // 根據查詢結果構造返回值
        // 使用ExecuteReader方法執行查詢，並將查詢結果賦給一個SqlDataReader
        SqlDataReader oReader = null;
        try
        {
            oReader = oCmd.ExecuteReader();
            // 檢查是否存在符合條件的記錄
            if (oReader.Read())
            {
                logined = true;
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
        return logined;
    }
}
