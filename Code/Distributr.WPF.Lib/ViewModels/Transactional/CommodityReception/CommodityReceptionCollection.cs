using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributr.WPF.Lib.ViewModels.Transactional.CommodityReception
{
    public class CommodityReceptionCollection : ObservableCollection<CommodityPeiChartItem>
    {
        public CommodityReceptionCollection(ListCommodityReceptionViewModel vm)
        {
            //int awaitingReception = vm.AwaitingReceptionCount();
            int incompleteReceptions = vm.AwaitingStorageCount();
            int completeReceptions = vm.CompleteReceptionsCount();

            //Add(new CommodityPeiChartItem { Tab = (int)TabIndex.AwaitingReception, CommodityCount = awaitingReception });
            Add(new CommodityPeiChartItem { Tab = (int)TabIndex.AwaitingStorage, CommodityCount = incompleteReceptions });
            Add(new CommodityPeiChartItem { Tab = (int)TabIndex.CompleteReception, CommodityCount = completeReceptions });
        }

        enum TabIndex
        {
            AwaitingReception = 0,
            AwaitingStorage = 1,
            CompleteReception = 2
        }
    }

    public class CommodityPeiChartItem
    {
        public int Tab { get; set; }
        public int CommodityCount { get; set; }
    }
}
