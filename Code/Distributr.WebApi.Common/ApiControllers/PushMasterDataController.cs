using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Transactions;
using System.Web.Http;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CentreEntity;
using Distributr.Core.Domain.Master.CommodityEntity;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.FarmActivities;
using Distributr.Core.Domain.Master.OutletVisitReasonsTypeEntities;
using Distributr.Core.MasterDataDTO;
using Distributr.Core.MasterDataDTO.DTOModels.AgrimanagrDTO.CommodityDTOs;
using Distributr.Core.MasterDataDTO.DTOModels.AgrimanagrDTO.FarmActivities;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.CostCentre;
using Distributr.Core.Repository.Master.Agrimanagr;
using Distributr.Core.Repository.Master.BankRepositories;
using Distributr.Core.Repository.Master.CentreRepositories;
using Distributr.Core.Repository.Master.CommodityOwnerRepository;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.MasterDataAllocationRepositories;
using Distributr.Core.Repository.Master.OutletVisitReasonsTypeRepositories;
using Distributr.Core.Repository.Master.SuppliersRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.Mapping;
using Distributr.Core.Utility.Validation;
using Distributr.WSAPI.Lib.Services.Bus;
using Distributr.WSAPI.Lib.Services.WebService.CommandDeserialization;
using Newtonsoft.Json.Linq;

namespace Distributr.WebApi.ApiControllers
{
    public class PushMasterDataController : ApiController
    {
        private IMasterDataDTODeserialize _masterDataDTODeserialize;
        private IPublishMasterData _publishMasterData;
        private ICostCentreRepository _costCentreRepository;
        private ICommodityOwnerTypeRepository _commodityOwnerTypeRepository;
        private ICommodityOwnerRepository _commodityOwnerRepository;
        private ICommodityProducerRepository _commodityProducerRepository;
        private ICentreRepository _centreRepository;
        private IMasterDataAllocationRepository _masterDataAllocationRepository;
        private IInfectionRepository _infectionRepository;
        private ISeasonRepository _seasonRepository;
        private IServiceRepository _commodityProducerServiceRepository;
        private IShiftRepository _shiftRepository;
        private IServiceProviderRepository _serviceProviderRepository;
        private IBankBranchRepository _bankBranchRepository;
        private IBankRepository _bankRepository;
        private IOutletVisitReasonsTypeRepository _outletVisitReasonsTypeRepository;
        private IDTOToEntityMapping _dtoToEntityMapping;
        private ISalesmanSupplierRepository _salesmanSupplierRepository;
        private ISupplierRepository _supplierRepository;

        public PushMasterDataController(IMasterDataDTODeserialize masterDataDtoDeserialize, IPublishMasterData publishMasterData, ICostCentreRepository costCentreRepository, ICommodityOwnerTypeRepository commodityOwnerTypeRepository, ICommodityOwnerRepository commodityOwnerRepository, ICommodityProducerRepository commodityProducerRepository, ICentreRepository centreRepository, IMasterDataAllocationRepository masterDataAllocationRepository, IInfectionRepository infectionRepository, ISalesmanSupplierRepository salesmanSupplierRepository, ISeasonRepository seasonRepository, IServiceRepository commodityProducerServiceRepository, IShiftRepository shiftRepository, IServiceProviderRepository serviceProviderRepository, IBankRepository bankRepository, IBankBranchRepository bankBranchRepository, IDTOToEntityMapping dtoToEntityMapping, IOutletVisitReasonsTypeRepository outletVisitReasonsTypeRepository, ISupplierRepository supplierRepository)
        {
            _masterDataDTODeserialize = masterDataDtoDeserialize;
            _publishMasterData = publishMasterData;
            _costCentreRepository = costCentreRepository;
            _commodityOwnerTypeRepository = commodityOwnerTypeRepository;
            _commodityOwnerRepository = commodityOwnerRepository;
            _commodityProducerRepository = commodityProducerRepository;
            _centreRepository = centreRepository;
            _masterDataAllocationRepository = masterDataAllocationRepository;
            _infectionRepository = infectionRepository;
            _seasonRepository = seasonRepository;
            _commodityProducerServiceRepository = commodityProducerServiceRepository;
            _shiftRepository = shiftRepository;
            _serviceProviderRepository = serviceProviderRepository;
            _bankRepository = bankRepository;
            _bankBranchRepository = bankBranchRepository;
            _dtoToEntityMapping = dtoToEntityMapping;
            _outletVisitReasonsTypeRepository = outletVisitReasonsTypeRepository;
            _supplierRepository = supplierRepository;
            _salesmanSupplierRepository = salesmanSupplierRepository;
        }

