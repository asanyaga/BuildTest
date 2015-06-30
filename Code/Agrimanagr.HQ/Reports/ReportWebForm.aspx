<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportWebForm.aspx.cs" Inherits="Agrimanagr.HQ.Reports.ReportWebForm" %>

<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        
        <rsweb:ReportViewer Height="1500px" Width="100%" ID="ReportViewer1" runat="server"></rsweb:ReportViewer>
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
       
    </div>
    </form>
</body>
</html>
