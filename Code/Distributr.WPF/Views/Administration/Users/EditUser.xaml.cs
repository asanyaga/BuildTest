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
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Resources.Util;
using Distributr.WPF.Lib;
using Distributr.WPF.Lib.UI.UI_Utillity;
using Distributr.WPF.Lib.ViewModels.Admin.Users;
using GalaSoft.MvvmLight.Messaging;
using StructureMap;
using ValidationResult = System.Windows.Controls.ValidationResult;

namespace Distributr.WPF.UI.Views.Administration.Users
{public partial class EditUser : PageBase
    {
        EditUsersViewModel _vm;
        Guid userId;
        bool isInitialized = false;
        private IMessageSourceAccessor _messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();

        public EditUser()
        {
            isInitialized = false;
            InitializeComponent();
            isInitialized = true;
            LabelControls();
            this.Loaded += new RoutedEventHandler(EditUser_Loaded);
        }

        void LabelControls()
        {
            lblUsername.Content = _messageResolver.GetText("sl.user.edit.name");
            lblCode.Content = _messageResolver.GetText("sl.user.edit.code");
            lblPin.Content = _messageResolver.GetText("sl.user.edit.pin");
            lblMobile.Content = _messageResolver.GetText("sl.user.edit.mobile");
            lblType.Content = _messageResolver.GetText("sl.user.edit.type");
            lblTill.Content = _messageResolver.GetText("sl.user.edit.till");
            btnSaveUser.Content = _messageResolver.GetText("sl.user.edit.save");
        }

        void EditUser_Loaded(object sender, RoutedEventArgs e)
        {
            _vm = this.DataContext as EditUsersViewModel;
            if (NavigationService != null) userId = PresentationUtility.ParseIdFromUrl(NavigationService.CurrentSource);
            _vm.SetUp();
            _vm.ChangingPassword = false;
            _vm.LoadByID(userId);
        }

        protected override void OnNavigatingFrom(object sender, NavigatingCancelEventArgs e)
        {
            if (_vm.ConfirmNavigatingAway)
            {
                if (
                    MessageBox.Show(
                        "Are you sure you want to navigate away from this page?\n Unsaved changes will be lost",
                        "Distributr: User Management", MessageBoxButton.YesNo) == MessageBoxResult.No)
                    e.Cancel = true;
            }
            base.OnNavigatingFrom(sender, e);
        }

        private void txtUserName_KeyUp(object sender, KeyEventArgs e)
        {
            WPFValidation.InvalidateSpecialCharactersOnKeyUp(sender);
        }

        private void txtUserPIN_KeyUp(object sender, KeyEventArgs e)
        {
            WPFValidation.InvalidateSpecialCharactersOnKeyUp(sender);
        }

        private void txtUserMobile_KeyUp(object sender, KeyEventArgs e)
        {
            WPFValidation.InvalidateSpecialCharactersOnKeyUp(sender);
        }

        private void txtTillNumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.Back || e.Key == Key.Tab))

                e.Handled = false;

            else
            {
                e.Handled = true;

            }
        }

        private void cmbUserTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (!isInitialized)
            //    return;
            //if (_vm.SelectedUserType == UserType.DistributorSalesman)
            //{
            //    txtCode.IsReadOnly = true;
            //    txtCode.Background = Brushes.Gray;
            //}
            //else
            //{
            //    txtCode.IsReadOnly = false;
            //    txtCode.Background = Brushes.White;
            //}
            //txtCode.UpdateLayout();
        }
    }
}
