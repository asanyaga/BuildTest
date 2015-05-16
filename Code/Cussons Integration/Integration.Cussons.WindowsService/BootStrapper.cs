using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Integration.Cussons.WPF.Lib.IOC;
using StructureMap;

namespace Integration.Cussons.WindowsService
{
   public class BootStrapper
    {
       public void Init()
       {
           PzInitializer.Init();
           ObjectFactory.Configure(p => p.For<IPzCussonsService>().Use<IntegrationsService>());
         // EncryptConfigSection("appSettings"); 
       }

       private void EncryptConfigSection(string sectionKey)
       {
           Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
           ConfigurationSection section = config.GetSection(sectionKey);
           if (section != null)
           {
               if (!section.SectionInformation.IsProtected)
               {
                   if (!section.ElementInformation.IsLocked)
                   {
                       section.SectionInformation.ProtectSection("DataProtectionConfigurationProvider");
                       section.SectionInformation.ForceSave = true;
                       config.Save(ConfigurationSaveMode.Full);
                   }
               }
           }
       }
    }
}
