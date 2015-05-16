using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Domain.Master.SettingsEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Services.Service.Utility;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Linq;

namespace Distributr.WPF.Lib.ViewModels.Sync
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm/getstarted
    /// </para>
    /// </summary>
    public class ArchiveViewModel : DistributrViewModelBase
    {

        public ArchiveViewModel()
        {
            ArchiveItem = new ObservableCollection<ArchiveItemViewModel>();
            ArchiveCommand = new RelayCommand(Archive);
            CheckCommand = new RelayCommand(Check);
            UnCheckCommand = new RelayCommand(UnCheck);
           // ArchiveOthersCommand = new RelayCommand(ArchiveOthers);
         
        }

       

        private void Check()
        {
           foreach(var item in ArchiveItem)
           {
               item.IsArchive = true;
           }
        }

        private void UnCheck()
        {
            foreach (var item in ArchiveItem)
            {
                item.IsArchive = false;
            }
        }

        public RelayCommand ArchiveCommand { get; set; }
        public RelayCommand CheckCommand { get; set; }
        public RelayCommand UnCheckCommand { get; set; }
        public RelayCommand ArchiveOthersCommand { get; set; }
        
        public ObservableCollection<ArchiveItemViewModel> ArchiveItem { get; set; }
        private void Archive()
        {
            using (StructureMap.IContainer c1 = NestedContainer)
            {
                if (ArchiveItem.Count == 0)
                {
                    MessageBox.Show("No Ducuments to Archive", "Document Archiving ", MessageBoxButton.OKCancel);
                    return;
                }
                if (!ArchiveItem.Any(a => a.IsArchive))
                {
                    MessageBox.Show("Select Document to Archive", "Document Archiving ", MessageBoxButton.OKCancel);
                    return;
                }
                Config con = Using<IConfigService>(c1).Load();
                Guid userid = Using<IConfigService>(c1).ViewModelParameters.CurrentUserId;
                MessageBoxResult isConfirmed = MessageBox.Show("Are sure you want to archive these Document",
                                                               "Document Archiving ", MessageBoxButton.OKCancel);
                if (isConfirmed == MessageBoxResult.OK)
                {
                    foreach (var item in ArchiveItem.Where(s => s.IsArchive))
                    {
                        RetireDocumentCommand cmd = new RetireDocumentCommand(Guid.NewGuid(), item.OrderId, userid,
                                                                              item.IssuerCostCentreId, 0,
                                                                              con.CostCentreApplicationId,
                                                                              Guid.NewGuid(),
                                                                              item.RecipientCostCentreId, null, null);
                       Using<IRetireDocumentWFManager>(c1).Submit(cmd, (DocumentType) item.DocumentType);

                    }

                }
                MessageBox.Show("Document Archived successfully", "Order Archiving ", MessageBoxButton.OKCancel);
                LoadItemToArchive();
                
            }
        }
        public void LoadItemToArchive()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                ArchiveItem.Clear();
                RetireDocumentSetting setting = Using<IRetireDocumentWFManager>(c).GetSetting();
                if (setting != null)
                {
                    if (setting.RetireType == RetireType.Delivered)
                    {
                        GetDeliverdOrderItem(setting.Duration);
                    }
                    else if (setting.RetireType == RetireType.Paid)
                    {
                        GetPaidOrderItem(setting.Duration);
                    }
                    GetClosedReturns(setting.Duration);
                }
            }

        }

        private void GetClosedReturns(int duration)
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                List<ReturnsNote> returns = Using<IRetireDocumentWFManager>(c).GetClosedReturns(duration);
                returns.ForEach(n => ArchiveItem.Add(MapReturns(n, RetireType.Delivered, DocumentType.ReturnsNote)));
            }
        }

        private ArchiveItemViewModel MapReturns(ReturnsNote returnsNote,RetireType type, DocumentType documentType)
        {
            if (returnsNote != null)
            {
                ArchiveItemViewModel item = new ArchiveItemViewModel();
                item.ArchiveMode = type.ToString();
                item.IsArchive = false;
                item.OrderId = returnsNote.DocumentParentId;
                item.OrderReference = returnsNote.DocumentReference;
                item.RecipientCostCentreId = returnsNote.DocumentRecipientCostCentre.Id;
                item.IssuerCostCentreId = returnsNote.DocumentIssuerCostCentre.Id;
                item.DocumentType = documentType;
                //item.OutletName = order.DocumentIssuerCostCentre.Name;
                return item;

            }
            return null;
        }

        private void GetDeliverdOrderItem(int duration)
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                ArchiveItem.Clear();
                List<Order> orders = Using<IRetireDocumentWFManager>(c).GetDeliveredOrders(duration);
                orders.ForEach(n => ArchiveItem.Add(Map(n, RetireType.Delivered, DocumentType.Order)));
            }
        }

        private ArchiveItemViewModel Map(Order order,RetireType type,DocumentType documentType)
        {
            if (order != null)
            {
                ArchiveItemViewModel item = new ArchiveItemViewModel();
                item.ArchiveMode = type.ToString();
                item.IsArchive = false;
                item.OrderId = order.Id;
                item.OrderReference = order.DocumentReference;
                item.RecipientCostCentreId = order.DocumentRecipientCostCentre.Id;
                item.IssuerCostCentreId = order.DocumentIssuerCostCentre.Id;
                item.DocumentType = documentType;
                
                //item.OutletName = order.DocumentIssuerCostCentre.Name;
                return item;

            }
            return null;
        }

        private void GetPaidOrderItem(int duration)
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                ArchiveItem.Clear();
                List<Order> orders = Using<IRetireDocumentWFManager>(c).GetFullPaidOrder(duration);
                orders.ForEach(n => ArchiveItem.Add(Map(n, RetireType.Paid, DocumentType.Order)));
            }
        }

        

    }
    public class ArchiveItemViewModel : ViewModelBase
    {
       
        public const string OrderReferencePropertyName = "OrderReference";
        private string _orderRef = "";
        public string OrderReference
        {
            get
            {
                return _orderRef;
            }

            set
            {
                if (_orderRef == value)
                {
                    return;
                }

                var oldValue = _orderRef;
                _orderRef = value;
                // Update bindings, no broadcast
                RaisePropertyChanged(OrderReferencePropertyName);

            }
        }
       
        public const string OrderIdPropertyName = "OrderId";
        private Guid _orderId = Guid.Empty;
        public Guid OrderId
        {
            get
            {
                return _orderId;
            }

            set
            {
                if (_orderId == value)
                {
                    return;
                }

                var oldValue = _orderId;
                _orderId = value;

               

                // Update bindings, no broadcast
                RaisePropertyChanged(OrderIdPropertyName);

                
            }
        }
       
        public const string IsArchivePropertyName = "IsArchive";
        private bool _IsArchive = false;
        public bool IsArchive
        {
            get
            {
                return _IsArchive;
            }

            set
            {
               
                if (_IsArchive == value)
                {
                    return;
                }

                var oldValue = _IsArchive;
                _IsArchive = value;


                // Update bindings, no broadcast
                RaisePropertyChanged(IsArchivePropertyName);

                
            }
        }

        
        public const string ArchiveModePropertyName = "ArchiveMode";
        private string _ArchiveMode = "";
        public string ArchiveMode
        {
            get
            {
                return _ArchiveMode;
            }

            set
            {
                if (_ArchiveMode == value)
                {
                    return;
                }

                var oldValue = _ArchiveMode;
                _ArchiveMode = value;
                // Update bindings, no broadcast
                RaisePropertyChanged(ArchiveModePropertyName);
               
            }
        }


      
        public const string DocumentIssuerPropertyName = "DocumentIssuer";
        private Guid _documentIssuer = Guid.Empty;
        public Guid DocumentIssuer
        {
            get
            {
                return _documentIssuer;
            }

            set
            {
                if (_documentIssuer == value)
                {
                    return;
                }

                var oldValue = _documentIssuer;
                _documentIssuer = value;

               

                // Update bindings, no broadcast
                RaisePropertyChanged(DocumentIssuerPropertyName);

               
            }
        }


        
        public const string RecipientCostCentreIdPropertyName = "RecipientCostCentreId";
        private Guid _RecipientCostCentreId = Guid.Empty;
        public Guid RecipientCostCentreId
        {
            get
            {
                return _RecipientCostCentreId;
            }

            set
            {
                if (_RecipientCostCentreId == value)
                {
                    return;
                }

                var oldValue = _RecipientCostCentreId;
                _RecipientCostCentreId = value;

               

                // Update bindings, no broadcast
                RaisePropertyChanged(RecipientCostCentreIdPropertyName);

               
            }
        }

       
        public const string IssuerCostCentreIdPropertyName = "IssuerCostCentreId";
        private Guid _IssuerCostCentreId = Guid.Empty;
        public Guid IssuerCostCentreId
        {
            get
            {
                return _IssuerCostCentreId;
            }

            set
            {
                if (_IssuerCostCentreId == value)
                {
                    return;
                }

                var oldValue = _IssuerCostCentreId;
                _IssuerCostCentreId = value;
                // Update bindings, no broadcast
                RaisePropertyChanged(IssuerCostCentreIdPropertyName);

            }
        }


        /// <summary>
        /// The <see cref="DocumentType" /> property's name.
        /// </summary>
        public const string DocumentTypePropertyName = "DocumentType";

        private DocumentType _documentType = 0;

        /// <summary>
        /// Gets the DocumentType property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public DocumentType DocumentType
        {
            get
            {
                return _documentType;
            }

            set
            {
                if (_documentType == value)
                {
                    return;
                }

                var oldValue = _documentType;
                _documentType = value;

             

                // Update bindings, no broadcast
                RaisePropertyChanged(DocumentTypePropertyName);

              
            }
        }

       
       
    }
}