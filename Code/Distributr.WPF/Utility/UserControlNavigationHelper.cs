using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Distributr.WPF.UI.Utility
{
    public static class UserControlNavigationHelper
    {
            public static T FindParentByType<T>(this DependencyObject child) where T : DependencyObject
            {
                Type type = typeof(T);
                DependencyObject parent = VisualTreeHelper.GetParent(child);

                if (parent == null)
                {
                    return null;
                }
                else if (parent.GetType() == type)
                {
                    return parent as T;
                }
                else
                {
                    return parent.FindParentByType<T>();
                }
            }
    }
}
