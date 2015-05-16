using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Distributr.Core.Domain.Master.EquipmentEntities;
using Distributr.Core.Domain.Transactional.ThirdPartyIntegrationEntities;

namespace Distributr_Middleware.WPF.Lib.Utils
{
   public  class CredentialsManager
   {
       private  static string userName;
       private static string password;
       private static string module;
      
       public void  SetCredentials(string usrname,string passw,IntegrationModule mod)
       {
           userName = usrname;
           password = passw;
           module = mod.ToString();
       }
       public  static string GetUserName()
       {
           return userName;
       }

       public static string GetPassword()
       {
           return password;
       }
       public static IntegrationModule GetIntegrator()
       {
          if(string.IsNullOrEmpty(module))
           {
             var val=  ConfigurationManager.AppSettings["integrator"];
              if(string.IsNullOrEmpty(val))
              {
                  throw new ArgumentNullException("Set Integation module on app config");
                 
              }
               IntegrationModule mod;

               if (Enum.TryParse(val,true, out mod))
                   return mod;

           }
          return (IntegrationModule)Enum.Parse(typeof(IntegrationModule), module);
       }

       public static void SetIntegrator(IntegrationModule mod)
       {
           module = mod.ToString();
       }
       public static void SetPassword(string pass)
       {
            password=pass;
       }
       public static void SetUserName(string name)
       {
           userName = name;
       }



       internal static void SetUserNameAndPassword(string username, string pass)
       {
           userName = username;
           password = pass;
       }
       public static SAPSettings GetSAPSettings()
       {
           return LoadSapSettings();
       }
       public static void StoreSapCredentials(SAPSettings settings)
       {
           try
           {
               SaveSettings(settings);
           }catch
           {
               
           }
       }
       public static void DeleteCredentials()
       {
           try
           {
               SaveSettings(new SAPSettings()
                                {
                                    ServerName = "",
                                    CompanyName = "",
                                    DbPassword = "",
                                    Servertype = "",
                                    Password = "",
                                    Dbusrname = "",
                                    UserName = ""
                                });
           }
           catch
           {

           }
       }

       #region Methods

       static SAPSettings LoadSapSettings()
       {
           // Read from XML
           SAPSettings rx;

           XmlSerializer reader = new XmlSerializer(typeof(SAPSettings));
           using (FileStream input = File.OpenRead(DbName))
           {
               rx = reader.Deserialize(input) as SAPSettings;
           }
           return rx;
          
       }

       static void SaveSettings(SAPSettings settings)
       {
       
           // Write to XML
           XmlSerializer writer = new XmlSerializer(typeof(SAPSettings));
           using (FileStream file = File.OpenWrite(DbName))
           {
               if (File.Exists(DbName))
                   File.Delete(DbName);
               writer.Serialize(file, settings);
           }

           

       }

       
       private static string DbName
       {
           get
           {
               return @"assets/AppSettings.xml";
           }

       }

       #endregion
        
   }
    [Serializable()]
    public class SAPSettings
    {
        public string UserName { get; set; }
        public string Password;
        public string Dbusrname { get; set; }
        public string DbPassword { get; set; }
        public string CompanyName { get; set; }
        public string Servertype { get; set; }
        public string ServerName { get; set; }
    }
}
