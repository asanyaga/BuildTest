using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Windows.Threading;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.WPF.Lib.UI.UI_Utillity.Converters;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using StructureMap;
using IContainer = StructureMap.IContainer;

namespace Distributr.WPF.Lib.ViewModels.Transactional.Orders
{
    public class MainOrderLineItem : ProductMainOrderLineItemBase
    {

        public const string ProductNamePropertyName = "ProductName";
        private string _productName = "";
        public string ProductName
        {
            get
            {
                return _productName;
            }

            set
            {
                if (_productName == value)
                {
                    return;
                }

                RaisePropertyChanging(ProductNamePropertyName);
                _productName = value;
                RaisePropertyChanged(ProductNamePropertyName);
            }
        }


        public const string ProductIdPropertyName = "ProductId";
        private Guid _productId = Guid.Empty;
        public Guid ProductId
        {
            get
            {
                return _productId;
            }

            set
            {
                if (_productId == value)
                {
                    return;
                }

                RaisePropertyChanging(ProductIdPropertyName);
                _productId = value;
                RaisePropertyChanged(ProductIdPropertyName);
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
                {
                    return;
                }

                RaisePropertyChanging(SequenceNoPropertyName);
                _sequenceNo = value;
                RaisePropertyChanged(SequenceNoPropertyName);
            }
        }

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

