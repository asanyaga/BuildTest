using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.MasterDataDTO.DataContracts;
using Distributr.Core.Repository.Transactional.ThirdPartyIntegrationRepository;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.Core.Data.Repository.Transactional.ThirdPartyIntegrationRepository
{
    public class TransactionsSummary : ITransactionsSummary
    {
        private CokeDataContext _ctx;

        public TransactionsSummary(CokeDataContext ctx)
        {
            _ctx = ctx;
        }

        public object GetFarmerSummary(Guid farmerId)
        {
            var farmerCommPurchaseNotes = _ctx.tblSourcingDocument.Where(
                n => n.CommodityOwnerId == farmerId &&
                     n.DocumentTypeId == (int)DocumentType.CommodityPurchaseNote &&
                     n.DocumentStatusId == (int)DocumentSourcingStatus.Confirmed)
                     .OrderByDescending(n=> n.IM_DateCreated);



            DateTime lastTranDate = new DateTime();
            decimal qtyLastDelivered = 0;

            var lastTran = farmerCommPurchaseNotes.FirstOrDefault();
            if (lastTran != null)
            {
                lastTranDate = lastTran.IM_DateCreated;

                qtyLastDelivered = lastTran.tblSourcingLineItem
                    .Sum(n => (n.Weight == null ? 0 : n.Weight.Value));
            }

            decimal totalLastMonthWeight = farmerCommPurchaseNotes.Where(n => n.IM_DateCreated.Month == lastTranDate.Month)
                .SelectMany(n => n.tblSourcingLineItem)
                .Sum(n => (n.Weight == null ? 0 : n.Weight.Value));

            var summary = new
                              {
                                  totalLastMonthWeight,
                                  qtyLastDelivered,
                                  lastTranDate,
                              };
            return summary;
        }

        public decimal GetFarmerCummulativeWeightDelivered(Guid farmerId)
        {
            var farmerCommPurchaseNotes = _ctx.tblSourcingDocument.Where(
                n => n.CommodityOwnerId == farmerId &&
                     n.DocumentTypeId == (int) DocumentType.CommodityPurchaseNote &&
                     n.DocumentStatusId == (int) DocumentSourcingStatus.Confirmed);

            var farmerLineItems = farmerCommPurchaseNotes.SelectMany(n => n.tblSourcingLineItem);
                return farmerLineItems.Sum(n => (n.Weight == null ? 0 : n.Weight.Value));
        }


        public List<FarmerSummaryDTO> GetFarmerCummulativeWeightDeliveredByCode(string farmerCode)
        {
            var from = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var to = DateTime.Now;

            var farmerId = _ctx.tblCommodityOwner.FirstOrDefault(x => x.Code == farmerCode).Id;
            var farmerCommPurchaseNotes = _ctx.tblSourcingDocument.Where(
                n => n.CommodityOwnerId == farmerId &&
                     n.DocumentTypeId == (int)DocumentType.CommodityPurchaseNote &&
                     n.DocumentStatusId == (int)DocumentSourcingStatus.Confirmed &&
                     n.DocumentDate>=from && n.DocumentDate<=to);

            var farmerLineItems = farmerCommPurchaseNotes.SelectMany(n => n.tblSourcingLineItem);
            var items=farmerLineItems.GroupBy(n => n.CommodityId)
                .Select(group =>new FarmerSummary() {CummWeight= group.Sum(n => (n.Weight == null ? 0 : n.Weight.Value)),CommodityId= group.Key.Value}).ToList();

            var dto=items.Select(Map).ToList();

            return dto;
        }

        public FarmerSummaryDetail GetFarmerSummary(QueryFarmerDetails farmerQuery)
        {
             var result = new FarmerSummaryDetail();
            try
            {
               

                var farmerCode = farmerQuery.FarmerCode;
                var farmerMobile = farmerQuery.Mobile;
                DateTime to;
                DateTime from;

                var farmer = _ctx.tblCommodityOwner.FirstOrDefault(x => x.Code == farmerCode);


                if (farmer == null)
                {
                    result.StatusCode = (int) FarmerQueryStatusCode.CodeNotFound;
                    result.StatusDetail = string.Format("Farmer with status code \"{0}\" not found", farmerCode);
                    result.SummaryDetail = null;

                    return result;
                }


                //if (farmerMobile != farmer.PhoneNo || farmerMobile != farmer.OfficeNo)
                //{
                //    result.StatusCode = (int) FarmerQueryStatusCode.NumberNotAuthenticated;
                //    result.StatusDetail = string.Format("The number used to Query does not belong to this farmer");
                //    result.SummaryDetail = null;

                //    return result;
                //}

                if (farmerQuery.Month!=string.Empty)
                {
                    var month=DateTime.ParseExact(farmerQuery.Month, "MMMM", CultureInfo.CurrentCulture).Month;
                    var daysInMonth = DateTime.DaysInMonth(DateTime.Now.Year, month);
                    from = new DateTime(DateTime.Now.Year, month, 1);
                    to = new DateTime(DateTime.Now.Year, month, daysInMonth);//DateTime.Now;
                }
                else
                {
                    from = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    to = DateTime.Now;
                }

               

                var farmerId = farmer.Id;
                var farmerCommPurchaseNotes = _ctx.tblSourcingDocument.Where(
                    n => n.CommodityOwnerId == farmerId &&
                         n.DocumentTypeId == (int) DocumentType.CommodityPurchaseNote &&
                         n.DocumentStatusId == (int) DocumentSourcingStatus.Confirmed &&
                         n.DocumentDate >= from && n.DocumentDate <= to);

                var farmerLineItems = farmerCommPurchaseNotes.SelectMany(n => n.tblSourcingLineItem);
                var items = farmerLineItems.GroupBy(n => n.CommodityId)
                    .Select(
                        group =>
                            new FarmerSummary()
                            {
                                CummWeight = group.Sum(n => (n.Weight == null ? 0 : n.Weight.Value)),
                                CommodityId = group.Key.Value
                            }).ToList();

                var dto = items.Select(Map).ToList();

                result.StatusCode = (int) FarmerQueryStatusCode.Success;
                result.StatusDetail = "Succesful";
                result.SummaryDetail = dto;

                return result;
            }

            catch (Exception ex)
            {
                result.StatusCode =(int) FarmerQueryStatusCode.Failure;
                result.StatusDetail = string.Format("Error {0}", ex.Message);
                result.SummaryDetail = null;

                return result;
            }
            
        }

        public decimal GetFarmerCummulativeWeightDelivered(Guid farmerId, DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        private FarmerSummaryDTO Map(FarmerSummary item)
        {
            var commodityName = _ctx.tblCommodity.FirstOrDefault(x=>x.Id==item.CommodityId).Name;

            var dto = new FarmerSummaryDTO();
            dto.CommodityName = commodityName;
            dto.CummWeight = item.CummWeight;

            return dto;
        }
    }
}
