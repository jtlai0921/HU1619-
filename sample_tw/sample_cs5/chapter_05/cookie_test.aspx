<%@ Page Language="C#" EnableSessionState="False" ResponseEncoding="utf-8"%>
<%
// 讀取所有Cookie
HttpCookieCollection cookieColl = Request.Cookies;
// 獲取其中名為VisitorCount的Cookie
HttpCookie oCookie = cookieColl["VisitorCount"];
// 檢查該Cookie是否存在，如果不存在就新建一個
// 也就是第一次訪問
if (oCookie == null) {
    HttpCookie newCookie = new HttpCookie("VisitorCount", "1");
    newCookie.Expires = DateTime.Now.AddYears(1);
    Response.Cookies.Add(newCookie);
} else {
    // 如果存在，就更新Cookie值
    oCookie.Value = "" + (Convert.ToInt32(oCookie.Value) + 1);
    Response.Cookies.Set(oCookie);
}
%><b>歡迎你第<font color="#FF0000"><%= cookieColl["VisitorCount"].Value %></font>訪問Cookie</b>
