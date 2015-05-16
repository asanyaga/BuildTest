using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.WPF.Lib.ViewModels.Transactional.Orders;
using GalaSoft.MvvmLight.Command;

namespace Integration.QuickBooks.Lib.QBIntegrationViewModels
{
    public class QBListOrdersViewModel : QBSalesOrderViewModelBase
    {
        public QBListOrdersViewModel()
        {
            OrderTypeToLoad = OrderType.OutletToDistributor;
            PageTitle = "Listing Closed Orders Ready for Exporting to Quick Books";
        }

        private RelayCommand<bool> _loadExportedOrdersPageCommand = null;
        public RelayCommand<bool> LoadExportedOrdersListingPageCommand
        {
            get
            {
                PageTitle = "Listing Exported Orders";
                return _loadExportedOrdersPageCommand ??
                       (_loadExportedOrdersPageCommand = new RelayCommand<bool>(LoadExportedSalesOrders));
            }
        }
    }
}
