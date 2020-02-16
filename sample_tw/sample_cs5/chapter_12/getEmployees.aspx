<%@ Page language="c#"%>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Register TagPrefix="amf" Namespace="FlashGateway" Assembly="flashgateway" %>

<amf:Flash ID="FlashID" runat="server" />
<script runat="server">
private void Page_Load(object sender, System.EventArgs e)
{
   String dbhost = "localhost";
   String dbport = "1433";
   String dbname = "myDatabase";
   String dbuser = "sa";
   String dbpass = "verysecret";
		
   // ��FlashID.Params�Ы@ȡ���f��ą�����
   // �@��һ�����FIList��������
   // ��õľ���ArrayList
   IList args = FlashID.Params;
   String firstName = "";
   String lastName = "";
   double age = 0;

   if (args == null) {
      throw new Exception("�o���@ȡ������");
   } else {
	  // ����D�Q
	  ArrayList list = (ArrayList)args;
	  // �z�酢���Ĕ���
	  if (list.Count == 3) {
	     firstName = list[0].ToString();
		 lastName = list[1].ToString();
		 age = Convert.ToDouble(list[2]);
	  }
   }
   
   // �������������ԃ�ִ�
   String sql;
   if (firstName.Equals("") && lastName.Equals("") && age==0){    
      sql = "SELECT firstName,lastName,EmpType,age " +
		    " FROM employees";
   } else {
      sql = "SELECT firstName,lastName,EmpType,age " +
		    " FROM employees " + " WHERE firstName LIKE '" +
            firstName + "' AND lastName LIKE '" + lastName +
            "' AND age >= " + age;
   }
   
   
   DataTable employeesDT = new DataTable();
   // ����һ���µ��B�ӣ���ԃ�Y��
   SqlConnection oConn = new SqlConnection();
   oConn.ConnectionString = "Server = " + dbhost +
			      ";Initial Catalog = " + dbname  +
			      ";UID=" + dbuser +
			      ";PWD=" + dbpass +  ";";
   try {
	  // ����һ��DataAdapter���K�O�ò�ԃ����
      SqlDataAdapter adapter = new SqlDataAdapter();
      adapter.SelectCommand = new SqlCommand(sql, oConn);
      // ʹ��DataAdapter���DataTable
      adapter.Fill(employeesDT);
   } catch (Exception ex) {
	   throw new Exception("�Y�ώ��ԃ���e");
   } finally {
	   try {
		  // �@ʽ���P�]����
          oConn.Close();
       } catch (Exception ex2) {}       
   }   

   // ���ؽY��
    FlashID.DataSource = employeesDT;
    FlashID.DataBind();
}
</script>
