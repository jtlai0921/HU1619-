<%@ Page Language="C#" ResponseEncoding="utf-8"%>
<%-- ��������Ҫ��� --%>
<%@ Import Namespace="System" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="System.Xml" %>
<%@ Import Namespace="System.IO" %>
<%
String userPwd = ""; 
String userName = "";
try{
  // ���հl���^���XML�Y�ϣ�λ�HTTP���^��
  Stream xmlPost = Request.InputStream;
  // �����x�Mһ��DataSet
  DataSet myDataSet = new DataSet();
  myDataSet.ReadXml(xmlPost);
  // ��DataSet�D��һ��XML�ִ�
  String xmlString = myDataSet.GetXml();
  // ����һXmlDocument������ʂ�����l���^���XML�Y��  
  XmlDocument xmldoc = new XmlDocument();
  xmldoc.LoadXml(xmlString);
  XmlElement root = xmldoc.DocumentElement;
  XmlNode node = root.FirstChild;
  XmlAttributeCollection attrColl = node.Attributes;
  // �@ȡ�Ñ������ܴa
  userPwd = attrColl["password"].Value;
  userName = attrColl["username"].Value;
} catch (Exception e) {
  Response.Write("<loginreply status='failed'/>");
  Response.End();
}

// ����һ���µ��Y�ώ��B��
SqlConnection oConn  = new SqlConnection();
oConn.ConnectionString = 
       "Server=localhost;Initial Catalog =users;UID=sa;PWD=verysecret;";
// ����һ��SqlCommand����Á팢��ԃ������l�ͽo�Y�ώ�	   
SqlCommand oCmd = oConn.CreateCommand();
oCmd.CommandText = "SELECT * FROM main WHERE username = '" +
               userName + "' AND userpwd= '" + userPwd + "'";
oConn.Open(); 
%>
<%
// ����ԃ�Y���x�oһ��OleDbDataReader
// ʹ��myReader.Read()�������Ƿ����ӛ�
// ע�ⷵ�ص��Y�ϱ����XML��ʽ
SqlDataReader oReader = oCmd.ExecuteReader(); 
if (oReader.Read()) {
 	 Response.Write("<loginreply status='ok'/>");
     Response.End();
} else {
     Response.Write("<loginreply status='failed'/>");
     Response.End();
}
// ���ᣬ�@ʽ���P�]���
oReader.Close();
oConn.Close();
%>
