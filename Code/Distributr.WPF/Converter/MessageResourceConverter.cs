using System;
using System.Globalization;
using System.Windows.Data;
using Distributr.Core.Resources.Util;
using StructureMap;

namespace Distributr.WPF.UI.Converter
{
    public class MessageResourceConverter : IValueConverter

    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IMessageSourceAccessor messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();
            string key =(string) parameter;
            string data = messageResolver.GetText(key);
            return data;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        
    }
}
