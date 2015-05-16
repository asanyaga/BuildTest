using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IIntegrationDocumentRepository;
using Distributr.Core.Utility;
using Distributr.WSAPI.Lib.Integrations;
using Distributr_Middleware.WPF.Lib.MiddlewareServices.Impl;
using Distributr_Middleware.WPF.Lib.Utils;
using QBFC13Lib;
using log4net;

namespace Integration.QuickBooks.Lib.QBIntegrationCore
{
    internal static class QBFC_Core
    {
        internal static string qdbpath = ConfigurationSettings.AppSettings["QuickBooksCompanyFilePath"];
        private static string _appName = "Distributr QB Intergration";
        private static ILog _logger = LogManager.GetLogger("DistributorQBIntegrationCoreLog");

        internal static IItemInventoryRet QBAddInventory(string inventoryName, string inventoryDescription, double inventorySalePrice, string cogsAccountRef, string assetAccountRef, string incomeAccountRef)
        {
            bool boolSessionBegun = false;
            QBSessionManager sessionManager = new QBSessionManager();
            try
            {
                IMsgSetRequest requestMsgSet;
                IMsgSetResponse responseMsgSet;
                sessionManager.OpenConnection("", _appName);
                sessionManager.BeginSession(qdbpath, ENOpenMode.omDontCare);
                boolSessionBegun = true;
                requestMsgSet = GetLatestMsgSetRequest(sessionManager);
                requestMsgSet.Attributes.OnError = ENRqOnError.roeContinue;
               
                string errecid = "{" + Guid.NewGuid().ToString() + "}";
                sessionManager.ErrorRecoveryID.SetValue(errecid);

                sessionManager.EnableErrorRecovery = true;

                sessionManager.SaveAllMsgSetRequestInfo = true;

                #region error recovery

                if (sessionManager.IsErrorRecoveryInfo())
                {
                    IMsgSetRequest reqMsgSet = null;
                    IMsgSetResponse resMsgSet = null;
                    resMsgSet = sessionManager.GetErrorRecoveryStatus();
                    if (resMsgSet.Attributes.MessageSetStatusCode.Equals("600"))
                    {
                        MessageBox.Show(
                            "The oldMessageSetID does not match any stored IDs, and no newMessageSetID is provided.");
                    }
                    else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9001"))
                    {
                        MessageBox.Show(
                            "Invalid checksum. The newMessageSetID specified, matches the currently stored ID, but checksum fails.");
                    }
                    else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9002"))
                    {
                        MessageBox.Show("No stored response was found.");
                    }
                    else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9004"))
                    {
                        MessageBox.Show("Invalid MessageSetID, greater than 24 character was given.");
                    }
                    else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9005"))
                    {
                        MessageBox.Show("Unable to store response.");
                    }
                    else
                    {
                        IResponse res = resMsgSet.ResponseList.GetAt(0);
                        int sCode = res.StatusCode;
                        if (sCode == 0)
                        {
                            MessageBox.Show("Last request was processed and customer was added successfully!");
                        }
                        else if (sCode > 0)
                        {
                            MessageBox.Show("There was a warning but last request was processed successfully!");
                        }
                        else
                        {
                            MessageBox.Show("It seems that there was an error in processing last request");
                            reqMsgSet = sessionManager.GetSavedMsgSetRequest();
                            resMsgSet = sessionManager.DoRequests(reqMsgSet);
                            IResponse resp = resMsgSet.ResponseList.GetAt(0);
                            int statCode = resp.StatusCode;
                            ICustomerRet custRet = null;
                            if (statCode == 0)
                            {
                                string resStr = null;
                                custRet = resp.Detail as ICustomerRet;
                                resStr = resStr + "Following inventory has been successfully submitted to QuickBooks:\n\n\n";
                                if (custRet.ListID != null)
                                {
                                    resStr = resStr + "ListID Number = " + Convert.ToString(custRet.ListID.GetValue()) +
                                             "\n";
                                    Log(resStr);
                                }
                            }
                            Log(QBCRUDEAction.ErrorRecovery, "Customer",
                                             (custRet == null ? "" : custRet.FullName.GetValue()),
                                             resp);
                        }
                    }

                    sessionManager.ClearErrorRecovery();
                    //MessageBox.Show("Proceeding with current transaction.");
                }

                #endregion

                IItemInventoryRet existingItemInventory = GetInventoryByNameFilter(inventoryName);
                if (existingItemInventory == null)
                {
                    IItemInventoryAdd itemInventoryAddRq = requestMsgSet.AppendItemInventoryAddRq();
                    itemInventoryAddRq.Name.SetValue(inventoryName); //Product name
                    itemInventoryAddRq.SalesDesc.SetValue(inventoryDescription);
                    itemInventoryAddRq.SalesPrice.SetValue(inventorySalePrice);
                    itemInventoryAddRq.IncomeAccountRef.FullName.SetValue(incomeAccountRef); //Sales
                    //itemInventoryAddRq.PurchaseDesc.SetValue(purchaseDecription); //Product name
                    //itemInventoryAddRq.PurchaseCost.SetValue(purchaseCost); //Product Price
                    itemInventoryAddRq.COGSAccountRef.FullName.SetValue(cogsAccountRef); //"Sales"
                    itemInventoryAddRq.AssetAccountRef.FullName.SetValue(assetAccountRef); //"Sales"
                    
                    responseMsgSet = sessionManager.DoRequests(requestMsgSet);

                    IResponse response = responseMsgSet.ResponseList.GetAt(0);
                    IItemInventoryRet itemInventoryRet = (IItemInventoryRet)response.Detail;
                    int statusCode = response.StatusCode;
                    if (statusCode == 0)
                    {
                        Console.Write("success adding inventory");
                    }
                    else if (statusCode == 3100)
                    {
                        Console.Write("inventory exists, use update");
                    }
                    Log(QBCRUDEAction.Add, "Inventory",
                                     (itemInventoryRet == null ? "" : itemInventoryRet.FullName.GetValue()), response);
                    return itemInventoryRet;
                }
                else
                {
                    IItemInventoryMod itemInventoryModRq = requestMsgSet.AppendItemInventoryModRq();
                    itemInventoryModRq.Name.SetValue(inventoryName);
                    itemInventoryModRq.SalesDesc.SetValue(inventoryDescription);
                    itemInventoryModRq.SalesPrice.SetValue(inventorySalePrice);
                    itemInventoryModRq.ListID.SetValue(existingItemInventory.ListID.GetValue());
                    itemInventoryModRq.EditSequence.SetValue(existingItemInventory.EditSequence.GetValue());
                    
                    responseMsgSet = sessionManager.DoRequests(requestMsgSet);
                    IResponse response = responseMsgSet.ResponseList.GetAt(0);
                    IItemInventoryRet itemInventoryRet = (IItemInventoryRet)response.Detail;
                    int statusCode = response.StatusCode;
                    if (statusCode == 0)
                    {
                        Console.WriteLine("success adding inventory");
                    }
                    else if (statusCode == 3100)
                    {
                        Console.WriteLine("inventory exists, use update");
                    }
                    Log(QBCRUDEAction.Update, "Inventory",
                                     (itemInventoryRet == null ? "" : itemInventoryRet.FullName.GetValue()), response);
                    sessionManager.ClearErrorRecovery();
                    sessionManager.EndSession();
                    boolSessionBegun = false;
                    sessionManager.CloseConnection();
                    return itemInventoryRet;
                }
            }
            catch (Exception ex)
            {
                if (boolSessionBegun)
                {
                    sessionManager.EndSession();
                    sessionManager.CloseConnection();
                }
                string error = (ex.Message.ToString() + "\nStack Trace: \n" + ex.StackTrace + "\nExiting the application");
                Log(error);
                MessageBox.Show(error);
            }
            return null;
        }

        internal static string GetStockSiteBySalesperson(string salesmanCode)
        {
            var stockSiteRef = string.Empty;

             
            bool boolSessionBegun = false;
            QBSessionManager sessionManager = new QBSessionManager();
            try
            {
                IMsgSetRequest requestMsgSet;
                IMsgSetResponse responseMsgSet;
                sessionManager.OpenConnection("", _appName);
                sessionManager.BeginSession(qdbpath, ENOpenMode.omDontCare);
                boolSessionBegun = true;
                requestMsgSet = GetLatestMsgSetRequest(sessionManager);
                requestMsgSet.Attributes.OnError = ENRqOnError.roeContinue;

                string errecid = "{" + Guid.NewGuid().ToString() + "}";
                sessionManager.ErrorRecoveryID.SetValue(errecid);

                sessionManager.EnableErrorRecovery = true;

                sessionManager.SaveAllMsgSetRequestInfo = true;

                #region error recovery

                if (sessionManager.IsErrorRecoveryInfo())
                {
                    IMsgSetRequest reqMsgSet = null;
                    IMsgSetResponse resMsgSet = null;
                    resMsgSet = sessionManager.GetErrorRecoveryStatus();
                    if (resMsgSet.Attributes.MessageSetStatusCode.Equals("600"))
                    {
                        MessageBox.Show(
                            "The oldMessageSetID does not match any stored IDs, and no newMessageSetID is provided.");
                    }
                    else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9001"))
                    {
                        MessageBox.Show(
                            "Invalid checksum. The newMessageSetID specified, matches the currently stored ID, but checksum fails.");
                    }
                    else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9002"))
                    {
                        MessageBox.Show("No stored response was found.");
                    }
                    else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9004"))
                    {
                        MessageBox.Show("Invalid MessageSetID, greater than 24 character was given.");
                    }
                    else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9005"))
                    {
                        MessageBox.Show("Unable to store response.");
                    }
                    else
                    {
                        IResponse res = resMsgSet.ResponseList.GetAt(0);
                        int sCode = res.StatusCode;
                        if (sCode == 0)
                        {
                            MessageBox.Show("Last request was processed and customer was added successfully!");
                        }
                        else if (sCode > 0)
                        {
                            MessageBox.Show("There was a warning but last request was processed successfully!");
                        }
                        else
                        {
                            MessageBox.Show("It seems that there was an error in processing last request");
                            reqMsgSet = sessionManager.GetSavedMsgSetRequest();
                            resMsgSet = sessionManager.DoRequests(reqMsgSet);
                            IResponse resp = resMsgSet.ResponseList.GetAt(0);
                            int statCode = resp.StatusCode;
                            if (statCode == 0)
                            {
                                string resStr = null;
                                ISalesOrderRet custRet = resp.Detail as ISalesOrderRet;
                                resStr = resStr +
                                         "Following sale/order has been successfully submitted to QuickBooks:\n\n\n";
                                if (custRet.TxnID != null)
                                {
                                    resStr = resStr + "ListID Number = " + Convert.ToString(custRet.TxnID.GetValue()) +
                                             "\n";
                                    Log(resStr);
                                }
                            }
                        }
                    }

                    sessionManager.ClearErrorRecovery();
                    //MessageBox.Show("Proceeding with current transaction.");
                }

                #endregion

                IInventorySiteQuery inventorySiteQuery = requestMsgSet.AppendInventorySiteQueryRq();

                responseMsgSet = sessionManager.DoRequests(requestMsgSet);
                IResponse responseList = responseMsgSet.ResponseList.GetAt(0);

                IInventorySiteRetList inventorySiteRet = (IInventorySiteRetList) responseList.Detail;

                var count = inventorySiteRet.Count;
                for (int i = 0; i < inventorySiteRet.Count; i++)
                {
                    var iterationpoint = i;
                    var siteRefContact = inventorySiteRet.GetAt(i).Contact != null
                                             ? inventorySiteRet.GetAt(i).Contact.GetValue()
                                             : null;

                    
                    
                    if (siteRefContact!=null )
                    {
                        var splitContacts = siteRefContact.Split(',');
                        if (splitContacts.Contains(salesmanCode))
                        {
                            return   inventorySiteRet.GetAt(i).Name.GetValue();
                           
                        }
                        
                        
                    }
                }
            }
            catch (Exception ex)
            {
                if (boolSessionBegun)
                {
                    sessionManager.EndSession();
                    sessionManager.CloseConnection();
                }
                string error = (ex.Message.ToString() + "\nStack Trace: \n" + ex.StackTrace + "\nExiting the application");
                Log(error);
                throw new Exception(ex.Message);
            }

            return stockSiteRef;
        }

        internal static ISalesOrderRet QBAddSalesOrder( QuickBooksOrderDocumentDto orderDoc, string externalOrderRef)
        {
            if (orderDoc.DocumentType != DocumentType.Order)
                throw new ArgumentException("document is not an an order");

            var stockSiteRef = GetStockSiteBySalesperson(orderDoc.SalesmanCode);
            if (string.IsNullOrEmpty(stockSiteRef))
            {
                stockSiteRef = "Unspecified Site";
            }

            
            bool boolSessionBegun = false;
            QBSessionManager sessionManager = new QBSessionManager();
            try
            {
                IMsgSetRequest requestMsgSet;
                IMsgSetResponse responseMsgSet;
                sessionManager.OpenConnection("", _appName);
                sessionManager.BeginSession(qdbpath, ENOpenMode.omDontCare);
                boolSessionBegun = true;
                requestMsgSet = GetLatestMsgSetRequest(sessionManager);
                requestMsgSet.Attributes.OnError = ENRqOnError.roeContinue;

                string errecid = "{" + Guid.NewGuid().ToString() + "}";
                sessionManager.ErrorRecoveryID.SetValue(errecid);

                sessionManager.EnableErrorRecovery = true;

                sessionManager.SaveAllMsgSetRequestInfo = true;

                #region error recovery

                if (sessionManager.IsErrorRecoveryInfo())
                {
                    IMsgSetRequest reqMsgSet = null;
                    IMsgSetResponse resMsgSet = null;
                    resMsgSet = sessionManager.GetErrorRecoveryStatus();
                    if (resMsgSet.Attributes.MessageSetStatusCode.Equals("600"))
                    {
                        MessageBox.Show(
                            "The oldMessageSetID does not match any stored IDs, and no newMessageSetID is provided.");
                    }
                    else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9001"))
                    {
                        MessageBox.Show(
                            "Invalid checksum. The newMessageSetID specified, matches the currently stored ID, but checksum fails.");
                    }
                    else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9002"))
                    {
                        MessageBox.Show("No stored response was found.");
                    }
                    else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9004"))
                    {
                        MessageBox.Show("Invalid MessageSetID, greater than 24 character was given.");
                    }
                    else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9005"))
                    {
                        MessageBox.Show("Unable to store response.");
                    }
                    else
                    {
                        IResponse res = resMsgSet.ResponseList.GetAt(0);
                        int sCode = res.StatusCode;
                        if (sCode == 0)
                        {
                            MessageBox.Show("Last request was processed and customer was added successfully!");
                        }
                        else if (sCode > 0)
                        {
                            MessageBox.Show("There was a warning but last request was processed successfully!");
                        }
                        else
                        {
                            MessageBox.Show("It seems that there was an error in processing last request");
                            reqMsgSet = sessionManager.GetSavedMsgSetRequest();
                            resMsgSet = sessionManager.DoRequests(reqMsgSet);
                            IResponse resp = resMsgSet.ResponseList.GetAt(0);
                            int statCode = resp.StatusCode;
                            if (statCode == 0)
                            {
                                string resStr = null;
                                ISalesOrderRet custRet = resp.Detail as ISalesOrderRet;
                                resStr = resStr + "Following sale/order has been successfully submitted to QuickBooks:\n\n\n";
                                if (custRet.TxnID != null)
                                {
                                    resStr = resStr + "ListID Number = " + Convert.ToString(custRet.TxnID.GetValue()) +
                                             "\n";
                                    Log(resStr);
                                }
                            }
                        }
                    }

                    sessionManager.ClearErrorRecovery();
                    //MessageBox.Show("Proceeding with current transaction.");
                }

                #endregion

                ISalesOrderAdd saleOrderAddRq = requestMsgSet.AppendSalesOrderAddRq();
               

                
                //Set field value for MatchCriterion
                //inventorySiteQuery.ORInventorySiteQuery.InventorySiteFilter.ORNameFilter.NameFilter.MatchCriterion.SetValue(ENMatchCriterion.mcContains);
                ////Set field value for Name
                //inventorySiteQuery.ORInventorySiteQuery.InventorySiteFilter.ORNameFilter.NameFilter.Name.SetValue(orderDoc.SalesmanName);

                //var value=inventorySiteQuery.IncludeRetElementList.Count.ToString();
                //var counted = inventorySiteQuery.ORInventorySiteQuery.FullNameList.Count.ToString();

               // IInventorySiteRetList list=requestMsgSet.appendinventorys

                saleOrderAddRq.CustomerRef.FullName.SetValue(orderDoc.OutletName);
                saleOrderAddRq.DueDate.SetValue(Convert.ToDateTime(orderDoc.OrderDateRequired));
                saleOrderAddRq.Memo.SetValue(orderDoc.Note);
                saleOrderAddRq.RefNumber.SetValue(externalOrderRef);

                
                //Get the Stock Site to be used to populate the inventory Site Ref
               
                

                saleOrderAddRq.TxnDate.SetValue(Convert.ToDateTime(orderDoc.DocumentDateIssued));
                

                foreach (var lineItem in orderDoc.LineItems)
                {
                    IItemInventoryRet product = GetProductByCode(lineItem.ProductCode);
                    if (product == null) continue;
                    
                    ISalesOrderLineAdd saleOrderLineAddRq = saleOrderAddRq.ORSalesOrderLineAddList.Append().SalesOrderLineAdd;
                    saleOrderLineAddRq.ItemRef.FullName.SetValue(product.FullName.GetValue());
                    saleOrderLineAddRq.Quantity.SetValue(Convert.ToDouble(lineItem.Quantity));
                    saleOrderLineAddRq.Amount.SetValue(Convert.ToDouble(Math.Round(lineItem.TotalNet,2)));
                   
                    //saleOrderLineAddRq
                   
                    if(!string.IsNullOrEmpty(lineItem.VATClass))
                    {
                        saleOrderLineAddRq.SalesTaxCodeRef.FullName.SetValue(lineItem.VATClass);
                        saleOrderLineAddRq.TaxAmount.SetValue(Convert.ToDouble(Math.Round(lineItem.TotalVat, 2)));
                    }
                    
                    //saleOrderLineAddRq.InventorySiteRef.FullName.SetValue("ELDORET");

                    saleOrderLineAddRq.InventorySiteRef.FullName.SetValue(stockSiteRef);
                    
                    //This feature is not supported in the specified version of qbXML.
                    
                    
                    //catch (Exception EX_NAME) {
                    //    Console.WriteLine(EX_NAME);
                    //    Log(EX_NAME.Message);
                    //}
                    saleOrderLineAddRq.Desc.SetValue(lineItem.ProductDescription);
                    saleOrderLineAddRq.ORRatePriceLevel.Rate.SetValue(Convert.ToDouble(lineItem.LineItemValue));
                }

                responseMsgSet = sessionManager.DoRequests(requestMsgSet);
                IResponse response = responseMsgSet.ResponseList.GetAt(0);
               
                    

                int statusCode = response.StatusCode;
                ISalesOrderRet salesOrderRet = response.Detail as ISalesOrderRet;
                if (statusCode == 0)
                {
                    Console.WriteLine("Success");
                }
                else
                {
                    MessageBox.Show(response.StatusMessage);
                }
                
                sessionManager.ClearErrorRecovery();
                sessionManager.EndSession();
                boolSessionBegun = false;
                sessionManager.CloseConnection();
                Log(QBCRUDEAction.Add, "SalesOrder", (salesOrderRet == null ? "" : salesOrderRet.RefNumber.GetValue()), response);
                return salesOrderRet;
            }
            catch (Exception ex)
            {
                if (boolSessionBegun)
                {
                    //sessionManager.EndSession();
                    //sessionManager.CloseConnection();
                }
                string error = (ex.Message.ToString() + "\nStack Trace: \n" + ex.StackTrace + "\nExiting the application");
                Log(error);
                throw new Exception(ex.Message);
            }
            return null;
        }

