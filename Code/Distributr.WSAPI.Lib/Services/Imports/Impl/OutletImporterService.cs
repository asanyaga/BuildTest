using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Utility.Validation;
using Distributr.Import.Entities;
using log4net;

namespace Distributr.WSAPI.Lib.Services.Imports.Impl
{
    public  class OutletImporterService:BaseImporterService,IOutletImporterService
    {
        private IUserRepository _userRepository;
        private IProductPricingRepository _productPricingRepository;
        private ICostCentreRepository _costCentreRepository;
        private IProductPricingTierRepository _productPricingTierRepository;
        private IRouteRepository _routeRepository;
        private ICostCentreRepository _outletRepository;
        private IOutletTypeRepository _outletTypeRepository;
        private IOutletCategoryRepository _outletCategoryRepository;
        private IVATClassRepository _vatClassRepository;
        private IDiscountGroupRepository _discountGroupRepository;

        private readonly CokeDataContext _context;

        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public OutletImporterService(CokeDataContext context, IUserRepository userRepository, IProductPricingRepository productPricingRepository, IRouteRepository routeRepository, IOutletRepository outletRepository, IOutletTypeRepository outletTypeRepository, IOutletCategoryRepository outletCategoryRepository, IProductPricingTierRepository productPricingTierRepository, IVATClassRepository vatClassRepository, ICostCentreRepository costCentreRepository, IDiscountGroupRepository discountGroupRepository)
        {
            _context = context;
            _userRepository = userRepository;
            _productPricingRepository = productPricingRepository;
            _routeRepository = routeRepository;
            _outletRepository = outletRepository;
            _outletTypeRepository = outletTypeRepository;
            _outletCategoryRepository = outletCategoryRepository;
            _productPricingTierRepository = productPricingTierRepository;
            _vatClassRepository = vatClassRepository;
            _costCentreRepository = costCentreRepository;
            _discountGroupRepository = discountGroupRepository;
        }

        public ImportResponse Save(List<OutletImport> imports)
        {
            try
            {
                var mappingValidationList = new List<string>();
                List<Outlet> outlets = imports.Select(s=>Map(s,mappingValidationList)).ToList();
                if (mappingValidationList.Any())
                {
                    return new ImportResponse() { Status = false, Info = String.Join(",", mappingValidationList) };

                }

                List<ValidationResultInfo> validationResults = outlets.Select(Validate).ToList();
                var invalidResults = validationResults.Where(p => !p.IsValid);
                if (validationResults.Any(p => !p.IsValid))
                {
                    return new ImportResponse() {Status = false, Info = ValidationResultsInfo(validationResults)};

                }
                List<Outlet> changedOutlets = HasChanged(outlets);

                foreach (var changedOutlet in changedOutlets)
                {
                    _outletRepository.Save(changedOutlet);
                }
                return new ImportResponse() { Status = true, Info = changedOutlets.Count + " Outlets Successfully Imported" };
            }
            catch(Exception ex)
            {
                _log.Error("Saving Outlet Error" + ex.InnerException);
                return new ImportResponse() { Status = false, Info =ex.Message+"\nSaving Outlet Error - Please check Log File" };
            }
           
        }

        public ImportResponse Delete(List<string> deletedCodes)
        {

            foreach (var deletedCode in deletedCodes)
            {
                try
                {
                    var outletId = _context.tblCostCentre.Where(p => p.Name == deletedCode && p.CostCentreType == (int)CostCentreType.Outlet ).Select(p => p.Id).FirstOrDefault();

                    var outlet = _costCentreRepository.GetById(outletId);
                    if (outlet != null)
                    {
                        _costCentreRepository.SetAsDeleted(outlet);
                    }
                }
                catch (Exception ex)
                {
                    _log.Error("Outlet Delete Error" + ex.ToString());
                }

            }
            return new ImportResponse() { Info = "Outlet Deleted Succesfully", Status = true };
        }


