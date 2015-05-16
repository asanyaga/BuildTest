using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Repository.InventoryRepository;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Resources.Util;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Service.Utility;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.Services.WorkFlow.Discount;
using Distributr.WPF.Lib.UI.Hierarchy;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.Linq;
using Distributr.Core.Domain.Master.ProductEntities;

namespace Distributr.WPF.Lib.ViewModels.Transactional.SalesmanOrders
{
    public class AmmendSalesmanOrderLineItemViewModel : DistributrViewModelBase
    {
        
        public List<LineItem> AddedLineItems = null;
        
        public string DocumentStatus;

        public AmmendSalesmanOrderLineItemViewModel()
        {
            
           

            //Setup();
            ProductSelected = new RelayCommand(RunProductSelected);
            ClearAndSetup = new RelayCommand(RunClearAndSetup);
            txtQtyGotFocusCommand = new RelayCommand(RuntxtQtyGotFocusCommand);
            txtBackOrderGotFocusCommand = new RelayCommand(RuntxtBackOrderGotFocusCommand);
            txtLostSaleGotFocusCommand = new RelayCommand(RuntxtLostSaleGotFocusCommand);
            txtApproveGotFocusCommand = new RelayCommand(RuntxtApproveGotFocusCommand);
            LineItems = new ObservableCollection<LineItem>();
            SellInBulkSelected = new RelayCommand(RunSellInBulkSelected);
            SellInUnitsSelected = new RelayCommand(RunSellInUnitsSelected);
        }

        #region Declarations
        public RelayCommand txtQtyGotFocusCommand { get; set; }
        public RelayCommand txtBackOrderGotFocusCommand { get; set; }
        public RelayCommand txtLostSaleGotFocusCommand { get; set; }
        public RelayCommand txtApproveGotFocusCommand { get; set; }
        public ObservableCollection<Product> LoadedProducts { get; set; }
        public ObservableCollection<LineItem> LineItems { get; set; }

        public RelayCommand SellInBulkSelected { get; set; }
        public RelayCommand SellInUnitsSelected { get; set; }

        public LineItem SaleProduct = null;
        public LineItem BulkSaleContainer = null;//e.g crate
        public LineItem SaleProductReturnable = null; //e.g bottle
        public LineItem UIProduct = null; //presentational
        #endregion

        public bool _atApproval = false;

        #region Methods
        void RuntxtQtyGotFocusCommand()
        {
            //RecalcTotal(Qty);
            SelectedActionDetails =
                "This is the quantity which can be satisfied by the available inventory.\nI.e Required Quantity <= Available Quantity.";
        }

        //void RuntxtAwaitingStockGotFocusCommand()
        //{
        //    RecalcTotal(BackOrder);
        //    SelectedActionDetails = "Awaiting Stock = Quantity To Approve - Required Quantity.";
        //}

        void RuntxtBackOrderGotFocusCommand()
        {
            //RecalcTotal(UIProduct);
            if (!ProcessingBackOrder)
                //SelectedActionDetails =
                //    "Back Order = Quantity To Approve - Original Required Quantity.";
                SelectedActionDetails =
                  "Back Order = Quantity To Approve - The Quantity Available.";
            else
                SelectedActionDetails =
                    "Back Order = Quantity To Approve - The Quantity Available.";
        }

        void RuntxtLostSaleGotFocusCommand()
        {
            //RecalcTotal(UIProduct);
            SelectedActionDetails = "LostSale = Original Required Qty (" + OriginalQty + ") - Approved Quantity";
        }

        void RuntxtApproveGotFocusCommand()
        {
            //RecalcTotal(Approve);
        }

        public void Setup()
        {
            using (var container = NestedContainer)
            {
                IProductRepository _productService = Using<IProductRepository>(container);
                IInventoryRepository _inventoryService = Using<IInventoryRepository>(container);
               
                IConfigService _configService = Using<IConfigService>(container);
               


                SellInUnits = true;
                //Products = new ObservableCollection<AmmendOrderLineItemProductLookupItem>();
                Products = new ObservableCollection<OrderLineItemProductLookupItem>();
                var ccId = _configService.Load().CostCentreId;
                var product = new OrderLineItemProductLookupItem()
                                  {
                                      ProductId = Guid.Empty,
                                      ProductDesc =
                                         GetLocalText("sl.order.addlineitems.modal.selectProduct"),
                                      /*"--Please Select a Product--",*/
                                      PackagingId = Guid.Empty,
                                      PackagingTypeId = Guid.Empty,
                                  };
                Products.Add(product);
                SelectedProduct = product;
                switch (ProductTypeToLoad)
                {
                    case ProducTypeToLoad.AllProducts:
                        LoadedProducts = new ObservableCollection<Product>();
                        _productService.GetAll().ToList().ForEach(n => LoadedProducts.Add(n));
                        LoadedProducts.OrderBy(n => n.Description).ToList().ForEach(
                            n => Products.Add(new OrderLineItemProductLookupItem
                                                  {
                                                      ProductId = n.Id,
                                                      ProductDesc = n.Description,
                                                      ProductCode = n.ProductCode,
                                                      PackagingId = n.Packaging == null ? Guid.Empty : n.Packaging.Id,
                                                      PackagingTypeId =
                                                          n.PackagingType == null ? Guid.Empty : n.PackagingType.Id,
                                                      ProductType = n.GetType().ToString().Split('.').Last(),
                                                      AvailableInventory =
                                                          _inventoryService.GetByProductIdAndWarehouseId(n.Id, ccId) != null
                                                              ? _inventoryService.GetByProductIdAndWarehouseId(n.Id, ccId).Balance
                                                              : 0,
                                                  }));
                        break;
                    case ProducTypeToLoad.NonReturnables:
                        LoadedProducts = new ObservableCollection<Product>();
                        _productService.GetAll().Where(n => n.GetType() != typeof (ReturnableProduct)).ToList().ForEach(
                            n => LoadedProducts.Add(n));

                        LoadedProducts.OrderBy(n => n.Description).ToList().ForEach(
                            n => Products.Add(new OrderLineItemProductLookupItem
                                                  {
                                                      ProductId = n.Id,
                                                      ProductDesc = n.Description,
                                                      ProductCode = n.ProductCode,
                                                      PackagingId = n.Packaging == null ? Guid.Empty : n.Packaging.Id,
                                                      PackagingTypeId =
                                                          n.PackagingType == null ? Guid.Empty : n.PackagingType.Id,
                                                      ProductType = n.GetType().ToString().Split('.').Last(),
                                                      AvailableInventory =
                                                          _inventoryService.GetByProductIdAndWarehouseId(n.Id, ccId).Balance,
                                                  }));
                        break;
                    case ProducTypeToLoad.Returnables:
                        LoadedProducts = new ObservableCollection<Product>();
                        _productService.GetAll().OfType<ReturnableProduct>().ToList().ForEach(n => LoadedProducts.Add(n));
                        LoadedProducts.OrderBy(n => n.Description).ToList()
                            .ForEach(n => Products.Add(new OrderLineItemProductLookupItem
                                                           {
                                                               ProductId = n.Id,
                                                               ProductDesc = n.Description,
                                                               ProductCode = n.ProductCode,
                                                               PackagingId =
                                                                   n.Packaging == null ? Guid.Empty : n.Packaging.Id,
                                                               PackagingTypeId =
                                                                   n.PackagingType == null
                                                                       ? Guid.Empty
                                                                       : n.PackagingType.Id,
                                                               ProductType = n.GetType().ToString().Split('.').Last(),
                                                               AvailableInventory =
                                                                   _inventoryService.GetByProductIdAndWarehouseId(n.Id, ccId).Balance,
                                                           }));
                        break;
                }
                if (ProductTypeToLoad == ProducTypeToLoad.Returnables)
                    ModalCrumbs = "Distributr: Receive Returnable Products from " + Salesman.Username + "";
                else
                    try
                    {
                        ModalCrumbs = "Distributr: Add Products to " + Salesman.Username + "'s Sale";
                    }
                    catch
                    {
                    }
                CapacityValueApplied = false;
            }
        }

