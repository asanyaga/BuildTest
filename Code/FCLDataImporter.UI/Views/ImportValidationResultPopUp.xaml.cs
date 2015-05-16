using System;
using System.Collections.Generic;
using System.Windows;
using Distributr.DataImporter.Lib.ImportService;
using Distributr.DataImporter.Lib.ViewModel;

namespace FCLDataImporter.UI.Views
{
    [Obsolete("Use ValidationResultPopUp.xaml")]
    public partial class ImportValidationResultPopUp : Window, IImportValidationPopUp
    {
        private  ValidationResultViewModel _vm;
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
