using System;
using Distributr.Core.Domain.Transactional.AuditLogEntity;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.AuditLogRepositories;

namespace Distributr.Core.Workflow.Impl.AuditLogs
{
    public class AuditLogWFManager : IAuditLogWFManager
    {
        private readonly IAuditLogRepository _auditLogService;
        
        private readonly IUserRepository _userService;
        private ICostCentreRepository _costCentreRepository;

        public AuditLogWFManager(IAuditLogRepository auditLogService, IUserRepository userService, ICostCentreRepository costCentreRepository)
        {
            _auditLogService = auditLogService;
            
            _userService = userService;
            _costCentreRepository = costCentreRepository;
        }

        

        public void AuditLogEntry(string module, string action)
        {
            //TODO AJM make this work
            //return;
            //var auditlog = new AuditLog(Guid.NewGuid())
            //{
            //    //ActionOwner =  _costCentreRepository.GetById(config.CostCentreId),
            //    //TODO modify config to include current user id
            //    //ActionUser = _userService.GetById(_configService.ViewModelParameters.CurrentUserId),
            //    Module = module,
            //    Action = action,
            //    ActionTimeStamp = DateTime.Now
            //};
            //_auditLogService.Save(auditlog);
        }
    }
}
