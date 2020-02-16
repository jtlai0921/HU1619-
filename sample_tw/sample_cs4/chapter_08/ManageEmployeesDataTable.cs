using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using FluorineFx;

namespace org.zhangyafei
{
    [RemotingService("http://www.zhang-yafei.com")]
    public class ManageEmployeesDataTable
    {
        private String dbhost = "localhost";
        private String dbport = "1433";
        private String dbname = "myDatabase";
        private String dbuser = "sa";
        private String dbpass = "verysecret";


        // 該方法用於獲取資料庫雇員記錄
        /**
         * @param  String  firstName
         * @param  String  lastName
         * @param  int     age
         * @return DataTable
         */
        public DataTable getEmployeesInfo(String firstName,
                                            String lastName,
                                            int age)
        {
            DataTable employeesDT = new DataTable();

            // [01]==================================================
            // 根據參數構造查詢字串
            String sql;
            if (firstName.Equals("") && lastName.Equals("") && age == 0)
            {
                // 注意這裏，如果傳入的是類如空值的參數，那麼就返回所有記錄
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
            // 創建一個新的連接，查詢資料
            SqlConnection oConn = new SqlConnection();
            oConn.ConnectionString = "Server = " + dbhost +
                                    ";Initial Catalog = " + dbname +
                                    ";UID=" + dbuser +
                                    ";PWD=" + dbpass + ";";
            try
            {
                // 創建一個DataAdapter，並設置查詢命令
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = new SqlCommand(sql, oConn);
                // 使用DataAdapter填充DataTable
                adapter.Fill(employeesDT);
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

            // [04]==================================================
            // 返回結果
            return employeesDT;
        }

        // 增加雇員
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
            // 創建一個SQL INSERT語句
            String sql = "INSERT INTO employees " +
                           " (firstName,lastName,EmpType,age) VALUES ('" +
                           firstName + "','" + lastName + "','" + EmpType +
                           "'," + age + ")";
            // [02]==================================================
            // 創建一個新的連接
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
                throw new Exception("資料庫查詢出錯");
            }

            // [03]==================================================
            // 執行SQL INSERT
            int myInsert = 0;
            try
            {
                myInsert = insertCommand.ExecuteNonQuery();
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
            if (myInsert > 0)
            {
                result = true;
            }

            // [04]==================================================
            // 返回結果
            return result;

        }

        // 刪除雇員
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
            // 創建一個SQL DELETE語句
            String sql = "DELETE FROM employees WHERE firstName = '" +
                          firstName + "' AND lastName = '" + lastName + "'";


            // [02]==================================================
            // 創建一個新的連接
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
                throw new Exception("資料庫查詢出錯");
            }

            // [03]==================================================
            // 執行SQL DELETE
            int myUpdate = 0;
            try
            {
                myUpdate = deleteCommand.ExecuteNonQuery();
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
            if (myUpdate > 0)
            {
                result = true;
            }

            // [04]==================================================
            // 返回結果
            return result;
        }
    }
}
