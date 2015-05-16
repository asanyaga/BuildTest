using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.WSAPI.Lib.Integrations;
using Distributr_Middleware.WPF.Lib.MiddlewareServices;
using Distributr_Middleware.WPF.Lib.MiddlewareServices.Impl;
using Distributr_Middleware.WPF.Lib.Utils;
using GalaSoft.MvvmLight.Messaging;
using SAPbobsCOM;
using StructureMap;

namespace SAPUtilityLib.Masterdata.Impl
{
    public class ExportTransactionsService : SAPUtilsBase, IExportTransactionsService
    {
       
        private List<ImportEntity> roworders;
       
        private readonly ITransactionsDownloadService _transactionsDownloadService;
       
        public ExportTransactionsService()
        {
            roworders=new List<ImportEntity>();
            //TableLines = CreateLinesTable();
            _transactionsDownloadService = ObjectFactory.GetInstance<ITransactionsDownloadService>();
        
        }


        public Task<bool> ExportToSap(OrderType orderType)
        {
            return Task.Run(() =>
                                {
                                    LoadFiles();
                                    if (!roworders.Any())
                                    {
                                        var task=_transactionsDownloadService.GetTransactions("",orderType).Result;
                                        
                                        if(task !=null)
                                        {
                                            if(!string.IsNullOrEmpty(task.TransactionData))
                                            {
                                                LoadFiles();
                                                Export(); 
                                            }
                                            else
                                            {
                                                Alert("No files downloaded from HQ");
                                                return true;
                                            }
                                            
                                        }
                                    }
                                    Export();
                                    return true;
                                });

        }

       void Alert(string msg)
       {
           Messenger.Default.Send(msg);
           MessageBox.Show(msg);
       }

