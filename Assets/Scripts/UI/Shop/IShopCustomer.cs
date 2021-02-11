using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IShopCustomer
{
    void BuyItem(int cost);
    void ObtainItem(Item.ItemType itemType);
}
