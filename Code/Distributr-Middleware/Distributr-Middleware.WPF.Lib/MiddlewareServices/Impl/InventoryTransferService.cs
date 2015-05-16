using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Transactional.ThirdPartyIntegrationEntities;
using Distributr.WSAPI.Lib.Integrations;
using Distributr_Middleware.WPF.Lib.Utils;
using GalaSoft.MvvmLight.Messaging;
using StructureMap;

namespace Distributr_Middleware.WPF.Lib.MiddlewareServices.Impl
{
    public class InventoryTransferService : MiddleWareBase, IInventoryTransferService
    {
       private string _userName;
        private string _password;
        private IntegrationModule module;
        
        public InventoryTransferService()
        {
            _userName = CredentialsManager.GetUserName();
            _password = CredentialsManager.GetPassword();
            try
            {
                module = CredentialsManager.GetIntegrator();
            }catch(Exception)
            {
                module=IntegrationModule.Other;
            }
           
        }
        public async Task<List<string>> GetAcknowledgements(DateTime date)
        {
            return await Task.Run(async() =>
                                {
                                    try
                                    {
                                        var httpClient = MiddlewareHttpClient;
                                        string _url = MiddlewareHttpClient.BaseAddress +
    "api/Integrations/GetInventoryAcknowledgements?username={0}&password={1}&integrationModule={2}&date={3}";

                                        string url = string.Format(_url, _userName, _otherUtilities.MD5Hash(_password), module, DateTime.Now);
                                        FileUtility.LogCommandActivity(string.Format("Getting inventory acknowledgements"));
                                        var response =
                                            await httpClient.GetAsync(url);
                                        response.EnsureSuccessStatusCode();
                                        var data = await response.Content.ReadAsAsync<InventoryImportAcknowledgment>();
                                        return data.ImportedDocumentRefs;
                                    }catch(Exception ex)
                                    {
                                        return null;

                                    }
                                    ;
                                });
        } 
        public async Task<IntegrationResponse> UploadInventory(InventoryTransferDTO file)
        {


            Task<IntegrationResponse> res = await Task.Factory.StartNew(async () =>
                                                           {
                                                               IntegrationResponse tresponse=new IntegrationResponse();
                                                               
                                                               var httpClient = MiddlewareHttpClient;
                                                               string _url = MiddlewareHttpClient.BaseAddress +
                          "api/Integrations/InventoryTransfer?username={0}&password={1}&integrationModule={2}";

                                                               string url = string.Format(_url, _userName, _otherUtilities.MD5Hash(_password), module);
                                                          
                                                               try
                                                               {
                                                                   file.Credentials.ApiUserName = _userName;
                                                                   file.Credentials.Password =
                                                                       _otherUtilities.MD5Hash(_password);
                                                                   file.Credentials.IntegrationModule = module;
                                                                   FileUtility.LogCommandActivity(string.Format("Posting inventory  {0} files to {1}", file.SalesmanInventoryList.Count, MiddlewareHttpClient.BaseAddress));
                                                                   var response =
                                                                       await httpClient.PostAsJsonAsync(url, file);
                                                                   response.EnsureSuccessStatusCode();
                                                                   tresponse = await response.Content.ReadAsAsync<IntegrationResponse>();

                                                               }
                                                               catch (Exception ex)
                                                               {
                                                                   FileUtility.LogError(ex.Message);
                                                                   MessageBox.Show("Server Error Details\n" + ex.Message);

                                                               }
                                                               return tresponse;
                                                           });
            return res.Result;

        }

        public List<ImportItemModel> Import(string directoryPath)
        {
            string tempFolder = string.Empty;
            var dataModels = new List<ImportItemModel>();
            try
            {
                tempFolder =
                   Path.Combine(FileUtility.GetApplicationTempFolder(),
                                Path.GetFileName(directoryPath));

                if (File.Exists(tempFolder))
                    File.Delete(tempFolder);
                File.Copy(directoryPath, tempFolder);


                using (var parser =new Microsoft.VisualBasic.FileIO.TextFieldParser(tempFolder))
                {
                    parser.SetDelimiters(",");
                    while (!parser.EndOfData)
                    {
                        string[] currentRow = parser.ReadFields();
                        if (currentRow != null && currentRow.Length > 1)
                        {
                            dataModels.Add(
                                new ImportItemModel
                                    {
                                        ProductCode = currentRow.ElementAtOrDefault(0),
                                        Quantity = currentRow.ElementAtOrDefault(1),
                                        SalesmanCode = (currentRow.ElementAtOrDefault(2) != null && !string.IsNullOrEmpty(currentRow[2])) ? currentRow.ElementAtOrDefault(2) : "default",
                                        DocumentRef = currentRow.ElementAtOrDefault(3)
                                    });


                        }
                    }
                   
                }
                return dataModels;
            }
            catch (IOException ex)
            {
                MessageBox.Show("File Load Error\n" + ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error\n" + ex.Message);
                return null;
            }
            finally
            {
                if (File.Exists(tempFolder))
                    File.Delete(tempFolder);
            }
        }

       
    }
  
    
}
