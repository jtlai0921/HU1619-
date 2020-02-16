using System;
using System.Data;
using System.Data.SqlClient;
using FluorineFx;

namespace org.zhangyafei
{
    [RemotingService("http://www.zhang-yafei.com")]
    public class Employees_paged
    {
        private String dbhost = "localhost";
        private String dbport = "1433";
        private String dbname = "myDatabase";
        private String dbuser = "sa";
        private String dbpass = "verysecret";


        // ԓ������춫@ȡ�Y�ώ�͆Tӛ�
        /**
         * @param  int         offset
		 * @param  int         limit
         * @return DataTable
         */
        public DataTable getEmployeesInfo(int offset, int limit)
        {
            DataTable employeesDT = new DataTable();

            // [01]==================================================
            // �������������ԃ�ִ�
            String sql = " SELECT * " +
                         " FROM (SELECT TOP  " + limit + " * " +
                         "         FROM (SELECT TOP  " +
                                         (limit + offset) + " * " +
                         "                 FROM employees " +
                         "             ORDER BY age ASC) AS foo " +
                         "         ORDER BY age DESC) bar " +
                         " ORDER BY age ";

            // [02]==================================================
            // ����һ���µ��B�ӣ���ԃ�Y��
            SqlConnection oConn = new SqlConnection();
            oConn.ConnectionString = "Server = " + dbhost +
                                    ";Initial Catalog = " + dbname +
                                    ";UID=" + dbuser +
                                    ";PWD=" + dbpass + ";";
            try
            {
                // [03]����һ��DataAdapter���K�O�ò�ԃ����
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = new SqlCommand(sql, oConn);
                // ʹ��DataAdapter���DataTable
                adapter.Fill(employeesDT);
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
            return employeesDT;
        }
    }
}
