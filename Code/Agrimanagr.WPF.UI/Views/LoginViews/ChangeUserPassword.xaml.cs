using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Navigation;
using Distributr.Core.Resources.Util;
using Distributr.WPF.Lib.UI.UI_Utillity;
using Distributr.WPF.Lib.ViewModels.Admin.Users;
using StructureMap;

namespace Agrimanagr.WPF.UI.Views.LoginViews
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
           // base.OnNavigatingFrom(sender, e);
        }   

        private void btnChangePassword_Click(object sender, RoutedEventArgs e)
        {
            _vm.OldPassword = txtOldPassword.Password.Trim();
            _vm.NewPassword = txtNewPassword.Password.Trim();
            _vm.ConfirmPassword = txtPasswordConfirm.Password.Trim();
            _vm.ChangingPassword = true;

            Regex regex = new Regex(@"^.*(?=.{6,20})(?=.*\d)(?=.*[a-zA-Z]).*$");
            if (regex.Match(txtNewPassword.Password).Success)
            {
                _vm.Save();
            }
            else
            {
                MessageBox.Show("Password Must Contain Numbers and Letter", "Agrimanager Error", MessageBoxButton.OK);
            }
            
        }

    }
}
