using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Factory.Documents;
using Distributr.Core.Factory.Master;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Service.Utility;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.UI.Hierarchy;
using GalaSoft.MvvmLight;
using System.Linq;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Test
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm/getstarted
    /// </para>
    /// </summary>
    public class TestGridViewModel : ViewModelBase
    {
        private IProductRepository _productService;
        private IProductPricingRepository _productPricingService;
        private IProductPricingTierRepository _productPricingTierService;
        private ICostCentreRepository _costCentreService;
        private IConfigService _configService;
        private IUserRepository _userService;
        private IConfirmReturnsNoteWFManager _confirmReturnsNoteWFManager;
        private IDocumentFactory _documentFactory;
        private IConfirmDispatchNoteWFManager _confirmDispatchNoteWFManager;
        private IConfirmCreditNoteWFManager _confirmCreditNoteWFManager;
        private IConfirmPaymentNoteWFManager _confirmPaymentNoteWFManager;

        public TestGridViewModel()
        {
            throw new NotImplementedException();
        }

        public TestGridViewModel(IConfirmPaymentNoteWFManager confirmPaymentNoteWFManager,
            IConfirmCreditNoteWFManager confirmCreditNoteWFManager,
            IConfirmDispatchNoteWFManager confirmDispatchNoteWFManager,IDocumentFactory documentFactory,
            IConfirmReturnsNoteWFManager confirmReturnsNoteWFManager, IUserRepository userService, ICostCentreRepository costCentreService, 
            IConfigService configService, IProductRepository productService, IProductPricingRepository productPricingService,
            IProductPricingTierRepository productPricingTierService)
        {
            _productService = productService;
            SetUp();
            ProcessCommand = new RelayCommand(Process);
            ReturnNoteCommand = new RelayCommand(TestRetire);
            _productPricingService = productPricingService;
            _productPricingTierService = productPricingTierService;
            _costCentreService = costCentreService;
            _configService = configService;
            _userService = userService;
            _confirmReturnsNoteWFManager = confirmReturnsNoteWFManager;
            _documentFactory = documentFactory;
            _confirmDispatchNoteWFManager = confirmDispatchNoteWFManager;
            _confirmCreditNoteWFManager = confirmCreditNoteWFManager;
            DiscountTestCommand = new RelayCommand(GeneratePaymentNote);
            _confirmPaymentNoteWFManager = confirmPaymentNoteWFManager;
        }

        private void TestDiscount()
        {
            Outlet outlet = _costCentreService.GetAll().Where(p => p.CostCentreCode=="O002").FirstOrDefault() as Outlet;
            SaleProduct sp = _productService.GetAll().Where(p => p.ProductCode == "SP001").FirstOrDefault() as SaleProduct ;
            SaleProduct sp2 = _productService.GetAll().Where(p => p.ProductCode == "SP004").FirstOrDefault() as SaleProduct;
            List<OrderDiscountLineItem> ls = new List<OrderDiscountLineItem>();
            OrderDiscountLineItem p1 = new OrderDiscountLineItem
                                           {
                                               ProductId=sp.Id,
                                               Quantity=10,
                                               UnitPrice=29,
                                               TotalPrice=8000
                                           };
            ls.Add(p1);
            OrderDiscountLineItem p2 = new OrderDiscountLineItem
            {
                ProductId = sp2.Id,
                Quantity = 10,
                UnitPrice = 29,
                TotalPrice = 10000
            };
            ls.Add(p2);
            DateTime start = DateTime.Now;
            //List<DiscountBase> list = _discountProService.GetDiscountSummary(ls,outlet.Id);
            TimeSpan diff = DateTime.Now.Subtract(start);
        }
        private void TestRetire()
        {

            Distributor dist = _costCentreService.GetAll().OfType<Distributor>().FirstOrDefault();
            DistributorSalesman salesman = _costCentreService.GetAll().OfType<DistributorSalesman>().FirstOrDefault();
            Config con = _configService.Load();
            User user = _userService.GetByCostCentre(salesman.Id).First();
            RetireDocumentCommand cmd = new RetireDocumentCommand(Guid.NewGuid(), Guid.Parse("6AA572CB-EB09-4FC9-A760-89037C8DC7F6"), user.Id, dist.Id, 0,
                                                                  con.CostCentreApplicationId, Guid.NewGuid(),
                                                                  salesman.Id,null,null);
            //_retireDocumentWFManager.Submit(cmd);
            MessageBox.Show("RetireDocumentCommand note Done");

        }

        public RelayCommand ProcessCommand { get; set; }
        public RelayCommand ReturnNoteCommand { get; set; }
        public RelayCommand DiscountTestCommand { get; set; }

        void GenerateReturnNote()
        {
            Distributor dist = _costCentreService.GetAll().OfType<Distributor>().FirstOrDefault();
            DistributorSalesman salesman = _costCentreService.GetAll().OfType<DistributorSalesman>().FirstOrDefault();
            Config con = _configService.Load();
            User user = _userService.GetByCostCentre(salesman.Id).First();
            Product product = _productService.GetAll().FirstOrDefault();
            ReturnsNote doc = new ReturnsNote(Guid.NewGuid())
                                  {
                                      DocumentDateIssued = DateTime.Now,
                                      DocumentIssuerCostCentre = salesman,
                                      DocumentIssuerCostCentreApplicationId = con.CostCentreApplicationId,
                                      DocumentIssuerUser = user,
                                      DocumentRecipientCostCentre=dist,
                                      DocumentReference=Guid.NewGuid().ToString(),
                                      DocumentType=DocumentType.ReturnsNote,
                                      EndDate = DateTime.Now,
                                      StartDate = DateTime.Now,
                                      Status=DocumentStatus.New,
                                      
                                  };
            doc.Add(new ReturnsNoteLineItem(Guid.NewGuid())
                        {
                            Actual = 4,
                            Description = "tests",
                            IsNew=true,
                            LineItemSequenceNo=1,
                            Product = product,
                            Qty=4,
                            ReturnType=ReturnsType.Inventory,
                            Value=0,
                           
                        });
            //doc.Add(new ReturnsNoteLineItem(Guid.NewGuid())
            //{
            //    Actual = 20,
            //    Description = "tests",
            //    IsNew = true,
            //    LineItemSequenceNo = 1,
            //    Qty = 90,
            //    ReturnType = ReturnsType.Cash,
            //    Value = 2000,

            //});

            _confirmReturnsNoteWFManager.SubmitChanges(doc, _configService.Load());
           
           // _confirmReturnsNoteWFManager.Close(doc.Id);
            MessageBox.Show("Close Returns note Done");

        }
        void GeneratePaymentNote()
        {
            Distributor dist = _costCentreService.GetAll().OfType<Distributor>().FirstOrDefault();
            DistributorSalesman salesman = _costCentreService.GetAll().OfType<DistributorSalesman>().FirstOrDefault();
            Config con = _configService.Load();
            User user = _userService.GetByCostCentre(salesman.Id).First();
            
            PaymentNote doc = new PaymentNote(Guid.NewGuid())
            {
                DocumentDateIssued = DateTime.Now,
                DocumentIssuerCostCentre = salesman,
                DocumentIssuerCostCentreApplicationId = con.CostCentreApplicationId,
                DocumentIssuerUser = user,
                DocumentRecipientCostCentre = dist,
                DocumentReference = Guid.NewGuid().ToString(),
                DocumentType = DocumentType.PaymentNote,
                EndDate = DateTime.Now,
                StartDate = DateTime.Now,
                Status = DocumentStatus.New,
                PaymentNoteType = PaymentNoteType.Availabe,

            };
            doc.LineItems.Add(new PaymentNoteLineItem(Guid.NewGuid())
            {
                Amount = 150,
                Description = "Payment note Test",
                IsNew = true,
                LineItemSequenceNo = 2,
                PaymentMode=PaymentMode.Cash,
            });
            doc.LineItems.Add(new PaymentNoteLineItem(Guid.NewGuid())
            {
                Amount = 100,
                Description = "Payment note Test",
                IsNew = true,
                LineItemSequenceNo = 1,
                PaymentMode = PaymentMode.MMoney,

            });
            _confirmPaymentNoteWFManager.SubmitChanges(doc, _configService.Load());

           
            MessageBox.Show("Close Payment note Done");

        }
        void GeneratedispatchNote()
        {
            Distributor dist = _costCentreService.GetAll().OfType<Distributor>().FirstOrDefault();
            Config con = _configService.Load();
            User user = _userService.GetById(_configService.ViewModelParameters.CurrentUserId);
            Product product = _productService.GetAll().FirstOrDefault();
            DispatchNote dn =
                _documentFactory.CreateDocument(Guid.NewGuid(), DocumentType.DispatchNote, dist, dist, user,
                                                Guid.NewGuid().ToString()) as DispatchNote;
            dn.DispatchType = DispatchNoteType.Delivery;
            dn.DocumentIssuerCostCentreApplicationId = con.CostCentreApplicationId;
            dn.EndDate = DateTime.Now;
            dn.StartDate = DateTime.Now;
            dn.LineItems.Add(new DispatchNoteLineItem(Guid.NewGuid())
                                 {
                                     Description="test dispatch",
                                     LineItemSequenceNo=1,
                                     Product=product,
                                     Qty=10,
                                     Value=20,
                                     IsNew = true,

                                 });
            dn.LineItems.Add(new DispatchNoteLineItem(Guid.NewGuid())
            {
                Description = "test dispatch",
                LineItemSequenceNo = 1,
                Product = product,
                Qty = 10,
                Value = 20,
                IsNew = true,

            });
            _confirmDispatchNoteWFManager.SubmitChanges(dn, _configService.Load());
            MessageBox.Show("Confirm Dispatch Done");
        }
        void GenerateCreditNote()
        {
            Distributor dist = _costCentreService.GetAll().OfType<Distributor>().FirstOrDefault();
            Config con = _configService.Load();
            User user = _userService.GetById(_configService.ViewModelParameters.CurrentUserId);
            Product product = _productService.GetAll().FirstOrDefault();
            CreditNote creditNote =
                _documentFactory.CreateDocument(Guid.NewGuid(), DocumentType.CreditNote, dist, dist, user,
                                                Guid.NewGuid().ToString()) as CreditNote;
           
            creditNote.DocumentIssuerCostCentreApplicationId = con.CostCentreApplicationId;
            creditNote.EndDate = DateTime.Now;
            creditNote.StartDate = DateTime.Now;
            creditNote.LineItems.Add(new CreditNoteLineItem(Guid.NewGuid())
            {
                Description = "test CreditNote",
                LineItemSequenceNo = 1,
                //Product = product,
                //Qty = 10,
                Value = 20,
                IsNew = true,

            });
            creditNote.LineItems.Add(new CreditNoteLineItem(Guid.NewGuid())
            {
                Description = "test CreditNote",
                LineItemSequenceNo = 1,
                //Product = product,
                //Qty = 10,
                Value = 20,
                IsNew = true,

            });
            _confirmCreditNoteWFManager.SubmitChanges(creditNote,_configService.Load());
            MessageBox.Show("Confirm CreditNote Done");
        }

        void Process()
        {
            if(ProductLookup!=null)
            {
                LineItem.Clear();
                LineItem.Add(new EditOrderViewModelLineItem
                                 {
                                     Product = ProductLookup.Name,
                                     ProductId = ProductLookup.ProductId,
                                     Qty = Quantity,
                                     SequenceNo = 0,
                                     TotalPrice = Quantity*Price,
                                     UnitPrice = Price,
                                     VatAmount = (Quantity*Price*(decimal.Parse("0.16")))

                                 });
            }
            ProcessDisplay();
        }

        private void SetUp()
        {
            DisplayLineItem = new ObservableCollection<ProductDataGridDisplay>();
            LineItem = new ObservableCollection<EditOrderViewModelLineItem>();
            ProductLookupList = new ObservableCollection<ProductLookup>();
            LoadData();
           
        }
        private void LoadData()
        {
            _productService.GetAll().ToList().ForEach(n => ProductLookupList.Add(new ProductLookup
                                                                   {
                                                                       Name=n.Description,
                                                                       ProductId=n.Id
                                                                   }));
        }

        public ObservableCollection<ProductDataGridDisplay> DisplayLineItem { get; set; }
        public ObservableCollection<EditOrderViewModelLineItem> LineItem { get; set; }
        public ObservableCollection<ProductLookup> ProductLookupList { get; set; }

        
        public const string ProductLookupPropertyName = "ProductLookup";
        private ProductLookup _ProductLookup = null;
        public ProductLookup ProductLookup
        {
            get
            {
                return _ProductLookup;
            }

            set
            {
                if (_ProductLookup == value)
                {
                    return;
                }

                var oldValue = _ProductLookup;
                _ProductLookup = value;

              
                // Update bindings, no broadcast
                RaisePropertyChanged(ProductLookupPropertyName);

             
            }
        }

      
        public const string PricePropertyName = "Price";
        private int _Price = 0;
        public int Price
        {
            get
            {
                return _Price;
            }

            set
            {
                if (_Price == value)
                {
                    return;
                }

                var oldValue = _Price;
                _Price = value;


                // Update bindings, no broadcast
                RaisePropertyChanged(PricePropertyName);

               
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

        private void ProcessDisplay()
        {
            DisplayLineItem.Clear();
            ProductPricingTier tier = _productPricingTierService.GetAll().First();
            foreach(EditOrderViewModelLineItem item in LineItem)
            {
                Product p = _productService.GetById(item.ProductId);
                if(p is ConsolidatedProduct)
                {
                    foreach(ProductViewer pv in ConsolidatedProductHierarchyHelper.GetConsolidatedProductFLat(p as ConsolidatedProduct,item.Qty))
                    {
                        Product product = _productService.GetById(pv.ProductId);
                       
                        decimal UnitPrice = 0;
                        if (product is ConsolidatedProduct)
                            try
                            {
                                UnitPrice = ((ConsolidatedProduct)product).ProductPrice(tier);
                            }
                            catch
                            {
                                UnitPrice = 0m;
                            }
                        else
                            try
                            {
                                UnitPrice = product.ProductPricings.First(n => n.Tier.Id == tier.Id).CurrentSellingPrice;
                            }
                            catch
                            {
                                UnitPrice = 0m;
                            }
                   
                        ProductDataGridDisplay disItem = new ProductDataGridDisplay
                                                             {
                                                                 Product = pv.Description,
                                                                 ProductId = pv.ProductId,
                                                                 Qty = pv.QtyPerConsolidatedProduct*item.Qty,
                                                                 TotalPrice = UnitPrice * pv.QtyPerConsolidatedProduct * item.Qty,
                                                                 UnitPrice = UnitPrice,
                                                                 VatAmount = 0,

                                                             };
                        DisplayLineItem.Add(disItem);
                    }
                }else
                { decimal UnitPrice = 0;
                    Product product = _productService.GetById(item.ProductId);
                     try
                            {
                                UnitPrice = product.ProductPricings.First(n => n.Tier.Id == tier.Id).CurrentSellingPrice;
                            }
                            catch
                            {
                                UnitPrice = 0m;
                            }
                    ProductDataGridDisplay disItem = new ProductDataGridDisplay
                    {
                        Product = p.Description,
                        ProductId = p.Id,
                        Qty = item.Qty,
                        TotalPrice = item.Qty * UnitPrice,
                        UnitPrice = UnitPrice,
                        VatAmount =item.VatAmount,

                    };
                    DisplayLineItem.Add(disItem);
                }
            }
        }

    }
    public class EditOrderViewModelLineItem : ViewModelBase
    {
        public const string SequenceNoPropertyName = "SequenceNo";
        private int _sequenceNo = -1;
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

        public Guid ProductId { get; set; }

        public const string ProductPropertyName = "Product";
        private string _product = "";
        public string Product
        {
            get
            {
                return _product;
            }

            set
            {
                if (_product == value)
                {
                    return;
                }

                var oldValue = _product;
                _product = value;
                // Update bindings, no broadcast
                RaisePropertyChanged(ProductPropertyName);
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
                {
                    return;
                }
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
                {
                    return;
                }
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


        public const string QtyPropertyName = "Qty";
        private int _qty = 0;
        public int Qty
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
                // Update bindings, no broadcast
                RaisePropertyChanged(QtyPropertyName);
            }
        }
    }

    public class ProductDataGridDisplay : ViewModelBase
    {


        public Guid ProductId { get; set; }

        public const string ProductPropertyName = "Product";
        private string _product = "";
        public string Product
        {
            get
            {
                return _product;
            }

            set
            {
                if (_product == value)
                {
                    return;
                }

                var oldValue = _product;
                _product = value;
                // Update bindings, no broadcast
                RaisePropertyChanged(ProductPropertyName);
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
                {
                    return;
                }
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
                {
                    return;
                }
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


        public const string QtyPropertyName = "Qty";
        private int _qty = 0;
        public int Qty
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
                // Update bindings, no broadcast
                RaisePropertyChanged(QtyPropertyName);
            }
        }
    }


   public class ProductLookup
   {
       public Guid ProductId { get; set; }
       public string Name { get; set; }
   }
}