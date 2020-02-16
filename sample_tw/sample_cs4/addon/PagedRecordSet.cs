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


        // 該方法用於以增量方式獲取資料庫雇員記錄
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

            // 獲取偏移量和每頁的記錄數
            int offset = PagingContext.Current.Offset;
            int limit = PagingContext.Current.Limit;

            // [01]==================================================
            // 根據參數構造查詢字串
            String sql;
            if (firstName == null && lastName == null && age == 0)
            {
                // 構造SELECT TOP語句，返回所有記錄
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
                // 返回符合條件的記錄
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

            // [03]==================================================
            // 返回結果
            return employeesDT;
        }


        // 所有需要返回分頁記錄集的方法都要定義一個類似的方法：
        // 方法名加上Count（getEmployeesInfo+Count），
        // FluorineFx會自動調用該方法返回記錄集總數
        // 該方法的參數參數必須與getEmployeesInfo方法相同
        /**
         * @return int
         */
        public int getEmployeesInfoCount(String firstName,
                                            String lastName,
                                            int age)
        {
            object result = 0;

            // [01]==================================================
            // 根據參數構造查詢字串
            String sql;
            if (firstName.Equals("") && lastName.Equals("") && age == 0)
            {
                // 注意這裏，如果傳入的是類如空值的參數，那麼就返回所有記錄
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
            // 創建一個新的連接，查詢資料庫
            SqlConnection oConn = new SqlConnection();
            oConn.ConnectionString = "Server = " + dbhost +
                                    ";Initial Catalog = " + dbname +
                                    ";UID=" + dbuser +
                                    ";PWD=" + dbpass + ";";
            try
            {
                // 執行查詢命令，獲取記錄總數
                oConn.Open();
                SqlCommand oCmd = oConn.CreateCommand();
                oCmd.CommandText = sql;
                result = oCmd.ExecuteScalar();
            }
            catch (Exception e)
            {
                throw new Exception("資料庫查詢出錯" + e.Message + sql);
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

            // [03]==================================================
            // 返回可用的資料記錄總數
            return System.Convert.ToInt32(result);
        }
    }
}
