using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Distributr.WPF.Lib;
using Distributr.WPF.Lib.UI.UI_Utillity;
using Distributr.WPF.Lib.ViewModels.Reports;

namespace Distributr.WPF.UI.Views.Reports
{
    /// <summary>
    /// Interaction logic for DocumentsReport.xaml
    /// </summary>
    public partial class DocumentsReport : Page
    {
        ProductTransactionsViewModel vm;
        public DocumentsReport()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(DocumentsReport_Loaded);
        }

        void DocumentsReport_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                vm = DataContext as ProductTransactionsViewModel;
                string _productId = NavigationService.Source.OriginalString.ParseQueryString("ProductId");
                if (!string.IsNullOrWhiteSpace(_productId))
                    _productId = PresentationUtility.ParseQueryString(NavigationService.CurrentSource, "ProductId");
                if (!string.IsNullOrEmpty(_productId))
                {
                    string ProductId = _productId;
                    vm.ProductId = Guid.Parse(ProductId);
                    vm.LoadDocumentsCommand.Execute(null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
