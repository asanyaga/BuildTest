using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Distributr.Core.Domain.Transactional.ThirdPartyIntegrationEntities;

using Distributr.WSAPI.Lib.Integrations;
using Distributr_Middleware.WPF.Lib.MiddlewareServices;
using Distributr_Middleware.WPF.Lib.Utils;

namespace PzIntegrations.Lib.MasterDataImports.Products
{
    //todo=>Merge  this to a single service with the  InventoryService in Distributr_Middleware.WPF.Lib.MiddlewareServices
    
   public class InventoryService:IInventoryService
    {
        private Dictionary<string, List<InventoryLineItemDto>> StocksPerSalesman { get; set; }
        private List<FileInfo> Files;
        private InventoryTransferDTO dto;
      
       public InventoryService()
       {
           Files = new List<FileInfo>();
           this.dto = new InventoryTransferDTO();
           StocksPerSalesman = new Dictionary<string, List<InventoryLineItemDto>>();
         
       }

       public void SetCredentials(string username, string password, IntegrationModule module = IntegrationModule.PZCussons)
       {
          CredentialsManager.SetUserName(username);
          CredentialsManager.SetPassword(password);
          CredentialsManager.SetIntegrator(module);
       }

       public async Task<bool> ImportInventoty()
       {

           return await Task.Run(async () =>
                                           {
                                               try
                                               {
                                                   var path = FileUtility.GetFilePath("stocklinepath");
                                                   FileUtility.LogCommandActivity(
                                                       string.Format("Loading inventory files from=>{0} ", path));
                                                   if (string.IsNullOrEmpty(path))
                                                   {
                                                       FileUtility.LogCommandActivity("Invalid file path,..abort.");
                                                       return false;
                                                   }
                                                   LoadDistributorInventoryFiles(path);

                                                   if ((dto.DistributorInventory != null && dto.DistributorInventory.Any()))
                                                  {
                                                       var response = await UploadInventory(dto);
                                                       if (response.Result == "Error")
                                                       {
                                                           FileUtility.LogCommandActivity(response.ResultInfo);
                                                       }
                                                       else if (response.Result == "Success")
                                                       {
                                                           await DampExported(Files.Select(n => n.FullName).ToList());
                                                           return true;
                                                       }
                                                   }
                                                   return true;
                                               }
                                               catch(Exception ex)
                                               {
                                                   FileUtility.LogCommandActivity(ex.Message);
                                                   return false;
                                               }
                                               
                                           });

       }
       private void LoadDistributorInventoryFiles(string path)
       {

           Files =FileUtility.GetStockLines(new DirectoryInfo(path)).Distinct().ToList();
           FileUtility.LogCommandActivity(DateTime.Now.ToString("hh:mm:ss") + "Generating import files");

           foreach (var fileInfo in Files)
           {
               //format=>stocline-distributorcode
               var highenSeparator = new[] {'-'};
               var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileInfo.FullName);
               if (fileNameWithoutExtension != null && fileNameWithoutExtension.StartsWith("Stockline"))
               {
                   FileUtility.LogCommandActivity(DateTime.Now.ToString("hh:mm:ss") +
                                                  string.Format(" Extracting file =>{0}", fileNameWithoutExtension));
                   var temp =
                       fileNameWithoutExtension.Split(highenSeparator, StringSplitOptions.RemoveEmptyEntries).ToList();

                   //only distributor
                   if (!string.IsNullOrEmpty(temp.ElementAtOrDefault(1)))
                   {
                       dto.DistributorCode = temp.ElementAt(1);
                   }
               }

               var dicties = new Dictionary<string, List<InventoryLineItemDto>>();
               var inventory =Import(fileInfo.FullName);

               if (inventory != null && inventory.Any())
               {
                   var grouped = inventory.GroupBy(p => p.SalesmanCode);
                  
                   foreach (var group in grouped)
                   {
                       var listofLineItems = new List<InventoryLineItemDto>();
                       foreach (var itemModel in group)
                       {
                           var qntity = 1m;
                           try
                           {
                               qntity = Convert.ToDecimal(itemModel.Quantity);
                           }
                           catch
                           {
                               qntity = 1m;
                           }
                           listofLineItems.Add(new InventoryLineItemDto()
                                                   {
                                                       ProductCode = itemModel.ProductCode,
                                                       Quantity = qntity

                                                   });

                       }
                       if (group.Key == "default" || string.IsNullOrEmpty(group.Key))
                           dto.DistributorInventory.AddRange(listofLineItems);
                       else
                       {
                           dicties.Add(group.Key, listofLineItems);
                          
                       }

                   }
               }
               if(dicties.Any())
                dto.SalesmanInventoryList.Add(dicties);


           }


       }

      
       Task DampExported(List<string> files)
       {
           return Task.Run(() =>
                               {
                                   try
                                   {
                                       var basefile = FileUtility.CreateImportedStockLineFile().FullName;
                                       foreach (string file in files.Where(File.Exists))
                                       {
                                           string destPath = Path.Combine(basefile, Path.GetFileName(file));
                                           if (File.Exists(destPath))
                                               File.Delete(destPath);
                                           File.Move(file, destPath);
                                       }

                                   }
                                   catch
                                   {
                                   }
                               });
       }


       async Task<IntegrationResponse> UploadInventory(InventoryTransferDTO file)
       {
           Task<IntegrationResponse> res = await Task.Factory.StartNew(async () =>
           {
               IntegrationResponse tresponse = new IntegrationResponse();

               var httpClient = HttpClient;
               httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
               string url = HttpClient.BaseAddress + "api/Integrations/InventoryTransfer";

              
               try
               {
                   file.Credentials = new IntegrationCredential()
                                          {
                                              Password = CredentialsManager.GetPassword(),
                                              ApiUserName = CredentialsManager.GetUserName(),
                                              IntegrationModule = CredentialsManager.GetIntegrator()
                                          };
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


       private  List<ImportItemModel>  Import(string directoryPath)
        {
           if(!directoryPath.Contains("Stockline"))
           {
               FileUtility.LogCommandActivity(string.Format("Invalid file {0}",directoryPath));
               return null;
           }
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
                                        SalesmanCode =(currentRow.ElementAtOrDefault(2) !=null && string.IsNullOrEmpty(currentRow[2]))?currentRow.ElementAtOrDefault(2):"default"
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
       protected HttpClient HttpClient
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


    }
}
