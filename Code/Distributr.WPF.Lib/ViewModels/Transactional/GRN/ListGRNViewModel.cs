using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows;
using Distributr.Core.Domain.Master.Util;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.OrderDocumentEntities;
using Distributr.Core.Repository.Transactional.DocumentRepositories.InventoryRepositories;
using Distributr.WPF.Lib.Services.Service;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.UI.Pages;
using Distributr.WPF.Lib.ViewModels.Admin;
using Distributr.WPF.Lib.ViewModels.Transactional.Orders;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Linq;
using System.Collections.Generic;
using GalaSoft.MvvmLight.Messaging;
using StructureMap;

namespace Distributr.WPF.Lib.ViewModels.Transactional.GRN
{
    public class ListGRNViewModel :ListingsViewModelBase
    {
        protected IPagenatedList<InventoryReceivedNote> PagedDocumentList;
        private IEnumerable<string> _cacheDocReferences; 
        public ListGRNViewModel()
        {
            GRNList = new ObservableCollection<ListGRNItemViewModel>();
        }

        #region properties

        private RelayCommand<ListGRNItemViewModel> _selectViewerAndGoCommand { get; set; }
        public RelayCommand<ListGRNItemViewModel> SelectViewerAndGoCommand
        {
            get
            {
                return _selectViewerAndGoCommand ??
                       (_selectViewerAndGoCommand = new RelayCommand<ListGRNItemViewModel>(DoSelectViewerAndGo));
            }
        }

        private RelayCommand<ListGRNItemViewModel> _viewPrintableGRNCommand;

        public RelayCommand<ListGRNItemViewModel> ViewPrintableGRNCommand
        {
            get
            {
                return _viewPrintableGRNCommand ??
                       (_viewPrintableGRNCommand = new RelayCommand<ListGRNItemViewModel>(GRNPrintView));
            }
        }

        private RelayCommand _clearSearchTextCommand { get; set; }
        public RelayCommand ClearSearchTextCommand
        {
            get
            {
                return _clearSearchTextCommand ??
                       (_clearSearchTextCommand = new RelayCommand (ClearSerachText));
            }
        }

       public ObservableCollection<ListGRNItemViewModel> GRNList { get; set; }
        
        #endregion

        #region methods

        protected override void Load(bool isFirstLoad = false)
        {
            if (isFirstLoad)
                SetUp();
                LoadGRNs();
        }

        private void SetUp()
        {
           GRNList.Clear();
           using (var c = NestedContainer)
           {
               var recordsPerPageSetting = Using<IGeneralSettingRepository>(c).GetByKey(GeneralSettingKey.RecordsPerPage);
               ItemsPerPage = recordsPerPageSetting != null ? Convert.ToInt32(recordsPerPageSetting.SettingValue) : 10;
               CurrentPage = 1;
               
           }

        }

     
        protected override void GoToPage(PageDestinations page)
        {
            GoToPageBase(page, PagedDocumentList.PageCount);
        }

        protected override void ComboPageSizesSelectionChanged(int take)
        {
            ItemsPerPage = take;
            Load();
        }

        protected override void UpdatePagenationControl()
        {
            UpdatePagenationControlBase(PagedDocumentList.PageNumber, PagedDocumentList.PageCount,
                                        PagedDocumentList.TotalItemCount,
                                        PagedDocumentList.IsFirstPage, PagedDocumentList.IsLastPage);
        }
        private void ClearSerachText()
        {
            this.SearchText = string.Empty;
        }
        #region UnUsed imports
        protected override void EditSelected()
        {
            throw new NotImplementedException();
        }
        protected override void ActivateSelected()
        {
            throw new NotImplementedException();
        }

        protected override void DeleteSelected()
        {
            throw new NotImplementedException();
        }
        #endregion

