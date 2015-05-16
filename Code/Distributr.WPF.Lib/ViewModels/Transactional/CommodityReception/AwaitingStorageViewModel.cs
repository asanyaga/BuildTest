using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Repository.Transactional.SourcingDocumentRepositories;
using Distributr.WPF.Lib.UI.Pages;
using Distributr.WPF.Lib.ViewModels.Transactional.Orders;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace Distributr.WPF.Lib.ViewModels.Transactional.CommodityReception
{
   
    public class AwaitingStorageViewModel : CommodityDocumentListViewModelBase
    {
      
        public AwaitingStorageViewModel()
        {
            LineItem = new ObservableCollection<CommodityReceptionListItem>();
            SelectedLineItems = new List<Guid>();
            LoadItemsCommand = new RelayCommand(LoadItems);
            AwaitingStoragePageLoadedCommand = new RelayCommand(LoadPage);
            StoreSelectedItemsCommand = new RelayCommand(StoreSelection);
            SelectAllItemsCommand = new RelayCommand<object>(SelectAllItems);
            SearchCommand = new RelayCommand(Search);
            StoreSingleItemCommand = new RelayCommand<CommodityReceptionListItem>(StoreSelectedItem);
            ViewSelectedItemCommand = new RelayCommand<CommodityReceptionListItem>(ViewSelectedItem);


        }

      
        #region properties
        public ObservableCollection<CommodityReceptionListItem> LineItem { get; set; }
        public List<Guid> SelectedLineItems { get; set; }
        public RelayCommand LoadItemsCommand { get; set; }
        public RelayCommand StoreSelectedItemsCommand { get; set; }
        public RelayCommand AwaitingStoragePageLoadedCommand { get; set; }
        public RelayCommand<object>  SelectAllItemsCommand { get; set; }
        public RelayCommand SearchCommand { get; set; }
        public RelayCommand<CommodityReceptionListItem> StoreSingleItemCommand { get; set; }
        public RelayCommand <CommodityReceptionListItem> ViewSelectedItemCommand { get; set; }

        #endregion

        #region Methods

        private void SelectAllItems(object sender)
        {
            var checkbox = sender as CheckBox;
            foreach (var item in LineItem)
            {
                item.IsChecked = checkbox.IsChecked.Value;

            }
            
           
        }

        private void LoadPage()
        {
            SetUp();
            LoadItems();
        }


        public override void SetUp()
        {
            LineItem.Clear();
            base.SetUp();
        }
        private void Search()
        {
            using (var c = NestedContainer)
            {
                LineItem.Clear();
                var storageNotes = Using<IReceivedDeliveryRepository>(c).GetPendingStorage()
                    .Where(p => p.DocumentReference.ToLower().StartsWith(NameSrchParam.ToLower())).ToList();
                storageNotes.ForEach(n => LineItem.Add(Map(n)));
            }
        }

        private void LoadItems()
        {
            
            using (var c = NestedContainer)
            {
                var receivedDeliveryNotes = Using<IReceivedDeliveryRepository>(c).GetPendingStorage();
              //  var receptionNotes = Using<ICommodityReceptionRepository>(c).GetPendingStorage();
               // receptionNotes.ForEach(n => LineItem.Add(Map(n)));

                receivedDeliveryNotes.ForEach(n => LineItem.Add(Map(n)));
            }

        }
        private CommodityReceptionListItem Map(ReceivedDeliveryNote doc)
        {

            var item = new CommodityReceptionListItem
            {
                DocumentId = doc.Id,
                DocumentReference = doc.DocumentReference,
                DateIssued = doc.DocumentDateIssued,
                ClerkName = doc.DocumentIssuerUser.Username,
                Description = doc.Description,
                NetWeight =TruncateDecimal(doc.TotalNetWeight,1),
                NoOfContainers = doc.LineItems.Count,
                Status = doc.Status
            };
            return item;
        }
       
        private void StoreSelection()
        {
            using (var c=NestedContainer)
            {
                List<Guid> list = (from p in LineItem where p.IsChecked select p.DocumentId).ToList();
                if(list.Any())
                {
                    Using<IStoreCommodityPopUp>(c).ShowCommodityToStore(list);
                    LineItem.Clear();
                    LoadItems();
                    
                }
                else
                {
                    MessageBox.Show("You must select item to store first.", "Agrimanagr: Store Commodity",
                                 MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
               
            }

        }
        private void StoreSelectedItem(CommodityReceptionListItem selectedItem)
        {
            if (selectedItem == null) return;
            
            using (var c = NestedContainer)
            {  SelectedLineItems.Clear();
               SelectedLineItems.Add(selectedItem.DocumentId);
               Using<IStoreCommodityPopUp>(c).ShowCommodityToStore(SelectedLineItems);
               LineItem.Clear();
                LoadItems();
            }
        }
        
        private void ViewSelectedItem(CommodityReceptionListItem selectedItem)
        {
            const string uri = "/views/CommodityReception/DocumentDetails.xaml";
            Messenger.Default.Send<DocumentDetailMessage>(new DocumentDetailMessage { Id = selectedItem.DocumentId, DocumentType = DocumentType.ReceivedDelivery });
            NavigateCommand.Execute(uri);
        }



        #endregion


    }
}