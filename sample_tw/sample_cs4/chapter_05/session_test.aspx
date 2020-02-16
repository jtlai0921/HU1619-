<%@ Page Language="C#" EnableSessionState="true" %>
<%
// 檢查是否是新會話
if (!Session.IsNewSession){
  // 如果不是新會話
  // 就檢查是否存在指定Session變數
  if (Session["VisitorCount"] != null) {
     Session["VisitorCount"] = (int)Session["VisitorCount"] + 1;
  } else {
     Session["VisitorCount"] = 1;
     Session.Timeout = 30;
  }
}else {
  // 如果是新會話
  Session["VisitorCount"] = 1;
  Session.Timeout = 30;
}
%>
<b>歡迎用戶第<font color="#FF0000">
<%= Session["VisitorCount"] %> </font>訪問Session
</b>
