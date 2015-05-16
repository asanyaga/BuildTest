using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using Distributr.DataImporter.Lib.ImportService.Orders;
using Distributr.DataImporter.Lib.Utils;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;

namespace Distributr.DataImporter.Lib.ViewModel.FCL
{
    public class TransactionsExportViewModel : MasterdataImportViewModelBase
    {
       public RelayCommand<SelectionChangedEventArgs> TabSelectionChangedCommand { get; set; }
       public TransactionsExportViewModel()
       {
           TabSelectionChangedCommand=new RelayCommand<SelectionChangedEventArgs>(TabSelectionChanged);
       }
       internal void ReceiveMessage(string msg)
       {
           ExportActivityMessage += "\n" + msg;
           FileUtility.LogCommandActivity(msg);
       }
       protected  void TabSelectionChanged(SelectionChangedEventArgs eventArgs)
       {
           Dispatcher.CurrentDispatcher.BeginInvoke(new Action(delegate
           {
               if (eventArgs.Source.GetType() != typeof(TabControl))
                   return;

               TabItem tabItem = eventArgs.AddedItems[0] as TabItem;
               LoadSelectedTab(tabItem);
               eventArgs.Handled = true;

           }));
       }

       private void LoadSelectedTab(TabItem selectedTab)
       {
           ExportActivityMessage = "";
           switch (selectedTab.Name)
           {
               case "ordersTab":
                   DoOrdersExport();
                   break;
               case "salesTab":
                   DoSalesExport();
                   break;
               case "paymentsTab":
                   DoPaymentsExport();
                   break;
               case "inventoryIssuesTab":
                   DoInventoryIssue();
                   break;
               case "commandsTab":
                   base.BeginCommandsUpload();
                   break;
           }
       }

       private async void DoInventoryIssue()
       {
         await  Task.Run(() => {
             Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") +
                                                        string.Format(" Loading inventory isssues started..."));
           var viewModel = SimpleIoc.Default.GetInstance<ImportStockLineViewModel>();
           viewModel.LoadPageCommand.Execute(null);
           });
       }

       private async void DoPaymentsExport()
       {
           await Task.Run(() =>
                              {
                                  Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") +
                                                         string.Format(" Loading payments started..."));
                                  using (var c = NestedContainer)
                                  {
                                      Using<ISalesExportService>(c).ExportPayments();

                                  }
                              });
       }

       private async void DoSalesExport()
       {
           await Task.Run(() =>
                              {
                                  Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") +
                                                         string.Format(" Loading sales started..."));

                                  using (var c = NestedContainer)
                                  {
                                      Using<ISalesExportService>(c).ExportSales();

                                  }
                              });
       }

       private async void DoOrdersExport()
       {
           await Task.Run(() =>
                              {
                                  Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") +
                                                         string.Format(" Loading orders started..."));
                                  var viewModel = SimpleIoc.Default.GetInstance<ListExportOrdersViewModel>();
                                  viewModel.LoadPageCommand.Execute(null);
                              });
       }

       
    }
}
