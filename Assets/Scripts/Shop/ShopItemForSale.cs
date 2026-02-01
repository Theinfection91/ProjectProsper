using Assets.Scripts.Item;
using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.Shop
{
    [Serializable]
    public class ShopItemForSale
    {
        public ItemData itemData;
        public int salePrice;
    }
}
