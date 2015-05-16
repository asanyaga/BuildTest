using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using Distributr.Core.Resources.Util;
using StructureMap;

namespace Distributr.WPF.UI.Converter
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
            IMessageSourceAccessor messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();
            _strings =messageResolver.GetResource();
           // _strings = new Strings();
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

   
    public class Strings : DynamicObject
    {
        Dictionary<string, string> values;

        public Strings()
        {
            IMessageSourceAccessor messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();
            this.values = messageResolver.GetResource();
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            string value;
            bool success = values.TryGetValue(binder.Name, out value);
            result = value;
            return success;
        }
    }

}
