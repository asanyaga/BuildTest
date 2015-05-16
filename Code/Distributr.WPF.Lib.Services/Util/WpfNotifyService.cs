using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.SettingsEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.OrderDocumentEntities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Notifications;
using Distributr.Core.Repository.Master.CentreRepositories;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.SettingsRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IInvoiceRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.WPF.Lib.Services.Service.CommandQueues;
using Distributr.WPF.Lib.Services.Service.Utility;

namespace Distributr.WPF.Lib.Services.Util
{
    public class WpfNotifyService : INotifyService
    {
        private IOutgoingNotificationQueueRepository _queueRepository;
        private IMainOrderRepository _mainOrderRepository;
        private IInvoiceRepository _invoiceRepository;
        private ISettingsRepository _settingsRepository;
        private ICentreRepository _centreRepository;
        private ICommoditySupplierRepository _commoditySupplierRepository;

        public WpfNotifyService(IOutgoingNotificationQueueRepository queueRepository, IMainOrderRepository mainOrderRepository, IInvoiceRepository invoiceRepository, ISettingsRepository settingsRepository, ICentreRepository centreRepository, ICommoditySupplierRepository commoditySupplierRepository)
        {
            _queueRepository = queueRepository;
            _mainOrderRepository = mainOrderRepository;
            _invoiceRepository = invoiceRepository;
            _settingsRepository = settingsRepository;
            _centreRepository = centreRepository;
            _commoditySupplierRepository = commoditySupplierRepository;
        }

        public void SubmitOrderSaleNotification(MainOrder order)
        {
            try
            {

                if (order == null) return;
                var allow = _settingsRepository.GetByKey(SettingsKeys.AllowOrderSaleNotification);
                bool canSend = false;
                if (allow != null) bool.TryParse(allow.Value, out canSend);
                if (!canSend)
                    return;

                Guid distributorId = Guid.Empty;
                Guid salesmanId = Guid.Empty;
                if (order.DocumentIssuerCostCentre is Distributor)
                {
                    distributorId = order.DocumentIssuerCostCentre.Id;
                    salesmanId = order.DocumentRecipientCostCentre.Id;
                }
                else
                {
                    distributorId = order.DocumentRecipientCostCentre.Id;
                    salesmanId = order.DocumentIssuerCostCentre.Id;
                }
                var notification = new NotificationOrderSale();
                notification.Id = Guid.NewGuid();

                notification.DistributorId = distributorId;
                notification.OutletId = order.IssuedOnBehalfOf.Id;
                notification.SalemanId = salesmanId;
                notification.DocumentRef = order.DocumentReference;
                notification.TotalGross = order.TotalGross;
                notification.SalevalueDiscount = order.SaleDiscount;
                notification.TotalNet = order.TotalNet;
                notification.TotalVat = order.TotalVat;

                notification.Items = new List<NotificationOrderSaleItem>();
                foreach (var lineItemSummary in order.ItemSummary)
                {
                    var item = new NotificationOrderSaleItem();
                    item.ItemName = lineItemSummary.Product.Description;
                    item.Quantity = lineItemSummary.Qty;
                    item.Discount = lineItemSummary.ProductDiscount;
                    item.TotalGross = lineItemSummary.TotalGross;
                    item.TotalVat = lineItemSummary.TotalVat;
                    item.TotalNet = lineItemSummary.TotalNet;
                    item.UnitPrice = lineItemSummary.Value;
                    notification.Items.Add(item);
                }
                _queueRepository.Add(notification);
            }catch(Exception ex)
            {
                
            }

        }

