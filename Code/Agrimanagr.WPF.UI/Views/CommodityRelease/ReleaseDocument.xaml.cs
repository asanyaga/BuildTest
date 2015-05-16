using System;
using System.Windows;
using System.Windows.Controls;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.WPF.Lib.UI.Pages;
using Distributr.WPF.Lib.ViewModels.Transactional.InventoryTransfer;

namespace Agrimanagr.WPF.UI.Views.CommodityRelease
{
    public partial class ReleaseDocument : Window, IReleaseDocumentPopUp
    {
        private CommodityReleaseDocumentViewModel _vm;
        public ReleaseDocument()
        {
            InitializeComponent();
            _vm = this.DataContext as CommodityReleaseDocumentViewModel;
        }

        void PrintToLaserPrinter()
        {
            try
            {
                PrintDialog dialog = new PrintDialog();
                if (dialog.ShowDialog() == true)
                {
                    dialog.PrintVisual(gridPrintArea,
                                       "Release document" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + "-" +
                                       DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year);

                    if (MessageBox.Show(
                        "Release document printed." 
                        , "Agrimanagr: Release"
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
        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            PrintToLaserPrinter();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public void ShowReleaseDocument(CommodityReleaseNote retreavcommodityReleaseNote)
        {
            _vm.LoadReleaseDocument(retreavcommodityReleaseNote);
            ShowDialog();
        }
    }
}