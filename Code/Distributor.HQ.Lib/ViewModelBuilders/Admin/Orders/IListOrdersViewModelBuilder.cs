using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.ViewModels.Admin.Orders;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Utility.MasterData;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Orders
{
    public interface IListOrdersViewModelBuilder
    {
        Dictionary<Guid, string> GetDistributor();

        //IList<OrderViewModel>GetAllList(int skip,int take);
        IList<OrderViewModel> GetAllList();
        // List<OrderViewModel> GetAllListByDate(DateTime startDate, DateTime endTime);
        List<OrderViewModel> GetAllOrdersByDate(string startDate, string endDate);
        OrderViewModel GetByDist(Guid distributor, int CurrentPage, int PageSize, bool inactive = false);
        //IList<OrderViewModel> GetByDist(int distributor);
        List<OrderViewModel> GetByDistDate(Guid distributor, string startDate, string endDate);
        List<OrderViewModel> GetSalesByDistDate(Guid distributor, string startDate, string endDate);
        List<OrderViewModel> GetAllSales(string startDate, string endDate);
        List<OrderViewModel> GetPendingOrders(Guid distributor, string startDate, string endDate);
        //List<OrderViewModel> GetAllPendingOrders();
        List<OrderViewModel> GetPendingDeliveries(Guid distributor, string startDate, string endDate);
        // List<OrderViewModel> GetAllClosedPurchaseOrders();
        //List<OrderViewModel> GetAllApprovedOrders();
        List<OrderViewModel> SearchOrders(string orderRef);

        List<OrderViewModel> GetUnImported();

        OrderViewModel GetOrdersSkipAndTake(int CurrentPage, int PageSize, bool inactive = false);
        OrderViewModel GetAllPendingOrders(int CurrentPage, int PageSize);
        OrderViewModel GetAllClosedPurchaseOrders(int CurrentPage, int PageSize);
        OrderViewModel GetAllApprovedOrders(int CurrentPage, int PageSize);
        OrderViewModel SearchPOrders(string orderRef, int CurrentPage, int PageSize);
        OrderViewModel FilterOrdersByDate(string startDate, string endDate, int CurrentPage, int PageSize);
        OrderViewModel FilterOrdersByDateDistributor(Guid distributor, string startDate, string endDate, int CurrentPage, int PageSize);

        List<OrderViewModel> GetCountPendingOrders();
        void SaveOrder(OrderViewModel ovm);
        // List<Order> ProcessOrder(List<OrderViewModel> ovm);
        void ProcessOrder(List<OrderViewModel> ovm);
        void SubmitOrder(Order order);

        QueryResult<OrderViewModel> Query(QueryOrders query);
    }
}
