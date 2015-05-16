using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Distributr.WSAPI.Lib.Integrations;
using GalaSoft.MvvmLight.Command;

namespace Distributr_Middleware.WPF.Lib.ViewModels
{
    public class ValidationResultViewModel : MiddleWareViewModelBase
    {
        public ValidationResultViewModel()
        {
            ErrorList = new ObservableCollection<ValidationResultItem>();
            ClosePopUpCommand = new RelayCommand(Cancel);
        }

        protected void Cancel()
        {
            RequestClose(this, null);
        }


        public ObservableCollection<ValidationResultItem> ErrorList { get; set; }
        public RelayCommand ClosePopUpCommand { get; set; }
        public event EventHandler RequestClose = (s, e) => { };

        public void Load(List<StringifiedValidationResult> resultInfos)
        {
            ErrorList.Clear();
            foreach (var info in resultInfos)
            {
                var item = new ValidationResultItem()
                {
                    Description = info.Description,
                    Result = GenerateResults(info.Results)
                };

                ErrorList.Add(item);
            }
        }

        private string GenerateResults(IEnumerable<string> list)
        {
            string results = "";
            int count = 1;
            foreach (var result in list)
            {
                results += count + " . " + result + " ";
                count++;
            }
            return results;
        }
    }

    public class ValidationResultItem
    {
        public string Description { get; set; }
        public string Result { get; set; }
    }
}
