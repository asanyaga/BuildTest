using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace Agrimanagr.DataImporter.Lib.ViewModels
{
    public abstract class ImportItemVM : ViewModelBase
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int SequenceNo { get; set; }

        public const string IsCheckedPropertyName = "IsChecked";
        private bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }

            set
            {
                if (_isChecked == value)
                {
                    return;
                }

                RaisePropertyChanging(IsCheckedPropertyName);
                _isChecked = value;
                RaisePropertyChanged(IsCheckedPropertyName);
            }
        }
    }
}
