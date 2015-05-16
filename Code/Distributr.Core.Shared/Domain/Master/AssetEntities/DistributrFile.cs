using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Domain.Master.AssetEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
  public   class DistributrFile:MasterEntity
    {
      public DistributrFile(Guid id) : base(id)
      {
      }

      public DistributrFile(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
          : base(id, dateCreated, dateLastUpdated, isActive)
      {
      }
      public string FileData { get; set; }
      public DistributrFileType FileType { get; set; }
      public string FileExtension { get; set; }
      public string Description { get; set; }
      
    }
    public   enum DistributrFileType: int
    {
        Image=1,
        Document=2,
    }
}
