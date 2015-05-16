using System;
using System.Windows;
using System.Windows.Data;

namespace Distributr.WPF.Lib.UI.UI_Utillity.Converters
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
    public class StringFormats
    {
        public static string AmountFormat = "Date: {0:dddd}";

        public static string Time = "Time: {0:HH:mm}";
        public static string QuantityFormat = "N0";
         public static string QuantityRegEx = @"^(?=.*[1-9])(\d{0,10})$";
       
    }  
}