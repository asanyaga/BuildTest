using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.OutletVisitReasonsTypeEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.DistributorTargetEntities;
using Distributr.Core.Domain.Master.SuppliersEntities;

namespace Distributr.Core.Data.MappingExtensions
{
    public static class MapToDomain
    {
        public static ProductBrand Map(this tblProductBrand productBrand)
        {
            ProductBrand returnValue = new ProductBrand(productBrand.id)
            {
                Name = productBrand.name,
                Description = productBrand.description != null ? productBrand.description : "",
                Code = productBrand.code,
                //_Status = productBrand.active

            };
            if (productBrand.SupplierId != null)
                returnValue.Supplier = productBrand.tblSupplier.Map();
            returnValue._SetStatus((EntityStatus)productBrand.IM_Status);
            returnValue._SetDateCreated(productBrand.IM_DateCreated);
            returnValue._SetDateLastUpdated(productBrand.IM_DateLastUpdated);
            return returnValue;
        }

        public static ProductFlavour Map(this tblProductFlavour productFlavour)
        {
            ProductFlavour pf = new ProductFlavour(productFlavour.id)
            {
                Name = productFlavour.name,
                Description = productFlavour.description,
                Code = productFlavour.code,
                
                 
            };

            pf._SetStatus((EntityStatus)productFlavour.IM_Status);
            pf._SetDateCreated(productFlavour.IM_DateCreated);
            pf._SetDateLastUpdated(productFlavour.IM_DateLastUpdated);
            return pf;
        }

        public static ProductPackaging Map(this tblProductPackaging productPackaging)
        {
            ProductPackaging p = new ProductPackaging(productPackaging.Id)
            {
                Name = productPackaging.Name,
                Description = productPackaging.description,
                Code=productPackaging.code
            };
            if (productPackaging.Containment != null) {
                p.Containment = productPackaging.tblContainment.Map();
            }
            if (productPackaging.ReturnableProduct != null)
            {
                p.ReturnableProductRef = new ProductRef { ProductId =productPackaging.ReturnableProduct.Value };
            }
            p._SetStatus((EntityStatus)productPackaging.IM_Status);
            p._SetDateCreated(productPackaging.IM_DateCreated);
            p._SetDateLastUpdated(productPackaging.IM_DateLastUpdated);
            return p;
        }
      
        public static ProductPackagingType Map(this tblProductPackagingType productPackagintType)
        {
            ProductPackagingType ppt = new ProductPackagingType(productPackagintType.id)
            {
                Name = productPackagintType.name,
                Description=productPackagintType.description,
                Code=productPackagintType.code
                //_Status = productPackagintType.active
            };
            ppt._SetDateCreated(productPackagintType.IM_DateCreated);
            ppt._SetDateLastUpdated(productPackagintType.IM_DateLastUpdated);
            ppt._SetStatus((EntityStatus)productPackagintType.IM_Status);
            return ppt;
        }

        public static ProductType Map(this tblProductType productType)
        {
            ProductType pt = new ProductType(productType.id)
            {
                Name = productType.name,
                Description=productType.Description,
                 Code=productType.code
            };
            pt._SetDateCreated(productType.IM_DateCreated);
            pt._SetDateLastUpdated(productType.IM_DateLastUpdated);
            pt._SetStatus((EntityStatus)productType.IM_Status);
            return pt;
        }

        public static Region Map(this tblRegion region)
        {
            Region reg = new Region(region.id)
            {
                Name = region.Name,
                Description = region.Description,
                
            };
            reg._SetDateCreated(region.IM_DateCreated);
            reg._SetDateLastUpdated(region.IM_DateLastUpdated);
            reg._SetStatus((EntityStatus)region.IM_Status);
            return reg;
        }

