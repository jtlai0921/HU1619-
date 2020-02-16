<%@ Page Language="C#" ContentType="text/html" ResponseEncoding="utf-8" %>
<%
string str = "";
foreach (string strKey in Request.ServerVariables) {
    str = str + "<b>" + strKey + "</b>:" 
          + Request.ServerVariables[strKey] + "<br />";
}
Response.Write(str);
%>
