using System.Windows;

namespace Distributr.WPF.Lib.UI.UI_Utillity
{
  public static  class WpfHelper
    {
      public static void CenterWindowOnScreen(this Window windows)
      {
          double screenWidth = SystemParameters.PrimaryScreenWidth;
          double screenHeight = SystemParameters.PrimaryScreenHeight;
          double windowWidth = windows.Width;
          double windowHeight = windows.Height;
          windows.Left = (screenWidth / 2) - (windowWidth / 2);
          windows.Top = (screenHeight / 2) - (windowHeight / 2);
      }
    }
}
