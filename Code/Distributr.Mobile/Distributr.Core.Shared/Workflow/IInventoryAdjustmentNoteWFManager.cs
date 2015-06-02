using Distributr.Core.Domain.Transactional.DocumentEntities;

namespace Distributr.Core.Workflow
{
    public interface IInventoryAdjustmentNoteWfManager : IWFManager<InventoryAdjustmentNote>
    {
        //void CreateAndConfirmIAN(Document document);
    }
}
