<%@ Page language="c#"%>
<%@ Register TagPrefix="amf" Namespace="FlashGateway"
                Assembly="flashgateway" %>
<amf:Flash ID="FlashID" runat="server" />
<%
String[] stringArray={"�ˣ�AMF-RPC for .NET"};
FlashID.DataSource = (Object) stringArray;
FlashID.DataBind();
%>