        public RelayCommand ProductSelected { get; set; }

        void RunProductSelected()
        {
            using (var container = NestedContainer)
            {
                 IConfigService _configService = Using<IConfigService>(container);
              
                if (SelectedProduct.ProductId == Guid.Empty)
                {
                    Clear();
                    return;
                }

                try
                {
                    var ccId = _configService.Load().CostCentreId;
                    var product = LoadedProducts.FirstOrDefault(n => n.Id == SelectedProduct.ProductId);

                    //Inventory productInv = null;
                    //if (SelectedProduct.ProductId > 0)
                    //    productInv = _inventoryService.GetByProductId(SelectedProduct.ProductId, ccId);

                    //AvailableProductInv = productInv == null ? 0 : productInv.Balance;

                    //if (SelectedProduct.ProductId > 0 && AddedLineItems != null && AddedLineItems.Count > 0)
                    //{
                    //    try
                    //    {
                    //        var item = AddedLineItems.First(n => n.ProductId == SelectedProduct.ProductId);
                    //        if (ProductTypeToLoad != ProducTypeToLoad.Returnables)
                    //            AvailableProductInv -= item.LineItemQty;
                    //    }
                    //    catch (Exception)
                    //    {
                    //    }
                    //}

                    PriceCalc(product);
                    //GetMyReturnable();
                    CanSellInBulk = !(product is ConsolidatedProduct);
                    if (!CanSellInBulk)
                        SellInUnits = true;


                    //if (SellInBulk)
                    //    RunSellInUnitsSelected();
                    if (SellInBulk)
                        RunSellInBulkSelected();
                    else if (SellInUnits)
                        RunSellInUnitsSelected();
                }
                catch
                {
                }
            }
        }

        void RunSellInBulkSelected()
        {
            if (SelectedProduct.ProductType == "ConsolidatedProduct")
            {
                RecalcTotal();
            }
            else
            {
                GetRequiredQty();
                //RecalcTotal();
                FireAllEvents();
            }
        }

        void RunSellInUnitsSelected()
        {
            if (SelectedProduct.ProductType == "ConsolidatedProduct" || SelectedProduct.ProductType == "ReturnableProduct" )
            {
                RequiredQty = Approve;
                RecalcTotal();
            }
            else
            {
                GetRequiredQty();
                //RecalcTotal();
                FireAllEvents();
            }
        }

        void FireAllEvents()
        {
            if (SelectedProduct != null && SelectedProduct.ProductId != Guid.Empty)
            {
                BuildAllProducts();
                BringToWall(UIProduct);
            }
        }

        void BuildAllProducts()
        {
            SaleProduct = null;
            BulkSaleContainer = null; //e.g crate
            SaleProductReturnable = null; //e.g bottle
            UIProduct = null;

            var product = LoadedProducts.First(n => n.Id == SelectedProduct.ProductId);
            if (product.GetType() == typeof(SaleProduct))
                SaleProduct = BuildSaleProduct();
            SaleProductReturnable = BuildSaleProductReturnable();
            BulkSaleContainer = BuildBulkSaleContainer();
            UIProduct = BuildUIProduct();
        }

        void GetBulkSaleContainerCapacity()
        {
            if (BulkSaleContainer == null)
                BulkSaleContainer = BuildBulkSaleContainer();
            try
            {
                CrateCapacity = BulkSaleContainer.Capacity;
            }
            catch
            {
                CrateCapacity = 1;
            }
        }

        void GetRequiredQty()
        {
            if (SelectedProduct != null && SelectedProduct.ProductId != Guid.Empty)
                GetBulkSaleContainerCapacity(); //will set crate capacity.
            else
                CrateCapacity = 1;

            if (SellInUnits)
            {
                RequiredQty = Approve;
            }
            if (SellInBulk)
            {
                RequiredQty = Approve * CrateCapacity;
            }
        }

        public void SetRequiredQtyOnEdit()
        {
            RequiredQty = Qty;
            GetBulkSaleContainerCapacity();
            Qty = RequiredQty / CrateCapacity;
        }

        private LineItem BuildSaleProduct()
        {
            Product product = LoadedProducts.First(n => n.Id == SelectedProduct.ProductId);
            LineItem saleProduct = new LineItem
            {
                ProductId = SelectedProduct.ProductId,
                ProductDesc = SelectedProduct.ProductDesc,
                ProductType = SelectedProduct.ProductType,
                Qty = RequiredQty,
                IsNew = IsNew,
                BackOrder = BackOrder,
                LostSale = LostSale,
                ReturnableId =
                    ((SaleProduct)product).ReturnableProduct == null
                        ? Guid.Empty
                        : ((SaleProduct)product).ReturnableProduct.Id,
                SequenceNo = LineItems.Count() + 1
            };
            saleProduct = PriceCalc(product, saleProduct);
            return saleProduct;
        }

        private LineItem BuildSaleProductReturnable()
        {
            ReturnableProduct myReturnable = null;
            ReturnableProduct myCrate = null;
            Product product = null;
            List<ReturnableProduct> returnables = null;
            if (SelectedProduct.ProductId !=Guid.Empty)
            {
                product = LoadedProducts.FirstOrDefault(n => n.Id == SelectedProduct.ProductId);
                if (product.GetType() == typeof(SaleProduct))
                {
                    var rp = ((SaleProduct)product).ReturnableProduct;
                    if (rp != null)
                        myReturnable = rp;
                }
            }
            if (myReturnable != null)
            {
                SelectedProduct.HasReturnable = true;
                //add new returnable product to LineItems
                LineItem returnableProduct = BuildReturnableProduct(myReturnable);
                //if (SellInBulk)
                //{
                try
                {
                    myCrate =
                        ((SaleProduct)LoadedProducts.FirstOrDefault(n => n.Id == SelectedProduct.ProductId)).
                            ReturnableProduct.
                            ReturnAbleProduct;
                }
                catch
                {
                    myCrate = null;
                }
                //}

                var firstReturnable = new LineItem
                {
                    ProductId = returnableProduct.ProductId,
                    ProductDesc = returnableProduct.ProductDesc,
                    ProductType = returnableProduct.ProductType,
                    Qty = returnableProduct.Qty,
                    UnitPrice = returnableProduct.UnitPrice,
                    LineItemVatValue = returnableProduct.LineItemVatValue,
                    Vat = returnableProduct.Vat,
                    VatAmount = returnableProduct.VatAmount,
                    LiTotalNet = returnableProduct.LiTotalNet,
                    TotalPrice = returnableProduct.TotalPrice,
                    ReturnableId = myCrate != null ? myCrate.Id : Guid.Empty,
                    Capacity = false ? 0 : myReturnable.Capacity,
                    IsNew = true,
                    SequenceNo = LineItems.Count() + 1
                };
                return firstReturnable;
            }
            SelectedProduct.HasReturnable = false;
            return null;
        }