        internal static IInvoiceRet QBAddInvoice(QuickBooksOrderDocumentDto invoice, string qbOrderTransactionId, string customerName, string externalInvoiceRef,string account, string memo = "", string templateRefName = "Custom Sales Receipt" )
        
        {
            if(invoice.DocumentType !=DocumentType.Invoice)
                throw new ArgumentException("document is not an invoice");
            bool boolSessionBegun = false;
            QBSessionManager sessionManager = new QBSessionManager();
            try
            {
                IMsgSetRequest requestMsgSet;
                IMsgSetResponse responseMsgSet;
                sessionManager.OpenConnection("", _appName);
                sessionManager.BeginSession(qdbpath, ENOpenMode.omDontCare);
                boolSessionBegun = true;
                requestMsgSet = GetLatestMsgSetRequest(sessionManager);
                requestMsgSet.Attributes.OnError = ENRqOnError.roeContinue;

                string errecid = "{" + Guid.NewGuid().ToString() + "}";
                sessionManager.ErrorRecoveryID.SetValue(errecid);

                sessionManager.EnableErrorRecovery = true;

                sessionManager.SaveAllMsgSetRequestInfo = true;

                #region error recovery

                if (sessionManager.IsErrorRecoveryInfo())
                {
                    IMsgSetRequest reqMsgSet = null;
                    IMsgSetResponse resMsgSet = null;
                    resMsgSet = sessionManager.GetErrorRecoveryStatus();
                    if (resMsgSet.Attributes.MessageSetStatusCode.Equals("600"))
                    {
                        MessageBox.Show(
                            "The oldMessageSetID does not match any stored IDs, and no newMessageSetID is provided.");
                    }
                    else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9001"))
                    {
                        MessageBox.Show(
                            "Invalid checksum. The newMessageSetID specified, matches the currently stored ID, but checksum fails.");
                    }
                    else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9002"))
                    {
                        MessageBox.Show("No stored response was found.");
                    }
                    else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9004"))
                    {
                        MessageBox.Show("Invalid MessageSetID, greater than 24 character was given.");
                    }
                    else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9005"))
                    {
                        MessageBox.Show("Unable to store response.");
                    }
                    else
                    {
                        IResponse res = resMsgSet.ResponseList.GetAt(0);
                        int sCode = res.StatusCode;
                        if (sCode == 0)
                        {
                            MessageBox.Show("Last request was processed and customer was added successfully!");
                        }
                        else if (sCode > 0)
                        {
                            MessageBox.Show("There was a warning but last request was processed successfully!");
                        }
                        else
                        {
                            MessageBox.Show("It seems that there was an error in processing last request");
                            reqMsgSet = sessionManager.GetSavedMsgSetRequest();
                            resMsgSet = sessionManager.DoRequests(reqMsgSet);
                            IResponse resp = resMsgSet.ResponseList.GetAt(0);
                            int statCode = resp.StatusCode;
                            if (statCode == 0)
                            {
                                string resStr = null;
                                IInvoiceRet custRet = resp.Detail as IInvoiceRet;
                                resStr = resStr +
                                         "Following customer has been successfully submitted to QuickBooks:\n\n\n";
                                if (custRet.TxnID != null)
                                {
                                    resStr = resStr + "ListID Number = " + Convert.ToString(custRet.TxnID.GetValue()) +
                                             "\n";
                                }
                                Log(QBCRUDEAction.ErrorRecovery, "Invoice", custRet.RefNumber.GetValue(), resp);

                            }
                        }
                    }

                    sessionManager.ClearErrorRecovery();
                    //MessageBox.Show("Proceeding with current transaction.");
                }

                #endregion

                IInvoiceAdd invoiceAddRq = requestMsgSet.AppendInvoiceAddRq();
                invoiceAddRq.CustomerRef.FullName.SetValue(customerName);
                invoiceAddRq.SalesRepRef.FullName.SetValue(invoice.SalesmanCode);
                
                invoiceAddRq.TxnDate.SetValue(Convert.ToDateTime(invoice.DocumentDateIssued));
                invoiceAddRq.RefNumber.SetValue(invoice.GenericReference.Substring((invoice.GenericReference.Length - 11), 11));//Check for duplicates
                //invoiceAddRq.RefNumber.SetValue(externalInvoiceRef);
                invoiceAddRq.LinkToTxnIDList.Add(qbOrderTransactionId);
                //invoiceAddRq.IsTaxIncluded.SetValue(true);

                if (!string.IsNullOrEmpty(account))
                {
                    invoiceAddRq.ARAccountRef.ListID.SetValue(account);
                }
                
         /* This will be used to reduce the stock from the stock site
                //invoiceAddRq.SalesRepRef.ListID.SetValue();
                //invoiceAddRq.SalesRepRef.FullName.SetValue();

         This is the end*/

                foreach (var invoiceLineItem in invoice.LineItems)
                {
                    //IItemInventoryRet product = GetProductByCode(invoiceLineItem.ProductCode);

                    //if (product == null) continue;
                    //IInvoiceLineAdd salesReceiptLineAddRq = invoiceAddRq.ORInvoiceLineAddList.Append().InvoiceLineAdd;
                   // salesReceiptLineAddRq.InventorySiteRef=
                    //salesReceiptLineAddRq.ORRatePriceLevel.Rate.SetValue(Convert.ToDouble(invoiceLineItem.LineItemValue));
                    ////salesReceiptLineAddRq.Quantity.SetValue(1);//invoiceLineItem.Qtly
                    ////salesReceiptLineAddRq.Quantity.SetValue(Convert.ToDouble(invoiceLineItem.Quantity)); Check for duplicates
                    //salesReceiptLineAddRq.Amount.SetValue(Convert.ToDouble(Math.Round(invoiceLineItem.GrossValue,2)));
                    //salesReceiptLineAddRq.ItemRef.FullName.SetValue(product.FullName.GetValue());
                    //salesReceiptLineAddRq.Desc.SetValue(invoiceLineItem.ProductDescription);
                    //try
                    //{
                    //    salesReceiptLineAddRq.TaxAmount.SetValue(Convert.ToDouble(Math.Round(invoiceLineItem.TotalVat,2)));
                    //}
                    //catch (Exception e) { Console.WriteLine(e); Log(e.Message); }
                }
                responseMsgSet = sessionManager.DoRequests(requestMsgSet);
                IResponse response = responseMsgSet.ResponseList.GetAt(0);
                IInvoiceRet invoiceRet = response.Detail as IInvoiceRet;
                int statusCode = response.StatusCode; if (statusCode == 0)
                {
                    Console.WriteLine("Success");
                }
                sessionManager.ClearErrorRecovery();
                sessionManager.EndSession();
                boolSessionBegun = false;
                sessionManager.CloseConnection();
                Log(QBCRUDEAction.Add, "Invoice", invoiceRet.RefNumber.GetValue(), response);
                return invoiceRet;
            }
            catch (Exception ex)
            {
                if (boolSessionBegun)
                {
                    sessionManager.EndSession();
                    sessionManager.CloseConnection();
                }
                string error = (ex.Message.ToString() + "\nStack Trace: \n" + ex.StackTrace + "\nExiting the application");
                Log(error);
                throw new Exception(ex.Message);

            }
            return null;
        }

        internal static ISalesReceiptRet QBAddSalesReceipt(QuickBooksOrderDocumentDto receipt, string memo = "", string templateRefName = "Custom Sales Receipt")
        {
            if (receipt.DocumentType != DocumentType.Receipt)
                throw new ArgumentException("document is not an an Receipt");
            bool boolSessionBegun = false;
            QBSessionManager sessionManager = new QBSessionManager();
            try
            {
                IMsgSetRequest requestMsgSet;
                IMsgSetResponse responseMsgSet;
                sessionManager.OpenConnection("", _appName);
                sessionManager.BeginSession(qdbpath, ENOpenMode.omDontCare);
                boolSessionBegun = true;
                requestMsgSet = GetLatestMsgSetRequest(sessionManager);
                requestMsgSet.Attributes.OnError = ENRqOnError.roeContinue;

                string errecid = "{" + Guid.NewGuid().ToString() + "}";
                sessionManager.ErrorRecoveryID.SetValue(errecid);

                sessionManager.EnableErrorRecovery = true;

                sessionManager.SaveAllMsgSetRequestInfo = true;

                #region error recovery

                if (sessionManager.IsErrorRecoveryInfo())
                {
                    IMsgSetRequest reqMsgSet = null;
                    IMsgSetResponse resMsgSet = null;
                    resMsgSet = sessionManager.GetErrorRecoveryStatus();
                    if (resMsgSet.Attributes.MessageSetStatusCode.Equals("600"))
                    {
                        MessageBox.Show(
                            "The oldMessageSetID does not match any stored IDs, and no newMessageSetID is provided.");
                    }
                    else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9001"))
                    {
                        MessageBox.Show(
                            "Invalid checksum. The newMessageSetID specified, matches the currently stored ID, but checksum fails.");
                    }
                    else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9002"))
                    {
                        MessageBox.Show("No stored response was found.");
                    }
                    else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9004"))
                    {
                        MessageBox.Show("Invalid MessageSetID, greater than 24 character was given.");
                    }
                    else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9005"))
                    {
                        MessageBox.Show("Unable to store response.");
                    }
                    else
                    {
                        IResponse res = resMsgSet.ResponseList.GetAt(0);
                        int sCode = res.StatusCode;
                        if (sCode == 0)
                        {
                            MessageBox.Show("Last request was processed and customer was added successfully!");
                        }
                        else if (sCode > 0)
                        {
                            MessageBox.Show("There was a warning but last request was processed successfully!");
                        }
                        else
                        {
                            MessageBox.Show("It seems that there was an error in processing last request");
                            reqMsgSet = sessionManager.GetSavedMsgSetRequest();
                            resMsgSet = sessionManager.DoRequests(reqMsgSet);
                            IResponse resp = resMsgSet.ResponseList.GetAt(0);
                            int statCode = resp.StatusCode;
                            if (statCode == 0)
                            {
                                string resStr = null;
                                ISalesReceiptRet custRet = resp.Detail as ISalesReceiptRet;
                                resStr = resStr +
                                         "Following customer has been successfully submitted to QuickBooks:\n\n\n";
                                if (custRet.TxnID != null)
                                {
                                    resStr = resStr + "ListID Number = " + Convert.ToString(custRet.TxnID.GetValue()) +
                                             "\n";
                                }
                                Log(QBCRUDEAction.ErrorRecovery, "Invoice", (custRet == null ? "" : custRet.RefNumber.GetValue()), resp);
                            }
                        }
                    }

                    sessionManager.ClearErrorRecovery();
                    //MessageBox.Show("Proceeding with current transaction.");
                }

                #endregion

                ISalesReceiptAdd salesReceiptAddRq = requestMsgSet.AppendSalesReceiptAddRq();
                salesReceiptAddRq.CustomerRef.FullName.SetValue(receipt.OutletName);
                //salesReceiptAddRq.TemplateRef.FullName.SetValue(templateRefName);//Custom Sales Receipt
                salesReceiptAddRq.Memo.SetValue(memo);
                salesReceiptAddRq.TxnDate.SetValue(Convert.ToDateTime(receipt.DocumentDateIssued));
                salesReceiptAddRq.RefNumber.SetValue(receipt.GenericReference.Substring((receipt.GenericReference.Length - 11), 11));

                foreach (var receiptLineItem in receipt.LineItems)
                {
                    IItemInventoryRet product = GetProductByCode(receiptLineItem.ProductCode);
                    if (product == null) continue;
                  
                    ISalesReceiptLineAdd salesReceiptLineAddRq = salesReceiptAddRq.ORSalesReceiptLineAddList.Append().SalesReceiptLineAdd;
                    salesReceiptLineAddRq.Amount.SetValue(Convert.ToDouble(Math.Round(receiptLineItem.GrossValue,2)));
                    //salesReceiptLineAddRq.Quantity.SetValue(1);
                    salesReceiptLineAddRq.ORRatePriceLevel.Rate.SetValue(Convert.ToDouble(receiptLineItem.LineItemValue));
                    salesReceiptLineAddRq.Quantity.SetValue(Convert.ToDouble(receiptLineItem.Quantity));
                    salesReceiptLineAddRq.ItemRef.FullName.SetValue(product.FullName.GetValue());
                    salesReceiptLineAddRq.Desc.SetValue(receiptLineItem.ProductDescription);
                    //salesReceiptLineAddRq.
                    salesReceiptLineAddRq.Other1.SetValue(receiptLineItem.PaymentType);
                    try
                    {
                        salesReceiptLineAddRq.TaxAmount.SetValue(Convert.ToDouble(Math.Round(receiptLineItem.TotalVat, 2)));
                    }
                    catch (Exception e) { Console.WriteLine(e); Log(e.Message); }
                }
                responseMsgSet = sessionManager.DoRequests(requestMsgSet);
                IResponse response = responseMsgSet.ResponseList.GetAt(0);
                ISalesReceiptRet salesReceipRet = response.Detail as ISalesReceiptRet;
                int statusCode = response.StatusCode; if (statusCode == 0)
                {
                    Console.WriteLine("Success");
                }
                sessionManager.ClearErrorRecovery();
                sessionManager.EndSession();
                boolSessionBegun = false;
                sessionManager.CloseConnection();
                Log(QBCRUDEAction.Add, "SalesReceipt", (salesReceipRet == null ? "" : salesReceipRet.RefNumber.GetValue()), response);
                return salesReceipRet;
            }
            catch (Exception ex)
            {
                if (boolSessionBegun)
                {
                    sessionManager.EndSession();
                    sessionManager.CloseConnection();
                }
                string error = (ex.Message.ToString() + "\nStack Trace: \n" + ex.StackTrace + "\nExiting the application");
                Log(error);
                MessageBox.Show(error);

            }
            return null;
        }

