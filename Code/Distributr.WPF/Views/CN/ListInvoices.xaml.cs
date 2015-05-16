using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Distributr.Core.Resources.Util;
using Distributr.WPF.Lib.ViewModels.Transactional.CN;

using StructureMap;

namespace Distributr.WPF.UI.Views.CN
{
    /// <summary>
    /// Interaction logic for ListInvoices.xaml
    /// </summary>
    public partial class ListInvoices : Page
    {
        ListInvoicesViewModel _vm = null;
        IMessageSourceAccessor _messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();
        public ListInvoices()
        {

            InitializeComponent();
            Loaded += new RoutedEventHandler(ListInvoices_Loaded);
            LabelControls();
            SetUpDataPager();
        }

        void LabelControls()
        {
            lblSearchText.Content = _messageResolver.GetText("sl.creditnote.invoicelist.invoicenumber");
            btnSearch.Content = _messageResolver.GetText("sl.creditnote.invoicelist.search");

            //grid
            colInvoiceRef.Header = _messageResolver.GetText("sl.creditnote.invoicelist.grid.col.header.invoiceref");
            colTotalNet.Header = _messageResolver.GetText("sl.creditnote.invoicelist.grid.col.header.totalnet");
            colTotalVat.Header = _messageResolver.GetText("sl.creditnote.invoicelist.grid.col.header.totalvat");
            colTotalGross.Header = _messageResolver.GetText("sl.creditnote.invoicelist.grid.col.header.totalgross");
        }

        private void InvoicesDG_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DataGridRow dataGridrow = DataGridRow.GetRowContainingElement(e.Row);

        //    Hyperlink edit = colIssue").GetCellContent(dataGridrow) as Hyperlink;
        //    edit.Content = _messageResolver.GetText("sl.creditnote.invoicelist.grid.col.issue.text");
        //    HyperlinkButton deactivate = colViewInvoice").GetCellContent(dataGridrow) as HyperlinkButton;
        //    deactivate.Content = _messageResolver.GetText("sl.creditnote.invoicelist.grid.col.view.text");
        }

        void ListInvoices_Loaded(object sender, RoutedEventArgs e)
        {
            _vm = DataContext as ListInvoicesViewModel;
           // _vm.ClearAndSetup();
            LoadInvoices();
        }

        void LoadInvoices()
        {
            lblProgress.Visibility = Visibility.Visible;
            _vm.LoadInvoices.Execute(null);
            InvoicesDG.ItemsSource = _vm.InvoicesList;

            DataPager.txtTotal.Text = _vm.PageCount.ToString();
            DataPager.lblTotalItems.Content = _vm.RecordsCount.ToString();
            DataPager.EnableOrDisableButtons(_vm.CurrentPage, _vm.PageCount);

            lblProgress.Visibility = Visibility.Collapsed;
        }

        

       

        
        private void cmdLoadSales_Click(object sender, RoutedEventArgs e)
        {
            txtSearchText.Text = "";
            LoadInvoices();
        }

        private void txtSearchText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtSearchText.Text.Trim() != "")
            {
                btnSearch_Click(this, null);
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (_vm == null)
                _vm = DataContext as ListInvoicesViewModel;
            _vm.CurrentPage = 1;
            LoadInvoices();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtSearchText.Text = "";
            if(_vm==null)
                _vm = DataContext as ListInvoicesViewModel;
            _vm.SearchText = "";
            LoadInvoices();

        }

        void SetUpDataPager()
        {
            DataPager.btnFirst.Click += new RoutedEventHandler(btnFirst_Click);
            DataPager.btnPrevious.Click += new RoutedEventHandler(btnPrevious_Click);
            DataPager.btnNext.Click += new RoutedEventHandler(btnNext_Click);
            DataPager.btnLast.Click += new RoutedEventHandler(btnLast_Click);
            DataPager.btnGoTo.Click += new RoutedEventHandler(btnGoTo_Click);
        }

        void btnGoTo_Click(object sender, RoutedEventArgs e)
        {
            if (_vm == null)
                _vm = DataContext as ListInvoicesViewModel;
            _vm.CurrentPage = Convert.ToInt32(DataPager.txtPage.Text);
            LoadInvoices();
        }

        void btnFirst_Click(object sender, RoutedEventArgs e)
        {
            if (_vm == null)
                _vm = DataContext as ListInvoicesViewModel;
            _vm.CurrentPage = 1;
            DataPager.txtPage.Text = _vm.CurrentPage.ToString();
            LoadInvoices();
        }

        void btnLast_Click(object sender, RoutedEventArgs e)
        {
            if (_vm == null)
                _vm = DataContext as ListInvoicesViewModel;
            _vm.CurrentPage = _vm.PageCount;
            DataPager.txtPage.Text = _vm.CurrentPage.ToString();
            LoadInvoices();
        }

        void btnNext_Click(object sender, RoutedEventArgs e)
        {
            _vm.CurrentPage++;
            DataPager.txtPage.Text = _vm.CurrentPage.ToString();
            LoadInvoices();
        }

        void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            _vm.CurrentPage--;
            DataPager.txtPage.Text = _vm.CurrentPage.ToString();
            LoadInvoices();
        }

        private void hlIssueCredintNote_Click(object sender, RoutedEventArgs e)
        {
            var hl = sender as Hyperlink;
            string url = "/Views/CN/AddCreditNote.xaml?InvoiceNo=" + hl.Tag.ToString();
            NavigationService.Navigate(new Uri(url, UriKind.Relative));
        }

        private void hlViewInvoice_Click(object sender, RoutedEventArgs e)
        {
            var hl = sender as Hyperlink;
            string url = "/views/invoicedocument/invoicedocument.xaml?orderid=" + hl.Tag.ToString();
            NavigationService.Navigate(new Uri(url, UriKind.Relative));
        }
    }
}