        //sell in bulk
        private LineItem BuildBulkSaleContainer()
        {
            //var myCrate = _productService.GetAll().OfType<ReturnableProduct>()
            //    .Where(n => n.Packaging.Id == SelectedProduct.PackagingId && n.PackagingType.Id == SelectedProduct.PackagingTypeId && n.Capacity > 1)
            //        .ToList().FirstOrDefault();
            ReturnableProduct myContainer = null;
            if (SelectedProduct.ProductType == "ReturnableProduct")
            {
                myContainer = (ReturnableProduct) LoadedProducts.FirstOrDefault(n => n.Id == SelectedProduct.ProductId);
            }
            else
            {
                try
                {
                    var re =
                        ((SaleProduct) LoadedProducts.FirstOrDefault(n => n.Id == SelectedProduct.ProductId)).
                            ReturnableProduct;
                    myContainer = re.ReturnAbleProduct;
                }
                catch
                {
                    myContainer = null;
                }
            }

            if (myContainer != null)
            {
                SelectedProduct.HasReturnable = true;
                //LineItem crate = BuildReturnableProduct(myContainer, true);
                LineItem crate = BuildBulkSaleContainer(myContainer);
                var mycrate = new LineItem
                {
                    ProductId = crate.ProductId,
                    ProductDesc = crate.ProductDesc,
                    ProductType = crate.ProductType,
                    Qty = crate.Qty,
                    UnitPrice = crate.UnitPrice,
                    LineItemVatValue = crate.LineItemVatValue,
                    Vat = crate.Vat,
                    VatAmount = crate.VatAmount,
                    TotalPrice = crate.TotalPrice,
                    LiTotalNet = crate.LiTotalNet,
                    ReturnableId = Guid.Empty,
                    Capacity = myContainer.Capacity,
                    IsNew = IsNew,
                    SequenceNo = LineItems.Count() + 1
                };
                CrateCapacity = mycrate.Capacity;
                return mycrate;
            }
            SelectedProduct.HasReturnable = false;
            return null;
        }

        private LineItem BuildBulkSaleContainer(ReturnableProduct returnableLi)
        {
            var returnableProduct = new LineItem
            {
                ProductId = returnableLi.Id,
                ProductDesc = returnableLi.Description
            };
            ReturnableId = returnableLi.Id;
            returnableProduct.Qty = RequiredQty;
            returnableProduct.ProductType = returnableLi.GetType().ToString().Split('.').Last();

            //if we are building the original container when editing then we divide by default loaded qty which is for sale product.
            if (!editReturnable)
            {
                if (buildingOrigCrate)
                    returnableProduct.Qty = (int)((Approve / returnableLi.Capacity) <= 0 ? 1 : Approve / returnableLi.Capacity);
                else
                {
                    if (SellInBulk)//sell in bulk option is selected
                        returnableProduct.Qty = Approve;
                    else if (SellInUnits)
                        returnableProduct.Qty = (int)((Approve / returnableLi.Capacity) <= 0 ? 1 : Approve / returnableLi.Capacity);
                }
            }


            returnableProduct.IsNew = IsNew;

            //get pricing details
            returnableProduct = PriceCalc(returnableLi, returnableProduct);
            return returnableProduct;
        }

        LineItem BuildReturnableProduct(ReturnableProduct returnableLi, bool isSecondaryReturnable = false)
        {
            var returnableProduct = new LineItem
            {
                ProductId = returnableLi.Id,
                ProductDesc = returnableLi.Description,
                SequenceNo = LineItems.Count() + 1
            };
            ReturnableId = returnableLi.Id;
            returnableProduct.Qty = RequiredQty;
            returnableProduct.ProductType = returnableLi.GetType().ToString().Split('.').Last();

            returnableProduct.IsNew = IsNew;
            //get pricing details
            returnableProduct = PriceCalc(returnableLi, returnableProduct);
            return returnableProduct;
        }

        LineItem BuildUIProduct()
        {
            LineItem uiProduct = null;

            SaleProduct = BuildSaleProduct();
            uiProduct = new LineItem
            {
                ProductId = SaleProduct.ProductId,
                IsNew = IsNew,
                LineItemVatValue = SaleProduct.LineItemVatValue,
                LiTotalNet = SaleProduct.LiTotalNet,
                ProductDesc = SaleProduct.ProductDesc,
                Qty = SaleProduct.Qty,
                Vat = SaleProduct.Vat,
                UnitPrice = SaleProduct.UnitPrice,
                VatAmount = SaleProduct.VatAmount,
                TotalPrice = SaleProduct.TotalPrice,
            };
            SaleProductReturnable = BuildSaleProductReturnable();
            if (SaleProductReturnable != null)
            {
                uiProduct.LiTotalNet += SaleProductReturnable.LiTotalNet;
                //uiProduct.Qty += SaleProductReturnable.Qty;
                uiProduct.Vat += SaleProductReturnable.Vat;
                uiProduct.UnitPrice += SaleProductReturnable.UnitPrice;
                uiProduct.VatAmount += SaleProductReturnable.VatAmount;
                uiProduct.TotalPrice += SaleProductReturnable.TotalPrice;
            }

            BulkSaleContainer = BuildBulkSaleContainer();
            if (BulkSaleContainer != null)
                if (SellInBulk || SaleProduct.Qty >= BulkSaleContainer.Capacity)
                {
                    uiProduct.LiTotalNet += BulkSaleContainer.LiTotalNet;
                    //uiProduct.Qty += BulkSaleContainer.Qty;
                    uiProduct.Vat += BulkSaleContainer.Vat;
                    uiProduct.UnitPrice += BulkSaleContainer.UnitPrice;
                    uiProduct.VatAmount += BulkSaleContainer.VatAmount;
                    uiProduct.TotalPrice += BulkSaleContainer.TotalPrice;
                }

            return uiProduct;
        }

        void BringToWall(LineItem lineItem)
        {
            LineItemVatValue = lineItem.LineItemVatValue;
            LiTotalNet = lineItem.LiTotalNet;
            Vat = lineItem.Vat;
            VatAmount = lineItem.VatAmount;
            UnitPrice = lineItem.UnitPrice;
            TotalPrice = lineItem.TotalPrice;
        }

        public void AddOrUpdateProducts()
        {
            if (SelectedProduct.ProductId ==Guid.Empty) return;

            if (IsNew)
                AddProducts();
            else
                UpdateProducts();
        }

