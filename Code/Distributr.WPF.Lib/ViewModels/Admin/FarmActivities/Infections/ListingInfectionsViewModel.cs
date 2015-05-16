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

namespace Distributr.WPF.Lib.ViewModels.Admin.FarmActivities.Infections
{
    public class ListingInfectionsViewModel : ListingsViewModelBase
    {
        private PagenatedList<Infection> _pagedInfections;
        private IDistributorServiceProxy _proxy;

        
        public ListingInfectionsViewModel()
        {
            ListOfInfections = new ObservableCollection<VmInfection>();
        }

        #region Class Members

        public ObservableCollection<VmInfection> ListOfInfections { get; set; }
        #endregion

        #region Methods
        protected override void Load(bool isFirstLoad = false)
        {
            Application.Current.Dispatcher.BeginInvoke(
                new Action(
                    delegate
                    {
                       // if (isFirstLoad)
                            
                        using (var c = NestedContainer)
                        {
                            var query = new QueryInfection();
                            query.Take = ItemsPerPage;
                            query.Skip = ItemsPerPage * (CurrentPage - 1);
                            query.ShowInactive = ShowInactive;
                            if (!string.IsNullOrWhiteSpace(SearchText))
                                query.Name = SearchText;

                            var rawList = Using<IInfectionRepository>(c).Query(query);
                            _pagedInfections = new PagenatedList<Infection>(rawList.Data.OfType<Infection>().AsQueryable(),
                                                                                      CurrentPage,
                                                                                      ItemsPerPage,
                                                                                      rawList.Count, true);

                            ListOfInfections.Clear();
                            int rownumber = 0;
                            _pagedInfections.ToList().ForEach(n =>
                                                                   ListOfInfections.Add(new VmInfection
                                                                   {
                                                                       Id = n.Id,
                                                                       Code = n.Code,
                                                                       Name = n.Name,
                                                                       InfectionType=n.InfectionType,
                                                                       Status=n._Status,
                                                                       RowNumber = ++rownumber,
                                                                       Action=n._Status == EntityStatus.Active? "Deactivate":"Activate"
                                                                   }));
                            UpdatePagenationControl();
                        }
                    }));
        }

        protected override void EditSelected()
        {
            if (SelectedInfection != null)
            {
                using (var c = NestedContainer)
                {

                    Messenger.Default.Send(new EditInfectionsMessage
                        {
                            InfectionId = SelectedInfection.Id
                        });
                }
                SendNavigationRequestMessage(
                    new Uri("/Views/Admin/Farmactivities/Infections/EditInfection.xaml", UriKind.Relative));
            }
        }

        protected async override void ActivateSelected()
        {
            string action = SelectedInfection.Status == EntityStatus.Active
                                ? "deactivate"
                                : "activate";
            if (MessageBox.Show("Are you sure you want to " + action + " this infection?",
                                "Agrimanagr: Manage Infection", MessageBoxButton.YesNo) ==
                MessageBoxResult.No) return;

            using(var c=NestedContainer)
            {
                ResponseBool response = new ResponseBool() {Success = false};
                if (SelectedInfection == null) return;
                _proxy = Using<IDistributorServiceProxy>(c);
                response = await _proxy.InfectionActivateOrDeactivateAsync(SelectedInfection.Id);

                MessageBox.Show(response.ErrorInfo, "Agrimangr: Manage Infection", MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
        }


        protected async override void DeleteSelected()
        {
            if (
               MessageBox.Show("Are you sure you want to delete this Infection?",
                                    "Agrimanagr: Delete Infection", MessageBoxButton.YesNo) ==
                    MessageBoxResult.No) return;

            using (var c = NestedContainer)
            {
                var response = new ResponseBool() { Success = false };
                if (SelectedInfection == null) return;
                _proxy = Using<IDistributorServiceProxy>(c);
                response = await _proxy.InfectionDeleteAsync(SelectedInfection.Id);
                if (response.Success)
                {
                    var infection = Using<IInfectionRepository>(c).GetById(SelectedInfection.Id);
                    Using<IInfectionRepository>(c).SetAsDeleted(infection);
                    MessageBox.Show(response.ErrorInfo, "Agrimangr:Manage Infection", MessageBoxButton.OK,
                                    MessageBoxImage.Information);
                }
                    
            }
        }

        protected override void GoToPage(PageDestinations page)
        {
            GoToPageBase(page, _pagedInfections.PageCount);
        }

        protected override void ComboPageSizesSelectionChanged(int take)
        {
            ItemsPerPage = take;
            Load();
        }

        protected override void UpdatePagenationControl()
        {
            UpdatePagenationControlBase(_pagedInfections.PageNumber, _pagedInfections.PageCount, _pagedInfections.TotalItemCount,
                                     _pagedInfections.IsFirstPage, _pagedInfections.IsLastPage);
        }
        #endregion

        #region Properties

        public const string InfectionIdPropertyName = "InfectionId";
        private Guid _infectionId = Guid.Empty;
        public Guid InfectionId
        {
            get
            {
                return _infectionId;
            }

            set
            {
                if (_infectionId == value)
                {
                    return;
                }

                RaisePropertyChanging(InfectionIdPropertyName);
                _infectionId = value;
                RaisePropertyChanged(InfectionIdPropertyName);
            }
        }

        public const string SelectedInfectionPropertyName = "SelectedInfection";
        private VmInfection _selectedInfection = null;
        public VmInfection SelectedInfection
        {
            get
            {
                return _selectedInfection;
            }

            set
            {
                if (_selectedInfection == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedInfectionPropertyName);
                _selectedInfection = value;
                RaisePropertyChanged(SelectedInfectionPropertyName);
            }
        }

        #endregion
    }
    public class VmInfection
    {
        public Guid Id { get; set; }
        public int RowNumber { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public EntityStatus Status { get; set; }
        public InfectionType InfectionType { get; set; }
        public string Action { get; set; }
    }
}
