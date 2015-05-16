using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.FarmActivities;
using Distributr.Core.Repository.Master.Agrimanagr;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.WPF.Lib.Messages;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Admin.FarmActivities.Seasons
{
    public class SeasonViewModel:DistributrViewModelBase
    {
        public SeasonViewModel()
        {
            LoadPageCommand = new RelayCommand(Load);
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
            CommodityProducerList = new ObservableCollection<CommodityProducer>();

        }

        

        #region Class Members
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public RelayCommand LoadPageCommand { get; set; }
        public ObservableCollection<CommodityProducer> CommodityProducerList { get; set; }
        #endregion

        #region Methods
        public void EditSeason(EditSeasonMessage messageFrom)
        {
            Id = messageFrom.SeasonId;
            IsEdit = true;
        }

        private void Load()
        {
            LoadCommodityProducers();
            ClearViewModel();
            if(!IsEdit)
            {
                Id = Guid.NewGuid();
                PageTitle = "Add Season";
            }
            else
            {
                LoadForEdit();
                IsEdit = false;
            }
        }

        private void ClearViewModel()
        {
            Code = "";
            Name = "";
            StartDate = DateTime.Now;
            EndDate = DateTime.Now.AddMonths(1);
            Description = "";
        }

        private void LoadForEdit()
        {
            PageTitle = "Edit Season";

           using(var c=NestedContainer)
           {
               var editSeason = Using<ISeasonRepository>(c).GetById(Id);
               if(editSeason!=null)
               {
                   Code = editSeason.Code;
                   Name = editSeason.Name;
                   SelectedCommodityProducer =CommodityProducerList.FirstOrDefault(p => p.Id == editSeason.CommodityProducer.Id);
                   StartDate = editSeason.StartDate;
                   EndDate = editSeason.EndDate;
                   Description = editSeason.Description;
               }
           }
        }


        private void LoadCommodityProducers()
        {
            CommodityProducerList.Clear();
            CommodityProducerList.Add(new CommodityProducer(Guid.Empty) {Name = "--Select Producer--"});

            using(var c=NestedContainer)
            {
                Using<ICommodityProducerRepository>(c).GetAll().ToList().ForEach(n => CommodityProducerList.Add(n));
            }
        }

        private void Cancel()
        {
            if (MessageBox.Show("Unsaved Changes will be lost. Do you want to continue?", "Agrimangr: " + PageTitle,
                             MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
            {
                ClearViewModel();
                SendNavigationRequestMessage(new Uri("/Views/Admin/Farmactivities/Seasons/ListSeasons.xaml", UriKind.Relative));
            }
        }

        private async void Save()
        {
            using(var c=NestedContainer)
            {
                
                var season = Using<ISeasonRepository>(c).GetById(Id);
                if(season==null)
                {
                    season = new Season(Guid.NewGuid());
                    
                }

                season.Code = Code;
                season.Name = Name;
                season.StartDate = StartDate;
                if (StartDate > EndDate)
                {
                    MessageBox.Show("Start Date should be less than End Date", "Agrimangr: Manage Season ",MessageBoxButton.OK,MessageBoxImage.Error);
                    return;
                }
                {
                   season.EndDate = EndDate ; 
                }
                
                season.Description = Description;
                season.CommodityProducer = SelectedCommodityProducer;
                season._Status = EntityStatus.Active;

                var response =await Using<IDistributorServiceProxy>(c).SeasonSaveAsync(season);
               
                if(response.Success)
                {
                    MessageBox.Show("Season Successfully Added", "Agrimangr: Manage Season ",
                                   MessageBoxButton.OK, MessageBoxImage.Information);
                    SendNavigationRequestMessage(new Uri("/Views/Admin/Farmactivities/Seasons/ListSeasons.xaml", UriKind.Relative));
                }
                else
                {
                    MessageBox.Show(response.ErrorInfo, "Agrimangr: Manage Season ",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            

        }

        #endregion

        #region Properties

        
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

        public const string PageTitlePropertyName = "PageTitle";
        private string _pageTitle = "";
        public string PageTitle
        {
            get
            {
                return _pageTitle;
            }

            set
            {
                if (_pageTitle == value)
                {
                    return;
                }

                RaisePropertyChanging(PageTitlePropertyName);
                _pageTitle = value;
                RaisePropertyChanged(PageTitlePropertyName);
            }
        }
        
        public const string IsEditPropertyName = "IsEdit";
        private bool _isEdit = false;
        public bool IsEdit
        {
            get
            {
                return _isEdit;
            }

            set
            {
                if (_isEdit == value)
                {
                    return;
                }

                RaisePropertyChanging(IsEditPropertyName);
                _isEdit = value;
                RaisePropertyChanged(IsEditPropertyName);
            }
        }

        public const string CodePropertyName = "Code";
        private string _code = "";
        public string Code
        {
            get
            {
                return _code;
            }

            set
            {
                if (_code == value)
                {
                    return;
                }

                RaisePropertyChanging(CodePropertyName);
                _code = value;
                RaisePropertyChanged(CodePropertyName);
            }
        }

        public const string NamePropertyName = "Name";
        private string _name = "";
        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                if (_name == value)
                {
                    return;
                }

                RaisePropertyChanging(NamePropertyName);
                _name = value;
                RaisePropertyChanged(NamePropertyName);
            }
        }


        public const string SelectedCommodityProducerPropertyName = "SelectedCommodityProducer";
        private CommodityProducer _selectedCommodityProducer = new CommodityProducer(Guid.Empty);
        public CommodityProducer SelectedCommodityProducer
        {
            get
            {
                return _selectedCommodityProducer;
            }

            set
            {
                if (_selectedCommodityProducer == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedCommodityProducerPropertyName);
                _selectedCommodityProducer = value;
                RaisePropertyChanged(SelectedCommodityProducerPropertyName);
            }
        }
        
        public const string StartDatePropertyName = "StartDate";
        private DateTime _startDate = DateTime.Now;
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
        private DateTime _endDate = DateTime.Now.AddMonths(2);
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

        
        public const string DescriptionPropertyName = "Description";
        private string _description = "";
        public string Description
        {
            get
            {
                return _description;
            }

            set
            {
                if (_description == value)
                {
                    return;
                }

                RaisePropertyChanging(DescriptionPropertyName);
                _description = value;
                RaisePropertyChanged(DescriptionPropertyName);
            }
        }
        #endregion

        
    }
}