        [HttpPost]
        public HttpResponseMessage Run(JObject pushMasterDataDto)
        {
            var response = new ResponseBool();
            string masterDataCollective = pushMasterDataDto["MasterDataCollective"].ToString();
            string jsonDTO = pushMasterDataDto["MasterDto"].ToString();
            MasterBaseDTO data = _masterDataDTODeserialize.DeserializeMasterDataDTO(masterDataCollective, jsonDTO);
            if (data != null)
            {
                MasterDataDTOSaveCollective ct = GetMasterDataCollective(masterDataCollective);
                _publishMasterData.Publish(data, ct);
                response.Success = true;
            }

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
        private MasterDataDTOSaveCollective GetMasterDataCollective(string masterDataCollective)
        {
            MasterDataDTOSaveCollective _masterDataCollective;
            Enum.TryParse(masterDataCollective, out _masterDataCollective);
            return _masterDataCollective;
        }
        [HttpPost]
        public HttpResponseMessage SaveCommoditySupplier(CommoditySupplierDTO dto)
        {
            var response = new ResponseBasic();
            try
            {
                //CommoditySupplier entity1 = _dtoToEntityMapping.Map(dto);
                CommoditySupplier entity = new CommoditySupplier(dto.MasterId);
                entity.JoinDate = dto.JoinDate;
                entity.CostCentreCode = dto.CostCentreCode;
                entity.AccountName = dto.AccountName;
                entity.BankBranchId = dto.BankBranchId;
                entity.BankId = dto.BankId;
                entity.CommoditySupplierType = (CommoditySupplierType)dto.CommoditySupplierTypeId;
                entity.AccountNo = dto.AccountNo;
                entity.CostCentreType = CostCentreType.CommoditySupplier;
                entity.Name = dto.Name;
                entity.ParentCostCentre = new CostCentreRef { Id = dto.ParentCostCentreId };
                entity.PinNo = dto.PinNo;
                entity._Status = (EntityStatus)dto.StatusId;
                _costCentreRepository.Save(entity);
                response.Result = "OK";
                response.ResultInfo = "OK";
                response.ErrorInfo = "OK";
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: Invalid Commodity supplier fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + ".\n"));
                response.ErrorInfo = errorMsg;
                response.Result = "OK";
                response.ResultInfo = "OK";

            }
            catch (Exception ex)
            {
                response.Result = "OK";
                response.ErrorInfo = ex.Message;
                response.ResultInfo = "OK";
            }

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
        [HttpPost]
        public HttpResponseMessage SaveCommodityOwner(CommodityOwnerDTO dto)
        {
            var response = new ResponseBasic();
            try
            {
                CommodityOwner entity = new CommodityOwner(dto.MasterId);
                entity.FirstName = dto.FirstName;
                entity.LastName = dto.LastName;
                entity.Surname = dto.Surname;
                entity.PhoneNumber = dto.PhoneNumber;
                entity.DateOfBirth = dto.DateOfBirth;
                entity.BusinessNumber = dto.BusinessNumber;
                entity.Code = dto.Code;
                entity.CommodityOwnerType = _commodityOwnerTypeRepository.GetById(dto.CommodityOwnerTypeId);
                entity.CommoditySupplier = _costCentreRepository.GetById(dto.CommoditySupplierId) as CommoditySupplier;
                entity.Description = dto.Description;
                entity.PinNo = dto.PinNo;
                entity.Gender = (Gender)dto.GenderId;
                entity.Email = dto.Email;
                entity.FaxNumber = dto.FaxNumber;
                entity.IdNo = dto.IdNo;
                entity.LastName = dto.LastName;
                entity.OfficeNumber = dto.OfficeNumber;
                entity.MaritalStatus = (MaritalStatas)dto.MaritalStatusId;
                entity.PhoneNumber = dto.PhoneNumber;
                entity.PostalAddress = dto.PostalAddress;
                entity.PhysicalAddress = dto.PhysicalAddress;
                entity._Status = (EntityStatus)dto.StatusId;
                _commodityOwnerRepository.Save(entity);
                response.Result = "OK";
                response.ResultInfo = "OK";
                response.ErrorInfo = "OK";
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: Invalid Commodity Owner fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + ".\n"));
                response.ErrorInfo = errorMsg;
                response.Result = "OK";
                response.ResultInfo = "OK";

            }
            catch (Exception ex)
            {
                response.Result = "OK";
                response.ErrorInfo = ex.Message;
                response.ResultInfo = "OK";
            }

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpPost]
        public HttpResponseMessage SaveCommodityProvider(CommodityProducerDTO dto)
        {
            var response = new ResponseBasic();
            try
            {
                CommodityProducer entity = new CommodityProducer(dto.MasterId);
                entity.Acrage = dto.Acrage;

                entity.Code = dto.Code;

                entity.CommoditySupplier = _costCentreRepository.GetById(dto.CommoditySupplierId) as CommoditySupplier;
                entity.Description = dto.Description;
                entity.Name = dto.Name;
                entity.PhysicalAddress = dto.PhysicalAddress;
                entity.RegNo = dto.RegNo;

                foreach (var centreId in dto.CenterIds)
                {
                    entity.CommodityProducerCentres.Add(_centreRepository.GetById(centreId));
                }




                using (TransactionScope scope = TransactionUtils.CreateTransactionScope())
                {
                    try
                    {
                        _commodityProducerRepository.Save(entity);
                        var existingAllocationForThisProducer = _masterDataAllocationRepository.GetByAllocationType(
                            MasterDataAllocationType.CommodityProducerCentreAllocation)
                            .Where(n => n.EntityAId == entity.Id);

                        var unallocated =
                            existingAllocationForThisProducer.Where(
                                n =>
                                entity.CommodityProducerCentres.Select(c => c.Id).All(cId => n.EntityBId != cId));

                        foreach (var centre in entity.CommodityProducerCentres)
                        {
                            var allocation = new MasterDataAllocation(Guid.NewGuid())
                            {
                                _Status = EntityStatus.Active,
                                AllocationType =
                                    MasterDataAllocationType.CommodityProducerCentreAllocation,
                                EntityAId = entity.Id,
                                EntityBId = centre.Id
                            };
                            _masterDataAllocationRepository.Save(allocation);
                        }

                        foreach (var allocation in unallocated)
                        {
                            _masterDataAllocationRepository.DeleteAllocation(allocation.Id);
                        }

                        response.Result = "OK";
                        response.ResultInfo = "OK";
                        response.ErrorInfo = "OK";
                        scope.Complete();
                    }
                    catch (DomainValidationException dve)
                    {
                        string errorMsg =
                            dve.ValidationResults.Results.Aggregate("Error: Invalid commodity producer fields.\n",
                                                                    (current, msg) =>
                                                                    current + ("\t- " + msg.ErrorMessage + "\n"));
                        response.ErrorInfo = errorMsg;

                    }
                    catch (Exception ex) //any other
                    {
                        response.ErrorInfo = "Error: An error occurred when saving the commodity producer.\n" +
                                             ex.ToString();

                    }
                }
                //    entity._Status = (EntityStatus)dto.StatusId;

                //    foreach (var centerId in dto.CenterIds)
                //    {
                //        var centre = _centreRepository.GetById(centerId);
                //        if (centre != null)
                //        {
                //            entity.CommodityProducerCentres.Add(centre);

                //        }

                //    }
                //    _commodityProducerRepository.Save(entity);

                //    foreach (var centre in entity.CommodityProducerCentres)
                //    {
                //        var allocation = new MasterDataAllocation(Guid.NewGuid())
                //        {
                //            _Status = EntityStatus.Active,
                //            AllocationType =
                //                MasterDataAllocationType.CommodityProducerCentreAllocation,
                //            EntityAId = entity.Id,
                //            EntityBId = centre.Id
                //        };
                //        _masterDataAllocationRepository.Save(allocation);


                //    }

                //    response.Result = "OK";
                //    response.ResultInfo = "OK";
                //    response.ErrorInfo = "OK";
                //}
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: Invalid Commodity Owner fields.\n",
                    (current, msg) => current + ("\t- " + msg.ErrorMessage + ".\n"));
                response.ErrorInfo = errorMsg;
                response.Result = "OK";
                response.ResultInfo = "OK";

            }
            catch (Exception ex)
            {
                response.Result = "OK";
                response.ErrorInfo = ex.Message;
                response.ResultInfo = "OK";
            }

            return Request.CreateResponse(HttpStatusCode.OK, response);

        }

        [HttpPost]
        public HttpResponseMessage SaveInfection(InfectionDTO dto)
        {
            var response = new ResponseBool();
            try
            {
                var entity = new Infection(dto.MasterId);
                entity.Code = dto.Code;
                entity.Name = dto.Name;
                entity.InfectionType = (InfectionType)dto.InfectionTypeId;
                entity.Description = dto.Description;
                entity._Status = (EntityStatus)dto.StatusId;
                _infectionRepository.Save(entity);
                response.Success = true;

                response.ErrorInfo = "OK";
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: Invalid Infection fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + ".\n"));
                response.ErrorInfo = errorMsg;
                response.Success = false;


            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorInfo = ex.Message;

            }

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpPost]
        public HttpResponseMessage SaveSeason(SeasonDTO dto)
        {
            var response = new ResponseBool();
            try
            {
                Season entity = new Season(dto.MasterId);
                entity.Code = dto.Code;
                entity.Name = dto.Name;
                entity.CommodityProducer = _commodityProducerRepository.GetById(dto.CommodityProducerId);
                entity.Description = dto.Description;
                entity.StartDate = dto.StartDate;
                entity.EndDate = dto.EndDate;
                entity._Status = (EntityStatus)dto.StatusId;
                _seasonRepository.Save(entity);
                response.Success = true;

                response.ErrorInfo = "OK";
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: Invalid Season fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + ".\n"));
                response.ErrorInfo = errorMsg;
                response.Success = false;


            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorInfo = ex.Message;

            }

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpPost]
        public HttpResponseMessage SaveCommodityProducerService(ServiceDTO dto)
        {
            var response = new ResponseBool();
            try
            {
                var entity = new CommodityProducerService(dto.MasterId)
                {
                    Code = dto.Code,
                    Name = dto.Name,
                    Cost = dto.Cost,
                    Description = dto.Description,
                    _Status = (EntityStatus)dto.StatusId
                };
                _commodityProducerServiceRepository.Save(entity);
                response.Success = true;

                response.ErrorInfo = "OK";
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: Invalid Commodity Producer Service fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + ".\n"));
                response.ErrorInfo = errorMsg;
                response.Success = false;


            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorInfo = ex.Message;

            }

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpPost]
        public HttpResponseMessage SaveShift(ShiftDTO dto)
        {
            var response = new ResponseBool();
            try
            {
                var entity = new Shift(dto.MasterId)
                {
                    Code = dto.Code,
                    Name = dto.Name,
                    StartTime = dto.StartTime,
                    EndTime = dto.EndTime,
                    Description = dto.Description,
                    _Status = (EntityStatus)dto.StatusId
                };
                _shiftRepository.Save(entity);
                response.Success = true;

                response.ErrorInfo = "OK";
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: Invalid Shift fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + ".\n"));
                response.ErrorInfo = errorMsg;
                response.Success = false;


            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorInfo = ex.Message;

            }

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpPost]
        public HttpResponseMessage SaveServiceProvider(ServiceProviderDTO dto)
        {
            var response = new ResponseBool();
            try
            {
                var entity = new ServiceProvider(dto.MasterId)
                {
                    Code = dto.Code,
                    Name = dto.Name,
                    IdNo = dto.IdNo,
                    PinNo = dto.PinNo,
                    MobileNumber = dto.MobileNumber,
                    AccountName = dto.AccountName,
                    AccountNumber = dto.AccountNumber,
                    Bank = _bankRepository.GetById(dto.BankId),
                    BankBranch = _bankBranchRepository.GetById(dto.BankBranchId),
                    Gender = (Gender)dto.GenderId,
                    Description = dto.Description,
                    _Status = (EntityStatus)dto.StatusId
                };
                _serviceProviderRepository.Save(entity);
                response.Success = true;

                response.ErrorInfo = "OK";
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: Invalid Service Provider fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + ".\n"));
                response.ErrorInfo = errorMsg;
                response.Success = false;


            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorInfo = ex.Message;

            }

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet]
        public HttpResponseMessage InfectionDelete(Guid id)
        {
            var response = new ResponseBool { Success = false };
            try
            {
                var infection = _infectionRepository.GetById(id);
                if (infection != null) _infectionRepository.SetAsDeleted(infection);

                response.Success = true;
                response.ErrorInfo = "Successfully deleted infection";
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: In entity fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                response.Success = false;

            }
            catch (Exception ex) //any other
            {
                response.Success = false;
                response.ErrorInfo = "Error: An error occurred when deleting the entity.";

            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet]
        public HttpResponseMessage ShiftDelete(Guid id)
        {
            var response = new ResponseBool { Success = false };
            try
            {
                var shift = _shiftRepository.GetById(id);
                if (shift != null) _shiftRepository.SetAsDeleted(shift);

                response.Success = true;
                response.ErrorInfo = "Successfully deleted Shift";
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: In entity fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                response.Success = false;

            }
            catch (Exception ex) //any other
            {
                response.Success = false;
                response.ErrorInfo = "Error: An error occurred when deleting the entity.";

            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet]
        public HttpResponseMessage SeasonDelete(Guid id)
        {
            var response = new ResponseBool { Success = false };
            try
            {
                var season = _seasonRepository.GetById(id);
                if (season != null) _seasonRepository.SetAsDeleted(season);

                response.Success = true;
                response.ErrorInfo = "Successfully deleted Season";
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: In entity fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                response.Success = false;

            }
            catch (Exception ex) //any other
            {
                response.Success = false;
                response.ErrorInfo = "Error: An error occurred when deleting the entity.";

            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet]
        public HttpResponseMessage ServiceProviderDelete(Guid id)
        {
            var response = new ResponseBool { Success = false };
            try
            {
                var serviceprovider = _serviceProviderRepository.GetById(id);
                if (serviceprovider != null) _serviceProviderRepository.SetAsDeleted(serviceprovider);

                response.Success = true;
                response.ErrorInfo = "Successfully deleted Service Provider";
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: In entity fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                response.Success = false;

            }
            catch (Exception ex) //any other
            {
                response.Success = false;
                response.ErrorInfo = "Error: An error occurred when deleting the entity.";

            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet]
        public HttpResponseMessage CommodityProducerServiceDelete(Guid id)
        {
            var response = new ResponseBool { Success = false };
            try
            {
                var commodityProducerService = _commodityProducerServiceRepository.GetById(id);
                if (commodityProducerService != null) _commodityProducerServiceRepository.SetAsDeleted(commodityProducerService);

                response.Success = true;
                response.ErrorInfo = "Successfully deleted Commodity Producer Service";
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: In entity fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                response.Success = false;

            }
            catch (Exception ex) //any other
            {
                response.Success = false;
                response.ErrorInfo = "Error: An error occurred when deleting the entity.";

            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet]
        public HttpResponseMessage InfectionActivateOrDeactivate(Guid id)
        {
            var response = new ResponseBool { Success = false };
            try
            {
                var infection = _infectionRepository.GetById(id);
                if (infection != null && infection._Status == EntityStatus.Inactive)
                {
                    _infectionRepository.SetActive(infection);
                    response.ErrorInfo = "Successfully activated Infection";
                }
                else if (infection != null && infection._Status == EntityStatus.Active)
                {
                    _infectionRepository.SetInactive(infection);
                    response.ErrorInfo = "Successfully deactivated Infection";
                }

                response.Success = true;
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: In entity fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;

            }
            catch (Exception ex) //any other
            {
                response.ErrorInfo = "Error: An error occurred when editing the entity.";

            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet]
        public HttpResponseMessage SeasonActivateOrDeactivate(Guid id)
        {
            var response = new ResponseBool { Success = false };
            try
            {
                var season = _seasonRepository.GetById(id);
                if (season != null && season._Status == EntityStatus.Inactive)
                {
                    _seasonRepository.SetActive(season);
                    response.ErrorInfo = "Successfully activated Season";
                }
                else if (season != null && season._Status == EntityStatus.Active)
                {
                    _seasonRepository.SetInactive(season);
                    response.ErrorInfo = "Successfully deactivated Season";
                }

                response.Success = true;
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: In entity fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;

            }
            catch (Exception ex) //any other
            {
                response.ErrorInfo = "Error: An error occurred when editing the entity.";

            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet]
        public HttpResponseMessage ShiftActivateOrDeactivate(Guid id)
        {
            var response = new ResponseBool { Success = false };
            try
            {
                var shift = _shiftRepository.GetById(id);
                if (shift != null && shift._Status == EntityStatus.Inactive)
                {
                    _shiftRepository.SetActive(shift);
                    response.ErrorInfo = "Successfully activated Shift";
                }
                else if (shift != null && shift._Status == EntityStatus.Active)
                {
                    _shiftRepository.SetInactive(shift);
                    response.ErrorInfo = "Successfully deactivated Shift";
                }

                response.Success = true;
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: In entity fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;

            }
            catch (Exception ex) //any other
            {
                response.ErrorInfo = "Error: An error occurred when editing the entity.";

            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet]
        public HttpResponseMessage ServiceProviderActivateOrDeactivate(Guid id)
        {
            var response = new ResponseBool { Success = false };
            try
            {
                var serviceProvider = _serviceProviderRepository.GetById(id);
                if (serviceProvider != null && serviceProvider._Status == EntityStatus.Inactive)
                {
                    _serviceProviderRepository.SetActive(serviceProvider);
                    response.ErrorInfo = "Successfully activated Service Provider";
                }
                else if (serviceProvider != null && serviceProvider._Status == EntityStatus.Active)
                {
                    _serviceProviderRepository.SetInactive(serviceProvider);
                    response.ErrorInfo = "Successfully deactivated Service Provider";
                }

                response.Success = true;
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: In entity fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;

            }
            catch (Exception ex) //any other
            {
                response.ErrorInfo = "Error: An error occurred when editing the entity.";

            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet]
        public HttpResponseMessage CommodityProducerServiceActivateOrDeactivate(Guid id)
        {
            var response = new ResponseBool { Success = false };
            try
            {
                var commodityProducerService = _commodityProducerServiceRepository.GetById(id);
                if (commodityProducerService != null && commodityProducerService._Status == EntityStatus.Inactive)
                {
                    _commodityProducerServiceRepository.SetActive(commodityProducerService);
                    response.ErrorInfo = "Successfully activated Commodity Producer Service";
                }
                else if (commodityProducerService != null && commodityProducerService._Status == EntityStatus.Active)
                {
                    _commodityProducerServiceRepository.SetInactive(commodityProducerService);
                    response.ErrorInfo = "Successfully deactivated commodity Producer Service";
                }

                response.Success = true;
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: In entity fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;

            }
            catch (Exception ex) //any other
            {
                response.ErrorInfo = "Error: An error occurred when editing the entity.";

            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpPost]
        public HttpResponseMessage SaveOutletVisitReasonType(OutletVisitReasonTypeDTO dto)
        {
            var response = new ResponseBool();
            try
            {
                var entity = new OutletVisitReasonsType(dto.MasterId);
                entity.Name = dto.Name;
                entity.OutletVisitAction = (OutletVisitAction)dto.OutletVisitActionId;
                entity.Description = dto.Description;
                entity._Status = (EntityStatus)dto.StatusId;
                _outletVisitReasonsTypeRepository.Save(entity);
                response.Success = true;

                response.ErrorInfo = "OK";
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: Invalid Outlet Visit Reason fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + ".\n"));
                response.ErrorInfo = errorMsg;
                response.Success = false;


            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorInfo = ex.Message;

            }

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet]
        public HttpResponseMessage OutletVisitReasonTypeDelete(Guid id)
        {
            var response = new ResponseBool { Success = false };
            try
            {
                var outletVisitReasonType = _outletVisitReasonsTypeRepository.GetById(id);
                if (outletVisitReasonType != null) _outletVisitReasonsTypeRepository.SetAsDeleted(outletVisitReasonType);

                response.Success = true;
                response.ErrorInfo = "Successfully deleted outlet visit reason";
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: In entity fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                response.Success = false;

            }
            catch (Exception ex) //any other
            {
                response.Success = false;
                response.ErrorInfo = "Error: An error occurred when deleting the entity.";

            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }


        [HttpPost]
        public HttpResponseMessage SaveSalesmanSupplier(List<SalesmanSupplierDTO> dto)
        {
            var response = new ResponseBool();
            try
            {
                if (!dto.Any())
                {
                    response.ErrorInfo = "Select Supplier";
                    response.Success = false;
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                var salesmanId = dto.FirstOrDefault().DistributorSalesmanMasterId;
                var existingSuppliers = _salesmanSupplierRepository.GetBySalesman(salesmanId).Select(s => s.Supplier.Id).ToList();
                var assignedsuppliers = dto.Where(s => s.Assigned).Select(s => s.SupplierMasterId).ToList();

                var deletedsuppliers = existingSuppliers.Where(s => !assignedsuppliers.Contains(s)).ToList();
                foreach (var deletedsupplier in deletedsuppliers)
                {
                    var assigned = _salesmanSupplierRepository.GetBySalesmanAndSupplier(deletedsupplier, salesmanId);
                    if (assigned != null)
                        _salesmanSupplierRepository.SetAsDeleted(assigned);
                }
                foreach (var newsupplier in assignedsuppliers)
                {
                    var assigned = _salesmanSupplierRepository.GetBySalesmanAndSupplier(newsupplier, salesmanId);
                    if (assigned == null)
                    {
                        assigned = new SalesmanSupplier(Guid.NewGuid());
                        assigned.DistributorSalesmanRef = new CostCentreRef { Id = salesmanId };
                        assigned.Supplier = _supplierRepository.GetById(newsupplier);
                    }
                    _salesmanSupplierRepository.Save(assigned);
                }

                response.Success = true;
                response.ErrorInfo = "OK";
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: Invalid Infection fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + ".\n"));
                response.ErrorInfo = errorMsg;
                response.Success = false;


            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorInfo = ex.Message;

            }

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
    }
}
