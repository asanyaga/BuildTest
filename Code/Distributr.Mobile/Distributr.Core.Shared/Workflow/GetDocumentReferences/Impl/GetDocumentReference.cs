using System;
using Distributr.Core.ClientApp;
using Distributr.Core.Domain.Master.SettingsEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.SettingsRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.CreditNoteRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.DisbursementRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.DispatchRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IInvoiceRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.InventoryRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.ReceiptInventories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.ReturnsRepositories;
using Distributr.Core.Repository.Transactional.SourcingDocumentRepositories;

namespace Distributr.Core.Workflow.GetDocumentReferences.Impl
{
    public class GetDocumentReference : IGetDocumentReference
    {
        private IReceiptRepository _receiptService;
        private IInvoiceRepository _invoiceService;
        private ICostCentreRepository _costCentreService;
       
        private IDispatchNoteRepository _dispatchNoteService;
        private IDisbursementNoteRepository _disbursementNoteService;
        private IInventoryTransferNoteRepository _inventoryTransferNoteService;
        private ICreditNoteRepository _creditNoteService;
        private IOrderRepository _orderService;
        private IReturnsNoteRepository _returnsNoteService;
        private ICommodityPurchaseRepository _commodityPurchaseRepository;
        private ICommodityReceptionRepository _commodityReceptionRepository;
        private ICommodityStorageRepository _commodityStorageRepository;
        private ICommodityWarehouseStorageRepository _commodityWarehouseStorageRepository;
        private ISettingsRepository _settingsRepository;
        private ICommodityReleaseRepository _commodityReleaseRepository;

        public GetDocumentReference(IReceiptRepository receiptService, IInvoiceRepository invoiceService, ICostCentreRepository costCentreService, IDispatchNoteRepository dispatchNoteService, IDisbursementNoteRepository disbursementNoteService, IInventoryTransferNoteRepository inventoryTransferNoteService, ICreditNoteRepository creditNoteService, IOrderRepository orderService, IReturnsNoteRepository returnsNoteService, ICommodityPurchaseRepository commodityPurchaseRepository, ICommodityReceptionRepository commodityReceptionRepository, ICommodityStorageRepository commodityStorageRepository, ISettingsRepository settingsRepository, ICommodityWarehouseStorageRepository commodityWarehouseStorageRepository, ICommodityReleaseRepository commodityReleaseRepository)
        {
            _receiptService = receiptService;
            _invoiceService = invoiceService;
            _costCentreService = costCentreService;
            
            _dispatchNoteService = dispatchNoteService;
            _disbursementNoteService = disbursementNoteService;
            _inventoryTransferNoteService = inventoryTransferNoteService;
            _creditNoteService = creditNoteService;
            _orderService = orderService;
            _returnsNoteService = returnsNoteService;
            _commodityPurchaseRepository = commodityPurchaseRepository;
            _commodityReceptionRepository = commodityReceptionRepository;
            _commodityStorageRepository = commodityStorageRepository;
            _settingsRepository = settingsRepository;
            _commodityWarehouseStorageRepository = commodityWarehouseStorageRepository;
            _commodityReleaseRepository = commodityReleaseRepository;
        }

