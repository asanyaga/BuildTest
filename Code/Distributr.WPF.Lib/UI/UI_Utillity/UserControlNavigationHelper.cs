using System;
using System.Windows;
using System.Windows.Media;

namespace Distributr.WPF.Lib.UI.UI_Utillity
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
