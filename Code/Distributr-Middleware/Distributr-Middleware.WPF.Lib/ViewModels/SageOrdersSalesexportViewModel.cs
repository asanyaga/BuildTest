using System;
using System.Threading.Tasks;
using System.Windows;
using Distributr_Middleware.WPF.Lib.MiddlewareServices;
using Distributr_Middleware.WPF.Lib.Utils;
using GalaSoft.MvvmLight.Command;

namespace Distributr_Middleware.WPF.Lib.ViewModels
{
    public class SageOrdersSalesexportViewModel : MiddleWareViewModelBase
    {
       public SageOrdersSalesexportViewModel()
        {
            SetUp();
        }
        private RelayCommand _exportTransactionCommand;
        public RelayCommand ExportTransactionCommand
        {
            get { return _exportTransactionCommand ?? (new RelayCommand(DownloadTransactions)); }
        }

        private RelayCommand<string> _searchCommand;
        public RelayCommand<string> SearchCommand
        {
            get { return _searchCommand ?? (new RelayCommand<string>(Search)); }
        }

        private void SetUp()
        {
            DownloadAllEnabled = true;
            DownloadSingleEnabled = true;
            ActivityMessage = "";
        }

        private void Search(string obj)
        {
            if (string.IsNullOrEmpty(obj)) return;
            try
            {
                Task.Run(async() =>
                             {
                                 DownloadSingleEnabled = false;
                    using (var c = NestedContainer)
                    {
                       await Using<ITransactionsDownloadService>(c).DownloadSaleOrOrder(obj); 
                        DownloadSingleEnabled = true;
                    }
                });

            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured Details\n" + ex.Message);
            }
            finally
            {
                DownloadAllEnabled = true;
                DownloadSingleEnabled = true;

            }
        } 

        private void DownloadTransactions()
        {
            
                try
                {
                    
                    Task.Run(() =>
                                 {
                                     using (var c = NestedContainer)
                                     {
                                         DownloadAllEnabled = false;
                                         Using<ITransactionsDownloadService>(c).DownloadSalesOrOrders();
                                         DownloadAllEnabled = true;
                                     }
                                 });



                }
                catch (Exception ex)
                {
                    DownloadAllEnabled = true;
                    MessageBox.Show("An error occured Details\n" + ex.Message);
                }
                finally
                {
                    DownloadAllEnabled = true;
                    DownloadSingleEnabled = true;
                    
                }
            
        }



        public void ReceiveMessage(string msg)
        {
            ActivityMessage += msg+"\n";
            FileUtility.LogCommandActivity(msg);
        }


        public const string DownloadAllEnabledPropertyName = "DownloadAllEnabled";
        private bool _downloadAll = false;
        public bool DownloadAllEnabled
        {
            get
            {
                return _downloadAll;
            }

            set
            {
                if (_downloadAll == value)
                {
                    return;
                }

                RaisePropertyChanging(DownloadAllEnabledPropertyName);
                _downloadAll = value;
                RaisePropertyChanged(DownloadAllEnabledPropertyName);

            }
        }

        public const string DownloadSingleEnabledPropertyName = "DownloadSingleEnabled";
        private bool _downloadSingle = false;
        public bool DownloadSingleEnabled
        {
            get
            {
                return _downloadSingle;
            }

            set
            {
                if (_downloadSingle == value)
                {
                    return;
                }

                RaisePropertyChanging(DownloadSingleEnabledPropertyName);
                _downloadSingle = value;
                RaisePropertyChanged(DownloadSingleEnabledPropertyName);

            }
        }
    }
}
