using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Windows;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IIntegrationDocumentRepository;
using Distributr.WSAPI.Lib.Integrations;
using Integration.QuickBooks.Lib.QBIntegrationViewModels;
using QBFC13Lib;

namespace Integration.QuickBooks.Lib.QBIntegrationCore
{
    public class QBIntegrationMethods
    {
        public static void SetQBCompanyFilePath(string filePath)
        {
            QBFC_Core.qdbpath = ConfigurationSettings.AppSettings["QuickBooksCompanyFilePath"];
        }

        public static bool AddOutlet(ImportEntity outlet)
        {
            ICustomerRet customerRet = QBFC_Core.QBAddCustomer(outlet);
            return customerRet != null;
        }

        public static bool AddProduct(ImportEntity product, string cogsAccountRef, string assetAccountRef, string incomeAccountRef)
        {
            string productcode = product.Fields[0];
            string productDescription = product.Fields[1];
            double exfactoryPrice = Convert.ToDouble(product.Fields[2]);
            IItemInventoryRet itemInventoryRet = QBFC_Core.QBAddInventory(productcode, productDescription,
                                                                          Convert.ToDouble(exfactoryPrice),cogsAccountRef, assetAccountRef, incomeAccountRef);
            return itemInventoryRet != null;
        }


        public static ISalesOrderRet AddOrder(QuickBooksOrderDocumentDto order, string externalOrderRef)
        {
            ISalesOrderRet salesOrderRet = QBFC_Core.QBAddSalesOrder(order, externalOrderRef);
            return salesOrderRet;
        }

        public static IInvoiceRet AddInvoice(QuickBooksOrderDocumentDto invoice, string qbSaleOrderTxnId, string saleOrderCustomerName, string externalInvoiceRef, string account)
        {
            IInvoiceRet invoiceRet = QBFC_Core.QBAddInvoice(invoice, qbSaleOrderTxnId, saleOrderCustomerName, externalInvoiceRef, account);
            return invoiceRet;
        }

        public static ISalesReceiptRet AddSaleReceipt(QuickBooksOrderDocumentDto receipt)
        {
            ISalesReceiptRet salesReceiptRet = QBFC_Core.QBAddSalesReceipt(receipt);
            return salesReceiptRet;
        }


        public static IReceivePaymentRet AddPayment(QuickBooksOrderDocumentDto receipt, string qbInvoiceTxnId, string accountRef,List<string> references)
        {
            IReceivePaymentRet salesReceiptRet = QBFC_Core.QbAddPayment(receipt, qbInvoiceTxnId, accountRef, references);
            return salesReceiptRet;
        }
        public static IAccountRet AddAccount(QBAccount account)
        {
            ENAccountType enAccountType = ENAccountType.atCostOfGoodsSold;
            switch(account.AccountType)
            {
                case QBAccountType.COGSAccount:
                    enAccountType = ENAccountType.atCostOfGoodsSold;
                    break;
                case QBAccountType.CurrentAssetsAccount:
                    enAccountType = ENAccountType.atOtherCurrentAsset;
                    break;
                case QBAccountType.IncomeAccount:
                    enAccountType = ENAccountType.atIncome;
                    break;
            }
            IAccountRet accountRet = QBFC_Core.QBAddAccount(account.AccountName, true, enAccountType,
                                                            account.AccountNumber);
            return accountRet;
        }

        public static IAccountRetList GetCOGSAccounts()
        {
            IAccountRetList cogsAccountList = QBFC_Core.QBGetChartOfAccountAccount(ENAccountType.atCostOfGoodsSold);
            return cogsAccountList;
        }

        public static IAccountRetList GetIncomeAccounts()
        {
            IAccountRetList accountList = QBFC_Core.QBGetChartOfAccountAccount(ENAccountType.atIncome);
            return accountList;
        }

        public static IAccountRetList GetAssetAccounts()
        {
            IAccountRetList accountList = QBFC_Core.QBGetChartOfAccountAccount(ENAccountType.atOtherCurrentAsset);
            return accountList;
        }

        public static IAccountRetList GetReceivableAccounts()
        {
            IAccountRetList accountList = QBFC_Core.QBGetChartOfAccountAccount(ENAccountType.atAccountsReceivable);
            return accountList;
        }

        public static bool CustomerExist(string outletName)
        {
            return QBFC_Core.GetCustomerByNameFilter(outletName) != null;
        }

        public static bool ProductExist(string productName)
        {
            return QBFC_Core.GetInventoryByNameFilter(productName) != null;
        }

        public  static ITransferInventoryRet ReturnInventory(QuickBooksReturnInventoryDocumentDto documentDto)
        {
            return QBFC_Core.ReturnInventory(documentDto);
        }
    }
}