        public SyncBasicResponse PullInventoryTransfer()
        {



            var response = new SyncBasicResponse();

            try
            {

                var config = InventoryConfiguration.Load();
                if (config == null)
                {
                    config = new InventoryConfiguration();
                    config.LastSyncDateTime = GetCurrentDatetime().AddDays(-5);
                }
                var query =
                    @";with warehousecte as(
	SELECT WhsCode,WhsName,
	(select WhsCode from OWHS where SUBSTRING(WhsCode, 1, 3)=SUBSTRING(sm.WhsCode, 1, 3) and WhsCode like '%MAIN%') as DistributorCode
	FROM OWHS sm
	 WHERE U_Company ='R' and WhsCode like '%VAN%' 								
),
 cte as (
SELECT  (select case when len (doc. doctime)=3 then
                                     convert ( datetime,convert (varchar,( convert(varchar ,doc. CreateDate,103 ) + ' '+ (left(doc .doctime, 1)) + ':' + (right(doc. doctime,2 ))) , 114),103 )
                                     else
                                     convert ( datetime,convert (varchar,( convert(varchar ,doc. CreateDate,103 ) + ' '+ (left(doc .doctime, 2)) + ':' + (right(doc. doctime,2 ))) , 114),103 )
                                     end) CreatedDateTime,                      
                        line.ItemCode as productcode,
                        line.Quantity ,
                        line.WhsCode as SalesmanCode,
                        doc.DocNum   as DocRef,
                                          getdate() as CurrentDateTime
                        FROM WTR1 line
                        JOIN OWTR doc on line .DocEntry= doc.DocEntry
						LEFT JOIN warehousecte sm on sm.WhsCode=line. WhsCode                                         
						join OITM product on product.ItemCode=line.ItemCode
                                          where sm.DistributorCode  is not null
										  and product.QryGroup64='Y' and product.validFor='Y'
                                          and product.ItemName is not null    
                                          )
                         
select productcode,Quantity,SalesmanCode,DocRef,CurrentDateTime from cte -- where salesmancode='nb-van13'

 where CreatedDateTime > '" +
                    config.LastSyncDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "'";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {

                    conn.Open();
                    using (SqlCommand comm = new SqlCommand(query, conn))
                    {
                        SqlDataReader reader = comm.ExecuteReader();
                        var dto = DampToDTO(reader).Result;
                        if (dto != null && (dto.SalesmanInventoryList.Any() || dto.DistributorInventory.Any()))
                        {
                            var res = new InventoryTransferService().UploadInventory(dto).Result;
                            if (res.Result.Equals("error", StringComparison.CurrentCultureIgnoreCase))
                            {
                                response = new SyncBasicResponse
                                           {
                                               Info =
                                                   "There was an error updating inventory files:\n ResultInfo" +
                                                   res.ResultInfo + "\nError Details:" +
                                                   res.ErrorInfo,
                                               Status = false
                                           };

                            }
                            else
                            {
                                response = new SyncBasicResponse {Info = res.ResultInfo, Status = true};
                                config.LastSyncDateTime = GetCurrentDatetime();
                                config.Save();


                            }

                        }
                        else
                        {
                            response = new SyncBasicResponse
                            {
                                Info =
                                    "There was no inventory  transfer to be uploaded ",
                                Status = false
                            };
                        }
                    }

                }



                return response;
               
            }
            catch (Exception ex)
            {
                response = new SyncBasicResponse {Info = "Error Uploading Inventory", Status = false};
              
                return response;
            }

        }
        public SyncBasicResponse PullIntialInventory()
        {



            var response = new SyncBasicResponse();

            try
            {

                var config = InventoryConfiguration.Load();
                if (config == null)
                {
                    config = new InventoryConfiguration();
                   
                }
                var query =
                    @";with warehousecte as(
	SELECT WhsCode,WhsName,
	(select WhsCode from OWHS where SUBSTRING(WhsCode, 1, 3)=SUBSTRING(sm.WhsCode, 1, 3) and WhsCode like '%MAIN%') as DistributorCode
	FROM OWHS sm
	 WHERE U_Company ='R' and WhsCode like '%VAN%' 								
), cte as (
SELECT                     
                        line.ItemCode as productcode,
                        line.OnHand as  Quantity,
                        line.WhsCode as SalesmanCode,
                        'Intial'   as DocRef
                                         
                        FROM OITW line
						join warehousecte warehouse on warehouse.WhsCode =line. WhsCode
										  join OITM product on product.ItemCode=line.ItemCode
                                          where warehouse.DistributorCode is not null
										  and product.QryGroup64='Y' and product.validFor='Y'
                                          and product.ItemName is not null    
                                          )
                         
select productcode,Quantity,SalesmanCode,DocRef from cte   where  Quantity > 0 

";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {

                    conn.Open();
                    using (SqlCommand comm = new SqlCommand(query, conn))
                    {
                        SqlDataReader reader = comm.ExecuteReader();
                        var dto = DampToDTO(reader).Result;
                        if (dto != null && (dto.SalesmanInventoryList.Any() || dto.DistributorInventory.Any()))
                        {
                            var res = new InventoryTransferService().UploadInventory(dto).Result;
                            if (res.Result.Equals("error", StringComparison.CurrentCultureIgnoreCase))
                            {
                                response = new SyncBasicResponse
                                {
                                    Info =
                                        "There was an error updating inventory files:\n ResultInfo" +
                                        res.ResultInfo + "\nError Details:" +
                                        res.ErrorInfo,
                                    Status = false
                                };

                            }
                            else
                            {
                                response = new SyncBasicResponse { Info = res.ResultInfo, Status = true };
                                config.LastSyncDateTime = GetCurrentDatetime();
                                config.Save();


                            }

                        }
                        else
                        {
                            response = new SyncBasicResponse
                            {
                                Info =
                                    "There was no inventory  transfer to be uploaded ",
                                Status = false
                            };
                        }
                    }

                }