        public void SubmitInvoiceNotification(Invoice invoice)
        {
            try
            {
                if (invoice == null) return;
                var allow = _settingsRepository.GetByKey(SettingsKeys.AllowInvoiceNotification);
                bool canSend = false;
                if (allow != null) bool.TryParse(allow.Value, out canSend);
                if (!canSend)
                    return;

                Guid distributorId = Guid.Empty;
                Guid salesmanId = Guid.Empty;
                if (invoice.DocumentIssuerCostCentre is Distributor)
                {
                    distributorId = invoice.DocumentIssuerCostCentre.Id;
                    salesmanId = invoice.DocumentRecipientCostCentre.Id;
                }
                else
                {
                    distributorId = invoice.DocumentRecipientCostCentre.Id;
                    salesmanId = invoice.DocumentIssuerCostCentre.Id;
                }
                var order = _mainOrderRepository.GetById(invoice.OrderId);
                var notification = new NotificationInvoice();
                if (order != null)
                {
                    notification.OutletId = order.IssuedOnBehalfOf.Id;
                    notification.OrderRef = order.DocumentReference;
                }

                notification.Id = Guid.NewGuid();
                notification.DistributorId = distributorId; //
                notification.SalemanId = salesmanId;
                notification.DocumentRef = invoice.DocumentReference;
                notification.TotalGross = invoice.TotalGross;
                notification.SalevalueDiscount = invoice.SaleDiscount;
                notification.TotalNet = invoice.TotalNet;
                notification.TotalVat = invoice.TotalVat;

                notification.Items = new List<NotificationInvoiceItem>();
                foreach (var lineItemSummary in invoice.LineItems)
                {
                    var item = new NotificationInvoiceItem();
                    item.ItemName = lineItemSummary.Product.Description;
                    item.Quantity = lineItemSummary.Qty;
                    item.Discount = lineItemSummary.ProductDiscount;
                    item.TotalGross = lineItemSummary.LineItemTotal;
                    item.TotalVat = lineItemSummary.LineItemVatTotal;
                    item.TotalNet = lineItemSummary.LineItemTotal;
                    item.UnitPrice = lineItemSummary.Value;
                    notification.Items.Add(item);
                }
                _queueRepository.Add(notification);
            }catch(Exception)
            {
                
            }

        }

        public void SubmitRecieptNotification(Receipt receipt)
        {

            try
            {
                if (receipt == null) return;
                var allow = _settingsRepository.GetByKey(SettingsKeys.AllowReceiptNotification);
                bool canSend = false;
                if (allow != null) bool.TryParse(allow.Value, out canSend);
                if (!canSend)
                    return;
                Guid distributorId = Guid.Empty;
                Guid salesmanId = Guid.Empty;
                if (receipt.DocumentIssuerCostCentre is Distributor)
                {
                    distributorId = receipt.DocumentIssuerCostCentre.Id;
                    salesmanId = receipt.DocumentRecipientCostCentre.Id;
                }
                else
                {
                    distributorId = receipt.DocumentRecipientCostCentre.Id;
                    salesmanId = receipt.DocumentIssuerCostCentre.Id;
                }
                var notification = new NotificationReceipt();
                var invoice = _invoiceRepository.GetById(receipt.InvoiceId);
                if (invoice != null)
                {
                    var order = _mainOrderRepository.GetById(invoice.OrderId);
                    notification.InvoiceRef = invoice.DocumentReference;
                    if (order != null)
                    {
                        notification.OutletId = order.IssuedOnBehalfOf.Id;
                        notification.OrderRef = order.DocumentReference;

                    }
                }

                notification.Id = Guid.NewGuid();
                notification.DistributorId = distributorId; //
                notification.SalemanId = salesmanId;
                notification.DocumentRef = receipt.DocumentReference;
                notification.TotalAmount = receipt.Total;


                notification.Items = new List<NotificationReceiptItem>();
                foreach (var lineItemSummary in receipt.LineItems)
                {
                    var item = new NotificationReceiptItem();
                    item.ItemName = lineItemSummary.PaymentType.ToString();
                    item.Quantity = lineItemSummary.Value;
                    item.Reference = lineItemSummary.PaymentRefId;
                    item.Description = lineItemSummary.Description;

                    notification.Items.Add(item);
                }
                _queueRepository.Add(notification);
            }catch
            {
                
            }
        }

