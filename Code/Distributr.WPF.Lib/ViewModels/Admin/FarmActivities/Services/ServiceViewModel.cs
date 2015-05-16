using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.FarmActivities;
using Distributr.Core.Repository.Master.Agrimanagr;
using Distributr.WPF.Lib.Messages;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Admin.FarmActivities.Services
{
    public class ServiceViewModel:DistributrViewModelBase
    {
        public ServiceViewModel()
        {
            LoadPageCommand = new RelayCommand(Load);
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);

        }

       

        #region Class Members

        public RelayCommand SaveCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public RelayCommand LoadPageCommand { get; set; }
        #endregion

        #region Methods
        public void EditService(EditCommodityProducerServiceMessage messageTo)
        {
            Id = messageTo.ServiceId;
            IsEdit = true;
        }

        private void Load()
        {
            ClearViewModel();
            if(!IsEdit)
            {
                Id = Guid.NewGuid();
                PageTitle = "Add Commodity Producer Service";
            }
            else
            {
                LoadForEdit();
                IsEdit = false;
            }

        }

        private void ClearViewModel()
        {
            Cost = 0m;
            Code = "";
            Description = "";
            Name = "";
        }

        private void LoadForEdit()
        {
            PageTitle = "Edit Commodity Producer Service";
            using(var c=NestedContainer)
            {
                var editService = Using<IServiceRepository>(c).GetById(Id);
                if(editService!=null)
                {
                    Code = editService.Code;
                    Name = editService.Name;
                    Cost = editService.Cost;
                    Description = editService.Description;
                }
            }
        }

        private void Cancel()
        {
            if (MessageBox.Show("Unsaved Changes will be lost. Do you want to continue?", "Agrimangr: " + PageTitle,
                            MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
            {
                ClearViewModel();
                SendNavigationRequestMessage(new Uri("/Views/Admin/Farmactivities/Services/ListServices.xaml", UriKind.Relative));
            }
              
        }

        private async void Save()
        {
            
            using(var c=NestedContainer)
            {
                var service = Using<IServiceRepository>(c).GetById(Id);

                if(service==null)
                {
                    service = new CommodityProducerService(Guid.NewGuid());
                }
                service.Code = Code;
                service.Name = Name;
                service.Cost = Cost;
                service.Description = Description;
                service._Status = EntityStatus.Active;

                var response = await Using<IDistributorServiceProxy>(c).CommodityProducerServiceSaveAsync(service);

               
                if(response.Success)
                {
                    MessageBox.Show("Service Successfully Added", "Agrimangr: Manage Commodity producer Service ",
                                     MessageBoxButton.OK, MessageBoxImage.Information);
                    SendNavigationRequestMessage(new Uri("/Views/Admin/Farmactivities/Services/ListServices.xaml", UriKind.Relative));
                }
                else 
                {
                    MessageBox.Show(response.ErrorInfo, "Agrimangr: Manage Commodity producer Service ",
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

        public const string CostPropertyName = "Cost";
        private decimal _cost = 0m;
        public decimal Cost
        {
            get
            {
                return _cost;
            }

            set
            {
                if (_cost == value)
                {
                    return;
                }

                RaisePropertyChanging(CostPropertyName);
                _cost = value;
                RaisePropertyChanged(CostPropertyName);
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
