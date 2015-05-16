using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Repository.Transactional.AuditLogRepositories;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.HQ.Lib.ViewModels.Admin.User;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.UserModelBuilder;
using Distributr.HQ.Lib.ViewModels.Admin.Transactional;
using Distributr.Core.Domain.Transactional.AuditLogEntity;
using Distributr.HQ.Lib.Helper;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Transaction
{
   public class AuditLogViewModelBuilder:IAuditLogViewModelBuilder
    {
       IAuditLogRepository _auditLogRepository;
       ICostCentreRepository _costCentreRepository;
       IUserViewModelBuilder _userViewModelBuilder;
       IUserRepository _userRepository;
       public AuditLogViewModelBuilder(IAuditLogRepository auditLogRepository,ICostCentreRepository costCentreRepository,
       IUserRepository userRepository, IUserViewModelBuilder userViewModelBuilder)
       {
           _auditLogRepository = auditLogRepository;
           _userRepository = userRepository;
           _costCentreRepository = costCentreRepository;
           _userViewModelBuilder = userViewModelBuilder;
       }
        public void AddAuditLog(string userName, string actionName, string moduleName, DateTime timeStamp)
       {
            try
            {
                UserViewModel usr = _userViewModelBuilder.GetByUserName(userName.ToLower());
                _auditLogRepository.addAuditLog(_costCentreRepository.GetById(usr.CostCentre),
                    _userRepository.GetById(usr.Id), moduleName, actionName, timeStamp);
            }catch(Exception ex){}
       }


        public List<AuditLogViewModel> GetAll()
        {
            return _auditLogRepository.GetAll()

                //.Where( d => d.ActionTimeStamp >= StartDate && d.ActionTimeStamp <= EndDate)
                .Select(n => Map(n)).ToList();
        }

       public List<AuditLogViewModel> FilterByUser(Guid? user,DateTime StartDate,DateTime EndDate)
       {
           if (user.HasValue)
           {
              return _auditLogRepository.GetAll()
                   .Where(n => n.ActionUser.Id == user)
                   .Where(d => d.ActionTimeStamp >= StartDate && d.ActionTimeStamp <= EndDate)
                   .Select(n => Map(n)).ToList();
           }
           return _auditLogRepository.GetAll()
               .Where(d => d.ActionTimeStamp >= StartDate && d.ActionTimeStamp <= EndDate)
               .Select(n => Map(n)).ToList();
       }


       public AuditLogViewModel GetById(Guid id)
        {
        
            AuditLog aL = new AuditLog(Guid.Empty);
            if (id != null)
                aL = _auditLogRepository.GetById(id);
            return Map(aL);
           
        }
        AuditLogViewModel Map(AuditLog alog)
        {
            //return new AuditLogViewModel
            //{
            //       id=alog.Id,
            //        actionName=alog.Action,
            //         moduleName=alog.Module,k
            //          timeStamp=alog.ActionTimeStamp,
            //           CostCentreId=alog.ActionOwner==null?0: alog.ActionOwner.Id,
            //            UserId=alog.ActionUser==null?0: alog.ActionUser.Id,
            //             CostCentreName=_costCentreRepository.GetById(alog.ActionOwner.Id).Name,
            //              UserName=_userRepository.GetById(alog.ActionUser.Id).Username
            //};
            AuditLogViewModel aLogViewModel = new AuditLogViewModel();
            {
             aLogViewModel.id=alog.Id;
             aLogViewModel.actionName=alog.Action;
             aLogViewModel.moduleName=alog.Module;
             aLogViewModel.timeStamp=alog.ActionTimeStamp;
             aLogViewModel.CostCentreId = alog.ActionOwner == null ? Guid .Empty: alog.ActionOwner.Id;
             aLogViewModel.UserId = alog.ActionUser == null ? Guid.Empty: alog.ActionUser.Id;
             if (alog.ActionOwner != null)

                 aLogViewModel.CostCentreName = _costCentreRepository.GetById(alog.ActionOwner.Id).Name;

             if (alog.ActionUser != null)

                 aLogViewModel.UserName = _userRepository.GetById(alog.ActionUser.Id).Username;
            
            }
            return aLogViewModel;
        }


        public Dictionary<int, string> GetAction()
        {
            return EnumHelper.EnumToList<AuditLogActions>()
                .ToDictionary(n => (int)n, n => n.ToString());
        }
    }
}
