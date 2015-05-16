<%@ Page Title="" Language="C#" MasterPageFile="~/rpt/Report.Master" AutoEventWireup="true" CodeBehind="ServiceProvider.aspx.cs" Inherits="PaymentGateway.WSApi.rpt.Admin.ServiceProvider" %>
<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<fieldset>
<legend>Report Filters</legend>
<table id="report-filter">
<tr>
<td>From</td>
<td><asp:TextBox ID="TextBoxFrom" runat="server"></asp:TextBox></td>
<td>To</td>
<td><asp:TextBox ID="TextBoxTo" runat="server"></asp:TextBox></td>

</tr>
 <tr>
 <td colspan="3"></td>
<td><asp:Button ID="ButtonView" runat="server" Text="View Report" 
        onclick="ButtonView_Click" /></td></tr>
</table>
    
</fieldset>


    <rsweb:ReportViewer ID="ReportViewer1" runat="server" Width="100%">
</rsweb:ReportViewer>
</asp:Content>
