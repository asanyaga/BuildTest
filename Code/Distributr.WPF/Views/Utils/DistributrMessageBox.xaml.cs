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
using Distributr.WPF.Lib.ViewModels.Utils;

namespace Distributr.WPF.UI.Views.Utils
{
    /// <summary>
    /// Interaction logic for DistributrMessageBox.xaml
    /// </summary>
    public partial class DistributrMessageBox : Window
    {
        private DistributrMessageBoxViewModel _vm;
        public DistributrMessageBox(bool showNewButton = true, bool showHomeButton = true, bool showOKButton = true, bool showCancelButton = true, bool showActin1Button = true, string newButtonText = "New", string homeButtonText = "Home", string OKButtonText = "OK", string cancelButtonText = "Cancel", string action1ButtonText = "View List")
        {
            InitializeComponent();
            _vm = this.DataContext as DistributrMessageBoxViewModel;
            _vm.ShowCancelButton = showCancelButton;
            _vm.ShowHomeButton = showHomeButton;
            _vm.ShowNewButton = showNewButton;
            _vm.ShowOKButton = showOKButton;
            _vm.ShowAction1Button = showActin1Button;

            _vm.NewButtonText = newButtonText;
            _vm.HomeButtonText = homeButtonText;
            _vm.OKButtonText = OKButtonText;
            _vm.CancelButtonText = cancelButtonText;
            _vm.Action1ButtonText = action1ButtonText;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            _vm = DataContext as DistributrMessageBoxViewModel;
            _vm.Command = DistributrMessageBoxViewModel.CommandToExcecute.OKButtonClickedCommand;
            _vm.DialogResult = true;
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _vm = DataContext as DistributrMessageBoxViewModel;
            _vm.Command = DistributrMessageBoxViewModel.CommandToExcecute.CancelButtonClickedCommand;
            _vm.DialogResult = true;
            this.DialogResult = true;
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            _vm = DataContext as DistributrMessageBoxViewModel;
            _vm.Command = DistributrMessageBoxViewModel.CommandToExcecute.NewButtonClickedCommand;
            _vm.DialogResult = true;
            this.DialogResult = true;
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            _vm = DataContext as DistributrMessageBoxViewModel;
            _vm.Command = DistributrMessageBoxViewModel.CommandToExcecute.HomeButtonClickedCommand;
            _vm.DialogResult = true;
            this.DialogResult = true;
        }

        private void ActionButton_Click(object sender, RoutedEventArgs e)
        {
            _vm = DataContext as DistributrMessageBoxViewModel;
            _vm.Command = DistributrMessageBoxViewModel.CommandToExcecute.Action1ButtonClickedCommand;
            _vm.DialogResult = true;
            this.DialogResult = true;
        }
    }
}
