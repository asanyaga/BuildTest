using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.FarmActivities;
using Distributr.Core.Repository.Master.Agrimanagr;
using Distributr.WPF.Lib.Messages;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Admin.FarmActivities.Infections
{
    public class InfectionsViewModel:DistributrViewModelBase
    {
        private IDistributorServiceProxy _proxy;

        public InfectionsViewModel()
        {
            LoadPageCommand = new RelayCommand(Load);
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
            InfectionTypeList = new ObservableCollection<InfectionType>();
        }

       

        #region Class Members
        public RelayCommand LoadPageCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public ObservableCollection<InfectionType> InfectionTypeList { get; set; }
        #endregion
        

        #region Methods

        private void Load()
        {
            LoadInfectionsTypes();
            ClearViewModel();
            if (!IsEdit)
            {
                Id = Guid.NewGuid();
                PageTitle = "Add Infection";
            }
            else
            {
               PageTitle = "Edit Infection";
               LoadForEdit();
                IsEdit = false;
            }
                
        }

        private void ClearViewModel()
        {
            Code = "";
            Name = "";
            //SelectedInfectionType = InfectionType.Default;
            SelectedInfectionType = InfectionType.Disease;
            Description = "";
        }

        private void LoadForEdit()
        {
            PageTitle = "Edit Infection";

            using(var c=NestedContainer)
            {
                var editInfection = Using<IInfectionRepository>(c).GetById(Id);

                if(editInfection!=null)
                {
                    Code = editInfection.Code;
                    Name = editInfection.Name;
                    SelectedInfectionType = InfectionType.Default;
                    SelectedInfectionType = InfectionTypeList.FirstOrDefault(i => i == editInfection.InfectionType);
                    Description = editInfection.Description;
                }
            }
           
        }

        private void LoadInfectionsTypes()
        {
            InfectionTypeList.Clear();
            Type _enumType = typeof(InfectionType);
            FieldInfo[] infos;
            infos = _enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
          
            foreach (FieldInfo fi in infos)
                InfectionTypeList.Add((InfectionType)Enum.Parse(_enumType, fi.Name, true));

            InfectionTypeList.Remove(InfectionType.Default);
        }

        public void EditInfection(EditInfectionsMessage messageFrom)
        {
            Id = messageFrom.InfectionId;
            IsEdit = true;
        }

        private void Cancel()
        {
            if (MessageBox.Show("Unsaved Changes will be lost. Do you want to continue?", "Agrimangr: " + PageTitle,
                               MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
            {
                ClearViewModel();
                SendNavigationRequestMessage(new Uri("/Views/Admin/FarmActivities/Infections/ListInfection.xaml", UriKind.Relative));
            }
                
        }

        private async void Save()
        {
            using (var c = NestedContainer)
            {
                Infection infection = Using<IInfectionRepository>(c).GetById(Id);;
            
                if (infection==null)
                {
                    infection = new Infection(Id);
                }
                infection.Code = Code;
                infection.Name = Name;
                infection.InfectionType = SelectedInfectionType;
                infection.Description = Description;
                infection._Status = EntityStatus.Active;
                infection._DateCreated = DateTime.Now;
                var response =await Using<IDistributorServiceProxy>(c).InfectionSaveAsync(infection);

                
                if(response.Success)
                {
                    MessageBox.Show("Infection Successfully Added", "Agrimangr: Manage Infection ",
                                   MessageBoxButton.OK, MessageBoxImage.Information);
                    SendNavigationRequestMessage(new Uri("/Views/Admin/FarmActivities/Infections/ListInfection.xaml", UriKind.Relative));
                }
                else
                {

                    MessageBox.Show(response.ErrorInfo, "Agrimangr: Manage Infection ",
                                   MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }

        }

        #endregion

        #region Properties

        
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

        
        public const string SelectedInfectionTypePropertyName = "SelectedInfectionType";
        private InfectionType _selectedInfectionType =InfectionType.Disease;
        public InfectionType SelectedInfectionType
        {
            get
            {
                return _selectedInfectionType;
            }

            set
            {
                if (_selectedInfectionType == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedInfectionTypePropertyName);
                _selectedInfectionType = value;
                RaisePropertyChanged(SelectedInfectionTypePropertyName);
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

        #endregion
    }
}
