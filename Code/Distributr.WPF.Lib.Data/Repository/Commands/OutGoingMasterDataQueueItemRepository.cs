using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.MasterDataDTO;
using Distributr.Core.MasterDataDTO.DTOModels.InventoriesDTO;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.CostCentre;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.DistributorTargets;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Settings;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.User;
using Distributr.WPF.Lib.Data.EF;
using Distributr.WPF.Lib.Services.Service.CommandQueues;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


namespace Distributr.WPF.Lib.Data.Repository.Commands
{
    public class OutGoingMasterDataQueueItemRepository : IOutGoingMasterDataQueueItemRepository
    {
        private DistributrLocalContext _ctx;

        public OutGoingMasterDataQueueItemRepository(DistributrLocalContext ctx)
        {
            _ctx = ctx;
        }
        public List<OutGoingMasterDataQueueItemLocal> GetUnSentMasterDataDTO()
        {
           return _ctx.OutGoingMasterDataQueueItemLocals.Where(n => !n.IsSent).ToList();
           
        }

        public void MarkMasterDataDTOAsSent(int OID)
        {
            OutGoingMasterDataQueueItemLocal c = GetByIOD(OID);
                if (c == null) return;
                c.DateSent = DateTime.Now;
                c.IsSent = true;
                
                Add(c);
           
           
        }
        public void DeleteOldCommand(int OID)
        {
           
                OutGoingMasterDataQueueItemLocal c = GetByIOD(OID);
                if (c == null) return;
                _ctx.OutGoingMasterDataQueueItemLocals.Remove(c);
                _ctx.SaveChanges();
           
        }

        public OutGoingMasterDataQueueItemLocal UpdateSerializedObjectWithOID(int commandOID)
        {
            OutGoingMasterDataQueueItemLocal dto = GetByIOD(commandOID);
            MasterBaseDTO saveItem = null;
            switch (dto.Type)
            {
                  
                case MasterDataDTOSaveCollective.OutletVisitDay:
                    saveItem = JsonConvert.DeserializeObject<OutletVisitDayDTO>(dto.JsonDTO,new IsoDateTimeConverter());
                    break;
                case MasterDataDTOSaveCollective.OutletPriority:
                    saveItem = JsonConvert.DeserializeObject<OutletPriorityDTO>(dto.JsonDTO, new IsoDateTimeConverter());
                    break;
                case MasterDataDTOSaveCollective.Target:
                    saveItem = JsonConvert.DeserializeObject<TargetDTO>(dto.JsonDTO, new IsoDateTimeConverter());
                    break;
                case MasterDataDTOSaveCollective.User:
                    saveItem = JsonConvert.DeserializeObject<UserDTO>(dto.JsonDTO, new IsoDateTimeConverter());
                    break;

                case MasterDataDTOSaveCollective.Contact:
                    saveItem = JsonConvert.DeserializeObject<ContactDTO>(dto.JsonDTO, new IsoDateTimeConverter());
                    break;
                ////case MasterDataDTOSaveCollective.Outlet:
                ////    saveItem = JsonConvert.DeserializeObject<OutletDTO>(dto.JsonDTO, new IsoDateTimeConverter());
                ////    break;
                ////case MasterDataDTOSaveCollective.Route:
                ////    saveItem = JsonConvert.DeserializeObject<RouteDTO>(dto.JsonDTO, new IsoDateTimeConverter());
                ////    break;
                case MasterDataDTOSaveCollective.DistributrSalesman:
                    saveItem = JsonConvert.DeserializeObject<DistributorSalesmanDTO>(dto.JsonDTO, new IsoDateTimeConverter());
                    break;
                case MasterDataDTOSaveCollective.AppSettings:
                    saveItem = JsonConvert.DeserializeObject<AppSettingsDTO>(dto.JsonDTO, new IsoDateTimeConverter());
                    break;
                case MasterDataDTOSaveCollective.InventorySerials:
                    saveItem = JsonConvert.DeserializeObject<InventorySerialsDTO>(dto.JsonDTO, new IsoDateTimeConverter());
                    break;
                default:
                    throw new Exception("Failed to update cost centre command sequence id");
            }
            dto.JsonDTO = JsonConvert.SerializeObject(saveItem, new IsoDateTimeConverter());
          
                Add(dto);
          
            return dto;
        }

        public OutGoingMasterDataQueueItemLocal GetFirstUnSentMasterDataDTO()
        {
            return _ctx.OutGoingMasterDataQueueItemLocals.OrderByDescending(o => o.DateSent).ToList().FirstOrDefault(n => n.IsSent == false);
        }

        public OutGoingMasterDataQueueItemLocal GetByIOD(int OID)
        {
            return _ctx.OutGoingMasterDataQueueItemLocals.FirstOrDefault(p => p.Id == OID);
        }

        public void Add(OutGoingMasterDataQueueItemLocal itemToAdd)
        {
                 var existing = GetByIOD(itemToAdd.Id);
            if (existing == null)
            {
                existing = new OutGoingMasterDataQueueItemLocal();
                _ctx.OutGoingMasterDataQueueItemLocals.Add(existing);
            }
            existing.Id = itemToAdd.Id;
            existing.Type = itemToAdd.Type;
            existing.JsonDTO = itemToAdd.JsonDTO;
            existing.MasterId = itemToAdd.MasterId;
            existing.IsSent = itemToAdd.IsSent;
            existing.DateSent = DateTime.Now;

           _ctx.SaveChanges();
           
        }

        public OutGoingMasterDataQueueItemLocal GetByDTOIDId(int dtoid)
        {
            return _ctx.OutGoingMasterDataQueueItemLocals.ToList().FirstOrDefault(n => n.Id == dtoid);
        }

        public List<OutGoingMasterDataQueueItemLocal> GetAll()
        {
            return _ctx.OutGoingMasterDataQueueItemLocals.ToList();
        }

        public bool IsAnyUnSent()
        {
           return _ctx.OutGoingMasterDataQueueItemLocals.Any(p => !p.IsSent);
        }



        public OutGoingMasterDataQueueItemLocal GetByDTOIDId(Guid dtoid)
        {
            return _ctx.OutGoingMasterDataQueueItemLocals.FirstOrDefault(p => p.MasterId == dtoid);
        }
    }
}
