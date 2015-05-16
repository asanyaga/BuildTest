using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Intergration;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IIntegrationDocumentRepository;
using Integration.QuickBooks.Lib.EF;
using Integration.QuickBooks.Lib.EF.Entities;

namespace Integration.QuickBooks.Lib.Repository.Impl
{
    public class TransactionRepository:ITransactionRepository
    {

        public List<Guid> SaveToLocal(OrderExportDocument document,TransactionType type)
        {
            var savedIds = new List<Guid>();
            var result = 0;
            using (var db=new AlidiLocalImportDatabaseContext())
            {
                //Find if the ExternalRef is present in local db so that we can mark it as exported in hq,
                //So that it is never exported again
                if(db.TransactionImports.Any(p=>p.ExternalRef==document.ExternalRef))
                {
                    var confirmListBypass = new List<Guid>();
                    confirmListBypass.Add(Guid.NewGuid());
                    return confirmListBypass;
                }
                foreach (var lineItem in document.LineItems)
                {
                    var id = Guid.NewGuid();
                    var transactionImport = new TransactionImport();
                    transactionImport.Id = id;
                    transactionImport.OutletCode = document.OutletCode;
                    transactionImport.OutletName = document.OutletName;
                    transactionImport.ExternalRef = document.ExternalRef;
                    transactionImport.GenericRef = document.OrderRef;
                    //var date = Convert.ToDateTime(document.OrderDate);
                    //var date2 = DateTime.Parse(document.OrderDate);
                    transactionImport.TransactionIssueDate = document.OrderDate;
                    transactionImport.TransactionDueDate = document.OrderDueDate;
                    transactionImport.TransactionType = (int)type;

                    //LineItems
                    transactionImport.ProductCode = lineItem.ProductCode;
                    transactionImport.Quantity = lineItem.Quantity;
                    transactionImport.TotalNet = lineItem.LineItemTotalNet;
                    transactionImport.TotalVat = lineItem.VatPerUnit;
                    transactionImport.VatClass = lineItem.VatClass;
                    transactionImport.SalesmanCode = document.SalesmanCode;
                    transactionImport.LineItemValue = lineItem.Price;

                    transactionImport.ExportStatus=(int)ExportStatus.New;

                    db.TransactionImports.Add(transactionImport);
                    savedIds.Add(id);
                }
                result=db.SaveChanges();
            } 

            if (result > 0)
            {
                return savedIds;
            }
            return null;
        }

        private TransactionType GetTransactionType(DocumentType documentType, OrderType orderType)
        {
           if(documentType==DocumentType.Order)
           {
                if (orderType == OrderType.DistributorPOS)
                {
                     return TransactionType.DistributorPOS;
                }
                if(orderType==OrderType.OutletToDistributor)
                {
                    return TransactionType.OutletToDistributor;
                }
                       
           }
           if(documentType==DocumentType.Invoice)
           {
               return TransactionType.Invoice;
           }
           if(documentType==DocumentType.Receipt)
           {
               return TransactionType.Receipt;
           }
                   
            return TransactionType.Unknown;
        }


        public void SaveToQuickBooks(TransactionImport transactionImport)
        {
            throw new NotImplementedException();
        }

        public void MarkExportedLocal(Guid id)
        {
            throw new NotImplementedException();
        }

        public void MarkExportedLocal(string orderRef, string QBOrderTransactionId)
        {
            throw new NotImplementedException();
        }

        public void MarkExportedLocal(string externalDocRef)
        {
            using (var db=new AlidiLocalImportDatabaseContext())
            {
                try
                {
                    var transactions = db.TransactionImports.Where(p => p.ExternalRef == externalDocRef);
                    foreach (var transactionImport in transactions)
                    {
                        transactionImport.ExportStatus =(int) ExportStatus.Exported;
                        db.SaveChanges();
                    }
                   
                }
                catch (Exception ex)
                {
                    
                    throw;
                }
            }
        }

        //public List<OrderExportDocument> LoadFromDB(TransactionType type)
        //{
        //    var listOfDocs = new List<OrderExportDocument>();
        //    using (var db = new AlidiLocalDatabaseContext())
        //    {
        //        var nextRef = GetNextExternalRef(type);
        //        var docs = db.TransactionImports.Where(p => p.ExternalRef == nextRef);//.GroupBy(p=>p.ExternalRef).ToList();
        //       listOfDocs = docs.Select(Map).ToList();
        //    }

        //}

        public List<TransactionImport> LoadFromDB(TransactionType type)
        {
            List<TransactionImport> listOfDocs;
            using (var db = new AlidiLocalImportDatabaseContext())
            {
                var nextRef = GetNextExternalRef(type);
                listOfDocs = db.TransactionImports.Where(p => p.ExportStatus == (int)ExportStatus.New && p.TransactionType == (int)type).ToList();
                return listOfDocs;
                //listOfDocs = docs.Select(Map).ToList();
            }
        }

        private string GetNextExternalRef(TransactionType type)
        {
            using (var db=new AlidiLocalImportDatabaseContext())
            {
//                var query = string.Format(@"SELECT top 1 ExternalRef FROM tblTransactionImports 
//                WHERE (TransactionType={0} 
//                AND ExportStatus ={1} )",
//              (int)type, (int)ExportStatus.New);

                var externalRef =db.TransactionImports.Where(p =>  p.TransactionType == (int) type && p.ExportStatus == (int)ExportStatus.New).Select(p=>p.ExternalRef).FirstOrDefault();
                return externalRef;
            }
        }

        private OrderExportDocument Map(TransactionImport transactionImport)
        {
            throw new NotImplementedException();
        }
    }
}
