using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Distributr.Core.Domain.Master.EquipmentEntities;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Utils
{
  public  class ListWeingScalesViewModel : DistributrViewModelBase
    {
      public ListWeingScalesViewModel()
      {
          WeighScaleList = new ObservableCollection<LocalEquipmentConfig>();
          LoadDevicesCommand = new RelayCommand(DoLoad);
          DeletedDeviceCommand = new RelayCommand(DoDeleteDevice);
      }

     
      public ObservableCollection<LocalEquipmentConfig> WeighScaleList { get; set; }

      public RelayCommand LoadDevicesCommand { get; set; }
      public RelayCommand DeletedDeviceCommand{get; set; }

      #region Properties

      public const string SelectedLocalEquipmentPropertyName = "SelectedLocalEquipment";
      private LocalEquipmentConfig _SelectedLocalEquipment;

      public LocalEquipmentConfig SelectedLocalEquipment
      {
          get { return _SelectedLocalEquipment; }

          set
          {
              if (_SelectedLocalEquipment == value)
              {
                  return;
              }

              RaisePropertyChanging(SelectedLocalEquipmentPropertyName);
              _SelectedLocalEquipment = value;
              RaisePropertyChanged(SelectedLocalEquipmentPropertyName);
          }
      }
      public const string EquipmentNamePropertyName = "Name";
      private string _Name = "";

      public string Name
      {
          get { return _Name; }

          set
          {
              if (_Name == value)
              {
                  return;
              }

              RaisePropertyChanging(EquipmentNamePropertyName);
              _Name = value;
              RaisePropertyChanged(EquipmentNamePropertyName);
          }
      }

      public const string EquipmentBaudRatePropertyName = "BaudRate";
      private int _BaudRate;

      public int BaudRate
      {
          get { return _BaudRate; }

          set
          {
              if (_BaudRate == value)
              {
                  return;
              }

              RaisePropertyChanging(EquipmentBaudRatePropertyName);
              _BaudRate = value;
              RaisePropertyChanged(EquipmentBaudRatePropertyName);
          }
      }

      public const string EquipmentParityPropertyName = "Parity";
      private int _Parity;

      public int Parity
      {
          get { return _Parity; }

          set
          {
              if (_Parity == value)
              {
                  return;
              }

              RaisePropertyChanging(EquipmentParityPropertyName);
              _Parity = value;
              RaisePropertyChanged(EquipmentParityPropertyName);
          }
      }

      public const string EquipmentStopBitsPropertyName = "StopBits";
      private int _StopBits;

      public int StopBits
      {
          get { return _StopBits; }

          set
          {
              if (_StopBits == value)
              {
                  return;
              }

              RaisePropertyChanging(EquipmentStopBitsPropertyName);
              _StopBits = value;
              RaisePropertyChanged(EquipmentStopBitsPropertyName);
          }
      }

      public const string EquipmentDataBitsPropertyName = "DataBits";
      private int _DataBits;

      public int DataBits
      {
          get { return _DataBits; }

          set
          {
              if (_DataBits == value)
              {
                  return;
              }

              RaisePropertyChanging(EquipmentDataBitsPropertyName);
              _StopBits = value;
              RaisePropertyChanged(EquipmentDataBitsPropertyName);
          }
      }


      public const string EquipmentModelPropertyName = "Model";
      private string _Model = "";

      public string Model
      {
          get { return _Model; }

          set
          {
              if (_Model == value)
              {
                  return;
              }

              RaisePropertyChanging(EquipmentModelPropertyName);
              _Model = value;
              RaisePropertyChanged(EquipmentModelPropertyName);
          }
      }

      public const string EquipmentPortPropertyName = "Port";
      private string _Port = "";

      public string Port
      {
          get { return _Port; }

          set
          {
              if (_Port == value)
              {
                  return;
              }

              RaisePropertyChanging(EquipmentPortPropertyName);
              _Port = value;
              RaisePropertyChanged(EquipmentPortPropertyName);
          }
      }

      public const string EquipmentCodePropertyName = "Code";
      private string _Code = "";

      public string Code
      {
          get { return _Code; }

          set
          {
              if (_Code == value)
              {
                  return;
              }

              RaisePropertyChanging(EquipmentCodePropertyName);
              _Code = value;
              RaisePropertyChanged(EquipmentCodePropertyName);
          }
      }
      #endregion

      #region methods
      private void DoDeleteDevice()
      {
          var devices = XElement.Load(@"ScalePrinterAssetsDB.xml");
           var deviceToDelete=devices.Elements("Equipment").FirstOrDefault(c => c.Element("Name").Value == SelectedLocalEquipment.Name);
           if (deviceToDelete != null)
           {
               deviceToDelete.Remove();
             //  deviceToDelete = null;
               devices.Save(@"ScalePrinterAssetsDB.xml");
            
           }


      }
      private void DoLoad()
      {
          WeighScaleList.Clear();
          var devices = from c in XElement.Load(@"ScalePrinterAssetsDB.xml").Elements("Equipment") select c;
          foreach (var device in devices)
          {
              if (device.Element("EquipmentType").Value == EquipmentType.WeighingScale.ToString())
              {
                  LocalEquipmentConfig scale = new LocalEquipmentConfig
                  {
                      Code = device.Element("Code").Value,
                      Name = device.Element("Name").Value,
                      Port = device.Element("Port").Value,
                      Parity = device.Element("Parity").Value,
                      EquipmentType = device.Element("EquipmentType").Value,
                      BaudRate = device.Element("BaudRate").Value,
                      DataBits = device.Element("DataBits").Value,
                      Model = device.Element("Model").Value,
                      Id = Guid.Parse(device.Element("Id").Value)
                  };

                  WeighScaleList.Add(scale);
              }

          }
      }

      #endregion

    }
}