        public static SocioEconomicStatus Map(this tblSocioEconomicStatus socioEconomicStatus)
        {
            SocioEconomicStatus ses = new SocioEconomicStatus(socioEconomicStatus.id)
            {
                //_Status = region.Active.Value,
                EcoStatus = socioEconomicStatus.Status
            };
            ses._SetDateCreated(socioEconomicStatus.IM_DateCreated);
            ses._SetDateLastUpdated(socioEconomicStatus.IM_DateLastUpdated);
            ses._SetStatus((EntityStatus)socioEconomicStatus.IM_Status);
            return ses;
        }

        public static ProductPricing Map(this tblPricing pricing)
        {
            ProductPricing pr = new ProductPricing(pricing.id)
                             {
                                 ProductRef =  new ProductRef{ProductId = pricing.ProductRef},
                                 Tier = pricing.tblPricingTier.Map()
                                 
                             };

            //chris ... set IsActive!!
            pr.ProductPricingItems = pricing.tblPricingItem.Select(s =>
                                                                       {
                                                                           var pi =
                                                                               new ProductPricing.ProductPricingItem(
                                                                                   s.id)
                                                                                   {
                                                                                       ExFactoryRate = s.Exfactory,
                                                                                       SellingPrice = s.SellingPrice,
                                                                                       EffectiveDate = s.EffecitiveDate,
                                                                                   };

                                                                           pi._SetStatus((EntityStatus)s.IM_Status);
                                                                           pi._SetDateCreated(s.IM_DateCreated);
                                                                           pi._SetDateLastUpdated(s.IM_DateLastUpdated);

                                                                           return pi;
                                                                       }).ToList();

                  
           
            pr._SetDateCreated(pricing.IM_DateCreated);
            pr._SetStatus((EntityStatus)pricing.IM_Status);
            pr._SetDateLastUpdated(pricing.IM_DateLastUpdated);
            

            return pr;
        }
 
        public static Country Map(this tblCountry country)
        {
            Country cntry = new Country(country.id)
            {
                Name = country.Name,
                 Code=country.Code,
                 Currency=country.Currency
            };
            cntry._SetStatus((EntityStatus)country.IM_Status);
            cntry._SetDateCreated(country.IM_DateCreated);
            cntry._SetDateLastUpdated(country.IM_DateLastUpdated);
            return cntry;
        }

        public static OutletVisitReasonsType Map(this tblOutletVisitReasonType outletVisitReasonsType)
        {
            OutletVisitReasonsType outletVrt = new OutletVisitReasonsType(outletVisitReasonsType.id)
            {
                Name = outletVisitReasonsType.Name,
                Description = outletVisitReasonsType.Description ,
                OutletVisitAction =(OutletVisitAction) outletVisitReasonsType.Action ,
             };
            outletVrt._SetStatus((EntityStatus)outletVisitReasonsType.IM_Status);
            outletVrt._SetDateCreated(outletVisitReasonsType.IM_DateCreated);
            outletVrt._SetDateLastUpdated(outletVisitReasonsType.IM_DateLastUpdated);
            return outletVrt;
        }

        public static Territory Map(this tblTerritory territory)
        {
            Territory ter = new Territory(territory.id)
            {
                
                Name = territory.Name
                
            };
            ter._SetDateCreated(territory.IM_DateCreated);
            ter._SetDateLastUpdated(territory.IM_DateLastUpdated);
            ter._SetStatus((EntityStatus)territory.IM_Status);
            return ter;
        }

        public static OutletType Map(this tblOutletType outletType)
        {
            OutletType pt = new OutletType(outletType.id)
            {
                Name = outletType.Name,
                Code = outletType.Code,
                
            };
            pt._SetDateCreated(outletType.IM_DateCreated);
            pt._SetDateLastUpdated(outletType.IM_DateLastUpdated);
            pt._SetStatus((EntityStatus)outletType.IM_Status);
            return pt;
        }
        
        public static DiscountGroup Map(this tblDiscountGroup discountGroup)
        {
            DiscountGroup dg = new DiscountGroup(discountGroup.id)
            {
                Name = discountGroup.Name,
                Code = discountGroup.Code
            };
            dg._SetDateCreated(discountGroup.IM_DateCreated);
            dg._SetDateLastUpdated(discountGroup.IM_DateLastUpdated);
            dg._SetStatus((EntityStatus)discountGroup.IM_Status);
            return dg;
        }
        
