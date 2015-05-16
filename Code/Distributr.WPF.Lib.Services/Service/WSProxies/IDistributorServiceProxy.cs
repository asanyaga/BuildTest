using Distributr.Core.Domain.Master.CentreEntity;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.EquipmentEntities;
using Distributr.Core.Domain.Master.FarmActivities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.MasterDataDTO.DTOModels.AgrimanagrDTO.CommodityDTOs;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.CostCentre;
using VCVouchers.API.Dtos;
using WarehouseReceipt.Client;
using Distributr.Core.MasterDataDTO.DataContracts;

namespace Distributr.WPF.Lib.Services.Service.WSProxies
{
    public interface IDistributorServiceProxy
    {
        Task<List<OutletItem>>  DistributorOutletListAsync(Guid distributorId);
        Task<ResponseBool> OutletAddAsync(OutletItem outletItem);
        Task<ResponseBool> OutletUpdateAsync(OutletItem outletItem);
        Task<ResponseBool> OutletsApproveAsync(List<Guid> outletIds);
        Task<ResponseBool> OutletDeactivateAsync(Guid outletId);
        Task<ResponseBool> OutletActivateAsync(Guid outletId);
        Task<ResponseBool> OutletDeleteAsync(Guid outletId);
        Task<ResponseBool> OutletPriorityAddAsync(List<OutletPriorityItem> items);
        Task<ResponseBool> OutletVisitAddAsync(List<OutletVisitDayItem> visitDays);
        Task<ResponseBool> OutletPriorityDeactivateAsync(Guid outletId);

        Task<ResponseBool> TargetAddAsync(CostCentreTargetItem targetItem);
        Task<ResponseBool> TargetUpdateAsync(CostCentreTargetItem targetItem);
        Task<ResponseBool> TargetDeactivateAsync(Guid targetId);
        Task<ResponseBool> TargetActivateAsync(Guid targetId);
        Task<ResponseBool> TargetDeleteAsync(Guid targetId);
        Task<List<RouteItem>> RouteListAsync(Guid distributorId);
        Task<ResponseBool> RouteAddAsync(RouteItem routeItem);
        Task<ResponseBool> RouteUpdateAsync(RouteItem routeItem);
        Task<ResponseBool> RouteDeactivateAsync(Guid routeId);
        Task<ResponseBool> RouteDeleteAsync(Guid routeId);
        Task<ResponseBool> RouteActivateAsync(Guid routeId);
        Task<List<UserItem>> UserListAsync(Guid distributorId);
        Task<ResponseBool> UserAddAsync(UserItem userItem);
        Task<ResponseBool> UserUpdateAsync(UserItem userItem);
        Task<ResponseBool> UserDeactivateAsync(Guid userId);
        Task<ResponseBool> UserActivateAsync(Guid userId);
        Task<ResponseBool> UserDeleteAsync(Guid userId);

        Task<List<UserItem>> AgriUserListAsync(Guid hubId);
        Task<ResponseBool> AgriUserAddAsync(UserItem user, ContactItem contact, List<RouteItem> routes );
        Task<ResponseBool> AgriUserDeactivateAsync(Guid userId);
        Task<ResponseBool> AgriUserActivateAsync(Guid userId);
        Task<ResponseBool> AgriUserDeleteAsync(Guid userId);

        Task<ResponseBool> SalesmanAddAsync(DistributorSalesmanItem distributorSalesmanItem);
        Task<ResponseBool> SalesmanUserAddAsync(DistributorSalesmanItem distributorSalesmanItem, UserItem userItem);
        Task<ResponseBool> SalesmanUpdateAsync(DistributorSalesmanItem distributorSalesmanItem);
        Task<ResponseBool> SalesmanDeactivateAsync(Guid salesmanId);
        Task<ResponseBool> SalesmanRoutesUpdateAsync(List<DistributorSalesmanRouteItem> routes);
        Task<ResponseBool> SalesmanActivateAsync(Guid salesmandId);
        Task<ResponseBool> SalesmanDeleteAsync(Guid salesmanId);
        Task<ResponseBool> SalesmanRouteDeactivateAsync(Guid salesmanRouteId);
        Task<ResponseBool> SalesmanRouteActivateAsync(Guid salesmanRouteId);
        Task<ResponseBool> SalesmanRouteDeleteAsync(Guid salesmanRouteId);
        Task<ResponseBool> PurchasingCerkAddAsync(PurchasingClerkItem purchasingClerkItem);
        Task<ResponseBool> PurchasingCerkActivateOrDeactivateAsync(Guid id);
        Task<ResponseBool> PurchasingCerkDeleteAsync(Guid id);
        Task<ResponseBool> PurchasingCerkRouteAddAsync(List<PurchasingClerkRouteItem> purchasingClerkRouteItems);
        Task<ResponseBool> PurchasingCerkRouteActivateOrDeactivateAsync(Guid id);
        Task<ResponseBool> PurchasingCerkRouteDeleteAsync(Guid id);

