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

namespace Distributr.WPF.Lib.ViewModels.Admin.FarmActivities.Shifts
{
    public class ShiftViewModel:DistributrViewModelBase
    {
        public ShiftViewModel()
        {
            LoadPageCommand = new RelayCommand(Load);
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
        }

        
        

        #region Class Members

        public RelayCommand LoadPageCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        #endregion

        #region Methods

        private void Load()
        {
            ClearViewModel();

           if(!IsEdit)
           {
               Id = Guid.NewGuid();
               PageTitle = "Add Shift";
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
            StartTime = DateTime.Now;
            EndTime = DateTime.Now.AddHours(1.00);
            Description = "";
        }

        private void LoadForEdit()
        {
            PageTitle = "Edit Shift";

            using(var c=NestedContainer)
            {
                var editShift = Using<IShiftRepository>(c).GetById(Id);
                if(editShift!=null)
                {
                    Code = editShift.Code;
                    Name = editShift.Name;
                    StartTime = editShift.StartTime;
                    EndTime = editShift.EndTime;
                    Description = editShift.Description;
                }
            }
        }

        public void EditShift(EditShiftMessage messageTo)
        {
            Id = messageTo.ShiftId;
            IsEdit = true;
        }

        private void Cancel()
        {
            if (MessageBox.Show("Unsaved Changes will be lost. Do you want to continue?", "Agrimangr: " + PageTitle,
                           MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
            {
                ClearViewModel();
                SendNavigationRequestMessage(new Uri("/Views/Admin/FarmActivities/Shifts/ListShifts.xaml", UriKind.Relative));
            }
                
        }

        private async void Save()
        {
            using(var c=NestedContainer)
            {
                var shift = Using<IShiftRepository>(c).GetById(Id);
                if(shift==null)
                {
                    shift = new Shift(Guid.NewGuid());
                }
                shift.Name = Name;
                shift.Code = Code;
                shift.StartTime = StartTime;
                shift.EndTime = EndTime;
                shift.Description = Description;
                shift._Status = EntityStatus.Active;

                var response = await Using<IDistributorServiceProxy>(c).ShiftSaveAsync(shift);

              
                if(response.Success)
                {
                    MessageBox.Show("Service Successfully Added", "Agrimangr: Manage Commodity producer Service ",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                    SendNavigationRequestMessage(new Uri("/Views/Admin/FarmActivities/Shifts/ListShifts.xaml", UriKind.Relative));
                }
                else
                {
                    MessageBox.Show(response.ErrorInfo, "Agrimangr: Manage Shift ",
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

        public const string StartTimePropertyName = "StartTime";
        private DateTime _startTime = DateTime.Now.AddHours(0.0);
        public DateTime StartTime
        {
            get
            {
                return _startTime;
            }

            set
            {
                if (_startTime == value)
                {
                    return;
                }

                RaisePropertyChanging(StartTimePropertyName);
                _startTime = value;
                RaisePropertyChanged(StartTimePropertyName);
            }
        }

        
        public const string EndTimePropertyName = "EndTime";
        private DateTime _endTime = DateTime.Now.AddHours(8.0);
        public DateTime EndTime
        {
            get
            {
                return _endTime;
            }

            set
            {
                if (_endTime == value)
                {
                    return;
                }

                RaisePropertyChanging(EndTimePropertyName);
                _endTime = value;
                RaisePropertyChanged(EndTimePropertyName);
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