        public void SubmitCommodityPurchase(CommodityPurchaseNote purchaseNote)
        {
            try
            {
                if (purchaseNote == null) return;
                var allow = _settingsRepository.GetByKey(SettingsKeys.AllowCommodityPurchaseNotification);
                bool canSend = false;
                if (allow != null) bool.TryParse(allow.Value, out canSend);
                if (!canSend)
                    return;

                Guid hubId = Guid.Empty;
                Guid purchaseClerkId = Guid.Empty;
                if (purchaseNote.DocumentIssuerCostCentre is Hub)
                {
                    hubId = purchaseNote.DocumentIssuerCostCentre.Id;
                    purchaseClerkId = purchaseNote.DocumentRecipientCostCentre.Id;
                    
                }
                else
                {
                    hubId = purchaseNote.DocumentRecipientCostCentre.Id;
                    purchaseClerkId = purchaseNote.DocumentIssuerCostCentre.Id;
                }
                var notification = new NotificationPurchase();
                var centre = _centreRepository.GetById(purchaseNote.CentreId);
                if(centre!= null)
                {
                    notification.CenterName = centre.Name+"("+centre.Code+")" ;
                }
                notification.CummulativeWeightDetail = "0.00";
                var cummulativeWeight   = _commoditySupplierRepository.GetCummulativeWeight(purchaseNote.CommoditySupplier.Id, Guid.Empty);
                if (cummulativeWeight != null)
                {
                    decimal cweight = (decimal) cummulativeWeight;
                    notification.CummulativeWeightDetail = cweight.ToString("N2") + "Kgs as at " +
                                                           DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                }
                notification.Id = Guid.NewGuid();
                notification.ServedBy = purchaseNote.DocumentIssuerUser.Username;
                notification.HubId = hubId;
                notification.FarmerId = purchaseNote.CommoditySupplier.Id;
                notification.PurchaseClerkId = purchaseClerkId;
                notification.DocumentRef = purchaseNote.DocumentReference;


                notification.Items = new List<NotificationPurchaseSaleItem>();
                foreach (var lineItemSummary in purchaseNote.LineItems)
                {
                    var item = new NotificationPurchaseSaleItem();
                    item.ItemName = lineItemSummary.Commodity.Name;
                    item.Quantity = lineItemSummary.Weight;
                    item.Grade = lineItemSummary.CommodityGrade.Name;

                    notification.Items.Add(item);
                }
                _queueRepository.Add(notification);
            }
            catch (Exception ex)
            {

            }
        }

        public void SubmitDispatch(DispatchNote dispatch)
        {

            try
            {
                if (dispatch == null) return;
                var allow = _settingsRepository.GetByKey(SettingsKeys.AllowDispatchNotification);
                bool canSend = false;
                if (allow != null) bool.TryParse(allow.Value, out canSend);
                if (!canSend)
                    return;

                Guid distributorId = Guid.Empty;
                Guid salesmanId = Guid.Empty;
                if (dispatch.DocumentIssuerCostCentre is Distributor)
                {
                    distributorId = dispatch.DocumentIssuerCostCentre.Id;
                    salesmanId = dispatch.DocumentRecipientCostCentre.Id;
                }
                else
                {
                    distributorId = dispatch.DocumentRecipientCostCentre.Id;
                    salesmanId = dispatch.DocumentIssuerCostCentre.Id;
                }
                var order = _mainOrderRepository.GetById(dispatch.OrderId);
                var notification = new NotificationDispatch();
                if (order != null)
                {
                    notification.OutletId = order.IssuedOnBehalfOf.Id;
                    notification.OrderRef = order.DocumentReference;
                    if (order.DocumentIssuerCostCentre is Distributor)
                    {
                        distributorId = order.DocumentIssuerCostCentre.Id;
                        salesmanId = order.DocumentRecipientCostCentre.Id;
                    }
                    else
                    {
                        distributorId = order.DocumentRecipientCostCentre.Id;
                        salesmanId = order.DocumentIssuerCostCentre.Id;
                    }
                }

                notification.Id = Guid.NewGuid();
                notification.DistributorId = distributorId; //
                notification.SalemanId = salesmanId;
                notification.DocumentRef = dispatch.DocumentReference;
               

                notification.Items = new List<NotificationDispatchItem>();
                foreach (var lineItemSummary in dispatch.LineItems)
                {
                    var item = new NotificationDispatchItem();
                    item.ItemName = lineItemSummary.Product.Description;
                    item.Quantity = lineItemSummary.Qty;
                   
                    notification.Items.Add(item);
                }
                _queueRepository.Add(notification);
            }
            catch (Exception)
            {

            }
        }
    }
}
