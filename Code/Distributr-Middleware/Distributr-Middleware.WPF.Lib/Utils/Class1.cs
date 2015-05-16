using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Distributr_Middleware.WPF.Lib.ViewModels;

namespace Distributr_Middleware.WPF.Lib.Utils
{
    public class SyncBasicResponse
    {
        public bool Status { get; set; }
        public string Info { get; set; }
    }
    public class BoolToImageConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ImageSource result = null;
            var item = (bool) value;
            var intValue = item;
            switch (intValue)
            {
                case true:
                    {
                        result = new BitmapImage(new Uri(@"/Distributr-Middleware.WPF.UI;component/Resources/sync2.jpg", UriKind.Relative));
                        break;
                    }

                case false:
                    {
                        result = new BitmapImage(new Uri(@"/Distributr-Middleware.WPF.UI;component/Resources/cancel.png", UriKind.Relative));
                        break;
                    }
            }
            return result;
        }
        private string GetImageUrl(bool isSuccess)
        {
            string appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (string.IsNullOrEmpty(appPath)) return string.Empty;

            var imageUrl = Path.Combine(appPath, "Resources");
            return isSuccess ? Path.Combine(imageUrl, "sync2.jpg") : Path.Combine(imageUrl, "cancel.png");

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
