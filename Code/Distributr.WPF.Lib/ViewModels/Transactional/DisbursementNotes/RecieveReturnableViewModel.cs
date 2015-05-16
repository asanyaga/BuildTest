using System;
using System.Collections.ObjectModel;
using System.Windows;
using Distributr.Core.Domain.InventoryEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Factory.Documents;
using Distributr.Core.Repository.InventoryRepository;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Service.Utility;
using Distributr.WPF.Lib.Services.Service.Utility;
using GalaSoft.MvvmLight;
using System.Linq;
using Distributr.Core.ClientApp;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Transactional.DisbursementNotes
{

    public class RecieveReturnableViewModel : DistributrViewModelBase
    {
        public RelayCommand SaveCommand { set; get; }
        public RecieveReturnableViewModel()
        {
          
            SaveCommand = new RelayCommand(Save);

            ReturnableItems = new ObservableCollection<Returnable>();
            OutletLookupList = new ObservableCollection<OutletLookup>();
        }

        public void Save()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                BasicConfig config = c.GetInstance<IConfigService>().Load();
                if (ReturnableItems.Count == 0)
                {
                    MessageBox.Show("Add Returnable Product first");
                    return;
                }
                else
                {
                    try
                    {
                        CostCentre current = Using<ICostCentreRepository>(c) .GetById( Using<IConfigService>(c).Load().CostCentreId);
                        User user = Using<IUserRepository>(c).GetById(Using<IConfigService>(c).ViewModelParameters.CurrentUserId);
                        CostCentre outlet = Using<ICostCentreRepository>(c).GetById(OutletLookups.OutletId);
                        Guid appId = Using<IConfigService>(c) .Load().CostCentreApplicationId;
                        IInventoryReceivedNoteFactory f = Using<IInventoryReceivedNoteFactory>(c);
                        string DocumentReference = "";
                        string LoadNo = "";
                        string OrderReferences = "returnable";
                        Guid irnid = Guid.NewGuid();

                        InventoryReceivedNote grn = f.Create(current, appId, current, outlet, LoadNo, OrderReferences,
                                                             user, DocumentReference, irnid);
                           
                        foreach (var item in ReturnableItems)
                        {
                            InventoryReceivedNoteLineItem li = f.CreateLineItem(item.ProductId, item.Quantity,
                                                                                item.UnitPrice, item.Name, 0);
                            grn.AddLineItem(li);
                        }
                        DisbursementNote disbursementnote = new DisbursementNote(Guid.NewGuid())
                            {
                                DocumentDateIssued = DateReceived,
                                DocumentIssuerCostCentre = current,
                                DocumentIssuerCostCentreApplicationId = appId,
                                DocumentIssuerUser = user,
                                DocumentRecipientCostCentre = current,
                                DocumentReference = DocumentReference,
                                DocumentType = DocumentType.DisbursementNote,
                                Status = DocumentStatus.New
                            };
                        foreach (var item in ReturnableItems)
                        {
                            DisbursementNoteLineItem li = new DisbursementNoteLineItem(Guid.NewGuid())
                                {
                                    IsNew = true,
                                    Description = item.Name,
                                    LineItemSequenceNo = 0,
                                    Product = Using<IProductRepository>(c).GetById(item.ProductId),
                                    Qty = item.Quantity,
                                    Value = item.UnitPrice
                                };
                            disbursementnote.AddLineItem(li);
                        }
                        Guid id = Guid.NewGuid();
                        var ian1 = Using<IInventoryAdjustmentNoteFactory>(c)
                            .Create(current, appId, current, user, Guid.NewGuid().ToString(),
                                    InventoryAdjustmentNoteType.Returns, Guid.Empty);
                        
                        foreach (var item in ReturnableItems)
                        {
                            Inventory inve = Using<IInventoryRepository>(c).GetByProductIdAndWarehouseId(item.ProductId, current.Id);
                            int inveValue = 0;
                            if (inve != null)
                                inveValue = item.Quantity;

                            InventoryAdjustmentNoteLineItem li = Using<IInventoryAdjustmentNoteFactory>(c)
                                    .CreateLineItem(item.Quantity + inveValue, item.ProductId, 0, item.UnitPrice,
                                                    item.Name);
                                ian1.AddLineItem(li);
                        }
                        ian1.Confirm();
                        grn.Confirm();
                        disbursementnote.Confirm();
                        //Using<IInventoryAdjustmentNoteWfManager>(c).SubmitChanges(ian1);
                        Using<IProducerIRNWFManager>(c).SubmitChanges(grn,config);
                        Using<IConfirmDisbursementNoteWorkFlow>(c).SubmitChanges(disbursementnote,config);

                        ClearAll();
                        if (
                            MessageBox.Show(
                                "Confirmed successfully,Click OK to view summary. Click Cancel to continue receiving Returnables",
                                "Receive Returnable ", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        {
                            SendNavigationRequestMessage(
                                new Uri("/views/Reports/InventoryAdjustmentsReport.xaml?current=" + ian1.Id,
                                        UriKind.Relative));
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }


                }
            }
        }

       

        public void ClearAll()
        {
            ReturnableItems.Clear();
            TotalAmount = 0;
            OutletLookups = null;
            DateReceived = DateTime.Now;
            IsOutletEnabled = true;
        }

        public void Setup()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                DateReceived = DateTime.Now;
                ReturnableItems.Clear();
                OutletLookupList.Clear();
                var outlet = new OutletLookup
                    {
                        Name = GetLocalText("sl.inventory.receive.returnable.select.outlet"),
                        OutletId = Guid.Empty
                    };
                OutletLookupList.Add(outlet);
                OutletLookups = outlet;

                Using<ICostCentreRepository>(c).GetAll().OfType<Outlet>().OrderBy(n => n.Name).ToList().ForEach(
                    n => OutletLookupList.Add(new OutletLookup {OutletId = n.Id, Name = n.Name}));
                IsOutletEnabled = true;
            }
        }

        public void DeleteReturns(Guid id)
        {
            Returnable rs = ReturnableItems.First(p => p.ProductId == id);
            ReturnableItems.Remove(rs);
            if (ReturnableItems.Count == 0)
                IsOutletEnabled = true;
            MessageBox.Show("Returnable entry deleted", "Distributr: Recieve Returnable", MessageBoxButton.OK);

        }

        public void AddLineItems(ProductLookup product, int quantity, bool isNew)
        {
            using (StructureMap.IContainer c = NestedContainer)
            {

                ProductPricingTier tier = null;
                var outlet = Using<ICostCentreRepository>(c).GetAll().FirstOrDefault(s => s.Id == OutletLookups.OutletId) as Outlet;
                if (outlet != null)
                    tier = Using<IProductPricingTierRepository>(c).GetById(outlet.OutletProductPricingTier.Id);
                try
                {

                    Returnable returnablez;
                    if (ReturnableItems.Any(a => a.ProductId == product.ProductId))
                    {
                        returnablez = ReturnableItems.First(a => a.ProductId == product.ProductId);
                        if (isNew)
                        {
                            returnablez.Quantity += quantity;
                            returnablez.Value = (returnablez.Quantity*returnablez.UnitPrice);
                        }
                        else
                        {
                            returnablez.Quantity = quantity;
                            returnablez.Value = (returnablez.Quantity*returnablez.UnitPrice);
                        }
                    }
                    else
                    {
                        Product prod = Using<IProductRepository>(c).GetById(product.ProductId);

                        decimal unitPrice = 0;
                        try
                        {
                            if (tier != null)
                                unitPrice = prod.ProductPrice(tier);

                        }
                        catch
                        {
                            unitPrice = 0;
                        }
                        ReturnableItems.Add(new Returnable
                            {
                                UnitPrice = unitPrice,
                                Name = product.Name,
                                ProductId = product.ProductId,
                                Quantity = quantity,
                                Value = (unitPrice*quantity)
                            });
                    }

                    if (IsOutletEnabled == true)
                        IsOutletEnabled = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        public ObservableCollection<Returnable> ReturnableItems { set; get; }
        public ObservableCollection<OutletLookup> OutletLookupList { set; get; }
        public const string TotalAmountPropertyName = "TotalAmount";
        private decimal _totalamount = 0;
        public decimal TotalAmount
        {
            get
            {
                return _totalamount;
            }

            set
            {
                if (_totalamount == value)
                {
                    return;
                }

                var oldValue = _totalamount;
                _totalamount = value;
                // Update bindings, no broadcast
                RaisePropertyChanged(TotalAmountPropertyName);

            }
        }

        public const string OutletLookupsPropertyName = "OutletLookups";
        private OutletLookup _outletlookups = null;
        [MasterDataDropDownValidation]
        public OutletLookup OutletLookups
        {
            get
            {
                return _outletlookups;
            }

            set
            {
                if (_outletlookups == value)
                {
                    return;
                }

                var oldValue = _outletlookups;
                _outletlookups = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(OutletLookupsPropertyName);

            }
        }


        public const string DateReceivedPropertyName = "DateReceived";
        private DateTime _dateReceived;
        public DateTime DateReceived
        {
            get
            {
                return _dateReceived;
            }

            set
            {
                if (_dateReceived == value)
                {
                    return;
                }

                var oldValue = _dateReceived;
                _dateReceived = value;
                // Update bindings, no broadcast
                RaisePropertyChanged(DateReceivedPropertyName);

            }
        }


       
        public const string IsOutletEnabledPropertyName = "IsOutletEnabled";
        private bool _IsOutletEnabled = true;
        public bool IsOutletEnabled
        {
            get
            {
                return _IsOutletEnabled;
            }

            set
            {
                if (_IsOutletEnabled == value)
                {
                    return;
                }

                var oldValue = _IsOutletEnabled;
                _IsOutletEnabled = value;

             

                // Update bindings, no broadcast
                RaisePropertyChanged(IsOutletEnabledPropertyName);

             
            }
        }
    }
    public class OutletLookup
    {
        public Guid OutletId { get; set; }
        public string Name { get; set; }
    }
    public class ProductLookup
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; }
    }
    public class Returnable:ViewModelBase
    {
       
        public const string UnitPricePropertyName = "UnitPrice";
        private decimal _UnitPrice = 0;
        public decimal UnitPrice
        {
            get
            {
                return _UnitPrice;
            }

            set
            {
                if (_UnitPrice == value)
                {
                    return;
                }

                var oldValue = _UnitPrice;
                _UnitPrice = value;
              

                // Update bindings, no broadcast
                RaisePropertyChanged(UnitPricePropertyName);

               
            }
        }

        public const string ValuePropertyName = "Value";
        private decimal _Value = 0;
        public decimal Value
        {
            get
            {
                return _Value;
            }

            set
            {
                if (_Value == value)
                {
                    return;
                }

                var oldValue = _Value;
                _Value = value;

             

                // Update bindings, no broadcast
                RaisePropertyChanged(ValuePropertyName);

              
            }
        }

       
        public const string QuantityPropertyName = "Quantity";
        private int _Quantity = 0;
        public int Quantity
        {
            get
            {
                return _Quantity;
            }

            set
            {
                if (_Quantity == value)
                {
                    return;
                }

                var oldValue = _Quantity;
                _Quantity = value;

              

                // Update bindings, no broadcast
                RaisePropertyChanged(QuantityPropertyName);

              
            }
        }

       
        public const string ProductIdPropertyName = "ProductId";
        private Guid _ProductId = Guid.Empty;
        public Guid ProductId
        {
            get
            {
                return _ProductId;
            }

            set
            {
                if (_ProductId == value)
                {
                    return;
                }

                var oldValue = _ProductId;
                _ProductId = value;


                // Update bindings, no broadcast
                RaisePropertyChanged(ProductIdPropertyName);

             
            }
        }

        public const string NamePropertyName = "Name";
        private string _Name = "";
        public string Name
        {
            get
            {
                return _Name;
            }

            set
            {
                if (_Name == value)
                {
                    return;
                }

                var oldValue = _Name;
                _Name = value;

              

                // Update bindings, no broadcast
                RaisePropertyChanged(NamePropertyName);

               
            }
        }

        /// <summary>
        /// The <see cref="IsEditable" /> property's name.
        /// </summary>
        public const string IsEditablePropertyName = "IsEditable";
        private bool _IsEditable = true;
        public bool IsEditable
        {
            get
            {
                return _IsEditable;
            }

            set
            {
                if (_IsEditable == value)
                {
                    return;
                }

                var oldValue = _IsEditable;
                _IsEditable = value;
                // Update bindings, no broadcast
                RaisePropertyChanged(IsEditablePropertyName);

               
            }
        }
    }
    public class RecieveReturnableLineItemViewModel : DistributrViewModelBase
    {
        public RecieveReturnableLineItemViewModel()
        {
           ProductLookupList = new ObservableCollection<ProductLookup>();
        }

        public ObservableCollection<ProductLookup> ProductLookupList { get; set; }
        public void SetUp()
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                ProductLookupList.Clear();
                var def = new ProductLookup {Name = "--Select Returnable Product--", ProductId = Guid.Empty};
                ProductLookupList.Add(def);
                Using<IProductRepository>(cont).GetAll().OfType<ReturnableProduct>()
                    .OrderBy(o => o.Description).ToList()
                    .ForEach(n => ProductLookupList.Add(new ProductLookup {Name = n.Description, ProductId = n.Id}));
                ProductLookups = def;
                Quantity = 0;
                IsEditable = true;
            }
        }
        public void EditLoad(Returnable returnable)
        {

            Quantity = returnable.Quantity;
            ProductLookups = ProductLookupList.First(s => s.ProductId == returnable.ProductId);
            IsEditable = false;
        }
       
        public const string ProductLookupsPropertyName = "ProductLookups";
        private ProductLookup _productLookUps = null;
        public ProductLookup ProductLookups
        {
            get
            {
                return _productLookUps;
            }

            set
            {
                if (_productLookUps == value)
                {
                    return;
                }

                var oldValue = _productLookUps;
                _productLookUps = value;

               
                // Update bindings, no broadcast
                RaisePropertyChanged(ProductLookupsPropertyName);

              
            }
        }
       
        public const string QuantityPropertyName = "Quantity";
        private int _quantity = 0;
        public int Quantity
        {
            get
            {
                return _quantity;
            }

            set
            {
                if (_quantity == value)
                {
                    return;
                }

                var oldValue = _quantity;
                _quantity = value;

             

                // Update bindings, no broadcast
                RaisePropertyChanged(QuantityPropertyName);

            }
        }

      
        public const string IsEditablePropertyName = "IsEditable";

        private bool _IsEditable = false;

        public bool IsEditable
        {
            get
            {
                return _IsEditable;
            }

            set
            {
                if (_IsEditable == value)
                {
                    return;
                }

                var oldValue = _IsEditable;
                _IsEditable = value;

              

                // Update bindings, no broadcast
                RaisePropertyChanged(IsEditablePropertyName);

              
            }
        }
       
    }
}