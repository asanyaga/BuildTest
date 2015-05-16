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
using Distributr.WPF.Lib.ViewModels.Transactional.DisbursementNotes;
using StructureMap;

namespace Distributr.WPF.UI.Views.ReceiveReturnable
{
    public partial class AddReturnableModal : Window
    {
        private IMessageSourceAccessor messageResolver;
        public AddReturnableModal()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(AddReturnableModal_Loaded);
        }

        void AddReturnableModal_Loaded(object sender, RoutedEventArgs e)
        {
            LocalizeLabel();
        }

        private void LocalizeLabel()
        {
            messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();
            this.Title = messageResolver.GetText("sl.inventory.receive.returnable.modal.title");
            lblproduct.Content = messageResolver.GetText("sl.inventory.receive.returnable.modal.product");
            lblquantity.Content = messageResolver.GetText("sl.inventory.receive.returnable.modal.quantity");
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
           this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult retult = MessageBox.Show("Are you sure you want to cancel receiveing returnables?",
                                                      "Recieve Returnables", MessageBoxButton.OKCancel);
            if (retult == MessageBoxResult.OK)
            {
                this.DialogResult = false;
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = WPFValidation.AllowNumberOnlyOnKeyDown(e);
        }
    }
}
