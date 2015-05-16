using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.CommodityEntities;
using Distributr.Core.Domain.Master.CommodityEntity;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;
using GalaSoft.MvvmLight;

namespace Distributr.WPF.Lib.ViewModels.Warehousing
{
    public class WarehouseLineItemViewModel : ViewModelBase
    {

        public const string LineItemIdPropertyName = "LineItemId";
        private Guid _lineItemid = Guid.Empty;
        public Guid Id
        {
            get
            {
                return _lineItemid;
            }

            set
            {
                if (_lineItemid == value)
                {
                    return;
                }

                RaisePropertyChanging(LineItemIdPropertyName);
                _lineItemid = value;
                RaisePropertyChanged(LineItemIdPropertyName);
            }
        }

        public const string DocumentIdPropertyName = "DocumentId";
        private Guid _documentId = Guid.Empty;
        public Guid DocumentId
        {
            get
            {
                return _documentId;
            }

            set
            {
                if (_documentId == value)
                {
                    return;
                }

                RaisePropertyChanging(DocumentIdPropertyName);
                _documentId = value;
                RaisePropertyChanged(DocumentIdPropertyName);
            }
        }

        public const string ParentLineItemIdPropertyName = "ParentLineItemId";
        private Guid _parentLineItemId = Guid.Empty;
        public Guid ParentLineItemId
        {
            get
            {
                return _parentLineItemId;
            }

            set
            {
                if (_parentLineItemId == value)
                {
                    return;
                }

                RaisePropertyChanging(ParentLineItemIdPropertyName);
                _parentLineItemId = value;
                RaisePropertyChanged(ParentLineItemIdPropertyName);
            }
        }

        public const string CommodityPropertyName = "Commodity";
        private Commodity _commodity;
        public Commodity Commodity
        {
            get { return _commodity; }

            set
            {
                if (_commodity == value)
                {
                    return;
                }

                RaisePropertyChanging(CommodityPropertyName);
                _commodity = value;
                RaisePropertyChanged(CommodityPropertyName);
            }

        }

       

        public const string CommodityGradePropertyName = "CommodityGrade";
        private CommodityGrade _commodityGrade;
        public CommodityGrade CommodityGrade
        {
            get
            {
                return _commodityGrade;
            }

            set
            {
                if (_commodityGrade == value)
                {
                    return;
                }

                RaisePropertyChanging(CommodityGradePropertyName);
                _commodityGrade = value;
                RaisePropertyChanged(CommodityGradePropertyName);
            }
        }

        public const string FarmerPropertyName = "Farmer";
        private CommodityOwner _farmer;
        public CommodityOwner Farmer
        {
            get
            {
                return _farmer;
            }

            set
            {
                if (_farmer == value)
                {
                    return;
                }

                RaisePropertyChanging(FarmerPropertyName);
                _farmer = value;
                RaisePropertyChanged(FarmerPropertyName);
            }
        }

        public const string GrossWeightPropertyName = "GrossWeight";
        private decimal _grossWeight = 0;
        public decimal GrossWeight
        {
            get
            {
                return _grossWeight;
            }

            set
            {
                if (_grossWeight == value)
                {
                    return;
                }

                RaisePropertyChanging(GrossWeightPropertyName);
                _grossWeight = value;
                RaisePropertyChanged(GrossWeightPropertyName);
            }
        }

       

        public const string NotePropertyName = "Note";
        private string _note = "";
        public string Note
        {
            get
            {
                return _note;
            }

            set
            {
                if (_note == value)
                {
                    return;
                }

                RaisePropertyChanging(NotePropertyName);
                _note = value;
                RaisePropertyChanged(NotePropertyName);
            }
        }

        public const string DescriptionPropertyName = "Description";
        private string _description = "";
        public string Description
        {
            get
            {
                return _description;
            }

            set
            {
                if (_description == value)
                {
                    return;
                }

                RaisePropertyChanging(DescriptionPropertyName);
                _description = value;
                RaisePropertyChanged(DescriptionPropertyName);
            }
        }

       

        public const string LineItemStatusPropertyName = "LineItemStatus";
        private SourcingLineItemStatus _lineItemStatus;
        public SourcingLineItemStatus LineItemStatus
        {
            get
            {
                return _lineItemStatus;
            }

            set
            {
                if (_lineItemStatus == value)
                {
                    return;
                }

                _lineItemStatus = value;
                RaisePropertyChanged(LineItemStatusPropertyName);
            }
        }
    }
}
