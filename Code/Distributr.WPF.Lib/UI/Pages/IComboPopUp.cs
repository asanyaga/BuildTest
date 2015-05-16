using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CommodityEntities;
using Distributr.Core.Domain.Master.CommodityEntity;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.DistributorTargetEntities;
using Distributr.Core.Domain.Master.EagcLogin;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Master.SuppliersEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;


namespace Distributr.WPF.Lib.UI.Pages
{
  public  interface IComboPopUp
  {
      object ShowDlg(object sender);
      object ShowDlg(IEnumerable<object> objects);
  }

    public interface IItemsLookUp
    {
        Guid ShowDlg(Type objectType, int defaultTake = 50);
        CostCentre SelectUnderbankingCostCentre(Guid salesmanId);
        CostCentre SelectDistributrSalesman();

       

        CostCentre SelectStockistDistributrSalesman();
        Outlet SelectOutletToMapToSupplier();
        Guid IssueInventory(Guid? salesmanid = null);

        Route SelectRoute(Guid salesmanId);
        Route SelectRoute();

       /* Contact SelectContact(int contactOwnerType);*/
        CostCentre SelectContactOwner (int contactOwnerType, Guid costCenId);
        ContactType SelectContactType();


        User SelectSelectedSalesman();

        Outlet SelectOutletByRoute(Guid routeId);

        Outlet SelectOutletBySalesman(Guid salesmanId);

        Guid SelectOutletShipToAddress(Guid outletId);

        TargetPeriod SelectTargetPeriod();


        Warehouse SelectDistribtrWarehouse(Guid? parentCostCentreId);

        Supplier SelectSupplier();
        Product SelectProduct(Guid? supplierId);
    }

    public interface IItemsEnumLookUp
    {
        ContactOwnerType SelectContactOwnerType();
        MaritalStatas SelectMaritalStatus();


       
    }

    public interface IAgriItemsLookUp
    {
       
        Outlet SelectOutletToMapToSupplier();
        Warehouse SelectWarehouse();
        Commodity SelectCommodity();
        CommodityOwner SelectFarmer();
        CommodityOwner SelectFarmersBySupplier(Guid supplierId);

        Store SelectStore();

        CommodityGrade SelectGrade(Guid commodityId);
        CommoditySupplier SelectCommoditySupplier();
    }


    public interface IAgrimanagrComboPopUp
    {
        object ShowDlg(object sender);
    }

    public interface IReceiptDocumentPopUp
    {
        void ShowReceipt(CommodityPurchaseNote commodityPurchaseNote);
    }

    public interface IReleaseDocumentPopUp
    {
        void ShowReleaseDocument(CommodityReleaseNote retreavcommodityReleaseNote);
    }
    public interface IWeighAndReceivePopUp
    {
        void ShowWeighAndReceive(Guid documentId);
    }
    public interface IFarmerOutletMapping
    {
        void SupplierToOutletToMappping(CommoditySupplier supplier);
    }
    
    

    public interface IStoreCommodityPopUp
    {
        void ShowCommodityToStore(List<Guid> items);
    }
    public interface IInventoryTransferPopUp
    {
        void ShowCommodityTransfer();
    }



    public interface ILoginPopup
    {

        LoginDetail ShowLoginPopup();
        LoginDetail CloseLoginPopup();
    }
}
