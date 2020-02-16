using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using FluorineFx;

namespace org.zhangyafei
{
    [RemotingService("http://www.zhang-yafei.com")]
    public class PagedRecordSet
    {
        private String dbhost = "localhost";
        private String dbport = "1433";
        private String dbname = "myDatabase";
        private String dbuser = "sa";
        private String dbpass = "verysecret";


        // ԓ���������������ʽ�@ȡ�Y�ώ�͆Tӛ�
        /**
         * @param  String  firstName
         * @param  String  lastName
         * @param  int     age
         * @return DataTable
         */
        [PageSize(10, 0, 10)]
        public DataTable getEmployeesInfo(string firstName,
                                                 string lastName,
                                                 int age)
        {
            DataTable employeesDT = new DataTable();

            // �@ȡƫ������ÿ퓵�ӛ䛔�
            int offset = PagingContext.Current.Offset;
            int limit = PagingContext.Current.Limit;

            // [01]==================================================
            // �������������ԃ�ִ�
            String sql;
            if (firstName == null && lastName == null && age == 0)
            {
                // ����SELECT TOP�Z�䣬��������ӛ�
                sql = "SELECT firstName, lastName, EmpType, age " +
                       "  FROM (SELECT TOP " + limit +
                       "        firstName, lastName, EmpType, age" +
                       "  FROM (SELECT TOP " + (offset + limit) +
                       "        firstName, lastName, EmpType, age" +
                       "  FROM employees" +
                       "  ORDER BY age ASC, firstName ASC, " +
                       "           lastName ASC, EmpType ASC) " +
                       "  AS foo " +
                       "  ORDER BY age DESC, firstName DESC, " +
                       "           lastName DESC, EmpType DESC) " +
                       "  AS bar " +
                       "  ORDER BY age ASC, firstName ASC, " +
                       "           lastName ASC, EmpType ASC";
            }
            else
            {
                // ���ط��ϗl����ӛ�
                sql = "SELECT firstName, lastName, EmpType, age " +
                       "  FROM (SELECT TOP " + limit +
                       "        firstName, lastName, EmpType, age" +
                       "  FROM (SELECT TOP " + (offset + limit) +
                       "        firstName, lastName, EmpType, age" +
                       "  FROM employees WHERE firstName LIKE '%" +
                                               firstName + "%' AND " +
                                               " lastName LIKE '%" +
                                               lastName + "%' AND " +
                                               " age >= " + age +
                       "  ORDER BY age ASC, firstName ASC, " +
                       "           lastName ASC, EmpType ASC) " +
                       "  AS foo " +
                       "  ORDER BY age DESC, firstName DESC, " +
                       "           lastName DESC, EmpType DESC) " +
                       "  AS bar " +
                       "  ORDER BY age ASC, firstName ASC, " +
                       "           lastName ASC, EmpType ASC";
            }

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
                adapter.Fill(employeesDT);
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

            // [03]==================================================
            // ���ؽY��
            return employeesDT;
        }


        // ������Ҫ���ط��ӛ䛼��ķ�����Ҫ���xһ����Ƶķ�����
        // ����������Count��getEmployeesInfo+Count����
        // FluorineFx���Ԅ��{��ԓ��������ӛ䛼�����
        // ԓ�����ą�����������cgetEmployeesInfo������ͬ
        /**
         * @return int
         */
        public int getEmployeesInfoCount(String firstName,
                                            String lastName,
                                            int age)
        {
            object result = 0;

            // [01]==================================================
            // �������������ԃ�ִ�
            String sql;
            if (firstName.Equals("") && lastName.Equals("") && age == 0)
            {
                // ע���@�Y����������������ֵ�ą��������N�ͷ�������ӛ�
                sql = "SELECT COUNT(*) FROM  employees";
            }
            else
            {
                sql = "SELECT COUNT(*) FROM " +
                       " employees WHERE firstName LIKE '%" +
                        firstName + "%' AND lastName LIKE '%" + lastName
                       + "%' AND age >= " + age;
            }

            // [02]==================================================
            // ����һ���µ��B�ӣ���ԃ�Y�ώ�
            SqlConnection oConn = new SqlConnection();
            oConn.ConnectionString = "Server = " + dbhost +
                                    ";Initial Catalog = " + dbname +
                                    ";UID=" + dbuser +
                                    ";PWD=" + dbpass + ";";
            try
            {
                // ���в�ԃ����@ȡӛ䛿���
                oConn.Open();
                SqlCommand oCmd = oConn.CreateCommand();
                oCmd.CommandText = sql;
                result = oCmd.ExecuteScalar();
            }
            catch (Exception e)
            {
                throw new Exception("�Y�ώ��ԃ���e" + e.Message + sql);
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

            // [03]==================================================
            // ���ؿ��õ��Y��ӛ䛿���
            return System.Convert.ToInt32(result);
        }
    }
}
