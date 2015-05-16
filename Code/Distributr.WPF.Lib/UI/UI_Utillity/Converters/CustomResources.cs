using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using Distributr.Core.Resources.Util;
using StructureMap;

namespace Distributr.WPF.Lib.UI.UI_Utillity.Converters
{
    public class CustomResources : INotifyPropertyChanged

    {

       private static Dictionary<string, string> _strings;//string.Empty;
        //private static Strings _strings;

       public Dictionary<string, string> Strings
        {

            get
            {
                if (_strings == null)
                   GetResource();
                return _strings;
            }
            set
            {
                _strings = value;
                OnPropertyChanged("Strings");
            }


        }
        private void GetResource()
        {
          
        }


        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

    }

   
   

}
