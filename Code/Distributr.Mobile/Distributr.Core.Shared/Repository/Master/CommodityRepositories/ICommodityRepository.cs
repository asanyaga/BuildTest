using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.CommodityEntities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.Core.Repository.Master.CommodityRepositories
{
    public interface ICommodityRepository : IRepositoryMaster<Commodity>
    {
        Commodity CreateCommodityItems(string commodityName, string commodityCode,
                                                      string commodityDescription,
                                                      Guid commodityType, string commodityGradeName, int usageTypeId,
                                                      string commodityGradeCode, string commodityGradeDescription);

        void AddCommodityGrade(Guid commodityId, Guid gradeId, string commodityGradeName, int usageTypeId,
                                                      string commodityGradeCode, string commodityGradeDescription);

        IEnumerable<CommodityGrade> GetAllGradeByCommodityId(Guid commodityId, bool includeDeactivated = false);
        void SetGradeInactive(Guid commodityId, Guid gradeId);
        void SetGradeActive(Guid commodityId, Guid gradeId);
        void SetGradeAsDeleted(Guid commodityId, Guid gradeId);
        CommodityGrade GetGradeByGradeId(Guid gradeId);


        QueryResult<Commodity> Query(QueryStandard q);
    }
}
