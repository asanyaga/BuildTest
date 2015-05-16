using System;
using System.Configuration;
using Distributr.Core.Domain.Master.SettingsEntities;
using Distributr.Core.Repository.Master.SettingsRepositories;
using StructureMap;

namespace Distributr.WPF.Lib.UI.UI_Utillity.Converters
{
    public  class AmountDecimalConverter : System.Windows.Data.IValueConverter
    {

      

        public object Convert(object value, Type targetType,

            object parameter, System.Globalization.CultureInfo culture)
        {
            IContainer container = ObjectFactory.Container.GetNestedContainer();
            var settingsRepo = container.GetInstance<ISettingsRepository>();
            var settings=settingsRepo.GetByKey(SettingsKeys.NumberOfDecimalPlaces); 

            var decimalValue =settings!= null  ? settings.Value : "4";
            //if (int.Parse(decimalValue) == 0)
            //{
            //    decimalValue=
            //}
            var decimalCharacter = string.Format("N{0}", decimalValue);

            if (value is decimal)
                return ((decimal)value).ToString(decimalCharacter).ToLower();
            else if (value is string)
            {
                decimal amount = 0;
                decimal.TryParse((string)value, out amount);
                return amount.ToString(decimalCharacter).ToLower();
            }
            return null;

            //if (value is decimal)
            //    return ((decimal)value).ToString("N4").ToLower();
            //else if (value is string)
            //{
            //    decimal amount = 0;
            //    decimal.TryParse((string)value, out amount);
            //    return amount.ToString("N4").ToLower();
            //}
            //return null;



        }



        public object ConvertBack(object value, Type targetType,

            object parameter, System.Globalization.CultureInfo culture)
        {

            throw new NotImplementedException();

        }



       

    }
}