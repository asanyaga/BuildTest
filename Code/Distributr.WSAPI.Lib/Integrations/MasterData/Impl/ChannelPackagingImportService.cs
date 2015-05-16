using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Repository.Master.ChannelPackagings;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility.Mapping;

namespace Distributr.WSAPI.Lib.Integrations.MasterData.Impl
{
    public class ChannelPackagingImportService : MasterDataImportServiceBase, IChannelPackagingImportService
    {
        private readonly IDTOToEntityMapping _mappingService;
        private IChannelPackagingRepository _repository;
        private readonly CokeDataContext _ctx;
        private List<ImportValidationResultInfo> validationResultInfos;

        public ChannelPackagingImportService(IDTOToEntityMapping mappingService, IChannelPackagingRepository repository, CokeDataContext ctx)
        {
            _mappingService = mappingService;
            _repository = repository;
            _ctx = ctx;
            validationResultInfos=new List<ImportValidationResultInfo>();
        }

        public Task<MasterDataImportResponse> ValidateAsync(IEnumerable<ImportEntity> imports)
        {
            throw new NotImplementedException();
        }

        public Task<MasterDataImportResponse> ValidateAndSaveAsync(IEnumerable<ImportEntity> imports)
        {
            throw new NotImplementedException();
        }

       
    }
}
