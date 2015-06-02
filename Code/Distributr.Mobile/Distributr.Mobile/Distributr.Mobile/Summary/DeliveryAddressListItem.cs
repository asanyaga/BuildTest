namespace Distributr.Mobile.Summary
{
    public class DeliveryAddressListItem
    {
        public string DisplayAddress { get; set; }
        public string DeliveryAddress { get; set; }

        public override string ToString()
        {
            return DisplayAddress;
        }
    }
}