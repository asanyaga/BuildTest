using System;
using System.Globalization;
using System.Windows.Data;

namespace Distributr.WPF.Lib.UI.UI_Utillity.Converters
{
    public class ReverseIsEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isEnabled = !((bool)value);
            return isEnabled;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value;
        }
    }
}
