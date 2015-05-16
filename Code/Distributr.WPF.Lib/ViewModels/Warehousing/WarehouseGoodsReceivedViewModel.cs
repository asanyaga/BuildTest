using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;
using Distributr.Core.Repository.Master.CommodityOwnerRepository;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Transactional.SourcingDocumentRepositories;
using Distributr.WPF.Lib.Messages;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using StructureMap;

namespace Distributr.WPF.Lib.ViewModels.Warehousing
{
    public class WarehouseGoodsReceivedViewModel : DistributrViewModelBase
    {
        public WarehouseGoodsReceivedViewModel()
        {
            CommodityLineItemsList = new ObservableCollection<CommodityLineItem>();
            PrintCommand=new RelayCommand<Panel>(PrintNote);
            BackCommand = new RelayCommand(Back);
        }


        public RelayCommand<Panel> PrintCommand { get; set; }
        public RelayCommand BackCommand { get; set; }
        public ObservableCollection<CommodityLineItem> CommodityLineItemsList { get; set; }


        public void SetDocumentId(WarehouseEntryUpdateMessage messageFrom)
        {
            DocumentId = messageFrom.DocumentId;

            LoadDetails();

        }

        private void LoadDetails()
         {
           // ClearViewModel();
            using (var c = ObjectFactory.Container.GetNestedContainer())
            {
                var document = c.GetInstance<ICommodityWarehouseStorageRepository>().GetById(DocumentId) as CommodityWarehouseStorageNote;
                if (document != null)
                {
                    var farmerId = document.CommodityOwnerId;
                    var commodityOwner = c.GetInstance<ICommodityOwnerRepository>().GetById(farmerId) ;
                    var commodityOwnerName = commodityOwner != null ? commodityOwner.FullName : "";
                    var commoditySupplier = document.DocumentRecipientCostCentre as CommoditySupplier;
                    string commoditySupplierName = string.Empty;

                    if(commoditySupplier.CommoditySupplierType==CommoditySupplierType.Cooperative)
                    {
                        commoditySupplierName = commoditySupplier.Name;
                    }


                    ReceivedBy = document.DocumentIssuerCostCentre.Name.ToString();
                    ReceivedByUser = document.DocumentIssuerUser.Username.ToString();
                    //ReceivedFromAccount = commoditySupplierName != string.Empty
                    //                   ? string.Format(commoditySupplierName + "(" + commodityOwnerName + ")")
                    //                   : commodityOwnerName;

                    ReceivedFromAccount = commoditySupplierName != string.Empty
                                       ? commoditySupplierName 
                                       : commodityOwnerName;

                    ReceivedFrom = commodityOwnerName;

                    var contactDetailsForReceivedBy =c.GetInstance<IContactRepository>().GetByContactOwnerId(document.DocumentIssuerCostCentre.Id);

                    ReceivedByAddress = contactDetailsForReceivedBy != null ? contactDetailsForReceivedBy.PostalAddress : "N/A";
                    ReceivedByLocation = contactDetailsForReceivedBy != null?contactDetailsForReceivedBy.PhysicalAddress:"N/A";
                    ReceivedByMobile = contactDetailsForReceivedBy != null?contactDetailsForReceivedBy.MobilePhone:"N/A";
                    ReceivedByEmail =contactDetailsForReceivedBy != null? contactDetailsForReceivedBy.Email:"N/A";

                    DeliveredBy = document.DriverName != string.Empty ? document.DriverName : "N/A";
                    VehicleRegistration = document.VehiclRegNo != string.Empty ? document.VehiclRegNo : "N/A";
                    
                    //var contactDetailsForReceivedFrom =c.GetInstance<IContactRepository>().GetByContactOwnerId(document.DocumentRecipientCostCentre.Id);
                     
                    //ReceivedFromAddress = contactDetailsForReceivedFrom != null?contactDetailsForReceivedFrom.PostalAddress:"";
                    //ReceivedFromMobile = contactDetailsForReceivedFrom != null?contactDetailsForReceivedFrom.MobilePhone:"";
                    //ReceivedFromEmail =contactDetailsForReceivedFrom!=null? contactDetailsForReceivedFrom.Email:"";

                    ReceivedFromAddress = commodityOwner != null ? commodityOwner.PostalAddress : "N/A";
                    ReceivedFromMobile = commodityOwner != null ? commodityOwner.PhoneNumber : "N/A";
                    ReceivedFromEmail = commodityOwner != null ? commodityOwner.Email : "N/A";
                    ReceivedFromId = commodityOwner != null ? commodityOwner.IdNo : "N/A";

                    var item = document.LineItems.FirstOrDefault();
                    CommodityLineItemsList.Clear();
                    if (item != null)
                    {
                        CommodityLineItemsList.Add( new CommodityLineItem()
                            {
                                CommodityGrade = item.CommodityGrade.Name,
                                CommodityName = item.Commodity.Name,
                                CommodityQuantity = (item.Weight-item.FinalWeight).ToString()
                            });
                    }
                        
                }
            }
        }

