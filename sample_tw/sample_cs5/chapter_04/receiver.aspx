<%@ Page Language="C#" ContentType="text/html" ResponseEncoding="utf-8" %>
<%
// 如果是POST方法就使用Request.Form
if (Request.RequestType == "POST"){
  foreach (string strItem in Request.Form) {
    Response.Write("變數名：" + strItem + "===");
    Response.Write("變數值：" + Request.Form[strItem] + "<br />");
  }
// 如果是GET方法就使用Request.QueryString
} else if (Request.RequestType == "GET"){
  foreach (string strItem in Request.QueryString) {
    Response.Write("變數名：" + strItem + "===");
    Response.Write("變數值：" + Request.QueryString[strItem] + "<br />");
  }
}
%>
