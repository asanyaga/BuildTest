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
using Distributr.WPF.Lib.ViewModels.Admin.Users;
using StructureMap;

namespace Distributr.WPF.UI.Views.Administration.Users
{
    /// <summary>
    /// Interaction logic for AddSalesmanRouteModal.xaml
    /// </summary>
    public partial class AddSalesmanRouteModal : Window
    {
        private IMessageSourceAccessor _messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();
        public AddSalesmanRouteModal()
        {
            InitializeComponent();
            LabelControls();
        }

        private void LabelControls()
        {
            Title = _messageResolver.GetText("sl.userroute.modal.title");
            lblRoute.Content = _messageResolver.GetText("sl.userroute.modal.route");
            OKButton.Content = _messageResolver.GetText("sl.userroute.modal.ok");
            CancelButton.Content = _messageResolver.GetText("sl.userroute.modal.cancel");
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

      
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           

        }
    }
}
