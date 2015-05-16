using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.InventoryEntities;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.AssetEntities;
using Distributr.Core.Domain.Master.CoolerEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.DistributorTargetEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Master.SettingsEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.MasterDataDTO;
using Distributr.Core.MasterDataDTO.DTOModels.InventoriesDTO;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO;
using Distributr.Core.Repository.InventoryRepository;
using Distributr.Core.Repository.Master.AssetRepositories;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.DistributorTargetRepositories;
using Distributr.Core.Repository.Master.SettingsRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.WSAPI.Lib.Services.Resolver;
using log4net;

namespace Distributr.WSAPI.Lib.Services.Bus.Impl
{
    public class HandleMasterData : IHandleMasterData
    {
        private IMasterDataEntityResolver _masterDataEntityResolver;
        private ICostCentreRepository _costCentreRepository;
        private IContactRepository _contactRepository;
        ILog _logger = LogManager.GetLogger("HandleMasterData");
        private IAssetRepository _assetRepository;
        private IDistributrFileRepository _distributrFileRepository;
        private IOutletVisitDayRepository _outletVisitDayRepository;
        private IOutletPriorityRepository _outletPriorityRepository;
        private ITargetRepository _targetRepository;
        private IUserRepository _userRepository;
        private ISettingsRepository _settingsRepository;
        private IInventorySerialsRepository _inventorySerialsRepository;

        public HandleMasterData(IMasterDataEntityResolver masterDataEntityResolver, ICostCentreRepository costCentreRepository, IContactRepository contactRepository, IAssetRepository assetRepository, IDistributrFileRepository distributrFileRepository, IOutletVisitDayRepository outletVisitDayRepository, IOutletPriorityRepository outletPriorityRepository, ITargetRepository targetRepository, IUserRepository userRepository, ISettingsRepository settingsRepository, IInventorySerialsRepository inventorySerialsRepository)
        {
            _masterDataEntityResolver = masterDataEntityResolver;
            _costCentreRepository = costCentreRepository;
            _contactRepository = contactRepository;
            _assetRepository = assetRepository;
            _distributrFileRepository = distributrFileRepository;
            _outletVisitDayRepository = outletVisitDayRepository;
            _outletPriorityRepository = outletPriorityRepository;
            _targetRepository = targetRepository;
            _userRepository = userRepository;
            _settingsRepository = settingsRepository;
            _inventorySerialsRepository = inventorySerialsRepository;
        }

        public HandleMasterData(IMasterDataEntityResolver masterDataEntityResolver)
        {
            _masterDataEntityResolver = masterDataEntityResolver;
        }

        public void HandleMasterDataDTO(MasterBaseDTO masterBaseDTO, MasterDataDTOSaveCollective collective)
        {
            try
            {
                if (collective == MasterDataDTOSaveCollective.InventorySerials)
                {
                    SaveInventorySerials(masterBaseDTO);
                }
                else
                {
                    MasterEntity entity = _masterDataEntityResolver.Resolver(masterBaseDTO, collective);
                    SaveMasterData(entity, collective);
                }
            }
            catch (Exception ex)
            {
                _logger.Info("Error", ex);
                throw new Exception(ex.Message);
            }
        }

        private void SaveMasterData(MasterEntity entity, MasterDataDTOSaveCollective collective)
        {
            switch (collective)
            {
                case MasterDataDTOSaveCollective.Outlet:
                    if (entity is Outlet)
                        _costCentreRepository.Save(entity as Outlet);
                    break;
                case MasterDataDTOSaveCollective.Contact:
                    if (entity is Contact)
                        _contactRepository.Save(entity as Contact);
                    break;
                case MasterDataDTOSaveCollective.Asset:
                    if (entity is Asset)
                        _assetRepository.Save(entity as Asset);
                    break;
                case MasterDataDTOSaveCollective.DistributrFile:
                    if (entity is DistributrFile)
                        _distributrFileRepository.Save(entity as DistributrFile);
                    break;
                case MasterDataDTOSaveCollective.OutletVisitDay:
                    if (entity is OutletVisitDay)
                        _outletVisitDayRepository.Save(entity as OutletVisitDay);
                    break;
                case MasterDataDTOSaveCollective.OutletPriority:
                    if (entity is OutletPriority)
                        _outletPriorityRepository.Save(entity as OutletPriority);
                    break;
                case MasterDataDTOSaveCollective.Target:
                    if (entity is Target)
                        _targetRepository.Save(entity as Target);
                    break;
                case MasterDataDTOSaveCollective.User:
                    if (entity is User)
                        _userRepository.Save(entity as User);
                    break;
                case MasterDataDTOSaveCollective.DistributrSalesman:
                    if (entity is DistributorSalesman)
                        _costCentreRepository.Save(entity as DistributorSalesman);
                    break;
                case MasterDataDTOSaveCollective.PasswordChange:
                    if (entity != null && entity is User)
                        _userRepository.Save(entity as User);
                    break;
                case MasterDataDTOSaveCollective.AppSettings:
                    if (entity != null && entity is AppSettings)
                        _settingsRepository.Save(entity as AppSettings);
                    break;
                case MasterDataDTOSaveCollective.InventorySerials:
                    if (entity != null && entity is InventorySerials)
                        _inventorySerialsRepository.AddInventorySerial(entity as InventorySerials);
                    break;

                default:
                    break;
            }

        }

        private void SaveInventorySerials(MasterBaseDTO masterbaseDTO)
        {
            InventorySerialsDTO inventorySerialsDTO = masterbaseDTO as InventorySerialsDTO;
            List<string> fromToList = inventorySerialsDTO.CSVFromToSerials.Split(',').ToList();

            foreach (var item in fromToList.Where(n => n.Trim() != ""))
            {
                string[] fromTo = item.Split('-');
                InventorySerials invSerials = new InventorySerials(Guid.NewGuid())
                {
                    CostCentreRef = new CostCentreRef { Id = inventorySerialsDTO.CostCentreMasterId },
                    ProductRef = new ProductRef { ProductId = inventorySerialsDTO.ProductMasterId },
                    DocumentId = inventorySerialsDTO.DocumentId,
                    From = fromTo[0].Trim(),
                    To = fromTo[1].Trim()
                };

                _inventorySerialsRepository.AddInventorySerial(invSerials);
            }
        }
    }
}
