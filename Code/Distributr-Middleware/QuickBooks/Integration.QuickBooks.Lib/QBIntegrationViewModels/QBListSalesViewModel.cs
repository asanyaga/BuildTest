using System;
using System.Collections.ObjectModel;
using Distributr.Core.Domain.Master.Util;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.OrderDocumentEntities;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.WPF.Lib.ViewModels.Admin;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using Integration.QuickBooks.Lib.QBIntegrationCore;

namespace Integration.QuickBooks.Lib.QBIntegrationViewModels
{
    public class QBListSalesViewModel : QBSalesOrderViewModelBase
    {
        public QBListSalesViewModel()
        {
            OrderTypeToLoad = OrderType.DistributorPOS;
            PageTitle = "Listing Closed Sales Ready for Exporting to Quick Books";
        }

        private RelayCommand<bool> _loadExportedSalesPageCommand = null;

        public RelayCommand<bool> LoadExportedSalesListingPageCommand
        {
            get
            {
                PageTitle = "Listing Exported Sales";
                return _loadExportedSalesPageCommand ?? (_loadExportedSalesPageCommand = new RelayCommand<bool>(LoadExportedSalesOrders));
            }
        }
    }
}
