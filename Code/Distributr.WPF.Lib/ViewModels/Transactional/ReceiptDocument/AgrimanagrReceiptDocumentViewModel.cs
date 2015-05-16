using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Transactional.SourcingDocumentRepositories;
using Distributr.WPF.Lib.ViewModels.Transactional.CommodityPurchase;
using Distributr.WPF.Lib.ViewModels.Transactional.Orders;
using Distributr.WPF.Lib.ViewModels.Utils;
using GalaSoft.MvvmLight.Command;
using StructureMap;

namespace Distributr.WPF.Lib.ViewModels.Transactional.ReceiptDocument
{
    public class AgrimanagrReceiptDocumentViewModel : DistributrViewModelBase
    {
        public AgrimanagrReceiptDocumentViewModel()
        {
            ReceiptLineItemsList = new ObservableCollection<CommodyLineItemViewModel>();
            PrintCommand = new RelayCommand<Panel>(OnPrintCommand);

        }
        #region Properties
        public RelayCommand ReceiptDocumentLoadedCommand { get; set; }
        public ICommand PrintCommand { get; set; }
        public ObservableCollection<CommodyLineItemViewModel> ReceiptLineItemsList { get; set; }
        public const string TotalNetPropertyName = "TotalNet";
        private string _totalNet = "";
        public string TotalNet
        {
            get
            {
                return _totalNet;
            }

            set
            {
                if (_totalNet == value)
                {
                    return;
                }

                var oldValue = _totalNet;
                _totalNet = value;
                RaisePropertyChanged(TotalNetPropertyName);
            }
        }
        

        public const string TotalVatPropertyName = "TotalVat";
        private string _totalVat = "";
        public string TotalVat
        {
            get
            {
                return _totalVat;
            }

            set
            {
                if (_totalVat == value)
                {
                    return;
                }

                var oldValue = _totalVat;
                _totalVat = value;
                RaisePropertyChanged(TotalVatPropertyName);
            }
        }

        public const string TotalGrossPropertyName = "TotalGross";
        private string _totalGross = "";
        public string TotalGross
        {
            get
            {
                return _totalGross;
            }

            set
            {
                if (_totalGross == value)
                {
                    return;
                }

                var oldValue = _totalGross;
                _totalGross = value;
                RaisePropertyChanged(TotalGrossPropertyName);
            }
        }

        public const string DocumentTitlePropertyName = "DocumentTitle";
        private string _documentTitle = "";
        public string DocumentTitle
        {
            get
            {
                return _documentTitle;
            }

            set
            {
                if (_documentTitle == value)
                {
                    return;
                }

                var oldValue = _documentTitle;
                _documentTitle = value;
                RaisePropertyChanged(DocumentTitlePropertyName);
            }
        }

        public const string ReceiptNoPropertyName = "ReceiptNo";
        private string _receiptNo = "";
        public string ReceiptNo
        {
            get
            {
                return _receiptNo;
            }

            set
            {
                if (_receiptNo == value)
                {
                    return;
                }

                var oldValue = _receiptNo;
                _receiptNo = value;
                RaisePropertyChanged(ReceiptNoPropertyName);
            }
        }

        public const string ReceiptDatePropertyName = "ReceiptDate";
        private string _receiptDate = DateTime.Now.ToString("dd-MMM-yyyy");
        public string ReceiptDate
        {
            get
            {
                return _receiptDate;
            }

            set
            {
                if (_receiptDate == value)
                {
                    return;
                }

                var oldValue = _receiptDate;
                _receiptDate = value;
                RaisePropertyChanged(ReceiptDatePropertyName);
            }
        }

        public const string CreditTermsPropertyName = "CreditTerms";
        private string _creditTerms = "";
        public string CreditTerms
        {
            get
            {
                return _creditTerms;
            }

            set
            {
                if (_creditTerms == value)
                {
                    return;
                }

                var oldValue = _creditTerms;
                _creditTerms = value;
                RaisePropertyChanged(CreditTermsPropertyName);
            }
        }

        public const string ChequePayableToPropertyName = "ChequePayableTo";
        private string _chequePayableTo = "";
        public string ChequePayableTo
        {
            get
            {
                return _chequePayableTo;
            }

            set
            {
                if (_chequePayableTo == value)
                {
                    return;
                }

                var oldValue = _chequePayableTo;
                _chequePayableTo = value;
                RaisePropertyChanged(ChequePayableToPropertyName);
            }
        }

