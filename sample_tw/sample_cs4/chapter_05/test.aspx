<%@ Page Language="C#" ResponseEncoding="utf-8"%>
<%
string CookieEnabled;
CookieEnabled = Request.QueryString["CookieEnabled"];
if (CookieEnabled == null) {
    // �����ڲ�ԃ׃����Ҳ�����״��L��������һ��Cookie��ԇ̽
    Response.Cookies["TestCookie"].Value = "ok";
    Response.Cookies["TestCookie"].Expires =
                                           DateTime.Now.AddMinutes(1);
    Response.Redirect("Test2.aspx");
} else { 
    HttpCookie TestCookie;
    TestCookie = Request.Cookies["TestCookie"];
    // �z���Ƿ����Cookie
    if (TestCookie != null){
       if (TestCookie.Value == "ok" && CookieEnabled == "true"){
	      // ���}�z�yһ��TestCookie�Ƿ���ڣ��Է������˞阋���ԃ׃�����Ɖ�
          // �ɂ���涼�f֧ԮCookie�����N����֧��Cookie
          Response.Write("֧��cookie");
       } else {
          // ��֧��Cookie 
          Response.Write("��֧��cookie");
       }
    } else {
       // ��֧��Cookie 
       Response.Write("��֧��cookie");
    }
}
%>