        private List<Outlet> HasChanged(List<Outlet> outlets)
        {
            var changedOutlets = new List<Outlet>();
            foreach (var outlet in outlets)
            {
                var entity = _outletRepository.GetById(outlet.Id) as Outlet;
                if (entity == null)
                {
                    changedOutlets.Add(outlet);
                    continue;
                }
                bool hasChanged=false; 
                
                if(entity.Name.ToLower() != outlet.Name.ToLower() || entity.CostCentreCode.ToLower() != outlet.CostCentreCode.ToLower())
                {
                    hasChanged = true;
                }

                var currentRoute = outlet.Route!=null?outlet.Route.Id.ToString():"";
                var currentVATClass = outlet.VatClass!=null?outlet.VatClass.Id.ToString():"";
                var currentOutletProductPricingTier = outlet.OutletProductPricingTier != null ? outlet.OutletProductPricingTier.Id.ToString() : "";
                var currentOutletDiscountGroup = outlet.DiscountGroup != null ? outlet.DiscountGroup.Id.ToString() : "";
                var currentSpecialPricingTier = outlet.SpecialPricingTier != null ? outlet.SpecialPricingTier.Code: "";

                var previousRoute = entity.Route!=null?entity.Route.Id.ToString():"";
                var previousVATClass = entity.VatClass!=null?entity.VatClass.Id.ToString():"";
                var previousOutletProductPricingTier = entity.OutletProductPricingTier!=null?entity.OutletProductPricingTier.Id.ToString():"";
                var previousOutletDiscountGroup = entity.DiscountGroup != null ? entity.DiscountGroup.Id.ToString() : "";
                var previousSpecialPricingTier = entity.SpecialPricingTier != null ? entity.SpecialPricingTier.Code : "";

                if(currentRoute!=previousRoute || currentVATClass!=previousVATClass||currentOutletProductPricingTier!=previousOutletProductPricingTier || currentOutletDiscountGroup!=previousOutletDiscountGroup ||currentSpecialPricingTier!=previousSpecialPricingTier)
                {
                    hasChanged = true;
                }

                if (hasChanged)
                {
                    changedOutlets.Add(outlet);
                }
            }
            return changedOutlets;
        }

        protected ValidationResultInfo Validate(Outlet outlet)
        {
            var result= _outletRepository.Validate(outlet);
            return result;
        }

        protected Outlet Map(OutletImport outletImport, List<string> mappingvalidationList)
        {
            var outletCode = outletImport.Code;
            var exists = _context.tblCostCentre.FirstOrDefault(p => p.Cost_Centre_Code.ToLower().Trim() == outletImport.Code.ToLower().Trim() && p.CostCentreType == (int)CostCentreType.Outlet);
            Guid id = exists != null ? exists.Id : Guid.NewGuid();

            var routeId = _context.tblRoutes.Where(p => p.Code == outletImport.RouteCode).Select(p=>p.RouteID).FirstOrDefault();
            var vatClassId =_context.tblVATClass.Where(p => p.Name == outletImport.VATClassCode).Select(p => p.id).FirstOrDefault();
            
            var route = _routeRepository.GetById(routeId);
            if(route==null){mappingvalidationList.Add(string.Format("Invalid Route Code {0}", outletImport.RouteCode));}

            var outletTypeId = _context.tblOutletType.Where(p => p.Code == outletImport.OutletType).Select(p=>p.id).FirstOrDefault();
            var outletType = _outletTypeRepository.GetById(outletTypeId);
            if(outletType==null){mappingvalidationList.Add(string.Format("Invalid Outlet Type Code {0}",outletImport.OutletType));}

            var outletCategoryId = _context.tblOutletCategory.Where(p => p.Code == outletImport.OutletCategory).Select(p => p.id).FirstOrDefault();
            var outletCategory = _outletCategoryRepository.GetById(outletCategoryId);
            if (outletCategory == null) { mappingvalidationList.Add(string.Format("Invalid Outlet Category Code {0}", outletImport.OutletCategory)); }

           
            var outletProductPricingTier = _productPricingTierRepository.GetByCode(outletImport.PricingTierCode);
            if(outletProductPricingTier==null){mappingvalidationList.Add(string.Format("Invalid Outlet Product Pricing Tier Code {0}", outletImport.PricingTierCode));}

            var specialProductPricingTier = _productPricingTierRepository.GetByCode(outletImport.SpecialPricingTierCode);
            var VATClass = _vatClassRepository.GetById(vatClassId);
            var parentCostCentre = _costCentreRepository.GetByCode(outletImport.DistributorCode,CostCentreType.Distributor);
            var discountGroup = _discountGroupRepository.GetByCode(outletImport.DiscountGroupCode);

            if (parentCostCentre == null) { mappingvalidationList.Add(string.Format("Invalid Distributor Code {0}", outletImport.DistributorCode)); }

            var outlet = new Outlet(id);
            outlet.Name = outletImport.Name;
            outlet.CostCentreCode = outletCode;//outletImport.Code;
            outlet.Route = route;
            outlet.OutletCategory = outletCategory;
            outlet.OutletType = outletType;
            outlet.OutletProductPricingTier = outletProductPricingTier;
            outlet.SpecialPricingTier = specialProductPricingTier;
            outlet.VatClass = VATClass;
            outlet.ParentCostCentre = parentCostCentre != null ? new CostCentreRef {Id = parentCostCentre.Id} : null;
            outlet.CostCentreType=CostCentreType.Outlet;
            outlet.DiscountGroup = discountGroup;
            outlet._Status=EntityStatus.Active;
            return outlet;

        }

       


    }
}
