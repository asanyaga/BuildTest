using System;
using System.IO;
using System.IO.Ports;
using System.Xml.Serialization;
using Distributr.Core.Domain.Master.EquipmentEntities;
using Distributr.Core.Utility;

namespace Distributr.WPF.Lib.ViewModels.Utils
{
    public class WeighConfiguration
    {
        private static string foldername = @"config\";
        private static string filename = foldername + "WeighConfiguration.vcda";

        static XmlSerializer xs;

        static WeighConfiguration()
        {
            xs = new XmlSerializer(typeof(WeighConfiguration));
        }
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public Parity Parity { get; set; }
        public int BaudRate { get; set; }
        public int DataBits { get; set; }
        public string Port { get; set; }
        public string Model { get; set; }
        public EquipmentType EquipmentType { get; set; }
        public WeighScaleType WeighScaleType { get; set; }
       
        public void Save()
        {
            bool isExists = System.IO.Directory.Exists(foldername);
            if (!isExists)
                System.IO.Directory.CreateDirectory(foldername);

            using (StreamWriter sw = new StreamWriter(filename))
            {
                xs.Serialize(sw, this);
            }
        }
         
        public static WeighConfiguration Load()
        {
            if (!IsConfigured())
            {
                return null;
            }

            using (StreamReader sw = new StreamReader(filename))
            {
                return xs.Deserialize(sw) as WeighConfiguration;
            }
        }
        public static bool IsConfigured()
        {

            return File.Exists(filename);

        }
    }
}