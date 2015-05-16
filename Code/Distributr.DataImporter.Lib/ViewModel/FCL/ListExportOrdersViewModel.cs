using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Distributr.DataImporter.Lib.ImportService.Orders;
using Distributr.DataImporter.Lib.Utils;

namespace Distributr.DataImporter.Lib.ViewModel.FCL
{
    public class ListExportOrdersViewModel : MasterdataImportViewModelBase
    {
        
       
        #region properties

      
       public const string StartDatePropertyName = "StartDate";
        private DateTime _startDate = DateTime.Now;
        public DateTime StartDate
        {
            get { return _startDate; }

            set
            {
                if (_startDate == value)
                {
                    return;
                }

                RaisePropertyChanging(StartDatePropertyName);
                _startDate = value;
                RaisePropertyChanged(StartDatePropertyName);
            }
        }
        

        public const string EndDatePropertyName = "EndDate";
        private DateTime _endDate = DateTime.Now;
        public DateTime EndDate
        {
            get { return _endDate; }

            set
            {
                if (_endDate == value)
                {
                    return;
                }

                RaisePropertyChanging(EndDatePropertyName);
                _endDate = value;
                RaisePropertyChanged(EndDatePropertyName);
            }
        }

     
        
        
        
        #endregion

        #region methods
        public void ReceiveMessage(string msg)
        {
            ExportActivityMessage += "\n" + msg;
            FileUtility.LogExportActivity(msg);

        }

        protected override  void Load(Page page)
        {
             Setup();
            ExecuteExportActivity();
        }

        private async void ExecuteExportActivity()
        {
            await Task.Run(() =>
                               {
                                   using (var c=NestedContainer)
                                   {
                                       var exportService = Using<IOrderExportService>(c);
                                       exportService.ExportOrders();
                                       
                                   }
                                   
                               });
        }

      private void Setup()
        {
            ExportActivityMessage = "";
            
          
        }

       
        #endregion
        
    }
   
}
