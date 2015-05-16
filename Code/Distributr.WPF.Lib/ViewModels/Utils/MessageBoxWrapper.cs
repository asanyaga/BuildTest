using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Distributr.WPF.Lib.ViewModels.Utils
{
public    class MessageBoxWrapper : IMessageBoxWrapper
    {

    public MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton messageBoxButton)
    {
        return MessageBox.Show(messageBoxText, caption, messageBoxButton);
    }
    }
}
