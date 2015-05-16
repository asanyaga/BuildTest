namespace Distributr.Mobile.Products
{
    public enum UnitType
    {
        Case,
        Each
    }

    public class UIProduct
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public int StockCount { get; set; }
        public long Quantity { get; set; }
        public long ReturnableQuantity { get; set; }
        public bool HasReturnables { get; set; }
        public UnitType UnitType { get; set; }

        public double TotalPrice
        {
            get { return Price*Quantity; }
        }
    }
}