using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using Distributr.Core.Domain.Master.EquipmentEntities;
using Distributr.Core.Utility;
using Distributr.WPF.Lib.ViewModels.Utils;

namespace Distributr.WPF.Lib.ViewModels.Admin.WeighingScales
{
    public class EquipmentViewModelBase : DistributrViewModelBase
    {

        public EquipmentViewModelBase()
        {
            SetUp();
        }

        
        #region Device setup properties
       public List<Parity> ParityBitsOptions { get; set; }
       public List<Int32> BaudRateOptions { get; set; }
       public List<Int32> DataBitsOptions { get; set; }
        public ObservableCollection<WeighScaleType> ScaleTypeList { get; set; }
       
       private void SetUp()
       {
           DataBitsOptions = new List<int> { 7, 8, 9 };
           ParityBitsOptions = new List<Parity> { Parity.None, Parity.Even, Parity.Odd, Parity.Space, Parity.Mark };
           BaudRateOptions = new List<Int32> { 300, 1200, 2400, 4800, 9600, 19200, 38400, 57600 };
           ScaleTypeList=new ObservableCollection<WeighScaleType>();

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

        public const string SelectedDataBitsOptionPropertyName = "SelectedDataBitsOption";
       private int _selectedDataBitsOption = 8;
       public int SelectedDataBitsOption
       {
           get { return _selectedDataBitsOption; }

           set
           {
               if (_selectedDataBitsOption == value)
               {
                   return;
               }

               RaisePropertyChanging(SelectedDataBitsOptionPropertyName);
               _selectedDataBitsOption = value;
               RaisePropertyChanged(SelectedDataBitsOptionPropertyName);
           }
       }

       public const string SelectedBaudRateOptionPropertyName = "SelectedBaudRateOption";
       private int _selectedBaudRateOption = 9600;
       public int SelectedBaudRateOption
       {
           get { return _selectedBaudRateOption; }

           set
           {
               if (_selectedBaudRateOption == value)
               {
                   return;
               }

               RaisePropertyChanging(SelectedBaudRateOptionPropertyName);
               _selectedBaudRateOption = value;
               RaisePropertyChanged(SelectedBaudRateOptionPropertyName);
           }
       }

       public const string SelectedScaleTypePropertyName = "SelectedScaleType";
       private WeighScaleType _selectedScaleType = WeighScaleType.Endel;
       public WeighScaleType SelectedScaleType
       {
           get { return _selectedScaleType; }

           set
           {
               if (_selectedScaleType == value)
               {
                   return;
               }

               RaisePropertyChanging(SelectedScaleTypePropertyName);
               _selectedScaleType = value;
               RaisePropertyChanged(SelectedScaleTypePropertyName);
           }
       }

       public const string SelectedParityBitsOptionPropertyName = "SelectedParityBitsOption";
       private Parity _selectedParityBitsOption = Parity.None;
       public Parity SelectedParityBitsOption
       {
           get { return _selectedParityBitsOption; }

           set
           {
               if (_selectedParityBitsOption == value)
               {
                   return;
               }

               RaisePropertyChanging(SelectedParityBitsOptionPropertyName);
               _selectedParityBitsOption = value;
               RaisePropertyChanged(SelectedParityBitsOptionPropertyName);
           }
       }
       public const string PortPropertyName = "SelectedParityBitsOption";
       private string _port = "";
       public string Port
       {
           get { return _port; }

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
       #endregion

        #region methods

       internal LocalEquipmentConfig LoadDeviceLocalConfigSettings(Equipment equipment)
        {
            var devices = from c in XElement.Load(@"ScalePrinterAssetsDB.xml").Elements("Equipment") select c;
            var scaleSettings = devices.FirstOrDefault(n => Guid.Parse(n.Element("Id").Value) == equipment.Id);
            if (scaleSettings != null)
            {
               return new LocalEquipmentConfig
                                                {
                                                    Code = scaleSettings.Element("Code").Value,
                                                    Name = scaleSettings.Element("Name").Value,
                                                    Port = scaleSettings.Element("Port").Value,
                                                    Parity = scaleSettings.Element("Parity").Value,
                                                    EquipmentType = scaleSettings.Element("EquipmentType").Value,
                                                    BaudRate = scaleSettings.Element("BaudRate").Value,
                                                    DataBits = scaleSettings.Element("DataBits").Value,
                                                    Model = scaleSettings.Element("Model").Value,
                                                    Id = Guid.Parse(scaleSettings.Element("Id").Value)
                                                };
               
            }
           return null;
        }

        internal bool SaveDeviceLocalConfigSettings(Equipment equipment)
        {
            bool isUpdate = false;
            if (ValidateProperties())
            {

                XDocument docs = XDocument.Load(@"ScalePrinterAssetsDB.xml");
                foreach (var item in docs.Descendants("Equipment"))
                {
                    if (item.Element("Id").Value == equipment.Id.ToString())
                    {
                        isUpdate = true;
                        break;
                    }
                }

                if (!isUpdate)
                {
                    FileStream fs = new FileStream(@"ScalePrinterAssetsDB.xml", FileMode.Open, FileAccess.Read,
                                                   FileShare.ReadWrite);
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(fs);
                    fs.Close();

                    XmlElement newEquipment = xmlDoc.CreateElement("Equipment");

                    XmlElement code = xmlDoc.CreateElement("Code");
                    code.InnerText = equipment.Code;
                    newEquipment.AppendChild(code);

                    XmlElement name = xmlDoc.CreateElement("Name");
                    name.InnerText = equipment.Name;
                    newEquipment.AppendChild(name);

                    XmlElement parity = xmlDoc.CreateElement("Parity");
                    parity.InnerText = SelectedParityBitsOption.ToString();
                    newEquipment.AppendChild(parity);

                    XmlElement port = xmlDoc.CreateElement("Port");
                    port.InnerText = Port;
                    newEquipment.AppendChild(port);

                    XmlElement baudRate = xmlDoc.CreateElement("BaudRate");
                    baudRate.InnerText = SelectedBaudRateOption.ToString();
                    newEquipment.AppendChild(baudRate);

                    XmlElement databits = xmlDoc.CreateElement("DataBits");
                    databits.InnerText = SelectedDataBitsOption.ToString();
                    newEquipment.AppendChild(databits);

                    XmlElement equipmentType = xmlDoc.CreateElement("EquipmentType");
                    equipmentType.InnerText = equipment.EquipmentType.ToString();
                    newEquipment.AppendChild(equipmentType);

                    XmlElement model = xmlDoc.CreateElement("Model");
                    model.InnerText = equipment.Model;
                    newEquipment.AppendChild(model);

                    XmlElement Id = xmlDoc.CreateElement("Id");
                    Id.InnerText = equipment.Id.ToString();
                    newEquipment.AppendChild(Id);

                    xmlDoc.DocumentElement.InsertAfter(newEquipment, xmlDoc.DocumentElement.LastChild);

                    FileStream fsxml = new FileStream(@"ScalePrinterAssetsDB.xml", FileMode.Truncate, FileAccess.Write,
                                                      FileShare.ReadWrite);
                    xmlDoc.Save(fsxml);
                    fsxml.Close();
                    return true;
                }
                else
                {
                    foreach (var item in docs.Descendants("Equipment"))
                    {
                        item.Element("Code").SetValue(equipment.Code);
                        item.Element("Name").SetValue(equipment.Name);
                        item.Element("Parity").SetValue(SelectedParityBitsOption);
                        item.Element("BaudRate").SetValue(SelectedBaudRateOption);
                        item.Element("Port").SetValue(Port);
                        item.Element("DataBits").SetValue(SelectedDataBitsOption);
                        item.Element("EquipmentType").SetValue(equipment.EquipmentType);
                        item.Element("Model").SetValue(equipment.Model);
                        item.Element("Id").SetValue(equipment.Id);
                        docs.Save(@"ScalePrinterAssetsDB.xml");

                        return true;
                    }
                }
            }
            return false;
        }

        internal bool DeleteDeviceLocalSettings(Equipment equipment)
        {
            XDocument docs = XDocument.Load(@"ScalePrinterAssetsDB.xml");

            foreach (var item in docs.Descendants("Equipment"))
            {
                if (item.Element("Id").Value == equipment.Id.ToString())
                {
                    item.Remove();
                    docs.Save(@"ScalePrinterAssetsDB.xml");
                    return true;
                }
            }
            return false;
        }

        private bool ValidateProperties()
        {
            var wrapper = new MessageBoxWrapper();
            if (!Port.ToLower().StartsWith("com"))
            {
                wrapper.Show("Port must start with Com", "Local Device Setup", MessageBoxButton.OK);
                return false;
            }
           
            return true;
        }

        #endregion

    }
}
