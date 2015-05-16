using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Utility;
using Distributr.WPF.Lib.Services.Service.Utility;

using Distributr.WSAPI.Lib.Integrations;
using Distributr_Middleware.WPF.Lib.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using StructureMap;

namespace Distributr_Middleware.WPF.Lib.MiddlewareServices.Impl
{
    public abstract class MiddleWareBase
    {
        protected MiddleWareBase()
        {
            _otherUtilities = ObjectFactory.GetInstance<IOtherUtilities>();
        }
        protected HttpClient MiddlewareHttpClient
        {
            get
            {
                string url = ConfigurationManager.AppSettings["WSURL"];
                if (string.IsNullOrEmpty(url))
                {
                    throw new ArgumentNullException("Invalid Url");
                }

                return new HttpClient { BaseAddress = new Uri(url), Timeout = TimeSpan.FromMinutes(30) };

            }
        }

        protected IOtherUtilities _otherUtilities;

        protected string TransactionsExportPath
        {
            get { return FileUtility.GetFilePath("exports"); }
        }
        
    }
    public class MasterDataImportService :MiddleWareBase, IMasterDataImportService
    {
        private List<ImportEntity> _imports;
        
      
        public MasterDataImportService()
        {
            _imports = new List<ImportEntity>();
            
        }

      

        public async Task<MasterDataValidationAndImportResponse> Upload()
        {
            string url = string.Format("api/Integrations/ImportMasterData");
            var httpClient = MiddlewareHttpClient;
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var _response = new MasterDataValidationAndImportResponse();
            try
            {
                var response = await httpClient.PostAsJsonAsync(url, _imports);
                _response = await response.Content.ReadAsAsync<MasterDataValidationAndImportResponse>();
                
            }
            catch (Exception ex)
            {
                FileUtility.LogError(ex.Message);
                //_log.Error("Failed to update salesman", ex);
            }
            return _response;
        }


        public Task<CostCentreLoginResponse> Login(string username, string password,string weburl="" ,UserType usertype=UserType.HQAdmin)
        {
            return Task.Factory.StartNew(() =>
                                             {
                                                 Uri currentBaseUrl = null;
                                                 if (!string.IsNullOrEmpty(weburl))
                                                 {
                                                     currentBaseUrl = new Uri(weburl);

                                                 }
                                                 else
                                                 {
                                                     var httpClient = MiddlewareHttpClient;
                                                     currentBaseUrl = httpClient.BaseAddress;
                                                 }

                                                 string _url = currentBaseUrl +
                                                               "Login/Login?Username={0}&Password={1}&UserType={2}";

                                                 string url = string.Format(_url, username,
                                                                            _otherUtilities.MD5Hash(password),usertype);

                                                 Uri uri = new Uri(url, UriKind.Absolute);
                                                 WebClient wc = new WebClient();

                                                 string response = wc.DownloadString(uri);
                                                 CostCentreLoginResponse _response =
                                                     JsonConvert.DeserializeObject<CostCentreLoginResponse>(response,
                                                                                                            new IsoDateTimeConverter
                                                                                                                ());

                                                 return _response;

                                             });

        }
       
        public async Task<MasterDataValidationAndImportResponse> Upload(List<ImportEntity> uploadData)
        {
            string url = string.Format("api/Integrations/ImportMasterData");
            var httpClient = MiddlewareHttpClient;
           
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var _response = new MasterDataValidationAndImportResponse();
            int batchSize = Convert.ToInt32(0.2 * uploadData.Count);
            var itemes =
                uploadData.OrderBy(p => p.MasterDataCollective).Batch(batchSize).Select(x => x.ToList()).ToList();


            while (itemes != null && itemes.Any())
            {
                var current = itemes.FirstOrDefault();
                if (current != null && current.Any())
                {
                    try
                    {

                        var response = await httpClient.PostAsJsonAsync(url, current);
                        response.EnsureSuccessStatusCode();
                        _response = await response.Content.ReadAsAsync<MasterDataValidationAndImportResponse>();

                    }
                    catch (Exception ex)
                    {
                        FileUtility.LogError(ex.Message);
                        MessageBox.Show("Server Error Details\n" + ex.Message);
                        break;
                    }
                    itemes.Remove(current);
                }

            }
            //var batches = uploadData.Count/batchSize;
            //for (int i = 0; i < batches; i++)
            //{
            //    var current = uploadData.Skip(batchSize*i).Take(batchSize);

            //    try
            //    {

            //        var response = await httpClient.PostAsJsonAsync(url, current);
            //        response.EnsureSuccessStatusCode();
            //        _response = await response.Content.ReadAsAsync<MasterDataValidationAndImportResponse>();

            //    }
            //    catch (Exception ex)
            //    {
            //        FileUtility.LogError(ex.Message);
            //        MessageBox.Show("Server Error Details\n" + ex.Message);
            //        break;
            //    }
            //}
           
            
            return _response;
        }