        public const string ServedByUserNamePropertyName = "ServedByUserName";
        private string _servedByUserName = "";
        public string ServedByUserName
        {
            get
            {
                return _servedByUserName;
            }

            set
            {
                if (_servedByUserName == value)
                {
                    return;
                }

                var oldValue = _servedByUserName;
                _servedByUserName = value;
                RaisePropertyChanged(ServedByUserNamePropertyName);
            }
        }

        public const string PreparedByJobTitlePropertyName = "PreparedByJobTitle";
        private string _preparedByJobTitle = "";
        public string PreparedByJobTitle
        {
            get
            {
                return _preparedByJobTitle;
            }

            set
            {
                if (_preparedByJobTitle == value)
                {
                    return;
                }

                var oldValue = _preparedByJobTitle;
                _preparedByJobTitle = value;
                RaisePropertyChanged(PreparedByJobTitlePropertyName);
            }
        }

        public const string DatePreparedPropertyName = "DatePrepared";
        private DateTime _datePrepared = DateTime.Now;
        public DateTime DatePrepared
        {
            get
            {
                return _datePrepared;
            }

            set
            {
                if (_datePrepared == value)
                {
                    return;
                }

                var oldValue = _datePrepared;
                _datePrepared = value;
                RaisePropertyChanged(DatePreparedPropertyName);
            }
        }

        public const string ApprovedByUserNamePropertyName = "ApprovedByUserName";
        private string _approvedByUserName = "";
        public string ApprovedByUserName
        {
            get
            {
                return _approvedByUserName;
            }

            set
            {
                if (_approvedByUserName == value)
                {
                    return;
                }

                var oldValue = _approvedByUserName;
                _approvedByUserName = value;
                RaisePropertyChanged(ApprovedByUserNamePropertyName);
            }
        }

        public const string DateApprovedPropertyName = "DateApproved";
        private DateTime _dateApproved = DateTime.Now;
        public DateTime DateApproved
        {
            get
            {
                return _dateApproved;
            }

            set
            {
                if (_dateApproved == value)
                {
                    return;
                }

                var oldValue = _dateApproved;
                _dateApproved = value;
                RaisePropertyChanged(DateApprovedPropertyName);
            }
        }


        //company Info
        public const string CompanyNamePropertyName = "CompanyName";
        private string _companyName = "";
        public string CompanyName
        {
            get
            {
                return _companyName;
            }

            set
            {
                if (_companyName == value)
                {
                    return;
                }

                var oldValue = _companyName;
                _companyName = value;
                RaisePropertyChanged(CompanyNamePropertyName);
            }
        }

        public const string HubNamePropertyName = "HubName";

        private string _hubName = string.Empty;

        public string HubName
        {
            get
            {
                return _hubName;
            }

            set
            {
                if (_hubName == value)
                {
                    return;
                }

                RaisePropertyChanging(HubNamePropertyName);
                _hubName = value;
                RaisePropertyChanged(HubNamePropertyName);
            }
        }

        public const string AddressPropertyName = "Address";
        private string _address = "";
        public string Address
        {
            get
            {
                return _address;
            }

            set
            {
                if (_address == value)
                {
                    return;
                }

                var oldValue = _address;
                _address = value;
                RaisePropertyChanged(AddressPropertyName);
            }
        }

        public const string PhysicalAddressPropertyName = "PhysicalAddress";
        private string _physicalAddress = "";
        public string PhysicalAddress
        {
            get
            {
                return _physicalAddress;
            }

            set
            {
                if (_physicalAddress == value)
                {
                    return;
                }

                var oldValue = _physicalAddress;
                _physicalAddress = value;
                RaisePropertyChanged(PhysicalAddressPropertyName);
            }
        }

        public const string TelNoPropertyName = "TelNo";
        private string _telNo = "";
        public string TelNo
        {
            get
            {
                return _telNo;
            }

            set
            {
                if (_telNo == value)
                {
                    return;
                }

                var oldValue = _telNo;
                _telNo = value;
                RaisePropertyChanged(TelNoPropertyName);
            }
        }

        public const string FaxNoPropertyName = "FaxNo";
        private string _faxNo = "";
        public string FaxNo
        {
            get
            {
                return _faxNo;
            }

            set
            {
                if (_faxNo == value)
                {
                    return;
                }

                var oldValue = _faxNo;
                _faxNo = value;
                RaisePropertyChanged(FaxNoPropertyName);
            }
        }

