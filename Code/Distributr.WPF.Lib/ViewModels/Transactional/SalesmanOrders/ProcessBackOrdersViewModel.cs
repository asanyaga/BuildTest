using System;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Transactional.SalesmanOrders
{

    public class ProcessBackOrdersViewModel : DistributrViewModelBase
    {
        Order _order;

        public ProcessBackOrdersViewModel()
        {
           
            _order = new Order(Guid.NewGuid());
        }

        #region Properties
        public RelayCommand ProcessBackOrdersCommand { get; set; }

        #endregion

        #region Methods
        void Load()
        {
            
        }
        #endregion
    }
}