<%@ Page Language="C#" ContentType="text/html" ResponseEncoding="utf-8" %>
<%
// �����POST������ʹ��Request.Form
if (Request.RequestType == "POST"){
  foreach (string strItem in Request.Form) {
    Response.Write("׃������" + strItem + "===");
    Response.Write("׃��ֵ��" + Request.Form[strItem] + "<br />");
  }
// �����GET������ʹ��Request.QueryString
} else if (Request.RequestType == "GET"){
  foreach (string strItem in Request.QueryString) {
    Response.Write("׃������" + strItem + "===");
    Response.Write("׃��ֵ��" + Request.QueryString[strItem] + "<br />");
  }
}
%>
