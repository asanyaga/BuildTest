using System;
using System.Collections.ObjectModel;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Repository.Transactional.SourcingDocumentRepositories;
using Distributr.WPF.Lib.UI.Pages;
using Distributr.WPF.Lib.ViewModels.Transactional.Orders;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;

namespace Distributr.WPF.Lib.ViewModels.Transactional.CommodityReception
{
   
    public class AwaitingReceptionViewModel : CommodityDocumentListViewModelBase
    {
        public RelayCommand LoadedCommand { get; set; }
        public ObservableCollection<CommodityDeliveryListItem> LineItem { get; set; }
        public RelayCommand<CommodityDeliveryListItem> WeighandReceiveCommand { get; set; }
        public RelayCommand<CommodityDeliveryListItem> ViewSelectedItemCommand { get; set; }
        public AwaitingReceptionViewModel()
        {
            LineItem = new ObservableCollection<CommodityDeliveryListItem>();
            LoadedCommand = new RelayCommand(LoadItem);
            WeighandReceiveCommand = new RelayCommand<CommodityDeliveryListItem>(WeighandReceive);
            ViewSelectedItemCommand = new RelayCommand<CommodityDeliveryListItem>(ViewSelectedItem);
        }

        private void WeighandReceive(CommodityDeliveryListItem item)
        {
            using(var c= NestedContainer)
            {
                Using<IWeighAndReceivePopUp>(c).ShowWeighAndReceive(item.DocumentId);
                LoadItem();
            }

        }

        private void LoadItem()
        {
            LineItem.Clear();
            using (var c = NestedContainer)
            {
                var deliveryRepository = Using<ICommodityDeliveryRepository>(c);
                var docs = deliveryRepository.GetAllByStatus(DocumentSourcingStatus.Confirmed);
                foreach (var item in docs)
                {
                    AddItemToList(item);
                }
            }
        }

        private void AddItemToList(CommodityDeliveryNote sourcingDocument)
        {
            var item = new CommodityDeliveryListItem();
            item.DocumentId = sourcingDocument.Id;
            item.DocumentReference = sourcingDocument.DocumentReference;
            item.Description = sourcingDocument.Description;
            item.DateIssued = sourcingDocument.DocumentDateIssued;
            item.ClerkName = sourcingDocument.DocumentIssuerUser.Username;
            item.NoOfContainers = sourcingDocument.LineItems.GroupBy(s => s.ContainerNo).Count();
            item.Status = sourcingDocument.Status;
            item.DriverName = sourcingDocument.DriverName;
            item.VehicleRegNo = sourcingDocument.VehiclRegNo;
            item.NetWeight =TruncateDecimal(sourcingDocument.LineItems.Sum(s => s.Weight),1);
            item.TareWeight = TruncateDecimal(sourcingDocument.LineItems.Sum(k => k.ContainerType.TareWeight), 1);
            item.GrossWeight = item.NetWeight + item.TareWeight;
            LineItem.Add(item);
        }

        private void ViewSelectedItem(CommodityDeliveryListItem selectedItem)
        {
            const string uri = "/views/CommodityReception/DocumentDetails.xaml";
            string messagesource="/views/CommodityReception/AwaitingReception.xaml";
            Messenger.Default.Send<DocumentDetailMessage>(new DocumentDetailMessage { Id = selectedItem.DocumentId, DocumentType = DocumentType.CommodityDelivery, MessageSourceUrl = messagesource });
            NavigateCommand.Execute(uri);
        }

    }
}