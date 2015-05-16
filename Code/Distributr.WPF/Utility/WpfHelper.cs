using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Distributr.WPF.UI.Utility
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