        Task<List<ContactItem>> ContactListAsync(Guid contactOwnerId);
        Task<ResponseBool> ContactsAddAsync(List<ContactItem> contactItems);
        Task<ResponseBool> ContactUpdateAsync(ContactItem contactItem);
        Task<ResponseBool> ContactDeactivateAsync(Guid contactId);
        Task<ResponseBool> ContactActivateAsync(Guid contactId);
        Task<ResponseBool> ContactDeleteAsync(Guid contactId);
        Task<List<BankItem>> BankListAsync();
        Task<ResponseBool> BankAddAsync(BankItem bankItem);
        Task<ResponseBool> BankUpdateAsync(BankItem bankItem);
        Task<List<BankBranchItem>> BankBranchListAsync(Guid bankId);
        Task<ResponseBool> BankBranchUpdateAsync(BankBranchItem bankBranchItem);
        Task<ResponseBool> BankBranchAddAsync(BankBranchItem bankBranchItem);

        Task<ResponseBool> CommoditySupplierAddAsync(CommoditySupplier commoditySupplier);
        Task<ResponseBool> CommoditySupplierAddAsync(CommoditySupplierDTO commoditySupplierdto);
        Task<ResponseBool> CommoditySupplierActivateOrDeactivateAsync(Guid id);
        Task<ResponseBool> CommoditySupplierDeleteAsync(Guid id);

        Task<ResponseBool> CommodityProducerAddAsync(CommodityProducerDTO commodityProducerdto);
        Task<ResponseBool> CommodityProducerAddAsync(CommodityProducer commodityProducer);
        Task<ResponseBool> CommodityProducerListAddAsync(List<CommodityProducer> commodityProducerList);
        Task<ResponseBool> CommodityProducerActivateOrDeactivateAsync(Guid id);
        Task<ResponseBool> CommodityProducerDeleteAsync(Guid id);

        Task<ResponseBool> CommodityOwnerAddAsync(CommodityOwnerDTO commodityOwnerdto);
        Task<ResponseBool> CommodityOwnerAddAsync(CommodityOwnerItem commodityOwnerItem);
        Task<ResponseBool> CommodityOwnerListAddAsync(List<CommodityOwnerItem> commodityOwnerItemList);
        Task<ResponseBool> CommodityOwnerActivateOrDeactivateAsync(Guid id);
        Task<ResponseBool> CommodityOwnerDeleteAsync(Guid id);

        Task<ResponseBool> CentreAddAsync(Centre centre);
        Task<ResponseBool> CentreActivateOrDeactivateAsync(Guid id);
        Task<ResponseBool> CentreDeleteAsync(Guid id);

        Task<ResponseBool> StoreAddAsync(Store store);
        Task<ResponseBool> StoreActivateOrDeactivateAsync(Guid id);
        Task<ResponseBool> StoreDeleteAsync(Guid id);

        Task<ResponseBool> ContainerAddAsync(SourcingContainer container);
        Task<ResponseBool> ContainerActivateOrDeactivateAsync(Guid id);
        Task<ResponseBool> ContainerDeleteAsync(Guid id);

        Task<ResponseBool> EquipmentAddAsync(Equipment equipment);
        Task<ResponseBool> EquipmentActivateOrDeactivateAsync(Guid id);
        Task<ResponseBool> EquipmentDeleteAsync(Guid id);

        Task<ResponseBool> VehicleAddAsync(Vehicle vehicle);
        Task<ResponseBool> VehicleActivateOrDeactivateAsync(Guid id);
        Task<ResponseBool> VehicleDeleteAsync(Guid id);
        Task<ResponseBool> SupplierMappingSaveAsync(CostCentreMapping mapping);

        //Task<ResponseBool> MasterDataAllocationAddAsync(List<MasterDataAllocation> masterDataAllocationList);
        //Task<ResponseBool> MasterDataAllocationDeleteAsync(List<Guid> ids);


        Task<ResponseBool> SalesmanSupplierSaveAsync(List<SalesmanSupplierDTO> salesmanSuppliers);
        Task<ResponseBool> InfectionSaveAsync(Infection infection);
        Task<ResponseBool> InfectionDeleteAsync(Guid id);

        Task<ResponseBool> SeasonSaveAsync(Season season);
        Task<ResponseBool> CommodityProducerServiceSaveAsync(CommodityProducerService commodityProducerService);
        Task<ResponseBool> ShiftSaveAsync(Shift shift);

        Task<ResponseBool> ServiceProviderSaveAsync(ServiceProvider serviceProvider);

        Task<ResponseBool> InfectionActivateOrDeactivateAsync(Guid id);
        Task<ResponseBool> SeasonActivateOrDeactivateAsync(Guid id);
        Task<ResponseBool> ServiceProviderActivateOrDeactivateAsync(Guid id);
        Task<ResponseBool> CommodityProducerServiceActivateOrDeactivateAsync(Guid id);
        Task<ResponseBool> ShiftActivateOrDeactivateAsync(Guid id);
        Task<ResponseBool> ShiftDeleteAsync(Guid id);
        Task<ResponseBool> CommodityProducerServiceDeleteAsync(Guid id);
        Task<ResponseBool> ServiceProviderDeleteAsync(Guid id);
        Task<ResponseBool> SeasonDeleteAsync(Guid id);
    }
}
