using System;
using System.Collections.Generic;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.SuppliersEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.ViewModels.Transactional.GRN;
using Distributr.WPF.Lib.ViewModels.Transactional.Orders;

namespace Distributr.WPF.Lib.UI.Pages
{
  public  interface IOrderProductPage
  {
      List<ProductPopUpItem> GetProduct(Outlet outlet,OrderType orderType);
      List<ProductPopUpItem> GetProductBySupplier(Outlet outlet, OrderType orderType, Supplier supplier);
       List<ProductPopUpItem> GetProduct(Outlet outlet,OrderType orderType,List<ProductPopUpItem> existing );
      List<ProductPopUpItem> GetReturnables(Outlet outlet,Dictionary<Guid,int> returnableIds, OrderType orderType);
      List<ProductPopUpItem> EditProduct(Outlet outlet, Guid productId, decimal quantity, OrderType orderType);
      void Close();
     
  }

    public interface IGrnPopUp
    {
        GRNModalItems AddGrnItems();
        GRNModalItems EditGrnItems(AddGrnLineItemViewModel lineItem,List<ProductSerialNumbers> productSerials);
    }

    public interface IPaymentPopup
    {
        List<PaymentInfo> GetPayments(decimal amountToPay,Guid orderId);
        List<PaymentInfo> GetPayments(decimal amountToPay, string orderdocumentReference = "", string invoiceref = "");
        
    }
    public interface IUnderBankingPopUp
    {
        bool AddUnderBanking();
       
    }
    public interface IUnderBankingConfirmationPopUp
    {
        bool ShowPaymentInfor();

    }
}
