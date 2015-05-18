using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Repository.Master.ChannelPackagings;
using Distributr.Core.Utility.Mapping;

namespace Distributr.Integrations.Legacy.Integrations.MasterData.Impl
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
