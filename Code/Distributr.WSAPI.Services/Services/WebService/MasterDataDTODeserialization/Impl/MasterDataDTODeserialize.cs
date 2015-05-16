using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.MasterDataDTO;
using Distributr.Core.MasterDataDTO.DTOModels.InventoriesDTO;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Assets;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.CostCentre;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.DistributorTargets;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Settings;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.User;
using Distributr.WSAPI.Lib.Services.WebService.MasterDataDTODeserialization;

namespace Distributr.WSAPI.Lib.Services.WebService.CommandDeserialization.Impl
{
    public class MasterDataDTODeserialize : IMasterDataDTODeserialize
    {
        private IMasterDataDTOValidation _masterDataDTOValidation;

        public MasterDataDTODeserialize(IMasterDataDTOValidation masterDataDtoValidation)
        {
            _masterDataDTOValidation = masterDataDtoValidation;
        }

        public MasterBaseDTO DeserializeMasterDataDTO(string masterDataCollective, string jsonMasterDataDTO)
        {
            MasterDataDTOSaveCollective ct = GetMasterDataCollective(masterDataCollective);
            if (ct == 0)
            {
                return null;
            }
            return DeserializeMasterDataDTO(ct, jsonMasterDataDTO);
        }

        private MasterBaseDTO DeserializeMasterDataDTO(MasterDataDTOSaveCollective ct, string jsonMasterDataDTO)
        {
            switch (ct)
            {

                case MasterDataDTOSaveCollective.Outlet:
                    OutletDTO outletDTO = null;
                    _masterDataDTOValidation.CanDeserializeMasterDataDTO(jsonMasterDataDTO, out outletDTO);
                    return outletDTO;
                case MasterDataDTOSaveCollective.Contact:
                    ContactDTO contactDTO = null;
                    _masterDataDTOValidation.CanDeserializeMasterDataDTO(jsonMasterDataDTO, out contactDTO);
                    return contactDTO;
                case MasterDataDTOSaveCollective.Asset:
                    AssetDTO assetDTO = null;
                    _masterDataDTOValidation.CanDeserializeMasterDataDTO(jsonMasterDataDTO, out assetDTO);
                    return assetDTO;
                case MasterDataDTOSaveCollective.DistributrFile:
                    DistributrFileDTO fileDTO = null;
                    _masterDataDTOValidation.CanDeserializeMasterDataDTO(jsonMasterDataDTO, out fileDTO);
                    return fileDTO;
                case MasterDataDTOSaveCollective.OutletVisitDay:
                    OutletVisitDayDTO outletVisitDTO = null;
                    _masterDataDTOValidation.CanDeserializeMasterDataDTO(jsonMasterDataDTO, out outletVisitDTO);
                    return outletVisitDTO;
                case MasterDataDTOSaveCollective.OutletPriority:
                    OutletPriorityDTO outletPriorityDTO = null;
                    _masterDataDTOValidation.CanDeserializeMasterDataDTO(jsonMasterDataDTO, out outletPriorityDTO);
                    return outletPriorityDTO;
                case MasterDataDTOSaveCollective.Target:
                    TargetDTO targetDTO = null;
                    _masterDataDTOValidation.CanDeserializeMasterDataDTO(jsonMasterDataDTO, out targetDTO);
                    return targetDTO;
                case MasterDataDTOSaveCollective.User:
                    UserDTO userDTO = null;
                    _masterDataDTOValidation.CanDeserializeMasterDataDTO(jsonMasterDataDTO, out userDTO);
                    return userDTO;
                case MasterDataDTOSaveCollective.DistributrSalesman:
                    DistributorSalesmanDTO distributorSalesmanDTO = null;
                    _masterDataDTOValidation.CanDeserializeMasterDataDTO(jsonMasterDataDTO, out distributorSalesmanDTO);
                    return distributorSalesmanDTO;
                case MasterDataDTOSaveCollective.PasswordChange:
                    ChangePasswordDTO changePasswordDTO = null;
                    _masterDataDTOValidation.CanDeserializeMasterDataDTO(jsonMasterDataDTO, out changePasswordDTO);
                    return changePasswordDTO;
                case MasterDataDTOSaveCollective.AppSettings:
                    AppSettingsDTO appSettingsDTO = null;
                    _masterDataDTOValidation.CanDeserializeMasterDataDTO(jsonMasterDataDTO, out appSettingsDTO);
                    return appSettingsDTO;
                case MasterDataDTOSaveCollective.InventorySerials:
                    InventorySerialsDTO inventorySerialsDTO = null;
                    _masterDataDTOValidation.CanDeserializeMasterDataDTO(jsonMasterDataDTO, out inventorySerialsDTO);
                    return inventorySerialsDTO;
                    break;
                default:
                    throw new Exception("Failed to deserialize MasterDataDTO in MasterDataDTO deserializer");
            }
            return null;
        }

        private MasterDataDTOSaveCollective GetMasterDataCollective(string masterDataCollective)
        {
            MasterDataDTOSaveCollective _masterDataCollective;
            Enum.TryParse(masterDataCollective, out _masterDataCollective);
            return _masterDataCollective;
        }
    }
}
