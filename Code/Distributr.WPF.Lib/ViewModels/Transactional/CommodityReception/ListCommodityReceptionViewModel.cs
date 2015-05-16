
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;
using Distributr.Core.Repository.Transactional.SourcingDocumentRepositories;
using GalaSoft.MvvmLight;

namespace Distributr.WPF.Lib.ViewModels.Transactional.CommodityReception
{
    public class ListCommodityReceptionViewModel : DistributrViewModelBase
    {
        #region Properties

        public ListCommodityReceptionViewModel()
        {
            AwaitingReceptionList = new ObservableCollection<AgriDocumentListItem>();
            AwaitingStorageList = new ObservableCollection<AgriDocumentListItem>();
            CompleteReceptionList = new ObservableCollection<AgriDocumentListItem>();

        }

        public ObservableCollection<AgriDocumentListItem> AwaitingStorageList { get; set; }
        public ObservableCollection<AgriDocumentListItem> CompleteReceptionList { get; set; }
        public ObservableCollection<AgriDocumentListItem> AwaitingReceptionList { get; set; }

        public const string PageProgressBarPropertyName = "PageProgressBar";
        private string _pageProgress = "";

        public string PageProgressBar
        {
            get { return _pageProgress; }

            set
            {
                if (_pageProgress == value)
                {
                    return;
                }

                RaisePropertyChanging(PageProgressBarPropertyName);
                _pageProgress = value;
                RaisePropertyChanged(PageProgressBarPropertyName);
            }
        }


        #endregion

        #region Methods

        public void ClearViewModel()
        {
            AwaitingReceptionList.Clear();
            AwaitingStorageList.Clear();
            CompleteReceptionList.Clear();
        }

        public int AwaitingStorageCount()
        {
            int count = 0;
            using (StructureMap.IContainer c = NestedContainer)
            {
                count = Using<ICommodityReceptionRepository>(c).GetPendingStorageCount();
            }

            return count;
        }

        public void LoadAwaitingStorage()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                var receptionNotes = Using<ICommodityReceptionRepository>(c).GetPendingStorage();
                AwaitingStorageList.Clear();
                receptionNotes.ForEach(n => AwaitingStorageList.Add(Map(n)));
            }
        }

        public int CompleteReceptionsCount()
        {
            int count = 0;
            using (StructureMap.IContainer c = NestedContainer)
            {
                count = Using<ICommodityStorageRepository>(c).GetCount();
            }

            return count;
        }

        public void LoadCompleteReceptions()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                var items = Using<ICommodityStorageRepository>(c).GetAll();
                CompleteReceptionList.Clear();
                items.ForEach(n => CompleteReceptionList.Add(Map(n)));
            }
        }

        private AgriDocumentListItem Map(SourcingDocument doc)
        {

            AgriDocumentListItem item = new AgriDocumentListItem
                                            {
                                                DocumentId = doc.Id,
                                                DocumentReference = doc.DocumentReference,
                                                DocumentDateIssued = doc.DocumentDateIssued,
                                                Clerk = doc.DocumentIssuerUser.Username,
                                                DocumentType = SplitByCaps(doc.DocumentType.ToString()),
                                                DocumentRecipentCC = doc.DocumentRecipientCostCentre.Name,
                                            };

            if (doc is CommodityReceptionNote)
            {
                var receptionNote = doc as CommodityReceptionNote;
                item.TotalWeight = receptionNote.TotalNetWeight;
                item.NumberOfContainers = receptionNote.LineItems.Count;

                CommodityPurchaseNote parent =
                    GetEntityById(typeof (CommodityPurchaseNote), doc.DocumentParentId) as CommodityPurchaseNote;
                if (parent != null)
                {
                    item.CommodityOwner = parent.CommodityOwner.FullName;
                    item.CommodityProducer = parent.CommodityProducer.Name;
                    item.DeliveredBy = parent.DeliveredBy;
                }

            }
            else if (doc is CommodityStorageNote)
            {
                var storageNote = doc as CommodityStorageNote;
                item.TotalWeight = storageNote.TotalNetWeight;
                item.NumberOfContainers = storageNote.LineItems.Count;
            }

            return item;
        }

        #endregion
    }

    #region OtherClasses
    public class AgriDocumentListItem : ViewModelBase
    {
        public Guid DocumentId { get; set; }
        public string DocumentReference { get; set; }
        public DateTime DocumentDateIssued { get; set; }
        public string CommodityOwner { get; set; }
        public string CommodityProducer { get; set; }
        public string DeliveredBy { get; set; }
        public string Clerk { get; set; }
        public decimal TotalWeight { get; set; }
        public int NumberOfContainers { get; set; }
        public string Driver { get; set; }
        public string DocumentRecipentCC { get; set; }
        public string DocumentType { get; set; }
       
        public const string IsSelectedPropertyName = "IsSelected";
        private bool _isSelected = false;
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

                RaisePropertyChanging(IsSelectedPropertyName);
                _isSelected = value;
                RaisePropertyChanged(IsSelectedPropertyName);
            }
        }
    }
    #endregion
}
