using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using FluorineFx;

namespace org.zhangyafei
{
    [RemotingService("http://www.zhang-yafei.com")]
    public class ManageEmployees
    {
        private String dbhost = "localhost";
        private String dbport = "1433";
        private String dbname = "myDatabase";
        private String dbuser = "sa";
        private String dbpass = "verysecret";


        // ԓ������춫@ȡ�Y�ώ�͆Tӛ�
        /**
         * @param  String  firstName
         * @param  String  lastName
         * @param  int     age
         * @return Array
         */
        public Array getEmployeesInfo(String firstName,
                                            String lastName,
                                            int age)
        {
            // ����һ��Array��춷��ز�ԃӛ䛣�
            // �ڶ��xҪ���ص��Y�ϕr��һ��Ҫע��FluorineFx֧�ֵ��Y����ͣ�
            // ֻ��֧Ԯ���Y����Ͳ��ܱ�FlashӰƬ���ó�ʽ�R�e�K���_�������л�
            Array employeesArray;

            // [01]==================================================
            // �������������ԃ�ִ�
            String sql;
            if (firstName.Equals("") && lastName.Equals("") && age == 0)
            {
                // ע���@�Y����������������ֵ�ą��������N�ͷ�������ӛ�
                sql = "SELECT firstName, lastName, EmpType, age FROM " +
                       " employees";
            }
            else
            {
                sql = "SELECT firstName, lastName, EmpType, age FROM " +
                       " employees WHERE firstName LIKE '" +
                        firstName + "' AND lastName LIKE '" + lastName +
                        "' AND age >= " + age;
            }

            // [02]==================================================
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

            // [03]==================================================
            // ������ԃ�Y�����췵��ֵ
            // ʹ��ExecuteReader�������в�ԃ���K����ԃ�Y���x�oһ��SqlDataReader
            ArrayList employees;
            SqlDataReader oReader = null;
            try
            {
                oReader = oCmd.ExecuteReader();
                employees = new ArrayList();
                // ʹ��oReader.Read()�������Ƿ����ӛ�
                // ��ѽY��������ArrayList��ÿ��Ԫ�ؾ���һ��Hashtable
                while (oReader.Read())
                {
                    Hashtable employee = new Hashtable();
                    employee.Add("firstName", oReader.GetValue(
                                        oReader.GetOrdinal("firstName")));
                    employee.Add("lastName", oReader.GetValue(
                                        oReader.GetOrdinal("lastName")));
                    employee.Add("EmpType", oReader.GetValue(
                                        oReader.GetOrdinal("EmpType")));
                    employee.Add("age", oReader.GetValue(
                                        oReader.GetOrdinal("age")));
                    employees.Add(employee);
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

            // [04]==================================================
            // ���ؽY��
            // ע��ArrayList�Y����Ͳ���֧�֣������D����Array
            employeesArray = employees.ToArray();
            return employeesArray;
        }

        // ���ӹ͆T
        /**
         * @param  String  firstName
         * @param  String  lastName
         * @param  String  EmpType
         * @param  int     age
         * @return Boolean
         */
        public Boolean newEmployeesInfo(String firstName,
                                              String lastName,
                                              String EmpType,
                                              int age)
        {
            Boolean result = false;

            // [01]==================================================
            // ����һ��SQL INSERT�Z��
            String sql = "INSERT INTO employees " +
                           " (firstName,lastName,EmpType,age) VALUES ('" +
                           firstName + "','" + lastName + "','" + EmpType +
                           "'," + age + ")";
            // [02]==================================================
            // ����һ���µ��B��
            SqlConnection oConn = new SqlConnection();
            oConn.ConnectionString = "Server = " + dbhost +
                                    ";Initial Catalog = " + dbname +
                                    ";UID=" + dbuser +
                                    ";PWD=" + dbpass + ";";
            SqlCommand insertCommand = oConn.CreateCommand();
            insertCommand.CommandText = sql;
            try
            {
                oConn.Open();
            }
            catch (Exception e)
            {
                throw new Exception("�Y�ώ��ԃ���e");
            }

            // [03]==================================================
            // ����SQL INSERT
            int myInsert = 0;
            try
            {
                myInsert = insertCommand.ExecuteNonQuery();
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

            // �鿴�Ƿ�ɹ��Ը�׃result׃����ֵ
            if (myInsert > 0)
            {
                result = true;
            }

            // [04]==================================================
            // ���ؽY��
            return result;

        }

        // �h���͆T
        /**
         * @param  String  firstName
         * @param  String  lastName
         * @return Boolean
         */
        public Boolean deleteEmployeesInfo(String firstName,
                                           String lastName)
        {
            Boolean result = false;

            // [01]==================================================
            // ����һ��SQL DELETE�Z��
            String sql = "DELETE FROM employees WHERE firstName = '" +
                          firstName + "' AND lastName = '" + lastName + "'";


            // [02]==================================================
            // ����һ���µ��B��
            SqlConnection oConn = new SqlConnection();
            oConn.ConnectionString = "Server = " + dbhost +
                                    ";Initial Catalog = " + dbname +
                                    ";UID=" + dbuser +
                                    ";PWD=" + dbpass + ";";
            SqlCommand deleteCommand = oConn.CreateCommand();
            deleteCommand.CommandText = sql;
            try
            {
                oConn.Open();
            }
            catch (Exception e)
            {
                throw new Exception("�Y�ώ��ԃ���e");
            }

            // [03]==================================================
            // ����SQL DELETE
            int myUpdate = 0;
            try
            {
                myUpdate = deleteCommand.ExecuteNonQuery();
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
            // �鿴�Ƿ�ɹ��Ը�׃result׃����ֵ
            if (myUpdate > 0)
            {
                result = true;
            }

            // [04]==================================================
            // ���ؽY��
            return result;
        }
    }
}