        //public string GetDocReference(string docType, string salesmanUserName, Guid outletId, Guid salesmanCCId, Guid CCAppId)
        public string GetDocReference(string docType, string salesmanUserName, string outletCode)
        {
            try
            {
                //int sequenceId = 0;

                DateTime dt = DateTime.Now;
                string refRule = "{D}_{SN}_{OC}_{DT}_{TM}_{SQ}";
                var docrefRule = _settingsRepository.GetByKey(SettingsKeys.DocReferenceRule);
                if (docrefRule != null)
                {
                    refRule = docrefRule.Value;
                }
                string D = "";
                string DT = dt.ToString("yyyyMMdd");
                string TM = dt.ToString("hhmmss");
                string SN = salesmanUserName;
 //               string SC = "";
//                string ON = "";
                string OC = "";
                if(outletCode!=null)
                {
                    OC = outletCode;
                }
                string SQ = "";


                //string dateStamp = dt.Year + "-" + dt.Month + "-" + dt.Day + "-" + dt.Hour + ":" + dt.Minute;
                var dateStamp = dt.Year + "." + dt.Month + "." + dt.Day + "." + dt.Hour + ":" + dt.Minute.ToString();

                switch (docType)
                {
                    case "SO":
                        D = "Ord";
                        var orderInitial = _settingsRepository.GetByKey(SettingsKeys.DocOrderInitial);
                        if (orderInitial!=null)
                        {
                            D = orderInitial.Value;
                        }
                        SQ = (_orderService.GetOrderCount() + 1).ToString();
                        break;
                    case "PO":
                        D = "PO";
                        SQ = (_orderService.GetPurchaseOrderCount() + 1).ToString();
                        break;
                    case "Sale":
                        D = "Sale";
                        var saleInitial = _settingsRepository.GetByKey(SettingsKeys.DocSaleInitial);
                        if (saleInitial != null)
                        {
                            D = saleInitial.Value;
                        }
                        SQ = (_orderService.GetSaleCount() + 1).ToString();
                        break;
                    case "Inv":
                        D = "Inv";
                         var invoiceInitial = _settingsRepository.GetByKey(SettingsKeys.DocInvoiceInitial);
                         if (invoiceInitial != null)
                        {
                            D = invoiceInitial.Value;
                        }
                        SQ = (_invoiceService.GetCount() + 1).ToString();
                        break;
                    case "Rpt":
                        D = "Rct";
                         var receiptInitial = _settingsRepository.GetByKey(SettingsKeys.DocReceiptInitial);
                         if (receiptInitial != null)
                        {
                            D = receiptInitial.Value;
                        }
                        SQ = (_receiptService.GetCount() + 1).ToString();
                        break;
                    case "DN":
                        D = "DN";
                        SQ = (_dispatchNoteService.GetCount() + 1).ToString();
                        break;
                    case "DsN":
                        D = "DsN";
                        SQ = (_disbursementNoteService.GetCount() + 1).ToString();
                        break;
                    case "ITN":
                        D = "ITN";
                        SQ = (_inventoryTransferNoteService.GetCount() + 1).ToString();
                        break;
                    case "CN":
                        D = "CN";
                        SQ = (_creditNoteService.GetCount() + 1).ToString();
                        break;
                    case "RN": //returns note
                        D = "RN";
                        SQ = (_returnsNoteService.GetCount() + 1).ToString();
                        break;
                    case "PurchaseNote":
                        D = "PN";
                        SQ = (_commodityPurchaseRepository.GetCount() + 1).ToString();
                        break;
                    case "ReceptionNote":
                        D = "RN";
                        SQ = (_commodityReceptionRepository.GetCount() + 1).ToString();
                        break;
                    case "StorageNote":
                        D = "SN";
                        SQ = (_commodityStorageRepository.GetCount() + 1).ToString();
                        break;
                }
                SQ = SQ.PadLeft(5, '0');
                //string formatString = prefix + "_{0}_{1}_{2}_{3}_{4}_{5}";
                //return string.Format(formatString, salesmanUserName, outletId, dateStamp, salesmanCCId, CCAppId, sequenceId);

                //docType_SalesmanName_OutletCode_DateStamp_SeqId
                string refno = refRule.Replace("{D}", D).Replace("{SN}", SN).Replace("{OC}", OC).Replace("{DT}", DT).Replace("{TM}", TM).Replace("{SQ}", SQ);
                return refno;
            }
            catch
            {
                //MessageBox.Show("Error getting new document reference.");
                return "";
            }
        }

        public string GetDocReference(string docType, string orderRef)
        {
            try
            {
                string formatString = "";
                int sequenceId = 0;
                string IdWithoutPrefix = orderRef.Substring(orderRef.IndexOf('_')+1);
                switch (docType)
                {
                    case "Inv":
                        //orderAppSeqId = _invoiceService.GetCount() + 1;
                         formatString = docType + "_{0}";
                        break;
                    case "Rpt":
                        docType = "Rct";
                        sequenceId = _receiptService.GetCount() + 1;
                        formatString = docType + "_{0}_{1}";
                        break;
                    case "DN":
                        docType = "DN";
                        sequenceId = _dispatchNoteService.GetCount() + 1;
                        formatString = docType + "_{0}_{1}";
                        break;
                    case "DsN":
                        sequenceId = _disbursementNoteService.GetCount() + 1;
                        formatString = docType + "_{0}_{1}";
                        break;
                    case "ITN":
                        sequenceId = _inventoryTransferNoteService.GetCount() + 1;
                        formatString = docType + "_{0}_{1}";
                        break;
                    case "CN":
                        sequenceId = _creditNoteService.GetCount() + 1;
                        formatString = docType + "_{0}_{1}";
                        break;
                    case "DISC": 
                        formatString = docType + "_{0}";
                        break;
                }
              
                return string.Format(formatString, IdWithoutPrefix, sequenceId);
            }
            catch
            {
                //MessageBox.Show("Error getting new document reference");
                return "";
            }
        }

