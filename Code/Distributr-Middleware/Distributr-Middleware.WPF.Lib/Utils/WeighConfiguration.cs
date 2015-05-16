using System;
using System.IO;
using System.IO.Ports;
using System.Xml.Serialization;
using Distributr.Core.Domain.Master.EquipmentEntities;
using Distributr.Core.Utility;

namespace Distributr_Middleware.WPF.Lib.Utils
{
    public class InventoryConfiguration
    {
        private static string foldername = @"config\";
        private static string filename = foldername + "InventoryConfiguration.vcda";

        static XmlSerializer xs;

        static InventoryConfiguration()
        {
            xs = new XmlSerializer(typeof(InventoryConfiguration));
        }
        public DateTime LastSyncDateTime { get; set; }
        

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

        public static InventoryConfiguration Load()
        {
            if (!IsConfigured())
            {
                return null;
            }

            using (StreamReader sw = new StreamReader(filename))
            {
                return xs.Deserialize(sw) as InventoryConfiguration;
            }
        }
        public static bool IsConfigured()
        {

            return File.Exists(filename);

        }
    }
}