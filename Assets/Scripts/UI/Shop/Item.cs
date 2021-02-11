using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public enum ItemType
    {
        Shield,
        Grapple
    }

    public static int GetCost(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Shield: return 100;
            case ItemType.Grapple: return 200;
            default: return -1;
        }
    }

    public static Sprite GetSprite(ItemType itemType)
    {
        switch (itemType)
        {
            default:
            case ItemType.Shield: return GameAssets.i.s_Shield;
            case ItemType.Grapple: return GameAssets.i.s_Grapple;
        }
    }
}
