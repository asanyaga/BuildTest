using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using Distributr.Core.Resources.Util;
using Distributr.WPF.Lib.UI.UI_Utillity;
using Distributr.WPF.Lib.ViewModels.Transactional.GRN;
using StructureMap;

namespace Distributr.WPF.UI.Views.GRN
{
    public partial class ListGRN : PageBase
    {
        public ListGRN()
        {
            InitializeComponent();
            LocalizeLabels();
        }

       

        private void LocalizeLabels()
        {
            var messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();
            lblPageHeader.Content = messageResolver.GetText("sl.inventory.receive.title");
            coldocref.Header = messageResolver.GetText("sl.inventory.receive.grid.col.docref");
            coldatereceived.Header = messageResolver.GetText("sl.inventory.receive.grid.col.datereceived");
            colorderref.Header = messageResolver.GetText("sl.inventory.receive.grid.col.orderref");
            colloadno.Header = messageResolver.GetText("sl.inventory.receive.grid.col.loadno");
            coluser.Header = messageResolver.GetText("sl.inventory.receive.grid.col.user");
            coltotal.Header = messageResolver.GetText("sl.inventory.receive.grid.col.total");
            lblSearch.Content = messageResolver.GetText("sl.inventory.receive.search");

        }
       
    }
}
