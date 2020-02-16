<%@ Page Language="C#" ResponseEncoding="utf-8"%>
<%
HttpCookie TestCookie;
TestCookie = Request.Cookies["TestCookie"];
if (TestCookie == null){
    // 如果沒有發現Cookie，也重定向回原頁面，但傳遞參數值false
    Response.Redirect("Test.aspx?CookieEnabled=false");
} else if (TestCookie.Value == "ok"){
    // 如果發現Cookie，就重定向回原頁面，並傳遞參數值true
    Response.Redirect("Test.aspx?CookieEnabled=true");
}
%>
