using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Distributr.Core.Domain.Master;
using NUnit.Framework;

namespace Distributr_Middleware.WPF.Lib.Utils
{
    public class MasterDataSyncConfiguration
    {
        private static string foldername = @"config\";
        private static string filename = foldername + "MasterDataSyncConfiguration.vcda";

        static XmlSerializer xs;

     
        static MasterDataSyncConfiguration()
        {
            xs = new XmlSerializer(typeof(MasterDataSyncConfiguration));
        }
        public List<MasterDataSyncItem> Item { get; set; }


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
        
        public static MasterDataSyncConfiguration Load()
        {
            if (!IsConfigured())
            {
                return null;
            }

            using (StreamReader sw = new StreamReader(filename))
            {
                return xs.Deserialize(sw) as MasterDataSyncConfiguration;
            }
        }
        public static bool IsConfigured()
        {

            return File.Exists(filename);

        }

        public class MasterDataSyncItem
        {
            public DateTime LastSyncDateTime { get; set; }
            public MasterDataCollective Collective { get; set; }

        }
    }
}