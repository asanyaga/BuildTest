using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.DataImporter.Lib.ImportService.DiscountGroups;
using Distributr.DataImporter.Lib.ImportService.Outlets;
using Distributr.DataImporter.Lib.ImportService.PriceGroups;
using Distributr.DataImporter.Lib.ImportService.Products;
using Distributr.DataImporter.Lib.ImportService.Salesman;
using Distributr.DataImporter.Lib.ImportService.Shipping;
using Distributr.DataImporter.Lib.Utils;
using Distributr.WPF.Lib.Services.Service.Sync;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using StructureMap;

namespace Distributr.DataImporter.Lib.ViewModel
{
   public class MasterdataImportViewModelBase:ImporterViewModelBase
    {
       public RelayCommand<Page> LoadPageCommand { get; set; }

       public MasterdataImportViewModelBase()
       {
           LoadPageCommand=new RelayCommand<Page>(Load);
       }

       #region import methods

       protected virtual void Load(Page page)
       {
       }

       protected async void BeginCommandsUpload()
       {
         await  Task.Run(async () =>
           {
               using (var c = ObjectFactory.Container.GetNestedContainer())
               {
                   try
                   {
                       var sendPendingLocalCommandsService =
                           c.GetInstance<ISendPendingLocalCommandsService>();

                       int noofcmdsent =
                           await sendPendingLocalCommandsService.SendPendingCommandsAsync(200);
                       string msg = "( " + noofcmdsent + ") sent";
                       if (noofcmdsent == -1)
                       {
                           msg = "(0)  sent, check application webservice";
                       }

                       Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") +
                                              " Sending of transaction : " + msg);

                       FileUtility.LogCommandActivity(DateTime.Now.ToString("hh:mm:ss") + "Sending of transaction : " +
                                    msg);

                   }

                   catch (Exception e)
                   {
                       string error = GetError(e);
                       FileUtility.LogCommandActivity(e.Message + error);
                       Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") +
                                              " Sending  failed " + error);

                   }

               }
           });
       }

       private static string GetError(Exception he)
       {
           string error = he.Message;
           string inner1 = he.InnerException != null ? he.InnerException.Message : "";
           string inner2 = he.InnerException != null && he.InnerException.InnerException != null
                               ? he.InnerException.InnerException.Message
                               : "";
           error += "\n\t" + inner1 + "\n\t " + inner2;
           return error;
       }

       protected virtual bool ProductImport(string path)
        {
            if (!ValidateFile(path)) return false;
            using (var c = NestedContainer)
            {
                GlobalStatus = "Started importing from file :" + path;
                var importservice = Using<IProductImportService>(c);

                var data = importservice.Import(path).ToList();
                var result = importservice.Validate(data);
                if (result.All(p => p.IsValid))
                {
                    importservice.Save(result.Select(p => p.Entity).OfType<Product>().ToList());
                    return true;
                }
                else
                {
                    Using<IImportValidationPopUp>(c).ShowPopUp(result.Where(p => !p.IsValid).ToList());
                    return false;
                }
            }
        }
        protected virtual bool DiscountImport(string path)
        {
            if (!ValidateFile(path)) return false;
            using (var c = NestedContainer)
            {
                var importservice = Using<IProductDiscountGroupImportService>(c);
                try
                {

                    var data = importservice.Import(path).ToList();
                    var result = importservice.Validate(data);
                    if (result.All(p => p.IsValid))
                    {
                        importservice.Save(result.Select(p => p.Entity).OfType<ProductGroupDiscount>().ToList());
                        return true;
                    }
                    else
                    {

                        Using<IImportValidationPopUp>(c).ShowPopUp(result.Where(p => !p.IsValid).ToList());
                        ImportStatusMessage = "Error importing outlets";
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        protected virtual bool OutletImport(string fileName)
        {
            if (!ValidateFile(fileName)) return false;
            using (var c = NestedContainer)
            {
                var importservice = Using<IOutletImportService>(c);
                try
                {

                    var data = importservice.Import(fileName).ToList();
                    var result = importservice.Validate(data);
                    if (result.All(p => p.IsValid))
                    {
                        importservice.Save(result.Select(p => p.Entity).OfType<Outlet>().ToList());
                        return true;
                    }
                    else
                    {

                        Using<IImportValidationPopUp>(c).ShowPopUp(result.Where(p => !p.IsValid).ToList());
                        ImportStatusMessage = "Error importing outlets";
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        protected virtual bool ProductPricingImport(string fileName)
        {
            if (!ValidateFile(fileName)) return false;
            using (var c = NestedContainer)
            {
                var importservice = Using<IPricingImportService>(c);
                try
                {

                    var data = importservice.Import(fileName).ToList();
                    var result = importservice.Validate(data);
                    if (result.All(p => p.IsValid))
                    {
                        importservice.Save(result.Select(p => p.Entity).OfType<ProductPricing>().ToList());
                        return true;
                    }
                    else
                    {

                        Using<IImportValidationPopUp>(c).ShowPopUp(result.Where(p => !p.IsValid).ToList());
                        ImportStatusMessage = "Error importing product pricing";
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        protected virtual bool SalesmenImport(string fileName)
        {
            if (!ValidateFile(fileName)) return false;
            using (var c = NestedContainer)
            {
                var importservice = Using<IDistributorSalesmanImportService>(c);
                try
                {

                    var data = importservice.Import(fileName).ToList();
                    var result = importservice.Validate(data);
                    if (result.All(p => p.IsValid))
                    {
                        importservice.Save(result.Select(p => p.Entity).OfType<DistributorSalesman>().ToList());
                        result = importservice.ValidateUsers(data);
                        if (result.All(p => p.IsValid))
                        {
                            importservice.Save(result.Select(p => p.Entity).OfType<User>().ToList());
                            return true;
                        }
                        else
                        {
                            Using<IImportValidationPopUp>(c).ShowPopUp(result.Where(p => !p.IsValid).ToList());
                            ImportStatusMessage = "Error importing distributor salesmen";
                            return false;
                        }
                    }
                    else
                    {
                        Using<IImportValidationPopUp>(c).ShowPopUp(result.Where(p => !p.IsValid).ToList());
                        ImportStatusMessage = "Error importing distributor salesmen";
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("Error importing salesmen\nDetails:"+ex.Message);
                    return false;
                }
            }
        }
        protected virtual bool ShipToAddressImport(string fileName)
        {
            if (!ValidateFile(fileName)) return false;
            using (var c = NestedContainer)
            {
                try
                {
                    var importservice = Using<IShipToAddressImportService>(c);

                    var data = importservice.Import(fileName).ToList();
                    var result = importservice.Validate(data);
                    if (result.All(p => p.IsValid))
                    {
                        importservice.Save(result.Select(p => p.Entity).OfType<Outlet>().ToList());
                        return true;
                    }
                    else
                    {

                        Using<IImportValidationPopUp>(c).ShowPopUp(result.Where(p => !p.IsValid).ToList());
                        ImportStatusMessage = "Error importing shpping Addresses";
                        return false;
                    }
                }
                catch (Exception ex)
                {

                    return false;
                }
            }
        }

        #endregion
    }
}