        public static OutletCategory Map(this tblOutletCategory outletCategory)
        {
            OutletCategory outletcategory = new OutletCategory(outletCategory.id)
            {
                Name = outletCategory.Name,
                Code = outletCategory.Code
            };
            outletcategory._SetDateCreated(outletCategory.IM_DateCreated);
            outletcategory._SetDateLastUpdated(outletCategory.IM_DateLastUpdated);
            outletcategory._SetStatus((EntityStatus)outletCategory.IM_Status);
            return outletcategory;
        }

        // mapping the entity ProductPricingTier
        public static ProductPricingTier Map(this tblPricingTier tier)
        {
            ProductPricingTier ppTier = new ProductPricingTier(tier.id)
            {
                Name = tier.Name,
                Code=tier.Code,
                Description=tier.Description
            };
            ppTier._SetStatus((EntityStatus)tier.IM_Status);
            ppTier._SetDateCreated(tier.IM_DateCreated);
            ppTier._SetDateLastUpdated(tier.IM_DateLastUpdated);
            return ppTier;
        }
        
        public static CostCentreRef Map(this tblCostCentre cCtr)
        {
            CostCentreRef cCentre = new CostCentreRef()
            {
                Id=cCtr.Id
            };
            return cCentre;
        }
      
        public static User Map(this tblUsers user)
        {


            User usr = new User(user.Id)
                           {
                               Username = user.UserName,
                               Password = user.Password,
                               PIN = user.PIN,
                               Mobile = user.Mobile,
                               CostCentre = user.CostCenterId,
                               UserType = (UserType) user.UserType,
                               Group = user.tblUserGroup.Map(),
 
            };
            List<UserRole> roles = new List<UserRole>();
            if(usr.Group!=null)
            {
                foreach(UserGroupRoles role in  user.tblUserGroup.tblUserGroupRoles.Where(v=>v.CanAccess).Select(s=>s.Map()).ToList())
                {
                    usr.UserRoles.Add(role.UserRole.ToString());
                }
            }
            if (user.TillNumber != null)
                usr.TillNumber =user.TillNumber;
            usr._SetDateCreated(user.IM_DateCreated);
            usr._SetDateLastUpdated(user.IM_DateLastUpdated.Value);
            usr._SetStatus((EntityStatus)user.IM_Status);
            return usr;
        }
 
        public static ClientMasterDataTracker Map(this tblClientMasterDataTracker tblTracker)
        {
            ClientMasterDataTracker tracker = new ClientMasterDataTracker(tblTracker.Id)
            {
                DateTimePushed = tblTracker.DateTimePushed,
                CostCentreApplicationId = tblTracker.CostCentreApplicationId,
                MasterDataId = tblTracker.MasterDataId,
                DateTimePushConfirmed=tblTracker.DateTimePushConfirmed!=null?tblTracker.DateTimePushConfirmed.Value:DateTime.Parse("01-jan-1900")
            };
            tracker._SetStatus((EntityStatus)tblTracker.IM_Status);
            if(tblTracker.DateTimePushConfirmed==null){
                tracker.DateTimePushConfirmed = (DateTime)tblTracker.DateTimePushConfirmed;
            }
            return tracker;
        }

        public static Area Map(this tblArea area)
        {
            Area retArea = new Area(area.id)
            {
                //_Status = region.Active.Value,
                Name = area.Name,
                Description = area.Description
            };
            retArea._SetDateCreated(area.IM_DateCreated);
            retArea._SetDateLastUpdated(area.IM_DateLastUpdated);
            retArea._SetStatus((EntityStatus)area.IM_Status);
            
            return retArea;
        }

