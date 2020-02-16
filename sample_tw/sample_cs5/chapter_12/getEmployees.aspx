<%@ Page language="c#"%>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Register TagPrefix="amf" Namespace="FlashGateway" Assembly="flashgateway" %>

<amf:Flash ID="FlashID" runat="server" />
<script runat="server">
private void Page_Load(object sender, System.EventArgs e)
{
   String dbhost = "localhost";
   String dbport = "1433";
   String dbname = "myDatabase";
   String dbuser = "sa";
   String dbpass = "verysecret";
		
   // 從FlashID.Params中獲取傳遞來的參數，
   // 這是一個實現IList介面的物件
   // 最常用的就是ArrayList
   IList args = FlashID.Params;
   String firstName = "";
   String lastName = "";
   double age = 0;

   if (args == null) {
      throw new Exception("無法獲取參數！");
   } else {
	  // 類型轉換
	  ArrayList list = (ArrayList)args;
	  // 檢查參數的數量
	  if (list.Count == 3) {
	     firstName = list[0].ToString();
		 lastName = list[1].ToString();
		 age = Convert.ToDouble(list[2]);
	  }
   }
   
   // 根據參數構造查詢字串
   String sql;
   if (firstName.Equals("") && lastName.Equals("") && age==0){    
      sql = "SELECT firstName,lastName,EmpType,age " +
		    " FROM employees";
   } else {
      sql = "SELECT firstName,lastName,EmpType,age " +
		    " FROM employees " + " WHERE firstName LIKE '" +
            firstName + "' AND lastName LIKE '" + lastName +
            "' AND age >= " + age;
   }
   
   
   DataTable employeesDT = new DataTable();
   // 創建一個新的連接，查詢資料
   SqlConnection oConn = new SqlConnection();
   oConn.ConnectionString = "Server = " + dbhost +
			      ";Initial Catalog = " + dbname  +
			      ";UID=" + dbuser +
			      ";PWD=" + dbpass +  ";";
   try {
	  // 創建一個DataAdapter，並設置查詢命令
      SqlDataAdapter adapter = new SqlDataAdapter();
      adapter.SelectCommand = new SqlCommand(sql, oConn);
      // 使用DataAdapter填充DataTable
      adapter.Fill(employeesDT);
   } catch (Exception ex) {
	   throw new Exception("資料庫查詢出錯");
   } finally {
	   try {
		  // 顯式的關閉對象
          oConn.Close();
       } catch (Exception ex2) {}       
   }   

   // 返回結果
    FlashID.DataSource = employeesDT;
    FlashID.DataBind();
}
</script>
