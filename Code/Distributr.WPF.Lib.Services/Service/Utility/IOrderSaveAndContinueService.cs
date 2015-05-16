using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core;
using Distributr.Core.Domain.Transactional.DocumentEntities;


namespace Distributr.WPF.Lib.Impl.Services.Transactional.SaveAndContinue
{
   public interface IOrderSaveAndContinueService
   {

       void Save(OrderSaveAndContinueLater order);
       OrderSaveAndContinueLater GetById(Guid id);

       List<OrderSaveAndContinueLater> Query(DateTime startdate,DateTime endDate, OrderType orderType);
       void MarkAsConfirmed(Guid orderId);
   }
}
