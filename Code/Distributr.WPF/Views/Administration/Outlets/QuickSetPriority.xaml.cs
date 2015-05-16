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
using System.Windows.Shapes;
using Distributr.Core.Resources.Util;
using Distributr.WPF.Lib.UI.UI_Utillity;
using Distributr.WPF.Lib.ViewModels.Admin.Outlets;
using StructureMap;

namespace Distributr.WPF.UI.Views.Administration.Outlets
{
    /// <summary>
    /// Interaction logic for QuickSetPriority.xaml
    /// </summary>
    public partial class QuickSetPriority : Window
    {
        private EditOutletPriorityViewModel _vm;
        IMessageSourceAccessor _messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();
        public QuickSetPriority()
        {
            InitializeComponent();
            LabelControls();
        }

        void LabelControls()
        {
            Title = _messageResolver.GetText("sl.outletPriority.quickset.title");
            lblOutlet.Content = _messageResolver.GetText("sl.outletPriority.quickset.outlet");
            btnOk.Content = _messageResolver.GetText("sl.outletPriority.quickset.ok");
            btnCancel.Content = _messageResolver.GetText("sl.outletPriority.quickset.cancel");
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void txtPriority_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = WPFValidation.AllowNumberOnlyOnKeyDown(e);
        }

        private void txtPriority_TextChanged(object sender, TextChangedEventArgs e)
        {
            _vm = this.DataContext as EditOutletPriorityViewModel;
            if (txtPriority.Text == "" || Convert.ToInt32(txtPriority.Text) == 0)
            {
                txtPriority.Text = "1";
            }
            if (Convert.ToInt32(txtPriority.Text) > _vm.RouteOutlets.Count)
            {
                MessageBox.Show("Enter value not greater than the number of outlets.", "Distributor: Outlet Prioritization", MessageBoxButton.OK);
                txtPriority.Text = _vm.RouteOutlets.Min(n => n.Priority).ToString();
                return;
            }
            if (Convert.ToInt32(txtPriority.Text) == 0)
            {
                MessageBox.Show("Enter value not greater than zero.", "Distributor: Outlet Prioritization", MessageBoxButton.OK);
                txtPriority.Text = _vm.RouteOutlets.Min(n => n.Priority).ToString();
                return;
            }
            _vm.OutletPriorityToSet = Convert.ToInt32(txtPriority.Text);
        }

    }
}