        public const string CellPropertyName = "Cell";
        private string _cell = "";
        public string Cell
        {
            get
            {
                return _cell;
            }

            set
            {
                if (_cell == value)
                {
                    return;
                }

                var oldValue = _cell;
                _cell = value;
                RaisePropertyChanged(CellPropertyName);
            }
        }

        public const string VatNoPropertyName = "VatNo";
        private string _vatNo = "";
        public string VatNo
        {
            get
            {
                return _vatNo;
            }

            set
            {
                if (_vatNo == value)
                {
                    return;
                }

                var oldValue = _vatNo;
                _vatNo = value;
                RaisePropertyChanged(VatNoPropertyName);
            }
        }

        public const string PinNoPropertyName = "PinNo";
        private string _pinNo = "";
        public string PinNo
        {
            get
            {
                return _pinNo;
            }

            set
            {
                if (_pinNo == value)
                {
                    return;
                }

                var oldValue = _pinNo;
                _pinNo = value;
                RaisePropertyChanged(PinNoPropertyName);
            }
        }

        public const string FarmerEmailPropertyName = "FarmerEmail";
        private string _farmeremail = "";
        public string FarmerEmail
        {
            get
            {
                return _farmeremail;
            }

            set
            {
                if (_farmeremail == value)
                {
                    return;
                }

               _farmeremail = value;
                RaisePropertyChanged(FarmerEmailPropertyName);
            }
        }
        public const string CompanyEmailPropertyName = "CompanyEmail";
        private string _companyemail = "";
        public string CompanyEmail
        {
            get
            {
                return _companyemail;
            }

            set
            {
                if (_companyemail == value)
                {
                    return;
                }
                _companyemail = value;
                RaisePropertyChanged(CompanyEmailPropertyName);
            }
        }

        public const string WebSitePropertyName = "WebSite";
        private string _webSite = "";
        public string WebSite
        {
            get
            {
                return _webSite;
            }

            set
            {
                if (_webSite == value)
                {
                    return;
                }

                var oldValue = _webSite;
                _webSite = value;
                RaisePropertyChanged(WebSitePropertyName);
            }
        }

      
        public const string ReceiptRecipientCompanyNamePropertyName = "ReceiptRecipientCompanyName";
        private string _receiptRecipientCompanyName = "";
        public string ReceiptRecipientCompanyName
        {
            get
            {
                return _receiptRecipientCompanyName;
            }

            set
            {
                if (_receiptRecipientCompanyName == value)
                {
                    return;
                }

             _receiptRecipientCompanyName = value;
                RaisePropertyChanged(ReceiptRecipientCompanyNamePropertyName);
            }
        }


        public const string PurchaseNoteIdPropertyName = "PurchaseNoteId";
       private Guid _purchaseNoteId = Guid.Empty;
       public Guid PurchaseNoteId
        {
            get
            {
                return _purchaseNoteId;
            }

            set
            {
                if (_purchaseNoteId == value)
                {
                    return;
                }
                _purchaseNoteId = value;
                RaisePropertyChanged(PurchaseNoteIdPropertyName);
            }
        }
    
        public const string FamerNoPropertyName = "FamerNo";
        private string _farmerNo = "";
        public string FamerNo
        {
            get
            {
                return _farmerNo;
            }

            set
            {
                if (_farmerNo == value)
                {
                    return;
                }

                var oldValue = _farmerNo;
                _farmerNo = value;


                // Update bindings, no broadcast
                RaisePropertyChanged(FamerNoPropertyName);

            }
        }

        public const string DeliveredByPropertyName = "DeliveredBy";
        private string _deliveredBy = "";
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

                var oldValue = _deliveredBy;
                _deliveredBy = value;

