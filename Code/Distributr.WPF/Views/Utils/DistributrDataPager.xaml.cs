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
using Distributr.WPF.Lib.UI.UI_Utillity;

namespace Distributr.WPF.UI.Views.Utils
{
    /// <summary>
    /// Interaction logic for DistributrDataPager.xaml
    /// </summary>
    public partial class DistributrDataPager : UserControl
    {
        bool isInitialized = false;
        public DistributrDataPager()
        {
            isInitialized = false;
            InitializeComponent();
            isInitialized = true;
            btnFirst.IsEnabled = false;
            btnPrevious.IsEnabled = false;
        }

        public void EnableOrDisableButtons(int currentPage, int pageCount)
        {
            if (currentPage == 1)//on first page
            {
                btnFirst.IsEnabled = false;
                btnPrevious.IsEnabled = false;
                btnNext.IsEnabled = true;
                btnLast.IsEnabled = true;
                btnGoTo.IsEnabled = true;
                txtPage.IsReadOnly = false;
            }
            if (currentPage == pageCount)//on last page
            {
                btnFirst.IsEnabled = true;
                btnPrevious.IsEnabled = true;
                btnNext.IsEnabled = false;
                btnLast.IsEnabled = false;
                btnGoTo.IsEnabled = true;
                txtPage.IsReadOnly = false;
            }
            if (pageCount == 1)
            {
                btnFirst.IsEnabled = false;
                btnPrevious.IsEnabled = false;
                btnNext.IsEnabled = false;
                btnLast.IsEnabled = false;
                btnGoTo.IsEnabled = false;
                txtPage.IsReadOnly = true;
            }
            if (currentPage < pageCount && currentPage > 1)
            {
                btnFirst.IsEnabled = true;
                btnPrevious.IsEnabled = true;
                btnNext.IsEnabled = true;
                btnLast.IsEnabled = true;
                btnGoTo.IsEnabled = true;
                txtPage.IsReadOnly = false;
            }
        }

        private void txtPage_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!isInitialized)
                return;

            if (txtPage.Text.Trim() == "")
                txtPage.Text = "1";

            if (Convert.ToInt32(txtPage.Text.Trim()) > Convert.ToInt32(txtTotal.Text.Trim()))
                txtPage.Text = txtTotal.Text.Trim();
            if (Convert.ToInt32(txtPage.Text.Trim()) == 0)
                txtPage.Text = "1";

            if (txtTotal.Text.Trim() == "1")
                txtPage.IsReadOnly = true;
            else
                txtPage.IsReadOnly = false;
        }

        private void txtPage_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = WPFValidation.AllowNumberOnlyOnKeyDown(e);
        }
    }
}
