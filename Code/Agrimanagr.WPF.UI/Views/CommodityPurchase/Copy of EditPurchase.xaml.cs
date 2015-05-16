using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Agrimanagr.WPF.UI.Views.Admin.Equipment.EquipmentSetup;
using Agrimanagr.WPF.UI.Views.ReceiptView;
using Agrimanagr.WPF.UI.Views.UtilityViews;
using Distributr.Core.Domain.Master.CommodityEntities;
using Distributr.Core.Domain.Master.CommodityEntity;
using Distributr.Core.Domain.Master.CoolerEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.EquipmentEntities;
using Distributr.WPF.Lib.UI.UI_Utillity;
using Distributr.WPF.Lib.ViewModels.Transactional.CommodityPurchase;
using Distributr.WPF.Lib.ViewModels.Utils;
using ComboPopUp = Agrimanagr.WPF.UI.Views.UtilityViews.ComboPopUp;


namespace Agrimanagr.WPF.UI.Views.CommodityPurchase
{
    public partial class EditPurchase : UserControl
    {
       public EditPurchase()
        {
            InitializeComponent();
          
        }
        
    }
}
