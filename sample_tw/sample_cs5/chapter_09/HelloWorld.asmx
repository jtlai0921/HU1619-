<%@ WebService Language="C#" Class="HelloWorld" %>
using System;
using System.Web.Services;

[WebService(Namespace="org.zhangyafei.webservices")]
public class HelloWorld
{
    [WebMethod]
    public string sayHelloWorld(String param)
    {
        return ("�ˣ�ASP.NET Web���գ��@�ǂ��f�ą�����" + param);
    }
}
