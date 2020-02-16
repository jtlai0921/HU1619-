/*****************************************************************
 * FMSAuthenticateTest.cs
 * ԓ����̎���Y�ώ������ʹ��Ticket��ʽ��C�Ñ�
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
     * @return String[]
     */
    public String[] checkUser(String userID, String userPwd)
    {

        // ����SQL�Z��
        String sql = "SELECT * FROM main WHERE " +
                     " username = @userID AND userpwd = @userPwd";

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

        String[] result = null;
        if (logined)
        {
            // �l�Fԓ�Ñ�
            // ����һ��UUID�K���浽�Y�ώ���
            String token = Guid.NewGuid().ToString();
            // �{��setTicket�������б��湤��
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

    // ԓ�����Á����Ticket���Y�ώ�
    /**
     * @param  String  userID
     * @param  String  ticket
     * @return Boolean
     */
    private Boolean setTicket(String userID, String ticket)
    {

        // �@ȡ��ǰ�r�g
        // ע�ⶨ�x��䛵��ӕr��2���
        DateTime dateTime = DateTime.Now;
        dateTime = dateTime.AddMinutes(2);

        // ����SQL�Z��
        String sql = "INSERT INTO tickets (name, ticket, expire_time)" +
                 "VALUES(@userID,@ticket,@dateTime)";

        // ����һ���µ��B��
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
            throw new Exception("�Y�ώ��ԃ���e");
        }

        // ����SQL INSERT
        int myInsert = 0;
        try
        {
            myInsert = oCmd.ExecuteNonQuery();
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

        // �z���Ƿ�ɹ������Y��
        if (myInsert > 0)
        {
            return true;
        }

        return false;
    }

    // ԓ�����Á�����Ƿ����Ticket
    /**
     * @param  String   ticket
     * @return Boolean
     */
    public Boolean findTicket(String ticket)
    {
        logined = false;

        // �@ȡ��ǰ�r�g
        DateTime dateTime = DateTime.Now;

        // ����SQL�Z��
        String sql = "SELECT main.username FROM main " +
             " INNER JOIN tickets ON main.username = tickets.name " +
             " WHERE tickets.ticket = @ticket AND tickets.expire_time > @dateTime";

        // ����һ���µ��B�ӣ���ԃ�Y��
        SqlConnection oConn = new SqlConnection();
        oConn.ConnectionString = "Server = " + dbhost +
                           ";Initial Catalog = " + dbname +
                           ";UID=" + dbuser +
                           ";PWD=" + dbpass + ";";
        // ����һ��SqlCommand����Á팢��ԃ������l�ͽo�Y�ώ�	   
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
