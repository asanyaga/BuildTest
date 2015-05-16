using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.WPF.Lib.Reporting;
using Distributr.WPF.Lib.UI.Pages;
using GalaSoft.MvvmLight.Command;
using Microsoft.Reporting.WinForms;
using StructureMap;

namespace Distributr.WPF.Lib.ViewModels.Transactional.Orders
{
    public class SalesmanOrderSummaryListingViewModel : DistributrViewModelBase
    {

        public RelayCommand ShowSalesmanPopupCommand { get; set; }
        public RelayCommand ShowOutletPopupCommand { get; set; }

        public SalesmanOrderSummaryListingViewModel()
        {
            ShowSalesmanPopupCommand = new RelayCommand(ShowSalesmanPopup);
            ShowOutletPopupCommand=new RelayCommand(ShowOutletPopup);
        }

        private void ShowOutletPopup()
        {
            SelectedOutlet = null;
            using (var container = NestedContainer)
            {
                if (SelectedSalesman != null)
                {
                    var selected = Using<IItemsLookUp>(container).SelectOutletBySalesman(SelectedSalesman.Id);
                
                    SelectedOutlet = selected as Outlet;
                if (selected == null)
                {
                    SelectedOutlet = new Outlet(Guid.Empty) { Name = "ALL" };
                }
                }
            }
        }

        private void ShowSalesmanPopup()
        {
            SelectedSalesman = null;
            using (var container = NestedContainer)
            {
                var selected = Using<IItemsLookUp>(container).SelectDistributrSalesman();

                SelectedSalesman = selected as DistributorSalesman;
                if (selected == null)
                {
                    SelectedSalesman = new DistributorSalesman(Guid.Empty) { Name = "ALL" };
                }
            }

            
        }

        public const string StartTimePropertyName = "StartTime";
        private DateTime _timespan = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 01);
        public DateTime StartTime
        {
            get
            {
                return _timespan;
            }

            set
            {
                if (_timespan == value)
                {
                    return;
                }

                RaisePropertyChanging(StartTimePropertyName);
                _timespan = value;
                RaisePropertyChanged(StartTimePropertyName);
            }
        }

        public const string EndTimePropertyName = "EndTime";
        private DateTime _endtimespan = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
        public DateTime EndTime
        {
            get
            {
                return _endtimespan;
            }

            set
            {
                if (_endtimespan == value)
                {
                    return;
                }

                RaisePropertyChanging(EndTimePropertyName);
                _endtimespan = value;
                RaisePropertyChanged(EndTimePropertyName);
            }
        }

       
        public const string SelectedSalesmanPropertyName = "SelectedSalesman";
        private DistributorSalesman _selectedSalesman = new DistributorSalesman(Guid.Empty){Name="ALL"};

        public DistributorSalesman SelectedSalesman
        {
            get
            {
                return _selectedSalesman;
            }

            set
            {
                if (_selectedSalesman == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedSalesmanPropertyName);
                _selectedSalesman = value;
                RaisePropertyChanged(SelectedSalesmanPropertyName);
            }
        }

        public const string SelectedOutletPropertyName = "SelectedOutlet";
        private Outlet _selectedOutlet = new Outlet(Guid.Empty) { Name = "ALL" };

        public Outlet SelectedOutlet
        {
            get
            {
                return _selectedOutlet;
            }

            set
            {
                if (_selectedOutlet == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedOutletPropertyName);
                _selectedOutlet = value;
                RaisePropertyChanged(SelectedOutletPropertyName);
            }
        }
        
        public ReportViewer ViewApprovedOrders()
        {
            var reportViewer = new ReportViewer();

            Stream stream = Assembly.GetEntryAssembly().GetManifestResourceStream(ReportCollective.ApprovedOrdersReport);
            reportViewer.LocalReport.DataSources.Clear();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.LocalReport.EnableHyperlinks = true;
            using (var c = ObjectFactory.Container.GetNestedContainer())
            {
                var orderReposiory = c.GetInstance<IMainOrderRepository>();
                var startDatetime = StartTime;
                var endDateTime = EndTime;
                var salesmanId = SelectedSalesman != null ? SelectedSalesman.Id : Guid.Empty;
                var outletId = SelectedOutlet != null ? SelectedOutlet.Id : Guid.Empty;
                var data = orderReposiory.GetApproveOrderAndDateProcessedList(startDatetime, endDateTime,
                                                                              salesmanId, outletId).ToList()
                    .OrderByDescending(p => p.DateProcessed);
                //.ThenByDescending(n => n.OrderReference)
                //.ThenByDescending(n => n.ExternalRefNo);

                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSetApprovedOrder", data));
            }

            reportViewer.LocalReport.LoadReportDefinition(stream);

            reportViewer.LocalReport.Refresh();
            reportViewer.RefreshReport();
            return reportViewer;
        }
        public ReportViewer ViewPendingOrders()
        {
            var reportViewer = new ReportViewer();

            Stream stream = Assembly.GetEntryAssembly().GetManifestResourceStream(ReportCollective.PendingOrdersReport);
            reportViewer.LocalReport.DataSources.Clear();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.LocalReport.EnableHyperlinks = true;
            using (var c = ObjectFactory.Container.GetNestedContainer())
            {
                var orderReposiory = c.GetInstance<IMainOrderRepository>();
                var startDatetime = StartTime;
                var endDateTime = EndTime;
                var salesmanId = SelectedSalesman != null ? SelectedSalesman.Id : Guid.Empty;
                var data = orderReposiory.GetPendingOrderAndDateProcessedList(startDatetime, endDateTime,
                                                                              salesmanId).ToList()
                    .OrderByDescending(p => p.DateProcessed);
                //.ThenByDescending(n => n.OrderReference)
                //.ThenByDescending(n => n.ExternalRefNo);

                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSetApprovedOrder", data));
            }

            reportViewer.LocalReport.LoadReportDefinition(stream);

            reportViewer.LocalReport.Refresh();
            reportViewer.RefreshReport();
            return reportViewer;
        }
    }
}