        private void PrintNote(Panel p)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintVisual(p, "Commodity Receive Note");
               
            }
            
        }

        private void Back()
        {
            SendNavigationRequestMessage(new Uri("/Views/Warehousing/WarehouseExitListingPage.xaml", UriKind.Relative));
        }

        public const string ReceivedByPropertyName = "ReceivedBy";
        private string _receivedBy = string.Empty;
        public string ReceivedBy
        {
            get
            {
                return _receivedBy;
            }

            set
            {
                if (_receivedBy == value)
                {
                    return;
                }

                RaisePropertyChanging(ReceivedByPropertyName);
                _receivedBy = value;
                RaisePropertyChanged(ReceivedByPropertyName);
            }
        }

        public const string ReceivedByUserPropertyName = "ReceivedByUser";
        private string _receivedByUser = string.Empty;
        public string ReceivedByUser
        {
            get
            {
                return _receivedByUser;
            }

            set
            {
                if (_receivedByUser == value)
                {
                    return;
                }

                RaisePropertyChanging(ReceivedByUserPropertyName);
                _receivedByUser = value;
                RaisePropertyChanged(ReceivedByUserPropertyName);
            }
        }

        public const string ReceivedFromPropertyName = "ReceivedFrom";
        private string _receivedFrom = string.Empty;
        public string ReceivedFrom
        {
            get
            {
                return _receivedFrom;
            }

            set
            {
                if (_receivedFrom == value)
                {
                    return;
                }

                RaisePropertyChanging(ReceivedFromPropertyName);
                _receivedFrom = value;
                RaisePropertyChanged(ReceivedFromPropertyName);
            }
        }

        public const string ReceivedFromAccountPropertyName = "ReceivedFromAccount";
        private string _receivedFromAccount = string.Empty;
        public string ReceivedFromAccount
        {
            get
            {
                return _receivedFromAccount;
            }

            set
            {
                if (_receivedFromAccount == value)
                {
                    return;
                }

                RaisePropertyChanging(ReceivedFromAccountPropertyName);
                _receivedFromAccount = value;
                RaisePropertyChanged(ReceivedFromAccountPropertyName);
            }
        }

        public const string ReceivedFromAddressPropertyName = "ReceivedFromAddress";
        private string _receivedFromAddress = string.Empty;
        public string ReceivedFromAddress
        {
            get
            {
                return _receivedFromAddress;
            }

            set
            {
                if (_receivedFromAddress == value)
                {
                    return;
                }

                RaisePropertyChanging(ReceivedFromAddressPropertyName);
                _receivedFromAddress = value;
                RaisePropertyChanged(ReceivedFromAddressPropertyName);
            }
        }

        public const string ReceivedFromMobilePropertyName = "ReceivedFromMobile";
        private string _receivedFromMobile = string.Empty;
        public string ReceivedFromMobile
        {
            get
            {
                return _receivedFromMobile;
            }

            set
            {
                if (_receivedFromMobile == value)
                {
                    return;
                }

                RaisePropertyChanging(ReceivedFromMobilePropertyName);
                _receivedFromMobile = value;
                RaisePropertyChanged(ReceivedFromMobilePropertyName);
            }
        }

        public const string ReceivedByMobilePropertyName = "ReceivedByMobile";
        private string _receivedByMobile = string.Empty;
        public string ReceivedByMobile
        {
            get
            {
                return _receivedByMobile;
            }

            set
            {
                if (_receivedByMobile == value)
                {
                    return;
                }

                RaisePropertyChanging(ReceivedByMobilePropertyName);
                _receivedByMobile = value;
                RaisePropertyChanged(ReceivedByMobilePropertyName);
            }
        }

        public const string ReceivedFromEmailPropertyName = "ReceivedFromEmail";
        private string _receivedFromEmail = string.Empty;
        public string ReceivedFromEmail
        {
            get
            {
                return _receivedFromEmail;
            }

            set
            {
                if (_receivedFromEmail == value)
                {
                    return;
                }

                RaisePropertyChanging(ReceivedFromEmailPropertyName);
                _receivedFromEmail = value;
                RaisePropertyChanged(ReceivedFromEmailPropertyName);
            }
        }

        public const string ReceivedFromIdPropertyName = "ReceivedFromId";
        private string _receivedFromId = string.Empty;
        public string ReceivedFromId
        {
            get
            {
                return _receivedFromId;
            }

            set
            {
                if (_receivedFromId == value)
                {
                    return;
                }

                RaisePropertyChanging(ReceivedFromIdPropertyName);
                _receivedFromId = value;
                RaisePropertyChanged(ReceivedFromIdPropertyName);
            }
        }

        public const string ReceivedByLocationPropertyName = "ReceivedByLocation";
        private string _receivedByLocation = string.Empty;
        public string ReceivedByLocation
        {
            get
            {
                return _receivedByLocation;
            }

            set
            {
                if (_receivedByLocation == value)
                {
                    return;
                }

                RaisePropertyChanging(ReceivedByLocationPropertyName);
                _receivedByLocation = value;
                RaisePropertyChanged(ReceivedByLocationPropertyName);
            }
        }

        public const string ReceivedByEmailPropertyName = "ReceivedByEmail";
        private string _receivedByEmail = string.Empty;
        public string ReceivedByEmail
        {
            get
            {
                return _receivedByEmail;
            }

            set
            {
                if (_receivedByEmail == value)
                {
                    return;
                }

                RaisePropertyChanging(ReceivedByEmailPropertyName);
                _receivedByEmail = value;
                RaisePropertyChanged(ReceivedByEmailPropertyName);
            }
        }

        public const string DeliveredByPropertyName = "DeliveredBy";
        private string _deliveredBy = string.Empty;
        public string DeliveredBy
        {
            get
            {
                return _deliveredBy;
            }

            set
            {
                if (_deliveredBy == value)
                {
                    return;
                }

                RaisePropertyChanging(DeliveredByPropertyName);
                _deliveredBy = value;
                RaisePropertyChanged(DeliveredByPropertyName);
            }
        }

        public const string VehicleRegistrationPropertyName = "VehicleRegistration";
        private string _vehicleRegistration = string.Empty;
        public string VehicleRegistration
        {
            get
            {
                return _vehicleRegistration;
            }

            set
            {
                if (_vehicleRegistration == value)
                {
                    return;
                }

                RaisePropertyChanging(VehicleRegistrationPropertyName);
                _vehicleRegistration = value;
                RaisePropertyChanged(VehicleRegistrationPropertyName);
            }
        }

        public const string ReceivedByAddressPropertyName = "ReceivedByAddress";
        private string _receivedByAddress = string.Empty;
        public string ReceivedByAddress
        {
            get
            {
                return _receivedByAddress;
            }

            set
            {
                if (_receivedByAddress == value)
                {
                    return;
                }

                RaisePropertyChanging(ReceivedByAddressPropertyName);
                _receivedByAddress = value;
                RaisePropertyChanged(ReceivedByAddressPropertyName);
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

       
    }

    public class CommodityLineItem
    {
        public string CommodityGrade { get; set; }
        public string CommodityName { get; set; }
        public string CommodityQuantity { get; set; }
    }
}
