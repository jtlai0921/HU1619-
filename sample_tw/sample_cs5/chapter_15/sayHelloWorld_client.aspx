<%@ Page Language="c#" ResponseEncoding="utf-8"%>
<%@ Import Namespace="System" %>
<%@ Import Namespace="CookComputing.XmlRpc" %>
<script runat="server">
// 創建一個介面，該介面用來創建代理
// 注意XmlRpcUrl屬性的設置
[XmlRpcUrl("http://localhost/xmlrpc/HelloWorld.aspx")]
public interface HelloWorldItf
{
    [XmlRpcMethod("flashRemoting.xmlrpc.HelloWorld.sayHelloWorld")]
    string sayHelloWorld(string str);
}
</script>
<%
// 根據介面創建代理
{
   HelloWorldItf proxy = default(HelloWorldItf);
   proxy = (HelloWorldItf)XmlRpcProxyGen.Create(typeof(HelloWorldItf));
   // 使用代理調用方法，返回結果
   string ret = null;
   ret = proxy.sayHelloWorld("嗨!");
   Response.Write(ret);
}
%>
