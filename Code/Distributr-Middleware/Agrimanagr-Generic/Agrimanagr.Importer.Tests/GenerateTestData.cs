using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agrimanagr.DataImporter.Lib.ImportEntities;
using Agrimanagr.DataImporter.Lib.Utils;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.IOC;
using Distributr.Core.Domain.Master.CommodityEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Utility;
using LINQtoCSV;
using NUnit.Framework;
using StructureMap;

namespace Agrimanagr.Importer.Tests
{
    public class GenerateTestData
    {
        [Test]
        public void Run()
        {
            string conn = ConfigurationManager.AppSettings["cokeconnectionstring"];
            ObjectFactory.Initialize(x => x.AddRegistry<DataRegistry>());
            using (var ctx = new CokeDataContext(conn))
            {
                GenerateTestCommodities(ctx);
                GenerateTestCommodityTypes(ctx);
                GenerateTestCommoditySuppliers(ctx);
                GenerateTestCommodityOwnerTypes(ctx);
                GenerateTestCommodityOwneres(ctx);

            }

        }

        private void GenerateTestCommodityOwneres(CokeDataContext ctx)
        {
            var cTypes = ctx.tblCommodityOwner.Take(10).Select(n => new CommodityOwnerImport()
                                                                        {
                                                                            Code = n.Code,
                                                                            Description = n.Description,
                                                                            Email = n.Email,
                                                                            LastName = n.LastName,
                                                                            FaxNumber = n.FaxNo,
                                                                            PinNo = n.PINNo,
                                                                            BusinessNumber = n.PINNo,
                                                                            CommodityOwnerTypeName =
                                                                                n.tblCommodityOwnerType.Name,
                                                                            CommoditySupplierName = n.tblCostCentre.Name,
                                                                            DateOfBirth = n.DOB,
                                                                            GenderEnum = n.Gender,
                                                                            IdNo = n.IdNo,
                                                                            OfficeNumber = n.OfficeNo,
                                                                            PhoneNumber = n.PhoneNo,
                                                                            PostalAddress = n.PostalAddress,
                                                                            Surname = n.Surname,
                                                                            FirstName = n.FirstName,
                                                                            PhysicalAddress = n.PhysicalAddress,
                                                                        }).ToList();
            DumpExportFilesAsync(cTypes.ToCsv(), "farmers.csv");
        }

        void GenerateTestCommoditySuppliers(CokeDataContext ctx)
        {
            var cTypes = ctx.tblCostCentre.Where(p => p.CostCentreType == (int)CostCentreType.CommoditySupplier).Take(10).Select(n => new CommoditySupplierImport()
            {
                Code = n.Cost_Centre_Code,
                JoinDate = n.JoinDate??DateTime.Now,
                AccountNo = n.MerchantNumber,
                PinNo = n.Revenue_PIN,
                BankBranchName = n.StandardWH_Longtitude,
                BankName = n.StandardWH_Latitude //hee you are here..don't hate,did what I had to do=>G.O,
                
            }).ToList();
            DumpExportFilesAsync(cTypes.ToCsv(), "commoditySupplier.csv");
        }
       void GenerateTestCommodityOwnerTypes(CokeDataContext ctx)
       {
           var cTypes = ctx.tblCommodityOwnerType.Take(10).Select(n => new CommodityOwnerTypeImport()
                                                                           {
                                                                               Code = n.Code,
                                                                               Description = n.Description,
                                                                               Name = n.Name
                                                                           }).ToList();
           DumpExportFilesAsync(cTypes.ToCsv(), "CommodityOwnerType.csv");
       }

        void GenerateTestCommodityTypes(CokeDataContext ctx)
        {
            var cTypes = ctx.tblCommodityType.Take(10).Select(n=>new CommodityTypeImport()
                                                                     {
                                                                         Code = n.Code,
                                                                         Name = n.Name,
                                                                         Description = n.Description
                                                                     }).ToList();
            DumpExportFilesAsync(cTypes.ToCsv(), "CommodityType.csv");
        }

        private void GenerateTestCommodities(CokeDataContext ctx)
        {
            var commodities = ctx.tblCommodity.Take(10).Select(n => new CommodityImport
            {
                Code = n.Code,
                CommodityTypeCode = n.tblCommodityType.Code,
                Description = n.Description,
                Name = n.Name
            }).ToList();

            DumpExportFilesAsync(commodities.ToCsv(), "commodity.csv");
        }

        private async void DumpExportFilesAsync(string orders, string entityname)
        {
            string selectedPath = Path.Combine(FileUtility.GetDefaultDirectory(), entityname);
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
                        await wr.WriteLineAsync(orders);

                    }
                }
            }
            catch (IOException ex)
            {

            }
        }

        [Test]
       public void Can_Read_From_CSV_File()
        {
            string selectedPath = Path.Combine(FileUtility.GetDefaultDirectory(), "commodity.csv");

            var inputFileDescription = new CsvFileDescription
            {
                // cool - I can specify my own separator!
                SeparatorChar = ',',
                FirstLineHasColumnNames = false,
                EnforceCsvColumnAttribute = false
            };


             var imports = new CsvContext().Read<MasterImportEntity>(selectedPath, inputFileDescription);
             var commodityImports = imports.Select(dataRow => new CommodityImport()
                                                                  {
                                                                      Code = dataRow[0].Value,
                                                                      Name = dataRow[1].Value,
                                                                      CommodityTypeCode = dataRow[2].Value,
                                                                      Description = dataRow[3].Value
                                                                  }).ToList();

             Assert.IsTrue(commodityImports.Any());
            

        }
    }
}
