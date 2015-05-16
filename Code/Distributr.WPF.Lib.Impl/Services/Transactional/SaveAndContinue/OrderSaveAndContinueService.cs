using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.WPF.Lib.Impl.Model.Transactional;
using Distributr.WPF.Lib.Impl.Repository.Utility;
using Newtonsoft.Json;

namespace Distributr.WPF.Lib.Impl.Services.Transactional.SaveAndContinue
{
   public class OrderSaveAndContinueService : IOrderSaveAndContinueService
    {
       //DistributrLocalContext
       private IAppTempTransactionRepository _appTempTransactionRepository;

       public OrderSaveAndContinueService(IAppTempTransactionRepository appTempTransactionRepository)
       {
           _appTempTransactionRepository = appTempTransactionRepository;
       }

       public void Save(OrderSaveAndContinueLater order)
       {
           var temp = new AppTempTransaction
                          {
                              Id = order.Id,
                              Json = JsonConvert.SerializeObject(order),
                              TransactionStatus = false,
                              TransactionType = order.OrderType.ToString()
                          };
           _appTempTransactionRepository.Save(temp);
       }

       public OrderSaveAndContinueLater GetById(Guid id)
       {
           var order = new OrderSaveAndContinueLater();
           var item = _appTempTransactionRepository.GetById(id);
           if(item!=null)
           {
               order = JsonConvert.DeserializeObject<OrderSaveAndContinueLater>(item.Json);
           }
           return order;
       }

       public List<OrderSaveAndContinueLater> Query(DateTime startdate, DateTime endDate, OrderType orderType)
       {
           return _appTempTransactionRepository.Query(startdate, endDate, orderType).Select(s => GetById(s.Id)).ToList();
       }

       public void MarkAsConfirmed(Guid orderId)
       {
          _appTempTransactionRepository.MarkAsConfirmed(orderId);
       }
    }
}
