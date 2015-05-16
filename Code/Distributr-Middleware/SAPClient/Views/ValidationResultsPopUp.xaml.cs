using System.Collections.Generic;
using System.Windows;
using Distributr.WSAPI.Lib.Integrations;
using Distributr_Middleware.WPF.Lib.Utils;
using Distributr_Middleware.WPF.Lib.ViewModels;

namespace SAPClient.Views
{
    /// <summary>
    /// Interaction logic for ValidationResultsPopUp.xaml
    /// </summary>
    public partial class ValidationResultsPopUp : Window, IImportValidationPopUp
    {
        private ValidationResultViewModel _vm;

        public ValidationResultsPopUp()
        {
            InitializeComponent();
            _vm = DataContext as ValidationResultViewModel;
            _vm.RequestClose += (s, e) => this.Close();

        }


        public void ShowPopUp(List<StringifiedValidationResult> resultInfos)
        {
            _vm.Load(resultInfos);
            ShowDialog();
        }

       
    }
}
