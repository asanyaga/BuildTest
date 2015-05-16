using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Agrimanagr.WPF.UI.Views.CommodityPurchase;
using Distributr.Core.Domain.Master.CoolerEntities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.WPF.Lib.UI.Pages;
using Distributr.WPF.Lib.UI.UI_Utillity;
using Distributr.WPF.Lib.ViewModels.Transactional.ReceiptDocument;

namespace Agrimanagr.WPF.UI.Views.ReceiptView
{
    public partial class ReceiptDocument : Window, IReceiptDocumentPopUp
    {
        private AgrimanagrReceiptDocumentViewModel _vm;
        public UserControl Parent { get; set; }

        public ReceiptDocument()
        {
            InitializeComponent();
            double height = SystemParameters.PrimaryScreenHeight - 40;
            this.Height = height;
            _vm = DataContext as AgrimanagrReceiptDocumentViewModel;

        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            /*
            ListFarmers listFarmers = new ListFarmers();
            CommodityHome rootWindow = UserControlNavigationHelper.FindParentByType<CommodityHome>(Parent);
            rootWindow.ctrlPurchase.Content = listFarmers;
             * */
            Close();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            PrintToLaserPrinter();
            //PrintToConfiguredPrinter();
        }

        void PrintToConfiguredPrinter()
        {
            //_vm = DataContext as ReceiptDocumentViewModel;
            //serialPortUtil = new SerialPortUtil();
            //HubDevice hubDevice = new HubDevice();
            //try
            //{
            //    if (hubDevice.GetDevice(AssetType.Printer, "Select Printer"))
            //    {
            //        serialPortUtil.BaudRate = 9600; // HubDevice.BaudRate;// 9600;

            //        serialPortUtil.HandShake = Handshake.None;
            //        serialPortUtil.Parity = Parity.None;
            //        serialPortUtil.Name = HubDevice.Port; // "COM4"; portName.FirstOrDefault();// "COM4";
            //        if (serialPortUtil.Name == null)
            //            return;
            //        serialPortUtil.StopBits = StopBits.One;
            //        serialPortUtil.DataBits = 8;

            //        //bool write = serialPortUtil.Write(_editCommodityPurchaseViewModel.LastTransactionDocumentId);
            //        bool write = serialPortUtil.Write(_vm.ReceiptIdLookup, _vm.doc);
            //        if (!write)
            //            MessageBox.Show("Could not print");

            //        serialPortUtil.DoClose();

            //    }
            //    else
            //    {
            //        MessageBox.Show("No Printer Scale Found", "Printer Scale");
            //    }
            //}
            //catch { }
        }

        void PrintToLaserPrinter()
        {
            try
            {
                PrintDialog dialog = new PrintDialog();
                if (dialog.ShowDialog() == true)
                {
                    dialog.PrintVisual(gridPrintArea,
                                       "Receipt" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + "-" +
                                       DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year);

                    if (MessageBox.Show(
                        "Receipt printed." + "\nClick OK to view purchase summary, Cancel to remain on receipt."
                        , "Agrimanagr: Transaction Receipt"
                        , MessageBoxButton.OKCancel
                        , MessageBoxImage.Information) == MessageBoxResult.OK)
                        btnBack_Click(this, null);
                }
            }
            catch
            {
                MessageBox.Show("No Printer Scale Found", "Printer Scale");
            }
        }

        public void ShowReceipt(CommodityPurchaseNote commodityPurchaseNote)
        {
           _vm.LoadReceipt(commodityPurchaseNote);
            this.ShowDialog();
        }
    }

}
