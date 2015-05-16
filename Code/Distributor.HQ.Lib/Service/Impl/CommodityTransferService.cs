using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.SettingsEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;
using Distributr.Core.Factory.SourcingDocuments;
using Distributr.Core.Repository.InventoryRepository;
using Distributr.Core.Repository.Master.CommodityRepositories;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.SettingsRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.SourcingDocumentRepositories;
using Distributr.Core.Workflow;
using Distributr.HQ.Lib.DTO;
using Distributr.HQ.Lib.ViewModels.Admin.User;
using Distributr.HQ.Lib.Workflows.CommodityStorage;
using StructureMap;

namespace Distributr.HQ.Lib.Service.Impl
{
    public class CommodityTransferService : ICommodityTransferService
    {
        private IStoreRepository _storeRepository;
        private ICommodityRepository _commodityRepository;
        private ICommodityStorageRepository _commodityStorageRepository;
        private ICostCentreRepository _costCentreRepository;
        private IUserRepository _userRepository;
        private ISettingsRepository _settingsRepository;
        private ICommodityReleaseRepository _commodityReleaseRepository;
        private ICommodityTransferRepository _commodityTransferRepository;

        public CommodityTransferService(IStoreRepository storeRepository, ICommodityRepository commodityRepository, ICommodityStorageRepository commodityStorageRepository, ICostCentreRepository costCentreRepository, IUserRepository userRepository, ISettingsRepository settingsRepository, ICommodityReleaseRepository commodityReleaseRepository, ICommodityTransferRepository commodityTransferRepository)
        {
            _storeRepository = storeRepository;
            _commodityRepository = commodityRepository;
            _commodityStorageRepository = commodityStorageRepository;
            _costCentreRepository = costCentreRepository;
            _userRepository = userRepository;
            _settingsRepository = settingsRepository;
            _commodityReleaseRepository = commodityReleaseRepository;
            _commodityTransferRepository = commodityTransferRepository;
        }

        public IList<StoreDTO> GetStores()
        {
            var storeList = new List<StoreDTO>();
            var stores = _storeRepository.GetAll();
            foreach (var item in stores)
            {
                var store = new StoreDTO()
                {
                    Id = item.Id,
                    Name = item.Name
                };
                storeList.Add(store);
            }
            return storeList;
        }

        public IList<CommodityDTO> GetCommodities(Guid storeId)
        {
            var commodityList = new List<CommodityDTO>();

            var commodityTrasferList = _commodityTransferRepository.GetAll();
            var commodityTransfer = commodityTrasferList.Where(n => n.DocumentIssuerCostCentre.Id.Equals(storeId)).ToList();
            foreach (CommodityTransferNote sourcingDocument in commodityTransfer)
            {
                if (sourcingDocument.Status != DocumentSourcingStatus.Approved) continue;
                foreach (var source in sourcingDocument.LineItems.Where(k => k.LineItemStatus == SourcingLineItemStatus.New).Select(l => l.Commodity.Id).Distinct())
                {
                    var commodities = _commodityRepository.GetById(source);
                    if(commodityList.Select(l=>l.Id).Distinct().Contains(commodities.Id)) continue;
                    var commodity = new CommodityDTO()
                    {
                        Id = commodities.Id,
                        Name = commodities.Name
                    };
                    commodityList.Add(commodity);
                }
            }
            return commodityList;
        }

        public IList<GradeDTO> GetGrades(Guid commodityId, Guid storeID)
        {
            var gradeList = new List<GradeDTO>();
            var commodityTransfer = _commodityTransferRepository.GetAll();
            foreach (CommodityTransferNote sourcingDocument in commodityTransfer.Where(n => n.DocumentIssuerCostCentre.Id.Equals(storeID)))
            {
                if (sourcingDocument.Status != DocumentSourcingStatus.Approved) continue;
                foreach (var source in sourcingDocument.LineItems.Where(l => l.Commodity.Id.Equals(commodityId) && l.LineItemStatus == SourcingLineItemStatus.New).Select(p => p.CommodityGrade.Id).Distinct())
                {
                    var CommodityGrade = _commodityRepository.GetGradeByGradeId(source);
                    if(gradeList.Select(k=>k.Id).Distinct().Contains(CommodityGrade.Id)) continue;
                    var grade = new GradeDTO()
                    {
                        Id = CommodityGrade.Id,
                        Name = CommodityGrade.Name
                    };
                    gradeList.Add(grade);
                }
            }
            return gradeList;
        }

