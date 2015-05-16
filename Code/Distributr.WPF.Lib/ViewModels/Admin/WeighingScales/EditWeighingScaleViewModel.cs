using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.EquipmentEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.EquipmentRepository;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using Distributr.WPF.Lib.ViewModels.Utils;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Admin.WeighingScales
{
    public class EditWeighingScaleViewModel : EquipmentViewModelBase
    {
        public EditWeighingScaleViewModel()
        {
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);

           
        }

        #region properties
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }

        /// <summary>
        /// The <see cref="Name" /> property's name.
        /// </summary>
        public const string NamePropertyName = "Name";

        private string  _name = "";

        /// <summary>
        /// Sets and gets the Name property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string  Name
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


        /// <summary>
        /// The <see cref="Code" /> property's name.
        /// </summary>
        public const string CodePropertyName = "Code";

        private string _code = "";

        /// <summary>
        /// Sets and gets the Code property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
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


        /// <summary>
        /// The <see cref="WeighScaleNumber" /> property's name.
        /// </summary>
        public const string WeighScaleNumberPropertyName = "WeighScaleNumber";

        private string _weighScaleNumber = "";

        /// <summary>
        /// Sets and gets the WeighScaleNumber property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string  WeighScaleNumber
        {
            get
            {
                return _weighScaleNumber;
            }

            set
            {
                if (_weighScaleNumber == value)
                {
                    return;
                }

                RaisePropertyChanging(WeighScaleNumberPropertyName);
                _weighScaleNumber = value;
                RaisePropertyChanged(WeighScaleNumberPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="Make" /> property's name.
        /// </summary>
        public const string MakePropertyName = "Make";

        private string _make = "";

        /// <summary>
        /// Sets and gets the Make property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Make
        {
            get
            {
                return _make;
            }

            set
            {
                if (_make == value)
                {
                    return;
                }

                RaisePropertyChanging(MakePropertyName);
                _make = value;
                RaisePropertyChanged(MakePropertyName);
            }
        }


        /// <summary>
        /// The <see cref="WeighScaleModel" /> property's name.
        /// </summary>
        public const string WeighScaleModelPropertyName = "WeighScaleModel";

        private string _weighScaleModel = "";

        /// <summary>
        /// Sets and gets the WeighScaleModel property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string WeighScaleModel
        {
            get
            {
                return _weighScaleModel;
            }

            set
            {
                if (_weighScaleModel == value)
                {
                    return;
                }

                RaisePropertyChanging(WeighScaleModelPropertyName);
                _weighScaleModel = value;
                RaisePropertyChanged(WeighScaleModelPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="Description" /> property's name.
        /// </summary>
        public const string DescriptionPropertyName = "Description";

        private string _description = "";

        /// <summary>
        /// Sets and gets the Description property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
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


        public const string WeighingScalePropertyName = "WeighingScale";
        private WeighScale _weighingScale = null;
        public WeighScale WeighingScale
        {
            get { return _weighingScale; }

            set
            {
                if (_weighingScale == value)
                {
                    return;
                }

                RaisePropertyChanging(WeighingScalePropertyName);
                _weighingScale = value;
                RaisePropertyChanged(WeighingScalePropertyName);
            }
        }

        public const string PageTitlePropertyName = "PageTitle";
        private string _pageTitle = "Edit Weighing Scale";

        public string PageTitle
        {
            get { return _pageTitle; }

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

        #endregion

        #region methods

        protected override void LoadPage(Page page)
        {
            Guid scaleId = PresentationUtility.ParseIdFromUrl(page.NavigationService.CurrentSource);
            using (var c = NestedContainer)
            {
                if (scaleId == Guid.Empty)
                {
                    PageTitle = "Create New Weighing Scale";
                    WeighingScale = null;
                    
                }
                else
                {
                    PageTitle = "Edit Weighing Scale";
                    var scale = Using<IEquipmentRepository>(c).GetById(scaleId) as WeighScale;
                    WeighingScale = scale.DeepClone();
                }
                Setup();
              LocalEquipmentConfig config=LoadDeviceLocalConfigSettings(WeighingScale);
                if(config!=null)
                {
                    SelectedBaudRateOption =Int32.Parse(config.BaudRate);
                    SelectedDataBitsOption = Int32.Parse(config.DataBits);
                    SelectedParityBitsOption = (Parity)Enum.Parse(typeof(Parity), config.Parity);
                }
            }
        }


        private void Setup()
        {
            Clear();
            if (WeighingScale != null)
            {
                Code = WeighingScale.Code;
                Name = WeighingScale.Name;
                WeighScaleModel = WeighingScale.Model;
                Make = WeighingScale.Make;
                WeighScaleNumber = WeighingScale.EquipmentNumber;
                Description = WeighingScale.Description;
            }
            
            
        }

        private void Clear()
        {
            Code = "";
            Name = "";
            WeighScaleModel = "";
            Make = "";
            WeighScaleNumber = "";
            Description = ""; 
        }


        private async void Save()
        {
            using (var c = NestedContainer)
            {
                if (!IsValid()) return;
           
               IDistributorServiceProxy proxy = Using<IDistributorServiceProxy>(c);
                ResponseBool response = null;

                if (WeighingScale == null)
                {
                    WeighingScale = new WeighScale(Guid.NewGuid());
                    WeighingScale.CostCentre =
                        Using<ICostCentreRepository>(c).GetById(GetConfigParams().CostCentreId) as Hub;
                }

                WeighingScale.Name = Name;
                WeighingScale.Code = Code;
                WeighingScale.EquipmentNumber = WeighScaleNumber;
                WeighingScale.Make = Make;
                WeighingScale.Model = WeighScaleModel;
                WeighingScale.Description = Description;
                WeighingScale.EquipmentType = EquipmentType.WeighingScale;
                    

                

                response = await proxy.EquipmentAddAsync(WeighingScale);
                
                MessageBox.Show(response.ErrorInfo, "Distributr: Add/ Edit Weighing Scale", MessageBoxButton.OK,
                                MessageBoxImage.Information);

                if (response.Success)
                {
                    SendNavigationRequestMessage(
                        new Uri("views/admin/weighingscales/listweighingscales.xaml", UriKind.Relative));
                }
            }
        }

        private void Cancel()
        {
            if (
               MessageBox.Show("Unsaved changes will be lost. Do you want to continue?",
                               "Agrimanagr: Edit weighing scale details", MessageBoxButton.YesNo,
                               MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
            {
                SendNavigationRequestMessage(new Uri("views/admin/weighingscales/listweighingscales.xaml",
                                                     UriKind.Relative));
            }
        }

        #endregion

    }
}
