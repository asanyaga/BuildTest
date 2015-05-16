using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;

namespace Distributr.WPF.Lib.ViewModels.Transactional.InventoryTransfer
{
    public class TransferedInventoryViewModel : DistributrViewModelBase
    {
        public ObservableCollection<CommodityTransferLineItem> LineItems { get; set; }

        public TransferedInventoryViewModel()
        {
            
        }
    }
}
