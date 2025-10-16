using System;

namespace MiniRPG.Models
{
    public class ShopItem
    {
        public Item Item { get; set; }
        public int BuyPrice { get; set; }
        public int SellPrice { get; set; }

        public ShopItem(Item item, int buyPrice, int sellPrice)
        {
            Item = item;
            BuyPrice = buyPrice;
            SellPrice = sellPrice;
        }

        // TODO: Add stock quantity and restock timers later
    }
}