using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Distributr.Azure.Lib.CommandProcessing
{
    //PK CC - RK CommandId
    public class CommandProcessingAuditTable : TableEntity
    {
        public Guid Id { get; set; }
        public int CostCentreCommandSequence { get; set; }
        public Guid CostCentreApplicationId { get; set; }
        public Guid DocumentId { get; set; }
        public Guid ParentDocumentId { get; set; }
        public string JsonCommand { get; set; }
        public string CommandType { get; set; }
        public int Status { get; set; }
        public int RetryCounter { get; set; }
        public DateTime DateInserted { get; set; }
        public string SendDateTime { get; set; }
    }

    //PK "commandlu" - RK CommandId
    public class CommandProcessingAuditCommandIndexTable : TableEntity
    {
        public Guid CostCentreId { get; set; }
    }

    //PK DocumentId - RK CommandId
    public class CommandProcessingAuditDocumentIndexTable : TableEntity
    {
        public Guid CostCentreId { get; set; }
    }

    //PK CCAppId - CommandId
    public class CommandProcessingAuditCCApplicationIndexTable : TableEntity
    {
        public Guid CostCentreId { get; set; }
    }

    //PK CommandProcessingStatus - RK Commandid
    public class CommandProcessingAuditStatusIndexTable : TableEntity
    {
        public Guid CostCentreId { get; set; }
    }



}
