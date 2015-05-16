using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Domain.Master
{
#if !SILVERLIGHT
   [Serializable]
#endif
    public class ClientMasterDataTracker : IInfrastructureMetadata
    {
        public Guid CostCentreApplicationId { get; set; }
        public int MasterDataId { get; set; }
        public Guid Id { get; set; }

        public ClientMasterDataTracker(Guid id)
        {
            Id = id;    
        }

        public DateTime _DateCreated { get; set; }
        internal void _SetDateCreated(DateTime dateCreated)
        {
            _DateCreated = dateCreated;
        }

        public DateTime _DateLastUpdated { get; set; }
        internal void _SetDateLastUpdated(DateTime dateLastUpdated)
        {
            _DateLastUpdated = dateLastUpdated;
        }

        public EntityStatus _Status{ get; set; }
        internal void _SetStatus(EntityStatus isActive)
        {
            _Status = isActive;
        }

        public DateTime DateTimePushed { get; set; }
        public DateTime? DateTimePushConfirmed { get; set; }


    }
}
