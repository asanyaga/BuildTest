using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Intergration;
using Distributr_Middleware.WPF.Lib.Utils;
using Newtonsoft.Json;
using SAPbobsCOM;
using SAPUtilityLib.Proxies;

namespace SAPUtilityLib.Masterdata.Impl
{
    public class OrderExportTransactionsService :SAPUtilsBase, IOrderExportTransactionsService
    {
        private ISapWebProxy _sapWebProxy;

        public OrderExportTransactionsService(ISapWebProxy sapWebProxy)
        {
            _sapWebProxy = sapWebProxy;
        }

        public Task<SyncBasicResponse> ExportToSap(OrderType orderType, DocumentStatus status)
        {
         
            return Task.Run(() =>
                            {
                                var responce = _sapWebProxy.GetNextOrder(orderType, status);
                                if (responce.Success)
                                {
                                    var data = JsonConvert.DeserializeObject<OrderExportDocument>(responce.TransactionData);
                                    return Export(data);
                                }
                                else
                                {
                                    return new SyncBasicResponse { Status = false,Info = responce.ErrorInfo};
                                }
                               
                            });
           
        }

        private SyncBasicResponse Export(OrderExportDocument doc)
        {
       
            if (!OrderRefExist(doc.ExternalRef))
            {
                Documents oOrder = (Documents)Company.GetBusinessObject(BoObjectTypes.oOrders);
               
               
                
                // set properties of the Order object
                oOrder.CardCode = doc.OutletCode; //CustomerCode;
               // oOrder.CardName = outlet.Field2; //CustomerName;

                oOrder.NumAtCard = doc.ExternalRef; //txtReference.Text;
                oOrder.HandWritten = BoYesNoEnum.tYES; //chkManual order reference generation

                oOrder.DocNum = GetDocNum(); //document number;
                oOrder.DocDate = doc.OrderDate; // DatePosting.Value;
                oOrder.DocDueDate = doc.OrderDueDate; // DatePosting.Value;

               
                foreach (var lineItem in doc.LineItems)
                {

                    var warehouse = doc.SalesmanCode;
                    oOrder.Lines.ItemCode = lineItem.ProductCode;
                  //  oOrder.Lines.ItemDescription = lineItem.ProductDescription;
                    oOrder.Lines.Quantity = decimal.ToDouble(lineItem.Quantity);
                    oOrder.Lines.TaxCode = lineItem.VatClass;
                    oOrder.Lines.UnitPrice =decimal.ToDouble( lineItem.Price);
                    oOrder.Lines.UseBaseUnits = BoYesNoEnum.tYES;
                    oOrder.Lines.TaxTotal = decimal.ToDouble(lineItem.VatPerUnit * lineItem.Quantity);
                   
                    
                        oOrder.Lines.WarehouseCode = warehouse;

                    oOrder.Lines.Add();
                }

              var  lRetCode = oOrder.Add(); // Try to add the orer to the database
                oOrder = null;
                int lErrCode = 0;
                string sErrMsg = "";
                if (lRetCode != 0)
                {
                    int temp_int = lErrCode;
                    string temp_string = sErrMsg;
                    Company.GetLastError(out temp_int, out temp_string);
                    if (lErrCode != -4006) // Incase adding an order failed
                    {
                        //MessageBox.Show(lErrCode + " " + temp_string); // Display error message
                        return new SyncBasicResponse { Status = false, Info = lErrCode + " " + temp_string };
                    }
                    else // If the currency Exchange is not set
                    {
                        double dCur;
                        object sCur;
                        sCur = 1.0;
                        if (double.TryParse(sCur.ToString(), out dCur))
                        {
                            dCur = Convert.ToDouble(sCur);
                            var oBob = (SBObob)Company.GetBusinessObject(BoObjectTypes.BoBridge);
                            //Update Currency rate
                            //oBob.SetCurrencyRate(priceObject.Field2, DateTime.Today, dCur, false);
                        }
                        else
                        {
                        return new SyncBasicResponse{Status = false,Info = "Invalid Value to Currency Exchange"};
                        }
                    }
                }
                _sapWebProxy.MarkOrderAsExported(doc.ExternalRef);
                return new SyncBasicResponse { Status = true, Info = string.Format("Order {0} inserted to SAP successfully", doc.ExternalRef) };
            }
            return new SyncBasicResponse { Status = false, Info = string.Format("Order {0} exist in SAP ", doc.ExternalRef) };
        }

        private bool OrderRefExist(string orderRef)
        {
            // Create the next Order number
            string sSQL = string.Format("SELECT top 1 numatcard FROM dbo.ORDR where numatcard='{0}' ORDER BY numatcard DESC", orderRef);
            var rs = (Recordset)Company.GetBusinessObject(BoObjectTypes.BoRecordset);
            rs.DoQuery(sSQL);
            bool exist = false;
            while (!(rs.EoF))
            {
                var item = rs.Fields.Item(0).Value;
                if (item != null && item.Equals(orderRef, StringComparison.CurrentCultureIgnoreCase))
                {
                    exist = true;
                    break;
                }
                rs.MoveNext();
            }
            return exist;
        }
        private int GetDocNum()
        {
            // Create the next Order number
            string sSQL = "SELECT TOP 1 DocNum FROM dbo.ORDR ORDER BY DocNum DESC";
            var rs = (Recordset)Company.GetBusinessObject(BoObjectTypes.BoRecordset);
            rs.DoQuery(sSQL);
            int number = 0;
            while (!(rs.EoF))
            {
                number = System.Convert.ToInt32(rs.Fields.Item(0).Value) + 1;
                rs.MoveNext();
            }
            return number;
        }
      
    }
}
