using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace Distributr.WPF.Lib.ViewModels.TestFrames
{
    public class Test1ViewModel : DistributrViewModelBase
    {

        private RelayCommand _mybuttonClick = null;
        public RelayCommand MyButtonClick
        {
            get
            {
                if(_mybuttonClick == null)
                    _mybuttonClick = new RelayCommand(()=>
                                                          {
                                                              Uri uri = new Uri("/Views/TestFrames/TestPage2.xaml", UriKind.Relative);
                                                              SendNavigationRequestMessage(uri);
                                                          });
                return _mybuttonClick;
                
            }
        }

        public const string NamePropertyName = "Name";
        private string _name = "";
        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                if (_name == value)
                {
                    return;
                }

                var oldValue = _name;
                _name = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(NamePropertyName);

            }
        }
    }
}
