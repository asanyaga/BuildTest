using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Integration.Cussons.WPF.Lib.ImportEntities;
using Integration.Cussons.WPF.Lib.MasterDataImportService;

namespace Integration.Cussons.WPF.Lib.CussonsIntegrationViewModels.MasterData
{
    public class ListProductPricingTierImportsViewModel : MasterDataImportListingsBase
    {
        internal IEnumerable<PricingTierImport> ImportItems;
        public ObservableCollection<PricingTierImportVM> ImportVmList { get; set; }
        public ListProductPricingTierImportsViewModel()
        {
        }
    }
    public class PricingTierImportVM:ImportItemVM
    {
        
    }
}
