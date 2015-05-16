using System;

namespace Distributr.WPF.Lib.Messages
{
    public class EditSupplierMessage
    {
        public Guid Id { get; set; }
         public Guid CommodityProducerId { get; set; }
         public Guid CommodityOwnerId { get; set; }
         public Guid ContactId { get; set; }
    }

    public class MemberOwnersMessage
    {
        public Guid Id { get; set; }
    }

    public class WarehouseEntryUpdateMessage
    {
        public Guid DocumentId { get; set; }
    }

    public class AddCommodityOwnerMessage
    {
        public Guid SupplierId { get; set; }
    }
    public class EditCommodityOwnerMessage
    {
        public Guid SupplierId { get; set; }
        public Guid CommodityOwnerId { get; set; }
    }

    public class MemberProducerMessage
    {
        public Guid Id { get; set; }
    }

    public class AddCommodityProducerMessage
    {                                                                                   
        public Guid SupplierId { get; set; }
    }

    public class EditCommodityProducerMessage
    {
        public Guid SupplierId { get; set; }
        public Guid CommodityProducerId { get; set; }
    }

    public class MemberContactsMessage
    {
        public Guid Id { get; set; }
    }

    public class AddContactMessage
    {
        public Guid SupplierId { get; set; }
    }

    public class EditContactMessage
    {
        public Guid SupplierId { get; set; }
        public Guid ContactId { get; set; }
    }

    public class EditInfectionsMessage
    {
        public Guid InfectionId { get; set; } 
    }

    public class EditSeasonMessage
    {
        public Guid SeasonId { get; set; }
    }



    public class EditCommodityProducerServiceMessage
    {
        public Guid ServiceId { get; set; }
    }

    public class EditShiftMessage
    {
        public Guid ShiftId { get; set; }
    }

    public class EditServiceProviderMessage
    {
        public Guid ServiceProviderId { get; set; }
    }

    public class DetailsPopUpMessage
    {
        public Guid ActivityId { get; set; }
    }

   

   
   
}