        public IEnumerable<ImportEntity> GetImportItems()
        {
            return _imports;
        }
        public async Task<List<ImportEntity>> ImportAsync(string directoryPath, MasterDataCollective entityType)
        {
            
            return await Task.Factory.StartNew(() =>
                                                   {
                    List<ImportEntity> data = new List<ImportEntity>();
                string tempFolder = string.Empty;
                try
                {
                    bool success = false;

                    var ext = FileUtility.GetFileExtension(directoryPath,
                                                           entityType.ToString());

                    var path = Path.Combine(directoryPath,
                                            string.Concat(entityType.ToString(),
                                                          ext));
                    tempFolder =
                       Path.Combine(FileUtility.GetApplicationTempFolder(),
                                    Path.GetFileName(path));

                    if (File.Exists(tempFolder))
                        File.Delete(tempFolder);
                    File.Copy(path, tempFolder);


                    using (
                        var parser =
                            new Microsoft.VisualBasic.FileIO.TextFieldParser(
                                tempFolder))
                    {
                        parser.SetDelimiters(",");

                        while (!parser.EndOfData)
                        {
                            string[] currentRow = parser.ReadFields();
                            if (currentRow != null && currentRow.Length > 0)
                            {
                                data.Add(new ImportEntity()
                                {
                                    Fields = currentRow,
                                    MasterDataCollective =
                                        entityType.ToString()
                                });
                            }
                        }
                        success = true;
                    }

                    if (success)
                        return data;
                }
                catch (IOException ex)
                {
                   // MessageBox.Show("File Load Error\n" + ex.Message);
                    throw ex;
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("Error\n" + ex.Message);
                    throw ex;
                }
                finally
                {
                    if (File.Exists(tempFolder))
                        File.Delete(tempFolder);
                }

                return null;
            });
        }
        public async Task<bool> Import(string directoryPath, MasterDataCollective entityType)
        {
          return  await Task.Factory.StartNew(() =>
                                                  {
                                                      string tempFolder = string.Empty;
                                                      try
                                                      {
                                                          bool success = false;

                                                          var ext = FileUtility.GetFileExtension(directoryPath,
                                                                                                 entityType.ToString());

                                                          var path = Path.Combine(directoryPath,
                                                                                  string.Concat(entityType.ToString(),
                                                                                                ext));
                                                           tempFolder =
                                                              Path.Combine(FileUtility.GetApplicationTempFolder(),
                                                                           Path.GetFileName(path));

                                                          if(File.Exists(tempFolder))
                                                              File.Delete(tempFolder);
                                                          File.Copy(path, tempFolder);


                                                          using (
                                                              var parser =
                                                                  new Microsoft.VisualBasic.FileIO.TextFieldParser(
                                                                      tempFolder))
                                                          {
                                                              parser.SetDelimiters(",");

                                                              while (!parser.EndOfData)
                                                              {
                                                                  string[] currentRow = parser.ReadFields();
                                                                  if (currentRow != null && currentRow.Length > 0)
                                                                  {
                                                                      _imports.Add(new ImportEntity()
                                                                                       {
                                                                                           Fields = currentRow,
                                                                                           MasterDataCollective =
                                                                                               entityType.ToString()
                                                                                       });
                                                                  }
                                                              }
                                                              success = true;
                                                          }


                                                          return success;
                                                      }
                                                      catch (IOException ex)
                                                      {
                                                          MessageBox.Show("File Load Error\n" + ex.Message);
                                                          return false;
                                                      }
                                                      catch(Exception ex)
                                                      {
                                                          MessageBox.Show("Error\n" + ex.Message);
                                                          return false;
                                                      }
                                                      finally
                                                      {
                                                          if (File.Exists(tempFolder))
                                                              File.Delete(tempFolder);
                                                      }

            });
        }

        
        
    }

    
}
