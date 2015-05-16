<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<Distributr.HQ.Lib.ViewModelBuilders.Admin.Orders.Impl.ListOrdersContainerViewModelBuilder>" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <title>Ord</title>
</head>
<body>
    <div>
        <% Html.RenderPartial("Ord", Model.orderPagedList); %>
    </div>
</body>
</html>
