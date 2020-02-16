/*****************************************************************
 * FMSAuthenticateTest.cs
 * 該類用於查詢資料庫，檢查用戶是否存在
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
     * @return boolean
     */
    public Boolean checkUser(String userID, String userPwd)
    {

        // 構造SQL語句
        String sql = "SELECT * FROM main WHERE " +
                 " username = '" + userID +
                 "' AND userpwd = '" + userPwd + "'";

        // 創建一個新的連接，查詢資料
        SqlConnection oConn = new SqlConnection();
        oConn.ConnectionString = "Server = " + dbhost +
                                 ";Initial Catalog = " + dbname +
                                 ";UID=" + dbuser +
                                 ";PWD=" + dbpass + ";";
        // 創建一個SqlCommand物件用來將查詢和命令發送給資料庫	   
        SqlCommand oCmd = oConn.CreateCommand();
        oCmd.CommandText = sql;

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