        internal static IReceivePaymentRet QbAddPayment(QuickBooksOrderDocumentDto receipt, string qbInvoiceTxnId, string accountRef, List<string> references)
        {
            if (receipt.DocumentType != DocumentType.Receipt)
                throw new ArgumentException("document is not an an Receipt");
            bool boolSessionBegun = false;
            QBSessionManager sessionManager = new QBSessionManager();
            try
            {
                IMsgSetRequest requestMsgSet;
                IMsgSetResponse responseMsgSet;
                sessionManager.OpenConnection("", _appName);
                sessionManager.BeginSession(qdbpath, ENOpenMode.omDontCare);
                boolSessionBegun = true;
                requestMsgSet = GetLatestMsgSetRequest(sessionManager);
                requestMsgSet.Attributes.OnError = ENRqOnError.roeContinue;

                string errecid = "{" + Guid.NewGuid().ToString() + "}";
                sessionManager.ErrorRecoveryID.SetValue(errecid);

                sessionManager.EnableErrorRecovery = true;

                sessionManager.SaveAllMsgSetRequestInfo = true;

                #region error recovery

                if (sessionManager.IsErrorRecoveryInfo())
                {
                    IMsgSetRequest reqMsgSet = null;
                    IMsgSetResponse resMsgSet = null;
                    resMsgSet = sessionManager.GetErrorRecoveryStatus();
                    if (resMsgSet.Attributes.MessageSetStatusCode.Equals("600"))
                    {
                        MessageBox.Show(
                            "The oldMessageSetID does not match any stored IDs, and no newMessageSetID is provided.");
                    }
                    else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9001"))
                    {
                        MessageBox.Show(
                            "Invalid checksum. The newMessageSetID specified, matches the currently stored ID, but checksum fails.");
                    }
                    else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9002"))
                    {
                        MessageBox.Show("No stored response was found.");
                    }
                    else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9004"))
                    {
                        MessageBox.Show("Invalid MessageSetID, greater than 24 character was given.");
                    }
                    else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9005"))
                    {
                        MessageBox.Show("Unable to store response.");
                    }
                    else
                    {
                        IResponse res = resMsgSet.ResponseList.GetAt(0);
                        int sCode = res.StatusCode;
                        if (sCode == 0)
                        {
                            MessageBox.Show("Last request was processed and customer was added successfully!");
                        }
                        else if (sCode > 0)
                        {
                            MessageBox.Show("There was a warning but last request was processed successfully!");
                        }
                        else
                        {
                            MessageBox.Show("It seems that there was an error in processing last request");
                            reqMsgSet = sessionManager.GetSavedMsgSetRequest();
                            resMsgSet = sessionManager.DoRequests(reqMsgSet);
                            IResponse resp = resMsgSet.ResponseList.GetAt(0);
                            int statCode = resp.StatusCode;
                            if (statCode == 0)
                            {
                                string resStr = null;
                                IReceivePaymentRet custRet = resp.Detail as IReceivePaymentRet;
                                resStr = resStr +
                                         "Following customer has been successfully submitted to QuickBooks:\n\n\n";
                                if (custRet.TxnID != null)
                                {
                                    resStr = resStr + "ListID Number = " + Convert.ToString(custRet.TxnID.GetValue()) +
                                             "\n";
                                }
                                Log(QBCRUDEAction.ErrorRecovery, "Invoice", (custRet == null ? "" : custRet.RefNumber.GetValue()), resp);
                            }
                        }
                    }

                    sessionManager.ClearErrorRecovery();
                    //MessageBox.Show("Proceeding with current transaction.");
                }

                #endregion


                    //var iterator =1;
                    //IItemInventoryRet product = GetProductByCode(receiptLineItem.ProductCode);
                    //if (product == null) continue;

                     IReceivePaymentAdd receivePaymentAddRq = requestMsgSet.AppendReceivePaymentAddRq();
                    
                    receivePaymentAddRq.CustomerRef.FullName.SetValue(receipt.OutletName);
                    receivePaymentAddRq.TxnDate.SetValue(Convert.ToDateTime(receipt.DocumentDateIssued));
                    

                    
                    receivePaymentAddRq.RefNumber.SetValue(receipt.GenericReference.Substring((receipt.GenericReference.Length - 11), 11));
                    
                    var paymentmethod = "Cash";
                    var quickBooksOrderDocLineItem = receipt.LineItems.FirstOrDefault();
                    if (quickBooksOrderDocLineItem != null && !string.IsNullOrEmpty(quickBooksOrderDocLineItem.PaymentType))
                    {
                        paymentmethod = quickBooksOrderDocLineItem.PaymentType;
                    }
                    //var paymentmethod = quickBooksOrderDocLineItem.PaymentType;

                    receivePaymentAddRq.PaymentMethodRef.FullName.SetValue(paymentmethod);
                 
                    
                   
                    receivePaymentAddRq.ARAccountRef.ListID.SetValue(accountRef);
                //decimal total = 0m;
                //foreach (var item in receipt.LineItems)
                //{
                //    total += item.LineItemValue;
                   
                //}
                   var total = receipt.LineItems.Sum(receiptLineItem => receiptLineItem.LineItemValue);

                    
                    receivePaymentAddRq.TotalAmount.SetValue(Convert.ToDouble(Math.Round(total, 2)));

                    receivePaymentAddRq.ORApplyPayment.IsAutoApply.SetValue(true);
                    try
                    {
                       // receivePaymentAddRq.SetValue(Convert.ToDouble(Math.Round(receiptLineItem.TotalVat, 2)));
                    }
                    catch (Exception e) { Console.WriteLine(e); Log(e.Message); }
                
                responseMsgSet = sessionManager.DoRequests(requestMsgSet);
                IResponse response = responseMsgSet.ResponseList.GetAt(0);
                IReceivePaymentRet salesReceipRet = response.Detail as IReceivePaymentRet;
                int statusCode = response.StatusCode; if (statusCode == 0)
                {
                    Console.WriteLine("Success");
                }
                sessionManager.ClearErrorRecovery();
                sessionManager.EndSession();
                boolSessionBegun = false;
                sessionManager.CloseConnection();
                Log(QBCRUDEAction.Add, "SalesReceipt", (salesReceipRet == null ? "" : salesReceipRet.RefNumber.GetValue()), response);
                return salesReceipRet;
            }
            catch (Exception ex)
            {
                if (boolSessionBegun)
                {
                    sessionManager.EndSession();
                    sessionManager.CloseConnection();
                }
                string error = (ex.Message.ToString() + "\nStack Trace: \n" + ex.StackTrace + "\nExiting the application");
                Log(error);
                MessageBox.Show(error);

            }
            return null;
        }

   
         static void VATClassList()
        {
            bool boolSessionBegun = false;
            QBSessionManager sessionManager = new QBSessionManager();
            try
            {
                IMsgSetRequest requestMsgSet;
                IMsgSetResponse responseMsgSet;
                sessionManager.OpenConnection("", _appName);
                sessionManager.BeginSession(qdbpath, ENOpenMode.omDontCare);
                boolSessionBegun = true;
                requestMsgSet = GetLatestMsgSetRequest(sessionManager);
                requestMsgSet.Attributes.OnError = ENRqOnError.roeStop;
                
                //requestMsgSet.AppendItemSalesTaxQueryRq();
                requestMsgSet.AppendSalesTaxCodeQueryRq();

                responseMsgSet = sessionManager.DoRequests(requestMsgSet);
                
                //IResponse response = responseMsgSet.ResponseList.GetAt(0);
               //ISalesTaxCodeRetList taxcodesList = response.Detail as ISalesTaxCodeRetList;
                //IItemSalesTaxRetList taxcodesList = null;
                //ISalesTaxCodeRetList taxescodesList = null;
                //IItemSalesTaxQuery taxQuery = null;

               //for(int k=0;k<responseMsgSet.ResponseList.Count;k++)
               //{
               //    IResponse response = responseMsgSet.ResponseList.GetAt(k);

               //    if (response.StatusCode >= 0)
               //    {


               //        ENResponseType responseType = (ENResponseType) response.Type.GetValue();

               //        if (responseType == ENResponseType.rtItemSalesTaxQueryRs)
               //        {
               //            taxcodesList = response.Detail as IItemSalesTaxRetList;
               //            taxQuery= response.Detail as IItemSalesTaxQuery;

               //            // IItemSalesTaxQuery taxQuery = response.Detail as IItemSalesTaxQuery;
               //        }
               //        if (responseType == ENResponseType.rtSalesTaxCodeQueryRs)
               //        {
               //            taxescodesList = response.Detail as ISalesTaxCodeRetList;
               //        }
               //    }
               //}

                IResponse response = responseMsgSet.ResponseList.GetAt(0);
                //var taxescodesList = response.Detail as IItemSalesTaxRetList;
                ISalesTaxCodeRetList taxescodesList = response.Detail as ISalesTaxCodeRetList;

                    var items = new List<VATImport>();

                           if (taxescodesList != null && taxescodesList.Count > 0)
                           {

                               for (var j = 0; j < taxescodesList.Count; j++)
                               {

                                  // IItemSalesTaxRet taxes = taxescodesList.GetAt(j);
                                   ISalesTaxCodeRet taxes = taxescodesList.GetAt(j);
                                   
                                   if (taxes != null && taxes.IsActive.GetValue())
                                   {
                                       var isTaxable = ((dynamic)taxes).IsTaxable.GetValue();
                                       if (!isTaxable)
                                       {
                                           var date = ((dynamic)taxes).TimeCreated.GetValue();
                                           var vatImport = new VATImport
                                           {
                                               Code = taxes.Name != null ? taxes.Name.GetValue() : "",
                                               Name = taxes.Desc != null ? taxes.Desc.GetValue() : "",
                                               Rate = 0m,
                                               EffectiveDate = date,
                                           };

                                           items.Add(vatImport);
                                       }
                                     

                                       if(taxes.ItemSalesTaxRef!=null && isTaxable )
                                       {
                                           var taxItem = ItemSalesReturn(taxes.ItemSalesTaxRef.ListID.GetValue());//taxes.ItemSalesTaxRef as IItemSalesTaxRet;
                                       
                                       
                                       var code = taxes.Name != null ? taxes.Name.GetValue() : "";
                                       if(taxItem!=null)
                                       {
                                           var vatImport = new VATImport
                                           {
                                               Code = taxes.Name != null ? taxes.Name.GetValue() : "",
                                               Name = taxes.Desc != null ? taxes.Desc.GetValue() : "",
                                               Rate = taxItem.TaxRate != null ? Convert.ToDecimal(taxItem.TaxRate.GetValue()) : 0m,
                                               EffectiveDate = taxItem.TimeCreated.GetValue()
                                           };

                                           items.Add(vatImport);
                                       }
                                      
                                       }

                                      
                                       

                                   }

                               }



                               if (items.Any())
                               {
                                   DumpExportFilesAsync(items.ToCsv(), MasterDataCollective.VatClass.ToString());
                               }
                }
               
                sessionManager.EndSession();
                boolSessionBegun = false;
                sessionManager.CloseConnection();
            }
            catch (Exception ex)
            {
                if (boolSessionBegun)
                {
                    sessionManager.EndSession();
                    sessionManager.CloseConnection();
                }
                string error = (ex.Message.ToString() + "\nStack Trace: \n" + ex.StackTrace + "\nExiting the application");
                Log(error);
                MessageBox.Show(error);

            }
        }

         private static IItemSalesTaxRet ItemSalesReturn(string Id)
         {

             IItemSalesTaxRet firstItem = null;

              bool boolSessionBegun = false;
              QBSessionManager sessionManager = new QBSessionManager();
              try
              {
                  IMsgSetRequest requestMsgSet;
                  IMsgSetResponse responseMsgSet;
                  sessionManager.OpenConnection("", _appName);
                  sessionManager.BeginSession(qdbpath, ENOpenMode.omDontCare);
                  boolSessionBegun = true;
                  requestMsgSet = GetLatestMsgSetRequest(sessionManager);
                  requestMsgSet.Attributes.OnError = ENRqOnError.roeContinue;

                  string errecid = "{" + Guid.NewGuid().ToString() + "}";
                  sessionManager.ErrorRecoveryID.SetValue(errecid);

                  sessionManager.EnableErrorRecovery = true;

                  sessionManager.SaveAllMsgSetRequestInfo = true;


                  requestMsgSet.AppendItemSalesTaxQueryRq();
                  
                  responseMsgSet = sessionManager.DoRequests(requestMsgSet);
                  IResponse responseList = responseMsgSet.ResponseList.GetAt(0);

                  IItemSalesTaxRetList itemSalesTaxRetList = (IItemSalesTaxRetList)responseList.Detail;
                  
                  for (int i = 0; i < itemSalesTaxRetList.Count; i++)
                  {
                      IItemSalesTaxRet item = itemSalesTaxRetList.GetAt(i);
                     
                      if(item.ListID.GetValue()==Id)
                      {
                          firstItem=item;
                          break;
                      }
                      

                  }
                      
                  sessionManager.EndSession();
                  boolSessionBegun = false;
                  sessionManager.CloseConnection();
              }
              catch (Exception ex)
              {
                  if (boolSessionBegun)
                  {
                      sessionManager.EndSession();
                      sessionManager.CloseConnection();
                  }
                  string error = (ex.Message.ToString() + "\nStack Trace: \n" + ex.StackTrace + "\nExiting the application");
                  var message = ex.Message + " ";
                  throw new Exception(message);
              }

             return  firstItem;
          
         }

        static void ProductPackagingAndPackagingType()
         {
             bool boolSessionBegun = false;
             QBSessionManager sessionManager = new QBSessionManager();
             try
             {
                 IMsgSetRequest requestMsgSet;
                 IMsgSetResponse responseMsgSet;
                 sessionManager.OpenConnection("", _appName);
                 sessionManager.BeginSession(qdbpath, ENOpenMode.omDontCare);
                 boolSessionBegun = true;

                 var version = QBFCLatestVersion(sessionManager);
                 short ver = Convert.ToInt16(version);
                 requestMsgSet = GetLatestMsgSetRequest(sessionManager); //sessionManager.CreateMsgSetRequest("US", ver, 0);;
                 requestMsgSet.Attributes.OnError = ENRqOnError.roeStop;
         
                 IUnitOfMeasureSetQuery query = requestMsgSet.AppendUnitOfMeasureSetQueryRq();
                 
                 responseMsgSet = sessionManager.DoRequests(requestMsgSet);

                 List<Packaging> items=new List<Packaging>();

                 IResponse response = responseMsgSet.ResponseList.GetAt(0);
                 IUnitOfMeasureSetRetList unitOfMeasureSet = response.Detail as IUnitOfMeasureSetRetList;
                 
                 if (unitOfMeasureSet != null && unitOfMeasureSet.Count > 0)
                 {

                     for (var i = 0; i < unitOfMeasureSet.Count; i++)
                     {
                         IUnitOfMeasureSetRet unitOfMeasure = unitOfMeasureSet.GetAt(i);
                         if (unitOfMeasure != null && unitOfMeasure.IsActive.GetValue())
                         {
                             var baseunit = unitOfMeasure.BaseUnit;
                             var p = new Packaging()
                                         {
                                             Name = unitOfMeasure.Name != null ? unitOfMeasure.Name.GetValue() : "",
                                             Description = unitOfMeasure.Name != null ? unitOfMeasure.Name.GetValue() : "",
                                         };
                             if(baseunit !=null)
                             {
                                 p.Code = baseunit.Abbreviation != null ? baseunit.Abbreviation.GetValue() : "";
                             }
                             if (string.IsNullOrEmpty(p.Code))
                                 p.Code = p.Name;

                             if (!string.IsNullOrEmpty(p.Name))
                                 items.Add(p);

                         }

                     }


                 }

                 sessionManager.EndSession();
                 boolSessionBegun = false;
                 sessionManager.CloseConnection();
                 if (items.Any())
                 {
                     var files = items.ToCsv();
                     DumpExportFilesAsync(files, MasterDataCollective.ProductPackaging.ToString());
                     DumpExportFilesAsync(files, MasterDataCollective.ProductPackagingType.ToString());
                 }
                 
             }
             catch (Exception ex)
             {
                 if (boolSessionBegun)
                 {
                     sessionManager.EndSession();
                     sessionManager.CloseConnection();
                 }
                 string error = (ex.Message.ToString() + "\nStack Trace: \n" + ex.StackTrace + "\nExiting the application");
                 Log(error);
                 MessageBox.Show(error);

             }

         }

