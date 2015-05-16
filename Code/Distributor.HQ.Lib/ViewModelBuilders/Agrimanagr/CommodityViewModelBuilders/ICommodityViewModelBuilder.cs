using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.CommodityEntities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.ViewModels.Agrimanagr.CommodityViewModel;

namespace Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.CommodityViewModelBuilders
{
    public interface ICommodityViewModelBuilder
    {
       
        IList<CommodityViewModel> Search(string searchParam, bool inactive = false);
        IList<CommodityGradeViewModel> Search(Guid commodityId, string searchParam, bool inactive = false);
        CommodityViewModel Get(Guid id);
        void Save(CommodityViewModel commodityViewModel);
        void SetInactive(Guid id);
        void SetAsDeleted(Guid id);
        void SetActive(Guid id);
        Dictionary<Guid, string> CommodityTypeList();
        void AddCommodityGrades(Guid commodityId, Guid gradeId, string commodityGradeName, int usageTypeId,
                                                       string commodityGradeCode, string commodityGradeDescription);

        void SetGradeInactive(Guid commodityId, Guid gradeId);

        void SetGradeActive(Guid commodityId, Guid gradeId);

        void SetGradeAsDeleted(Guid commodityId, Guid gradeId);

        QueryResult<CommodityViewModel> Query(QueryStandard query);
        IList<CommodityViewModel> QueryList(List<Commodity> list);

      /*  QueryResult QueryGrade(QueryGrade query);*/
        IList<CommodityGradeViewModel> QueryGradeList(List<CommodityGrade> list);
    }
}