                // Remove one of the two calls below
                RaisePropertyChanged(DeliveredByPropertyName);

            }
        }

        public const string FarmerNamePropertyName = "FarmerName";
        private string _farmerName = "";
        public string FarmerName
        {
            get
            {
                return _farmerName;
            }

            set
            {
                if (_farmerName == value)
                {
                    return;
                }

                var oldValue = _farmerName;
                _farmerName = value;



                // Update bindings, no broadcast
                RaisePropertyChanged(FarmerNamePropertyName);

            }
        }

        public const string FarmerIdPropertyName = "FarmerId";
        private Guid _farmerId = Guid.Empty;
        public Guid FarmerId
        {
            get
            {
                return _farmerId;
            }

            set
            {
                if (_farmerId == value)
                {
                    return;
                }

                var oldValue = _farmerId;
                _farmerId = value;


                // Update bindings, no broadcast
                RaisePropertyChanged(FarmerIdPropertyName);

            }
        }


        public const string TotalWeightPropertyName = "TotalWeight";
        private decimal _totalWeight = 0;
        public decimal TotalWeight
        {
            get
            {
                return _totalWeight;
            }

            set
            {
                if (_totalWeight == value)
                {
                    return;
                }

                var oldValue = _totalWeight;
                _totalWeight = value;
                RaisePropertyChanged(TotalWeightPropertyName);
            }
        }
        #endregion
        #region Methods
        public void SetId(ViewModelMessage obj)
        {
            PurchaseNoteId = obj.Id;

        }
        public void OnPrintCommand(Panel p)
        {
           var printer = new SerialPortUtil();
            PrintDocument pd = new PrintDocument();
            pd.PrintPage += (s, e) =>
            {
                //p.Width = e.PrintableArea.Width;
                //p.Height = e.PrintableArea.Height;
                p.HorizontalAlignment = HorizontalAlignment.Center;
                //e.PageVisual = p;
                e.HasMorePages = false;
            };
            //printer.Write(Guid.Empty, null);

            pd.Print();

        }
        public void LoadReceipt(CommodityPurchaseNote purchaseNote)
        {
            if (purchaseNote !=null)
            {
                ReceiptLineItemsList.Clear();
                Reset(); 
                
                using (IContainer cont = NestedContainer)
                {
                    var _costCentreRepo = Using<ICostCentreRepository>(cont);
                    var costCentre = _costCentreRepo.GetById(purchaseNote.DocumentIssuerCostCentre.ParentCostCentre.Id);
                    CompanyName = costCentre.Name;
                }

                if (purchaseNote.DocumentIssuerCostCentre != null)
                {
                    HubName = purchaseNote.DocumentIssuerCostCentre.Name;
                }
                if (purchaseNote.CommodityOwner != null)
                {

                    FarmerName = purchaseNote.CommodityOwner.FullName;
                    FamerNo = purchaseNote.CommodityOwner.Code;

                    TelNo = purchaseNote.CommodityOwner.PhoneNumber;
                    FaxNo = purchaseNote.CommodityOwner.FaxNumber;
                    FarmerEmail = purchaseNote.CommodityOwner.Email;
                }

                TotalWeight = purchaseNote.LineItems.Sum(n => n.Weight);

                DeliveredBy = purchaseNote.DeliveredBy;

                ServedByUserName = purchaseNote.DocumentIssuerUser.Username;

                ReceiptNo = purchaseNote.DocumentReference;
                try
                {

                    using (var c = NestedContainer)
                    {
                        var contact =
                            Using<IContactRepository>(c).GetByContactsOwnerId(purchaseNote.DocumentIssuerCostCentre.Id);

                        if(contact !=null)
                        {
                            if(contact.Any())
                                CompanyEmail = contact.FirstOrDefault().Email;
                        }
                       
                    }
                }catch
                {
                }
               

                foreach (var item in purchaseNote.LineItems)
                {
                    var commodityItem = new CommodyLineItemViewModel()
                    {
                        Id = item.Id,
                        Commodity = item.Commodity,
                        CommodityGrade = item.CommodityGrade,
                        ContainerNo = item.ContainerNo,
                        ContainerType = item.ContainerType,
                        Description = item.Description,
                        GrossWeight = (item.Weight+item.TareWeight+item.ContainerType.BubbleSpace),
                        NetWeight = item.Weight,
                        BubbleSpace =item.ContainerType.BubbleSpace,
                        TareWeight =item.TareWeight
                        
                    };
                    ReceiptLineItemsList.Add(commodityItem);

                }
                

            }
        }

        void Reset()
        {
            CompanyName = string.Empty;
            FarmerName = string.Empty;
            FamerNo = string.Empty;
            TotalWeight = 0;
            DeliveredBy = string.Empty;
            TelNo = string.Empty;
            FaxNo = string.Empty;
            CompanyEmail = string.Empty;
            FarmerEmail = string.Empty;
        }
        #endregion

    }
}
