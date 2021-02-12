using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IShopCustomer
{
    bool SpendCoins(int cost);
    void ObtainItem(Item.ItemType itemType);
}
