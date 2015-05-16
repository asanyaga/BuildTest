using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Utility.Validation;
using Distributr.Import.Entities;
using log4net;

namespace Distributr.WSAPI.Lib.Services.Imports.Impl
{
    public class DiscountGroupImporterService:BaseImporterService,IDiscountGroupImporterService
    {
        private readonly CokeDataContext _context;
        private readonly IDiscountGroupRepository _discountGroupRepository;
        private readonly IProductDiscountGroupRepository _productDiscountGroupRepository;
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public DiscountGroupImporterService(CokeDataContext context, IDiscountGroupRepository discountGroupRepository, IProductDiscountGroupRepository productDiscountGroupRepository)
        {
            _context = context;
            _discountGroupRepository = discountGroupRepository;
            _productDiscountGroupRepository = productDiscountGroupRepository;
        }

        public ImportResponse Save(List<DiscountGroupImport> imports)
        {
            List<DiscountGroup> discountGroups = imports.Select(Map).ToList();

            List<ValidationResultInfo> validationResults = discountGroups.Select(Validate).ToList();

            if (validationResults.Any(p => !p.IsValid))
            {
                return new ImportResponse() { Status = false, Info = ValidationResultsInfo(validationResults) };

            }
            List<DiscountGroup> changedDiscountGroups = HasChanged(discountGroups);

            foreach (var changedDiscountGroup in changedDiscountGroups)
            {
                _discountGroupRepository.Save(changedDiscountGroup);
            }
            return new ImportResponse() { Status = true, Info = changedDiscountGroups.Count + " Discount Groups Successfully Imported" };
        }

        private ValidationResultInfo Validate(DiscountGroup discountGroup)
        {
            return _discountGroupRepository.Validate(discountGroup);
        }


        private List<DiscountGroup> HasChanged(List<DiscountGroup> discountGroups)
        {
            var changedDiscountGroups = new List<DiscountGroup>();
            foreach (var discountGroup in discountGroups)
            {
                var entity = _discountGroupRepository.GetById(discountGroup.Id);
                if (entity == null)
                {
                    changedDiscountGroups.Add(discountGroup);
                    continue;
                }
                bool hasChanged = false;
                if (entity.Name.ToLower() != discountGroup.Name.ToLower() || entity.Code.ToLower() != discountGroup.Code.ToLower())
                {
                    hasChanged = true;
                }


                if (hasChanged)
                {
                    changedDiscountGroups.Add(discountGroup);
                }
            }
            return changedDiscountGroups;
        }

        public ImportResponse Delete(List<string> deletedCodes)
        {
            foreach (var deletedCode in deletedCodes)
            {
                var split = deletedCode.Split('|');
                var discountGroupCode = split[1];
                var productCode = split[0];
                try
                {
                    var discountGroupId = _context.tblDiscountGroup.Where(p => p.Code == discountGroupCode).Select(p => p.id).FirstOrDefault();
                    var produId = _context.tblProduct.Where(p => p.ProductCode == productCode).Select(p => p.id).FirstOrDefault();


                    var toDeleteDiscountGroup = _context.tblProductDiscountGroup.Where(k=>k.DiscountGroup == discountGroupId && k.ProductRef == produId).FirstOrDefault();

                    if (toDeleteDiscountGroup != null)
                    {
                        var discountGroup = _productDiscountGroupRepository.GetById(toDeleteDiscountGroup.id);
                        if (discountGroup != null)
                        {
                            _productDiscountGroupRepository.SetAsDeleted(discountGroup);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _log.Error("Discount Group Delete Error" + ex.ToString());
                }

            }
            return new ImportResponse() { Info = "Discount Group Deleted Succesfully", Status = true };
        }

        private DiscountGroup Map(DiscountGroupImport discountGroupImport)
        {
            var exists = _context.tblDiscountGroup.FirstOrDefault(p => p.Code == discountGroupImport.Code);


            Guid id = exists != null ? exists.id : Guid.NewGuid();

            var discountGroup = new DiscountGroup(id);
            
            discountGroup.Name = discountGroupImport.Name;
            discountGroup.Code = discountGroupImport.Code;

            return discountGroup;
        }
    }
}
