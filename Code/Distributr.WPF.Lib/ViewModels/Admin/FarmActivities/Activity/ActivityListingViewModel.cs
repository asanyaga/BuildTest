using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.FarmActivities;
using Distributr.Core.Domain.Master.Util;
using Distributr.Core.Domain.Transactional.ActivityDocumentEntities;
using Distributr.Core.Repository.Master.Agrimanagr;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.WPF.Lib.Messages;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace Distributr.WPF.Lib.ViewModels.Admin.FarmActivities.Activity
{
   
    public class ActivityListingViewModel:ListingsViewModelBase
    {
        private PagenatedList<ActivityDocument> _pagedActivities;

        public  ActivityListingViewModel()
        {
            ActivityTypesList = new ObservableCollection<ActivityTypeItem>();
            ActivityItemsList = new ObservableCollection<ActivityItem>();
            DetailsPopUpCommand = new RelayCommand<ActivityItem>(DetailsPopUp);
            TabSelectionChangedCommand = new RelayCommand<SelectionChangedEventArgs>(TabSelectionChanged);
        }

        private void DetailsPopUp(ActivityItem SelectedActivityItem)
        {
            Messenger.Default.Send(new DetailsPopUpMessage
            {
                ActivityId = SelectedActivityItem.Id
            });
            SendNavigationRequestMessage(new Uri("/Views/Activities/ActivityDetailsPopUp.xaml", UriKind.Relative));
        }

        private void TabSelectionChanged(SelectionChangedEventArgs e)
        {

            var original = e.OriginalSource.GetType();
           
            if (original != typeof(TabControl))
                return;
            if (SelectedTabItem != null)
           {
              // DetailsPopUp();
               LoadActivity();
            }
            e.Handled = true;

        }

        #region Class Members
       
        public RelayCommand<SelectionChangedEventArgs> TabSelectionChangedCommand { get; set; }
        public ObservableCollection<ActivityTypeItem> ActivityTypesList { get; set; }
        public ObservableCollection<ActivityItem> ActivityItemsList { get; set; }
        public RelayCommand<ActivityItem> DetailsPopUpCommand { get; set; }
        #endregion

        #region Methods
        protected override void Load(bool isFirstLoad = false)
        {
            Application.Current.Dispatcher.BeginInvoke(
              new Action(
                  delegate
                  {
                      if (isFirstLoad)
                      {
                          //Activitytypeslist.Add();
                      }

                      using (var c = NestedContainer)
                      {
                          var query = new QueryActivityType();
                          if (!string.IsNullOrWhiteSpace(SearchText))
                              query.Name = SearchText;
                              
                          var rawList = Using<IActivityTypeRepository>(c).Query(query);
                          ActivityTypesList.Clear();
                          foreach(var item in rawList.Data.OfType<ActivityType>())
                          {
                              ActivityTypesList.Add(new ActivityTypeItem {Name = item.Name, Id = item.Id});
                          }
                          
                          
                          
                      }
                  }));
            //LoadActivity();
        }

      

        private void LoadActivity()
        {
            ActivityItemsList.Clear();
         
                            using (var c = NestedContainer)
                            {
                                var query = new QueryActivity(){ActivityTypeId=SelectedTabItem.Id};
                                var result = Using<IActivityRepository>(c).Query(query);
                                foreach (var activityDocument in result.Data)
                                {
                                    ActivityItemsList.Add(new ActivityItem
                                                              {
                                                                  Id=activityDocument.Id,
                                                                  Description = activityDocument.Description,
                                                                  Hub = activityDocument.Hub.Name,
                                                                  Season = activityDocument.Season.Name,
                                                                  ActivityReference = activityDocument.ActivityReference,
                                                                  ActivityDate = activityDocument.DocumentDate,
                                                                  Centre = activityDocument.Centre.Name,
                                                                  FieldClerk = activityDocument.FieldClerk.Name,
                                                                  ActivityType = activityDocument.ActivityType.Name,
                                                                  Producer = activityDocument.Producer.Name

                                                              });
                                }
                            }
                        
        }

        protected override void EditSelected()
        {
            throw new NotImplementedException();
        }

        protected override void ActivateSelected()
        {
            throw new NotImplementedException();
        }

        protected override void DeleteSelected()
        {
            throw new NotImplementedException();
        }

        protected override void GoToPage(PageDestinations page)
        {
            GoToPageBase(page, _pagedActivities.PageCount);
        }

        protected override void ComboPageSizesSelectionChanged(int take)
        {
            ItemsPerPage = take;
            Load();
        }

        protected override void UpdatePagenationControl()
        {
            UpdatePagenationControlBase(_pagedActivities.PageNumber, _pagedActivities.PageCount, _pagedActivities.TotalItemCount,
                                     _pagedActivities.IsFirstPage, _pagedActivities.IsLastPage);
        }
        #endregion

        
        
        #region Properties
        public const string SelectedTabItemPropertyName = "SelectedTabItem";
        private ActivityTypeItem _activitytype = null;

        public ActivityTypeItem SelectedTabItem
        {
            get
            {
                return _activitytype;
            }

            set
            {
                if (_activitytype == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedTabItemPropertyName);
                _activitytype = value;
                RaisePropertyChanged(SelectedTabItemPropertyName);
            }
        }

        
        public const string SelectedActivityItemPropertyName = "SelectedActivityItem";
        private ActivityItem _selectedActivityItem = null;
        public ActivityItem SelectedActivityItem
        {
            get
            {
                return _selectedActivityItem;
            }

            set
            {
                if (_selectedActivityItem == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedActivityItemPropertyName);
                _selectedActivityItem = value;
                RaisePropertyChanged(SelectedActivityItemPropertyName);
            }
        }

       
        #endregion                  

        
    }

    public class ActivityTypeItem
    {
        public Guid Id { get; set; }
        public string Name{get;set;}
    }
    public class ActivityItem
    {
        public Guid Id { get; set; }
        public string DocumentReference { get; set; }
        public string Hub { get; set; }
        public string FieldClerk { get; set; }
        public string Supplier { get; set; }
        public string Producer { get; set; }
        public string Route { get; set; }
        public string Centre { get; set; }
        public string ActivityType { get; set; }
        public DateTime ActivityDate { get; set; }
        public Guid DocumentIssuerCostCentreApplicationId { get; set; }
        public string Season { get; set; }
        public string Description { get; set; }
        public string ActivityReference { get; set; }
        
        
        
    }
}
  