        void AddProducts()
        {
            if (SelectedProduct.ProductType == "ConsolidatedProduct")
            {
                AddLineItem(SelectedProduct.ProductId,
                            SelectedProduct.ProductDesc,
                            SelectedProduct.ProductType,
                            Qty,
                            UnitPrice,
                            LineItemVatValue,
                            Vat,
                            VatAmount,
                            LiTotalNet,
                            TotalPrice,
                            Guid.Empty,
                            BackOrder,
                            LostSale,
                            IsNew,
                            0);
            }
            else
            {
                var product = LoadedProducts.First(n => n.Id == SelectedProduct.ProductId);
                if (product.GetType() == typeof(SaleProduct))
                    SaleProduct = BuildSaleProduct();
                SaleProductReturnable = BuildSaleProductReturnable();
                BulkSaleContainer = BuildBulkSaleContainer();

                LineItem existingSaleProd = null;
                if (AddedLineItems.Count > 0)
                {
                    try
                    {
                        existingSaleProd = AddedLineItems.First(n => n.ProductId == SaleProduct.ProductId);
                    }
                    catch { }
                }

                if (!ReceiveReturnables)
                {

                    AddSaleProduct();
                    //add bottles
                    AddFirstReturnable();
                    if (SellInBulk)
                    {
                        //add crate
                        AddCrate();
                    }
                    else if (CrateCapacity > 1)
                    {
                        //if qty can fit in a crate, give him in a f*#@* crate
                        decimal reqQty = RequiredQty; 
                        if (existingSaleProd != null)
                        {
                            if (existingSaleProd.Qty < CrateCapacity) //item had no crate
                            {
                                reqQty += existingSaleProd.Qty;
                                Qty = reqQty;
                                BulkSaleContainer = BuildBulkSaleContainer();
                            }
                        }

                        if ((reqQty / CrateCapacity) >= 1)
                        {

                            //add crate
                            AddCrate();
                        }
                    }
                }
                else
                {
                    AddSaleProduct();
                    if (SellInBulk)
                    {
                        //add crate
                        AddCrate();
                    }
                }
            }
        }

        void AddSaleProduct()
        {
            if (SaleProduct == null) return;
            AddLineItem(SelectedProduct.ProductId,
                            SaleProduct.ProductDesc,
                            SaleProduct.ProductType,
                            SaleProduct.Qty,
                            SaleProduct.UnitPrice,
                            SaleProduct.LineItemVatValue,
                            SaleProduct.Vat,
                            SaleProduct.VatAmount,
                            SaleProduct.LiTotalNet,
                            SaleProduct.TotalPrice,
                            SaleProduct.ReturnableId,
                            SaleProduct.BackOrder,
                            SaleProduct.LostSale,
                            SaleProduct.IsNew,
                            SaleProduct.SequenceNo
                    );
        }

        void AddCrate()
        {
            if (BulkSaleContainer == null) return;
            AddLineItem(
                BulkSaleContainer.ProductId,
                BulkSaleContainer.ProductDesc,
                BulkSaleContainer.ProductType,
                BulkSaleContainer.Qty,
                BulkSaleContainer.UnitPrice,
                BulkSaleContainer.LineItemVatValue,
                BulkSaleContainer.Vat,
                BulkSaleContainer.VatAmount,
                BulkSaleContainer.LiTotalNet,
                BulkSaleContainer.TotalPrice,
                BulkSaleContainer.ReturnableId,
                BulkSaleContainer.BackOrder,
                BulkSaleContainer.LostSale,
                IsNew);
            BulkSaleContainer = null;
        }

        void AddFirstReturnable()
        {
            if (SaleProductReturnable != null)
            {
                AddLineItem(
                    SaleProductReturnable.ProductId,
                    SaleProductReturnable.ProductDesc,
                    SaleProductReturnable.ProductType,
                    SaleProductReturnable.Qty,
                    SaleProductReturnable.UnitPrice,
                    SaleProductReturnable.LineItemVatValue,
                    SaleProductReturnable.Vat,
                    SaleProductReturnable.VatAmount,
                    SaleProductReturnable.LiTotalNet,
                    SaleProductReturnable.TotalPrice,
                    SaleProductReturnable.ReturnableId,
                    SaleProductReturnable.BackOrder,
                    SaleProductReturnable.LostSale,
                    IsNew);
                SaleProductReturnable = null;
            }
        }

        void UpdateProducts()
        {
            if (editReturnable || SelectedProduct.ProductType == "ConsolidatedProduct")
            {
                AddLineItem(SelectedProduct.ProductId,
                            SelectedProduct.ProductDesc,
                            SelectedProduct.ProductType,
                            RequiredQty,
                            UnitPrice,
                            LineItemVatValue,
                            Vat,
                            VatAmount,
                            LiTotalNet,
                            TotalPrice,Guid.Empty,
                            BackOrder,
                            LostSale,
                            IsNew,
                            SequenceNo);
            }
            else
            {
                var product = LoadedProducts.First(n => n.Id == SelectedProduct.ProductId);
                if (product.GetType() == typeof(SaleProduct))
                {
                    SaleProduct = BuildSaleProduct();
                    SaleProduct.SequenceNo = SequenceNo;

                    SaleProductReturnable = BuildSaleProductReturnable();

                    AddSaleProduct();

                    if (SaleProductReturnable != null)
                    {
                        var originalFirstReturnable =
                            OriginalLineItems.FirstOrDefault(n => n.ProductId == SaleProductReturnable.ProductId);
                        AddLineItem(
                            SaleProductReturnable.ProductId,
                            SaleProductReturnable.ProductDesc,
                            SaleProductReturnable.ProductType,
                            SaleProductReturnable.Qty - originalFirstReturnable.Qty,
                            SaleProductReturnable.UnitPrice,
                            SaleProductReturnable.LineItemVatValue,
                            SaleProductReturnable.Vat,
                            SaleProductReturnable.VatAmount - originalFirstReturnable.VatAmount,
                            SaleProductReturnable.LiTotalNet - originalFirstReturnable.LiTotalNet,
                            SaleProductReturnable.TotalPrice - originalFirstReturnable.TotalPrice,
                            SaleProductReturnable.ReturnableId,
                            CalcBackOrder(SaleProductReturnable),
                            CalcLostSale(SaleProductReturnable),
                            true);
                    }
                }

                //update crate
                if (BulkSaleContainer == null) return;
                var origCrate = OriginalLineItems.FirstOrDefault(n => n.ProductId == BulkSaleContainer.ProductId);
                if (enforceAddCrate)
                {
                    if (RequiredQty / CrateCapacity >= 1)
                    {
                        AddLineItem(BulkSaleContainer.ProductId,
                                    BulkSaleContainer.ProductDesc,
                                    BulkSaleContainer.ProductType,
                                    BulkSaleContainer.Qty,
                                    BulkSaleContainer.UnitPrice,
                                    BulkSaleContainer.LineItemVatValue,
                                    BulkSaleContainer.Vat,
                                    BulkSaleContainer.VatAmount,
                                    BulkSaleContainer.LiTotalNet,
                                    BulkSaleContainer.TotalPrice,
                                    BulkSaleContainer.ReturnableId,
                                    CalcBackOrder(BulkSaleContainer),
                                    CalcLostSale(BulkSaleContainer),
                                    true);
                        enforceAddCrate = false;
                    }
                }
                else
                {
                    if (RequiredQty / CrateCapacity < 1)
                    {
                        AddLineItem(
                            BulkSaleContainer.ProductId,
                            BulkSaleContainer.ProductDesc,
                            BulkSaleContainer.ProductType,
                            0 - origCrate.Qty,
                            BulkSaleContainer.UnitPrice,
                            BulkSaleContainer.LineItemVatValue,
                            BulkSaleContainer.Vat,
                            BulkSaleContainer.VatAmount - origCrate.VatAmount,
                            BulkSaleContainer.LiTotalNet - origCrate.LiTotalNet,
                            BulkSaleContainer.TotalPrice - origCrate.TotalPrice,
                            BulkSaleContainer.ReturnableId,
                            CalcBackOrder(BulkSaleContainer),
                            CalcLostSale(BulkSaleContainer),
                            true);
                    }
                    else
                    {
                        AddLineItem(
                            BulkSaleContainer.ProductId,
                            BulkSaleContainer.ProductDesc,
                            BulkSaleContainer.ProductType,
                            BulkSaleContainer.Qty - origCrate.Qty,
                            BulkSaleContainer.UnitPrice,
                            BulkSaleContainer.LineItemVatValue,
                            BulkSaleContainer.Vat,
                            BulkSaleContainer.VatAmount - origCrate.VatAmount,
                            BulkSaleContainer.LiTotalNet - origCrate.LiTotalNet,
                            BulkSaleContainer.TotalPrice - origCrate.TotalPrice,
                            BulkSaleContainer.ReturnableId,
                            CalcBackOrder(BulkSaleContainer),
                            CalcLostSale(BulkSaleContainer),
                            true);
                    }
                }
            }
        }

