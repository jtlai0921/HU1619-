<%@ Page Language="C#" ContentType="text/html" ResponseEncoding="utf-8" %>
<%
String clientData, returnToFlash;
// 獲取發送的資料
clientData = Request.QueryString["clientData"];
returnToFlash = "replyData=服務端返回：" + clientData;
// 將回應發回用戶端
Response.Write(returnToFlash);
%>
