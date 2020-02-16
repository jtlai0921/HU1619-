<%@ Page language="c#"%>
<%@ Import Namespace="System.Collections" %>
<%@ Register TagPrefix="amf" Namespace="FlashGateway"
                Assembly="flashgateway" %>
<amf:Flash ID="FlashID" runat="server" />
<script runat="server">
private void Page_Load(object sender, System.EventArgs e)
{
   // 從FlashID.Params中獲取傳遞來的參數，
   // 這是一個實現IList介面的物件
   // 最常用的就是ArrayList
   IList args = FlashID.Params;
   string arg = "";
   if (args == null) {
      throw new Exception("無法獲取參數！");
   } else {
	  // 類型轉換
	  ArrayList list = (ArrayList)args;
	  // 檢查參數的數量
	  if (list.Count > 0) {
	     arg = list[0].ToString();
	  }
   }
   // 將結果保存在Flash.Result
   FlashID.Result = "嗨！AMF-RPC for .NET" + arg;
}
</script>