        internal static Dictionary<string, string> GetStockSiteReference()
        {
            var stockSiteCodeMappingList = new Dictionary<string, string>();

            bool boolSessionBegun = false;
            QBSessionManager sessionManager = new QBSessionManager();
            try
            {
                IMsgSetRequest requestMsgSet;
                IMsgSetResponse responseMsgSet;
                sessionManager.OpenConnection("", _appName);
                sessionManager.BeginSession(qdbpath, ENOpenMode.omDontCare);
                boolSessionBegun = true;
                requestMsgSet = GetLatestMsgSetRequest(sessionManager);
                requestMsgSet.Attributes.OnError = ENRqOnError.roeContinue;

                string errecid = "{" + Guid.NewGuid().ToString() + "}";
                sessionManager.ErrorRecoveryID.SetValue(errecid);

                sessionManager.EnableErrorRecovery = true;

                sessionManager.SaveAllMsgSetRequestInfo = true;

                #region error recovery

                if (sessionManager.IsErrorRecoveryInfo())
                {
                    IMsgSetRequest reqMsgSet = null;
                    IMsgSetResponse resMsgSet = null;
                    resMsgSet = sessionManager.GetErrorRecoveryStatus();
                    if (resMsgSet.Attributes.MessageSetStatusCode.Equals("600"))
                    {
                        MessageBox.Show(
                            "The oldMessageSetID does not match any stored IDs, and no newMessageSetID is provided.");
                    }
                    else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9001"))
                    {
                        MessageBox.Show(
                            "Invalid checksum. The newMessageSetID specified, matches the currently stored ID, but checksum fails.");
                    }
                    else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9002"))
                    {
                        MessageBox.Show("No stored response was found.");
                    }
                    else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9004"))
                    {
                        MessageBox.Show("Invalid MessageSetID, greater than 24 character was given.");
                    }
                    else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9005"))
                    {
                        MessageBox.Show("Unable to store response.");
                    }

                    
                    sessionManager.ClearErrorRecovery();
                }

                #endregion

                requestMsgSet.AppendInventorySiteQueryRq();

                responseMsgSet = sessionManager.DoRequests(requestMsgSet);
                IResponse responseList = responseMsgSet.ResponseList.GetAt(0);

                IInventorySiteRetList inventorySiteRet = (IInventorySiteRetList)responseList.Detail;

                for (int i = 0; i < inventorySiteRet.Count; i++)
                {
                    var id = inventorySiteRet.GetAt(i).ListID.GetValue();
                    var name = inventorySiteRet.GetAt(i).Name.GetValue();

                   
                    stockSiteCodeMappingList.Add(id, name);

                }
                //if (inventorySiteRet != null && inventorySiteRet.GetAt(0).Contact != null)
                //{
                //    distributorCode = inventorySiteRet.GetAt(0).Contact.GetValue();

                //    sessionManager.EndSession();
                //    boolSessionBegun = false;
                //    sessionManager.CloseConnection();

                //    return distributorCode;
                //}

                sessionManager.EndSession();
                boolSessionBegun = false;
                sessionManager.CloseConnection();
            }
            catch (Exception ex)
            {
                if (boolSessionBegun)
                {
                    sessionManager.EndSession();
                    sessionManager.CloseConnection();
                }
                string error = (ex.Message.ToString() + "\nStack Trace: \n" + ex.StackTrace + "\nExiting the application");
                Log(error);
                var message = ex.Message + " ";
                throw new Exception(message);
            }

            return stockSiteCodeMappingList;
        }

          internal static Dictionary<string,string> GetStockSiteCodeMapping()
          {
              var stockSiteCodeMappingList = new Dictionary<string, string>();

              bool boolSessionBegun = false;
              QBSessionManager sessionManager = new QBSessionManager();
              try
              {
                  IMsgSetRequest requestMsgSet;
                  IMsgSetResponse responseMsgSet;
                  sessionManager.OpenConnection("", _appName);
                  sessionManager.BeginSession(qdbpath, ENOpenMode.omDontCare);
                  boolSessionBegun = true;
                  requestMsgSet = GetLatestMsgSetRequest(sessionManager);
                  requestMsgSet.Attributes.OnError = ENRqOnError.roeContinue;

                  string errecid = "{" + Guid.NewGuid().ToString() + "}";
                  sessionManager.ErrorRecoveryID.SetValue(errecid);

                  sessionManager.EnableErrorRecovery = true;

                  sessionManager.SaveAllMsgSetRequestInfo = true;

                  #region error recovery

                  if (sessionManager.IsErrorRecoveryInfo())
                  {
                      IMsgSetRequest reqMsgSet = null;
                      IMsgSetResponse resMsgSet = null;
                      resMsgSet = sessionManager.GetErrorRecoveryStatus();
                      if (resMsgSet.Attributes.MessageSetStatusCode.Equals("600"))
                      {
                          MessageBox.Show(
                              "The oldMessageSetID does not match any stored IDs, and no newMessageSetID is provided.");
                      }
                      else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9001"))
                      {
                          MessageBox.Show(
                              "Invalid checksum. The newMessageSetID specified, matches the currently stored ID, but checksum fails.");
                      }
                      else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9002"))
                      {
                          MessageBox.Show("No stored response was found.");
                      }
                      else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9004"))
                      {
                          MessageBox.Show("Invalid MessageSetID, greater than 24 character was given.");
                      }
                      else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9005"))
                      {
                          MessageBox.Show("Unable to store response.");
                      }
                    

                      sessionManager.ClearErrorRecovery();
                  }

                  #endregion

                  requestMsgSet.AppendInventorySiteQueryRq();
                  
                  responseMsgSet = sessionManager.DoRequests(requestMsgSet);
                  IResponse responseList = responseMsgSet.ResponseList.GetAt(0);

                  IInventorySiteRetList inventorySiteRet = (IInventorySiteRetList)responseList.Detail;

                  for (int i = 0; i < inventorySiteRet.Count;i++ )
                  {
                      var name = inventorySiteRet.GetAt(i).Name.GetValue();

                      //Added in order to Exlude inventorysites without a contact
                      if (inventorySiteRet.GetAt(i).Contact==null) //(string.IsNullOrEmpty(inventorySiteRet.GetAt(i).Contact.GetValue()))
                      {
                          continue;
                      }
                      
                      var code = inventorySiteRet.GetAt(i).Contact.GetValue().Split(',');

                      
                      stockSiteCodeMappingList.Add(name,code[0]);

                  }
                      //if (inventorySiteRet != null && inventorySiteRet.GetAt(0).Contact != null)
                      //{
                      //    distributorCode = inventorySiteRet.GetAt(0).Contact.GetValue();

                      //    sessionManager.EndSession();
                      //    boolSessionBegun = false;
                      //    sessionManager.CloseConnection();

                      //    return distributorCode;
                      //}

                  sessionManager.EndSession();
                  boolSessionBegun = false;
                  sessionManager.CloseConnection();
              }
              catch (Exception ex)
              {
                  if (boolSessionBegun)
                  {
                      sessionManager.EndSession();
                      sessionManager.CloseConnection();
                  }
                  string error = (ex.Message.ToString() + "\nStack Trace: \n" + ex.StackTrace + "\nExiting the application");
                  Log(error);
                  var message = ex.Message + " ";
                  throw new Exception(message);
              }

              return stockSiteCodeMappingList;
          }
          internal static string GetStockSiteCode(string name)
          {
              var distributorCode = "default";

              bool boolSessionBegun = false;
              QBSessionManager sessionManager = new QBSessionManager();
              try
              {
                  IMsgSetRequest requestMsgSet;
                  IMsgSetResponse responseMsgSet;
                  sessionManager.OpenConnection("", _appName);
                  sessionManager.BeginSession(qdbpath, ENOpenMode.omDontCare);
                  boolSessionBegun = true;
                  requestMsgSet = GetLatestMsgSetRequest(sessionManager);
                  requestMsgSet.Attributes.OnError = ENRqOnError.roeContinue;

                  string errecid = "{" + Guid.NewGuid().ToString() + "}";
                  sessionManager.ErrorRecoveryID.SetValue(errecid);

                  sessionManager.EnableErrorRecovery = true;

                  sessionManager.SaveAllMsgSetRequestInfo = true;

                  #region error recovery

                  if (sessionManager.IsErrorRecoveryInfo())
                  {
                      IMsgSetRequest reqMsgSet = null;
                      IMsgSetResponse resMsgSet = null;
                      resMsgSet = sessionManager.GetErrorRecoveryStatus();
                      if (resMsgSet.Attributes.MessageSetStatusCode.Equals("600"))
                      {
                          MessageBox.Show(
                              "The oldMessageSetID does not match any stored IDs, and no newMessageSetID is provided.");
                      }
                      else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9001"))
                      {
                          MessageBox.Show(
                              "Invalid checksum. The newMessageSetID specified, matches the currently stored ID, but checksum fails.");
                      }
                      else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9002"))
                      {
                          MessageBox.Show("No stored response was found.");
                      }
                      else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9004"))
                      {
                          MessageBox.Show("Invalid MessageSetID, greater than 24 character was given.");
                      }
                      else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9005"))
                      {
                          MessageBox.Show("Unable to store response.");
                      }
                      else
                      {
                          IResponse res = resMsgSet.ResponseList.GetAt(0);
                          int sCode = res.StatusCode;
                          if (sCode == 0)
                          {
                              MessageBox.Show("Last request was processed and customer was added successfully!");
                          }
                          else if (sCode > 0)
                          {
                              MessageBox.Show("There was a warning but last request was processed successfully!");
                          }
                          else
                          {
                              MessageBox.Show("It seems that there was an error in processing last request");
                              reqMsgSet = sessionManager.GetSavedMsgSetRequest();
                              resMsgSet = sessionManager.DoRequests(reqMsgSet);
                              IResponse resp = resMsgSet.ResponseList.GetAt(0);
                              int statCode = resp.StatusCode;
                              if (statCode == 0)
                              {
                                  string resStr = null;
                                  ISalesOrderRet custRet = resp.Detail as ISalesOrderRet;
                                  resStr = resStr +
                                           "Following sale/order has been successfully submitted to QuickBooks:\n\n\n";
                                  if (custRet.TxnID != null)
                                  {
                                      resStr = resStr + "ListID Number = " + Convert.ToString(custRet.TxnID.GetValue()) +
                                               "\n";
                                      Log(resStr);
                                  }
                              }
                          }
                      }

                      sessionManager.ClearErrorRecovery();
                  }

                  #endregion

                  IInventorySiteQuery inventorySiteQuery = requestMsgSet.AppendInventorySiteQueryRq();
                  inventorySiteQuery.ORInventorySiteQuery.InventorySiteFilter.ORNameFilter.NameFilter.MatchCriterion.SetValue(ENMatchCriterion.mcContains);
                  inventorySiteQuery.ORInventorySiteQuery.InventorySiteFilter.ORNameFilter.NameFilter.Name.SetValue(name);

                  responseMsgSet = sessionManager.DoRequests(requestMsgSet);
                  IResponse responseList = responseMsgSet.ResponseList.GetAt(0);

                  IInventorySiteRetList inventorySiteRet = (IInventorySiteRetList)responseList.Detail;

                  if(inventorySiteRet!=null && inventorySiteRet.GetAt(0).Contact!=null)
                  {
                       distributorCode = inventorySiteRet.GetAt(0).Contact.GetValue();

                       sessionManager.EndSession();
                       boolSessionBegun = false;
                       sessionManager.CloseConnection();

                       return distributorCode;
                  }

                  
              }
              catch (Exception ex)
              {
                  if (boolSessionBegun)
                  {
                      sessionManager.EndSession();
                      sessionManager.CloseConnection();
                  }
                  string error = (ex.Message.ToString() + "\nStack Trace: \n" + ex.StackTrace + "\nExiting the application");
                  Log(error);
                  var message = ex.Message + " "+ name;
                  throw new Exception(message);
              }

              return distributorCode;
          }

          internal static string GetValidProductName(string productCode)
          {
              bool boolSessionBegun = false;
              QBSessionManager sessionManager = new QBSessionManager();
              IItemInventoryRet product = null;
              try
              {
                  IMsgSetRequest requestMsgSet;
                  IMsgSetResponse responseMsgSet;
                  sessionManager.OpenConnection("", _appName);
                  sessionManager.BeginSession(qdbpath, ENOpenMode.omDontCare);
                  boolSessionBegun = true;
                  requestMsgSet = GetLatestMsgSetRequest(sessionManager);
                  requestMsgSet.Attributes.OnError = ENRqOnError.roeStop;
                  // ISalesTaxCodeRetList
                  IItemInventoryQuery itemInventoryQ = requestMsgSet.AppendItemInventoryQueryRq();

                  itemInventoryQ.ORListQueryWithOwnerIDAndClass.ListWithClassFilter.ORNameFilter.NameFilter.MatchCriterion.SetValue(ENMatchCriterion.mcContains);
                  itemInventoryQ.ORListQueryWithOwnerIDAndClass.ListWithClassFilter.ORNameFilter.NameFilter.Name.SetValue(productCode);

                  responseMsgSet = sessionManager.DoRequests(requestMsgSet);


                  IResponse response = responseMsgSet.ResponseList.GetAt(0);
                  IItemInventoryRetList itemInventoryRetList = response.Detail as IItemInventoryRetList;

                  if (itemInventoryRetList != null && itemInventoryRetList.Count > 0)
                  {
                     
                      for (var i = 0; i < itemInventoryRetList.Count; i++)
                      {

                          IItemInventoryRet inventoryRet = itemInventoryRetList.GetAt(i);
                          if (inventoryRet != null && inventoryRet.IsActive.GetValue() && inventoryRet.Name.GetValue().Equals(productCode, StringComparison.InvariantCultureIgnoreCase)&& inventoryRet.ParentRef!=null)
                          {
                              var productDesc = inventoryRet.SalesDesc.GetValue();
                              product = inventoryRet;
                              break;
                          }

                      }


                  }

                  sessionManager.EndSession();
                  boolSessionBegun = false;
                  sessionManager.CloseConnection();

              }
              catch (Exception ex)
              {
                  if (boolSessionBegun)
                  {
                      sessionManager.EndSession();
                      sessionManager.CloseConnection();
                  }
                  string error = (ex.Message.ToString() + "\nStack Trace: \n" + ex.StackTrace + "\nExiting the application");
                  Log(error);
                  MessageBox.Show(error);

              }
              if (product != null)
              {
                  return product.SalesDesc.GetValue();
              }

              return "";
          }

        internal static string GetProductName(string productCode)
          {
              bool boolSessionBegun = false;
              QBSessionManager sessionManager = new QBSessionManager();
              IItemInventoryRet product = null;
              try
              {
                  IMsgSetRequest requestMsgSet;
                  IMsgSetResponse responseMsgSet;
                  sessionManager.OpenConnection("", _appName);
                  sessionManager.BeginSession(qdbpath, ENOpenMode.omDontCare);
                  boolSessionBegun = true;
                  requestMsgSet = GetLatestMsgSetRequest(sessionManager);
                  requestMsgSet.Attributes.OnError = ENRqOnError.roeStop;
                  // ISalesTaxCodeRetList
                  IItemInventoryQuery itemInventoryQ = requestMsgSet.AppendItemInventoryQueryRq();

                  itemInventoryQ.ORListQueryWithOwnerIDAndClass.ListWithClassFilter.ORNameFilter.NameFilter.MatchCriterion.SetValue(ENMatchCriterion.mcContains);
                  itemInventoryQ.ORListQueryWithOwnerIDAndClass.ListWithClassFilter.ORNameFilter.NameFilter.Name.SetValue(productCode);

                  responseMsgSet = sessionManager.DoRequests(requestMsgSet);


                  IResponse response = responseMsgSet.ResponseList.GetAt(0);
                  IItemInventoryRetList itemInventoryRetList = response.Detail as IItemInventoryRetList;
                  
                  if (itemInventoryRetList != null && itemInventoryRetList.Count > 0)
                  {

                      for (var i = 0; i < itemInventoryRetList.Count; i++)
                      {
                          IItemInventoryRet inventoryRet = itemInventoryRetList.GetAt(i);
                          if (inventoryRet != null && inventoryRet.IsActive.GetValue() && inventoryRet.Name.GetValue().Equals(productCode, StringComparison.InvariantCultureIgnoreCase))
                          {
                              var productDesc = inventoryRet.SalesDesc.GetValue();
                              product= inventoryRet;
                              break;
                          }

                      }


                  }

                  sessionManager.EndSession();
                  boolSessionBegun = false;
                  sessionManager.CloseConnection();
                 
              }
              catch (Exception ex)
              {
                  if (boolSessionBegun)
                  {
                      sessionManager.EndSession();
                      sessionManager.CloseConnection();
                  }
                  string error = (ex.Message.ToString() + "\nStack Trace: \n" + ex.StackTrace + "\nExiting the application");
                  Log(error);
                  MessageBox.Show(error);

              }
            if(product!=null)
            {
                return product.SalesDesc.GetValue();
            }

            return "";
          }
          internal static IItemInventoryRet GetProductByCode(string productCode)
          {
              bool boolSessionBegun = false;
              QBSessionManager sessionManager = new QBSessionManager();
              IItemInventoryRet product = null;
              try
              {
                  IMsgSetRequest requestMsgSet;
                  IMsgSetResponse responseMsgSet;
                  sessionManager.OpenConnection("", _appName);
                  sessionManager.BeginSession(qdbpath, ENOpenMode.omDontCare);
                  boolSessionBegun = true;
                  requestMsgSet = GetLatestMsgSetRequest(sessionManager);
                  requestMsgSet.Attributes.OnError = ENRqOnError.roeStop;
                  // ISalesTaxCodeRetList
                  IItemInventoryQuery itemInventoryQ = requestMsgSet.AppendItemInventoryQueryRq();

                  responseMsgSet = sessionManager.DoRequests(requestMsgSet);


                  IResponse response = responseMsgSet.ResponseList.GetAt(0);
                  IItemInventoryRetList itemInventoryRetList = response.Detail as IItemInventoryRetList;
                  
                  if (itemInventoryRetList != null && itemInventoryRetList.Count > 0)
                  {

                      for (var i = 0; i < itemInventoryRetList.Count; i++)
                      {
                          IItemInventoryRet inventoryRet = itemInventoryRetList.GetAt(i);
                          if (inventoryRet != null && inventoryRet.IsActive.GetValue() && inventoryRet.Name.GetValue().Equals(productCode,StringComparison.InvariantCultureIgnoreCase))
                          {
                              product= inventoryRet;
                              break;
                          }

                      }


                  }

                  sessionManager.EndSession();
                  boolSessionBegun = false;
                  sessionManager.CloseConnection();
                 
              }
              catch (Exception ex)
              {
                  if (boolSessionBegun)
                  {
                      sessionManager.EndSession();
                      sessionManager.CloseConnection();
                  }
                  string error = (ex.Message.ToString() + "\nStack Trace: \n" + ex.StackTrace + "\nExiting the application");
                  Log(error);
                  MessageBox.Show(error);

              }
              return product;
          }

