<%@ Page language="c#"%>
<%@ Import Namespace="System.Collections" %>
<%@ Register TagPrefix="amf" Namespace="FlashGateway"
                Assembly="flashgateway" %>
<amf:Flash ID="FlashID" runat="server" />
<script runat="server">
private void Page_Load(object sender, System.EventArgs e)
{
   // ��FlashID.Params�Ы@ȡ���f��ą�����
   // �@��һ�����FIList��������
   // ��õľ���ArrayList
   IList args = FlashID.Params;
   string arg = "";
   if (args == null) {
      throw new Exception("�o���@ȡ������");
   } else {
	  // ����D�Q
	  ArrayList list = (ArrayList)args;
	  // �z�酢���Ĕ���
	  if (list.Count > 0) {
	     arg = list[0].ToString();
	  }
   }
   // ���Y��������Flash.Result
   FlashID.Result = "�ˣ�AMF-RPC for .NET" + arg;
}
</script>
