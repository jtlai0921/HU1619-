<%@ Page Language="C#" EnableSessionState="False" ResponseEncoding="utf-8"%>
<%
// �xȡ����Cookie
HttpCookieCollection cookieColl = Request.Cookies;
// �@ȡ��������VisitorCount��Cookie
HttpCookie oCookie = cookieColl["VisitorCount"];
// �z��ԓCookie�Ƿ���ڣ���������ھ��½�һ��
// Ҳ���ǵ�һ���L��
if (oCookie == null) {
    HttpCookie newCookie = new HttpCookie("VisitorCount", "1");
    newCookie.Expires = DateTime.Now.AddYears(1);
    Response.Cookies.Add(newCookie);
} else {
    // ������ڣ��͸���Cookieֵ
    oCookie.Value = "" + (Convert.ToInt32(oCookie.Value) + 1);
    Response.Cookies.Set(oCookie);
}
%><b>�gӭ���<font color="#FF0000"><%= cookieColl["VisitorCount"].Value %></font>�L��Cookie</b>
