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
using Distributr.DataImporter.Lib.ImportService;
using Distributr.DataImporter.Lib.ViewModel;

namespace Distributr.DataImporter.UI
{
    /// <summary>
    /// Interaction logic for ImportValidationResultPopUp.xaml
    /// </summary>
    public partial class ImportValidationResultPopUp : Window, IImportValidationPopUp
    {
        private ValidationResultViewModel _vm;
        public ImportValidationResultPopUp()
        {
            InitializeComponent();
            _vm = DataContext as ValidationResultViewModel;
        }

        public void ShowPopUp(List<ImportValidationResultInfo> resultInfos)
        {
           _vm.Load(resultInfos);
            ShowDialog();
        }
    }
}