        private void LoadGRNs()
        {
            Application.Current.Dispatcher.BeginInvoke(
                new Action(delegate
                               {
                                   using (var c = NestedContainer)
                                   {
                                       var irns =
                                           Using<IInventoryReceivedNoteRepository>(c).GetAll().OrderByDescending(p=>p.DocumentDateIssued).ToList();
                                       if (!string.IsNullOrEmpty(SearchText))
                                       {
                                           irns =
                                               irns.Where(
                                                   p =>
                                                   (p.OrderReferences != null &&
                                                    p.OrderReferences.ToLower().Contains(SearchText.ToLower()))
                                                   || p.DocumentReference.ToLower().Contains(SearchText.ToLower())
                                                   ||
                                                   p.DocumentIssuerUser != null &&
                                                   p.DocumentIssuerUser.Username.ToLower().Contains(
                                                       SearchText.ToLower())
                                                   ).ToList();
                                       }

                                       if (irns.Any())
                                       {
                                           PagedDocumentList = new PagenatedList<InventoryReceivedNote>(
                                               irns.AsQueryable(), CurrentPage,
                                               ItemsPerPage, irns.Count());

                                           UpdatePagenationControl();
                                           Map(PagedDocumentList);
                                       }
                                   }
                               }));

        }
        private void Map(IEnumerable<InventoryReceivedNote> irns)
        {   GRNList.Clear();
            irns.ToList().ForEach(n => GRNList.Add(new ListGRNItemViewModel
            {
                CreatedBy =
                    n.DocumentIssuerUser == null
                        ? "--"
                        : n.DocumentIssuerUser.Username,
                DocumentRef = n.DocumentReference,
                DateReceived = n.DocumentDateIssued.ToString("dd-MMM-yyyy"),
                LoadNo = n.LoadNo,
                OrderReferences = n.OrderReferences,
                TotalValue = n.LineItems.Sum(x => x.Qty * x.Value).ToString("0.00"),
                Id = n.Id
            }));
        }
        
       

        private void DoSelectViewerAndGo(ListGRNItemViewModel grnItem)
        {
            SendNavigationRequestMessage(new Uri("/views/grn/addgrn.xaml?" + grnItem.Id, UriKind.Relative));
        }

        private void GRNPrintView(ListGRNItemViewModel grnItem)
        {
            if(grnItem != null)
            {
                using (var c = NestedContainer)
                {
                    Using<IPrintableDocumentViewer>(c).ViewDocument(grnItem.Id, DocumentType.InventoryReceivedNote);
                }
            }
        }

        #endregion

    }

    #region helpers

    public class ListGRNItemViewModel : ViewModelBase
    {
        public const string DocumentRefPropertyName = "DocumentRef";
        private string _documentRef = "";

        public string DocumentRef
        {
            get { return _documentRef; }

            set
            {
                if (_documentRef == value)
                    return;
                var oldValue = _documentRef;
                _documentRef = value;
                RaisePropertyChanged(DocumentRefPropertyName);
            }
        }

        public const string DateReceivedPropertyName = "DateReceived";
        private string _dateReceived = "";

        public string DateReceived
        {
            get { return _dateReceived; }

            set
            {
                if (_dateReceived == value)
                    return;
                var oldValue = _dateReceived;
                _dateReceived = value;
                RaisePropertyChanged(DateReceivedPropertyName);
            }
        }

        public const string OrderReferencesPropertyName = "OrderReferences";
        private string _orderReferences = "";

        public string OrderReferences
        {
            get { return _orderReferences; }

            set
            {
                if (_orderReferences == value)
                    return;
                var oldValue = _orderReferences;
                _orderReferences = value;
                RaisePropertyChanged(OrderReferencesPropertyName);
            }
        }

        public const string LoadNoPropertyName = "LoadNo";
        private string _loadNo = "";

        public string LoadNo
        {
            get { return _loadNo; }

            set
            {
                if (_loadNo == value)
                    return;
                var oldValue = _loadNo;
                _loadNo = value;
                RaisePropertyChanged(LoadNoPropertyName);
            }
        }

        public const string TotalValuePropertyName = "TotalValue";
        private string _totalValue = "";

        public string TotalValue
        {
            get { return _totalValue; }

            set
            {
                if (_totalValue == value)
                    return;
                var oldValue = _totalValue;
                _totalValue = value;
                RaisePropertyChanged(TotalValuePropertyName);
            }
        }

        public const string CreatedByPropertyName = "CreatedBy";
        private string _createdBy = "";

        public string CreatedBy
        {
            get { return _createdBy; }

            set
            {
                if (_createdBy == value)
                    return;
                var oldValue = _createdBy;
                _createdBy = value;
                RaisePropertyChanged(CreatedByPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="Id" /> property's name.
        /// </summary>
        public const string IdPropertyName = "Id";

        private Guid _Id = Guid.Empty;

        /// <summary>
        /// Gets the Id property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public Guid Id
        {
            get { return _Id; }

            set
            {
                if (_Id == value)
                {
                    return;
                }

                var oldValue = _Id;
                _Id = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(IdPropertyName);
            }
        }
    }

    #endregion
    
}
