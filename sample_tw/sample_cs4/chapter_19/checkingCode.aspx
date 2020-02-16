<%@ Page Language="C#" ContentType="image/png"%>
<%@ Import Namespace="System.Drawing" %>
<%@ Import Namespace="System.Drawing.Imaging" %>
<%@ Import Namespace="System.IO" %>

<script language="C#" runat="server">
// 給定範圍獲得隨機顏色
private Pen getRandPen(int cr, int cg, int cb){
    Random random = new Random();
    int r= random.Next(cr);
    int g= random.Next(cg);
    int b= random.Next(cb);
	Pen pen = new Pen(Color.FromArgb(r, g, b), 1);
    return pen;
}
</script>
<%
// 設置頁面不緩存
Response.AppendHeader("Cache-Control", "no-cache"); 
Response.AppendHeader("Pragma", "no-cache");
Response.AppendHeader("Expires", "Thu, 01 Jan 1970 00:00:00 GMT"); 
Response.AppendHeader("Last-Modified", DateTime.Now.ToString("r"));

// 初始化驗證碼
String checkingCode="";
if(Session.Contents["checkingCode"]!=null){
  Session.Contents.Remove("checkingCode");
}

// 定義圖片幅面大小，並在記憶體中創建圖像
int picWidth = 80;
int picHeight = 22;
Bitmap image = new Bitmap(picWidth, picHeight);

// 獲取圖形上下文
Graphics g = Graphics.FromImage(image);

// 創建圖片邊框
Pen blackPen = new Pen(Color.Black, 1);
g.DrawRectangle(blackPen, 0,0, picWidth-1, picHeight-1);

// 設定背景色
SolidBrush whiteBrush = new SolidBrush(Color.White);
g.FillRectangle(whiteBrush, 1, 1, picWidth-2, picHeight-2);

// 創建隨機雜點，迴圈次數越多，雜點越密
Random random = new Random();
Pen randPen;
for (int i=0;i<500;i++){
   randPen = getRandPen(255, 255, 255);
   int x = random.Next(picWidth-3)+1;
   int y = random.Next(picHeight-3)+1;
   g.DrawLine(randPen, x, y, x+1, y+1);
}

// 定義可用的字元和字元數量
String ConstCode = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
int codeAmount = ConstCode.Length;

// 設定字體和顏色
Font font = new Font("Times New Roman", 16, FontStyle.Bold);
SolidBrush fontBrush = new SolidBrush(Color.FromArgb(70, 70, 70));

// 創建4個隨機字元
for (int i=0; i<=3; i++){
   int index = random.Next(codeAmount-1);
   String rand = ConstCode.Substring(index, 1);
   checkingCode+=rand;
   // 定義字元位置
   PointF pointF = new PointF(16*i+5, random.Next(i));
   // 將字元畫到圖片中
   g.DrawString(rand, font, fontBrush, pointF);
}

// 將字元驗證碼存入Session，以待未來檢驗
Session.Contents["checkingCode"] = checkingCode;

// 保存圖像到一個資料流程
MemoryStream stream = new MemoryStream();
image.Save(stream, ImageFormat.Png);

// 釋放此圖形的上下文以及它使用的所有系統資源
g.Dispose();
image.Dispose();

// 輸出圖像到頁面
Response.ClearContent();
Response.ContentType = "image/png";
Response.BinaryWrite(stream.ToArray());
%>
