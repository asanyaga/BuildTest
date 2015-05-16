using System;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.WPF.Lib.ViewModels;

namespace Distributr.WPF.Lib.WorkFlow.Orders
{
    public class OrderLineItemBase : DistributrViewModelBase
    {
        public const string LineItemIdPropertyName = "LineItemId";
        private Guid _lineItemId = Guid.Empty;
        public Guid LineItemId
        {
            get
            {
                return _lineItemId;
            }

            set
            {
                if (_lineItemId == value)
                    return;
                var oldValue = _lineItemId;
                _lineItemId = value;
                RaisePropertyChanged(LineItemIdPropertyName);
            }
        }

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

                var oldValue = _productId;
                _productId = value;
                RaisePropertyChanged(ProductIdPropertyName);
            }
        }

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

                var oldValue = _productType;
                _productType = value;
                RaisePropertyChanged(ProductTypePropertyName);
            }
        }

        public const string OrderLineItemTypePropertyName = "OrderLineItemType";
        private OrderLineItemType _orderLineItemType = OrderLineItemType.DuringConfirmation;
        public OrderLineItemType OrderLineItemType
        {
            get
            {
                return _orderLineItemType;
            }

            set
            {
                if (_orderLineItemType == value)
                {
                    return;
                }

                var oldValue = _orderLineItemType;
                _orderLineItemType = value;
                RaisePropertyChanged(OrderLineItemTypePropertyName);
            }
        }

        public const string QtyPropertyName = "Qty";
        private decimal _qty = 0;
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
                // Update bindings, no broadcast
                RaisePropertyChanged(QtyPropertyName);
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


    }
}
