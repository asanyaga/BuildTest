using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Distributr.DataImporter.Lib.ViewModel;

namespace FCLDataImporter.UI.Views
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

        
        public void ShowPopUp(List<Distributr.DataImporter.Lib.ImportService.ImportValidationResultInfo> resultInfos)
        {
            _vm.Load(resultInfos);
            ShowDialog();
        }
    }
}
