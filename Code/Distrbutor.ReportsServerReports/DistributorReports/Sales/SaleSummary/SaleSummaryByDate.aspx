<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SaleSummaryByDate.aspx.cs" Inherits="Distrbutor.ReportsServerReports.DistributorReports.Sales.SaleSummary.SaleSummaryByDate" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" ProcessingMode="Remote" Width="100%">
<%--            <ServerReport ReportPath="/DistributrReports/Sales summary by date" ReportServerUrl="http://10.0.0.19/reportserver" />--%>
        </rsweb:ReportViewer>
    </div>
    </form>
</body>
</html>
