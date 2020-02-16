<%@ Page language="c#"%>
<%@ Register TagPrefix="amf" Namespace="FlashGateway"
                Assembly="flashgateway" %>
<amf:Flash ID="FlashID" runat="server" />
<%
String[] stringArray={"àË£¡AMF-RPC for .NET"};
FlashID.DataSource = (Object) stringArray;
FlashID.DataBind();
%>