        internal static void Products()
        {
            int productbrandcount = 0;
            int productsubbrandcount = 0;
            int productcount = 1;

            bool boolSessionBegun = false;
            QBSessionManager sessionManager = new QBSessionManager();
            try
            {
                IMsgSetRequest requestMsgSet;
                IMsgSetResponse responseMsgSet;
                sessionManager.OpenConnection("", _appName);
                sessionManager.BeginSession(qdbpath, ENOpenMode.omDontCare);
                boolSessionBegun = true;
                requestMsgSet = GetLatestMsgSetRequest(sessionManager);
                requestMsgSet.Attributes.OnError = ENRqOnError.roeStop;
               // ISalesTaxCodeRetList
                IItemInventoryQuery itemInventoryQ = requestMsgSet.AppendItemInventoryQueryRq();
               
                responseMsgSet = sessionManager.DoRequests(requestMsgSet);
                
               
                IResponse response = responseMsgSet.ResponseList.GetAt(0);
                IItemInventoryRetList itemInventoryRetList = response.Detail as IItemInventoryRetList;
                List<ProductImport> items= new List<ProductImport>();
                List<ProductPricing> pricings=new List<ProductPricing>();
                 List<Packaging> packagings=new List<Packaging>();
                List<ProductBrand> brands=new List<ProductBrand>();
                if (itemInventoryRetList != null && itemInventoryRetList.Count > 0)
                {
                   
                    for (var i = 0; i < itemInventoryRetList.Count; i++)
                    {
                        IItemInventoryRet inventoryRet = itemInventoryRetList.GetAt(i);
                        if(!inventoryRet.Sublevel.IsSet())
                        {
                            productbrandcount++;
                        }
                        if (inventoryRet.Sublevel.IsSet())
                        {
                            productsubbrandcount++;
                        }
                        if (inventoryRet.UnitOfMeasureSetRef!=null)
                        {
                            productcount++;
                        }

                        if (inventoryRet != null && inventoryRet.IsActive.GetValue())
                        {
                            var p = new ProductImport
                                        {
                                            pCode = inventoryRet.FullName != null ? inventoryRet.Name.GetValue() : "",
                                            pDesc =
                                                inventoryRet.SalesDesc != null ? inventoryRet.SalesDesc.GetValue() : "",
                                            exfactory = inventoryRet.PurchaseCost != null
                                                            ? Convert.ToDecimal(inventoryRet.PurchaseCost.GetAsString())
                                                            : 0m,
                                            vatclassName = inventoryRet.SalesTaxCodeRef != null
                                                               ? inventoryRet.SalesTaxCodeRef.FullName.GetValue()
                                                               : "",
                                            packagingTypeName = inventoryRet.UnitOfMeasureSetRef !=null?inventoryRet.UnitOfMeasureSetRef.FullName.GetValue():"",
                                            productBrandName = 
                                           inventoryRet.ParentRef != null ? inventoryRet.ParentRef.FullName.GetValue() : ""
                                        };
                            items.Add(p);
                            if (inventoryRet.ParentRef != null && inventoryRet.ParentRef.FullName != null )
                            {
                                IItemInventoryRet b = GetInventoryByNameFilter(inventoryRet.ParentRef.FullName.GetValue());
                                if (b != null )
                                {
                                    var brand = new ProductBrand
                                    {
                                        Code = b.FullName != null ? b.FullName.GetValue() : "",
                                        Name = b.SalesDesc != null ? b.SalesDesc.GetValue() : "",
                                        Description = b.SalesDesc != null ? b.SalesDesc.GetValue() : "",
                                        supplierName = "NESTLE KENYA LIMITED"   //b.PrefVendorRef.FullName.GetValue()
                                    };
                                    if (!string.IsNullOrEmpty(brand.Code) && brands.All(n => n.Code != brand.Code))
                                        brands.Add(brand);

                                    brands.Select(n => new {});
                                }
                                
                            }
                            if(inventoryRet!=null && inventoryRet.UnitOfMeasureSetRef!=null)//p !=null && items.Any(n=>n.pCode==p.pCode))
                            {
                                //Michael Added Code
                              //  if(p.exfactory==0) continue;
                                //End of Michael Added Code
                                var pricing = new ProductPricing
                                {
                                    ProductCode = inventoryRet.FullName != null ? inventoryRet.Name.GetValue() : "",
                                    Exfactory = inventoryRet.PurchaseCost != null
                                                            ? Convert.ToDecimal(inventoryRet.PurchaseCost.GetAsString())
                                                            : 0m,
                                    SellingPrice = inventoryRet.SalesPrice !=null?Convert.ToDecimal(inventoryRet.SalesPrice.GetValue()):0m,
                                    StartDate = DateTime.Today.ToShortDateString(),
                                    TierNameCode = "Default"
                                };
                                pricings.Add(pricing); 
                            }
                            

                        }

                    }

                    
                }

                sessionManager.EndSession();
                boolSessionBegun = false;
                sessionManager.CloseConnection();
                if (items.Any())
                {
                    DumpExportFilesAsync(items.Where(p=>!string.IsNullOrEmpty(p.productBrandName) && p.exfactory >0m).ToCsv(), MasterDataCollective.SaleProduct.ToString());
                }
                if (pricings.Any())
                {
                    DumpExportFilesAsync(pricings.ToCsv(), MasterDataCollective.Pricing.ToString());
                }
                if (packagings.Any())
                {
                    DumpExportFilesAsync(packagings.ToCsv(), MasterDataCollective.ProductPackaging.ToString());
                }

                if (brands.Any())
                {
                    DumpExportFilesAsync(brands.ToCsv(), MasterDataCollective.ProductBrand.ToString());
                }
            }
            catch (Exception ex)
            {
                if (boolSessionBegun)
                {
                    sessionManager.EndSession();
                    sessionManager.CloseConnection();
                }
                string error = (ex.Message.ToString() + "\nStack Trace: \n" + ex.StackTrace + "\nExiting the application");
                Log(error);
                MessageBox.Show(error);

            }
          
        }

        internal static void GetDistributors()
        {
            List<DistributorImport> listOfDistributors=new List<DistributorImport>();
            bool boolSessionBegun = false;
            QBSessionManager sessionManager = new QBSessionManager();
            try
            {
                IMsgSetRequest requestMsgSet;
                IMsgSetResponse responseMsgSet;
                sessionManager.OpenConnection("", _appName);
                sessionManager.BeginSession(qdbpath, ENOpenMode.omDontCare);
                boolSessionBegun = true;
                requestMsgSet = GetLatestMsgSetRequest(sessionManager);
                requestMsgSet.Attributes.OnError = ENRqOnError.roeContinue;

                string errecid = "{" + Guid.NewGuid().ToString() + "}";
                sessionManager.ErrorRecoveryID.SetValue(errecid);

                sessionManager.EnableErrorRecovery = true;

                sessionManager.SaveAllMsgSetRequestInfo = true;

                

                IInventorySiteQuery inventorySiteQuery = requestMsgSet.AppendInventorySiteQueryRq();
                
                responseMsgSet = sessionManager.DoRequests(requestMsgSet);
                IResponse responseList = responseMsgSet.ResponseList.GetAt(0);

                var inventorySiteRet = (IInventorySiteRetList)responseList.Detail;
                
                var count = inventorySiteRet.Count;
                for (int i = 0; i < count;i++)
                {
                    if(inventorySiteRet.GetAt(i)!=null &&inventorySiteRet.GetAt(i).SiteDesc!=null &&inventorySiteRet.GetAt(i).SiteDesc.GetValue()=="MainWarehouse")
                    {
                        var distributor = new DistributorImport()
                            {
                                DistributorCode = inventorySiteRet.GetAt(i).Name.GetValue(),
                                MerchantNo ="",
                                Name = inventorySiteRet.GetAt(i).Name.GetValue(),
                                RegionName = "default",
                                Pin="",
                                VatRegNo = "",
                                PayBillNo = "",
                                Latitude = "",
                                Longitude = "",
                            };

                        listOfDistributors.Add(distributor);
                    }
                    else
                    {
                        continue;
                    }
                }

                sessionManager.EndSession();
                boolSessionBegun = false;
                sessionManager.CloseConnection();

                if (listOfDistributors.Any())
                {
                    var files = listOfDistributors.ToCsv();
                    DumpExportFilesAsync(files, MasterDataCollective.Distributor.ToString());
                }
            }
            catch (Exception ex)
            {
                if (boolSessionBegun)
                {
                    sessionManager.EndSession();
                    sessionManager.CloseConnection();
                }
                string error = (ex.Message.ToString() + "\nStack Trace: \n" + ex.StackTrace + "\nExiting the application");
                Log(error);
                var message = ex.Message;
                throw new Exception(message);
            }

          //  return distributorCode;
        }

        internal static ITransferInventoryRet ReturnInventory(QuickBooksReturnInventoryDocumentDto orderDoc)
        {
         

            bool boolSessionBegun = false;
            QBSessionManager sessionManager = new QBSessionManager();
            try
            {
               

                     IMsgSetRequest requestMsgSet;
                     IMsgSetResponse responseMsgSet;
                     sessionManager.OpenConnection("", _appName);
                     sessionManager.BeginSession(qdbpath, ENOpenMode.omDontCare);
                     boolSessionBegun = true;
                     requestMsgSet = GetLatestMsgSetRequest(sessionManager);
                     requestMsgSet.Attributes.OnError = ENRqOnError.roeStop;

                     
                     var salesmanCode = orderDoc.SalesmanCode;
                     var fromSite = GetStockSiteBySalesperson(salesmanCode);

                     var toSite =string.Empty;

                     if (!string.IsNullOrEmpty(fromSite))
                     {
                         toSite = GetIssuingWarehouse(fromSite);
                     }
                    

                     if (string.IsNullOrEmpty(toSite))
                     {
                         toSite = "Unspecified Site";
                     }

                     ITransferInventoryAdd transferInventoryAddRq = requestMsgSet.AppendTransferInventoryAddRq();

                     transferInventoryAddRq.FromInventorySiteRef.FullName.SetValue(fromSite);
                     transferInventoryAddRq.ToInventorySiteRef.FullName.SetValue(toSite);

                     transferInventoryAddRq.TxnDate.SetValue(DateTime.Parse(DateTime.Now.ToString()));//orderDoc.DateOfIssue.ToString())); //Certain date form the incoming document
                     
               
                    

                
                    foreach (var lineItem in orderDoc.LineItems)
                    {
                        ITransferInventoryLineAdd transferInventoryLineAddList = transferInventoryAddRq.TransferInventoryLineAddList.Append();

                        IItemInventoryRet product = GetProductByCode(lineItem.ProductCode);
                        var prod = product.FullName.GetValue();
                        if (product != null)
                            transferInventoryLineAddList.ItemRef.FullName.SetValue(product.FullName.GetValue());
                        transferInventoryLineAddList.QuantityToTransfer.SetValue(Convert.ToDouble(Math.Round(lineItem.Quantity, 2)));//Set the value of the qantity being transfered
                    }



                     responseMsgSet = sessionManager.DoRequests(requestMsgSet);
                     IResponse response = responseMsgSet.ResponseList.GetAt(0);

                     int statusCode = response.StatusCode;
                     ITransferInventoryRet transferInventoryRet = response.Detail as ITransferInventoryRet;
                     if (statusCode == 0)
                     {
                         Console.WriteLine("Success");
                     }
                     else
                     {
                         MessageBox.Show(response.StatusMessage);
                     }

                     //sessionManager.ClearErrorRecovery();
                     sessionManager.EndSession();
                     boolSessionBegun = false;
                     sessionManager.CloseConnection();
                     Log(QBCRUDEAction.Add, "TransferInventoryReturns", (transferInventoryRet == null ? "" : transferInventoryRet.RefNumber.GetValue()), response);
                     return transferInventoryRet;

           
            
            }
            catch (Exception ex)
            {
                if (boolSessionBegun)
                {
                    sessionManager.EndSession();
                    sessionManager.CloseConnection();
                }
                string error = (ex.Message.ToString() + "\nStack Trace: \n" + ex.StackTrace + "\nExiting the application");
                Log(error);
                MessageBox.Show(error);

            }
            return null;
        }