        public void AddLineItem(Guid productId, string productDesc, string productType, decimal qty, decimal unitPrice, decimal lineItemVatValue, decimal vat, decimal vatAmount, decimal liTotalNet, decimal totalPrice, Guid returnableId, decimal backOrder, decimal lostSale, bool isNew, int sequenceNo = 0)
        {
            if (LineItems == null)
                LineItems = new ObservableCollection<LineItem>();
            if (productId != Guid.Empty)
            {
                if (LineItems.Any(n => n.ProductId == productId))
                {
                    var existingLi = LineItems.First(n => n.ProductId == productId);
                    existingLi.Qty = qty;
                    existingLi.VatAmount = vatAmount;
                    existingLi.LiTotalNet = liTotalNet;
                    existingLi.TotalPrice = totalPrice;
                }
                else
                {
                    LineItems.Add(new LineItem
                                      {
                                          ProductId = productId,
                                          ProductDesc = productDesc,
                                          ProductType = productType,
                                          Qty = qty,
                                          BackOrder = backOrder,
                                          LostSale = lostSale,
                                          UnitPrice = unitPrice,
                                          LineItemVatValue = lineItemVatValue,
                                          Vat = vat,
                                          VatAmount = vatAmount,
                                          LiTotalNet = liTotalNet,
                                          TotalPrice = totalPrice,
                                          ReturnableId = returnableId,
                                          IsNew = isNew,
                                          SequenceNo = sequenceNo,
                                      });
                }
            }
        }

        void PriceCalc(Product product)
        {
            using (var container = NestedContainer)
            {
                  IDiscountProWorkflow _discountProService = Using<IDiscountProWorkflow>(container);
               
                UnitPrice = _discountProService.GetUnitPrice(product, SelectedOutlet.OutletProductPricingTier);

                LineItemVatValue = _discountProService.GetVATRate(product, SelectedOutlet);

                RecalcTotal();
                BindTree(product);
            }
        }

        LineItem PriceCalc(Product product, LineItem lineItem)
        {
            using (var container = NestedContainer)
            {
                IDiscountProWorkflow _discountProService = Using<IDiscountProWorkflow>(container);
               
                lineItem.UnitPrice = _discountProService.GetUnitPrice(product, SelectedOutlet.OutletProductPricingTier);

                LineItemVatValue = _discountProService.GetVATRate(product, SelectedOutlet);
                lineItem.LineItemVatValue = LineItemVatValue;

                lineItem = RecalcTotal(lineItem);
                //BindTree(product);
                return lineItem;
            }
        }

        LineItem PriceCalc(ReturnableProduct product, LineItem lineItem)
        {
            using (var container = NestedContainer)
            {
                IDiscountProWorkflow _discountProService = Using<IDiscountProWorkflow>(container);
               
                lineItem.UnitPrice = _discountProService.GetUnitPrice(product, SelectedOutlet.OutletProductPricingTier);

                LineItemVatValue = _discountProService.GetVATRate(product, SelectedOutlet);
                lineItem.LineItemVatValue = LineItemVatValue;

                lineItem = RecalcTotal(lineItem);
                //BindTree(product);
                return lineItem;
            }
        }

        void BindTree(Guid productId)
        {
            using (var container = NestedContainer)
            {
                IProductRepository _productService = Using<IProductRepository>(container);
                
                Product p = _productService.GetById(productId);
                BindTree(p);
            }
        }

        void BindTree(Product product)
        {
            if (product is ConsolidatedProduct)
            {
                ProductTree = ConsolidatedProductHierarchyHelper.GetConsolidatedProductForTreeview((ConsolidatedProduct)product, Qty);
            }
            else
                ProductTree = null;
        }

        public RelayCommand ClearAndSetup { get; set; }
        void RunClearAndSetup()
        {
            ProcessingBackOrder = false;
            Qty = 1;
            UnitPrice = 0;
            UnitVat = 0;
            Vat = 0;
            VatAmount = 0;
            TotalPrice = 0;
            UnitVat = 0;
            LiTotalNet = 0;
            BackOrder = 0;
            LostSale = 0;
            Approve = 0;
            RequiredQty = 0;
            editReturnable = false;
            Setup();
            SelectedActionDetails = "Click on a quantity field to view its pricing details.";
        }

        void Clear()
        {
            ProcessingBackOrder = false;
            Qty = 1;
            UnitPrice = 0;
            UnitVat = 0;
            Vat = 0;
            VatAmount = 0;
            TotalPrice = 0;
            UnitVat = 0;
            LiTotalNet = 0;
            BackOrder = 0;
            LostSale = 0;
            Approve = 0;

            SellInUnits = true;
            SelectedActionDetails = "Click on a quantity field to view its pricing details.";
        }

        public void InstanciateLineItems()
        {
            LineItems = new ObservableCollection<LineItem>();
        } 

        public void RecalcTotal()
        {
            decimal net = UnitPrice * RequiredQty;
            //VatAmount = .17m * net;
            Vat = UnitPrice * LineItemVatValue;//vat for single item
            LiTotalNet = UnitPrice * RequiredQty;
            VatAmount = net * LineItemVatValue;//total vat amount
            TotalPrice = net + VatAmount;
        }

        void RecalcTotal(decimal qty)
        {
            if (SellInBulk)
                qty = RequiredQty;

            decimal net = UnitPrice * qty;
            //VatAmount = .17m * net;
            Vat = UnitPrice * LineItemVatValue;//vat for single item
            LiTotalNet = UnitPrice * qty;
            VatAmount = net * LineItemVatValue;//total vat amount
            TotalPrice = net + VatAmount;
        }

        public LineItem RecalcTotal(LineItem lineItem)
        {
            decimal net = lineItem.UnitPrice * lineItem.Qty;
            //VatAmount = .17m * net;
            lineItem.Vat = lineItem.UnitPrice * lineItem.LineItemVatValue;//vat for single item
            lineItem.LiTotalNet = lineItem.UnitPrice * lineItem.Qty;
            lineItem.VatAmount = net * lineItem.LineItemVatValue;//total vat amount
            lineItem.TotalPrice = net + lineItem.VatAmount;

            return lineItem;
        }
        void CalcLostSale()
        {
            LostSale = OriginalQty - Approve;
            if (LostSale < 0)
                LostSale = 0;
        }

        void CalcBackOrder()
        {
            BackOrder = Approve - AvailableProductInv;
            if (BackOrder < 0)
                BackOrder = 0;
        }

        decimal CalcLostSale(LineItem lineItem)
        {
            decimal lostSale = 0;
            var oli = OriginalLineItems.First(n => n.ProductId == lineItem.ProductId);
            lostSale = oli.Qty - lineItem.Qty;
            return lostSale < 0 ? 0 : lostSale;
        }

