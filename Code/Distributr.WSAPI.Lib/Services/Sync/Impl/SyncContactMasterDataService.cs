using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.CostCentre;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.WSAPI.Lib.System.Utility;

namespace Distributr.WSAPI.Lib.Services.Sync.Impl
{
    public class SyncContactMasterDataService : SyncMasterDataBase, ISyncContactMasterDataService
    {
        private readonly CokeDataContext _context;

        public SyncContactMasterDataService(CokeDataContext context) : base(context)
        {
            _context = context;
        }

        public SyncResponseMasterDataInfo<ContactDTO> GetContact(QueryMasterData myQuery)
        {
            var response = new SyncResponseMasterDataInfo<ContactDTO>();
            response.MasterData = new SyncMasterDataInfo<ContactDTO>();
            response.MasterData.EntityName = MasterDataCollective.Contact.ToString();
            try
            {
                var costCentre = GetSyncCostCentre(myQuery.ApplicationId);
                var query = _context.tblContact.AsQueryable();
                query = query.Where(n => n.IM_DateLastUpdated > myQuery.From
                    && (n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive));

                var deletedQuery = _context.tblContact.AsQueryable();
                deletedQuery = deletedQuery.Where(n => n.IM_DateLastUpdated > myQuery.From
                    && (n.IM_Status == (int)EntityStatus.Deleted));

                if (costCentre != null)
                {
                    List<Guid> reqCostCentreIds;
                    switch (costCentre.CostCentreType)
                    {
                        case CostCentreType.Distributor:
                        case CostCentreType.Hub:
                            
                            var contactIds =
                                _context.ExecuteStoreQuery<Guid>(string.Format(SyncResources.SyncResource.DHubContact,
                                    costCentre.Id)).ToList();
                            query = query.Where(n => contactIds.Contains(n.id) || n.id != null ).OrderBy(s => s.IM_DateCreated);
                            deletedQuery = deletedQuery.Where(n => contactIds.Contains(n.id)).OrderBy(s => s.IM_DateCreated);
                            break;
                        case CostCentreType.DistributorSalesman:
                        case CostCentreType.PurchasingClerk:
                            var parentId = costCentre.TblCostCentre.ParentCostCentreId;
                            string sql = string.Format(SyncResources.SyncResource.SalesmanContact,
                                costCentre.TblCostCentre.Id);
                             var salesmanContactIds =
                                _context.ExecuteStoreQuery<Guid>(sql).ToList();
                            query = query.Where(n => salesmanContactIds.Contains(n.id)).OrderBy(s => s.IM_DateCreated);
                            deletedQuery = deletedQuery.Where(n => salesmanContactIds.Contains(n.CostCenterId)).OrderBy(s => s.IM_DateCreated);
                            break;
                    }
                }
                if (myQuery.Skip.HasValue && myQuery.Take.HasValue)
                    query = query.Skip(myQuery.Skip.Value).Take(myQuery.Take.Value);

                if (myQuery.Skip.HasValue && myQuery.Skip.Value == 0)
                {
                    response.DeletedItems = deletedQuery.Select(s => s.id).ToList();
                }
                response.MasterData.MasterDataItems = query.ToList().Select(n => Map(n)).ToArray();
                response.ErrorInfo = "Success";
            }
            catch (Exception ex)
            {
                response.ErrorInfo = ex.Message;
            }
            response.MasterData.LastSyncTimeStamp = DateTime.Now;
            return response;
        }

        private ContactDTO Map(tblContact tbl)
        {
            var dto = new ContactDTO
                          {
                              MasterId = tbl.id,
                              DateCreated = tbl.IM_DateCreated,
                              DateLastUpdated = tbl.IM_DateLastUpdated,
                              StatusId = tbl.IM_Status,
                              Firstname = tbl.Firstname,
                              Lastname = tbl.Lastname,
                              BusinessPhone = tbl.BusinessPhone,
                              Fax = tbl.Fax,
                              MobilePhone = tbl.MobilePhone,
                              PhysicalAddress = tbl.PhysicalAddress,
                              PostalAddress = tbl.PostalAddress,
                              SpouseName = tbl.SpouseName,
                              Email = tbl.Email,
                              Company = tbl.Company,
                              JobTitle = tbl.JobTitle,
                              HomeTown = tbl.HomeTown,
                              HomePhone = tbl.HomePhone,
                              WorkExtPhone = tbl.WorkExtPhone,
                              ChildrenNames = tbl.ChildrenNames,
                              City = tbl.City,
                              ContactTypeMasterId = tbl.ContactType ?? Guid.Empty,
                              ContactOwnerMasterId = tbl.CostCenterId,
                              MaritalStatusMasterId = tbl.MaritalStatusId ?? 0,
                              ContactClassificationId = tbl.ContactClassification ?? 0,
                              ContactOwnerType = tbl.ContactOwner,
                              DateOfBirth = tbl.DateOfBirth
                          };
            return dto;
        }
    }
}
