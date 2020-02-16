<%@ WebService Language="C#" Class="HelloWorld" %>
using System;
using System.Web.Services;

[WebService(Namespace="org.zhangyafei.webservices")]
public class HelloWorld
{
    [WebMethod]
    public string sayHelloWorld(String param)
    {
        return ("àË£¡ASP.NET Web·þ„Õ£¬ß@ÊÇ‚÷ßfµÄ…¢”µ£º" + param);
    }
}
