using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using Distributr.Core.Resources.Util;
using Distributr.WPF.Lib.ViewModels.Transactional.RN;
using GalaSoft.MvvmLight.Messaging;
using StructureMap;

namespace Distributr.WPF.UI.Views.RN
{
    public partial class RetunsList : Page
    {
       
        public RetunsList()
        {
           
            InitializeComponent();
            LocalizeLabels();
        }

        private void LocalizeLabels()
        {
            var messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();
            labeltitle.Content = messageResolver.GetText("sl.returns.title");
            IncompleteTabItem.Header = messageResolver.GetText("sl.returns.tab.incomplete");
            ClosedTabItem.Header = messageResolver.GetText("sl.returns.tab.closed");
            colicompletereturndate.Header = messageResolver.GetText("sl.returns.incomplete.grid.col.returndate");
            colicompletesaleman.Header = messageResolver.GetText("sl.returns.incomplete.grid.col.saleman");
            colclosedreturndate.Header = messageResolver.GetText("sl.returns.closed.grid.col.returndate");
            colclosedsaleman.Header = messageResolver.GetText("sl.returns.closed.grid.col.saleman");
       
        }

        
        
      
    }
}
