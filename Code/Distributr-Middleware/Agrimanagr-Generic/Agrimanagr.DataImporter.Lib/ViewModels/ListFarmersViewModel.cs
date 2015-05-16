using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Agrimanagr.DataImporter.Lib.ImportEntities;
using Agrimanagr.DataImporter.Lib.ImportServices;
using Agrimanagr.DataImporter.Lib.ImportServices.Commodities.Impl;
using Agrimanagr.DataImporter.Lib.Utils;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CommodityEntity;
using Distributr.Core.Domain.Master.Util;

namespace Agrimanagr.DataImporter.Lib.ViewModels
{
    public class ListFarmersViewModel : ImporterListingsViewModelBase
    {
        public ObservableCollection<CommodityOwnerImportVM> CommodityOwnerImportVmList { get; set; }
        internal List<CommodityOwnerImportVM> PagedListAll; 
        public ListFarmersViewModel()
        {
            CommodityOwnerImportVmList = new ObservableCollection<CommodityOwnerImportVM>();
            PagedListAll=new List<CommodityOwnerImportVM>();
        }

        protected override void Load(bool isFirstLoad = false)
        {
            if (isFirstLoad)
                Setup();
            LoadFarmers();
        }

        private async void LoadFarmers()
        {
            using (var c = NestedContainer)
            {
                var importService = Using<ICsvHandlerService>(c);
                try
                {
                    if (string.IsNullOrEmpty(SelectedPath) || !File.Exists(SelectedPath))
                        SelectedPath = FileUtility.OpenImportDirectoryPath();
                    UploadStatusMessage = "Please Wait......!";
                    var importItems = await importService.ReadFromCsVFileAsync(SelectedPath);
                    Map(importItems);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error Importing orders\nDetails:" + ex.Message);

                }

            }
        }

        private void Map(IEnumerable<MasterImportEntity> importItems)
        {
            if (!importItems.Any()) return;
            if(!Positions.Any())
            using (var c = NestedContainer)
            {
                Positions = Using<IMapEntityColumnPosition>(c).GetEntityMapping(new ImportEntity() { EntityName = "commodityowner" });
            }
            if (!Positions.Any())
            {
                GotoHomePage();
                return;
            }
            PagedListAll.Clear();
            
           var paged= importItems.Select((row, index) => new CommodityOwnerImportVM()
                                                         {
                                                             Code = GetColumn(row, GetIndex("code")),
                                                             Name = GetColumn(row, GetIndex("name")),
                                                             Description = GetColumn(row, GetIndex("description")),
                                                             FirstName = GetColumn(row,GetIndex("FirstName")),
                                                             LastName = GetColumn(row,GetIndex("lastname")),
                                                             BusinessNumber =GetColumn(row,GetIndex("businessnumber")),
                                                             OfficeNumber =GetColumn(row,GetIndex("OfficeNumber")),
                                                             Email =GetColumn(row,GetIndex("Email")),
                                                             FaxNumber =GetColumn(row,GetIndex("FaxNumber")),
                                                             IdNo =GetColumn(row,GetIndex("IdNo")),
                                                             PinNo = GetColumn(row, GetIndex("PinNo")),
                                                             PhoneNumber =GetColumn(row,GetIndex("PhoneNumber")),
                                                             CommodityOwnerTypeName =GetColumn(row,GetIndex("CommodityOwnerTypeName")),
                                                             PostalAddress =GetColumn(row,GetIndex("PostalAddress")),
                                                             Surname =GetColumn(row,GetIndex("Surname")),
                                                             CommoditySupplierName =GetColumn(row,GetIndex("CommoditySupplierName")),
                                                             GenderEnum =GetColumn(row,GetIndex("GenderEnum"),handleEnum:true),
                                                             DateOfBirth =GetColumn(row,GetIndex("DateOfBirth"),handleDateTime:true),
                                                             IsChecked = false,
                                                             SequenceNo = index + 1
                                                         }).AsQueryable();

           PagedListAll.AddRange(paged.ToList());

            PagedList = new PagenatedList<ImportItemVM>(paged, CurrentPage, ItemsPerPage, paged.Count());
            CommodityOwnerImportVmList.Clear();
            PagedList.ToList().ForEach(n => CommodityOwnerImportVmList.Add((CommodityOwnerImportVM)n));
            UpdatePagenationControl();
        }
        protected override async void UploadAll()
        {
            if (!PagedListAll.Any()) return;
            var items = PagedListAll.Select(n => new CommodityOwnerImport()
                                                                   {
                                                                       Code = n.Code,
                                                                       Description = n.Description,
                                                                       BusinessNumber = n.BusinessNumber,
                                                                       CommodityOwnerTypeName = n.CommodityOwnerTypeName,
                                                                       CommoditySupplierName = n.CommoditySupplierName,
                                                                       DateOfBirth =DateTime.Parse(n.DateOfBirth),
                                                                       Email = n.Email,
                                                                       FaxNumber = n.FaxNumber,
                                                                       FirstName = n.FirstName,
                                                                       GenderEnum =(int)Enum.Parse(typeof(Gender),n.GenderEnum),
                                                                       IdNo = n.IdNo,
                                                                       LastName = n.LastName,
                                                                       OfficeNumber = n.OfficeNumber,
                                                                       PhoneNumber = n.PhoneNumber,
                                                                       PhysicalAddress = n.PhysicalAddress,
                                                                       PinNo = n.PinNo,
                                                                       PostalAddress = n.PostalAddress,
                                                                       Surname = n.Surname,
                                                                   }).ToList();

            var skippedsurnames = items.Where(p => string.IsNullOrEmpty(p.Surname)).ToList();
            var skippedCodes = items.Where(p => string.IsNullOrEmpty(p.Code)).ToList();

            if(skippedsurnames.Any()||skippedCodes.Any())
            {
                MessageBox.Show(string.Format("Items with missing codes={0} and missing surname is ={1}:Import terminated", skippedCodes.Count, skippedsurnames.Count));
                return;
            }
            
            items = items.Where(p => !string.IsNullOrEmpty(p.Surname) || !string.IsNullOrEmpty(p.Code)).ToList();


            await Task.Factory.StartNew(() => Import(items.ToList()));
        }

