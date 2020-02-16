<%@ Page Language="C#" ContentType="text/html" ResponseEncoding="utf-8" %>
<%-- 導入ADO.NET所需要的類 --%>
<%@ Import Namespace="System.Data.SqlClient" %>
<%
// 創建一個新的資料庫連接
SqlConnection oConn  = new SqlConnection();
oConn.ConnectionString = 
       "Server = localhost;Initial Catalog = myDatabase;UID=sa;PWD=verysecret;";
// 創建一個SqlCommand物件用來將查詢和命令發送給資料庫	   
SqlCommand oCmd = oConn.CreateCommand();
oCmd.CommandText = "SELECT *  FROM employees";
oConn.Open();
%>
<%
// 構造一個XML字串
String resultXML = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
resultXML = resultXML + "<employees>";
  
// 使用ExecuteReader方法執行查詢，並將查詢結果賦給一個SqlDataReader
// 使用oReader.Read()方法看是否存在記錄
// 如果存在記錄，就迴圈寫出資料記錄，構造XML字串
SqlDataReader oReader = oCmd.ExecuteReader(); 
   
while (oReader.Read()) {
  resultXML = resultXML + "<employee>";
  resultXML = resultXML + "<EmpType>" + 
              oReader.GetValue(oReader.GetOrdinal("EmpType")) + 
	 	 	  "</EmpType>";
  resultXML = resultXML + "<firstName>" + 
	          oReader.GetValue(oReader.GetOrdinal("firstName"))+ 
	 	 	  "</firstName>";
  resultXML = resultXML + "<lastName>" + 
	          oReader.GetValue(oReader.GetOrdinal("lastName")) + 
	 	 	  "</lastName>";
  resultXML = resultXML + "<age>" + 
	          oReader.GetValue(oReader.GetOrdinal("age")) + 
			  "</age>";
  resultXML = resultXML + "</employee>";
}  
resultXML = resultXML + "</employees>";
  
// 最後，顯式的關閉物件
oReader.Close();
oConn.Close();
  
// 返回回應
Response.Write(resultXML);
%>
