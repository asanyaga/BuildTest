using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Drawing.Printing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Distributr.Core.Domain.Master.CommodityEntities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility;
using Distributr.WPF.Lib.ViewModels.Utils;
using Distributr.WPF.Lib.ViewModels.Warehousing;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using StructureMap;

namespace Distributr.WPF.Lib.ViewModels.Transactional.InventoryTransfer
{

    public class CommodityReleaseDocumentViewModel : DistributrViewModelBase
    {

        public CommodityReleaseDocumentViewModel()
        {
            ReleaseLineItemList = new ObservableCollection<ReleaseDocumentLineItem>();
            PrintCommand = new RelayCommand<Panel>(OnPrintCommand);
        }

        public ObservableCollection<ReleaseDocumentLineItem> ReleaseLineItemList { get; set; }
        public ICommand PrintCommand { get; set; }

        #region Properties
        public const string CompanyNamePropertyName = "CompanyName";

        private string _companyName = "Hub";

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

                RaisePropertyChanging(CompanyNamePropertyName);
                _companyName = value;
                RaisePropertyChanged(CompanyNamePropertyName);
            }
        }

        public const string AddressPropertyName = "Address";

        private string _address = "76460-00508";

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

                RaisePropertyChanging(AddressPropertyName);
                _address = value;
                RaisePropertyChanged(AddressPropertyName);
            }
        }


        public const string TelephonePropertyName = "Telephone";

        private string _tel = "0722000000";

        public string Telephone
        {
            get
            {
                return _tel;
            }

            set
            {
                if (_tel == value)
                {
                    return;
                }

                RaisePropertyChanging(TelephonePropertyName);
                _tel = value;
                RaisePropertyChanged(TelephonePropertyName);
            }
        }


        public const string ReleaseNoPropertyName = "ReleaseNo";

        private string _releaseNo = "";

        public string ReleaseNo
        {
            get
            {
                return _releaseNo;
            }

            set
            {
                if (_releaseNo == value)
                {
                    return;
                }

                RaisePropertyChanging(ReleaseNoPropertyName);
                _releaseNo = value;
                RaisePropertyChanged(ReleaseNoPropertyName);
            }
        }

        public const string ClerkPropertyName = "Clerk";

        private string _clerk = "";

        public string Clerk
        {
            get
            {
                return _clerk;
            }

            set
            {
                if (_clerk == value)
                {
                    return;
                }

                RaisePropertyChanging(ClerkPropertyName);
                _clerk = value;
                RaisePropertyChanged(ClerkPropertyName);
            }
        }


        public const string DatePropertyName = "Date";

        private string _date = "";


        public string Date
        {
            get
            {
                return _date;
            }

            set
            {
                if (_date == value)
                {
                    return;
                }

                RaisePropertyChanging(DatePropertyName);
                _date = value;
                RaisePropertyChanged(DatePropertyName);
            }
        }

        public const string StorePropertyName = "Store";

        private string _store = "";

        public string Store
        {
            get
            {
                return _store;
            }

            set
            {
                if (_store == value)
                {
                    return;
                }

                RaisePropertyChanging(StorePropertyName);
                _store = value;
                RaisePropertyChanged(StorePropertyName);
            }
        }

        public const string AppNAmePropertyName = "AppNAme";

        private string _appName = "";

        public string AppNAme
        {
            get
            {
                return _appName;
            }

            set
            {
                if (_appName == value)
                {
                    return;
                }

                RaisePropertyChanging(AppNAmePropertyName);
                _appName = value;
                RaisePropertyChanged(AppNAmePropertyName);
            }
        }

        public const string RecepientNamePropertyName = "RecepientName";

        private string _recipientName = string.Empty;

        public string RecepientName
        {
            get
            {
                return _recipientName;
            }

            set
            {
                if (_recipientName == value)
                {
                    return;
                }

                RaisePropertyChanging(RecepientNamePropertyName);
                _recipientName = value;
                RaisePropertyChanged(RecepientNamePropertyName);
            }
        }

        public const string RecepientCoNamePropertyName = "RecepientCoName";

        private string _recepientCoName = string.Empty;

        public string RecepientCoName
        {
            get
            {
                return _recepientCoName;
            }

            set
            {
                if (_recepientCoName == value)
                {
                    return;
                }

                RaisePropertyChanging(RecepientCoNamePropertyName);
                _recepientCoName = value;
                RaisePropertyChanged(RecepientCoNamePropertyName);
            }
        }

        public const string RecepientAddressPropertyName = "RecepientAddress";

        private string _recepientAddress = string.Empty;

        public string RecepientAddress
        {
            get
            {
                return _recepientAddress;
            }

            set
            {
                if (_recepientAddress == value)
                {
                    return;
                }

                RaisePropertyChanging(RecepientAddressPropertyName);
                _recepientAddress = value;
                RaisePropertyChanged(RecepientAddressPropertyName);
            }
        }
        #endregion

        public void LoadReleaseDocument(CommodityReleaseNote commodityReleaseNote)
        {
            if (commodityReleaseNote != null)
            {
                Reset();
                ReleaseLineItemList.Clear();
                #region Initialize

                using (IContainer cont = NestedContainer)
                {
                    var _costCentreRepo = Using<ICostCentreRepository>(cont);
                    var costCentre = _costCentreRepo.GetById(commodityReleaseNote.DocumentIssuerCostCentre.ParentCostCentre.Id);
                    CompanyName = costCentre.Name;
                }

                //CompanyName = ConfigurationManager.AppSettings["CompanyName"];
                Address = ConfigurationManager.AppSettings["Address"];
                Telephone = ConfigurationManager.AppSettings["Telephone"];
                AppNAme = commodityReleaseNote.DocumentIssuerCostCentre.Name;
                ReleaseNo = commodityReleaseNote.DocumentReference;
                Clerk = string.Format(commodityReleaseNote.DocumentIssuerUser.Username);
                Date = DateTime.Now.ToShortDateString();
                Store = commodityReleaseNote.DocumentRecipientCostCentre.Name;

                string[] data = commodityReleaseNote.Description.Split(';');

                string[] Name = data[0].Split(':');
                RecepientName = Name[1];
                
                string[] CoName = data[1].Split(':');
                RecepientCoName = CoName[1];
                
                string[] ClinetAddress = data[2].Split(':');
                RecepientAddress = ClinetAddress[1];

                foreach (var item in commodityReleaseNote.LineItems)
                {
                    var lineItem = new ReleaseDocumentLineItem()
                    {
                        Commodity = item.Commodity.Name,
                        Grade = item.CommodityGrade.Name,
                        Weight = item.Weight.ToString(),
                        ContainerNo = item.ContainerNo
                    };
                    ReleaseLineItemList.Add(lineItem);
                }

                #endregion
            }
        }

        public void OnPrintCommand(Panel p)
        {
            var printer = new SerialPortUtil();
            PrintDocument pd = new PrintDocument();
            pd.PrintPage += (s, e) =>
            {
                p.HorizontalAlignment = HorizontalAlignment.Center;
                e.HasMorePages = false;
            };

            pd.Print();

        }

        private void Reset()
        {
            CompanyName = string.Empty;
            Address = string.Empty;
            Telephone = string.Empty;
            ReleaseNo = string.Empty;
            Clerk = string.Empty;
            Date = string.Empty;
            Store = string.Empty;
        }
    }

    public class ReleaseDocumentLineItem
    {
        public string Grade { get; set; }
        public string Commodity { get; set; }
        public string Weight { get; set; }
        public string ContainerNo { get; set; }
    }
}