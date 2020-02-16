<%@ Page Language="C#" ContentType="image/png"%>
<%@ Import Namespace="System.Drawing" %>
<%@ Import Namespace="System.Drawing.Imaging" %>
<%@ Import Namespace="System.IO" %>

<script language="C#" runat="server">
// �o�������@���S�C�ɫ
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
// �O����治����
Response.AppendHeader("Cache-Control", "no-cache"); 
Response.AppendHeader("Pragma", "no-cache");
Response.AppendHeader("Expires", "Thu, 01 Jan 1970 00:00:00 GMT"); 
Response.AppendHeader("Last-Modified", DateTime.Now.ToString("r"));

// ��ʼ����C�a
String checkingCode="";
if(Session.Contents["checkingCode"]!=null){
  Session.Contents.Remove("checkingCode");
}

// ���x�DƬ�����С���K��ӛ���w�Є����D��
int picWidth = 80;
int picHeight = 22;
Bitmap image = new Bitmap(picWidth, picHeight);

// �@ȡ�D��������
Graphics g = Graphics.FromImage(image);

// �����DƬ߅��
Pen blackPen = new Pen(Color.Black, 1);
g.DrawRectangle(blackPen, 0,0, picWidth-1, picHeight-1);

// �O������ɫ
SolidBrush whiteBrush = new SolidBrush(Color.White);
g.FillRectangle(whiteBrush, 1, 1, picWidth-2, picHeight-2);

// �����S�C�s�c��ޒȦ�Δ�Խ�࣬�s�cԽ��
Random random = new Random();
Pen randPen;
for (int i=0;i<500;i++){
   randPen = getRandPen(255, 255, 255);
   int x = random.Next(picWidth-3)+1;
   int y = random.Next(picHeight-3)+1;
   g.DrawLine(randPen, x, y, x+1, y+1);
}

// ���x���õ���Ԫ����Ԫ����
String ConstCode = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
int codeAmount = ConstCode.Length;

// �O�����w���ɫ
Font font = new Font("Times New Roman", 16, FontStyle.Bold);
SolidBrush fontBrush = new SolidBrush(Color.FromArgb(70, 70, 70));

// ����4���S�C��Ԫ
for (int i=0; i<=3; i++){
   int index = random.Next(codeAmount-1);
   String rand = ConstCode.Substring(index, 1);
   checkingCode+=rand;
   // ���x��Ԫλ��
   PointF pointF = new PointF(16*i+5, random.Next(i));
   // ����Ԫ�����DƬ��
   g.DrawString(rand, font, fontBrush, pointF);
}

// ����Ԫ��C�a����Session���Դ�δ��z�
Session.Contents["checkingCode"] = checkingCode;

// ����D��һ���Y������
MemoryStream stream = new MemoryStream();
image.Save(stream, ImageFormat.Png);

// ጷŴˈD�ε��������Լ���ʹ�õ�����ϵ�y�YԴ
g.Dispose();
image.Dispose();

// ݔ���D�����
Response.ClearContent();
Response.ContentType = "image/png";
Response.BinaryWrite(stream.ToArray());
%>
