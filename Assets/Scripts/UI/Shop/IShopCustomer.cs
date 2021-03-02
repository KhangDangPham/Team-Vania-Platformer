using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IShopCustomer
{
    bool SpendCoins(int cost, Item.ItemType itemType);
    void ObtainItem(Item.ItemType itemType);
}