        public string GetDocReference(string docType, Guid salesmanId, Guid outletId)
        {
            try
            {
                DateTime dt = DateTime.Now;
                string D = "";
                string DT = dt.ToString("yyyyMMdd");
                string TM = dt.ToString("hhmmss");
                string SN = "";
                string SC = "";
                string ON = "";
                string OC = "";
                string SQ = "";
                //int sequenceId = 0;
                var salesman = _costCentreService.GetById(salesmanId);
                var outlet = _costCentreService.GetById(outletId);
                ON = outlet != null ? outlet.Name : "";
                OC = outlet != null ? outlet.CostCentreCode : "";
                SN = salesman != null ? salesman.Name : "";
                SC = salesman != null ? salesman.CostCentreCode : "";

               
                string refRule = "{D}_{SN}_{OC}_{DT}_{TM}_{SQ}";
                var docrefRule = _settingsRepository.GetByKey(SettingsKeys.DocReferenceRule);
                if (docrefRule != null)
                {
                    refRule = docrefRule.Value;
                }
                
                


                //string dateStamp = dt.Year + "-" + dt.Month + "-" + dt.Day + "-" + dt.Hour + ":" + dt.Minute;
                var dateStamp = dt.Year + "." + dt.Month + "." + dt.Day + "." + dt.Hour + ":" + dt.Minute.ToString();

                switch (docType)
                {
                    case "SO":
                        D = "Ord";
                        var orderInitial = _settingsRepository.GetByKey(SettingsKeys.DocOrderInitial);
                        if (orderInitial != null)
                        {
                            D = orderInitial.Value;
                        }
                        SQ = (_orderService.GetOrderCount() + 1).ToString();
                        break;
                    case "PO":
                        D = "PO";
                        SQ = (_orderService.GetPurchaseOrderCount() + 1).ToString();
                        break;
                    case "SPO":
                        D = "SPO";
                        SQ = (_orderService.GetStockistPurchaseOrderCount() + 1).ToString();
                        break;
                    case "Sale":
                        D = "Sale";
                        var saleInitial = _settingsRepository.GetByKey(SettingsKeys.DocSaleInitial);
                        if (saleInitial != null)
                        {
                            D = saleInitial.Value;
                        }
                        SQ = (_orderService.GetSaleCount() + 1).ToString();
                        break;
                    case "Inv":
                        D = "Inv";
                        var invoiceInitial = _settingsRepository.GetByKey(SettingsKeys.DocInvoiceInitial);
                        if (invoiceInitial != null)
                        {
                            D = invoiceInitial.Value;
                        }
                        SQ = (_invoiceService.GetCount() + 1).ToString();
                        break;
                    case "Rpt":
                        D = "Rct";
                        var receiptInitial = _settingsRepository.GetByKey(SettingsKeys.DocReceiptInitial);
                        if (receiptInitial != null)
                        {
                            D = receiptInitial.Value;
                        }
                        SQ = (_receiptService.GetCount() + 1).ToString();
                        break;
                    case "DN":
                        D = "DN";
                        SQ = (_dispatchNoteService.GetCount() + 1).ToString();
                        break;
                    case "DsN":
                        D = "DsN";
                        SQ = (_disbursementNoteService.GetCount() + 1).ToString();
                        break;
                    case "ITN":
                        D = "ITN";
                        SQ = (_inventoryTransferNoteService.GetCount() + 1).ToString();
                        break;
                    case "CN":
                        D = "CN";
                        SQ = (_creditNoteService.GetCount() + 1).ToString();
                        break;
                    case "RN": //returns note
                        D = "RN";
                        SQ = (_returnsNoteService.GetCount() + 1).ToString();
                        break;
                    case "PurchaseNote":
                        D = "PN";
                        SQ = (_commodityPurchaseRepository.GetCount() + 1).ToString();
                        break;
                    case "ReceptionNote":
                        D = "RN";
                        SQ = (_commodityReceptionRepository.GetCount() + 1).ToString();
                        break;
                    case "StorageNote":
                        D = "SN";
                        SQ = (_commodityStorageRepository.GetCount() + 1).ToString();
                        break;
                    case "CommodityTransfer":
                        D = "CT";
                        SQ = (_commodityStorageRepository.GetCount() + 1).ToString();
                        break;
                    case "WarehouseAddEntryNote":
                        D = "WHE";
                        SQ = (_commodityWarehouseStorageRepository.GetCount() + 1).ToString();
                        break;
                    case "WarehouseReceipt":
                        D = "WHR";
                        SQ = (_commodityWarehouseStorageRepository.GetCount() + 1).ToString();
                        break;
                    case "CommodityRelease":
                        D = "CRN";
                        SQ = (_commodityReleaseRepository.GetCount() + 1).ToString();
                        break;
                }
                SQ = SQ.PadLeft(5, '0');
                //string formatString = prefix + "_{0}_{1}_{2}_{3}_{4}_{5}";
                //return string.Format(formatString, salesmanUserName, outletId, dateStamp, salesmanCCId, CCAppId, sequenceId);

                //docType_SalesmanName_OutletCode_DateStamp_SeqId
                string refno = refRule.Replace("{D}", D).Replace("{SN}", SN).Replace("{SC}", SC).Replace("{ON}", ON).Replace("{OC}", OC).Replace("{DT}", DT).Replace("{TM}", TM).Replace("{SQ}", SQ);
                refno= refno.Replace(" ","");
                return refno;
            }
            catch
            {
                //MessageBox.Show("Error getting new document reference.");
                return "";
            }
        }
    }
}
