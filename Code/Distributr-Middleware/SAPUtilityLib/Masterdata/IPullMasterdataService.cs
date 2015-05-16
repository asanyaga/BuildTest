using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Windows;
using Distributr.Core.Utility;
using Distributr.WSAPI.Lib.Integrations;
using Distributr_Middleware.WPF.Lib.MiddlewareServices.Impl;
using Distributr_Middleware.WPF.Lib.Utils;
using GalaSoft.MvvmLight.Messaging;
using SAPUtilityLib.Masterdata.Impl;
using SAPbobsCOM;

namespace SAPUtilityLib.Masterdata
{
    public interface IPullMasterdataService
    {
        SyncBasicResponse Import(string masterData);
        bool TestSapcOnnection();
    }

    public abstract class SAPUtilsBase
    {
        private SAPSettings settings = null;
        protected string masterdataEntity;
        protected List<ImportEntity> MasterDataList = null;
        protected string connectionString;
        protected  SAPUtilsBase()
        {
            settings = CredentialsManager.GetSAPSettings();
            Init();
            connectionString = string.Format("data source={0};initial catalog={1};uid={2};pwd={3};",
                                             settings.ServerName, settings.CompanyName, settings.Dbusrname,
                                             settings.DbPassword);
            masterdataEntity = "";
            MasterDataList=new List<ImportEntity>();

        }
        int lastError = 0;
        string lastErromsg = "";
        
        private Company _company = null;
        protected Company Company
        {
           get { return _company; }
        }

     
       private void Init()
       {
           if (settings==null)
           {
               throw new ArgumentNullException("Please provide valid SAP Settings");
           }
           if(_company==null)
           {
               
               _company = new Company
                              {
                                  language = BoSuppLangs.ln_English,
                                  Server = settings.ServerName,
                                  DbServerType =BoDataServerTypes.dst_MSSQL2008,//(BoDataServerTypes)Enum.Parse(typeof(BoDataServerTypes), settings.Servertype),
                                  UseTrusted = false,
                                  DbPassword = settings.DbPassword,
                                  DbUserName = settings.Dbusrname,
                                  UserName = settings.UserName,
                                  Password = settings.Password,
                                  CompanyDB = settings.CompanyName,
                                // LicenseServer = "10.0.0.2:3000"

                              };
               ConnectToCompany();
               return;
           }
           ConnectToCompany();
           
       }
        public bool TestSapcOnnection()
        {
            if (_company == null)
                Init();
            return _company != null && _company.Connected;
        }
        bool ConnectToCompany()
        {
            try
            {
                Messenger.Default.Send<string>("Contacting SAP...");
               lastError=  _company.Connect();
               _company.GetLastError(out lastError, out lastErromsg);
                if(!string.IsNullOrEmpty(lastErromsg))
                {
                    MessageBox.Show(lastErromsg, "DI Message", MessageBoxButton.OK, MessageBoxImage.Error);
                    FileUtility.LogError(lastErromsg);
                }
                
             if(_company.Connected)
                    Messenger.Default.Send("Done contacting SAP...");
                return _company.Connected;
                
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        protected virtual SyncBasicResponse DampToFolder(Recordset rs, string filename = "")
        {
            try
            {
                if (string.IsNullOrEmpty(filename))
                    filename = FileUtility.GetSApFile(masterdataEntity);
                DumpExportFilesAsync(MapForCSVFile(MapMasterDataList(rs)).ToCsv(), filename);
                return new SyncBasicResponse{Status=true};
            }
            catch (Exception ex)
            {
                FileUtility.LogError(ex.Message);
                return new  SyncBasicResponse{Status = false,Info = ex.Message};
            }
        }
        protected virtual SyncBasicResponse DampToFolder(SqlDataReader rs, string filename = "")
        {
            try
            {
                if (string.IsNullOrEmpty(filename))
                    filename = FileUtility.GetSApFile(masterdataEntity);
                DumpExportFilesAsync(MapForCSVFile(MapMasterDataList(rs)).ToCsv(), filename);
                return new SyncBasicResponse { Status = true };
            }
            catch (Exception ex)
            {
                FileUtility.LogError(ex.Message);
                return new SyncBasicResponse { Status = false, Info = ex.Message };
            }
        }
        private IEnumerable<ImportEntity> MapMasterDataList(SqlDataReader rs)
        {
           
            while (rs.Read())
            {
                var imports = new ImportEntity();
                var list = new List<string>();
                for (int i = 0; i < rs.FieldCount; i++)
                {
                    string field = rs[i].ToString();
                    list.Add(field);
                }
              
                imports.Fields = list.ToArray();
                imports.MasterDataCollective = masterdataEntity;
                MasterDataList.Add(imports);
               
            }
            return MasterDataList;
        }
        private IEnumerable<ImportEntity> MapMasterDataList(Recordset rs)
        {
            rs.MoveFirst();
            while (!(rs.EoF))
            {
                var imports = new ImportEntity();
                var list = new List<string>();
                for (int i = 0; i < rs.Fields.Count; i++)
                {
                    string field = rs.Fields.Item(i).Value.ToString();
                    list.Add(field);
                }
                imports.Fields = list.ToArray();
                imports.MasterDataCollective = masterdataEntity;
                MasterDataList.Add(imports);
                rs.MoveNext();
            }
            return MasterDataList;
        }
        private  void DumpExportFilesAsync(string file, string selectedPath)
        {
            if (string.IsNullOrEmpty(selectedPath))
            {
                return;
            }
            try
            {
                using (var fs = new FileStream(selectedPath, FileMode.OpenOrCreate))
                {
                    fs.Close();
                    using (var wr = new StreamWriter(selectedPath, false))
                    {
                         wr.WriteLine(file);

                    }
                }


            }
            catch (IOException ex)
            {

            }
        }
        private static IEnumerable<BaseImportObject> MapForCSVFile(IEnumerable<ImportEntity> file)
        {
           
           var files= file.Select(n => n.Fields).Select(p => new BaseImportObject
            {
                Field1 = p.ElementAtOrDefault(0),
                Field2 = !string.IsNullOrEmpty(p.ElementAtOrDefault(1)) ? p.ElementAtOrDefault(1).Replace('"', ' ').Trim() : "",
                Field3 = p.ElementAtOrDefault(2),
                Field4 = p.ElementAtOrDefault(3),
                Field5 = p.ElementAtOrDefault(4),
                Field6 = p.ElementAtOrDefault(5),
                Field7 = p.ElementAtOrDefault(6),
                Field8 = p.ElementAtOrDefault(7),
                Field9 = p.ElementAtOrDefault(8),
                Field10 = p.ElementAtOrDefault(9)
            }).ToList();
            try
            {

                //var skipped = new InventoryTransferService().GetAcknowledgements(DateTime.Now).Result;
                //if(skipped !=null && skipped.Any())
                //{
                //    files.RemoveAll(n => !string.IsNullOrEmpty(n.Field4) && skipped.Contains(n.Field4));
                //}
                return files;
            }catch(Exception ex)
            {
                MessageBox.Show("Unable to contact Server,duplicate inventory transfers will not be checked", "Warning",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return files;
            }
            


        }
       
    }
}
