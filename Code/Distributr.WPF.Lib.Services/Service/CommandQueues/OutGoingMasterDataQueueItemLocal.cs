using System;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.MasterDataDTO;

namespace Distributr.WPF.Lib.Services.Service.CommandQueues
{
    public class 
        
        OutGoingMasterDataQueueItemLocal 
    {
        public MasterDataDTOSaveCollective Type { get; set; }
        [MaxLength(2000)]
        public string JsonDTO { get; set; }
        public bool IsSent { get; set; }
        public DateTime DateSent { get; set; }
        public Guid MasterId { get; set; }
        public int Id { get; set; }
    }
}