        decimal CalcBackOrder(LineItem lineItem)
        {
            decimal backOrder = 0;
            var productItem = Products.First(n => n.ProductId == lineItem.ProductId);
            backOrder = lineItem.Qty - productItem.AvailableInventory;
            return backOrder < 0 ? 0 : backOrder;
        }
        
        public const string VatPropertyName = "Vat";
        private decimal _vat = 0m;
        public decimal Vat
        {
            get
            {
                return _vat;
            }

            set
            {
                if (_vat == value)
                {
                    return;
                }

                var oldValue = _vat;
                _vat = value;
                RaisePropertyChanged(VatPropertyName);
            }
        }

        //Ammend order at/after approval 'ApproveSalesmanOrder'
        public void LoadForEdit(Guid selectedProductId, decimal unitPrice,
            decimal lineItemVatValue, decimal totalPrice, decimal vatAmount, int sequenceNo, decimal qty, decimal availableInv = 0, decimal backOrdered = 0, decimal approved = 0)//, EditSalesmanOrderItem.enumLineItemSaleType saleType = EditSalesmanOrderItem.enumLineItemSaleType.unitSale
        {
            IsNew = false;
            SelectedProduct = Products.First(n => n.ProductId == selectedProductId);
            AvailableProductInv = availableInv;
            BackOrder = backOrdered;
            UnitPrice = unitPrice;
            LineItemVatValue = lineItemVatValue;
            TotalPrice = totalPrice;
            VatAmount = vatAmount;
            SequenceNo = sequenceNo;
            Qty = qty;
            RequiredQty = Qty;
            Approve = approved > 0 ? approved : Qty;
            RecalcTotal();

            BuildOriginalLineItems();

            DetermineSaleMode();//saleType
        }

        public List<LineItem> OriginalLineItems = null;
        bool buildingOrigCrate = false;
        bool enforceAddCrate = false;
        public bool loadingItemsToDelete = false;
        private bool editReturnable = false;

        void BuildOriginalLineItems()
        {
            OriginalLineItems = new List<LineItem>();
            if (SelectedProduct.ProductType == "ConsolidatedProduct") return;
            if (SelectedProduct.ProductType == "ReturnableProduct")
            {
                editReturnable = true;
                //var rp = BuildBulkSaleContainer();
                //OriginalLineItems.Add(rp);
                return;
            }

            var saleProduct = BuildSaleProduct();
            OriginalLineItems.Add(saleProduct);

            var firstReturnable = BuildSaleProductReturnable();
            if (firstReturnable != null)
                OriginalLineItems.Add(firstReturnable);

            buildingOrigCrate = true;
            var mycrate = BuildBulkSaleContainer();
            buildingOrigCrate = false;

            //this is to check whether a crate should be added in case where original line items did not have
            if ((RequiredQty / CrateCapacity) > 0)
                enforceAddCrate = false;
            else enforceAddCrate = true;
            if (!loadingItemsToDelete)
            {
                if (mycrate != null)
                    OriginalLineItems.Add(mycrate);
            }
            else
            {
                if (!enforceAddCrate)
                    OriginalLineItems.Add(mycrate);
            }
        }

        void DetermineSaleMode()//EditSalesmanOrderItem.enumLineItemSaleType saleType
        {
            //switch (saleType)
            //{
            //    case EditSalesmanOrderItem.enumLineItemSaleType.undefined:
            //        if (RequiredQty / CrateCapacity >= 1)
            //        {
            //            if (RequiredQty % CrateCapacity == 0)
            //            {
            //                SellInBulk = true;
            //                SellInUnits = false;
            //            }
            //            else
            //            {
            //                SellInUnits = true;
            //                SellInBulk = false;
            //            }
            //        }
            //        else
            //        {
            //            SellInUnits = true;
            //            SellInBulk = false;
            //        }
            //        break;
            //    case EditSalesmanOrderItem.enumLineItemSaleType.unitSale:
                    SellInUnits = true;
                    SellInBulk = false;
            //        break;
            //    case EditSalesmanOrderItem.enumLineItemSaleType.bulkSale:
            //        SellInBulk = true;
            //        SellInUnits = false;
            //        RunSellInBulkSelected();
            //        break;
            //}
        }

        #endregion

        #region Properties

        public const string SelectedProductPropertyName = "SelectedProduct";
        private OrderLineItemProductLookupItem _selectedProduct = null;
        public OrderLineItemProductLookupItem SelectedProduct
        {
            get
            {
                return _selectedProduct;
            }

            set
            {
                if (_selectedProduct == value)
                {
                    return;
                }
                var oldValue = _selectedProduct;
                _selectedProduct = value;
                RaisePropertyChanged(SelectedProductPropertyName);
            }
        }

        public ObservableCollection<OrderLineItemProductLookupItem> Products { get; set; }
        public const string QtyPropertyName = "Qty";
        private decimal _qty = 1;
        [Range(1, 99999999)]
        public decimal Qty
        {
            get
            {
                return _qty;
            }

            set
            {
                if (_qty == value)
                {
                    return;
                }
                var oldValue = _qty;
                _qty = value;

                RaisePropertyChanged(QtyPropertyName);
                //RecalcTotal();
                //BindTree(SelectedProduct.ProductId);
            }
        }

        public const string UnitPricePropertyName = "UnitPrice";
        private decimal _unitPrice = 0;
        public decimal UnitPrice
        {
            get
            {
                return _unitPrice;
            }

            set
            {
                if (_unitPrice == value)
                    return;
                var oldValue = _unitPrice;
                _unitPrice = value;
                RaisePropertyChanged(UnitPricePropertyName);
            }
        }

        public const string VatAmountPropertyName = "VatAmount";
        private decimal _vatAmount = 0;
        public decimal VatAmount
        {
            get
            {
                return _vatAmount;
            }

            set
            {
                if (_vatAmount == value)
                    return;

                var oldValue = _vatAmount;
                _vatAmount = value;
                RaisePropertyChanged(VatAmountPropertyName);
            }
        }

        public const string TotalPricePropertyName = "TotalPrice";
        private decimal _totalPrice = 0;
        public decimal TotalPrice
        {
            get
            {
                return _totalPrice;
            }

            set
            {
                if (_totalPrice == value)
                {
                    return;
                }
                var oldValue = _totalPrice;
                _totalPrice = value;
                RaisePropertyChanged(TotalPricePropertyName);
            }
        }

        public const string IsNewPropertyName = "IsNew";
        private bool _myProperty = true;
        public bool IsNew
        {
            get
            {
                return _myProperty;
            }

            set
            {
                if (_myProperty == value)
                    return;
                var oldValue = _myProperty;
                _myProperty = value;
                RaisePropertyChanged(IsNewPropertyName);
            }
        }

        public const string IsUpdatedPropertyName = "IsUpdated";
        private bool _isUpdated = false;
        public bool IsUpdated
        {
            get
            {
                return _isUpdated;
            }

            set
            {
                if (_isUpdated == value)
                {
                    return;
                }

                var oldValue = _isUpdated;
                _isUpdated = value;
                RaisePropertyChanged(IsUpdatedPropertyName);
            }
        }

        public const string IsDeletedPropertyName = "IsDeleted";
        private bool _isDeleted = false;
        public bool IsDeleted
        {
            get
            {
                return _isDeleted;
            }

            set
            {
                if (_isDeleted == value)
                {
                    return;
                }

                var oldValue = _isDeleted;
                _isDeleted = value;
                RaisePropertyChanged(IsDeletedPropertyName);
            }
        }

