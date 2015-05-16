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
using Distributr.WPF.Lib.ViewModels.Admin.Contacts;
using StructureMap;
using ValidationResult = System.Windows.Controls.ValidationResult;

namespace Distributr.WPF.UI.Views.Administration.Contacts
{
    public partial class EditContact : Page
    {
       private EditContactViewModel _vm;
        IMessageSourceAccessor _messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();
     
        public EditContact()
        {
           
            InitializeComponent();
         
        }

        //protected override void OnNavigatingFrom(object sender, NavigatingCancelEventArgs e)
        //{
        //    if (_vm.ConfirmNavigatingAway)
        //    {
        //        if (
        //           MessageBox.Show(/*"Are you sure you want to move away from this page?\n" +
        //                        "Unsaved changes will be lost"*/
        //                        _messageResolver.GetText("sl.contacts.edit.cancel.messagebox.propmt"),
        //                        _messageResolver.GetText("sl.contacts.edit.navigateaway.messagebox.caption")/*"Distributr: Confirm Navigating Away"*/
        //                    , MessageBoxButton.YesNo) ==
        //            MessageBoxResult.No)
        //        e.Cancel = true;
        //    }
        //    base.OnNavigatingFrom(sender, e);
        //}

    
        private void txtName_KeyUp(object sender, KeyEventArgs e)
        {
            WPFValidation.InvalidateSpecialCharactersOnKeyUp(sender, "!@#$%^&*+=[]\\;/{}|\":<>?");
        }

        private void txtEmail_KeyUp(object sender, KeyEventArgs e)
        {
            WPFValidation.InvalidateSpecialCharactersOnKeyUp(sender, "!#$%^&*()+=[]\\;,/{}|\":<>?");
        }

        private void txtPhysicalAddress_KeyUp(object sender, KeyEventArgs e)
        {
            WPFValidation.InvalidateSpecialCharactersOnKeyUp(sender, "!#$%^&*+=[]/{}|<>?");
        }

      
        
    }
}
