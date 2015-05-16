using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Distributr.WPF.Lib.UI.UI_Utillity
{
    public static class FormChildControls
    {
        private static List<object> lstChildren;

        public static List<object> GetChildren(Visual p_vParent, int p_nLevel)
        {
            if (p_vParent == null)
            {
                throw new ArgumentNullException("Element {0} is null!", p_vParent.ToString());
            }

            lstChildren = new List<object>();

            GetChildControls(p_vParent, p_nLevel);

            return lstChildren;

        }

        private static void GetChildControls(Visual p_vParent, int p_nLevel)
        {
            int nChildCount = VisualTreeHelper.GetChildrenCount(p_vParent);

            for (int i = 0; i <= nChildCount - 1; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(p_vParent, i);

                lstChildren.Add((object)v);

                if (VisualTreeHelper.GetChildrenCount(v) > 0)
                {
                    GetChildControls(v, p_nLevel + 1);
                }
            }
        }


        public static T FindVisualChild<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        return (T)child;
                    }

                    T childItem = FindVisualChild<T>(child);
                    if (childItem != null) return childItem;
                }
            }
            return null;
        }
    }
}
