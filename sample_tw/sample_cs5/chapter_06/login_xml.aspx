<%@ Page Language="C#" ResponseEncoding="utf-8"%>
<%-- 導入所需要的類 --%>
<%@ Import Namespace="System" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="System.Xml" %>
<%@ Import Namespace="System.IO" %>
<%
String userPwd = ""; 
String userName = "";
try{
  // 接收發送過來的XML資料，位於HTTP報頭中
  Stream xmlPost = Request.InputStream;
  // 將它讀進一個DataSet
  DataSet myDataSet = new DataSet();
  myDataSet.ReadXml(xmlPost);
  // 將DataSet轉成一個XML字串
  String xmlString = myDataSet.GetXml();
  // 構建一XmlDocument物件，準備解析發送過來的XML資料  
  XmlDocument xmldoc = new XmlDocument();
  xmldoc.LoadXml(xmlString);
  XmlElement root = xmldoc.DocumentElement;
  XmlNode node = root.FirstChild;
  XmlAttributeCollection attrColl = node.Attributes;
  // 獲取用戶名和密碼
  userPwd = attrColl["password"].Value;
  userName = attrColl["username"].Value;
} catch (Exception e) {
  Response.Write("<loginreply status='failed'/>");
  Response.End();
}

// 創建一個新的資料庫連接
SqlConnection oConn  = new SqlConnection();
oConn.ConnectionString = 
       "Server=localhost;Initial Catalog =users;UID=sa;PWD=verysecret;";
// 創建一個SqlCommand物件用來將查詢和命令發送給資料庫	   
SqlCommand oCmd = oConn.CreateCommand();
oCmd.CommandText = "SELECT * FROM main WHERE username = '" +
               userName + "' AND userpwd= '" + userPwd + "'";
oConn.Open(); 
%>
<%
// 將查詢結果賦給一個OleDbDataReader
// 使用myReader.Read()方法看是否存在記錄
// 注意返回的資料必須是XML格式
SqlDataReader oReader = oCmd.ExecuteReader(); 
if (oReader.Read()) {
 	 Response.Write("<loginreply status='ok'/>");
     Response.End();
} else {
     Response.Write("<loginreply status='failed'/>");
     Response.End();
}
// 最後，顯式的關閉物件
oReader.Close();
oConn.Close();
%>