                return response;

            }
            catch (Exception ex)
            {
                response = new SyncBasicResponse { Info = "Error Uploading Inventory", Status = false };

                return response;
            }

        }

        public SyncBasicResponse PullInventory()
        {



            var response = new SyncBasicResponse();

          try
          {
            
              var config = InventoryConfiguration.Load();
              if (config == null)
              {
                  config = new InventoryConfiguration();
                  config.LastSyncDateTime = GetCurrentDatetime().AddDays(-5);
              }
              Recordset rs = (Recordset)Company.GetBusinessObject(BoObjectTypes.BoRecordset);
             

  


              var query = @";with cte as (
SELECT  (select case when len (doc. doctime)=3 then
                                     convert ( datetime,convert (varchar,( convert(varchar ,doc. CreateDate,103 ) + ' '+ (left(doc .doctime, 1)) + ':' + (right(doc. doctime,2 ))) , 114),103 )
                                     else
                                     convert ( datetime,convert (varchar,( convert(varchar ,doc. CreateDate,103 ) + ' '+ (left(doc .doctime, 2)) + ':' + (right(doc. doctime,2 ))) , 114),103 )
                                     end) CreatedDateTime,                      
                        line.ItemCode as productcode,
                        line.Quantity ,
                        line.WhsCode as SalesmanCode,
                        doc.DocNum   as DocRef,
                                          getdate() as CurrentDateTime
                        FROM WTR1 line
                        JOIN OWTR doc on line .DocEntry= doc.DocEntry
                                          join OWHS warehouse on warehouse.WhsCode =line. WhsCode
										  join OITM product on product.ItemCode=line.ItemCode
                                          where warehouse .U_Company ='R' and warehouse.WhsCode like '%VAN%'
										  and product.QryGroup64='Y'
                                          )
                         
select productcode,Quantity,SalesmanCode,DocRef,CurrentDateTime from cte  where CreatedDateTime > '"+config.LastSyncDateTime.ToString("yyyy-MM-dd HH:mm:ss")+"'";
              rs.DoQuery(query);

             
              var da = rs.RecordCount;
              
              response=  DampToFolder(rs).Result;
              config.LastSyncDateTime = GetCurrentDatetime();
              config.Save();
              return response;
              // response = new SyncBasicResponse {Info = "Inventory Uploaded successfully", Status = true};
              // return response;
          }catch(Exception ex)
          {
              response = new SyncBasicResponse { Info = "Error Uploading Inventory", Status = false };
              //Alert(ex.Message);
              return response;
          }

        }

        private  DateTime GetCurrentDatetime()
        {
            var time = Company.GetCompanyTime().Split(':');
            var date = Company.GetCompanyDate();
            int hour = 12;
            int.TryParse(time[0], out hour);
            int min = 0;
            int.TryParse(time[1], out min);
            return new DateTime(date.Year,date.Month,date.Day,hour,min,0);
        }
        async Task<SyncBasicResponse> DampToFolder(SqlDataReader rs)
        {
            var response = new SyncBasicResponse();
            //go=> currently pull inventory from Nairobi warehouse(the distributor code is NBO-MAIN) only
            ;
            var filename = FileUtility.GetSApFile(string.Format("Stockline-NBO-MAIN-00"));
            if (base.DampToFolder(rs, filename).Status)
            {

                var dto = LoadDistributorInventoryFiles(filename);
                if (dto != null && (dto.SalesmanInventoryList.Any() || dto.DistributorInventory.Any()))
                {
                    var res = await new InventoryTransferService().UploadInventory(dto);
                    if (res.Result.Equals("error", StringComparison.CurrentCultureIgnoreCase))
                    {
                        response = new SyncBasicResponse { Info = "There was an error updating inventory files:\n ResultInfo" + res.ResultInfo + "\nError Details:" + res.ErrorInfo, Status = false };
                        // Alert("There was an error updating inventory files:\n ResultInfo" + res.ResultInfo + "\nError Details:"+res.ErrorInfo);
                    }
                    else
                    {
                        response = new SyncBasicResponse { Info = res.ResultInfo, Status = true };

                        //  Alert(res.ResultInfo);
                    }
                }
                else
                {
                    response = new SyncBasicResponse { Info = string.Format("Files cannot be loaded or the file on path: {0} is empty", filename), Status = false };
                    //Alert(string.Format("Files cannot be loaded or the file on path: {0} is empty",filename));
                }

            }
            else
            {
                response = new SyncBasicResponse { Info = "An error occured while updating distributor inventory", Status = false };

                //Alert("An error occured while updating distributor inventory");

            }
            return response;
        }
        async Task<InventoryTransferDTO> DampToDTO(SqlDataReader rs)
        {
            var response = new SyncBasicResponse();
            //go=> currently pull inventory from Nairobi warehouse(the distributor code is NBO-MAIN) only
            ;
            var filename = FileUtility.GetSApFile(string.Format("Stockline-NBO-MAIN-00"));
            if (base.DampToFolder(rs, filename).Status)
            {

              return LoadDistributorInventoryFiles(filename);
                //if (dto != null && (dto.SalesmanInventoryList.Any() || dto.DistributorInventory.Any()))
                //{
                //    var res = await new InventoryTransferService().UploadInventory(dto);
                //    if (res.Result.Equals("error", StringComparison.CurrentCultureIgnoreCase))
                //    {
                //        response = new SyncBasicResponse { Info = "There was an error updating inventory files:\n ResultInfo" + res.ResultInfo + "\nError Details:" + res.ErrorInfo, Status = false };
                //        // Alert("There was an error updating inventory files:\n ResultInfo" + res.ResultInfo + "\nError Details:"+res.ErrorInfo);
                //    }
                //    else
                //    {
                //        response = new SyncBasicResponse { Info = res.ResultInfo, Status = true };

                //        //  Alert(res.ResultInfo);
                //    }
                //}
                //else
                //{
                //    response = new SyncBasicResponse { Info = string.Format("Files cannot be loaded or the file on path: {0} is empty", filename), Status = false };
                //    //Alert(string.Format("Files cannot be loaded or the file on path: {0} is empty",filename));
                //}

            }
            
            return null;
        }
        async Task<SyncBasicResponse>  DampToFolder(Recordset rs)
        {
            var response = new SyncBasicResponse();
            //go=> currently pull inventory from Nairobi warehouse(the distributor code is NBO-MAIN) only
            ;
            var filename = FileUtility.GetSApFile(string.Format("Stockline-NBO-MAIN-00"));
            if (base.DampToFolder(rs, filename).Status)
            {

                var dto = LoadDistributorInventoryFiles(filename);
                if (dto != null && (dto.SalesmanInventoryList.Any() || dto.DistributorInventory.Any()))
                {
                  var res= await new InventoryTransferService().UploadInventory(dto);
                    if(res.Result.Equals("error",StringComparison.CurrentCultureIgnoreCase))
                    {
                        response = new SyncBasicResponse { Info = "There was an error updating inventory files:\n ResultInfo" + res.ResultInfo + "\nError Details:"+res.ErrorInfo, Status = false };
                       // Alert("There was an error updating inventory files:\n ResultInfo" + res.ResultInfo + "\nError Details:"+res.ErrorInfo);
                    }
                    else
                    {
                        response = new SyncBasicResponse { Info = res.ResultInfo, Status = true };
                    
                      //  Alert(res.ResultInfo);
                    }
                }
                else
                {
                    response = new SyncBasicResponse { Info = string.Format("Files cannot be loaded or the file on path: {0} is empty", filename), Status = false };
                    //Alert(string.Format("Files cannot be loaded or the file on path: {0} is empty",filename));
                }

            }
            else
            {
                response = new SyncBasicResponse { Info = "An error occured while updating distributor inventory", Status = false };
                    
                //Alert("An error occured while updating distributor inventory");
                
            }
            return response;
        }
        private InventoryTransferDTO LoadDistributorInventoryFiles(string path)
        {
               InventoryTransferDTO dto=new InventoryTransferDTO();
          var  Files = FileUtility.GetStockLines(new DirectoryInfo(path));
          if (Files == null) return null;
            Files = Files.Distinct().ToArray();
            FileUtility.LogCommandActivity(DateTime.Now.ToString("hh:mm:ss") + "Generating import files");

            foreach (var fileInfo in Files)
            {
                //format=>stocline-distributorcode
                var highenSeparator = new[] { '-' };
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
                        dto.DistributorCode = temp.ElementAt(1) + "-" + temp.ElementAt(2);
                    }
                }


                var inventory = new InventoryTransferService().Import(fileInfo.FullName);
                var dicties = new Dictionary<string, List<InventoryLineItemDto>>();
                var externalDocRefs = new List<string>();
                if (inventory != null && inventory.Any())
                {
                    var grouped = inventory.GroupBy(p => p.SalesmanCode);
                  
                    foreach (var group in grouped)
                    {
                        var listofLineItems = new List<InventoryLineItemDto>();
                        foreach (var itemModel in group)
                        {
                            if(!string.IsNullOrEmpty(itemModel.DocumentRef)&& !externalDocRefs.Contains(itemModel.DocumentRef))
                            {
                                externalDocRefs.Add(itemModel.DocumentRef);
                            }
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
                if (externalDocRefs.Any())
                    dto.ExternalDocumentRefList = externalDocRefs.Distinct().ToList();

            }
            return dto;
        }
        
        void Export()
        {
            
            if(!roworders.Any())return;
            var orderLines = roworders.GroupBy(p => p.Fields.ElementAt(1));
            
            var docs = orderLines.ToDictionary(line => line.Key, line => line.Select(n => new ImportEntity()
                                                                                              {
                                                                                                  Fields = n.Fields
                                                                                              }).ToList());
            int lRetCode = 0;
            var skiped = new Dictionary<string, List<string>>();
            foreach (var doc in docs)
            {
               if(!OrderRefExist(doc.Key))
                {
                    Documents oOrder = (Documents) Company.GetBusinessObject(BoObjectTypes.oOrders);
                var firstRow = doc.Value.FirstOrDefault().Fields;
               if(firstRow.Any(string.IsNullOrEmpty))
               {
                   Alert("One or more files have empty fields..process aborted");
                    break;
                }
                SapObject outlet = GetOutlet(firstRow.ElementAtOrDefault(0));
                if(outlet==null)
                {
                    Alert("Invalid customer...");
                    break;
                }

                // set properties of the Order object
                oOrder.CardCode = outlet.Field1; //CustomerCode;
                oOrder.CardName = outlet.Field2; //CustomerName;
               
                oOrder.NumAtCard = firstRow.ElementAtOrDefault(1); //txtReference.Text;
                oOrder.HandWritten = BoYesNoEnum.tYES; //chkManual order reference generation

                oOrder.DocNum = GetDocNum(); //document number;
                oOrder.DocDate = DateTime.Parse(firstRow.ElementAtOrDefault(2)); // DatePosting.Value;
                oOrder.DocDueDate = DateTime.Parse(firstRow.ElementAtOrDefault(3)); // DatePosting.Value;
                
                SapObject priceObject = null;
               
                List<SapDocumentLineItem> lineItems=new List<SapDocumentLineItem>();
                foreach (var line in doc.Value.Select(n => n.Fields.ToList()).ToList())
                {
                    lineItems.Add(AddLineItem(line, out priceObject, oOrder));
                    
                }
                if (priceObject != null)
                    oOrder.DocCurrency = priceObject.Field2; //Currency=>I take any first value
                foreach (var lineItem in lineItems)
                {
                    
                    var warehouse = lineItem.SalesmanCode;
                    oOrder.Lines.ItemCode = lineItem.ProductCode;
                    oOrder.Lines.ItemDescription = lineItem.ProductDescription;
                    oOrder.Lines.Quantity =lineItem.Quantity;
                    oOrder.Lines.TaxCode =lineItem.VatClassName;
                    oOrder.Lines.UnitPrice= lineItem.Price;
                    oOrder.Lines.UseBaseUnits=BoYesNoEnum.tYES;
                    //oOrder.Lines.LineTotal = lineItem.Total;
                    if (warehouse.Equals("NBO-VAN4") || warehouse.Equals("NBOVAN10"))
                        oOrder.Lines.WarehouseCode = warehouse;

                    oOrder.Lines.Add();
                }

               lRetCode = oOrder.Add(); // Try to add the orer to the database
                oOrder = null;
                int lErrCode = 0;
                string sErrMsg = "";
                if (lRetCode != 0)
                {
                    int temp_int = lErrCode;
                    string temp_string = sErrMsg;
                    Company.GetLastError(out temp_int, out temp_string);
                    if (lErrCode != -4006) // Incase adding an order failed
                    {
                        MessageBox.Show(lErrCode + " " + temp_string); // Display error message
                    }
                    else // If the currency Exchange is not set
                    {
                        double dCur;
                        object sCur;
                        sCur = 1.0;
                        if (double.TryParse(sCur.ToString(), out dCur))
                        {
                            dCur = Convert.ToDouble(sCur);
                            var oBob = (SBObob) Company.GetBusinessObject(BoObjectTypes.BoBridge);
                            //Update Currency rate
                            oBob.SetCurrencyRate(priceObject.Field2, DateTime.Today, dCur, false);
                        }
                        else
                        {
                            MessageBox.Show("Invalid Value to Currency Exchange", "Invalid Value");
                        }
                    }
                }
                }
              
            }
            if (lRetCode == 0)
                MoveFile();
        }

        private SapDocumentLineItem AddLineItem(List<string> doc, out SapObject priceObject, Documents oOrder)
        {
            SapDocumentLineItem row = new SapDocumentLineItem();
            // Get an initialized SBObob object
            var product = GetProduct(doc.ElementAtOrDefault(4));
            if (product == null)
            {
                Messenger.Default.Send("Invalid product");
                throw new ArgumentNullException("Invalid product");
            }
            var qntity = Convert.ToDouble(doc.ElementAtOrDefault(5));//quantity
            priceObject = GetProductPrice(oOrder.CardCode, product.Field1, qntity, oOrder.DocDate);
            row.ProductCode = product.Field1; //ProductCode;
            row.ProductDescription= product.Field2; //ProductDesc;
            row.Quantity = (int)qntity;
            row.Price = Convert.ToDouble(priceObject.Field1);
            row.VatClassName = doc.ElementAtOrDefault(6); // VATClass.Name;
            row.Total = Convert.ToDouble(priceObject.Field1) * Convert.ToInt32(qntity);
            row.SalesmanCode= doc.ElementAtOrDefault(7);//salesmancode
            return row;
        }

        class SapDocumentLineItem
        {
            public int Quantity { get; set; }
            public double Price { get; set; }
            public string ProductCode { get; set; }
            public string ProductDescription { get; set; }
            public string VatClassName { get; set; }
            public string SalesmanCode { get; set; }
            public double Total { get; set; }
        }
        private void MoveFile()
        {
            try
            {
                string directoryPath = FileUtility.GetWorkingDirectory("exports");
                var exported = Directory.CreateDirectory(Path.Combine(directoryPath, "Exported")).FullName;

                foreach (var file in Directory.GetFiles(directoryPath))
                {
                    string destPath = Path.Combine(exported, Path.GetFileName(file));
                    if (File.Exists(file))
                    {
                        if (File.Exists(destPath))
                            File.Delete(destPath);
                        File.Move(file, destPath);
                    }

                }
                MessageBox.Show("done...");
            }
            catch (IOException ex)
            {

            }
            catch (Exception ex)
            {

            }
        }
        private SapObject GetProductPrice(string outletCode,string productCode,double quantity,DateTime orderdate)
        {
            SBObob oObj = (SBObob)Company.GetBusinessObject(BoObjectTypes.BoBridge);
            Recordset rs = (Recordset)Company.GetBusinessObject(BoObjectTypes.BoRecordset);
            
            rs = oObj.GetItemPrice(outletCode, productCode, quantity, orderdate);
            var units = Units(productCode);
            return new SapObject
                       {
                           Field1 = Convert.ToString(((double)rs.Fields.Item(0).Value / units)), //price
                           Field2 = rs.Fields.Item(1).Value //currency

                       };
        }
     private double Units(string productCode)
     {
         var query = string.Format(@"select  salpackun from OITM where itemcode='{0}'", productCode);
         var rs = (Recordset)Company.GetBusinessObject(BoObjectTypes.BoRecordset);
         rs.DoQuery(query);
         double units = 1;
         while (!(rs.EoF))
         {
             var item = rs.Fields.Item(0).Value;
             if (item != null)
             {
                 units = item;
                 break;
             }
             rs.MoveNext();
         }
         return units;
         
     }
        private SapObject GetProduct(string productCode)
        {
            SBObob oObj = (SBObob)Company.GetBusinessObject(BoObjectTypes.BoBridge);
            Recordset rs = (Recordset)Company.GetBusinessObject(BoObjectTypes.BoRecordset);

            rs = oObj.GetItemList();
            rs.MoveFirst();
            SapObject product = null;
            while (!(rs.EoF))
            {
                if (rs.Fields.Item(0).Value.Equals(productCode, StringComparison.CurrentCultureIgnoreCase))
                {
                    product = new SapObject()
                    {
                        Field1 = rs.Fields.Item(0).Value, //name
                        Field2 = rs.Fields.Item(1).Value //description
                    };
                    break;
                }

                rs.MoveNext();
            }
            return product;
           
        }

        private SapObject GetOutlet(string customerCode)
        {
            SBObob oObj = (SBObob)Company.GetBusinessObject(BoObjectTypes.BoBridge);
            Recordset rs = (Recordset)Company.GetBusinessObject(BoObjectTypes.BoRecordset);
            rs = oObj.GetBPList(BoCardTypes.cCustomer);
            rs.MoveFirst();
            SapObject outlet = null;
            while (!(rs.EoF))
            {
                if (rs.Fields.Item(0).Value.Equals(customerCode, StringComparison.CurrentCultureIgnoreCase))
                {
                    outlet = new SapObject()
                                 {
                                     Field1 = rs.Fields.Item(0).Value, //code
                                     Field2 = rs.Fields.Item(1).Value //name
                                 };
                    break;
                }

                rs.MoveNext();
            }
            return outlet;
        }
      
        private void LoadFiles()
        {
          try
            {
                string directoryPath = FileUtility.GetWorkingDirectory("exports");

                var files = Directory.GetFiles(directoryPath);
                int skipped = 0;
                foreach (var file in files)
                {
                    using (var parser = new Microsoft.VisualBasic.FileIO.TextFieldParser(file))
                    {
                        parser.SetDelimiters(",");

                        while (!parser.EndOfData)
                        {
                            string[] currentRow = parser.ReadFields();
                            if (currentRow != null && currentRow.Length >=8)
                            {
                                if (currentRow.Any(string.IsNullOrEmpty))
                                {
                                    skipped++;
                                }
                                else
                                {
                                    roworders.Add(new ImportEntity()
                                    {
                                        Fields = currentRow
                                    }); 
                                }
                               
                            }
                        }
                    }

                }
              if(skipped>0)
              {
                  MessageBox.Show(string.Format("{0} Export lines skipped,Chec export file=>{1} for details", skipped, string.Join(";",files)));
              }
            }
            catch (IOException ex)
            {
                MessageBox.Show("File Load Error\n" + ex.Message);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error\n" + ex.Message);

            }

        }

        private bool OrderRefExist(string orderRef)
        {
            // Create the next Order number
            string sSQL = string.Format("SELECT top 1 numatcard FROM dbo.ORDR where numatcard='{0}' ORDER BY numatcard DESC",orderRef);
            var rs = (Recordset)Company.GetBusinessObject(BoObjectTypes.BoRecordset);
            rs.DoQuery(sSQL);
            bool exist = false;
            while (!(rs.EoF))
            {
                var item = rs.Fields.Item(0).Value;
                if(item !=null && item.Equals(orderRef,StringComparison.CurrentCultureIgnoreCase))
                {
                    exist =true;
                    break;
                }
                rs.MoveNext();
            }
            return exist;
        }
        private int GetDocNum()
        {
            // Create the next Order number
            string sSQL = "SELECT TOP 1 DocNum FROM dbo.ORDR ORDER BY DocNum DESC";
          var  rs = (Recordset)Company.GetBusinessObject(BoObjectTypes.BoRecordset);
            rs.DoQuery(sSQL);
            int number = 0;
            while (!(rs.EoF))
            {
                number = System.Convert.ToInt32(rs.Fields.Item(0).Value) + 1;
                rs.MoveNext();
            }
            return number;
        }
      
    }
    public class SapObject
    {
        public string Field1 { get; set; }
        public string Field2 { get; set; }
    }
}
