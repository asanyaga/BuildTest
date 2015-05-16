using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Distributr.Core.Repository.Transactional.DocumentRepositories;
using Distributr.Reports;
using StructureMap;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.HQ.Lib.ViewModels.Admin.Orders;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Orders;

using Distributr.Core.Data.EF;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Transaction
{
    public static class HQCountClass
    {
        public static int CountPendingOrders(this HtmlHelper htmlHelper)
        {
            //IDocumentRepository _documentRepository = ObjectFactory.GetInstance<IDocumentRepository>();
            //int PendingOrders = _documentRepository.GetAll().OfType<Order>().Where(n => (n.Status == DocumentStatus.Confirmed && n.OrderType==OrderType.DistributorToProducer)).ToList().Count();

            //return PendingOrders;
            using (var _ctx = new CokeDataContext(ReportConnection.connectionString))
            {
                var pendingOrdersCount = from doc in _ctx.tblDocument
                                         .Where(n => n.DocumentTypeId == (int)DocumentType.Order)
                                         .Where(n => n.OrderOrderTypeId == (int)OrderType.DistributorToProducer)
                                         .Where(n => n.DocumentStatusId == (int)DocumentStatus.Confirmed)
                                         select doc;
                return pendingOrdersCount.Count();
            }

        }
        public static int CountPendingDeliveries(this HtmlHelper htmlHelper)
        {
            //IDocumentRepository _documentRepository = ObjectFactory.GetInstance<IDocumentRepository>();
            //int pendingDeliveries = _documentRepository.GetAll().OfType<Order>().Where(n => (n.Status == DocumentStatus.OrderPendingDispatch && n.OrderType==OrderType.DistributorToProducer)).ToList().Count();

            //return pendingDeliveries;
            using (var _ctx = new CokeDataContext(ReportConnection.connectionString))
            {
                var pendingDeliveries = from doc in _ctx.tblDocument.Where(n => (n.DocumentStatusId == (int)DocumentStatus.OrderPendingDispatch && n.OrderOrderTypeId == (int)OrderType.DistributorToProducer))
                                        select doc;

                return pendingDeliveries.Count();
            }

        }
        public static int CountApprovedOrders(this HtmlHelper htmlHelper)
        {
            //IDocumentRepository _documentRepository = ObjectFactory.GetInstance<IDocumentRepository>();
            //int approvedPOrders= _documentRepository.GetAll().OfType<Order>().Where(n => (n.Status == DocumentStatus.Approved && n.OrderType==OrderType.DistributorToProducer)).ToList().Count();
            //return approvedPOrders;
            using (var _ctx = new CokeDataContext(ReportConnection.connectionString))
            {
                var approvedPOrders = from doc in _ctx.tblDocument.Where(n => (n.DocumentStatusId == (int)DocumentStatus.Approved && n.OrderOrderTypeId == (int)OrderType.DistributorToProducer))
                                      select doc;
                return approvedPOrders.Count();
            }
        }
        public static int CountDeliveredPOrders(this HtmlHelper htmlHelper)
        {
            //IDocumentRepository _documentRepository = ObjectFactory.GetInstance<IDocumentRepository>();
            //int deliveredPOrders = _documentRepository.GetAll().OfType<Order>().Where(n => (n.Status == DocumentStatus.Closed && n.OrderType==OrderType.DistributorToProducer)).ToList().Count();
            //return deliveredPOrders;
            using (var _ctx = new CokeDataContext(ReportConnection.connectionString))
            {
                var deliveredPOrders = from doc in _ctx.tblDocument.Where(n => (n.DocumentStatusId == (int)DocumentStatus.Closed && n.OrderOrderTypeId == (int)OrderType.DistributorToProducer))
                                       select doc;
                return deliveredPOrders.Count();
            }
        }

    }
}
