using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CodeMonkey.Utils;

public class UI_Shop : MonoBehaviour
{
    private Transform container;
    private Transform shopItemTemplate;
    private IShopCustomer shopCustomer;

    private void Awake()
    {
        container = transform.Find("container");
        shopItemTemplate = container.Find("shopItemTemplate");
        shopItemTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        CreateItemButton(Item.ItemType.Shield, Item.GetSprite(Item.ItemType.Shield), "Shield", Item.GetCost(Item.ItemType.Shield), 0);
        CreateItemButton(Item.ItemType.Grapple, Item.GetSprite(Item.ItemType.Grapple), "Grapple", Item.GetCost(Item.ItemType.Grapple), 1);

        Hide();
    }

    private void CreateItemButton(Item.ItemType itemType, Sprite itemSprite, string itemName, int itemCost, int positionIndex)
    {
        Transform shopItemTransform = Instantiate(shopItemTemplate, container);
        shopItemTransform.gameObject.SetActive(true);
        RectTransform shopItemRectTransform = shopItemTransform.GetComponent<RectTransform>();
        
        float shopItemHeight = 120f;
        shopItemRectTransform.anchoredPosition = new Vector2(0, -shopItemHeight * positionIndex);

        shopItemTransform.Find("nameTxt").GetComponent<TextMeshProUGUI>().SetText(itemName);
        shopItemTransform.Find("costTxt").GetComponent<TextMeshProUGUI>().SetText(itemCost.ToString());

        shopItemTransform.Find("itemImg").GetComponent<Image>().sprite = itemSprite;

        shopItemTransform.GetComponent<Button_UI>().ClickFunc = () =>
        {
            BuyItem(itemType);
        };
    }

    private void BuyItem(Item.ItemType itemType)
    {
        if (shopCustomer.SpendCoins(Item.GetCost(itemType), itemType))
        {
            shopCustomer.ObtainItem(itemType);
        }
        else
        {
            Tooltip_Warning.ShowTooltip_Static("Cannot afford " + Item.GetCost(itemType) + " or already own!");
        }
    }

    public void Show(IShopCustomer shopCustomer)
    {
        this.shopCustomer = shopCustomer;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
