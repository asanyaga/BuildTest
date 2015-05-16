using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Reflection;
using System.Windows;
using System.Windows.Documents;
using Distributr.Core.Domain.Master.EquipmentEntities;
using Distributr.Core.Utility;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using Distributr.WPF.Lib.ViewModels.Utils;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.ApplicSettings
{
    public class WeighScaleDefaultViewModel : DistributrViewModelBase{

    
    public  RelayCommand LoadCommand { get; set; }
        public WeighScaleDefaultViewModel()
    {
        LoadCommand = new RelayCommand(Load);
        SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
         
            SetUp();
    }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        private void Load()
        {
            var configuration = WeighConfiguration.Load();
            if (configuration != null)
            {
                Name = configuration.Name;
                Port = configuration.Port;
                DataBits = Convert.ToInt32(configuration.DataBits.ToString());
                ParityBits = configuration.Parity;
                BaudRate = Convert.ToInt32(configuration.BaudRate.ToString());
                ScaleType=configuration.WeighScaleType;
               

            }
           
        }


         public List<Parity> ParityBitsOptions { get; set; }
       public List<Int32> BaudRateOptions { get; set; }
       public List<Int32> DataBitsOptions { get; set; }
        public ObservableCollection<WeighScaleType> ScaleTypeList { get; set; }

        private void SetUp()
        {
            DataBitsOptions = new List<int> { 7, 8, 9 };
            ParityBitsOptions = new List<Parity> { Parity.None, Parity.Even, Parity.Odd, Parity.Space, Parity.Mark };
            BaudRateOptions = new List<Int32> { 300, 1200, 2400, 4800, 9600, 19200, 38400, 57600 };
            ScaleTypeList = new ObservableCollection<WeighScaleType>();

            LoadScaleTypes();
        }

        private void LoadScaleTypes()
        {
           ScaleTypeList.Clear();
            Type _enumType = typeof(WeighScaleType);
            FieldInfo[] infos;
            infos = _enumType.GetFields(BindingFlags.Public | BindingFlags.Static);

            foreach (FieldInfo fi in infos)
                ScaleTypeList.Add((WeighScaleType)Enum.Parse(_enumType, fi.Name, true));

        }

        private async void Save()
        {
            using (var c = NestedContainer)
            {
                if (!IsValid()) return;
                IDistributorServiceProxy proxy = Using<IDistributorServiceProxy>(c);
              //  ResponseBool response = null;

                var configuration =  WeighConfiguration.Load();
                if (configuration == null)
                    configuration = new WeighConfiguration();


               
                
                    configuration.Name = Name;
                    configuration.Port = Port;
                    configuration.DataBits = DataBits;
                    configuration.Parity = ParityBits;
                    configuration.BaudRate = BaudRate;
                    configuration.WeighScaleType = ScaleType;
                
                    configuration.Save();
                SerialPortHelper.Close();
             if( MessageBox.Show("Do you want to Save the Configurations", "Agrimanagr " , MessageBoxButton.YesNo,
                                       MessageBoxImage.Question)== MessageBoxResult.Yes)
                                         {
                SendNavigationRequestMessage(new Uri("views/admin/weighingscales/listweighingscales.xaml",
                                                     UriKind.Relative));
            };  
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

        
        public const string PortPropertyName = "Port";
        private string _port = "";
        public string Port
        {
            get
            {
                return _port;
            }

            set
            {
                if (_port == value)
                {
                    return;
                }

                RaisePropertyChanging(PortPropertyName);
                _port = value;
                RaisePropertyChanged(PortPropertyName);
            }
        }

      
        public const string DataBitsPropertyName = "DataBits";
        private int _databits = 8;
        public int DataBits
        {
            get
            {
                return _databits;
            }

            set
            {
                if (_databits == value)
                {
                    return;
                }

                RaisePropertyChanging(DataBitsPropertyName);
                _databits = value;
                RaisePropertyChanged(DataBitsPropertyName);
            }
        }

        public const string ParityBitsPropertyName = "ParityBits";
        private Parity _paritybits = 0;
        public Parity ParityBits
        {
            get
            {
                return _paritybits;
            }

            set
            {
                if (_paritybits == value)
                {
                    return;
                }

                RaisePropertyChanging(ParityBitsPropertyName);
                _paritybits = value;
                RaisePropertyChanged(ParityBitsPropertyName);
            }
        }

        public const string BaudRatePropertyName = "BaudRate";
        private int _baudrate = 9600;
        public int BaudRate
        {
            get
            {
                return _baudrate;
            }

            set
            {
                if (_baudrate == value)
                {
                    return;
                }

                RaisePropertyChanging(BaudRatePropertyName);
                _baudrate = value;
                RaisePropertyChanged(BaudRatePropertyName);
            }
        }



        public string  ScaleTypePropertyName = "ScaleType";
        private WeighScaleType _string = WeighScaleType.Endel;
        public WeighScaleType ScaleType
        {
            get
            {
                return _string;
            }

            set
            {
                if (_string == value)
                {
                    return;
                }

                RaisePropertyChanging(ScaleTypePropertyName);
                _string = value;
                RaisePropertyChanged(ScaleTypePropertyName);
            }
        }
    }
   
}