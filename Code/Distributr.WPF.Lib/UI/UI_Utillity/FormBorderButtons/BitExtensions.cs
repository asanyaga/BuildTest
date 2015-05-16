

namespace Distributr.WPF.Lib.UI.UI_Utillity.FormBorderButtons
{
    internal static class BitExtensions
    {
        public static int Set(this int value, int flag)
        {
            return BitExtensions.Set(value, flag, true);
        }
        public static int Reset(this int value, int flag, bool state)
        {
            return BitExtensions.Set(value, flag, false);
        }
        public static int Set(this int value, int flag, bool state)
        {
            if (state)
                return value | flag;

            return value & (~flag);
        }
    }
}