        public IList<LineItemDTO> GetLineItems(Guid storeId, Guid commodityId, Guid gradeID)
        {
            var lineItemList = new List<LineItemDTO>();
            var commodityTransfer = _commodityTransferRepository.GetAll();
            foreach (CommodityTransferNote sourcingDocument in commodityTransfer.Where(n => n.DocumentIssuerCostCentre.Id.Equals(storeId)))
            {
                if (sourcingDocument.Status != DocumentSourcingStatus.Approved) continue;
                foreach (var items in sourcingDocument.LineItems)
                {
                    if (items.Commodity.Id.Equals(commodityId) && items.CommodityGrade.Id.Equals(gradeID) && items.LineItemStatus == SourcingLineItemStatus.New)
                    {
                        var store = _storeRepository.GetById(storeId);
                        var itemStore = new StoreDTO()
                        {
                            Id = store.Id,
                            Name = store.Name
                        };
                        var commodity = new CommodityDTO()
                        {
                            Id = items.Commodity.Id,
                            Name = items.Commodity.Name
                        };
                        var grade = new GradeDTO()
                        {
                            Id = items.CommodityGrade.Id,
                            Name = items.CommodityGrade.Name
                        };
                        var item = new LineItemDTO()
                        {
                            BatchNo = items.ContainerNo,
                            Store = itemStore,
                            Commodity = commodity,
                            Grade = grade,
                            IsSelected = false,
                            Id = items.Id,
                            Weight = items.Weight
                        };
                        lineItemList.Add(item);
                    }
                }
            }
            return lineItemList;
        }

        public void Transfer(List<TransferLineItemDTO> lineItemList, UserViewModel userViewModel)
        {
            var costcentre = _costCentreRepository.GetById(userViewModel.CostCentre);
            var store = _costCentreRepository.GetById(new Guid(lineItemList.First().StoreId));
            var user = _userRepository.GetById(userViewModel.Id);

            using (var c = ObjectFactory.Container.GetNestedContainer())
            {
                var factory = c.GetInstance<ICommodityReleaseNoteFactory>();

                var docRef = GetDocumentReference("CommodityRelease", costcentre);
                CommodityReleaseNote commodityReleaseNote = factory.Create(costcentre, costcentre.Id, store, user,
                        docRef, Guid.Empty, DateTime.Now, DateTime.Now, string.Format("{0},{0}", "Commodity storage note"));

                foreach (var item in lineItemList)
                {
                    var lineItem = factory.CreateLineItem(Guid.Empty, new Guid(item.ItemId), new Guid(item.CommodityId), new Guid(item.GradeId), Guid.Empty,
                        item.BatchNo, item.Weight, "Release note line item");
                    commodityReleaseNote.AddLineItem(lineItem);
                }

                commodityReleaseNote.Confirm();
                var commodityReleaseWorkFlow = c.GetInstance<ICommodityReleaseWFManager>();
                commodityReleaseWorkFlow.SubmitChanges(commodityReleaseNote);
            }
        }



        protected string GetDocumentReference(string docRef, CostCentre issuerCostCentre)
        {
            DateTime dt = DateTime.Now;
            string D = "";
            string DT = dt.ToString("yyyyMMdd");
            string TM = dt.ToString("hhmmss");
            string SN = "";
            string SC = "";
            string ON = "";
            string OC = "";
            string SQ = "";
            int sequenceId = 0;
            var salesman = _costCentreRepository.GetById(issuerCostCentre.Id);
            var outlet = _costCentreRepository.GetById(issuerCostCentre.Id);
            ON = outlet != null ? outlet.Name : "";
            OC = outlet != null ? outlet.CostCentreCode : "";
            SN = salesman != null ? salesman.Name : "";
            SC = salesman != null ? salesman.CostCentreCode : "";


            string refRule = "{D}_{SN}_{OC}_{DT}_{TM}_{SQ}";
            var docrefRule = _settingsRepository.GetByKey(SettingsKeys.DocReferenceRule);
            if (docrefRule != null)
            {
                refRule = docrefRule.Value;
            }

            D = "CRN";
            SQ = (_commodityReleaseRepository.GetCount() + 1).ToString();

            SQ = SQ.PadLeft(5, '0');
            string refno = refRule.Replace("{D}", D).Replace("{SN}", SN).Replace("{SC}", SC).Replace("{ON}", ON).Replace("{OC}", OC).Replace("{DT}", DT).Replace("{TM}", TM).Replace("{SQ}", SQ);
            refno = refno.Replace(" ", "");
            return refno;
        }
    }
}