                RaisePropertyChanging(BackOrderPropertyName);
                _backOrder = value;
                RaisePropertyChanged(BackOrderPropertyName);
            }
        }


        
        public const string ApprovableQuantityPropertyName = "ApprovableQuantity";
        private decimal _approvableQuantity = 0;
        public decimal ApprovableQuantity
        {
            get
            {
                return _approvableQuantity;
            }

            set
            {
                if (_approvableQuantity == value)
                {
                    return;
                }

                RaisePropertyChanging(ApprovableQuantityPropertyName);
                _approvableQuantity = value;
                RaisePropertyChanged(ApprovableQuantityPropertyName);
            }
        }

       
        public const string ProductPropertyName = "Product";
        private Product _product = null;
        public Product Product
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

                RaisePropertyChanging(ProductPropertyName);
                _product = value;
                RaisePropertyChanged(ProductPropertyName);
            }
        }

       
        public const string LossSaleQuantityPropertyName = "LossSaleQuantity";
        private decimal _losssaleQuantity = 0;
        public decimal LossSaleQuantity
        {
            get
            {
                return _losssaleQuantity;
            }

            set
            {
                if (_losssaleQuantity == value)
                {
                    return;
                }

                RaisePropertyChanging(LossSaleQuantityPropertyName);
                _losssaleQuantity = value;
                RaisePropertyChanged(LossSaleQuantityPropertyName);
            }
        }

    }
    public class ProductMainOrderLineItemBase : ViewModelBase
    {

        protected StructureMap.IContainer NestedContainer
        {
            get { return ObjectFactory.Container.GetNestedContainer(); }
        }

        protected T Using<T>(StructureMap.IContainer container) where T : class
        {
            return container.GetInstance<T>();
        }

        public const string QuantityPropertyName = "Quantity";
        private decimal _quantity = 1;
       
        public decimal Quantity
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

                RaisePropertyChanging(QuantityPropertyName);
                _quantity = value;
                // string qs = value.ToString(StringFormats.QuantityFormat);
                //decimal.TryParse(qs,out _quantity);
               
                RaisePropertyChanged(QuantityPropertyName);
            }
        }

        

        public const string UnitPricePropertyName = "UnitPrice";
        private decimal _unitprice = 0;
        public decimal UnitPrice
        {
            get
            {
                return _unitprice;
            }

            set
            {
                if (_unitprice == value)
                {
                    return;
                }

                RaisePropertyChanging(UnitPricePropertyName);
                _unitprice = value;
                RaisePropertyChanged(UnitPricePropertyName);
            }
        }


        public const string UnitDiscountPropertyName = "UnitDiscount";
        private decimal _unitdiscount = 0;
        public decimal UnitDiscount
        {
            get
            {
                return _unitdiscount;
            }

            set
            {
                if (_unitdiscount == value)
                {
                    return;
                }

                RaisePropertyChanging(UnitDiscountPropertyName);
                _unitdiscount = value;
                RaisePropertyChanged(UnitDiscountPropertyName);
            }
        }


        public const string TotalAmountPropertyName = "TotalAmount";
        private decimal _totalAmount = 0;
        public decimal TotalAmount
        {
            get
            {
                return _totalAmount;
            }

            set
            {
                if (_totalAmount == value)
                {
                    return;
                }

                RaisePropertyChanging(TotalAmountPropertyName);
                _totalAmount = value;
                RaisePropertyChanged(TotalAmountPropertyName);
            }
        }


        public const string UnitVATPropertyName = "UnitVAT";
        private decimal _unitVat = 0;
        public decimal UnitVAT
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

                RaisePropertyChanging(UnitVATPropertyName);
                _unitVat = value;
                RaisePropertyChanged(UnitVATPropertyName);
            }
        }



        public const string TotalVATPropertyName = "TotalVAT";
        private decimal _totalVat = 0;
        public decimal TotalVAT
        {
            get
            {
                return _totalVat;
            }

            set
            {
                if (_totalVat == value)
                {
                    return;
                }

                RaisePropertyChanging(TotalVATPropertyName);
                _totalVat = value;
                RaisePropertyChanged(TotalVATPropertyName);
            }
        }


        public const string GrossAmountPropertyName = "GrossAmount";
        private decimal _grossamount = 0;
        public decimal GrossAmount
        {
            get
            {
                return _grossamount;
            }

            set
            {
                if (_grossamount == value)
                {
                    return;
                }

                RaisePropertyChanging(GrossAmountPropertyName);
                _grossamount = value;
                RaisePropertyChanged(GrossAmountPropertyName);
            }
        }


        public const string ProductTypePropertyName = "ProductType";
        private string _productType = "";
        public string ProductType
        {
            get
            {
                return _productType;
            }

            set
            {
                if (_productType == value)
                {
                    return;
                }

                RaisePropertyChanging(ProductTypePropertyName);
                _productType = value;
                RaisePropertyChanged(ProductTypePropertyName);
            }
        }

       
        public const string TotalNetPropertyName = "TotalNet";
        private decimal _totalNet = 0;
        public decimal TotalNet
        {
            get
            {
                return _totalNet;
            }

            set
            {
                if (_totalNet == value)
                {
                    return;
                }

                RaisePropertyChanging(TotalNetPropertyName);
                _totalNet = value;
                RaisePropertyChanged(TotalNetPropertyName);
            }
        }

       
        public const string TotalProductDiscountPropertyName = "TotalProductDiscount";
        private decimal _totalProductDiscount = 0;

        public decimal TotalProductDiscount
        {
            get
            {
                return _totalProductDiscount;
            }

            set
            {
                if (_totalProductDiscount == value)
                {
                    return;
                }

                RaisePropertyChanging(TotalProductDiscountPropertyName);
                _totalProductDiscount = value;
                RaisePropertyChanged(TotalProductDiscountPropertyName);
            }
        }
        public const string AddedProductsCountPropertyName = "AddedProductsCount";
        private string _addedProductsCount = "Added products(0)";
        public string AddedProductsCount
        {
            get
            {
                return _addedProductsCount;
            }

            set
            {
                if (_addedProductsCount == value)
                {
                    return;
                }

                RaisePropertyChanging(AddedProductsCountPropertyName);
                _addedProductsCount = value;
                RaisePropertyChanged(AddedProductsCountPropertyName);
            }
        }
       
        public const string LineItemTypePropertyName = "LineItemType";
        private MainOrderLineItemType _lineType = MainOrderLineItemType.Sale;
        public MainOrderLineItemType LineItemType
        {
            get
            {
                return _lineType;
            }

            set
            {
                if (_lineType == value)
                {
                    return;
                }

                RaisePropertyChanging(LineItemTypePropertyName);
                _lineType = value;
                RaisePropertyChanged(LineItemTypePropertyName);
            }
        }

       
        public const string CanChangePropertyName = "CanChange";
        private bool _canchange = false;
        public bool CanChange
        {
            get
            {
                return _canchange;
            }

            set
            {
                if (_canchange == value)
                {
                    return;
                }

                RaisePropertyChanging(CanChangePropertyName);
                _canchange = value;
                RaisePropertyChanged(CanChangePropertyName);
            }
        }

        public const string DiscountTypePropertyName = "DiscountType";
        private DiscountType _discountType = DiscountType.None;
        public DiscountType DiscountType
        {
            get
            {
                return _discountType;
            }

            set
            {
                if (_discountType == value)
                {
                    return;
                }

                RaisePropertyChanging(DiscountTypePropertyName);
                _discountType = value;
                RaisePropertyChanged(DiscountTypePropertyName);
            }
        }

        public const string AvailablePropertyName = "Available";
        private decimal _available = 0;
        public decimal Available
        {
            get
            {
                return _available;
            }

            set
            {
                if (_available == value)
                {
                    return;
                }

                RaisePropertyChanging(AvailablePropertyName);
                _available = value;
                RaisePropertyChanged(AvailablePropertyName);
            }
        }

        public const string AvailableLabelPropertyName = "AvailableLabel";
        private string _availablelabel = "Available";
        public string AvailableLabel
        {
            get
            {
                return _availablelabel;
            }

            set
            {
                if (_availablelabel == value)
                {
                    return;
                }

                RaisePropertyChanging(AvailableLabelPropertyName);
                _availablelabel = value;
                RaisePropertyChanged(AvailableLabelPropertyName);
            }
        }
    }
}
