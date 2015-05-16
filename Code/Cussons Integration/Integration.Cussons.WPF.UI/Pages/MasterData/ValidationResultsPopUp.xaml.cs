using System.Collections.Generic;
using System.Windows;
using Integration.Cussons.WPF.Lib.CussonsIntegrationViewModels.MasterData;
using Integration.Cussons.WPF.Lib.ImportService;
using Integration.Cussons.WPF.Lib.Utils;

namespace Integration.Cussons.WPF.UI.Pages.MasterData
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


        public void ShowPopUp(List<ImportValidationResultInfo> resultInfos)
        {
            _vm.Load(resultInfos);
            ShowDialog();
        }
    }
}
