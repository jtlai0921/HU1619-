<%@ Page Language="c#" ResponseEncoding="utf-8"%>
<%@ Import Namespace="System" %>
<%@ Import Namespace="CookComputing.XmlRpc" %>
<script language="cs" runat="server">
// 這裏也要定義一個結構
public struct SumAndDiffValue
{
    public int sum;
    public int difference;
}
[XmlRpcUrl("http://localhost/xmlrpc/SumAndDiff.aspx")]
public interface SumAndDiffItf
{
    [XmlRpcMethod("flashRemoting.xmlrpc.SumAndDiff.sumAndDifference")]
    SumAndDiffValue SumAndDifference(int x, int y);
}
</script>
<%
SumAndDiffItf proxy = default(SumAndDiffItf); 
proxy = (SumAndDiffItf)XmlRpcProxyGen.Create(typeof(SumAndDiffItf)); 

SumAndDiffValue ret = default(SumAndDiffValue); 
ret = proxy.SumAndDifference(6, 3); 
Response.Write("6 + 3 =" + ret.sum + "<br />"); 
Response.Write("6 - 3 =" + ret.difference + "<br />"); 
%>
