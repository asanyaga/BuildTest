using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;
using Distributr.Core.Repository.Transactional.SourcingDocumentRepositories;
using Distributr.WPF.Lib.UI.Pages;
using Distributr.WPF.Lib.ViewModels.Transactional.CommodityPurchase;
using Distributr.WPF.Lib.ViewModels.Transactional.Orders;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Transactional.CommodityReception
{
    public class DocumentDetailsViewModel : DistributrViewModelBase
    {
        public DocumentDetailsViewModel()
        {
            LineItems = new ObservableCollection<CommodyLineItemViewModel>();
            MoveBackCommand = new RelayCommand(CloseWindow);
            PageLoadedCommand= new RelayCommand(SetUp);
        }
        
        #region Methods
        public void SetId(DocumentDetailMessage msg)
        {
            DocumentId = msg.Id;
            DocumentType = msg.DocumentType;
            NavigatedFrom = msg.MessageSourceUrl == string.Empty ? "" : msg.MessageSourceUrl;
        }
       private void SetUp()
       {
           LineItems.Clear();
           DocumentReference = "";
           if(DocumentId !=Guid.Empty)
           {
               Load();
           }
           else
           {
               MessageBox.Show("Error loading Page", "Agrimanagr Error", MessageBoxButton.OK, MessageBoxImage.Error);
               CloseWindow();
           }

       }
       private void CloseWindow()
       {
            string uri = "/views/CommodityReception/AwaitingStorage.xaml";
           if (!string.IsNullOrEmpty(NavigatedFrom))
           {
               
               if (NavigatedFrom.Equals("/views/CommodityReception/StoreCommodity.xaml"))
               {
                   using (var c = NestedContainer)
                   {
                       var SelectedLineItems = new List<Guid>();
                       SelectedLineItems.Add(DocumentId);
                       Using<IStoreCommodityPopUp>(c).ShowCommodityToStore(SelectedLineItems);
                   }
               }
               else
               {
                   uri = NavigatedFrom;
               }

               NavigateCommand.Execute(uri);
           }
           else
           {
              
                   NavigateCommand.Execute(uri);
               
           }
          
       }

        private void Load()
        {

            using (var c = NestedContainer)
            {
                if (LineItems.Any())
                    LineItems.Clear();
                switch (DocumentType)
                {
                    case DocumentType.CommodityReceptionNote:
                        var receptionNote =
                            Using<ICommodityReceptionRepository>(c).GetById(DocumentId) as CommodityReceptionNote;
                        foreach (var lineitem in receptionNote.LineItems)
                        {
                            var lineItem = new CommodyLineItemViewModel
                                               {
                                                   Id = lineitem.Id,
                                                   GrossWeight = lineitem.Weight + lineitem.ContainerType.TareWeight,
                                                   NetWeight = lineitem.Weight,
                                                   ContainerType = lineitem.ContainerType,
                                                   Commodity = lineitem.Commodity,
                                                   CommodityGrade = lineitem.CommodityGrade,
                                                   ContainerNo = lineitem.ContainerNo,
                                                   Description = lineitem.Description,

                                               };
                            LineItems.Add(lineItem);
                        }
                        DocumentReference = receptionNote.DocumentReference;
                        break;

                    case DocumentType.CommodityPurchaseNote:
                        var purchaseNote =
                            Using<ICommodityPurchaseRepository>(c).GetById(DocumentId) as CommodityPurchaseNote;
                        foreach (var lineitem in purchaseNote.LineItems)
                        {
                            var lineItem = new CommodyLineItemViewModel
                                               {
                                                   Id = lineitem.Id,
                                                   GrossWeight = lineitem.Weight + lineitem.ContainerType.TareWeight,
                                                   NetWeight = lineitem.Weight,
                                                   ContainerType = lineitem.ContainerType,
                                                   Commodity = lineitem.Commodity,
                                                   CommodityGrade = lineitem.CommodityGrade,
                                                   ContainerNo = lineitem.ContainerNo,
                                                   Description = lineitem.Description,

                                               };
                            LineItems.Add(lineItem);
                        }
                        DocumentReference = purchaseNote.DocumentReference;
                        break;
                    case DocumentType.CommodityStorageNote:
                        var storageNote =
                            Using<ICommodityStorageRepository>(c).GetById(DocumentId) as CommodityStorageNote;
                        foreach (var lineitem in storageNote.LineItems)
                        {
                            var lineItem = new CommodyLineItemViewModel
                                               {
                                                   Id = lineitem.Id,
                                                   GrossWeight = lineitem.Weight ,
                                                   NetWeight = lineitem.Weight,
                                                   //ContainerType = lineitem.ContainerType,
                                                   Commodity = lineitem.Commodity,
                                                   CommodityGrade = lineitem.CommodityGrade,
                                                   ContainerNo = lineitem.ContainerNo,
                                                   Description = lineitem.Description,

                                               };
                            LineItems.Add(lineItem);
                        }
                        DocumentReference = storageNote.DocumentReference;
                        break;
                    case DocumentType.CommodityDelivery:
                        var deliveryNote =
                            Using<ICommodityDeliveryRepository>(c).GetById(DocumentId) as CommodityDeliveryNote;
                        foreach (var lineitem in deliveryNote.LineItems)
                        {
                            var lineItem = new CommodyLineItemViewModel
                                               {
                                                   Id = lineitem.Id,
                                                   GrossWeight = lineitem.Weight + lineitem.ContainerType.TareWeight,
                                                   NetWeight = lineitem.Weight,
                                                   ContainerType = lineitem.ContainerType,
                                                   Commodity = lineitem.Commodity,
                                                   CommodityGrade = lineitem.CommodityGrade,
                                                   ContainerNo = lineitem.ContainerNo,
                                                   Description = lineitem.Description,

                                               };
                            LineItems.Add(lineItem);
                        }
                        DocumentReference = deliveryNote.DocumentReference;
                        break;
                    case DocumentType.ReceivedDelivery:
                        var receivedDelivery =
                            Using<IReceivedDeliveryRepository>(c).GetById(DocumentId) as ReceivedDeliveryNote;
                        foreach (var lineitem in receivedDelivery.LineItems)
                        {
                            var item = new CommodyLineItemViewModel
                                               {
                                                   Id = lineitem.Id,
                                                   GrossWeight = lineitem.Weight,
                                                   NetWeight = lineitem.Weight,
                                                   CommodityGrade = lineitem.CommodityGrade,
                                                   ContainerNo = lineitem.ContainerNo,
                                                   Description = lineitem.Description,
                                                   Commodity =lineitem.Commodity,
                                                    ContainerType = lineitem.ContainerType

                                               };
                            LineItems.Add(item);
                        }
                        DocumentReference = receivedDelivery.DocumentReference;
                        ShowTareWeight=Visibility.Collapsed;
                        break;
                }

            }
        }
   #endregion

        #region properites

        public ObservableCollection<CommodyLineItemViewModel> LineItems { get; set; }

        public RelayCommand PageLoadedCommand { get; set; }
        public RelayCommand MoveBackCommand { get; set; }


        public const string DocumentIdPropertyName = "DocumentId";
        private Guid _documentid;
        public Guid DocumentId
        {
            get
            {
                return _documentid;
            }

            set
            {
                if (_documentid == value)
                {
                    return;
                }

                RaisePropertyChanging(DocumentIdPropertyName);
                _documentid = value;
                RaisePropertyChanged(DocumentIdPropertyName);
            }
        }
        public const string NavigatedFromPropertyName = "NavigatedFrom";
        private string _navigated=string.Empty;
        public string NavigatedFrom
        {
            get
            {
                return _navigated;
            }

            set
            {
                if (_navigated == value)
                {
                    return;
                }

                RaisePropertyChanging(NavigatedFromPropertyName);
                _navigated = value;
                RaisePropertyChanged(NavigatedFromPropertyName);
            }
        }

        public const string DocumentTypePropertyName = "DocumentType";
        private DocumentType _documentype;
        public DocumentType DocumentType
        {
            get
            {
                return _documentype;
            }

            set
            {
                if (_documentype == value)
                {
                    return;
                }

                RaisePropertyChanging(DocumentTypePropertyName);
                _documentype = value;
                RaisePropertyChanged(DocumentTypePropertyName);
            }
        }

        public const string ShowTareWeightPropertyName = "ShowTareWeight";
        private Visibility _showTareWeight=Visibility.Collapsed;
        public Visibility ShowTareWeight
        {
            get
            {
                return _showTareWeight;
            }

            set
            {
                if (_showTareWeight == value)
                {
                    return;
                }

                RaisePropertyChanging(ShowTareWeightPropertyName);
                _showTareWeight = value;
                RaisePropertyChanged(ShowTareWeightPropertyName);
            }
        }
        public const string DocumentReferencePropertyName = "DocumentReference";
        private string _docRefNo = "";
        public string DocumentReference
        {
            get
            {
                return _docRefNo;
            }

            set
            {
                if (_docRefNo == value)
                {
                    return;
                }

                RaisePropertyChanging(DocumentReferencePropertyName);
                _docRefNo = value;
                RaisePropertyChanged(DocumentReferencePropertyName);
            }
        }
        #endregion

    }
}
