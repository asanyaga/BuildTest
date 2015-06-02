using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributr.Core.Utility
{
    public static class MathHelper
    {
        public static decimal GetTruncatedValue(this decimal value)
        {
            var truncatedValue = Math.Truncate(value * 100) / 100;
            return truncatedValue;
        }
        public static decimal? GetTruncatedValue(this decimal? value)
        {
            if (value.HasValue)
            {
                var truncatedValue = Math.Truncate(value.Value*100)/100;
                return truncatedValue;
            }
            return 0;
        }

        public static decimal GetTotalGross(this decimal amount)
        {

            var decimalValues = amount - decimal.Truncate(amount);
            if (decimalValues >= 0.04m)
            {
                return decimal.Truncate(amount) + 1;
            }
            return decimal.Truncate(amount);
        }

        public static decimal? GetTotalGross(this decimal? amount)
        {
            if (amount.HasValue)
            {
                var decimalValues = amount - decimal.Truncate(amount.Value);
                if (decimalValues >= 0.04m)
                {
                    return decimal.Truncate(amount.Value) + 1;
                }
                return decimal.Truncate(amount.Value);
            }
            return 0;
        }
    }
}