        protected override async void UploadSelected()
        {
            var selected = CommodityOwnerImportVmList.Where(o => o.IsChecked).ToList();
            if (selected.Any())
            {
                var items = selected.Select(n => new CommodityOwnerImport()
                                                                   {
                                                                       Code = n.Code,
                                                                       Description = n.Description,
                                                                       BusinessNumber = n.BusinessNumber,
                                                                       CommodityOwnerTypeName = n.CommodityOwnerTypeName,
                                                                       CommoditySupplierName = n.CommoditySupplierName,
                                                                       DateOfBirth =DateTime.Parse(n.DateOfBirth),
                                                                       Email = n.Email,
                                                                       FaxNumber = n.FaxNumber,
                                                                       FirstName = n.FirstName,
                                                                       GenderEnum =(int)Enum.Parse(typeof(Gender),n.GenderEnum),
                                                                       IdNo = n.IdNo,
                                                                       LastName = n.LastName,
                                                                       OfficeNumber = n.OfficeNumber,
                                                                       PhoneNumber = n.PhoneNumber,
                                                                       PhysicalAddress = n.PhysicalAddress,
                                                                       PinNo = n.PinNo,
                                                                       PostalAddress = n.PostalAddress,
                                                                       Surname = n.Surname,
                                                                   }).ToList();
                Import(items);
            }
        }

        internal async void Import(List<CommodityOwnerImport> commodityOwnerImportItems)
        {
            try
            {
                using (var c = NestedContainer)
                {
                    var importService = Using<ICommodityOwnerImportService>(c);
                    MainViewModel.ProsessMessage = "Validating...Please wait";
                    var result = await importService.ValidateAsync(commodityOwnerImportItems);
                    if (result.All(p => p.IsValid))
                    {
                        var items = result.Select(o => o.Entity).OfType<CommodityOwner>().ToList();
                        MainViewModel.ProsessMessage = "Validation Success..Saving entities";
                        bool isSuccess = await importService.SaveAsync(items);

                        MainViewModel.ProsessMessage = string.Format("Successfully uploaded {0} Farmers", items.Count);
                    }
                    else
                    {
                        Application.Current.Dispatcher.BeginInvoke(
                            new Action(
                                delegate
                                {
                                    using (var cont = NestedContainer)
                                    {
                                        Using<IImportValidationPopUp>(cont).ShowPopUp(
                                            result.Where(o => !o.IsValid).ToList());
                                    }
                                }));


                    }
                }
            }catch(Exception ex)
            {
                MessageBox.Show("An error occured \n Details=>" + ex.Message + ex.InnerException != null
                    ? ex.InnerException.Message:"");
            }
        }

        private void Setup()
        {
            CommodityOwnerImportVmList.Clear();
            PagedListAll.Clear();
            UploadStatusMessage = "";
            SelectedPath = Path.Combine(MainViewModel.Filepath, @"farmers.csv");
            PageTitle = "Farmers";
        }
    }

    public class CommodityOwnerImportVM : ImportItemVM
    {
        public string FirstName { get; set; }
        public string IdNo { get; set; }
        public string PinNo { get; set; }
        public string GenderEnum { get; set; }
        public string CommodityOwnerTypeName { get; set; }
        public string CommoditySupplierName { get; set; }
        public string DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }


        //Optional fields
      
        public string Surname { get; set; }
        public string LastName { get; set; }
        public string PhysicalAddress { get; set; }
        public string PostalAddress { get; set; }
        public string Email { get; set; }
        public string BusinessNumber { get; set; }
        public string FaxNumber { get; set; }
        public string OfficeNumber { get; set; }
    }
}