        public const string SequenceNoPropertyName = "SequenceNo";
        private int _sequenceNo = 0;
        public int SequenceNo
        {
            get
            {
                return _sequenceNo;
            }

            set
            {
                if (_sequenceNo == value)
                    return;
                var oldValue = _sequenceNo;
                _sequenceNo = value;
                RaisePropertyChanged(SequenceNoPropertyName);
            }
        }

        public const string LineItemVatValuePropertyName = "LineItemVatValue";
        private decimal _lineItemVatValue = 0;
        public decimal LineItemVatValue
        {
            get
            {
                return _lineItemVatValue;
            }

            set
            {
                if (_lineItemVatValue == value)
                    return;
                var oldValue = _lineItemVatValue;
                _lineItemVatValue = value;
                RaisePropertyChanged(LineItemVatValuePropertyName);
            }
        }

        public const string AvailableProductInvPropertyName = "AvailableProductInv";
        private decimal _availableProductInv = 0;
        public decimal AvailableProductInv
        {
            get
            {
                return _availableProductInv;
            }

            set
            {
                if (_availableProductInv == value)
                {
                    return;
                }

                var oldValue = _availableProductInv;
                _availableProductInv = value;
                RaisePropertyChanged(AvailableProductInvPropertyName);
            }
        }

        public const string ProductTreePropertyName = "ProductTree";
        private IEnumerable<HierarchyNode<ProductViewer>> _productTree = null;
        public IEnumerable<HierarchyNode<ProductViewer>> ProductTree
        {
            get
            {
                return _productTree;
            }

            set
            {
                if (_productTree == value)
                    return;
                var oldValue = _productTree;
                _productTree = value;
                RaisePropertyChanged(ProductTreePropertyName);
            }
        }

        #region enum
        public enum ProducTypeToLoad
        {
            AllProducts = 1,
            Returnables = 2,
            NonReturnables = 3
        }
        #endregion

        public const string ProductTypeToLoadPropertyName = "ProductTypeToLoad";
        private ProducTypeToLoad _productTypeToLoad = ProducTypeToLoad.AllProducts;
        public ProducTypeToLoad ProductTypeToLoad
        {
            get
            {
                return _productTypeToLoad;
            }

            set
            {
                if (_productTypeToLoad == value)
                {
                    return;
                }

                var oldValue = _productTypeToLoad;
                _productTypeToLoad = value;
                RaisePropertyChanged(ProductTypeToLoadPropertyName);
            }
        }

        public const string SalesmanPropertyName = "Salesman";
        private User _salesman = null;
        public User Salesman
        {
            get
            {
                return _salesman;
            }

            set
            {
                if (_salesman == value)
                {
                    return;
                }

                var oldValue = _salesman;
                _salesman = value;
                RaisePropertyChanged(SalesmanPropertyName);
            }
        }

        public const string SelectedOutletIdPropertyName = "SelectedOutletId";
        private Guid _selectedOutletId = Guid.Empty;
        public Guid SelectedOutletId
        {
            get
            {
                return _selectedOutletId;
            }

            set
            {
                if (_selectedOutletId == value)
                {
                    return;
                }

                var oldValue = _selectedOutletId;
                _selectedOutletId = value;
                RaisePropertyChanged(SelectedOutletIdPropertyName);
            }
        }


        public const string SelectedOutletPropertyName = "SelectedOutlet";
        private Outlet _selectedOutlet = null;
        public Outlet SelectedOutlet
        {
            get
            {
                if (_selectedOutlet != null)
                    if (_selectedOutlet.Id == SelectedOutletId)
                        return _selectedOutlet;
                using (var container = NestedContainer)
                {
                   var _costCentreService = Using<ICostCentreRepository>(container);
                    
                    return _costCentreService.GetById(SelectedOutletId) as Outlet;
                }
            }
        }

        public const string ModalCrumbsPropertyName = "ModalCrumbs";
        private string _modalCrumbs = "";
        public string ModalCrumbs
        {
            get
            {
                return _modalCrumbs;
            }

            set
            {
                if (_modalCrumbs == value)
                {
                    return;
                }

                var oldValue = _modalCrumbs;
                _modalCrumbs = value;
                RaisePropertyChanged(ModalCrumbsPropertyName);
            }
        }

        public const string UnitVatPropertyName = "UnitVat";
        private decimal _unitVat = 0m;
        public decimal UnitVat
        {
            get
            {
                return _unitVat;
            }

            set
            {
                if (_unitVat == value)
                {
                    return;
                }

                var oldValue = _unitVat;
                _unitVat = value;
                RaisePropertyChanged(UnitVatPropertyName);
            }
        }

        public const string LiTotalNetPropertyName = "LiTotalNet";
        private decimal _liTotalNet = 0m;
        public decimal LiTotalNet
        {
            get
            {
                return _liTotalNet;
            }

            set
            {
                if (_liTotalNet == value)
                {
                    return;
                }

                var oldValue = _liTotalNet;
                _liTotalNet = value;
                RaisePropertyChanged(LiTotalNetPropertyName);
            }
        }

        public const string IsEditingPropertyName = "IsEditing";
        private bool _IsEditing = false;
        public bool IsEditing
        {
            get
            {
                return _IsEditing;
            }

            set
            {
                if (_IsEditing == value)
                {
                    return;
                }

                var oldValue = _IsEditing;
                _IsEditing = value;
                RaisePropertyChanged(IsEditingPropertyName);
            }
        }

        public const string SelectedActionDetailsPropertyName = "SelectedActionDetails";
        private string _selectedOptionDetalails = "";
        public string SelectedActionDetails
        {
            get
            {
                return _selectedOptionDetalails;
            }

            set
            {
                if (_selectedOptionDetalails == value)
                {
                    return;
                }

                var oldValue = _selectedOptionDetalails;
                _selectedOptionDetalails = value;
                RaisePropertyChanged(SelectedActionDetailsPropertyName);
            }
        }

        //public const string AwaitingStockPropertyName = "AwaitingStock";
        //private int _awaitingStock = 0;
        //public int AwaitingStock
        //{
        //    get
        //    {
        //        return _awaitingStock;
        //    }

        //    set
        //    {
        //        if (_awaitingStock == value)
        //        {
        //            return;
        //        }

        //        var oldValue = _awaitingStock;
        //        _awaitingStock = value;
        //        RaisePropertyChanged(AwaitingStockPropertyName);
        //        //RecalcQuantities();
        //        //RecalcTotal(_awaitingStock);
        //        BindTree(SelectedProduct.ProductId);
        //    }
        //}

        public const string BackOrderPropertyName = "BackOrder";
        private decimal _backOrder = 0;
        public decimal BackOrder
        {
            get
            {
                return _backOrder;
            }

            set
            {
                if (_backOrder == value)
                {
                    return;
                }

                var oldValue = _backOrder;
                _backOrder = value;
                RaisePropertyChanged(BackOrderPropertyName);
                CalcLostSale();
                RecalcTotal(_backOrder);
                BindTree(SelectedProduct.ProductId);
            }
        }

        public const string LostSalePropertyName = "LostSale";
        private decimal _lostSale = 0;
        public decimal LostSale
        {
            get
            {
                return _lostSale;
            }

            set
            {
                if (_lostSale == value)
                {
                    return;
                }

                var oldValue = _lostSale;
                _lostSale = value;
                RaisePropertyChanged(LostSalePropertyName);
                RecalcTotal(_lostSale);
                BindTree(SelectedProduct.ProductId);
            }
        }

