using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.ClientApp;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.OrderDocumentEntities;

namespace Distributr.Core.Workflow
{
  public  interface IOrderWorkflow
  {
      void Submit(MainOrder order,BasicConfig config);
  }
  public interface IExternalOrderWorkflow
  {
      void Submit(MainOrder order);
  }
  public interface IOrderPosWorkflow
  {
      void Submit(MainOrder order,BasicConfig config);
  }

  public interface IPurchaseOrderWorkflow
  {
      void Submit(MainOrder order);
  }
  public interface IStockistPurchaseOrderWorkflow
  {
      void Submit(MainOrder order, BasicConfig config);
  }
  public interface IWsInventoryAdjustmentWorflow
  {
      void Submit(InventoryAdjustmentNote note);
      void Submit(InventoryTransferNote note);
  }
}
