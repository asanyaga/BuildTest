using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.CommodityEntities;
using Distributr.Core.Domain.Master.EquipmentEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;
using GalaSoft.MvvmLight;
using StructureMap;

namespace Distributr.WPF.Lib.ViewModels.Transactional.CommodityPurchase
{
    public class CommodyLineItemViewModel : CommodityLineItemBase
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

        public const string ContainerTypePropertyName = "ContainerType";
        private ContainerType _containerType = null;
        public ContainerType ContainerType
        {
            get
            {
                return _containerType;
            }

            set
            {
                if (_containerType == value)
                {
                    return;
                }

                RaisePropertyChanging(ContainerTypePropertyName);
                _containerType = value;
                RaisePropertyChanged(ContainerTypePropertyName);
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

        public const string BubbleSpacePropertyName = "BubbleSpace";
        private decimal _bubbleSpace = 0;
        public decimal BubbleSpace
        {
            get
            {
                return _bubbleSpace;
            }

            set
            {
                if (_bubbleSpace == value)
                {
                    return;
                }

                RaisePropertyChanging(BubbleSpacePropertyName);
                _bubbleSpace = value;
                RaisePropertyChanged(BubbleSpacePropertyName);
            }
        }

        public const string TareWeightPropertyName = "TareWeight";
        private decimal _tareWeight = 0;
        public decimal TareWeight
        {
            get
            {
                return _tareWeight;
            }

            set
            {
                if (_tareWeight == value)
                {
                    return;
                }

                RaisePropertyChanging(TareWeightPropertyName);
                _tareWeight = value;
                RaisePropertyChanged(TareWeightPropertyName);
            }
        }

        public const string NoOfContainersPropertyName = "NoOfContainers";
        private decimal _noOfContainers = 0;
        public decimal NoOfContainers
        {
            get
            {
                return _noOfContainers;
            }

            set
            {
                if (_noOfContainers == value)
                {
                    return;
                }

                RaisePropertyChanging(NoOfContainersPropertyName);
                _noOfContainers = value;
                RaisePropertyChanged(NoOfContainersPropertyName);
            }
        }

        public const string NetWeightPropertyName = "NetWeight";
        private decimal _netWeight = 0m;
        public decimal NetWeight
        {
            get
            {
                return _netWeight;
            }

            set
            {
                if (_netWeight == value)
                {
                    return;
                }

                RaisePropertyChanging(NetWeightPropertyName);
                _netWeight = value;
                RaisePropertyChanged(NetWeightPropertyName);
            }
        }

        public const string ContainerNoPropertyName = "ContainerNo";
        private string _containerNo = "";
        public string ContainerNo
        {
            get
            {
                return _containerNo;
            }

            set
            {
                if (_containerNo == value)
                {
                    return;
                }

                RaisePropertyChanging(ContainerNoPropertyName);
                _containerNo = value;
                RaisePropertyChanged(ContainerNoPropertyName);
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

        public const string IsSelectedPropertyName = "IsSelected";
        private bool _isSelected;
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }

            set
            {
                if (_isSelected == value)
                {
                    return;
                }

               _isSelected = value;
                RaisePropertyChanged(IsSelectedPropertyName);
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

    public class CommodityLineItemBase : ViewModelBase
    {
        #region infrastructure
        protected StructureMap.IContainer NestedContainer
        {
            get { return ObjectFactory.Container.GetNestedContainer(); }
        }

        protected T Using<T>(StructureMap.IContainer container) where T : class
        {
            return container.GetInstance<T>();
        }
        #endregion
    }
}
