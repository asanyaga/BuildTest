using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.BankEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.DistributorTargetEntities;
using Distributr.Core.Repository.Master.BankRepositories;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Factory.Master;
using Distributr.Core.Repository.Master.DistributorTargetRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Validation;
using Distributr.WSAPI.Lib.Services.WCFServices.DataContracts;
using System.Collections.Generic;
using log4net;
using StructureMap;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Repository.Master.UserRepositories;
using TargetItem = Distributr.WSAPI.Lib.Services.WCFServices.DataContracts.CostCentreTargetItem;

namespace Distributr.WSAPI
{
   
}
