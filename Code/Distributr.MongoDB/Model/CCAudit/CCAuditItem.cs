﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;

namespace Distributr.MongoDB.Model.CCAudit
{
    public class CCAuditItem
    {
        public Guid Id { get; set; }
        public Guid CostCentreId { get; set; }
        public string Action { get; set; }
        public string Info { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime DateInsert { get; set; }
        public string Result { get; set; }
    }
}
