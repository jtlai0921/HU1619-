using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using FluorineFx;

namespace org.zhangyafei
{
    [RemotingService("http://www.zhang-yafei.com")]
    public class ResultSetTest
    {
        private String dbhost = "localhost";
        private String dbport = "1433";
        private String dbname = "restaurant";
        private String dbuser = "sa";
        private String dbpass = "verysecret";


        // ԓ������춫@ȡ�Y�ώ�͆Tӛ�
        /**
         * @param  String  firstName
         * @param  String  lastName
         * @param  int     age
         * @return DataTable
         */
        public DataTable getRestaurant()
        {
            DataTable restaurant = new DataTable();

            // [01]==================================================
            // �����ԃ�ִ�
            string sql = "SELECT * FROM main";

            // [02]==================================================
            // ����һ���µ��B�ӣ���ԃ�Y��
            SqlConnection oConn = new SqlConnection();
            oConn.ConnectionString = "Server = " + dbhost +
                                    ";Initial Catalog = " + dbname +
                                    ";UID=" + dbuser +
                                    ";PWD=" + dbpass + ";";
            try
            {
                // ����һ��DataAdapter���K�O�ò�ԃ����
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = new SqlCommand(sql, oConn);
                // ʹ��DataAdapter���DataTable
                adapter.Fill(restaurant);
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

            // [04]==================================================
            // ���ؽY��
            return restaurant;
        }
    }
}
