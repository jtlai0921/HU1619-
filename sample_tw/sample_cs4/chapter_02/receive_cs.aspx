<%@ Page Language="C#" ResponseEncoding="big5" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"><html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>HTML��������</title>
<meta http-equiv="Content-Type" content="text/html; charset=big5" />
</head>

<body>

<%
String theData = Request.Form["textField"];
Response.Write("textField=" + theData);
%>

</body>
</html>
