using System;
using System.Collections.Generic;
using System.Windows;
using Distributr.WPF.Lib.Service.Utility;
using Distributr.WPF.Lib.Services.Service;
using Distributr.WPF.Lib.Services.Service.Utility;
using GalaSoft.MvvmLight.Command;
using StructureMap;


namespace Distributr.WPF.Lib.ViewModels.Reports
{
    public class ReportsViewModel: DistributrViewModelBase
    {
        public ReportsViewModel()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                DistributorID = Using<IConfigService>(c).Load().CostCentreId;
            }
            RunDisplayReportCommand = new RelayCommand(DisplayReportCommand);
        }

        #region methods

        public RelayCommand RunDisplayReportCommand { get; set; }

        public void Setup()
        {
            using (IContainer c = NestedContainer)
            {
                GeneralSetting reportUrl = Using<IGeneralSettingRepository>(c).GetByKey(GeneralSettingKey.ReportUrl);
                if (reportUrl == null)
                {
                    MessageBox.Show("Contact system admin to setup Report url", "Distributr Online Reports",
                                    MessageBoxButton.OK);
                    ServerUrlIsSet = false;
                    return;
                }
                ServerUrlIsSet = true;
                ServerUrl = reportUrl.SettingValue;
            }
        }

        public Uri getReportUri()
        {
            string url = ServerUrl + ReportUrl + "?DistributorID=" + DistributorID;
            return new Uri(url, UriKind.Absolute);
        }
        public string GetReportPath()
        {
            string url = ReportUrl;
            return url;
        }
        public Uri getReportServerUri()
        {
            string url = ReportServerUrl + ReportUrl /*+ "?DistributorID=" + DistributorID*/;
            return new Uri(url, UriKind.Absolute);
        }
        private void DisplayReportCommand()
        {
            //GeneralSetting reportUrl = _generalSettingService.GetByKey(GeneralSettingKey.ReportUrl);
            //if(reportUrl==null)
            //{
            //    MessageBox.Show("Contact system admin to setup Report url", "Distributr Online Reports",
            //                    MessageBoxButton.OK);
            //    return;
            //}
            //ServerUrl = reportUrl.SettingValue;
            //HtmlPopupWindowOptions options = new HtmlPopupWindowOptions();
            //options.Left = 0;
            //options.Top = 0;
            //options.Width = 930;
            //options.Height = 800;
            //options.Menubar = false;
            //options.Toolbar = false;
            //options.Directories = false;
            //options.Status = false;
            //options.Location = false;
            //string url = ServerUrl + ReportUrl + "?DistributorID=" + DistributorID;
            //if (true == HtmlPage.IsPopupWindowAllowed)
            //    HtmlPage.PopupWindow(new Uri(url), "new", options);
            //else
            //{
            //    MyHyperlinkButton button = new MyHyperlinkButton();
            //    button.NavigateUri = new Uri(url);
            //    button.TargetName = "_blank";
            //    button.ClickMe();
            //}
        }

        #endregion

        #region Properties

        public const string ServerUrlIsSetPropertyName = "ServerUrlIsSet";
        private bool _serverUrlIsSet = false;
        public bool ServerUrlIsSet
        {
            get
            {
                return _serverUrlIsSet;
            }

            set
            {
                if (_serverUrlIsSet == value)
                {
                    return;
                }

                var oldValue = _serverUrlIsSet;
                _serverUrlIsSet = value;

                RaisePropertyChanged(ServerUrlIsSetPropertyName);
            }
        }

        public const string DistributorIDPropertyName = "DistributorID";
        private Guid _DistributorID = Guid.Empty;
        public Guid DistributorID
        {
            get
            {
                return _DistributorID;
            }

            set
            {
                if (_DistributorID == value)
                {
                    return;
                }

                var oldValue = _DistributorID;
                _DistributorID = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(DistributorIDPropertyName);
            }
        }
         
        public const string StartDatePropertyName = "StartDate";
        private DateTime _startDate = DateTime.Now.Add(-(new TimeSpan(7,0,0,0)));
        public DateTime StartDate
        {
            get
            {
                return _startDate;
            }

            set
            {
                if (_startDate == value)
                {
                    return;
                }

                RaisePropertyChanging(StartDatePropertyName);
                _startDate = value;
                RaisePropertyChanged(StartDatePropertyName);
            }
        }
         
        public const string EndDatePropertyName = "EndDate";
        private DateTime _endDate = DateTime.Now;
        public DateTime EndDate
        {
            get
            {
                return _endDate;
            }

            set
            {
                if (_endDate == value)
                {
                    return;
                }

                RaisePropertyChanging(EndDatePropertyName);
                _endDate = value;
                RaisePropertyChanged(EndDatePropertyName);
            }
        }

        public const string ReportUrlPropertyName = "ReportUrl";
        private string _ReportUrl= null;
        public string ReportUrl
        {
            internal get
            {
                return _ReportUrl;
            }

            set
            {
                if (_ReportUrl == value)
                {
                    return;
                }

                var oldValue = _ReportUrl;
                _ReportUrl = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(ReportUrlPropertyName);
            }
        }
        public const string ReportServerUrlPropertyName = "ReportServerUrl";
        private string _ReportServerUrl = "http://10.0.0.19/distributrReport/";
        public string ReportServerUrl
        {
             internal get
            {
                return _ReportServerUrl;
            }

            set
            {
                if (_ReportServerUrl == value)
                {
                    return;
                }

                var oldValue = _ReportServerUrl;
                _ReportServerUrl = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(ReportServerUrlPropertyName);
            }
        }

        public const string ServerUrlPropertyName = "ServerUrl";
        //private string _serverUrl = "http://qa2hq.virtualcity.co.ke/qa2hq/";
        //private string _serverUrl = "http://virtualcity.distribut-r.com/distributrhq/";// 
        private string _serverUrl = "http://localhost:54514/";
        public string ServerUrl
        {
            internal get
            {
                return _serverUrl;
            }

            set
            {
                if (_serverUrl == value)
                {
                    return;
                }

                var oldValue = _serverUrl;
                _serverUrl = value;

                RaisePropertyChanged(ServerUrlPropertyName);
            }
        }
        #endregion
    }
}
