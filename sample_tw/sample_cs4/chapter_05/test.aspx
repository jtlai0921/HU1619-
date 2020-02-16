<%@ Page Language="C#" ResponseEncoding="utf-8"%>
<%
string CookieEnabled;
CookieEnabled = Request.QueryString["CookieEnabled"];
if (CookieEnabled == null) {
    // 不存在查詢變數，也就是首次訪問，創建一個Cookie做試探
    Response.Cookies["TestCookie"].Value = "ok";
    Response.Cookies["TestCookie"].Expires =
                                           DateTime.Now.AddMinutes(1);
    Response.Redirect("Test2.aspx");
} else { 
    HttpCookie TestCookie;
    TestCookie = Request.Cookies["TestCookie"];
    // 檢查是否存在Cookie
    if (TestCookie != null){
       if (TestCookie.Value == "ok" && CookieEnabled == "true"){
	      // 重複檢測一下TestCookie是否存在，以防有人人為構造查詢變數搞破壞
          // 兩個頁面都說支援Cookie，那麼就是支持Cookie
          Response.Write("支持cookie");
       } else {
          // 不支持Cookie 
          Response.Write("不支持cookie");
       }
    } else {
       // 不支持Cookie 
       Response.Write("不支持cookie");
    }
}
%>
