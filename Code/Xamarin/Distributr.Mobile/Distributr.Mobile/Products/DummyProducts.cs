using System.Collections;
using System.Collections.Generic;

namespace Distributr.Mobile.Products
{
    public class DummyProducts
    {
        public static List<UIProduct> products = new List<UIProduct>()
        {
            new UIProduct
            {
                Price = 0.99,
                Name = "Coca Cola 500ml",
                StockCount = 137,
                HasReturnables = false
            },
            new UIProduct
            {
                Price = 1.75,
                Name = "Walker's Cheese Crips",
                StockCount = 664,
                HasReturnables = true
            },
            new UIProduct
            {
                Price = 0.99,
                Name = "Coca Cola 750ml",
                StockCount = 137,
                HasReturnables = true
            },
            new UIProduct
            {
                Price = 1.75,
                Name = "Walker's Cheese & Onion Crips",
                StockCount = 12,
                HasReturnables = true
            },
            new UIProduct
            {
                Price = 0.99,
                Name = "Coca Cola 1.5L",
                StockCount = 137,
                HasReturnables = true
            },
            new UIProduct
            {
                Price = 1.75,
                Name = "Walker's Salt & Vinegar Crisps",
                StockCount = 664,
                HasReturnables = true
            },
            new UIProduct
            {
                Price = 0.99,
                Name = "Tusker Lager 500ml",
                StockCount = 137
            },
            new UIProduct
            {
                Price = 1.75,
                Name = "Kingsize Mars Bar",
                StockCount = 664,
                HasReturnables = true
            },
            new UIProduct
            {
                Price = 1.75,
                Name = "Kinsize Snickers",
                StockCount = 664
            },
            new UIProduct
            {
                Price = 0.99,
                Name = "Fanta 500m",
                StockCount = 137
            },
            new UIProduct
            {
                Price = 1.75,
                Name = "Fanta 1.5L",
                StockCount = 664
            }
        };

    }

}