using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Distributr.WPF.UI.Helpers
{
   public static class WPFExtensions
    {
       public static DataGridColumn GetByName(this ObservableCollection<DataGridColumn> col, string name)
       {
           return col.SingleOrDefault(p =>
                                      (string)p.GetValue(FrameworkElement.NameProperty) == name
               );
       }
    }
}