        private static string GetIssuingWarehouse(string fromSite)
        {
            var stockSiteRef = string.Empty;


            bool boolSessionBegun = false;
            QBSessionManager sessionManager = new QBSessionManager();
            try
            {
                IMsgSetRequest requestMsgSet;
                IMsgSetResponse responseMsgSet;
                sessionManager.OpenConnection("", _appName);
                sessionManager.BeginSession(qdbpath, ENOpenMode.omDontCare);
                boolSessionBegun = true;
                requestMsgSet = GetLatestMsgSetRequest(sessionManager);
                requestMsgSet.Attributes.OnError = ENRqOnError.roeContinue;

                string errecid = "{" + Guid.NewGuid().ToString() + "}";
                sessionManager.ErrorRecoveryID.SetValue(errecid);

                sessionManager.EnableErrorRecovery = true;

                sessionManager.SaveAllMsgSetRequestInfo = true;

                #region error recovery

                if (sessionManager.IsErrorRecoveryInfo())
                {
                    IMsgSetRequest reqMsgSet = null;
                    IMsgSetResponse resMsgSet = null;
                    resMsgSet = sessionManager.GetErrorRecoveryStatus();
                    if (resMsgSet.Attributes.MessageSetStatusCode.Equals("600"))
                    {
                        MessageBox.Show(
                            "The oldMessageSetID does not match any stored IDs, and no newMessageSetID is provided.");
                    }
                    else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9001"))
                    {
                        MessageBox.Show(
                            "Invalid checksum. The newMessageSetID specified, matches the currently stored ID, but checksum fails.");
                    }
                    else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9002"))
                    {
                        MessageBox.Show("No stored response was found.");
                    }
                    else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9004"))
                    {
                        MessageBox.Show("Invalid MessageSetID, greater than 24 character was given.");
                    }
                    else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9005"))
                    {
                        MessageBox.Show("Unable to store response.");
                    }
                    else
                    {
                        IResponse res = resMsgSet.ResponseList.GetAt(0);
                        int sCode = res.StatusCode;
                        if (sCode == 0)
                        {
                            MessageBox.Show("Last request was processed and customer was added successfully!");
                        }
                        else if (sCode > 0)
                        {
                            MessageBox.Show("There was a warning but last request was processed successfully!");
                        }
                        else
                        {
                            MessageBox.Show("It seems that there was an error in processing last request");
                            reqMsgSet = sessionManager.GetSavedMsgSetRequest();
                            resMsgSet = sessionManager.DoRequests(reqMsgSet);
                            IResponse resp = resMsgSet.ResponseList.GetAt(0);
                            int statCode = resp.StatusCode;
                            if (statCode == 0)
                            {
                                string resStr = null;
                                ISalesOrderRet custRet = resp.Detail as ISalesOrderRet;
                                resStr = resStr +
                                         "Following sale/order has been successfully submitted to QuickBooks:\n\n\n";
                                if (custRet.TxnID != null)
                                {
                                    resStr = resStr + "ListID Number = " + Convert.ToString(custRet.TxnID.GetValue()) +
                                             "\n";
                                    Log(resStr);
                                }
                            }
                        }
                    }

                    sessionManager.ClearErrorRecovery();
                    //MessageBox.Show("Proceeding with current transaction.");
                }

                #endregion

                IInventorySiteQuery inventorySiteQuery = requestMsgSet.AppendInventorySiteQueryRq();

                responseMsgSet = sessionManager.DoRequests(requestMsgSet);
                IResponse responseList = responseMsgSet.ResponseList.GetAt(0);

                IInventorySiteRetList inventorySiteRet = (IInventorySiteRetList)responseList.Detail;

                var count = inventorySiteRet.Count;
                for (int i = 0; i < inventorySiteRet.Count; i++)
                {
                    var iterationpoint = i;
                    var siteRefName = inventorySiteRet.GetAt(i).Name != null
                                             ? inventorySiteRet.GetAt(i).Name.GetValue()
                                             : null;

                    if (siteRefName != null && siteRefName == fromSite)
                    {
                        stockSiteRef = inventorySiteRet.GetAt(i).SiteAddress.Addr1.GetValue();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                if (boolSessionBegun)
                {
                    sessionManager.EndSession();
                    sessionManager.CloseConnection();
                }
                string error = (ex.Message.ToString() + "\nStack Trace: \n" + ex.StackTrace + "\nExiting the application");
                Log(error);
                throw new Exception(ex.Message);
            }

            return stockSiteRef;
        }


        internal static List<InventoryImport> PullInitialInventory()
        {
            bool boolSessionBegun = false;
            QBSessionManager sessionManager = new QBSessionManager();
            List<InventoryImport> inventory = new List<InventoryImport>();

            var products=GetAllProducts();
            var distributorInventoryList = new List<InventoryImport>();

            var stockSiteCodMappingList = GetStockSiteCodeMapping();
            try
            {
                IMsgSetRequest requestMsgSet;
                IMsgSetResponse responseMsgSet;
                sessionManager.OpenConnection("", _appName);
                sessionManager.BeginSession(qdbpath, ENOpenMode.omDontCare);
                boolSessionBegun = true;
                requestMsgSet = GetLatestMsgSetRequest(sessionManager);
                requestMsgSet.Attributes.OnError = ENRqOnError.roeStop;

                //ITransferInventoryQuery transferInventoryQuery = requestMsgSet.AppendTransferInventoryQueryRq();


                ////Change from to the date in the lastdateofsync
                //var fileLocation = FileUtility.GetInventoryFile("LastDateOfSync");
                //var from = FileUtility.ReadFile(fileLocation);// DateTime.Now.AddMonths(-1).ToString();

                //if (string.IsNullOrWhiteSpace(from))
                //{
                //    from = ConfigurationManager.AppSettings["lastDateOfSync"];
                //}
                ////var fromDate = DateTime.Now.AddHours(-7).ToString();

                //var newSyncDate = DateTime.Now.ToString();
                //////if(from)
                ////transferInventoryQuery.ORTransferInventoryQuery.TxnFilterNoCurrency.ORDateRangeFilter.ModifiedDateRangeFilter.FromModifiedDate.SetValue(DateTime.Parse(from), false);
                ////transferInventoryQuery.ORTransferInventoryQuery.TxnFilterNoCurrency.ORDateRangeFilter.ModifiedDateRangeFilter.ToModifiedDate.SetValue(DateTime.Parse(newSyncDate), false);


                
                IItemSitesQuery itemSitesQuery = requestMsgSet.AppendItemSitesQueryRq();
               var references = GetStockSiteReference();
               // references.ToList().ForEach(s => itemSitesQuery.ORItemSitesQuery.ListIDList.Add(s.Key));
                

                responseMsgSet = sessionManager.DoRequests(requestMsgSet);
                IResponse respons = responseMsgSet.ResponseList.GetAt(0);

                IItemSitesRetList lineRetList = respons.Detail as IItemSitesRetList;
                if (lineRetList != null)
                {


                    var RetCount = lineRetList.Count;
                    for (int i = 0; i < RetCount; i++)
                    {
                        if (lineRetList != null)
                        {
                            var TransferInventoryRet = lineRetList.GetAt(i);
                            if (TransferInventoryRet != null)
                            {
                                if (TransferInventoryRet.InventorySiteRef == null || TransferInventoryRet.InventorySiteRef.FullName==null)
                                {
                                    continue;
                                }
                                var siteName = TransferInventoryRet.InventorySiteRef.FullName.GetValue();
                                var salesman =stockSiteCodMappingList.ContainsKey(siteName)? stockSiteCodMappingList[siteName]:"Default";//GetStockSiteCode(siteName);
                                var quantityOnHand = TransferInventoryRet.QuantityOnHand.GetValue();
                                var inventoryItemFullName=TransferInventoryRet.ORItemAssemblyORInventory.ItemInventoryRef.FullName.GetValue();
                               

                                var product = products.FirstOrDefault(p=>p.pDesc==inventoryItemFullName);
                                 if(product!=null )
                                 {
                                   
                                   
                                     var productCode = product.pCode;
                                     if(product.exfactory>0)
                                     {
                                    
                                        var distributorInventory = new InventoryImport()
                                        {
                                            ToSiteName = salesman,
                                            Balance = Convert.ToDecimal(quantityOnHand),
                                            ProductName = productCode
                                        };
                                        distributorInventoryList.Add(distributorInventory);
                                     }
                                }
                            }

                        }
                    }
                }


                sessionManager.EndSession();
                boolSessionBegun = false;
                sessionManager.CloseConnection();

                //var filename = "Stockline-Alidi Kenya Limited-" + DateTime.Now.ToString("yyyyMMdd");
                distributorInventoryList = distributorInventoryList.GroupBy(n => new { n.ProductName, n.ToSiteName })
                    .Select(m => new InventoryImport { Balance = m.Sum(g => g.Balance), ProductName = m.Key.ProductName, ToSiteName = m.Key.ToSiteName }).ToList();

                //DumpExportInventoryFileAsync(distributorInventoryList.ToCsv(), filename);

                //DumpExportInventoryFileAsync(newSyncDate, "LastDateOfSync");

                //var filePath = FileUtility.GetInventoryFile(filename);
                //SendToDistributor(filePath);
            }
            catch (Exception ex)
            {
                if (boolSessionBegun)
                {
                    sessionManager.EndSession();
                    sessionManager.CloseConnection();
                }
                string error = (ex.Message.ToString() + "\nStack Trace: \n" + ex.StackTrace + "\nExiting the application");
                Log(error);
                MessageBox.Show(error);

            }
            return distributorInventoryList.OrderBy(n=>n.ToSiteName).ThenBy(n=>n.ProductName).Distinct().ToList();
        }

        internal static async void PullInventory()
        {
            var fileLocation = FileUtility.GetInventoryFile("LastDateOfSync");
            var fromDate = FileUtility.ReadFile(fileLocation);
            var newSyncDate = DateTime.Now.ToString();

            bool boolSessionBegun = false;
            QBSessionManager sessionManager = new QBSessionManager();
           try
           {
            if(string.IsNullOrEmpty(fromDate))
            {
                var results = PullInitialInventory();

                var filename = "Stockline-Alidi Kenya Limited-Initial" + DateTime.Now.ToString("yyyyMMdd");
                results = results.GroupBy(n => new { n.ProductName, n.ToSiteName })
                    .Select(m => new InventoryImport { Balance = m.Sum(g => g.Balance), ProductName = m.Key.ProductName, ToSiteName = m.Key.ToSiteName }).ToList();

                DumpExportInventoryFileAsync(results.ToCsv(), filename);


                var filePath = FileUtility.GetInventoryFile(filename);
                bool success=await SendToDistributor(filePath);
                if (success)
                    DumpExportInventoryFileAsync(newSyncDate, "LastDateOfSync");
            }
            else
            {

                
                List<InventoryImport> inventory = new List<InventoryImport>();

                var distributorsList = new List<string>();
                var inventoryPerDistributor = new Dictionary<string, List<InventoryImport>>();
                var distributorsCodeDictionary = new Dictionary<string, string>();
                var distributorInventoryList = new List<InventoryImport>();

                var stockSiteCodMappingList = GetStockSiteCodeMapping();

                IMsgSetRequest requestMsgSet;
                IMsgSetResponse responseMsgSet;
                sessionManager.OpenConnection("", _appName);
                sessionManager.BeginSession(qdbpath, ENOpenMode.omDontCare);
                boolSessionBegun = true;
                requestMsgSet = GetLatestMsgSetRequest(sessionManager);
                requestMsgSet.Attributes.OnError = ENRqOnError.roeStop;

                ITransferInventoryQuery transferInventoryQuery = requestMsgSet.AppendTransferInventoryQueryRq();


                transferInventoryQuery.ORTransferInventoryQuery.TxnFilterNoCurrency.ORDateRangeFilter.
                    ModifiedDateRangeFilter.FromModifiedDate.SetValue(DateTime.Parse(fromDate), false);
                transferInventoryQuery.ORTransferInventoryQuery.TxnFilterNoCurrency.ORDateRangeFilter.
                    ModifiedDateRangeFilter.ToModifiedDate.SetValue(DateTime.Parse(newSyncDate), false);

                transferInventoryQuery.IncludeLineItems.SetValue(true);
                responseMsgSet = sessionManager.DoRequests(requestMsgSet);
                IResponse respons = responseMsgSet.ResponseList.GetAt(0);

                ITransferInventoryRetList lineRetList = respons.Detail as ITransferInventoryRetList;
                if (lineRetList != null)
                {


                    var RetCount = lineRetList.Count;
                    for (int i = 0; i < RetCount; i++)
                    {
                        if (lineRetList != null)
                        {
                            var TransferInventoryRet = lineRetList.GetAt(i);
                            if (TransferInventoryRet != null)
                            {
                                var fromSite = TransferInventoryRet.FromInventorySiteRef.FullName.GetValue();
                                var toSite = TransferInventoryRet.ToInventorySiteRef.FullName.GetValue();
                                var number = TransferInventoryRet.RefNumber.GetValue();
                                double lineItemsCount = 0.0;

                                if (TransferInventoryRet.TransferInventoryLineRetList != null)
                                {
                                    var toSiteName = TransferInventoryRet.ToInventorySiteRef.FullName.GetValue();
                                    var fromSiteName = TransferInventoryRet.FromInventorySiteRef.FullName.GetValue();
                                    var transferInventoryLineItemsCount =
                                        TransferInventoryRet.TransferInventoryLineRetList.Count;


                                    for (int j = 0; j < transferInventoryLineItemsCount; j++)
                                    {
                                        var productFullName =
                                            TransferInventoryRet.TransferInventoryLineRetList.GetAt(j).ItemRef.FullName.
                                                GetValue();
                                        var lastColonIndex = productFullName.LastIndexOf(":");
                                        var productCode =
                                            TransferInventoryRet.TransferInventoryLineRetList.GetAt(j).ItemRef.FullName.
                                                GetValue().Substring(lastColonIndex + 1);
                                        var productName = GetProductName(productCode);
                                        var balance =
                                            TransferInventoryRet.TransferInventoryLineRetList.GetAt(j).
                                                QuantityTransferred.GetValue();


                                        var distributorInventory = new InventoryImport()
                                            {
                                                ToSiteName = stockSiteCodMappingList[toSite],
                                                Balance = Convert.ToDecimal(balance),
                                                ProductName = productCode
                                            };
                                        distributorInventoryList.Add(distributorInventory);

                                    }


                                    Console.WriteLine(@"From {0} To {1} , the Ref number is {2}, Items Transfered {3}",
                                                      fromSite,
                                                      toSite, number, lineItemsCount);
                                }

                            }
                        }
                    }
                }


                sessionManager.EndSession();
                boolSessionBegun = false;
                sessionManager.CloseConnection();

                var filename = "Stockline-Alidi Kenya Limited-" + DateTime.Now.ToString("yyyyMMdd");
                distributorInventoryList = distributorInventoryList.GroupBy(n => new {n.ProductName, n.ToSiteName})
                    .Select(
                        m =>
                        new InventoryImport
                            {
                                Balance = m.Sum(g => g.Balance),
                                ProductName = m.Key.ProductName,
                                ToSiteName = m.Key.ToSiteName
                            }).ToList();

                DumpExportInventoryFileAsync(distributorInventoryList.ToCsv(), filename);


                var filePath = FileUtility.GetInventoryFile(filename);
                bool success = await SendToDistributor(filePath);
                if(success)
                    DumpExportInventoryFileAsync(newSyncDate, "LastDateOfSync");


            }

           }
            catch (Exception ex)
            {
                if (boolSessionBegun)
                {
                    sessionManager.EndSession();
                    sessionManager.CloseConnection();
                }
                string error = (ex.Message.ToString() + "\nStack Trace: \n" + ex.StackTrace + "\nExiting the application");
                Log(error);
                MessageBox.Show(error);

            
            }

        }

        private static async Task<bool> SendToDistributor(string filename)
        {
            var dto = LoadDistributorInventoryFiles(filename);
                if (dto != null && (dto.SalesmanInventoryList.Any() || dto.DistributorInventory.Any()))
                {
                  var res= await new InventoryTransferService().UploadInventory(dto);
                    if(res.Result.Equals("error",StringComparison.CurrentCultureIgnoreCase))
                    {
                        //Alert("There was an error updating inventory files:\n ResultInfo" + res.ResultInfo + "\nError Details:"+res.ErrorInfo);
                        return false;
                    }
                    //Alert(res.ResultInfo);
                    return true;
                }
            Alert(string.Format("Files cannot be loaded or the file on path: {0} is empty",filename));
            return false;
        }

        private static  InventoryTransferDTO LoadDistributorInventoryFiles(string path)
        {
            InventoryTransferDTO dto = new InventoryTransferDTO();
            var Files = FileUtility.GetStockLines(new DirectoryInfo(path));
            if (Files == null) return null;
            Files = Files.Distinct().ToArray();
            FileUtility.LogCommandActivity(DateTime.Now.ToString("hh:mm:ss") + "Generating import files");

            foreach (var fileInfo in Files)
            {
                //format=>stocline-distributorcode
                var highenSeparator = new[] { '-' };
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileInfo.FullName);
                if (fileNameWithoutExtension != null && fileNameWithoutExtension.StartsWith("Stockline"))
                {
                    FileUtility.LogCommandActivity(DateTime.Now.ToString("hh:mm:ss") +
                                                   string.Format(" Extracting file =>{0}", fileNameWithoutExtension));
                    var temp =
                        fileNameWithoutExtension.Split(highenSeparator, StringSplitOptions.RemoveEmptyEntries).ToList();

                    //only distributor
                    if (!string.IsNullOrEmpty(temp.ElementAtOrDefault(1)))
                    {
                        dto.DistributorCode = temp.ElementAt(1);// +"-" + temp.ElementAt(2);
                    }
                }


                var inventory = new InventoryTransferService().Import(fileInfo.FullName);
                var dicties = new Dictionary<string, List<InventoryLineItemDto>>();
                var externalDocRefs = new List<string>();
                if (inventory != null && inventory.Any())
                {
                    var grouped = inventory.GroupBy(p => p.SalesmanCode);

                    foreach (var group in grouped)
                    {
                        var listofLineItems = new List<InventoryLineItemDto>();
                        foreach (var itemModel in group)
                        {
                            if (!string.IsNullOrEmpty(itemModel.DocumentRef) && !externalDocRefs.Contains(itemModel.DocumentRef))
                            {
                                externalDocRefs.Add(itemModel.DocumentRef);
                            }
                            var qntity = 1m;
                            try
                            {
                                qntity = Convert.ToDecimal(itemModel.Quantity);
                            }
                            catch
                            {
                                qntity = 1m;
                            }
                            listofLineItems.Add(new InventoryLineItemDto()
                            {
                                ProductCode = itemModel.ProductCode,
                                Quantity = qntity

                            });

                        }
                        if (group.Key == "default" || string.IsNullOrEmpty(group.Key))
                            dto.DistributorInventory.AddRange(listofLineItems);
                        else
                        {
                            dicties.Add(group.Key, listofLineItems);

                        }

                    }
                }
                if (dicties.Any())
                    dto.SalesmanInventoryList.Add(dicties);
                if (externalDocRefs.Any())
                    dto.ExternalDocumentRefList = externalDocRefs.Distinct().ToList();

            }
            return dto;
        }
         static void PullCustomers()
        {
            bool boolSessionBegun = false;
            QBSessionManager sessionManager = new QBSessionManager();
            List<OutletImport> customers = new List<OutletImport>();
            List<RouteRegionInfo> routeRegionInfos=new List<RouteRegionInfo>();
            try
            {
                IMsgSetRequest requestMsgSet;
                IMsgSetResponse responseMsgSet;
                sessionManager.OpenConnection("", _appName);
                sessionManager.BeginSession(qdbpath, ENOpenMode.omDontCare);
                boolSessionBegun = true;
                requestMsgSet = GetLatestMsgSetRequest(sessionManager);
                requestMsgSet.Attributes.OnError = ENRqOnError.roeStop;

                ICustomerQuery custQ = requestMsgSet.AppendCustomerQueryRq();
               // custQ.ORCustomerListQuery.FullNameList.
                responseMsgSet = sessionManager.DoRequests(requestMsgSet);

                IResponse response = responseMsgSet.ResponseList.GetAt(0);
                ICustomerRetList customerRetList = response.Detail as ICustomerRetList;

                
                if (customerRetList != null && customerRetList.Count != 0)
                {
                    var company = GetCurrentCompany();
                    for (var i = 0; i < customerRetList.Count; i++)
                    {
                       ICustomerRet  customerRet = customerRetList.GetAt(i);
                       if (customerRet != null && customerRet.IsActive.GetValue())
                        {
                            var outlet = new OutletImport
                                             {
                                                 DistributrCode = company != null ? company.CompanyName.GetValue() : "default",
                                                 OutletName = customerRet.FullName != null ? customerRet.FullName.GetValue() : "",
                                                 OutletCode = customerRet.Name != null ? customerRet.Name.GetValue() : ""
                                             };
                            if(customerRet.CustomerTypeRef !=null)
                                outlet.Outletype = customerRet.CustomerTypeRef.FullName.GetValue();
                            if (customerRet.SalesTaxCodeRef != null)
                                outlet.VatClassCode = "";//customerRet.SalesTaxCodeRef.FullName.GetValue();
                            if (customerRet.PriceLevelRef !=null)
                            outlet.PricingTierCode = customerRet.PriceLevelRef.FullName.GetValue();
                           
                            if (customerRet.SalesRepRef !=null)
                            {
                                outlet.RouteCode = customerRet.SalesRepRef.FullName.GetValue();
                              
                            }
                            else
                            {
                                continue;
                            }
                            if (customerRet.ShipAddress != null)
                            {
                                var address = customerRet.ShipAddress;
                              var route=  new RouteRegionInfo()
                                    {
                                        Salemanref = outlet.RouteCode,
                                        Region = address.State != null ? address.State.GetValue() : ""
                                    };
                              if (route != null && string.IsNullOrEmpty(route.Region))
                                  routeRegionInfos.Add(route);
                            }

                            
                            customers.Add(outlet);
                        }
                    }
                }

                sessionManager.EndSession();
                boolSessionBegun = false;
                sessionManager.CloseConnection();
                if (customers.Any())
                {
                    DumpExportFilesAsync(customers.ToCsv(), MasterDataCollective.Outlet.ToString());
                }
                if(routeRegionInfos.Any())
                {
                    
                }
            }
            catch (Exception ex)
            {
                if (boolSessionBegun)
                {
                    sessionManager.EndSession();
                    sessionManager.CloseConnection();
                }
                string error = (ex.Message.ToString() + "\nStack Trace: \n" + ex.StackTrace + "\nExiting the application");
                Log(error);
                MessageBox.Show(error);

            }
          
        }

   
        static void GetSalesmenAndRoutes()
        {
            bool boolSessionBegun = false;
           
            QBSessionManager sessionManager = new QBSessionManager();
            try
            {
                IMsgSetRequest requestMsgSet;
                IMsgSetResponse responseMsgSet;
                sessionManager.OpenConnection("", _appName);
                sessionManager.BeginSession(qdbpath, ENOpenMode.omDontCare);
                boolSessionBegun = true;
                requestMsgSet = GetLatestMsgSetRequest(sessionManager);
                requestMsgSet.Attributes.OnError = ENRqOnError.roeStop;
                // ISalesTaxCodeRetList

                ISalesRepQuery repsQuery = requestMsgSet.AppendSalesRepQueryRq();
               // repsQuery.ORListQuery.FullNameList.Add(name);
                responseMsgSet = sessionManager.DoRequests(requestMsgSet);

                IResponse response = responseMsgSet.ResponseList.GetAt(0);
                ISalesRepRetList repsList = response.Detail as ISalesRepRetList;
                var salemen = new List<Salesman>();
                if (repsList != null && repsList.Count > 0)
                {
                    var company = GetCurrentCompany();
                    for (var i = 0; i < repsList.Count; i++)
                    {
                        ISalesRepRet rep = repsList.GetAt(i);
                    
                        if (rep != null && rep.IsActive.GetValue())
                        {
                            var result = new Salesman()
                                         {
                                             distributrCode =company!=null?company.CompanyName.GetValue(): "",
                                             name = rep.SalesRepEntityRef != null ? rep.Initial.GetValue() : "",//rep.SalesRepEntityRef != null ? rep.SalesRepEntityRef.FullName.GetValue() : "",
                                             salesmanCode = rep.SalesRepEntityRef != null ? rep.Initial.GetValue() : "",
                                             mobileNumber =""
                                         };
                           salemen.Add(result);


                        }

                    }
                  
                }

                sessionManager.EndSession();
                boolSessionBegun = false;
                sessionManager.CloseConnection();
                if (salemen.Any())
                {
                    
                    DumpExportFilesAsync(salemen.ToCsv(), MasterDataCollective.DistributorSalesman.ToString());
                    var routes = salemen.Select(n=>new
                                                       {
                                                           Name=n.name,
                                                           Code = n.salesmanCode,
                                                           Region="default" //todo=>Figute out how to determine regions for each route
                                                       }).ToCsv();
                    DumpExportFilesAsync(routes, MasterDataCollective.Route.ToString());

                }
            }
            catch (Exception ex)
            {
                if (boolSessionBegun)
                {
                    sessionManager.EndSession();
                    sessionManager.CloseConnection();
                }
                string error = (ex.Message.ToString() + "\nStack Trace: \n" + ex.StackTrace + "\nExiting the application");
                Log(error);
                MessageBox.Show(error);

            }
          
        }
       
      internal  static ICustomerRet QBAddCustomer(ImportEntity outlet)
        {
            bool boolSessionBegun = false;
            QBSessionManager sessionManager = new QBSessionManager();
            try
            {
                IMsgSetRequest requestMsgSet;
                IMsgSetResponse responseMsgSet;
                sessionManager.OpenConnection("", _appName);
                sessionManager.BeginSession(qdbpath, ENOpenMode.omDontCare);
                boolSessionBegun = true;
                requestMsgSet = GetLatestMsgSetRequest(sessionManager);
                requestMsgSet.Attributes.OnError = ENRqOnError.roeContinue;

                string errecid = "{" + Guid.NewGuid().ToString() + "}";
                sessionManager.ErrorRecoveryID.SetValue(errecid);

                sessionManager.EnableErrorRecovery = true;

                sessionManager.SaveAllMsgSetRequestInfo = true;

                #region error recovery

                if (sessionManager.IsErrorRecoveryInfo())
                {
                    IMsgSetRequest reqMsgSet = null;
                    IMsgSetResponse resMsgSet = null;
                    resMsgSet = sessionManager.GetErrorRecoveryStatus();
                    if (resMsgSet.Attributes.MessageSetStatusCode.Equals("600"))
                    {
                        MessageBox.Show(
                            "The oldMessageSetID does not match any stored IDs, and no newMessageSetID is provided.");
                    }
                    else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9001"))
                    {
                        MessageBox.Show(
                            "Invalid checksum. The newMessageSetID specified, matches the currently stored ID, but checksum fails.");
                    }
                    else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9002"))
                    {
                        MessageBox.Show("No stored response was found.");
                    }
                    else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9004"))
                    {
                        MessageBox.Show("Invalid MessageSetID, greater than 24 character was given.");
                    }
                    else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9005"))
                    {
                        MessageBox.Show("Unable to store response.");
                    }
                    else
                    {
                        IResponse res = resMsgSet.ResponseList.GetAt(0);
                        int sCode = res.StatusCode;
                        if (sCode == 0)
                        {
                            MessageBox.Show("Last request was processed and customer was added successfully!");
                        }
                        else if (sCode > 0)
                        {
                            MessageBox.Show("There was a warning but last request was processed successfully!");
                        }
                        else
                        {
                            MessageBox.Show("It seems that there was an error in processing last request");
                            reqMsgSet = sessionManager.GetSavedMsgSetRequest();
                            resMsgSet = sessionManager.DoRequests(reqMsgSet);
                            IResponse resp = resMsgSet.ResponseList.GetAt(0);
                            int statCode = resp.StatusCode;
                            if (statCode == 0)
                            {
                                string resStr = null;
                                ICustomerRet custRet = resp.Detail as ICustomerRet;
                                resStr = resStr + "Following customer has been successfully submitted to QuickBooks:\n\n\n";
                                if (custRet.ListID != null)
                                {
                                    resStr = resStr + "ListID Number = " + Convert.ToString(custRet.ListID.GetValue()) +
                                             "\n";
                                }
                                Log(QBCRUDEAction.ErrorRecovery, "SalesReceipt", (custRet == null ? "" : custRet.FullName.GetValue()), resp);
                            }
                        }
                    }

                    sessionManager.ClearErrorRecovery();
                    //MessageBox.Show("Proceeding with current transaction.");
                }

                #endregion

                string customerName = outlet.Fields[1];
                bool state = true;
                try
                {
                    if (Boolean.TryParse(outlet.Fields[2], out state)) ;
                    
                }catch(Exception)
                {
                    state = true;

                }
               
                ICustomerRet existingQB = GetCustomerByNameFilter(customerName);
                ICustomerRet customerRet = null;

                
                if (existingQB == null)
                {
                   
                    ICustomerAdd customerAdd = requestMsgSet.AppendCustomerAddRq();
                    customerAdd.Name.SetValue(customerName);
                   // customerAdd.CompanyName.SetValue(companyName);
                    customerAdd.IsActive.SetValue(state);
                    responseMsgSet = sessionManager.DoRequests(requestMsgSet);
                    IResponse response = responseMsgSet.ResponseList.GetAt(0);
                    int statusCode = response.StatusCode;
                    customerRet = response.Detail as ICustomerRet;
                    if (statusCode == 0)
                    {
                        Console.WriteLine("Success");
                    }
                    else if (statusCode == 3100)
                    {
                        MessageBox.Show("Customer with same name exists");
                    }
                    Log(QBCRUDEAction.Add, "Customer", (customerRet == null ? "" : customerRet.FullName.GetValue()), response);
                    return customerRet;
                }
                else
                {
                    ICustomerMod customerModRq = requestMsgSet.AppendCustomerModRq();
                    customerModRq.Name.SetValue(customerName);
                    customerModRq.IsActive.SetValue(state);
                  //  customerModRq.CompanyName.SetValue(companyName);
                    customerModRq.ListID.SetValue(existingQB.ListID.GetValue());
                    customerModRq.EditSequence.SetValue(existingQB.EditSequence.GetValue());
                    responseMsgSet = sessionManager.DoRequests(requestMsgSet);
                    IResponse response = responseMsgSet.ResponseList.GetAt(0);
                    int statusCode = response.StatusCode;
                    customerRet = response.Detail as ICustomerRet;
                    if (statusCode == 0)
                    {
                        Console.WriteLine("Success");
                    }
                    Log(QBCRUDEAction.Update, "Customer", (customerRet == null ? "" : customerRet.FullName.GetValue()), response);
                }

                sessionManager.ClearErrorRecovery();
                sessionManager.EndSession();
                boolSessionBegun = false;
                sessionManager.CloseConnection();
                return customerRet;
            }
            catch (Exception ex)
            {

                if (boolSessionBegun)
                {
                    sessionManager.EndSession();
                    sessionManager.CloseConnection();
                }
                string error = (ex.Message.ToString() + "\nStack Trace: \n" + ex.StackTrace + "\nExiting the application");
                Log(error);
                int attemp = 0;
                if (ex.Message.Contains("Object reference not set to an instance of an object")&& attemp<3)
                {
                    QBAddCustomer(outlet);
                    attemp++;
                }
                else
                {
                    MessageBox.Show(ex.Message);
                }
               

            }
            return null;
        }

        internal static IAccountRet QBAddAccount(string accountName, bool isActive, ENAccountType accountType, string accountNumber)
        {
            bool boolSessionBegun = false;
            QBSessionManager sessionManager = new QBSessionManager();
            try
            {
                IMsgSetRequest requestMsgSet;
                IMsgSetResponse responseMsgSet;
                sessionManager.OpenConnection("", _appName);
                sessionManager.BeginSession(qdbpath, ENOpenMode.omDontCare);
                boolSessionBegun = true;
                requestMsgSet = GetLatestMsgSetRequest(sessionManager);
                requestMsgSet.Attributes.OnError = ENRqOnError.roeContinue;

                string errecid = "{" + Guid.NewGuid().ToString() + "}";
                sessionManager.ErrorRecoveryID.SetValue(errecid);

                sessionManager.EnableErrorRecovery = true;

                sessionManager.SaveAllMsgSetRequestInfo = true;

                #region error recovery

                if (sessionManager.IsErrorRecoveryInfo())
                {
                    IMsgSetRequest reqMsgSet = null;
                    IMsgSetResponse resMsgSet = null;
                    resMsgSet = sessionManager.GetErrorRecoveryStatus();
                    if (resMsgSet.Attributes.MessageSetStatusCode.Equals("600"))
                    {
                        MessageBox.Show(
                            "The oldMessageSetID does not match any stored IDs, and no newMessageSetID is provided.");
                    }
                    else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9001"))
                    {
                        MessageBox.Show(
                            "Invalid checksum. The newMessageSetID specified, matches the currently stored ID, but checksum fails.");
                    }
                    else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9002"))
                    {
                        MessageBox.Show("No stored response was found.");
                    }
                    else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9004"))
                    {
                        MessageBox.Show("Invalid MessageSetID, greater than 24 character was given.");
                    }
                    else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9005"))
                    {
                        MessageBox.Show("Unable to store response.");
                    }
                    else
                    {
                        IResponse res = resMsgSet.ResponseList.GetAt(0);
                        int sCode = res.StatusCode;
                        if (sCode == 0)
                        {
                            MessageBox.Show("Last request was processed and customer was added successfully!");
                        }
                        else if (sCode > 0)
                        {
                            MessageBox.Show("There was a warning but last request was processed successfully!");
                        }
                        else
                        {
                            MessageBox.Show("It seems that there was an error in processing last request");
                            reqMsgSet = sessionManager.GetSavedMsgSetRequest();
                            resMsgSet = sessionManager.DoRequests(reqMsgSet);
                            IResponse resp = resMsgSet.ResponseList.GetAt(0);
                            int statCode = resp.StatusCode;
                            if (statCode == 0)
                            {
                                string resStr = null;
                                ICustomerRet custRet = resp.Detail as ICustomerRet;
                                resStr = resStr + "Following customer has been successfully submitted to QuickBooks:\n\n\n";
                                if (custRet.ListID != null)
                                {
                                    resStr = resStr + "ListID Number = " + Convert.ToString(custRet.ListID.GetValue()) +
                                             "\n";
                                }
                                Log(QBCRUDEAction.ErrorRecovery, "SalesReceipt", (custRet == null ? "" : custRet.FullName.GetValue()), resp);
                            }
                        }
                    }

                    sessionManager.ClearErrorRecovery();
                    //MessageBox.Show("Proceeding with current transaction.");
                }

                #endregion

                IAccountRet existingQB = GetAccountByNameFilter(accountName);
                IAccountRet accountRet = null;
                if (existingQB == null)
                {
                    IAccountAdd accountAddRq = requestMsgSet.AppendAccountAddRq();
                    accountAddRq.Name.SetValue(accountName);
                    accountAddRq.AccountType.SetValue(accountType);
                    accountAddRq.AccountNumber.SetValue(accountNumber);
                    accountAddRq.IsActive.SetValue(isActive);

                    responseMsgSet = sessionManager.DoRequests(requestMsgSet);
                    IResponse response = responseMsgSet.ResponseList.GetAt(0);
                    int statusCode = response.StatusCode;

                    accountRet = response.Detail as IAccountRet;
                    if (statusCode == 0)
                    {
                        Console.WriteLine("Success");
                    }
                    else if (statusCode == 3100)
                    {
                        MessageBox.Show("Account with same name exists");
                    }
                    Log(QBCRUDEAction.Add, "Account", (accountRet == null ? "" : accountRet.FullName.GetValue()), response);
                    return accountRet;
                }
                else
                {
                    IAccountMod customerModRq = requestMsgSet.AppendAccountModRq();
                    customerModRq.Name.SetValue(accountName);
                    customerModRq.IsActive.SetValue(isActive);
                    customerModRq.AccountType.SetValue(accountType);
                    customerModRq.AccountNumber.SetValue(accountNumber);
                    customerModRq.ListID.SetValue(existingQB.ListID.GetValue());
                    customerModRq.EditSequence.SetValue(existingQB.EditSequence.GetValue());
                    responseMsgSet = sessionManager.DoRequests(requestMsgSet);
                    IResponse response = responseMsgSet.ResponseList.GetAt(0);
                    int statusCode = response.StatusCode;
                    accountRet = response.Detail as IAccountRet;
                    if (statusCode == 0)
                    {
                        Console.WriteLine("Success");
                    }
                    Log(QBCRUDEAction.Update, "Account", (accountRet == null ? "" : accountRet.FullName.GetValue()), response);
                }

                sessionManager.ClearErrorRecovery();
                sessionManager.EndSession();
                boolSessionBegun = false;
                sessionManager.CloseConnection();
                return accountRet;
            }
            catch (Exception ex)
            {

                if (boolSessionBegun)
                {
                    sessionManager.EndSession();
                    sessionManager.CloseConnection();
                }
                string error = (ex.Message.ToString() + "\nStack Trace: \n" + ex.StackTrace + "\nExiting the application");
                Log(error);
                MessageBox.Show(error);
              //  throw new Exception(ex.Message);

            }
            return null;
        }

       internal  static ICustomerRet GetCustomerByNameFilter(string customerName)
        {

            bool boolSessionBegun = false;
            QBSessionManager sessionManager = new QBSessionManager();
            ICustomerRet customerRet = null;
            try
            {
                IMsgSetRequest requestMsgSet;
                IMsgSetResponse responseMsgSet;
                sessionManager.OpenConnection("", _appName);
                sessionManager.BeginSession(qdbpath, ENOpenMode.omDontCare);
                boolSessionBegun = true;
                requestMsgSet = GetLatestMsgSetRequest(sessionManager);
                requestMsgSet.Attributes.OnError = ENRqOnError.roeStop;

                ICustomerQuery custQ = requestMsgSet.AppendCustomerQueryRq();
                custQ.ORCustomerListQuery.FullNameList.Add(customerName);
                responseMsgSet = sessionManager.DoRequests(requestMsgSet);

                IResponse response = responseMsgSet.ResponseList.GetAt(0);
                ICustomerRetList customerRetList = response.Detail as ICustomerRetList; 
                if (customerRetList != null && customerRetList.Count != 0)
                {
                    customerRet = customerRetList.GetAt(0);
                }

                sessionManager.EndSession();
                boolSessionBegun = false;
                sessionManager.CloseConnection();
            }
            catch (Exception ex)
            {
                if (boolSessionBegun)
                {
                    sessionManager.EndSession();
                    sessionManager.CloseConnection();
                }
                string error = (ex.Message.ToString() + "\nStack Trace: \n" + ex.StackTrace + "\nExiting the application");
                Log(error);
                MessageBox.Show(error);

            }
            return customerRet;
        }

        static ICompanyRet GetCurrentCompany()
        {
            bool boolSessionBegun = false;
            QBSessionManager sessionManager = new QBSessionManager();
            ICompanyRet companyRet = null;
            try
            {
                IMsgSetRequest requestMsgSet;
                IMsgSetResponse responseMsgSet;
                sessionManager.OpenConnection("", _appName);
                sessionManager.BeginSession(qdbpath, ENOpenMode.omDontCare);
                boolSessionBegun = true;
                requestMsgSet = GetLatestMsgSetRequest(sessionManager);
                requestMsgSet.Attributes.OnError = ENRqOnError.roeStop;

                ICompanyQuery company = requestMsgSet.AppendCompanyQueryRq();

               
                responseMsgSet = sessionManager.DoRequests(requestMsgSet);
                 IResponse response = responseMsgSet.ResponseList.GetAt(0);
                companyRet = response.Detail as ICompanyRet;
             
                sessionManager.EndSession();
                boolSessionBegun = false;
                sessionManager.CloseConnection();

                return companyRet;
            }
            catch (Exception ex)
            {
                if (boolSessionBegun)
                {
                    sessionManager.EndSession();
                    sessionManager.CloseConnection();
                }
                string error = (ex.Message.ToString() + "\nStack Trace: \n" + ex.StackTrace + "\nExiting the application");
                Log(error);
                throw new Exception(ex.Message);

            }
       
        }
        internal static IAccountRet GetAccountByNameFilter(string accountName)
        {

            bool boolSessionBegun = false;
            QBSessionManager sessionManager = new QBSessionManager();
            IAccountRet accountRet = null;
            try
            {
                IMsgSetRequest requestMsgSet;
                IMsgSetResponse responseMsgSet;
                sessionManager.OpenConnection("", _appName);
                sessionManager.BeginSession(qdbpath, ENOpenMode.omDontCare);
                boolSessionBegun = true;
                requestMsgSet = GetLatestMsgSetRequest(sessionManager);
                requestMsgSet.Attributes.OnError = ENRqOnError.roeStop;

                IAccountQuery accQ = requestMsgSet.AppendAccountQueryRq();
                accQ.ORAccountListQuery.FullNameList.Add(accountName);
                responseMsgSet = sessionManager.DoRequests(requestMsgSet);

                IResponse response = responseMsgSet.ResponseList.GetAt(0);
                IAccountRetList accountRetList = response.Detail as IAccountRetList;
                if (accountRetList != null && accountRetList.Count != 0)
                {
                    accountRet = accountRetList.GetAt(0);
                }

                sessionManager.EndSession();
                boolSessionBegun = false;
                sessionManager.CloseConnection();
            }
            catch (Exception ex)
            {
                if (boolSessionBegun)
                {
                    sessionManager.EndSession();
                    sessionManager.CloseConnection();
                }
                string error = (ex.Message.ToString() + "\nStack Trace: \n" + ex.StackTrace + "\nExiting the application");
                Log(error);
                throw new Exception(ex.Message);

            }
            return accountRet;
        }

        internal static List<ProductImport> GetAllProducts()
        {
            var products = new List<ProductImport>();

            bool boolSessionBegun = false;
            QBSessionManager sessionManager = new QBSessionManager();
            IItemInventoryRet itemInventoryRet = null;
            try
            {
                IMsgSetRequest requestMsgSet;
                IMsgSetResponse responseMsgSet;
                sessionManager.OpenConnection("", _appName);
                sessionManager.BeginSession(qdbpath, ENOpenMode.omDontCare);
                boolSessionBegun = true;
                requestMsgSet = GetLatestMsgSetRequest(sessionManager);
                requestMsgSet.Attributes.OnError = ENRqOnError.roeStop;
                IItemInventoryQuery itemInventoryQ = requestMsgSet.AppendItemInventoryQueryRq();
                responseMsgSet = sessionManager.DoRequests(requestMsgSet);

                IResponse response = responseMsgSet.ResponseList.GetAt(0);
                IItemInventoryRetList itemInventoryRetList = response.Detail as IItemInventoryRetList;

               
                    if (itemInventoryRetList != null && itemInventoryRetList.Count > 0)
                    {
                        for (int i = 0; i < itemInventoryRetList.Count; i++)
                        {
                            itemInventoryRet = itemInventoryRetList.GetAt(i);
                            if(itemInventoryRet!=null)
                            {
                                var product = new ProductImport();
                                product.pDesc = itemInventoryRet.FullName.GetValue();
                                product.pCode = itemInventoryRet.Name.GetValue();
                                product.exfactory = itemInventoryRet.PurchaseCost != null
                                                        ? Convert.ToDecimal(itemInventoryRet.PurchaseCost.GetAsString())
                                                        : 0m;

                               
                                products.Add(product);
                            }
                            
                        }
                        
                    }

                sessionManager.EndSession();
                boolSessionBegun = false;
                sessionManager.CloseConnection();
            }
            catch (Exception ex)
            {
                if (boolSessionBegun)
                {
                    sessionManager.EndSession();
                    sessionManager.CloseConnection();
                }
                string error = (ex.Message.ToString() + "\nStack Trace: \n" + ex.StackTrace + "\nExiting the application");
                Log(error);
                MessageBox.Show(error);

            }
            return products;
        }

       internal static IItemInventoryRet GetInventoryByNameFilter(string inventoryName)
        {

            bool boolSessionBegun = false;
            QBSessionManager sessionManager = new QBSessionManager();
            IItemInventoryRet itemInventoryRet = null;
            try
            {
                IMsgSetRequest requestMsgSet;
                IMsgSetResponse responseMsgSet;
                sessionManager.OpenConnection("", _appName);
                sessionManager.BeginSession(qdbpath, ENOpenMode.omDontCare);
                boolSessionBegun = true;
                requestMsgSet = GetLatestMsgSetRequest(sessionManager);
                requestMsgSet.Attributes.OnError = ENRqOnError.roeStop;
                IItemInventoryQuery itemInventoryQ = requestMsgSet.AppendItemInventoryQueryRq();
                itemInventoryQ.ORListQueryWithOwnerIDAndClass.FullNameList.Add(inventoryName);
                responseMsgSet = sessionManager.DoRequests(requestMsgSet);

                IResponse response = responseMsgSet.ResponseList.GetAt(0);
                IItemInventoryRetList itemInventoryRetList = response.Detail as IItemInventoryRetList;

                if (itemInventoryRetList != null && itemInventoryRetList.Count > 0)
                {
                    itemInventoryRet = itemInventoryRetList.GetAt(0);
                }

                sessionManager.EndSession();
                boolSessionBegun = false;
                sessionManager.CloseConnection();
            }
            catch (Exception ex)
            {
                if (boolSessionBegun)
                {
                    sessionManager.EndSession();
                    sessionManager.CloseConnection();
                }
                string error = (ex.Message.ToString() + "\nStack Trace: \n" + ex.StackTrace + "\nExiting the application");
                Log(error);
                MessageBox.Show(error);

            }
            return itemInventoryRet;
        }

        static IMsgSetRequest GetLatestMsgSetRequest(QBSessionManager sessionManager)
        {
            // Find and adapt to supported version of QuickBooks
            double supportedVersion = QBFCLatestVersion(sessionManager);

            short qbXMLMajorVer = 0;
            short qbXMLMinorVer = 0;
            if (supportedVersion >= 12.0)
            {
                qbXMLMajorVer = 12;
                qbXMLMinorVer = 0;
                IMsgSetRequest request = sessionManager.CreateMsgSetRequest("US", qbXMLMajorVer, qbXMLMinorVer);
                return request;
            }
            if (supportedVersion >= 6.0)
            {
                qbXMLMajorVer = 6;
                qbXMLMinorVer = 0;
            }
            else if (supportedVersion >= 5.0)
            {
                qbXMLMajorVer = 5;
                qbXMLMinorVer = 0;
            }
            else if (supportedVersion >= 4.0)
            {
                qbXMLMajorVer = 4;
                qbXMLMinorVer = 0;
            }
            else if (supportedVersion >= 3.0)
            {
                qbXMLMajorVer = 3;
                qbXMLMinorVer = 0;
            }
            else if (supportedVersion >= 2.0)
            {
                qbXMLMajorVer = 2;
                qbXMLMinorVer = 0;
            }
            else if (supportedVersion >= 1.1)
            {
                qbXMLMajorVer = 1;
                qbXMLMinorVer = 1;
            }
            else
            {
                qbXMLMajorVer = 1;
                qbXMLMinorVer = 0;
                MessageBox.Show("It seems that you are running QuickBooks 2002 Release 1. We strongly recommend that you use QuickBooks' online update feature to obtain the latest fixes and enhancements");
            }

            IMsgSetRequest requestMsgSet = sessionManager.CreateMsgSetRequest("US", qbXMLMajorVer, qbXMLMinorVer);
            return requestMsgSet;
        }

        static double QBFCLatestVersion(QBSessionManager SessionManager)
        {
            IMsgSetRequest msgset = SessionManager.CreateMsgSetRequest("US", 1, 0);
            msgset.AppendHostQueryRq();
            IMsgSetResponse QueryResponse = SessionManager.DoRequests(msgset);
            IResponse response = QueryResponse.ResponseList.GetAt(0);

            IHostRet HostResponse = response.Detail as IHostRet;
            IBSTRList supportedVersions = HostResponse.SupportedQBXMLVersionList as IBSTRList;

            int i;
            double vers;
            double LastVers = 0;
            string svers = null;

            for (i = 0; i <= supportedVersions.Count - 1; i++)
            {
                svers = supportedVersions.GetAt(i);
                vers = Convert.ToDouble(svers);
                if (vers > LastVers)
                {
                    LastVers = vers;
                }
            }
            return LastVers;
        }

        static void Log(QBCRUDEAction action, string itemObjectType, string itemName, IResponse response)
        {
            string log = DateTime.Now.ToString("MM:HH dd/mm/yyy") + action + " " + itemObjectType + " " + itemName + "." + "\nStatus Code :" + response.StatusCode +
                         ": Status message: " + response.StatusMessage + ": Status severity: " + response.StatusSeverity;
            _logger.Info(log);
        }

        static void Log(string action)
        {
            string log = DateTime.Now.ToString("MM:HH dd/mm/yyy") + action;
            _logger.Info(log);
        }

        private enum QBCRUDEAction
        {
            Add = 1,
            Update = 2,
            Delete = 3,
            ErrorRecovery = 4
        };

        public static bool CanConnect()
        {
          bool boolSessionBegun = false;
            QBSessionManager sessionManager = new QBSessionManager();
           
            try
            {
                IMsgSetRequest requestMsgSet;
                IMsgSetResponse responseMsgSet;
                sessionManager.OpenConnection("", _appName);
                sessionManager.BeginSession(qdbpath, ENOpenMode.omDontCare);
                boolSessionBegun = true;
                requestMsgSet = GetLatestMsgSetRequest(sessionManager);
                requestMsgSet.Attributes.OnError = ENRqOnError.roeStop;
                sessionManager.EndSession();
                sessionManager.CloseConnection();
                return boolSessionBegun;
            }catch(Exception ex)
            {
                if (boolSessionBegun)
                {
                    sessionManager.EndSession();
                    sessionManager.CloseConnection();
                }
                string error = (ex.Message.ToString() + "\nStack Trace: \n" + ex.StackTrace + "\nExiting the application");
                MessageBox.Show(error);
                return false;
            }
        }

        public static IAccountRetList QBGetChartOfAccountAccount(ENAccountType enAccountType)
        {
            bool boolSessionBegun = false;
            QBSessionManager sessionManager = new QBSessionManager();
            IAccountRetList accountRetList;
            try
            {
                IMsgSetRequest requestMsgSet;
                IMsgSetResponse responseMsgSet;
                sessionManager.OpenConnection("", _appName);
                sessionManager.BeginSession(qdbpath, ENOpenMode.omDontCare);
                boolSessionBegun = true;
                requestMsgSet = GetLatestMsgSetRequest(sessionManager);
                requestMsgSet.Attributes.OnError = ENRqOnError.roeStop;
                
                IAccountQuery accQ = requestMsgSet.AppendAccountQueryRq();
                accQ.ORAccountListQuery.AccountListFilter.AccountTypeList.Add(enAccountType);
                responseMsgSet = sessionManager.DoRequests(requestMsgSet);

                IResponse response = responseMsgSet.ResponseList.GetAt(0);
                accountRetList = response.Detail as IAccountRetList;
                
                sessionManager.EndSession();
                boolSessionBegun = false;
                sessionManager.CloseConnection();
            }
            catch (Exception ex)
            {
                if (boolSessionBegun)
                {
                    sessionManager.EndSession();
                    sessionManager.CloseConnection();
                }
                string error = (ex.Message.ToString() + "\nStack Trace: \n" + ex.StackTrace + "\nExiting the application");
                Log(error);
                int attemp = 0;
                if (ex.Message.Contains("Object reference not set to an instance of an object") && attemp < 3)
                {
                    QBGetChartOfAccountAccount(enAccountType);
                    attemp++;
                }
                else
                {
                    MessageBox.Show(ex.Message);
                }
                return null;

            }
            return accountRetList;
        }
        static void Alert(string message)
        {
            MessageBox.Show(message, "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        static void DumpExportFilesAsync(string file, string masterdataEntity)
        {
            
            var    filename = FileUtility.GetSApFile(masterdataEntity);
            if (string.IsNullOrEmpty(filename))
            {
                Alert("masterdata file path cannot be determined..check config settings");
                return;
            }
            try
            {
                using (var fs = new FileStream(filename, FileMode.OpenOrCreate))
                {
                    fs.Close();
                    using (var wr = new StreamWriter(filename, false))
                    {
                        wr.WriteLine(file);

                    }
                }


            }
            catch (IOException ex)
            {

            }
        }

        static void DumpExportInventoryFileAsync(string file, string masterdataEntity)
        {

            var filename = FileUtility.GetInventoryFile(masterdataEntity);
            if (string.IsNullOrEmpty(filename))
            {
                Alert("masterdata file path cannot be determined..check config settings");
                return;
            }
            try
            {
                using (var fs = new FileStream(filename, FileMode.OpenOrCreate))
                {
                    fs.Close();
                    using (var wr = new StreamWriter(filename, false))
                    {
                        wr.WriteLine(file);

                    }
                }


            }
            catch (IOException ex)
            {

            }
        }
        public class InventoryImport
        {
           // public string FromSite { get; set; }
            public string ProductName { get; set; }
            public decimal Balance { get; set; }
            public string ToSiteName { get; set; }
        }

        class OutletImport
        {
            public string OutletCode { get; set; }
            public string OutletName { get; set; }
            public string DistributrCode { get; set; }
            public string RouteCode { get; set; }
            public string OutletCategory { get; set; }
            public string Outletype { get; set; }
            public string DiscountGroupCode { get; set; }
            public string PricingTierCode { get; set; }
            public string VatClassCode { get; set; }
        }
        class RouteRegionInfo
        {
            public string Region { get; set; }
            public string Salemanref { get; set; }
        }

        class Packaging
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
        }

       public class ProductImport
        {
            public string pCode { get; set; }
            public string pDesc { get; set; }
            public decimal exfactory { get; set; }
            public string productTypeName { get; set; }
            public string productBrandName { get; set; }
            public string flavourname { get; set; }
            public string packagingTypeName { get; set; }
            public string vatclassName { get; set; }
             
        }

        class ProductPricing
        {
            public string ProductCode { get; set; }
            public string TierNameCode { get; set; }
            public decimal SellingPrice { get; set; }
            public string StartDate { get; set; }
            public decimal Exfactory { get; set; }
        }

        class VATImport
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public decimal Rate { get; set; }
            public DateTime EffectiveDate { get; set; }
        }

        class ProductBrand
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string supplierName { get; set; }
        }
       
        class Salesman
        {
            
            public string name { get; set; }
            public string salesmanCode { get; set; }
            public string distributrCode { get; set; }
            public string mobileNumber { get; set; }
        }

        class DistributorImport
        {
            public string DistributorCode { get; set; }
            public string Name { get; set; }
            public string RegionName { get; set; }
            public string Pin { get; set; }
            public string VatRegNo { get; set; }
            public string PayBillNo { get; set; }
            public string MerchantNo { get; set; }
            public string Latitude { get; set; }
            public string Longitude { get; set; }

        }

        internal static void GenerateMasterData()
        {
            ProductPackagingAndPackagingType();
            PullCustomers();
            Products();

            GetSalesmenAndRoutes();
            VATClassList();

            /*
            //GetDistributors();
           //PullInventory();
             */


        }

       
    }

   
}
