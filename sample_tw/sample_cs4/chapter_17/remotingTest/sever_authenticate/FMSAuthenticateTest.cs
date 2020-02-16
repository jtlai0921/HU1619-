/*****************************************************************
 * FMSAuthenticateTest.cs
 * ԓ���춲�ԃ�Y�ώ죬�z���Ñ��Ƿ����
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

    // ԓ�����Á���C�Ñ�
    /**
     * @param  String  userID
     * @param  String  userPwd
     * @return boolean
     */
    public Boolean checkUser(String userID, String userPwd)
    {

        // ����SQL�Z��
        String sql = "SELECT * FROM main WHERE " +
                 " username = '" + userID +
                 "' AND userpwd = '" + userPwd + "'";

        // ����һ���µ��B�ӣ���ԃ�Y��
        SqlConnection oConn = new SqlConnection();
        oConn.ConnectionString = "Server = " + dbhost +
                                 ";Initial Catalog = " + dbname +
                                 ";UID=" + dbuser +
                                 ";PWD=" + dbpass + ";";
        // ����һ��SqlCommand����Á팢��ԃ������l�ͽo�Y�ώ�	   
        SqlCommand oCmd = oConn.CreateCommand();
        oCmd.CommandText = sql;

        try
        {
            oConn.Open();
        }
        catch (Exception e)
        {
            throw new Exception("�Y�ώ��ԃ���e");
        }

        // ������ԃ�Y�����췵��ֵ
        // ʹ��ExecuteReader�������в�ԃ���K����ԃ�Y���x�oһ��SqlDataReader
        SqlDataReader oReader = null;
        try
        {
            oReader = oCmd.ExecuteReader();
            // �z���Ƿ���ڷ��ϗl����ӛ�
            if (oReader.Read())
            {
                logined = true;
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
        return logined;
    }
}
