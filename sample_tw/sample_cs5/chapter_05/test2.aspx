<%@ Page Language="C#" ResponseEncoding="utf-8"%>
<%
HttpCookie TestCookie;
TestCookie = Request.Cookies["TestCookie"];
if (TestCookie == null){
    // ����]�аl�FCookie��Ҳ�ض����ԭ��棬�����f����ֵfalse
    Response.Redirect("Test.aspx?CookieEnabled=false");
} else if (TestCookie.Value == "ok"){
    // ����l�FCookie�����ض����ԭ��棬�K���f����ֵtrue
    Response.Redirect("Test.aspx?CookieEnabled=true");
}
%>
