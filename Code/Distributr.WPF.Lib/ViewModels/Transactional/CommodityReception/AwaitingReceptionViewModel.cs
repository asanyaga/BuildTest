using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Distributr.Core.Domain.Master.CentreEntity;
using Distributr.Core.Domain.Master.CommodityEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;
using Distributr.Core.Repository.Master.CentreRepositories;
using Distributr.Core.Repository.Master.CommodityRepositories;
using Distributr.Core.Repository.Master.CostCentreRepositories;
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
        public RelayCommand SearchCommand { get; set; }
        public ObservableCollection<CommodityDeliveryListItem> LineItem { get; set; }
        public RelayCommand<CommodityDeliveryListItem> WeighandReceiveCommand { get; set; }
        public RelayCommand<CommodityDeliveryListItem> ViewSelectedItemCommand { get; set; }
        public AwaitingReceptionViewModel()
        {
            LineItem = new ObservableCollection<CommodityDeliveryListItem>();
            LoadedCommand = new RelayCommand(LoadItem);
            WeighandReceiveCommand = new RelayCommand<CommodityDeliveryListItem>(WeighandReceive);
            ViewSelectedItemCommand = new RelayCommand<CommodityDeliveryListItem>(ViewSelectedItem);
            SearchCommand = new RelayCommand(Search);
        }
        
        private void WeighandReceive(CommodityDeliveryListItem item)
        {
            using (var c = NestedContainer)
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
            item.NetWeight = TruncateDecimal(sourcingDocument.LineItems.Sum(s => s.Weight), 1);
            item.TareWeight = TruncateDecimal(sourcingDocument.LineItems.Sum(k => k.ContainerType.TareWeight), 1);
            item.GrossWeight = item.NetWeight + item.TareWeight;
            LineItem.Add(item);
        }

        private void ViewSelectedItem(CommodityDeliveryListItem selectedItem)
        {
            const string uri = "/views/CommodityReception/DocumentDetails.xaml";
            string messagesource = "/views/CommodityReception/AwaitingReception.xaml";
            Messenger.Default.Send<DocumentDetailMessage>(new DocumentDetailMessage { Id = selectedItem.DocumentId, DocumentType = DocumentType.CommodityDelivery, MessageSourceUrl = messagesource });
            NavigateCommand.Execute(uri);
        }

        private void Search()
        {
            if (!string.IsNullOrEmpty(NameSrchParam))
            {
                using (var c = NestedContainer)
                {
                    var deliveryRepository = Using<ICommodityDeliveryRepository>(c);
                    var docs = deliveryRepository.GetAllByStatus(DocumentSourcingStatus.Confirmed);
                    var ccentre = Using<ICentreRepository>(c);
                    var allcc = ccentre.GetAll();
                    var allroutes = Using<IRouteRepository>(c).GetAll();
                    var routes = allroutes as IList<Route> ?? allroutes.ToList();
                    var centres = allcc as IList<Centre> ?? allcc.ToList();
                    IEnumerable<CommodityDeliveryNote> filteredItems = null;

                    var comm = Using<ICommodityRepository>(c).GetAll();
                    var commodities = comm as IList<Commodity> ?? comm.ToList();
                    LineItem.Clear();
                    foreach (var t in docs)
                    {
                        LineItem.Clear();
                        var f = t.LineItems;
                        foreach (var v in f)
                        {
                            filteredItems = from doc in docs
                                            join cm in commodities on v.Commodity.Id equals cm.Id into cmGroup
                                            from cm in cmGroup
                                            orderby cm.Name
                                            select doc;
                        }
                        foreach (var item in filteredItems)
                        {
                            var items = item.LineItems.Where(d => d.Commodity.Name.ToLower().Contains(NameSrchParam)
                                || d.CommodityGrade.Name.ToLower().Contains(NameSrchParam));
                            int itemNo = items.Count();
                            if (itemNo > 0)
                            {
                                AddItemToList(item);
                            }
                        }

                    }
                    //f by route name and centre 
                    var y = from doc in docs
                            join centre in centres on doc.CentreId equals centre.Id into cGroup
                            from dc in cGroup
                            join rt in routes on doc.RouteId equals rt.Id into rGroup
                            from rt in rGroup
                            where rt.Name.ToLower().Contains(NameSrchParam) || dc.Name.ToLower().Contains(NameSrchParam)
                            select doc;
                    foreach (var item in y)
                    {
                        AddItemToList(item);
                    }

                }

            }
            else
            {
                LoadItem();
            }
        }

    }
}