using System;
using System.Collections.Generic;
using Distributr.Core;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.WPF.Lib.Impl.Model.Transactional;

namespace Distributr.WPF.Lib.Impl.Repository.Utility
{
    public interface IAppTempTransactionRepository
    {
        void Save(AppTempTransaction appTempTransaction);
        AppTempTransaction GetById(Guid id);
        List<AppTempTransaction> Query(DateTime startdate, DateTime endDate, OrderType orderType);
        void MarkAsConfirmed(Guid orderId);

    }
}