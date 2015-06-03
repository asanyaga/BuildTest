using System;
using System.Collections.Generic;
using Distributr.Core.Commands.CommandPackage;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.MasterDataDTO.DataContracts;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO;
using Distributr.Core.Utility;

namespace Distributr.Core.ClientApp.CommandResults
{
    public abstract class ResponseBase
    {
        public string ErrorInfo { get; set; }
    }


    public class BasicResponse
    {
        public string Info { get; set; }
        public bool Status { get; set; }
    }
    public class ResponseBool : ResponseBase
    {
        public bool Success { get; set; }
    }

    public class ResponseBasic : ResponseBase
    {
        public string Result { get; set; }
        public string ResultInfo { get; set; }
        public bool Status { get; set; }
    }

    public class ResponseSyncRequired : ResponseBase
    {
        public bool RequiresToSync { get; set; }
    }

    public class ResponseMasterDataEntities : ResponseBase
    {
        string[] MasterDataTables { get; set; }
    }

    public class ResponseCostCentreSyncTables : ResponseBase
    {
        public string[] TablesToSync { get; set; }

    }
    
    public class ResponseMasterDataInfo : ResponseBase
    {
        public  ResponseMasterDataInfo(){
            DeletedItems= new List<Guid>();
        }
        public List<Guid> DeletedItems { get; set; }
        public MasterDataInfo MasterData { get; set; }
    }
    public class ResponseFarmerCummulativeDataInfo : ResponseBase
    {
        public List<FarmerCummulative> FarmersCummulative { get; set; }
    }

    public class MasterDataInfo
    {
        public DateTime LastSyncTimeStamp { get; set; }
        public string EntityName { get; set; }
        public MasterBaseDTO[] MasterDataItems { get; set; }
    }

    public class ResponseCostCentreTest : ResponseBase
    {
        public Guid costCentreId { get; set; }
        public string costCentreType { get; set; }
    }

    public class CreateCostCentreApplicationResponse : ResponseBase
    {
        public Guid CostCentreApplicationId { get; set; }
    }
    public class CostCentreLoginResponse : ResponseBase
    {
        public Guid CostCentreId { get; set; }
    }
    [Obsolete("Command Envelope Refactoring")]
    public class DocumentCommandRoutingResponse : ResponseBase
    {
        public string CommandType { get; set; }
        public DocumentCommand Command { get; set; }
        public long CommandRouteItemId { get; set; }
    }
    public class CommandEnvelopeWrapper
    {
        public string DocumentType { get; set; }
        public CommandEnvelope Envelope { get; set; }
        public long EnvelopeArrivedAtServerTick { get; set; }
    }
    public class EnvelopeRoutingRequest 
    {
        public EnvelopeRoutingRequest()
        {
            DeliveredEnvelopeIds = new List<Guid>();
        }
        public List<Guid> DeliveredEnvelopeIds { get; set; }
        public Guid CostCentreApplicationId { get; set; }
        public int BatchSize { get; set; }

    }
    public class BatchDocumentCommandEnvelopeRoutingResponse : ResponseBase
    {
        public BatchDocumentCommandEnvelopeRoutingResponse()
        {
            Envelopes = new List<CommandEnvelopeWrapper>();
        }

        public List<CommandEnvelopeWrapper> Envelopes { get; set; }
        //public int CommandRoutingCount { get; set; }
        //public long LastCommandRouteItemId { get; set; }

    }
    [Obsolete("Command Envelope Refactoring")]
    public class BatchDocumentCommandRoutingResponse : ResponseBase
    {
        public BatchDocumentCommandRoutingResponse()
        {
            RoutingCommands = new List<DocumentCommandRoutingResponse>();
        }

        public List<DocumentCommandRoutingResponse> RoutingCommands { get; set; }
        public int CommandRoutingCount { get; set; }
        public long LastCommandRouteItemId { get; set; }
       
    }

    public class SyncResponseMasterDataInfo<T> : ResponseBase where T : class
    {
       public SyncResponseMasterDataInfo()
        {
            DeletedItems= new List<Guid>();
        }
        public SyncMasterDataInfo<T> MasterData { get; set; }
        public List<Guid> DeletedItems { get; set; }
    }
   

    public class SyncMasterDataInfo<T> where T : class
    {
        public string EntityName { get; set; }
        public DateTime LastSyncTimeStamp { get; set; }
        public T[] MasterDataItems { get; set; }
    }
}
