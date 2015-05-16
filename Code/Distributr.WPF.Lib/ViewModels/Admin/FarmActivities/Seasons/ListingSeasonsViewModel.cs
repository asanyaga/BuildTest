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
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace Distributr.WPF.Lib.ViewModels.Admin.FarmActivities.Seasons
{
    public class ListingSeasonsViewModel:ListingsViewModelBase
    {
        private PagenatedList<Season> _pagedSeasons;
        private IDistributorServiceProxy _proxy;

        public ListingSeasonsViewModel()
        {
            ListOfSeasons = new ObservableCollection<VmSeason>();
        }

        #region Class Members
        public ObservableCollection<VmSeason> ListOfSeasons { get; set; }
        #endregion

        protected override void Load(bool isFirstLoad = false)
        {
            

            Application.Current.Dispatcher.BeginInvoke(
                new Action(
                    delegate
                    {
                        if (isFirstLoad)
                            Setup();
                        using (var c = NestedContainer)
                        {
                            var query = new QuerySeason();
                            query.Take = ItemsPerPage;
                            query.Skip = ItemsPerPage * (CurrentPage - 1);
                            query.ShowInactive = ShowInactive;
                            if (!string.IsNullOrWhiteSpace(SearchText))
                                query.Name = SearchText;

                            var rawList = Using<ISeasonRepository>(c).Query(query);
                            _pagedSeasons = new PagenatedList<Season>(rawList.Data.OfType<Season>().AsQueryable(),
                                                                                      CurrentPage,
                                                                                      ItemsPerPage,
                                                                                      rawList.Count, true);

                            ListOfSeasons.Clear();
                            int rownumber = 0;
                            _pagedSeasons.ToList().ForEach(n =>
                                                                   ListOfSeasons.Add(new VmSeason
                                                                   {
                                                                       Id = n.Id,
                                                                       Code = n.Code,
                                                                       Name=n.Name,
                                                                       StartDate=n.StartDate.Date,
                                                                       EndDate=n.EndDate.Date,
                                                                       Status = n._Status,
                                                                       Action=n._Status == EntityStatus.Active? "Deactivate":"Activate",
                                                                       RowNumber = ++rownumber
                                                                   }));
                            UpdatePagenationControl();
                        }
                    }));
        }

        private void Setup()
        {
            LoadSeasons();
        }

        private void LoadSeasons()
        {
            ListOfSeasons.Clear();
            using (var c = NestedContainer)
            {
                var listOfFarmers = Using<ISeasonRepository>(c).GetAll();
                int rowNumber = 0;
                foreach (var l in listOfFarmers)
                {
                    ListOfSeasons.Add(new VmSeason
                    {
                        Id = l.Id,
                        RowNumber = rowNumber + 1,
                        Name=l.Name,
                        Code = l.Code,
                        StartDate=l.StartDate,
                        EndDate=l.EndDate
                    });
                    rowNumber++;
                }
            }
        }

        protected override void EditSelected()
        {
            if (SelectedSeason != null)
            {
                using (var c = NestedContainer)
                {

                    Messenger.Default.Send(new EditSeasonMessage
                    {
                        SeasonId=SelectedSeason.Id
                    });
                }
                SendNavigationRequestMessage(
                    new Uri("/Views/Admin/Farmactivities/Seasons/EditSeason.xaml", UriKind.Relative));


            }
        }

        protected async override void ActivateSelected()
        {
            string action = SelectedSeason.Status == EntityStatus.Active
                                ? "deactivate"
                                : "activate";
            if (MessageBox.Show("Are you sure you want to " + action + " this season?",
                                "Agrimanagr: Manage season", MessageBoxButton.YesNo) ==
                MessageBoxResult.No) return;

            using (var c = NestedContainer)
            {
                ResponseBool response = new ResponseBool() { Success = false };
                if (SelectedSeason == null) return;
                _proxy = Using<IDistributorServiceProxy>(c);
                response = await _proxy.SeasonActivateOrDeactivateAsync(SelectedSeason.Id);

                MessageBox.Show(response.ErrorInfo, "Agrimangr: Manage Season", MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
        }

        protected async override void DeleteSelected()
        {
            if (
               MessageBox.Show("Are you sure you want to delete this season?",
                                    "Agrimanagr: Delete Season", MessageBoxButton.YesNo) ==
                    MessageBoxResult.No) return;

            using (var c = NestedContainer)
            {
                
                var response = new ResponseBool() { Success = false };
                if (SelectedSeason == null) return;
                _proxy = Using<IDistributorServiceProxy>(c);
                response = await _proxy.SeasonDeleteAsync(SelectedSeason.Id);
                if (response.Success)
                {
                    var season = Using<ISeasonRepository>(c).GetById(SelectedSeason.Id);
                    Using<ISeasonRepository>(c).SetAsDeleted(season);
                    MessageBox.Show(response.ErrorInfo, "Agrimangr:Manage Season", MessageBoxButton.OK,
                                    MessageBoxImage.Information);
                }
                    
            }
        }

        protected override void GoToPage(PageDestinations page)
        {
            GoToPageBase(page, _pagedSeasons.PageCount);
        }

        protected override void ComboPageSizesSelectionChanged(int take)
        {
            ItemsPerPage = take;
            Load();
        }

        protected override void UpdatePagenationControl()
        {
            UpdatePagenationControlBase(_pagedSeasons.PageNumber, _pagedSeasons.PageCount, _pagedSeasons.TotalItemCount,
                                     _pagedSeasons.IsFirstPage, _pagedSeasons.IsLastPage);
        }

        #region Properties
        public const string SeasonIdPropertyName = "SeasonId";
        private Guid _seasonId = Guid.Empty;
        public Guid SeasonId
        {
            get
            {
                return _seasonId;
            }

            set
            {
                if (_seasonId == value)
                {
                    return;
                }

                RaisePropertyChanging(SeasonIdPropertyName);
                _seasonId = value;
                RaisePropertyChanged(SeasonIdPropertyName);
            }
        }

        
        public const string SelectedSeasonPropertyName = "SelectedSeason";
        private VmSeason _selectedSeason = null;
        public VmSeason SelectedSeason
        {
            get
            {
                return _selectedSeason;
            }

            set
            {
                if (_selectedSeason == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedSeasonPropertyName);
                _selectedSeason = value;
                RaisePropertyChanged(SelectedSeasonPropertyName);
            }
        }

        #endregion

    }
    public class VmSeason
    {
        public Guid Id { get; set; }
        public int RowNumber { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public EntityStatus Status { get; set; }
        public string Action { get; set; }
    }
}