        public static Supplier Map(this tblSupplier supp)
        {
            if (supp == null)
                return null;
            Supplier supplier = new Supplier(supp.id)
                                    {
                                        Name = supp.Name,
                                        Description = supp.Description,
                                        Code = supp.Code
                                    };
            supplier._SetDateCreated(supp.IM_DateCreated);
            supplier._SetDateLastUpdated(supp.IM_DateLastUpdated);
            supplier._SetStatus((EntityStatus) supp.IM_Status);
            return supplier;
        }

        public static Containment Map(this tblContainment containment)
        {
            Containment retContainment = new Containment(containment.id)
            {
                ProductPackagingType = containment.tblProductPackagingType.Map(),//_productPackagingTypeRepository.GetById(containment.ProductPackagingType),
                Quantity = containment.Quantity,
                //ProductRef = containment.tblProduct.Map_productRepository.GetById(containment.ReturnableProduct) as ReturnableProduct
                ProductRef = new ProductRef { ProductId = containment.ReturnableProduct },
            };
            retContainment._SetDateCreated(containment.IM_DateCreated);
            retContainment._SetDateLastUpdated(containment.IM_DateLastUpdated);
            retContainment._SetStatus((EntityStatus)containment.IM_Status);

            return retContainment;
        }
      
        //public static TargetPeriod Map (this tblTargetPeriod targetPeriod)
        //    {
        //    TargetPeriod period=new TargetPeriod(targetPeriod.Id )
        //    {
        //    Name=targetPeriod.Name ,
        //    StartDate=targetPeriod.StartDate,
        //    EndDate=targetPeriod.EndDate,
        //    };
        //    period._SetDateCreated(targetPeriod.IM_DateCreated );
        //    period._SetDateLastUpdated(targetPeriod.IM_DateLastUpdated );
        //    period._SetStatus(targetPeriod.IM_Status );

        //    return period;
        //}

        public static UserGroup Map(this tblUserGroup tblusergroup)
        {
            if (tblusergroup == null)
                return null;

            UserGroup usergroup = new UserGroup(tblusergroup.Id)
            {
                Descripition = tblusergroup.Description,
                Name = tblusergroup.Name,
            };
            usergroup._SetStatus((EntityStatus)tblusergroup.IM_Status);
            usergroup._SetDateCreated(tblusergroup.IM_DateCreated);
            usergroup._SetDateLastUpdated(tblusergroup.IM_DateLastUpdated);
            return usergroup;
        }
        
        public static UserGroupRoles Map(this tblUserGroupRoles tblusergroup)
        {
            UserGroupRoles usergroup = new UserGroupRoles(tblusergroup.Id)
            {
                UserGroup=tblusergroup.tblUserGroup.Map(),
                UserRole=tblusergroup.RoleId,
                CanAccess=tblusergroup.CanAccess
            };
            usergroup._SetStatus((EntityStatus)tblusergroup.IM_Status);
            usergroup._SetDateCreated(tblusergroup.IM_DateCreated);
            usergroup._SetDateLastUpdated(tblusergroup.IM_DateLastUpdated);
            return usergroup;
        }
        public static VATClass Map(this tblVATClass vatclass)
        {
            VATClass vat = new VATClass(vatclass.id)
            {
                Name = vatclass.Name,
                VatClass = vatclass.Class,

            };

            vat._SetDateCreated(vatclass.IM_DateCreated);
            vat._SetDateLastUpdated(vatclass.IM_DateLastUpdated);
            vat._SetStatus((EntityStatus)vatclass.IM_Status);

            var items = vatclass.tblVATClassItem;
            vat.VATClassItems = items
                                .Select(n => MapItem(n))
                                .ToList();

            return vat;
        }

        public static VATClass.VATClassItem MapItem(this tblVATClassItem vci)
        {
            var item = new VATClass.VATClassItem(vci.id)
            {
                Rate = vci.Rate,
                EffectiveDate = vci.EffectiveDate,

            };
            item._SetDateCreated(vci.IM_DateCreated);
            item._SetDateLastUpdated(vci.IM_DateLastUpdated);
            item._SetStatus((EntityStatus)vci.IM_Status);
            return item;
        }
    }
}
