<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Dashboards.aspx.cs" Inherits="Distributr.HQ.Web.DistributrReports.Dashboards" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
	Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id ="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager2" runat="server"></asp:ScriptManager>
        <div>
               <rsweb:ReportViewer ID="ReportViewer2" runat="server" ProcessingMode="Remote" Width="100%"  Height="1000px"></rsweb:ReportViewer>
        </div>
    </form>

</body>
</html>

