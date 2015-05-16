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
using Distributr.Core.Resources.Util;
using Distributr.WPF.Lib;
using Distributr.WPF.Lib.UI.UI_Utillity;
using Distributr.WPF.Lib.ViewModels.Admin.Users;
using StructureMap;
using ValidationResult = System.Windows.Controls.ValidationResult;

namespace Distributr.WPF.UI.Views.Administration.Users
{
    public partial class ChangeUserPassword : PageBase
    {
        EditUsersViewModel _vm;
        Guid userId;
        readonly IMessageSourceAccessor _messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();
        public ChangeUserPassword()
        {
            InitializeComponent();
            LabelControls();
            this.Loaded += new RoutedEventHandler(ChangeUserPassword_Loaded);
        }

        void LabelControls()
        {
            lblOldPassword.Content = _messageResolver.GetText("sl.changepwd.old");
            lblNewPassword.Content = _messageResolver.GetText("sl.changepwd.new");
            lblConfirmedPassword.Content = _messageResolver.GetText("sl.changepwd.confirm");
            btnChangePassword.Content = _messageResolver.GetText("sl.changepwd.save");
            btnCancel.Content = _messageResolver.GetText("sl.changepwd.cancel");
        }

        void ChangeUserPassword_Loaded(object sender, RoutedEventArgs e)
        {
            _vm = this.DataContext as EditUsersViewModel;
            try
            {
                _vm.SetUp();
                _vm.ChangingPassword = true;
                _vm.LoadByID(userId);
                txtOldPassword.Password = _vm.OldPassword;
                txtNewPassword.Password = _vm.NewPassword;
                txtPasswordConfirm.Password = _vm.ConfirmPassword;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error" + ex.Message);
            }
        }

        protected override void OnNavigatingFrom(object sender, NavigatingCancelEventArgs e)
        {
            if (_vm.ConfirmNavigatingAway)
            {
                if (MessageBox.Show("Are you sure you want to leave this page?\nUnsaved pages will be lost."
                                    , "Distributr: Change Password",
                                    MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
            }
            base.OnNavigatingFrom(sender, e);
        }   

        private void btnChangePassword_Click(object sender, RoutedEventArgs e)
        {
            _vm.OldPassword = txtOldPassword.Password.Trim();
            _vm.NewPassword = txtNewPassword.Password.Trim();
            _vm.ConfirmPassword = txtPasswordConfirm.Password.Trim();
            _vm.ChangingPassword = true;
            _vm.Save();
        }

    }
}
