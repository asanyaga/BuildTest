using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace Distributr.WPF.Lib.UI.UI_Utillity.Converters
{
    public class MultivalueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Tuple<bool, Page> tuple = new Tuple<bool, Page>(
                (bool) values[0], (Page) values[1]);
            return (object) tuple;

            //FindCommandParameters parameters = new FindCommandParameters();
            //foreach (var obj in values)
            //{
            //    if (obj is string) parameters.Page = (Page)obj;
            //    else if (obj is bool) parameters.IsFirstLoad = (bool)obj;
            //}
            //return parameters;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class FindCommandParameters
    {
        public bool IsFirstLoad { get; set; }
        public Page Page { get; set; }
    }
}
