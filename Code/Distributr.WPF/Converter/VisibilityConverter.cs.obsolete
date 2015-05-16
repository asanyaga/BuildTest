using System;
using System.Windows;
using System.Windows.Data;

namespace Distributr.WPF.UI.Converter
{
    public class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool visibility = (bool) value;
            return visibility ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility vis = (Visibility) value;
            return vis == Visibility.Visible;
        }
    }
}
