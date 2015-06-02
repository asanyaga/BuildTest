using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;


namespace Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories
{
    public interface IOrderRepository : IDocumentRepository<Order>, IDocumentRepositorySaveable<Order>
    {
        List<Order> GetByIssuerCostCentre(int issuerCostCentreId, DocumentStatus? status, DateTime? fromDateIssued, DateTime? toDateIssued);
        List<Order> GetByRecipientCostCentre(Guid recipientCostCentreId, DocumentStatus? status, DateTime? fromDateIssued, DateTime? toDateIssued);
        List<Order> GetByIssuedOnBehalfOfCostCentre(Guid recipientCostCentreId, DocumentStatus? status, DateTime? fromDateIssued, DateTime? toDateIssued);
        List<Order> GetAll(OrderType orderType, DateTime startDate, DateTime endDate, string searchText = "");
        List<Order> GetAllDistributorPurchaseOrder(Guid distributorId);
        List<Order> GetDistributorPurchaseOrdersToProducer(Guid distributorId, DocumentStatus status);

        List<Order> GetByOrderTypeAndDocumentStatus(OrderType orderType, DocumentStatus docStatus,
                                                         bool ofThisStatus = true, DateTime startDate = new DateTime(),
                                                         DateTime endDate = new DateTime(), string searchText = "");//cn

        int GetOrderCount(DateTime startDate = new DateTime(), DateTime endDate = new DateTime());//cn
        int GetSaleCount(DateTime startDate = new DateTime(), DateTime endDate = new DateTime());
        int GetPurchaseOrderCount(DateTime startDate = new DateTime(), DateTime endDate = new DateTime());
        int GetStockistPurchaseOrderCount(DateTime startDate = new DateTime(), DateTime endDate = new DateTime());
        int CountAllOrders(DateTime startDate = new DateTime(), DateTime endDate = new DateTime());
        List<Order> GetAllPagenated(int skip, int take, int orderType = 0, DateTime startDate = new DateTime(), DateTime endDate = new DateTime());

        /// <summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="documentStatus"></param>
        /// <param name="orderType"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="searchText"></param>
        /// <returns></returns>
        /// </summary>
        List<Order> GetByDocumentStatusPagenated(int skip, int take, int documentStatus = -1,
                                                      int orderType = 0, DateTime startDate = new DateTime(),
                                                      DateTime endDate = new DateTime(), string searchText = "");

        /// <summary>
        /// if you assign documentStatus2 you must also assign hasStatus2
        /// extend method if necessary to suit your needs
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="documentStatus1"></param>
        /// <param name="hasStatus1"></param>
        /// <param name="documentStatus2"></param>
        /// <param name="hasStatus2"></param>
        /// <param name="orderType"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="searchText"></param>
        /// <returns></returns>
        List<Order> GetByDocumentStatusPagenated(int skip, int take,
                                                      int documentStatus1, bool hasStatus1 = true,
                                                      int? documentStatus2 = -1, bool? hasStatus2 = false,
                                                      int orderType = 0,
                                                      DateTime startDate = new DateTime(),
                                                      DateTime endDate = new DateTime(), string searchText = "");

        int GetCountByDocumentStatus(int documentStatus1, bool hasStatus1 = true,
                                            int? documentStatus2 = -1, bool? hasStatus2 = false,
                                            int orderType = 0,
                                            DateTime startDate = new DateTime(),
                                            DateTime endDate = new DateTime());

        int GetCountByDocumentStatus(int documentStatus = -1, int orderType = 0, DateTime startDate = new DateTime(),
                                     DateTime endDate = new DateTime());

        OrderLineItem GetLineItemById(Guid lineItemId);
        bool ChangeStatus(Guid documentId, DocumentStatus status);
        void SaveLineItem(OrderLineItem oli, Guid orderId); 
        void DeleteLineItem(OrderLineItem oli);
        int OrdersPendingApprovalCount();
        int OrdersPendingDispatchCount();
        int ApprovedPurchaseOrdersCount();
        List<Order> GetByDocumentStatus(DocumentStatus status);
        QueryResult<Order> Query(QueryOrders query);
    }
}