        public const string OriginalQtyPropertyName = "OriginalQty";
        private decimal _originalQty = 0;
        public decimal OriginalQty
        {
            get
            {
                return _originalQty;
            }

            set
            {
                if (_originalQty == value)
                {
                    return;
                }

                var oldValue = _originalQty;
                _originalQty = value;
                RaisePropertyChanged(OriginalQtyPropertyName);
            }
        }

        public const string ApprovePropertyName = "Approve";
        private decimal _approve = 0;
        public decimal Approve
        {
            get
            {
                return _approve;
            }

            set
            {
                if (_approve == value)
                {
                    return;
                }

                var oldValue = _approve;
                _approve = value;
                RaisePropertyChanged(ApprovePropertyName);
                CalcLostSale();
                CalcBackOrder();
                BindTree(SelectedProduct.ProductId);
            }
        }

        public const string POSPropertyName = "POS";
        private bool _pos = false;
        public bool POS
        {
            get
            {
                return _pos;
            }

            set
            {
                if (_pos == value)
                {
                    return;
                }

                var oldValue = _pos;
                _pos = value;
                RaisePropertyChanged(POSPropertyName);
            }
        }

        public const string ProcessingBackOrderPropertyName = "ProcessingBackOrder";
        private bool _processingBackOrder = false;
        public bool ProcessingBackOrder
        {
            get
            {
                return _processingBackOrder;
            }

            set
            {
                if (_processingBackOrder == value)
                {
                    return;
                }

                var oldValue = _processingBackOrder;
                _processingBackOrder = value;
                RaisePropertyChanged(ProcessingBackOrderPropertyName);
            }
        }

        public const string CanSellInBulkPropertyName = "CanSellInBulk";
        private bool _canSellInBulk = false;
        public bool CanSellInBulk
        {
            get
            {
                return _canSellInBulk;
            }

            set
            {
                if (_canSellInBulk == value)
                {
                    return;
                }

                var oldValue = _canSellInBulk;
                _canSellInBulk = value;
                RaisePropertyChanged(CanSellInBulkPropertyName);
            }
        }

        //public const string SellInUnitsPropertyName = "SellInUnits";
        //private bool _sellInUnits = false;
        //public bool SellInUnits
        //{
        //    get
        //    {
        //        return _sellInUnits;
        //    }

        //    set
        //    {
        //        if (_sellInUnits == value)
        //        {
        //            return;
        //        }

        //        var oldValue = _sellInUnits;
        //        _sellInUnits = value;
        //        RaisePropertyChanged(SellInUnitsPropertyName);
        //    }
        //}

        //public const string SellInBulkPropertyName = "SellInBulk";
        //private bool _sellInBulk = false;
        //public bool SellInBulk
        //{
        //    get
        //    {
        //        return _sellInBulk;
        //    }

        //    set
        //    {
        //        if (_sellInBulk == value)
        //        {
        //            return;
        //        }

        //        var oldValue = _sellInBulk;
        //        _sellInBulk = value;
        //        RaisePropertyChanged(SellInBulkPropertyName);
        //    }
        //}

        public const string SellInUnitsPropertyName = "SellInUnits";
        private bool _sellInUnits = true;
        public bool SellInUnits
        {
            get
            {
                return _sellInUnits;
            }

            set
            {
                if (_sellInUnits == value)
                {
                    return;
                }

                var oldValue = _sellInUnits;
                _sellInUnits = value;
                _sellInBulk = !_sellInUnits;
                RaisePropertyChanged(SellInUnitsPropertyName);
            }
        }

        public const string SellInBulkPropertyName = "SellInBulk";
        private bool _sellInBulk = false;
        public bool SellInBulk
        {
            get
            {
                return _sellInBulk;
            }

            set
            {
                if (_sellInBulk == value)
                {
                    return;
                }

                var oldValue = _sellInBulk;
                _sellInBulk = value;
                RaisePropertyChanged(SellInBulkPropertyName);
            }
        }

        public const string CapacityValueAppliedPropertyName = "CapacityValueApplied";
        private bool _capacityValueApplied = false;
        public bool CapacityValueApplied
        {
            get
            {
                return _capacityValueApplied;
            }

            set
            {
                if (_capacityValueApplied == value)
                {
                    return;
                }

                var oldValue = _capacityValueApplied;
                _capacityValueApplied = value;
                RaisePropertyChanged(CapacityValueAppliedPropertyName);
            }
        }

        public const string CrateCapacityPropertyName = "CrateCapacity";
        private int _crateCapacity = 1;
        public int CrateCapacity
        {
            get
            {
                return _crateCapacity;
            }

            set
            {
                if (_crateCapacity == value)
                {
                    return;
                }

                var oldValue = _crateCapacity;
                _crateCapacity = value;
                RaisePropertyChanged(CrateCapacityPropertyName);
            }
        }

        public const string RequiredQtyPropertyName = "RequiredQty";
        private decimal _requiredQty = 1;
        public decimal RequiredQty
        {
            get
            {
                return _requiredQty;
            }

            set
            {
                if (_requiredQty == value)
                {
                    return;
                }

                var oldValue = _requiredQty;
                _requiredQty = value;
                RaisePropertyChanged(RequiredQtyPropertyName);
            }
        }

        public const string ReturnableIdPropertyName = "ReturnableId";
        private Guid _returnableId = Guid.Empty;
        public Guid ReturnableId
        {
            get
            {
                return _returnableId;
            }

            set
            {
                if (_returnableId == value)
                {
                    return;
                }

                var oldValue = _returnableId;
                _returnableId = value;
                RaisePropertyChanged(ReturnableIdPropertyName);
            }
        }

        public const string ReceiveReturnablesPropertyName = "ReceiveReturnables";
        private bool _receiveReturnables = false;
        public bool ReceiveReturnables
        {
            get
            {
                return _receiveReturnables;
            }

            set
            {
                if (_receiveReturnables == value)
                {
                    return;
                }

                var oldValue = _receiveReturnables;
                _receiveReturnables = value;
                RaisePropertyChanged(ReceiveReturnablesPropertyName);
            }
        }
        #endregion


        //#region Helper Classes
        //public class OrderLineItemProductLookupItem
        //{
        //    public int ProductId { get; set; }
        //    public string ProductDesc { get; set; }
        //    public int PackagingId { get; set; }
        //    public int PackagingTypeId { get; set; }
        //    public string ProductType { get; set; }
        //    public int LineItemQty { get; set; }
        //    public bool HasReturnable { get; set; }
        //    public int MyReturnableProducrId { get; set; }
        //}

        //public class LineItem
        //{
        //    public int ProductId { get; set; }
        //    public string ProductDesc { get; set; }
        //    public decimal UnitPrice { get; set; }
        //    public decimal LineItemVatValue { get; set; }
        //    public decimal LiTotalNet { get; set; }
        //    public decimal VatAmount { get; set; }
        //    public decimal Vat { get; set; }
        //    public int Qty { get; set; }
        //    public decimal TotalPrice { get; set; }
        //    public string ProductType { get; set; }
        //    public bool IsNew { get; set; }
        //    public int ReturnableId { get; set; }
        //    public int SequenceNo { get; set; }
        //    public int Capacity { get; set; }

        //    public int Approve { get; set; }
        //    public int BackOrder { get; set; }
        //    public int LostSale { get; set; }
        //}
        //#endregion
    }
}