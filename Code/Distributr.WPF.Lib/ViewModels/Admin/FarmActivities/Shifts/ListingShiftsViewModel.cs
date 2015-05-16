using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.FarmActivities;
using Distributr.Core.Domain.Master.Util;
using Distributr.Core.Repository.Master.Agrimanagr;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.WPF.Lib.Messages;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using GalaSoft.MvvmLight.Messaging;

namespace Distributr.WPF.Lib.ViewModels.Admin.FarmActivities.Shifts
{
    public class ListingShiftsViewModel:ListingsViewModelBase
    {
        private PagenatedList<Shift> _pagedCommodityProducerService;
        private IDistributorServiceProxy _proxy;

        public ListingShiftsViewModel()
        {
            ListOfShifts = new ObservableCollection<VmShift>();
        }

        #region Class Members

        public ObservableCollection<VmShift> ListOfShifts { get; set; }
        #endregion

        protected override void Load(bool isFirstLoad = false)
        {
            Application.Current.Dispatcher.BeginInvoke(
                new Action(
                    delegate
                    {
                            //if (isFirstLoad)

                            using (var c = NestedContainer)
                            {
                                var query = new QueryShift();
                                query.Take = ItemsPerPage;
                                query.Skip = ItemsPerPage * (CurrentPage - 1);
                                query.ShowInactive = ShowInactive;
                                if (!string.IsNullOrWhiteSpace(SearchText))
                                    query.Name = SearchText;

                                var rawList = Using<IShiftRepository>(c).Query(query);
                                _pagedCommodityProducerService = new PagenatedList<Shift>(rawList.Data.OfType<Shift>().AsQueryable(),
                                                                                          CurrentPage,
                                                                                          ItemsPerPage,
                                                                                          rawList.Count, true);

                                ListOfShifts.Clear();
                                int rownumber = 0;
                                _pagedCommodityProducerService.ToList().ForEach(n =>
                                                                       ListOfShifts.Add(new VmShift
                                                                       {
                                                                           Id = n.Id,
                                                                           Code = n.Code,
                                                                           Name = n.Name,
                                                                           StartTime=n.StartTime,
                                                                           EndTime=n.EndTime,
                                                                           Status = n._Status,
                                                                           Action = n._Status == EntityStatus.Active ? "Deactivate" : "Activate",
                                                                           RowNumber = ++rownumber
                                                                       }));
                                UpdatePagenationControl();
                            }
                    }));
        }

        protected override void EditSelected()
        {
            if (SelectedShift != null)
            {
                using (var c = NestedContainer)
                {

                    Messenger.Default.Send(new EditShiftMessage
                    {
                        ShiftId = SelectedShift.Id
                    });
                }
                SendNavigationRequestMessage(
                    new Uri("/Views/Admin/Farmactivities/Shifts/EditShift.xaml", UriKind.Relative));
            }
        }

        protected async override void ActivateSelected()
        {
            string action = SelectedShift.Status == EntityStatus.Active
                              ? "deactivate"
                              : "activate";
            if (MessageBox.Show("Are you sure you want to " + action + " this Shift ?",
                                "Agrimanagr: Manage Shift ", MessageBoxButton.YesNo) ==
                MessageBoxResult.No) return;

            using (var c = NestedContainer)
            {
                ResponseBool response = new ResponseBool() { Success = false };
                if (SelectedShift == null) return;
                _proxy = Using<IDistributorServiceProxy>(c);
                response = await _proxy.ShiftActivateOrDeactivateAsync(SelectedShift.Id);

                if(response.Success)
                {
                    MessageBox.Show(response.ErrorInfo, "Agrimangr: Manage Shift ", MessageBoxButton.OK,
                              MessageBoxImage.Information);
                }
              
            }
        }

        protected async override void DeleteSelected()
        {
            if (
               MessageBox.Show("Are you sure you want to delete this Shift?",
                                    "Agrimanagr: Delete Shift", MessageBoxButton.YesNo) ==
                    MessageBoxResult.No) return;

            using (var c = NestedContainer)
            {
                
                var response = new ResponseBool() { Success = false };
                if (SelectedShift == null) return;
                _proxy = Using<IDistributorServiceProxy>(c);
                response = await _proxy.ShiftDeleteAsync(SelectedShift.Id);
                if (response.Success)
                {
                    var shift = Using<IShiftRepository>(c).GetById(SelectedShift.Id);
                    Using<IShiftRepository>(c).SetAsDeleted(shift);

                    MessageBox.Show(response.ErrorInfo, "Agrimangr:Manage Shift", MessageBoxButton.OK,
                                    MessageBoxImage.Information);
                }
            }
        }

        protected override void GoToPage(PageDestinations page)
        {
            GoToPageBase(page, _pagedCommodityProducerService.PageCount);
        }

        protected override void ComboPageSizesSelectionChanged(int take)
        {
            ItemsPerPage = take;
            Load();
        }

        protected override void UpdatePagenationControl()
        {
            UpdatePagenationControlBase(_pagedCommodityProducerService.PageNumber, _pagedCommodityProducerService.PageCount, _pagedCommodityProducerService.TotalItemCount,
                                     _pagedCommodityProducerService.IsFirstPage, _pagedCommodityProducerService.IsLastPage);
        }

        #region

        
        public const string IdPropertyName = "Id";
        private Guid _id = Guid.Empty;
        public Guid Id
        {
            get
            {
                return _id;
            }

            set
            {
                if (_id == value)
                {
                    return;
                }

                RaisePropertyChanging(IdPropertyName);
                _id = value;
                RaisePropertyChanged(IdPropertyName);
            }
        }

        
        public const string SelectedShiftPropertyName = "SelectedShift";
        private VmShift _selectedShift = null;
        public VmShift SelectedShift
        {
            get
            {
                return _selectedShift;
            }

            set
            {
                if (_selectedShift == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedShiftPropertyName);
                _selectedShift = value;
                RaisePropertyChanged(SelectedShiftPropertyName);
            }
        }

        #endregion
    }
    public class VmShift
    {
        public Guid Id { get; set; }
        public int RowNumber { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public EntityStatus Status { get; set; }
        public string Action { get; set; }
    }
}
