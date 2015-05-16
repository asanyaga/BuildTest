using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IInvoiceRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.ReceiptInventories;
using Distributr.Core.Utility;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.ViewModels.Transactional.Orders;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Transactional.ReceiptDocument
{
    public class ReceiptVm : DistributrViewModelBase
    {
        public ReceiptVm()
        {
            ReceiptLineItemsList=new ObservableCollection<ReceiptLineItemVm>();
        }

        #region Methods

        
        public void SetId(ViewModelMessage obj)
        {
            ReceiptId = obj.Id;

        }
        public void Load()
        {
          ReceiptLabel="OFFICIAL RECEIPT";
            using (StructureMap.IContainer c = NestedContainer)
            {
                var receipt = Using<IReceiptRepository>(c).GetById(ReceiptId);
                var costcentreRepository = Using<ICostCentreRepository>(c);
                Invoice = Using<IInvoiceRepository>(c).GetById(receipt.InvoiceId);
               
                CostCentre cc = costcentreRepository.GetById(receipt.DocumentIssuerCostCentre.Id);
                CostCentre cc2 = costcentreRepository.GetById(receipt.DocumentRecipientCostCentre.Id);
                Distributor distributr = null;
                if (cc is Distributor)
                {
                    distributr = cc as Distributor;
                }
                if (cc2 is Distributor)
                {
                    distributr = cc2 as Distributor;
                }

               
                Contact contact = null;
                if (distributr.Contact.Any())
                {
                    contact = distributr.Contact.FirstOrDefault(
                        n => n.ContactClassification == ContactClassification.PrimaryContact) ??
                              distributr.Contact.FirstOrDefault();
                }
                if (contact == null)
                {
                    var contacts = Using<IContactRepository>(c).GetByContactsOwnerId(distributr.Id);
                    contact = contacts.FirstOrDefault(
                        n => n.ContactClassification == ContactClassification.PrimaryContact) ??
                              contacts.FirstOrDefault();
                }

                ReceiptHeader = FormatHeader(contact, cc.Name,distributr);
                
                ReceiptNo = receipt.DocumentReference;
                ReceiptDate = receipt.DocumentDateIssued.ToString("dd-MMM-yyyy");
                ServedByUserName = receipt.DocumentIssuerUser.Username;
               
                ReceiptRecipientCompanyName = receipt.DocumentRecipientCostCentre.Name;

                var rli = Invoice.LineItems.Select((n, i) => new ReceiptLineItemVm()
                                                                 {
                                                                     ProductCode = n.Product.ProductCode,
                                                                     LineItemTotal = n.LineItemTotal,
                                                                     ProductDescription = n.Product.Description,
                                                                     Qty = n.Qty,
                                                                     RowNumber = i + 1,
                                                                     Value = n.Value,
                                                                     VatTotal = n.LineItemVatTotal,
                                                                     VatValue = n.LineItemVatValue,
                                                                 }).ToList();

                var priceService = Using<IDiscountProWorkflow>(c);
                rli.ForEach(x=>x.LineItemTotal=x.LineItemTotal.GetTruncatedValue());

                TotalPaid =
                    receipt.LineItems
                        .Where(n => n.LineItemType == OrderLineItemType.PostConfirmation)
                        .Sum(n => n.Value).ToString("N2");

                var invoiceGross = rli.Sum(x => x.LineItemTotal);
                //TotalGross =Invoice.TotalGross.GetTotalGross().ToString("N2"); 
                TotalGross = invoiceGross.GetTotalGross().ToString("N2"); 

                ReceiptLineItemsList.Clear();
                rli.ForEach(ReceiptLineItemsList.Add);
               
            }
        }

        private ReceiptHeaderInfo FormatHeader(Contact contact, string companyName,Distributor distributr )
        {
            if (contact == null)
                return new ReceiptHeaderInfo()
                           {
                               CompanyName =string.IsNullOrEmpty(companyName)?"Company Name":companyName,
                               PinVatNo =
                               string.Format("VAT No.:{0}  PIN No.:{1}",
                                             distributr != null ? distributr.VatRegistrationNo : "",
                                             distributr != null ? distributr.PIN : "")
                           };

            var physical=string.IsNullOrEmpty(contact.PhysicalAddress)
                                          ? contact.City
                                          : contact.City + " ,"
                                            + contact.PhysicalAddress;
            return new ReceiptHeaderInfo()
                       {
                           CompanyName = string.IsNullOrEmpty(companyName)?"Company Name":companyName,
                           WebSite = "Website:_____________",
                           Email =string.Format("Email:{0}",contact.Email),
                           Address = string.Format("P.O Box: {0},{1}", contact.PostalAddress ?? "_ _ _ _", physical),
                           PinVatNo =
                               string.Format("VAT No.:{0}  PIN No.:{1}",
                                             distributr != null ? distributr.VatRegistrationNo : "",
                                             distributr != null ? distributr.PIN : ""),
                           TelNo =
                               string.Format("Fixed No.:{0},Cell.:{1},Fax:{2}", contact.BusinessPhone,
                                             contact.MobilePhone, contact.Fax)

                       };
        }

       

    
        #endregion

        

        #region properties
        public ObservableCollection<ReceiptLineItemVm> ReceiptLineItemsList { get; set; }

        private RelayCommand _receiptLoadedCommand;
        public RelayCommand ReceiptLoadedCommand
        {
            get { return _receiptLoadedCommand ?? (_receiptLoadedCommand = new RelayCommand(Load)); }
        }

        public const string ReceiptIdPropertyName = "ReceiptId";
        private Guid _receiptId = Guid.Empty;
        public Guid ReceiptId
        {
            get
            {
                return _receiptId;
            }

            set
            {
                if (_receiptId == value)
                {
                    return;
                }
                _receiptId = value;
                RaisePropertyChanged(ReceiptIdPropertyName);
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
                _dateApproved = value;
                RaisePropertyChanged(DateApprovedPropertyName);
            }
        }
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

                _totalVat = value;
                RaisePropertyChanged(TotalVatPropertyName);
            }
        }
        public const string TotalPaidPropertyName = "TotalPaid";
        private string _totalPaid = "";
        public string TotalPaid
        {
            get
            {
                return _totalPaid;
            }

            set
            {
                if (_totalPaid == value)
                {
                    return;
                }
                _totalPaid = value;
                RaisePropertyChanged(TotalPaidPropertyName);
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
                _totalGross = value;
                RaisePropertyChanged(TotalGrossPropertyName);
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
                _receiptDate = value;
                RaisePropertyChanged(ReceiptDatePropertyName);
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
                _servedByUserName = value;
                RaisePropertyChanged(ServedByUserNamePropertyName);
            }
        }

        public const string ReceiptLabelPropertyName = "ReceiptLabel";
        private string _receiptlabel = "OFFICIAL RECEIPT";
        public string ReceiptLabel
        {
            get { return _receiptlabel; }

            set
            {
                if (_receiptlabel == value)
                {
                    return;
                }

                _receiptlabel = value;
                RaisePropertyChanged(ReceiptLabelPropertyName);
            }
        }

        public const string ReceiptHeaderPropertyName = "ReceiptHeader";
        private ReceiptHeaderInfo _receiptHeader = null;
        public ReceiptHeaderInfo ReceiptHeader
        {
            get
            {
                return _receiptHeader;
            }

            set
            {
                if (_receiptHeader == value)
                {
                    return;
                }
                _receiptHeader = value;
                RaisePropertyChanged(ReceiptHeaderPropertyName);
            }
        }

        public const string InvoicePropertyName = "Invoice";
        private Invoice _invoice = null;
        public Invoice Invoice
        {
            get
            {
                return _invoice;
            }

            set
            {
                if (_invoice == value)
                {
                    return;
                }

                _invoice = value;
                RaisePropertyChanged(InvoicePropertyName);
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

                var oldValue = _receiptRecipientCompanyName;
                _receiptRecipientCompanyName = value;
                RaisePropertyChanged(ReceiptRecipientCompanyNamePropertyName);
            }
        }
        #endregion

    }

    public class ReceiptHeaderInfo
    {
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string TelNo { get; set; }
        public string PinVatNo { get; set; }
        public string Email { get; set; }
        public string WebSite { get; set; }
    }

    public class ReceiptLineItemVm
    {
        public int RowNumber { get; set; }
        public string ProductCode { get; set; }
        public string ProductDescription { get; set; }
        public decimal Qty { get; set; }
        public decimal Value { get; set; }
        public decimal VatValue { get; set; }
        public decimal VatTotal { get; set; }
        public decimal LineItemTotal { get; set; }
    }
}
