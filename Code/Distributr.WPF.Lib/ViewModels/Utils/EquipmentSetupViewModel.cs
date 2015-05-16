using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using Distributr.Core.Domain.Master.EquipmentEntities;
using GalaSoft.MvvmLight.Command;
using System.Xml.Linq;

namespace Distributr.WPF.Lib.ViewModels.Utils
{
    public class EquipmentSetupViewModel : DistributrViewModelBase
    {

        public EquipmentSetupViewModel()
        {
            SaveCommand = new RelayCommand (DoSave);
            GetWeighScaleByIdCommand = new RelayCommand(DoLoadDeviceById);
            DeleteEqupmentCommand = new RelayCommand (DeleteDevice);
            WeighScales = new ObservableCollection<LocalEquipmentConfig>();
            LoadDeviceCommand = new RelayCommand(LoadDevices);
            CancelCommand = new RelayCommand(CancelSetUp);
        }

        private void CancelSetUp()
        {
            RequestClose(this, EventArgs.Empty);
        }


        public void SetUp()
        {
            EquipmentId = Guid.Empty;
            Name = string.Empty;
            Parity = 0;
            DataBits = 8;
            Model = string.Empty;
            BaudRate = 9600;
            Port = string.Empty;
        }

        #region properites

        public ObservableCollection<LocalEquipmentConfig> WeighScales { get; set; }
        
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public RelayCommand GetWeighScaleByIdCommand { get; set; }
        public RelayCommand DeleteEqupmentCommand { get; set; }
        public RelayCommand LoadDeviceCommand { get; set; }
        public event EventHandler RequestClose = (s, e) => { };
        

        public bool IsValid = true;

        public List<EquipmentType> EquipmentTypes
        {
            get { return GetDeviceTypes(); }
        }

        public bool IsSaveActionSuccess = false;
        public bool IsDeleted = false;

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
        public const string EquipmentIdPropertyName = "EquipmentId";
        private Guid _equipmentId = Guid.Empty;
        public Guid EquipmentId
        {
            get { return _equipmentId; }

            set
            {
                if (_equipmentId == value)
                {
                    return;
                }

                RaisePropertyChanging(EquipmentIdPropertyName);
                _equipmentId = value;
                RaisePropertyChanged(EquipmentIdPropertyName);
            }
        }

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

        public const string SelectedEquipmentTypePropertyName = "SelectedEquipmentType";
        private EquipmentType _SelectedEquipmentType;

        public EquipmentType SelectedEquipmentType
        {
            get { return _SelectedEquipmentType; }

            set
            {
                if (_SelectedEquipmentType == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedEquipmentTypePropertyName);
                _SelectedEquipmentType = value;
                RaisePropertyChanged(SelectedEquipmentTypePropertyName);
            }
        }

        public const string EquipmentBaudRatePropertyName = "BaudRate";
        private int _BaudRate = 9600;

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
        private int _Parity=0;

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
        private int _StopBits=1;

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
        private int _DataBits = 8;

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
        private string _Code ="";

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

        public const string EquipmentTypePropertyName = "Type";
        private EquipmentType _Type;

        public EquipmentType Type
        {
            get { return _Type; }

            set
            {
                if (_Type == value)
                {
                    return;
                }

                RaisePropertyChanging(EquipmentTypePropertyName);
                _Type = value;
                RaisePropertyChanged(EquipmentTypePropertyName);
            }
        }

        private SerialData _serialData;

        public SerialData SerialData
        {
            get { return _serialData; }
            set { _serialData = value; }
        }

        private SerialError _serialError;

        public SerialError SerialError
        {
            get { return _serialError; }
            set { _serialError = value; }
        }

        private string _portError;

        public string PortError
        {
            get { return _portError; }
            set { _portError = value; }
        }

        #endregion

        #region methods


        private void DoLoadDeviceById()
        {
            if (EquipmentId != Guid.Empty)
            {
                var devices = from c in XElement.Load(@"ScalePrinterAssetsDB.xml").Elements("Equipment") select c;
                foreach (var device in devices)
                {
                    if (device.Element("EquipmentType").Value == SelectedEquipmentType.ToString() &&
                        device.Element("Id").Value == EquipmentId.ToString())
                    {
                        Code = device.Element("Code").Value;
                        Name = device.Element("Name").Value;
                        Port = device.Element("Port").Value;
                        Parity = Int32.Parse(device.Element("Parity").Value);
                        SelectedEquipmentType =
                            (EquipmentType)
                            Enum.Parse(typeof (EquipmentType), device.Element("EquipmentType").Value, true);
                        BaudRate = Int32.Parse(device.Element("BaudRate").Value);
                        DataBits = Int32.Parse(device.Element("DataBits").Value);
                        Model = device.Element("Model").Value;
                        EquipmentId = Guid.Parse(device.Element("Id").Value);


                    }
                }
            }

        }

