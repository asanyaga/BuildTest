using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using GalaSoft.MvvmLight;

namespace Distributr.WPF.Lib.ViewModels.Transactional.Orders
{
    public class OrderItemSummary : ViewModelBase
    {
       
        public const string OrderIdPropertyName = "OrderId";
        private Guid _orderId = Guid.Empty;
        public Guid OrderId
        {
            get
            {
                return _orderId;
            }

            set
            {
                if (_orderId == value)
                {
                    return;
                }

                RaisePropertyChanging(OrderIdPropertyName);
                _orderId = value;
                RaisePropertyChanged(OrderIdPropertyName);
            }
        }
     

        
        public const string OrderReferencePropertyName = "OrderReference";
        private string _orderref = "";
        public string OrderReference
        {
            get
            {
                return _orderref;
            }

            set
            {
                if (_orderref == value)
                {
                    return;
                }

                RaisePropertyChanging(OrderReferencePropertyName);
                _orderref = value;
                RaisePropertyChanged(OrderReferencePropertyName);
            }
        }

        
        public const string RequiredPropertyName = "Required";
        private DateTime _Required = DateTime.Now;
        public DateTime Required
        {
            get
            {
                return _Required;
            }

            set
            {
                if (_Required == value)
                {
                    return;
                }

                RaisePropertyChanging(RequiredPropertyName);
                _Required = value;
                RaisePropertyChanged(RequiredPropertyName);
            }
        }

       
        public const string NetAmountPropertyName = "NetAmount";
        private decimal _netamount = 0;
        public decimal NetAmount
        {
            get
            {
                return _netamount;
            }

            set
            {
                if (_netamount == value)
                {
                    return;
                }

                RaisePropertyChanging(NetAmountPropertyName);
                _netamount = value;
                RaisePropertyChanged(NetAmountPropertyName);
            }
        }

      
        public const string TotalVatPropertyName = "TotalVat";
        private decimal _totalvat = 0;
        public decimal TotalVat
        {
            get
            {
                return _totalvat;
            }

            set
            {
                if (_totalvat == value)
                {
                    return;
                }

                RaisePropertyChanging(TotalVatPropertyName);
                _totalvat = value;
                RaisePropertyChanged(TotalVatPropertyName);
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

        public const string SaleDiscountPropertyName = "SaleDiscount";
        private decimal _saleDiscount = 0;
        public decimal SaleDiscount
        {
            get
            {
                return _saleDiscount;
            }

            set
            {
                if (_saleDiscount == value)
                {
                    return;
                }

                RaisePropertyChanging(SaleDiscountPropertyName);
                _saleDiscount = value;
                RaisePropertyChanged(SaleDiscountPropertyName);
            }
        }

       
        public const string PaidAmountPropertyName = "PaidAmount";
        private decimal _paidAmount = 0;
        public decimal PaidAmount
        {
            get
            {
                return _paidAmount;
            }

            set
            {
                if (_paidAmount == value)
                {
                    return;
                }

                RaisePropertyChanging(PaidAmountPropertyName);
                _paidAmount = value;
                RaisePropertyChanged(PaidAmountPropertyName);
            }
        }

        
        public const string OutstandingAmountPropertyName = "OutstandingAmount";
        private decimal _outstandingAmount = 0;
        public decimal OutstandingAmount
        {
            get
            {
                return _outstandingAmount;
            }

            set
            {
                if (_outstandingAmount == value)
                {
                    return;
                }

                RaisePropertyChanging(OutstandingAmountPropertyName);
                _outstandingAmount = value;
                RaisePropertyChanged(OutstandingAmountPropertyName);
            }
        }

       
        public const string StatusPropertyName = "Status";
        private DocumentStatus _status = DocumentStatus.Approved;
        public DocumentStatus Status
        {
            get
            {
                return _status;
            }

            set
            {
                if (_status == value)
                {
                    return;
                }

                RaisePropertyChanging(StatusPropertyName);
                _status = value;
                RaisePropertyChanged(StatusPropertyName);
            }
        }

        
        public const string SalesmanPropertyName = "Salesman";
        private string _salesman = "";
        public string Salesman
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

                RaisePropertyChanging(SalesmanPropertyName);
                _salesman = value;
                RaisePropertyChanged(SalesmanPropertyName);
            }
        }

        public const string OutletPropertyName = "Outlet";
        private string _outlet = "";
        public string Outlet
        {
            get
            {
                return _outlet;
            }

            set
            {
                if (_outlet == value)
                {
                    return;
                }

                RaisePropertyChanging(OutletPropertyName);
                _outlet = value;
                RaisePropertyChanged(OutletPropertyName);
            }
        }



        public const string ExRefNoPropertyName = "ExRefNo";
        private string _ExRefNo = "";
        public string ExRefNo
        {
            get
            {
                return _ExRefNo;
            }

            set
            {
                if (_ExRefNo == value)
                {
                    return;
                }

                RaisePropertyChanging(ExRefNoPropertyName);
                _ExRefNo = value;
                RaisePropertyChanged(ExRefNoPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="SequenceNo" /> property's name.
        /// </summary>
        public const string SequenceNoPropertyName = "SequenceNo";

        private int _sequence = 0;

        /// <summary>
        /// Sets and gets the SequenceNo property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int SequenceNo
        {
            get
            {
                return _sequence;
            }

            set
            {
                if (_sequence == value)
                {
                    return;
                }

                RaisePropertyChanging(SequenceNoPropertyName);
                _sequence = value;
                RaisePropertyChanged(SequenceNoPropertyName);
            }
        }

        public const string ShippingAddressPropertyName = "ShippingAddress";
        private string _shippingAddress = "";
        public string ShippingAddress
        {
            get
            {
                return _shippingAddress;
            }

            set
            {
                if (_shippingAddress == value)
                {
                    return;
                }

                RaisePropertyChanging(ShippingAddressPropertyName);
                _shippingAddress = value;
                RaisePropertyChanged(ShippingAddressPropertyName);
            }
        }

        public const string IsCheckedPropertyName = "IsChecked";
        private bool _myProperty = false;
        public bool IsChecked
        {
            get
            {
                return _myProperty;
            }

            set
            {
                if (_myProperty == value)
                {
                    return;
                }

                RaisePropertyChanging(IsCheckedPropertyName);
                _myProperty = value;
                RaisePropertyChanged(IsCheckedPropertyName);
            }
        }


    }

    public class OrderDispatchItemSummary : OrderItemSummary
    {
       
        public const string OrderRoutePropertyName = "OrderRoute";
        private Route _route = null;
        public Route OrderRoute
        {
            get
            {
                return _route;
            }

            set
            {
                if (_route == value)
                {
                    return;
                }

                RaisePropertyChanging(OrderRoutePropertyName);
                _route = value;
                RaisePropertyChanged(OrderRoutePropertyName);
            }
        }

       
        public const string ChangeToSalesmanPropertyName = "ChangeToSalesman";
        private DistributorSalesman _changeToSalesman = null;
        public DistributorSalesman ChangeToSalesman
        {
            get
            {
                return _changeToSalesman;
            }

            set
            {
                if (_changeToSalesman == value)
                {
                    return;
                }

                RaisePropertyChanging(ChangeToSalesmanPropertyName);
                _changeToSalesman = value;
                RaisePropertyChanged(ChangeToSalesmanPropertyName);
            }
        }
    }

    
}
