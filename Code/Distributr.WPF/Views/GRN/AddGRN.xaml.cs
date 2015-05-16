using Distributr.Core.Resources.Util;
using Distributr.WPF.Lib.UI.UI_Utillity;
using StructureMap;

namespace Distributr.WPF.UI.Views.GRN
{
    /// <summary>
    /// Interaction logic for AddGRN.xaml
    /// </summary>
    public partial class AddGRN : PageBase
    {
        
        public AddGRN()
        {
            InitializeComponent();
            LocalizeLabels();
           
        }
        
        private void LocalizeLabels()
        {
            var messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();

            labeldocref.Content = messageResolver.GetText("sl.inventory.receive.edit.docref");
            labelorderref.Content = messageResolver.GetText("sl.inventory.receive.edit.orderref");
            labelloadno.Content = messageResolver.GetText("sl.inventory.receive.edit.loadno");
            labeldatereceived.Content = messageResolver.GetText("sl.inventory.receive.edit.datelastreceived");
            labeluser.Content = messageResolver.GetText("sl.inventory.receive.edit.user");
            labeltotalcost.Content = messageResolver.GetText("sl.inventory.receive.edit.totalcost");
            colproduct.Header = messageResolver.GetText("sl.inventory.receive.edit.grid.col.product");
            colrecievedqty.Header = messageResolver.GetText("sl.inventory.receive.edit.grid.col.receivedqty");
            colexpectedqty.Header = messageResolver.GetText("sl.inventory.receive.edit.grid.col.expectedqty");
            colunitcost.Header = messageResolver.GetText("sl.inventory.receive.edit.grid.col.unitcost");
            coltotalcost.Header = messageResolver.GetText("sl.inventory.receive.edit.grid.col.totalcost");
            colbreakbulk.Header = messageResolver.GetText("sl.inventory.receive.edit.grid.col.breakbulk");

        }
    }
}