        private void LoadDevices()
        {
            // Execute the query using the LINQ to XML
            var devices = from c in XElement.Load(@"ScalePrinterAssetsDB.xml").Elements("Equipment") select c;
            foreach (var device in devices)
            {
                if (device.Element("EquipmentType").Value == SelectedEquipmentType.ToString())
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
                        Model =device.Element("Model").Value,
                        Id = Guid.Parse(device.Element("Id").Value)
                    };

                    WeighScales.Add(scale);
                }

            }
        }

        private List<EquipmentType> GetDeviceTypes()
        {
          var  types = new List<EquipmentType>();
            types.Add(EquipmentType.WeighingScale);
            types.Add(EquipmentType.Printer);
            return types;

        }
        private void  DeleteDevice()
        {
            XDocument docs = XDocument.Load(@"ScalePrinterAssetsDB.xml");

            foreach (var item in docs.Descendants("Equipment"))
            {
                if (item.Element("Id").Value == SelectedLocalEquipment.Id.ToString())
                {
                    item.Remove();
                    docs.Save(@"ScalePrinterAssetsDB.xml");
                    IsDeleted = true;
                }
            }
           
        }

        private void DoSave()
        {

            bool isUpdate = false;
            ValidateProperties();
            if (IsValid)
            {

                XDocument docs = XDocument.Load(@"ScalePrinterAssetsDB.xml");
                foreach (var item in docs.Descendants("Equipment"))
                {
                    if (item.Element("Id").Value == EquipmentId.ToString())
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
                    EquipmentId = Guid.NewGuid();
                    XmlElement newEquipment = xmlDoc.CreateElement("Equipment");

                    XmlElement _code = xmlDoc.CreateElement("Code");
                    _code.InnerText = SelectedEquipmentType == EquipmentType.WeighingScale
                                          ? Code = "WeighScale"
                                          : Code = "Printer";
                    newEquipment.AppendChild(_code);

                    XmlElement name = xmlDoc.CreateElement("Name");
                    name.InnerText = Name;
                    newEquipment.AppendChild(name);

                    XmlElement parity = xmlDoc.CreateElement("Parity");
                    parity.InnerText = Parity.ToString();
                    newEquipment.AppendChild(parity);

                    XmlElement port = xmlDoc.CreateElement("Port");
                    port.InnerText = Port;
                    newEquipment.AppendChild(port);

                    XmlElement baudRate = xmlDoc.CreateElement("BaudRate");
                    baudRate.InnerText = BaudRate.ToString();
                    newEquipment.AppendChild(baudRate);

                    XmlElement databits = xmlDoc.CreateElement("DataBits");
                    databits.InnerText = DataBits.ToString();
                    newEquipment.AppendChild(databits);

                    XmlElement equipmentType = xmlDoc.CreateElement("EquipmentType");
                    equipmentType.InnerText = SelectedEquipmentType.ToString();
                    newEquipment.AppendChild(equipmentType);

                    XmlElement model = xmlDoc.CreateElement("Model");
                    model.InnerText = Model;
                    newEquipment.AppendChild(model);

                    XmlElement Id = xmlDoc.CreateElement("Id");
                    Id.InnerText = EquipmentId.ToString();
                    newEquipment.AppendChild(Id);

                    xmlDoc.DocumentElement.InsertAfter(newEquipment, xmlDoc.DocumentElement.LastChild);

                    FileStream fsxml = new FileStream(@"ScalePrinterAssetsDB.xml", FileMode.Truncate, FileAccess.Write,
                                                      FileShare.ReadWrite);
                    xmlDoc.Save(fsxml);
                    fsxml.Close();
                    IsSaveActionSuccess = true;
                }
                else
                {

                    foreach (var item in docs.Descendants("Equipment"))
                    {
                        item.Element("Code").SetValue(Code);
                        item.Element("Name").SetValue(Name);
                        item.Element("Parity").SetValue(Parity);
                        item.Element("BaudRate").SetValue(BaudRate);
                        item.Element("Port").SetValue(Port);
                        item.Element("DataBits").SetValue(DataBits);
                        item.Element("EquipmentType").SetValue(SelectedEquipmentType.ToString());
                        item.Element("Model").SetValue(Model);
                        item.Element("Id").SetValue(EquipmentId);
                        docs.Save(@"ScalePrinterAssetsDB.xml");
                        IsSaveActionSuccess = true;

                    }
                }
                RequestClose(this, EventArgs.Empty);
            }
        }

        private void ValidateProperties()
        {
            MessageBoxWrapper wrapper = new MessageBoxWrapper();
            if (SelectedEquipmentType == 0)
            {
                IsValid = false;
                wrapper.Show("Device Type Not selected", "Weigh Scale", MessageBoxButton.OK);
                
             
            }
            else if (!Port.ToLower().StartsWith("com"))
            {
                IsValid = false;
                wrapper.Show("Port must start with Com", "Weigh Scale", MessageBoxButton.OK);
                // throw new ArgumentException("Port must start with Com");
            }
            else
            {
                IsValid = true;
            }
               
            
        }

        #endregion

        
    }
    
    [Serializable]
    public class LocalEquipmentConfig
    {
        public Guid Id { get; set; }    
        public string Code { get; set; }
        public string Name { get; set; }
        public string Parity { get; set; }
        public string BaudRate { get; set; }
        public string DataBits { get; set; }
        public string Port { get; set; }
        public string Model { get; set; }
        public string EquipmentType { get; set; }

    }
}
