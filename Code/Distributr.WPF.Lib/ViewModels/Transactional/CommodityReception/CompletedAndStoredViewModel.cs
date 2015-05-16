using System.Collections.ObjectModel;
using System.Linq;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Repository.Transactional.SourcingDocumentRepositories;
using Distributr.WPF.Lib.ViewModels.Transactional.Orders;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace Distributr.WPF.Lib.ViewModels.Transactional.CommodityReception
{
    
    public class CompletedAndStoredViewModel : CommodityDocumentListViewModelBase
    {

       public CompletedAndStoredViewModel()
        {
           LoadedCommand = new RelayCommand(LoadPage);
           SearchCommand =new RelayCommand(Search);
           LineItem = new ObservableCollection<CompletedAndStoredCommodityListItem>();
           ViewSelectedItemCommand=new RelayCommand<CompletedAndStoredCommodityListItem>(ViewSelectedItem);
        }

      
       public RelayCommand LoadedCommand { get; set; }
       public RelayCommand SearchCommand { get; set; }
        public ObservableCollection<CompletedAndStoredCommodityListItem> LineItem { get; set; }
        public RelayCommand<CompletedAndStoredCommodityListItem> ViewSelectedItemCommand { get; set; }

       private void LoadPage()
       {
           SetUp();
           LoadItems();
       }
        private void SetUp()
        {
            NameSrchParam = "";
            LineItem.Clear();
        }

        private void LoadItems()
        {
            if (LineItem.Any())
                LineItem.Clear();

           using (var c = NestedContainer)
           {
               var storageNotes = Using<ICommodityStorageRepository>(c).GetAll();
               storageNotes.ForEach(n => LineItem.Add(Map((CommodityStorageNote)n)));
           }

       }
        private void Search()
        {
            using (var c = NestedContainer)
            {   LineItem.Clear();
                var storageNotes = Using<ICommodityStorageRepository>(c).GetAll()
                    .Where(p => p.DocumentReference.ToLower().StartsWith(NameSrchParam.ToLower())).ToList();
                storageNotes.ForEach(n => LineItem.Add(Map((CommodityStorageNote)n)));
            }
        }

        private CompletedAndStoredCommodityListItem Map(CommodityStorageNote doc)
       {

           var item = new CompletedAndStoredCommodityListItem
           {
               DocumentId = doc.Id,
               DocumentReference = doc.DocumentReference,
               DateIssued = doc.DocumentDateIssued,
               ClerkName = doc.DocumentIssuerUser.Username,
               Description = doc.Description,
               NetWeight = doc.TotalNetWeight,
               NoOfContainers = doc.LineItems.Count,
               Status = doc.Status
           };
           return item;
       }
        private void ViewSelectedItem(CompletedAndStoredCommodityListItem selectedItem)
        {
            const string uri = "/views/CommodityReception/DocumentDetails.xaml";
            Messenger.Default.Send<DocumentDetailMessage>(new DocumentDetailMessage { Id = selectedItem.DocumentId, DocumentType = DocumentType.CommodityStorageNote});
            NavigateCommand.Execute(uri);
        }
    }
}