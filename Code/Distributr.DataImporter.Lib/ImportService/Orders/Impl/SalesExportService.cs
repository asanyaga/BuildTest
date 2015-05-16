using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IIntegrationDocumentRepository;
using Distributr.Core.Repository.Transactional.ThirdPartyIntegrationRepository;
using Distributr.Core.Utility;
using Distributr.DataImporter.Lib.Utils;
using GalaSoft.MvvmLight.Messaging;
using LINQtoCSV;
using StructureMap;

namespace Distributr.DataImporter.Lib.ImportService.Orders.Impl
{
    internal class SalesExportService : ISalesExportService
    {
      
        private IIntegrationDocumentRepository _integrationDocumentRepository;
       private List<FclExportOrderDto> _salesPendingExport;
        private List<FclPaymentExportDto> _fclPaymentExportDtos;

        public SalesExportService(IIntegrationDocumentRepository integrationDocumentRepository)
        {
            _integrationDocumentRepository = integrationDocumentRepository;
           
            _salesPendingExport=new List<FclExportOrderDto>();
            _fclPaymentExportDtos=new List<FclPaymentExportDto>();
        }

        public void ExportSales()
        {

            Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + " Fetching sales pending export");
            var orders=new List<FclExportOrderDto>();

            orders = _integrationDocumentRepository.GetFclOrdersPendingExport(OrderType.DistributorPOS, "");
            
                
           
            _salesPendingExport.AddRange(orders);
            Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") +
                                  string.Format(" Fetched {0} sales for export", orders.Count));
            Export();
            if(!exportFailed)
                Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") +
                                       string.Format(" Export Sales task done.."));
        }

        private bool exportFailed = false;
        private void Export()
        {
            if (_salesPendingExport.Any())
            { 
                var orders = _salesPendingExport.Select(OrderExportHelper.MapSalesExport).ToList();
                var orderToCsv = new List<ExportSaleItem>();
                foreach (var order in orders)
                {
                    orderToCsv.AddRange(order);
                }
                var outputFileDescription = new CsvFileDescription
                {
                    // cool - I can specify my own separator!
                    SeparatorChar = ',',
                    FirstLineHasColumnNames =
                        false,
                    QuoteAllFields = true,
                    EnforceCsvColumnAttribute =
                        true
                };
                try
                {
                    Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + " Attempting to export {0} sales", orders.Count);
                    //var cc = new CsvContext();
                    //cc.Write(orderToCsv, OrderExportHelper.GetExportFileName("AllSales"), outputFileDescription);

                    DumpExportFilesAsync(orderToCsv.ToCsv(),OrderExportHelper.GetExportFileName("AllSales"));

                    _salesPendingExport.OrderBy(p => p.GenericOrderReference).Distinct().ToList().ForEach(
                        OrderExportHelper.MarkAsExported);
                   Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + string.Format(" Export compeleted for={0} sales", orders.Count));
                   _salesPendingExport.Clear();
                }
                catch (Exception ex)
                {
                    Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + "Error occured while exporting..See error logs for details");
                    FileUtility.LogError(ex.Message);
                }

            }
            else
            {
                Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + " No  sales exported");
            }
        }
  
        public void ExportPayments()
        {
           
            Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + " Fetching payments pending export");
            var payments = _integrationDocumentRepository.GetFclPaymentsPendingExport();
            _fclPaymentExportDtos.AddRange(payments);
            Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") +
                                  string.Format(" Fetched {0} payments for export", payments.Count));
            ExportPayments(payments);

            if (!paymentsExportfailed)
                Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") +
                                           string.Format(" Export payments task done.."));
        }

        private bool paymentsExportfailed = false;
        void ExportPayments(IEnumerable<FclPaymentExportDto> dtos)
        {
            var payments = _fclPaymentExportDtos.Select(OrderExportHelper.MapExport).ToList();

            if (payments.Any())
            {

              
                try
                {
                    Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + " Attempting to export {0} payments",
                                           payments.Count);

                    DumpExportFilesAsync(payments.ToCsv(), OrderExportHelper.GetExportFileName("Payments"));
                    Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") +
                                           string.Format(" Export compeleted for={0} payments", payments.Count));
                    OrderExportHelper.MarkPaymentAsExported(dtos);
                    
                }
                catch (Exception ex)
                {
                    Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") +
                                           "Error occured while exporting..payments...See error logs for details");
                    paymentsExportfailed = true;
                    FileUtility.LogError(ex.Message);
                }
            }
            else
            {
                Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + " No pending payments");
            }
        }

        private async void DumpExportFilesAsync(string file, string selectedPath)
        {
          try
            {
                using (var fs = new FileStream(selectedPath, FileMode.OpenOrCreate))
                {
                    fs.Close();
                    using (var wr = new StreamWriter(selectedPath, false))
                    {
                        await wr.WriteLineAsync(file);

                    }
                }


            }
            catch (IOException ex)
            {

            }
        }
    }
    public class PaymentExportItem
    {
        [CsvColumn(FieldIndex = 1)]
        public string OrderReference { get; set; }

        [CsvColumn(FieldIndex = 2)]
        public string PaymentDate { get; set; }

        [CsvColumn(FieldIndex = 3)]
        public string SalesmanCode { get; set; }

        [CsvColumn(FieldIndex = 4)]
        public string OutletCode { get; set; }

        [CsvColumn(FieldIndex = 5)]
        public string ShiptoAddressCode { get; set; }

        [CsvColumn(FieldIndex = 6)]
        public string Salevalue { get; set; }

        [CsvColumn(FieldIndex = 7)]
        public string AmountPaid { get; set; }

        [CsvColumn(FieldIndex = 8)]
        public string Balance { get; set; }
    }

    //outletcode,sale date,salesman code,shiptoadrdresscode,document reference,product code,quantity
    public class ExportSaleItem
    {
        [CsvColumn(FieldIndex = 1, CanBeNull = false)]
        public string OutletCode { get; set; }

        [CsvColumn(FieldIndex = 2, CanBeNull = false)]
        public string SaleDate { get; set; }

        [CsvColumn(FieldIndex = 3, CanBeNull = false)]
        public string SalesmanCode { get; set; }

        [CsvColumn(FieldIndex = 4)]
        public string ShiptoAddressCode { get; set; }

        [CsvColumn(FieldIndex = 5, CanBeNull = false)]
        public string OrderReference { get; set; }

        [CsvColumn(FieldIndex = 6, CanBeNull = false)]
        public string ProductCode { get; set; }

        [CsvColumn(FieldIndex = 7, CanBeNull = false)]
        public decimal ApprovableQuantity { get; set; }
    }
}
