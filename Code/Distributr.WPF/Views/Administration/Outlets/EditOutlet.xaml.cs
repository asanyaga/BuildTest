using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
using Distributr.Core.Domain.Master;
using Distributr.Core.Resources.Util;
using Distributr.WPF.Lib;
using Distributr.WPF.Lib.UI.UI_Utillity;
using Distributr.WPF.Lib.ViewModels.Admin.Outlets;
using GalaSoft.MvvmLight.Messaging;
using StructureMap;
using ValidationResult = System.Windows.Controls.ValidationResult;

namespace Distributr.WPF.UI.Views.Administration.Outlets
{
    public partial class EditOutlet : PageBase
    {
        EditOutletViewModel _vm;
        private Guid OutletId { get; set; }
        IMessageSourceAccessor _messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();
        public EditOutlet()
        {
            InitializeComponent();
            _vm = this.DataContext as EditOutletViewModel;
           this.Loaded += new RoutedEventHandler(EditOutlet_Loaded);
            this.Unloaded += new RoutedEventHandler(EditOutlet_Unloaded);
        }

       void EditOutlet_Loaded(object sender, RoutedEventArgs e)
        {
            OutletId = PresentationUtility.ParseIdFromUrl(NavigationService.CurrentSource);

            _vm.SetUp();
            _vm.LoadById(OutletId);
        }

        void EditOutlet_Unloaded(object sender, RoutedEventArgs e)
        {
        }

        protected override void OnNavigatingFrom(object sender, NavigatingCancelEventArgs e)
        {

            if (!_vm.CanManageOutlet)
                return;

            if (_vm.ConfirmNavigatingAway)
            {
                if (
                    MessageBox.Show(
                        "Are you sure you want to navigate away from this page?\n Unsaved changes will be lost",
                        "Distributr: Outlet Management", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                    e.Cancel = true;
            }
            base.OnNavigatingFrom(sender, e);
        }

        private void txtName_KeyUp(object sender, KeyEventArgs e)
        {
            WPFValidation.InvalidateSpecialCharactersOnKeyUp(sender);
        }

        private void chkApproved_Click(object sender, RoutedEventArgs e)
        {
            if (_vm.Id == Guid.Empty)
                return;
            if (((CheckBox)sender).IsChecked == true)
            {
                if (
                    MessageBox.Show(/*"Are you sure you want to approve this outlet?"*/
                    _messageResolver.GetText("sl.outlet.edit.approve.messagebox.propmt")
                    , _messageResolver.GetText("sl.outlet.edit.approve.messagebox.text") /*"Distributr: Approve Outlet"*/
                    , MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    ((CheckBox) sender).IsChecked = false;
                }
            }
        }
    }
}
